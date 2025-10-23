using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UploadDocumentsCommand : IRequest<ServiceResponse<DocumentDto>>
    {
        public Guid FolderId { get; set; }
        public IFormFileCollection Documents { get; set; }
    }
}
