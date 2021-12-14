using System.Text.RegularExpressions;

namespace MagicBots.Overseer.Framework.Triggers
{
    public class RegexTriggerProcessor : BaseTriggerProcessor<RegexTriggerAttribute>
    {
        public override bool Match(RegexTriggerAttribute trigger, DiscordTriggerContext context,
            TriggerableMethod method)
        {
            var content = context.Message.Content;
            return Regex.Match(content, trigger.Regex).Success;
        }
    }
}