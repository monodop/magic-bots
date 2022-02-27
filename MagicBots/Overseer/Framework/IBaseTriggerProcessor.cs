namespace MagicBots.Overseer.Framework
{
    public interface IBaseTriggerProcessor<in T> : IBaseTriggerProcessor where T : TriggerAttribute
    {
        /**
         * Returns true if the method should trigger given the context.
         */
        public bool Match(T trigger, DiscordTriggerContext context, TriggerableMethod method);
    }

    public interface IBaseTriggerProcessor
    {
        /**
         * Helper method that calls the correct extended method given a trigger.
         */
        public bool Match(TriggerAttribute trigger, DiscordTriggerContext context, TriggerableMethod method);
    }
}