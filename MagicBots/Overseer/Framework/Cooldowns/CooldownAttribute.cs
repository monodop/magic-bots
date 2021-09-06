using System;

namespace MagicBots.Overseer.Framework.Cooldowns
{
    /**
     * Base attribute for cooldown.
     */
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CooldownAttribute : Attribute
    {
    }
}