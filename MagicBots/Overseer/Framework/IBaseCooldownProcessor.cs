namespace MagicBots.Overseer.Framework
{
    /**
     * Processor to help with dynamic processor calling.
     */
    public interface IBaseCooldownProcessor<in T> : IBaseCooldownProcessor where T : CooldownAttribute
    {
        /** Returns true if the method should be allowed to proceed.*/
        public bool IsReady(T cooldown, DiscordTriggerContext context, TriggerableMethod method);
    }

    public interface IBaseCooldownProcessor
    {
        /**
         * Helper method that calls the correct extended method given a cooldown.
         */
        public bool IsReady(CooldownAttribute cooldown, DiscordTriggerContext context, TriggerableMethod method);
    }
}