using System.Threading.Tasks;

namespace Ora.Common.Mongo
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}