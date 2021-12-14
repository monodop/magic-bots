namespace MagicBots.Overseer.Framework.Triggers
{
    public class RegexTriggerAttribute : TriggerAttribute
    {
        public RegexTriggerAttribute(string regex)
        {
            Regex = regex;
        }

        public string Regex { get; }
    }
}