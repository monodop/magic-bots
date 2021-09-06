using Discord;

namespace MagicBots.Overseer.Framework
{
    public interface ITriggerContext
    {
        IDiscordClient Client { get; }
        IGuild? Guild { get; }
        IMessageChannel Channel { get; }
        IUser User { get; }
        IUserMessage Message { get; }
    }
}