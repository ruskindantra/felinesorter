using System.Collections.Generic;
using FelineSorter.WebserviceContract;

namespace FelineSorter.Components
{
    public interface IOwnerSorter
    {
        IEnumerable<OwnerAndCats> Sort(Owner[] owners);
    }
}