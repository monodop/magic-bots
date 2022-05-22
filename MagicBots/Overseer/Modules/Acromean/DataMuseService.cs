using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Modules.Acromean
{
    /**
     * Container for response from API.
     */
    public class DataMuseResult
    {
        public string? Word { get; set; }
        public int? Score { get; set; }
        public IList<string>? Tags { get; set; }
    }

    /**
     * Service that retrieves acronyms by using words from datamuse.
     */
    public class DataMuseService
    {
        private const string _apiEndpoint = "https://api.datamuse.com/words";
        private const int _maxResults = 1000;

        private readonly ILogger<DataMuseService> _logger;
        private readonly HttpClient _httpClient;

        private readonly IDictionary<string, IList<DataMuseResult>> _cache =
            new Dictionary<string, IList<DataMuseResult>>();

        private readonly Random _random = new();

        public DataMuseService(ILogger<DataMuseService> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<string> GetAcronymAsync(string word)
        {
            var selectedWords = new List<string>();

            for (var i = 0; i < word.Length; i++)
            {
                var letter = word[i];
                var wantedTypes = new HashSet<string>();

                if (i == 0)
                {
                    wantedTypes.Add("v");
                }
                else if (i < word.Length - 1)
                {
                    wantedTypes.Add("adj");
                }
                else
                {
                    wantedTypes.Add("n");
                }

                var results = await LoadAcronymsAsync(letter.ToString().ToUpper());
                if (results == null)
                {
                    throw new Exception($"No results found for letter '{letter}'");
                }
                var resultsFiltered = results
                    .Where(result => result.Tags?.Any(tag => wantedTypes.Contains(tag)) ?? false).ToList();
                int index = _random.Next(resultsFiltered.Count);
                var selectedWord = resultsFiltered[index].Word ?? "";
                selectedWords.Add(selectedWord[0].ToString().ToUpper() + selectedWord.Substring(1));
            }

            return string.Join(" ", selectedWords);
        }

        private async Task<IList<DataMuseResult>?> LoadAcronymsAsync(string letter)
        {
            if (_cache.TryGetValue(letter, out var results))
            {
                return results;
            }

            var requestUri = new Uri($"{_apiEndpoint}?sp={letter}*&md=p&max={_maxResults}");
            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = requestUri,
            });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();
            var parsedResults = JsonConvert.DeserializeObject<IList<DataMuseResult>>(result);
            _cache.Add(letter, parsedResults!);

            return parsedResults;
        }
    }
}