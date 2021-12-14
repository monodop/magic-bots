using System;
using System.Collections.Generic;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    public class TimerCooldownProcessor : BaseCooldownProcessor<TimerCooldownAttribute>
    {
        private readonly Dictionary<TriggerableMethod, DateTime> _cooldownMap = new();

        public override bool IsReady(TimerCooldownAttribute cooldown, DiscordTriggerContext context,
            TriggerableMethod method)
        {
            if (_cooldownMap.ContainsKey(method))
            {
                var allowedTime = _cooldownMap[method];
                if (DateTime.Now < allowedTime) return false;

                _cooldownMap.Remove(method);
            }

            _cooldownMap.Add(method, DateTime.Now.Add(cooldown.Timespan));

            return true;
        }
    }
}