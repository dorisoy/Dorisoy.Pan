using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Command;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DeleteAllFolderAndFileCommandHandler : IRequestHandler<DeleteAllFolderAndFileCommand, bool>
    {
        private readonly IMediator _mediator;

        public DeleteAllFolderAndFileCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<bool> Handle(DeleteAllFolderAndFileCommand request, CancellationToken cancellationToken)
        {
            var getAllDeletedDocumentQuery = new GetAllDeletedDocumentQuery() { };
            var documents = await _mediator.Send(getAllDeletedDocumentQuery);

            var getAllDeletedFolderQuery = new GetAllDeletedFolderQuery() { };
            var folders = await _mediator.Send(getAllDeletedFolderQuery);

            foreach(var document in documents)
            {
                var restoreDeletedFolderCommand = new DeleteDeletedDocumentCommand() { Id = document.Id };
                var result = await _mediator.Send(restoreDeletedFolderCommand);
            }
            foreach (var folder in folders)
            {
                var deleteDeletedFolderCommand = new DeleteDeletedFolderCommand() { Id = folder.Id };
                var result = await _mediator.Send(deleteDeletedFolderCommand);
            }
            return true;
        }
    }
}
