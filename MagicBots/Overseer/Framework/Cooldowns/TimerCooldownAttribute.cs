using System;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    public class TimerCooldownAttribute : CooldownAttribute
    {
        public TimerCooldownAttribute(float seconds)
        {
            Timespan = TimeSpan.FromSeconds(seconds);
        }

        public TimeSpan Timespan { get; }
    }
}