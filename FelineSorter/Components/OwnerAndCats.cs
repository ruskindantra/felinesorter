using System.Linq;
using System.Text;
using FelineSorter.WebserviceContract;
using RuskinDantra.Extensions;

namespace FelineSorter.Components
{
    public class OwnerAndCats
    {
        public readonly string Gender;
        public readonly IOrderedEnumerable<Pet> Cats;
        
        public OwnerAndCats(string gender, IOrderedEnumerable<Pet> cats)
        {
            gender.ThrowIfArgumentNull(nameof(gender));
            cats.ThrowIfArgumentNull(nameof(cats));

            Gender = gender;
            Cats = cats;
        }

        public override string ToString()
        {
            // TODO: Needs unit test

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Gender);
            stringBuilder.AppendLine();
            foreach (var cat in Cats)
            {
                stringBuilder.AppendLine($"-\t{cat.Name}");
            }
            return stringBuilder.ToString();
        }
    }
}