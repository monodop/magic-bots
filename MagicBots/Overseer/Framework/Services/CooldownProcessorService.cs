using MagicBots.Overseer.Framework.Cooldowns;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MagicBots.Overseer.Framework.Services
{
    /**
     * Service that processes cooldowns.
     */
    public class CooldownProcessorService : IBaseCooldownProcessor<CooldownAttribute>
    {
        private readonly Container _container;
        private readonly ILogger _logger;

        /**
         * Dictionary of CooldownAttribute type to CooldownProcessorService.
         */
        private Dictionary<Type, Type> _moduleMapping = new();

        public CooldownProcessorService(ILogger logger, Container container)
        {
            _logger = logger;
            _container = container;
            PopulateProcessors();
        }

        public bool IsReady(CooldownAttribute cooldown, DiscordTriggerContext context, TriggerableMethod method)
        {
            var cooldownProcessor = GetCooldownProcessor(cooldown);

            var isReady = cooldownProcessor.IsReady(cooldown, context, method);

            if (!isReady)
                _logger.LogInformation(
                    $"Cooldown '{cooldownProcessor.GetType().FullName}' prevented '{method.Type.FullName}#{method.Method.Name}' from processing");

            return isReady;
        }

        /**
         * Populates cooldown map.
         */
        private void PopulateProcessors()
        {
            var modules = Assembly.GetEntryAssembly()!.DefinedTypes
                .Where(ModuleHelper.IsLoadableCooldown)
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => type != typeof(CooldownProcessorService))
                .Select(x => x.AsType());

            // Determine the Attribute/Processor mapping
            var extractedMapping = modules
                .SelectMany(module => module.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseCooldownProcessor<>))
                    .Select(i => (module, attribute: i.GetGenericArguments()[0])));

            _moduleMapping = extractedMapping
                .ToDictionary(x => x.attribute, x => x.module);
        }

        private IBaseCooldownProcessor GetCooldownProcessor(CooldownAttribute attribute)
        {
            var processorType = _moduleMapping[attribute.GetType()];
            return (IBaseCooldownProcessor) _container.GetInstance(processorType);
        }
    }
}