using MediatR;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class GetTotalSizeOfFilesQuery : IRequest<long>
    {
    }
}
