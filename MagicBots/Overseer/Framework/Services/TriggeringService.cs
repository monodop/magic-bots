using Microsoft.Extensions.Logging;
using SimpleInjector;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Services
{
    /**
     * Service responsible for processing Discord signals and firing correct trigger
     * and cooldown configurations.
     */
    public class TriggeringService
    {
        private readonly Container _container;
        private readonly CooldownProcessorService _cooldownProcessorService;
        private readonly ILogger<TriggeringService> _logger;
        private readonly TriggerProcessorService _triggerProcessorService;
        private readonly List<TriggerableMethod> _triggers = new();

        public TriggeringService(Container container,
            ILogger<TriggeringService> logger,
            TriggerProcessorService triggerProcessorService,
            CooldownProcessorService cooldownProcessorService)
        {
            _container = container;
            _logger = logger;
            _triggerProcessorService = triggerProcessorService;
            _cooldownProcessorService = cooldownProcessorService;

            PopulateModules();
        }

        /**
         * Populates trigger list by searching the assembly for any classes that use
         * at least one TriggerAttribute method annotation.
         */
        private void PopulateModules()
        {
            var foundTriggers = Assembly.GetEntryAssembly()!.DefinedTypes
                .Where(ModuleHelper.IsLoadableModule)
                .SelectMany(type => type.DeclaredMethods.Select(method => (type, method)))
                .Where(x => ModuleHelper.IsTriggerableMethod(x.method))
                .Select(x => new TriggerableMethod
                {
                    Type = x.type,
                    Method = x.method,
                    TriggerAttributes = x.method.GetCustomAttributes<TriggerAttribute>().ToList(),
                    CooldownAttributes = x.method.GetCustomAttributes<CooldownAttribute>().ToList()
                }).ToArray();
            _triggers.Clear();
            _triggers.AddRange(foundTriggers);
        }

        /**
         * Processes a single message, iterating over all triggers and checking if they apply
         * and are not in cooldown.
         */
        public async Task ProcessMessage(DiscordTriggerContext context)
        {
            var tasks = new List<Task>();
            foreach (var trigger in _triggers)
            {
                if (!trigger.TriggerAttributes.Any(attribute =>
                    _triggerProcessorService.Match(attribute, context, trigger)))
                    continue;
                if (!trigger.CooldownAttributes.All(
                    attribute => _cooldownProcessorService.IsReady(attribute, context, trigger)))
                    continue;
                tasks.Add(InvokeTrigger(trigger, context));
            }
            await Task.WhenAll(tasks);
        }

        /**
         * Invokes the trigger using the context.
         */
        private Task InvokeTrigger(TriggerableMethod trigger, DiscordTriggerContext context)
        {
            var service = _container.GetInstance(trigger.Type);
            var method = trigger.Method;
            var fullName = $"{method.ReflectedType!.FullName}#{method.Name}";
            _logger.LogInformation($"Triggering '{fullName}'");
            return Task.Run(async () => await (Task)method.Invoke(service, new object[] {context})!);
        }
    }
}