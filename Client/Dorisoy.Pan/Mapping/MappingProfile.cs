using Role = Dorisoy.Pan.Data.Role;

namespace Dorisoy.Pan.Mapping;

public class MappingProfile : Profile
{
    //Create using CreateMap<User, RoleModel>
    public MappingProfile()
    {
        CreateMap<User, UserModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.Ignore());

        //RoleClaim
        CreateMap<Role, RoleModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<RoleClaim, RoleClaimModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<UserClaim, UserClaimModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, src => src.Ignore());
        CreateMap<UserClaimModel, UserClaim>()
            .ReverseMap();

        CreateMap<Operate, ActionModel>()
         .ReverseMap()
         .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<Page, PageModel>()
         .ReverseMap()
         .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<Document, DocumentModel>()
           .ReverseMap()
           .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<Department, DepartmentModel>()
           .ReverseMap()
           .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<Patient, PatientModel>()
         .ReverseMap()
         .ForMember(dest => dest.Id, src => src.Ignore());

        CreateMap<RoleModel, Role>().ReverseMap();

        CreateMap<RoleClaimResponse, RoleClaim>()
           .ForMember(nameof(RoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
           .ForMember(nameof(RoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
           .ReverseMap();


        CreateMap<PhysicalFolder, PhysicalFolderModel>().ReverseMap()
            .ForMember(dest => dest.Parent, src => src.Ignore())
            .ForMember(dest => dest.Children, src => src.Ignore());


        CreateMap<PhysicalFolderModel, PhysicalFolder>()
            .ReverseMap();

        CreateMap<VirtualFolder, VirtualFolderModel>().ReverseMap()
            .ForMember(dest => dest.Parent, src => src.Ignore())
            .ForMember(dest => dest.PhysicalFolder, src => src.Ignore())
            .ForMember(dest => dest.Children, src => src.Ignore())
            .ForMember(dest => dest.VirtualFolderUsers, src => src.Ignore());


        CreateMap<VirtualFolderModel, VirtualFolder>().ReverseMap()
            .ForMember(dest => dest.Parent, src => src.Ignore())
            .ForMember(dest => dest.PhysicalFolder, src => src.Ignore())
            .ForMember(dest => dest.Children, src => src.Ignore())
            .ForMember(dest => dest.VirtualFolderUsers, src => src.Ignore());


        CreateMap<PhysicalFolderUser, PhysicalFolderUserModel>()
            .ReverseMap();

        CreateMap<VirtualFolderUser, VirtualFolderUserModel>()
            .ReverseMap();

        CreateMap<AddFolderModel, VirtualFolder>()
            .ReverseMap();

        CreateMap<DocumentFolderModel, DocumentModel>()
            .ReverseMap();

        CreateMap<DocumentFolderModel, VirtualFolderInfoModel>()
            .ReverseMap();

        CreateMap<DocumentComment, DocumentCommentModel>()
           .ReverseMap();

        CreateMap<UserDto, OnlinUserUserModel>()
            .ReverseMap();

        CreateMap<UserDto, UserModel>()
         .ReverseMap();

        CreateMap<RoomsDto, Rooms>()
           .ReverseMap();

        CreateMap<ConnectionsDto, Connections>()
        .ReverseMap();

        CreateMap<RoleClaimResponseModel, RoleClaimResponse>()
            .ReverseMap();

    }
}
