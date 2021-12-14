using Discord;
using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Cooldowns;
using MagicBots.Overseer.Framework.Discord;
using MagicBots.Overseer.Framework.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Modules
{
    public class RandomojiModule : OverseerModule
    {
        private const string ConfigSection = "Randomoji:Builtin";
        private readonly Random _random = new();
        private IEnumerable<Emoji> _builtinReacts = new List<Emoji>();

        public RandomojiModule(OverseerHostedService overseer,
            ILogger<RandomojiModule> logger, IConfiguration config) :
            base(overseer.Client!, logger, config)
        {
            BuildBuiltinReactsList();
        }

        private void BuildBuiltinReactsList()
        {
            var foundConfig = GetFromConfig<List<string>>(ConfigSection);
            if (foundConfig == null)
                return;
            _builtinReacts = foundConfig.Select(emojiString => new Emoji(emojiString));
        }

        private IEmote SelectRandomEmote(IEnumerable<IEmote> guildEmotes)
        {
            var enumerable = guildEmotes as IEmote[] ?? guildEmotes.ToArray();
            var index = _random.Next(0, enumerable.Length + _builtinReacts.Count());
            if (index < _builtinReacts.Count())
                return _builtinReacts.ElementAt(index);
            index -= _builtinReacts.Count();
            return enumerable.ElementAt(index);
        }

        [AlwaysTrigger]
        [ChanceCooldown(0.1f)]
        public async Task RandomojiAsync(DiscordTriggerContext context)
        {
            await Discord.SetActivityAsync(new Game("Testing Things"));
            await context.Message.AddReactionAsync(
                SelectRandomEmote(context.Guild!.Emotes));
        }
    }
}