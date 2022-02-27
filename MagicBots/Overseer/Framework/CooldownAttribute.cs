using System;

namespace MagicBots.Overseer.Framework
{
    /**
     * Base attribute for cooldown.
     */
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CooldownAttribute : Attribute
    {
    }
}