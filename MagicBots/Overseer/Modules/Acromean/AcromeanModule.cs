using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Discord;
using MagicBots.Overseer.Framework.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Modules.Acromean
{
    public class AcromeanModule : OverseerModule
    {
        public static IList<Type> GetServices() => new List<Type> { typeof(DataMuseService) };
        
        private readonly DataMuseService _dataMuseService;
        
        public AcromeanModule(OverseerDiscordService overseer,
            ILogger<AcromeanModule> logger, IConfiguration config,
            DataMuseService dataMuseService) : base(overseer.Client!,
            logger, config)
        {
            _dataMuseService = dataMuseService;
        }
        
        [RegexTrigger("[A-Z]+")]
        public async Task AcromeanAsync(DiscordTriggerContext context)
        {
            var pattern = @"([A-Z]+)";
            var matches = Regex.Matches(context.Message.Content, pattern);
            if (matches.Count == 0)
            {
                return;
            }
            
            var match = matches[0];
            
            var acronym = await _dataMuseService.GetAcronymAsync(match.Value);
            
            await context.Channel.SendMessageAsync($"({acronym})");
        }
    }
}