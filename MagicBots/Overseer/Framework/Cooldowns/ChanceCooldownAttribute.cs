using System;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    public class ChanceCooldownAttribute : CooldownAttribute
    {
        public ChanceCooldownAttribute(double percent)
        {
            if (percent is < 0f or > 1f)
                throw new ArgumentException("Percent must be between 0.0 and 1.0");

            Percent = percent;
        }

        public double Percent { get; }
    }
}