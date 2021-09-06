using System;

namespace MagicBots.Overseer.Framework.Triggers
{
    /**
     * Base attribute for trigger.
     */
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class TriggerAttribute : Attribute
    {
    }
}