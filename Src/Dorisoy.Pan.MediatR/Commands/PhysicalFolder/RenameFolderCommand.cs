using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class RenameFolderCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
