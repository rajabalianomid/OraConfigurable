using Ora.Common.Commands;
using System.Threading.Tasks;

namespace Ora.Common.CommandsHandler
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task HandleAsync(T command);
    }
}