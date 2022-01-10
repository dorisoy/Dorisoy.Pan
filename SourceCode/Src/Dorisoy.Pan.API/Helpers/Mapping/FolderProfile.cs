using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class FolderProfile : Profile
    {
        public FolderProfile()
        {
            CreateMap<PhysicalFolder, PhysicalFolderDto>().ReverseMap();
            CreateMap<VirtualFolder, VirtualFolderDto>().ReverseMap(); 
            CreateMap<PhysicalFolderUser, PhysicalFolderUserDto>().ReverseMap();
            CreateMap<VirtualFolderUser, VirtualFolderUserDto>().ReverseMap();
            CreateMap<AddFolderCommand, VirtualFolder>();
        }
    }
}
