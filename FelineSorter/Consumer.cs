using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace FelineSorter
{
    [UsedImplicitly]
    internal class Consumer : IConsumer
    {
        private readonly ILogger<Consumer> _logger;

        public Consumer(ILogger<Consumer> logger)
        {
            _logger = logger;
        }
        public void Consume()
        {
            _logger.LogInformation("Consuming");
        }
    }
}