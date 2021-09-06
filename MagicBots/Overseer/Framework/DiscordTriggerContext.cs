using Discord;
using Discord.WebSocket;

namespace MagicBots.Overseer.Framework
{
    public class DiscordTriggerContext : ITriggerContext
    {
        public DiscordTriggerContext(DiscordSocketClient client, SocketUserMessage msg)
        {
            Client = client;
            Guild = msg.Channel is SocketGuildChannel channel ? channel.Guild : null;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }

        public DiscordSocketClient Client { get; }
        public SocketGuild? Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketUser User { get; }
        public SocketUserMessage Message { get; }
        public bool IsPrivate => Channel is IPrivateChannel;

        IDiscordClient ITriggerContext.Client => Client;
        IGuild? ITriggerContext.Guild => Guild;
        IMessageChannel ITriggerContext.Channel => Channel;
        IUser ITriggerContext.User => User;
        IUserMessage ITriggerContext.Message => Message;
    }
}