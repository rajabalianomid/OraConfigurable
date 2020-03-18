using System.Threading.Tasks;

namespace Ora.Common.Mongo
{
    public interface IDatabaseSeeder
    {
         Task SeedAsync();
    }
}