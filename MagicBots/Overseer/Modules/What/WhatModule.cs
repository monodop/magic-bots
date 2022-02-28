using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Discord;
using MagicBots.Overseer.Framework.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Modules.What
{
    public class WhatModule : OverseerModule
    {
        public WhatModule(OverseerDiscordService overseer,
            ILogger<WhatModule> logger, IConfiguration config) : base(overseer.Client!,
            logger, config)
        {
        }

        [RegexTrigger("^WHAT$")]
        public async Task WhatAsync(DiscordTriggerContext context)
        {
            var cache = context.Channel.GetCachedMessages();
            var lastMessage =
                cache.ElementAtOrDefault(1)?.Content.ToUpper() ?? "";
            if (lastMessage != "")
                await context.Channel.SendMessageAsync(lastMessage);
        }
    }
}