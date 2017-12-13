using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FelineSorter.Components;
using FelineSorter.WebserviceContract;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuskinDantra.Extensions;

namespace FelineSorter
{
    [UsedImplicitly]
    internal class Consumer : IConsumer
    {
        private readonly ILogger<Consumer> _logger;
        private readonly WebserviceOptions _webserviceOptions;
        private readonly Func<Uri, IHttpClient> _httpClientFactory;
        private readonly IOwnerSorter _felineOwnerSorter;
        private readonly IConsoleWriter _consoleWriter;

        public Consumer(ILogger<Consumer> logger, WebserviceOptions webserviceOptions, Func<Uri, IHttpClient> httpClientFactory, IOwnerSorter felineOwnerSorter, IConsoleWriter consoleWriter)
        {
            logger.ThrowIfArgumentNull(nameof(logger));
            webserviceOptions.ThrowIfArgumentNull(nameof(webserviceOptions));
            httpClientFactory.ThrowIfArgumentNull(nameof(httpClientFactory));
            felineOwnerSorter.ThrowIfArgumentNull(nameof(felineOwnerSorter));
            consoleWriter.ThrowIfArgumentNull(nameof(consoleWriter));

            _logger = logger;
            _webserviceOptions = webserviceOptions;
            _httpClientFactory = httpClientFactory;
            _felineOwnerSorter = felineOwnerSorter;
            _consoleWriter = consoleWriter;
        }
        public async Task<bool> Consume()
        {
            try
            {
                _logger.LogInformation($"Consuming data from endpoint: <{_webserviceOptions.PeopleEndpoint}>");
                using (var httpClient = _httpClientFactory(new Uri(_webserviceOptions.EndpointBaseUrl)))
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_webserviceOptions.PeopleEndpoint);

                    if (httpResponseMessage == null)
                    {
                        _logger.LogError("Response message from endpoint was invalid");
                        return false;
                    }

                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Response message from endpoint had an invalid Http status code of <{httpResponseMessage.StatusCode}>");
                        return false;
                    }

                    if (httpResponseMessage.Content == null)
                    {
                        _logger.LogError("Response message from endpoint has null content");
                        return false;
                    }

                    _logger.LogInformation("Response from endpoint looks good, working on it");

                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    var owners = JsonConvert.DeserializeObject<Owner[]>(content);

                    IEnumerable<OwnerAndCats> ownerAndCatsCollection = _felineOwnerSorter.Sort(owners);
                    foreach (var ownerAndCats in ownerAndCatsCollection)
                    {
                        _consoleWriter.Write(ownerAndCats); 
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred while consuming data from endpoint");
                return false;
            }
        }
    }
}