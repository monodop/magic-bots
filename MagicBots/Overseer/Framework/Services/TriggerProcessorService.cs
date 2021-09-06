using MagicBots.Overseer.Framework.Triggers;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MagicBots.Overseer.Framework.Services
{
    /**
     * Service that processes triggers.
     */
    public class TriggerProcessorService : IBaseTriggerProcessor<TriggerAttribute>
    {
        private readonly Container _container;
        private readonly ILogger _logger;

        /**
         * Dictionary of TriggerAttribute type to TriggerProcessorService.
         */
        private Dictionary<Type, Type> _moduleMapping = new();

        public TriggerProcessorService(Container container, ILogger logger)
        {
            _logger = logger;
            _container = container;
            PopulateProcessors();
        }

        public bool Match(TriggerAttribute trigger, DiscordTriggerContext context, TriggerableMethod method)
        {
            var triggerProcessor = GetTriggerProcessor(trigger);

            return triggerProcessor.Match(trigger, context, method);
        }

        /**
         * Populates trigger map.
         */
        private void PopulateProcessors()
        {
            var modules = Assembly.GetEntryAssembly()!.DefinedTypes
                .Where(ModuleHelper.IsLoadableTrigger)
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => type != typeof(TriggerProcessorService))
                .Select(x => x.AsType());

            // Determine the Attribute/Processor mapping
            var extractedMapping = modules
                .SelectMany(module => module.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseTriggerProcessor<>))
                    .Select(i => (module, attribute: i.GetGenericArguments()[0])));

            _moduleMapping = extractedMapping
                .ToDictionary(x => x.attribute, x => x.module);
        }

        private IBaseTriggerProcessor GetTriggerProcessor(TriggerAttribute attribute)
        {
            var processorType = _moduleMapping[attribute.GetType()];
            return (IBaseTriggerProcessor) _container.GetInstance(processorType);
        }
    }
}