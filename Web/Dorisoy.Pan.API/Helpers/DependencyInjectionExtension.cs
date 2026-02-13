using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Dorisoy.Pan.API.Helpers
{
	public static class DependencyInjectionExtension
	{
		public static void AddDependencyInjection(this IServiceCollection services)
		{
			services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
			services.AddScoped<IPropertyMappingService, PropertyMappingService>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ILoginAuditRepository, LoginAuditRepository>();
			services.AddScoped<INLogRespository, NLogRespository>();
			services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
			services.AddScoped<IEmailSMTPSettingRepository, EmailSMTPSettingRepository>();
			services.AddScoped<IPhysicalFolderRepository, PhysicalFolderRepository>();
			services.AddScoped<IPhysicalFolderUserRepository, PhysicalFolderUserRepository>();
			services.AddScoped<IDocumentAuditTrailRepository, DocumentAuditTrailRepository>();
			services.AddScoped<IDocumentCommentRepository, DocumentCommentRepository>();
			services.AddScoped<IDocumentReminderRepository, DocumentReminderRepository>();
			services.AddScoped<IDocumentRepository, DocumentRepository>();
			services.AddScoped<IDocumentStarredRepository, DocumentStarredRepository>();
			services.AddScoped<IDocumentUserPermissionRepository, DocumentUserPermissionRepository>();
			services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();
			services.AddScoped<IVirtualFolderRepository, VirtualFolderRepository>();
			services.AddScoped<IVirtualFolderUserRepository, VirtualFolderUserRepository>();
			services.AddScoped<IDocumentSharedUserRepository, DocumentSharedUserRepository>();
			services.AddScoped<IDocumentDeletedRepository, DocumentDeletedRepository>();
			services.AddScoped<IDocumentTokenRepository, DocumentTokenRepository>();
			services.AddScoped<IRecentActivityRepository, RecentActivityRepository>();
			services.AddScoped<IDocumentShareableLinkRepository, DocumentShareableLinkRepository>();
			services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

		}
	}
}
