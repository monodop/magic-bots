using System;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    public class ChanceCooldownProcessor : BaseCooldownProcessor<ChanceCooldownAttribute>
    {
        private readonly Random _random = new();

        public override bool IsReady(ChanceCooldownAttribute cooldown, DiscordTriggerContext context,
            TriggerableMethod method)
        {
            var randVal = _random.NextDouble();
            return randVal < cooldown.Percent;
        }
    }
}