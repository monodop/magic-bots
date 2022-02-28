using Discord;
using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Cooldowns;
using MagicBots.Overseer.Framework.Discord;
using MagicBots.Overseer.Framework.Services;
using MagicBots.Overseer.Framework.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Modules.Randomoji
{
    public class RandomojiModule : OverseerModule
    {
        private readonly FirestoreService _firestoreService;
        private const string ConfigSection = "Randomoji:Builtin";
        private readonly Random _random = new();
        private IEnumerable<Emoji> _builtinReacts = new List<Emoji>();

        public RandomojiModule(OverseerDiscordService overseer,
            ILogger<RandomojiModule> logger, IConfiguration config,
            FirestoreService firestoreService) :
            base(overseer.Client!, logger, config)
        {
            _firestoreService = firestoreService;
            BuildBuiltinReactsList();
        }

        private void BuildBuiltinReactsList()
        {
            var foundConfig = Config.GetFromConfig<List<string>>(ConfigSection, Logger);
            if (foundConfig == null)
                return;
            _builtinReacts = foundConfig.Select(emojiString => new Emoji(emojiString));
        }

        private async Task WriteReacts(DiscordTriggerContext context, IEnumerable<IEmote> guildEmotes)
        {
            var collection = _firestoreService.GetGuildScopedCollection(context, "Randomoji");
            var document = collection.Document("FoundReacts");
            var data = new Dictionary<string, object>
            {
                ["Reacts"] = guildEmotes.Select(emoji => emoji.ToString()).ToList()
            };

            await document.SetAsync(data);
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
            await WriteReacts(context, context.Guild!.Emotes);
            await context.Message.AddReactionAsync(
                SelectRandomEmote(context.Guild!.Emotes));
        }
    }
}