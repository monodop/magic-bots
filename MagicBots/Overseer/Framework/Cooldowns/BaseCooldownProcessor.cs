using System;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    /**
     * Base processor to help with dynamic processor calling.
     */
    public abstract class BaseCooldownProcessor<T> : IBaseCooldownProcessor<T> where T : CooldownAttribute
    {
        public abstract bool IsReady(T cooldown, DiscordTriggerContext context, TriggerableMethod method);

        public virtual bool IsReady(CooldownAttribute cooldown, DiscordTriggerContext context, TriggerableMethod method)
        {
            if (cooldown is T attribute)
                return IsReady(attribute, context, method);

            throw new InvalidCastException($"Cannot cast '{cooldown.GetType()}' to '{typeof(T)}'");
        }
    }
}