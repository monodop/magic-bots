namespace MagicBots.Overseer.Framework.Triggers
{
    public class AlwaysTriggerProcessor : BaseTriggerProcessor<AlwaysTriggerAttribute>
    {
        public override bool Match(AlwaysTriggerAttribute trigger, DiscordTriggerContext context,
            TriggerableMethod method)
        {
            return true;
        }
    }
}