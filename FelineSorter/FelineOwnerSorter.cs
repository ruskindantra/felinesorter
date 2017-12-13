using System;
using System.Linq;
using System.Text;
using FelineSorter.WebserviceContract;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RuskinDantra.Extensions;

namespace FelineSorter
{
    [UsedImplicitly]
    internal class FelineOwnerSorter : IOwnerSorter
    {
        private readonly ILogger<FelineOwnerSorter> _logger;

        public FelineOwnerSorter(ILogger<FelineOwnerSorter> logger)
        {
            logger.ThrowIfNull(nameof(logger));

            _logger = logger;
        }

        public void Sort(Owner[] owners)
        {
            var stringBuilder = new StringBuilder();
            var groupedByGender = owners.GroupBy(o => o.Gender).ToList();

            _logger.LogInformation($"Found <{groupedByGender.Count}> distinct groups for 'Gender'");

            foreach (var group in groupedByGender)
            {
                var pets = group.Where(g => g.Pets != null).SelectMany(g => g.Pets).ToList();
                _logger.LogInformation($"Found <{pets.Count}> pets in group <{group.Key}>");

                var cats = pets.Where(p => string.Compare(p.Type, "cat", StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
                _logger.LogInformation($"Found <{cats.Count}> cats in group <{group.Key}>");
                
                stringBuilder.AppendLine(group.Key);
                stringBuilder.AppendLine();
                foreach (var cat in cats.OrderBy(p => p.Name))
                {
                    stringBuilder.AppendLine($"-\t{cat.Name}");
                }
                stringBuilder.AppendLine();
            }
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}