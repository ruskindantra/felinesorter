using System;
using System.Collections.Generic;
using System.Linq;
using FelineSorter.WebserviceContract;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RuskinDantra.Extensions;

namespace FelineSorter.Components
{
    [UsedImplicitly]
    internal class FelineOwnerSorter : IOwnerSorter
    {
        private readonly ILogger<FelineOwnerSorter> _logger;

        public FelineOwnerSorter(ILogger<FelineOwnerSorter> logger)
        {
            logger.ThrowIfArgumentNull(nameof(logger));

            _logger = logger;
        }

        public IEnumerable<OwnerAndCats> Sort(Owner[] owners)
        {
            var groupedByGender = owners.GroupBy(o => o.Gender.ToUpperInvariant()).ToList();

            _logger.LogInformation($"Found <{groupedByGender.Count}> distinct groups for 'Gender'");

            var ownerAndCats = new List<OwnerAndCats>();

            foreach (var group in groupedByGender)
            {
                var pets = group.Where(g => g.Pets != null).SelectMany(g => g.Pets).ToList();
                _logger.LogInformation($"Found <{pets.Count}> pets in group <{group.Key}>");

                var cats = pets.Where(p => string.Compare(p.Type, "cat", StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
                _logger.LogInformation($"Found <{cats.Count}> cats in group <{group.Key}>");

                var sortedCats = cats.Where(p => !string.IsNullOrWhiteSpace(p.Name)).OrderBy(p => p.Name);
                ownerAndCats.Add(new OwnerAndCats(group.Key, sortedCats));
            }

            return ownerAndCats;
        }
    }
}