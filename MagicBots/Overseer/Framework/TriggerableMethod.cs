using MagicBots.Overseer.Framework.Cooldowns;
using MagicBots.Overseer.Framework.Triggers;
using System.Collections.Generic;
using System.Reflection;

namespace MagicBots.Overseer.Framework
{
    public class TriggerableMethod
    {
        public List<CooldownAttribute> CooldownAttributes;
        public MethodInfo Method;
        public List<TriggerAttribute> TriggerAttributes;
        public TypeInfo Type;

        public TriggerableMethod()
        {
            TriggerAttributes = new List<TriggerAttribute>();
            CooldownAttributes = new List<CooldownAttribute>();
            Type = null!;
            Method = null!;
        }
    }
}