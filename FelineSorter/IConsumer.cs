using System.Threading.Tasks;

namespace FelineSorter
{
    public interface IConsumer
    {
        Task<bool> Consume();
    }
}