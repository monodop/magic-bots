using System;

namespace MagicBots.Overseer.Framework
{
    /**
     * Base processor to help with dynamic processor calling.
     */
    public abstract class BaseTriggerProcessor<T> : IBaseTriggerProcessor<T> where T : TriggerAttribute
    {
        public abstract bool Match(T trigger, DiscordTriggerContext context, TriggerableMethod method);

        public virtual bool Match(TriggerAttribute trigger, DiscordTriggerContext context, TriggerableMethod method)
        {
            if (trigger is T attribute)
                return Match(attribute, context, method);

            throw new InvalidCastException($"Cannot cast '{trigger.GetType()}' to '{typeof(T)}'");
        }
    }
}