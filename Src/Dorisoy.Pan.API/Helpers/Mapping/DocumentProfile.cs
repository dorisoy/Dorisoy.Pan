using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<DocumentToken, DocumentTokenDto>().ReverseMap();
            CreateMap<DocumentShareableLink, DocumentShareableLinkDto>().ReverseMap();
            CreateMap<CreateDocumentShareableLinkCommand, DocumentShareableLink>();
        }
    }
}
