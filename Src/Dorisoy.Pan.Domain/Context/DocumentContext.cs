using Dorisoy.Pan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Dorisoy.Pan.Domain
{
    public class DocumentContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DocumentContext(DbContextOptions options) : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<NLog> NLog { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailSMTPSetting> EmailSMTPSettings { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentComment> DocumentComments { get; set; }
        public DbSet<DocumentReminder> DocumentReminders { get; set; }
        public DbSet<DocumentStarred> DocumentStarreds { get; set; }
        public DbSet<DocumentUserPermission> DocumentUserPermissions { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentAuditTrail> DocumentAuditTrails { get; set; }
        public DbSet<PhysicalFolder> PhysicalFolders { get; set; }
        public DbSet<PhysicalFolderUser> PhysicalFolderUsers { get; set; }
        public DbSet<VirtualFolder> VirtualFolders { get; set; }
        public DbSet<VirtualFolderUser> VirtualFolderUsers { get; set; }
        public DbSet<HierarchyFolder> HierarchyFolders { get; private set; }
        public DbSet<DocumentDeleted> DocumentDeleteds { get; set; }
        public DbSet<DocumentToken> DocumentTokens { get; set; }
        public DbSet<RecentActivity> RecentActivities { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<DocumentShareableLink> DocumentShareableLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DocumentUserPermission>().HasKey(c => new { c.DocumentId, c.UserId });
            builder.Entity<DocumentUserPermission>().HasIndex(c => new { c.DocumentId, c.UserId });
            builder.Entity<PhysicalFolderUser>().HasKey(c => new { c.FolderId, c.UserId });
            builder.Entity<VirtualFolderUser>().HasKey(c => new { c.FolderId, c.UserId });
            builder.Entity<DocumentStarred>().HasKey(c => new { c.DocumentId, c.UserId });

            builder.Entity<PhysicalFolder>(entity =>
            {

                entity.Property(u => u.SystemFolderName).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasAlternateKey(x => x.SystemFolderName).HasName("AK_PhysicalFolder"); ;

                entity.HasIndex(c => new { c.Name, c.IsDeleted, c.ParentId });
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.PhysicalFolderUsers)
                   .WithOne(x => x.PhysicalFolder)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);

            });

            builder.Entity<VirtualFolder>(entity =>
            {
                entity.HasIndex(c => new { c.Name, c.IsDeleted, c.ParentId, c.PhysicalFolderId });
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.VirtualFolderUsers)
                   .WithOne(x => x.VirtualFolder)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SharedDocumentUser>().HasKey(sc => new { sc.UserId, sc.DocumentId });

            builder.Entity<Document>(entity =>
            {
                entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(ur => ur.CreatedBy)
                  .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(c => new { c.Name, c.IsDeleted, c.PhysicalFolderId });
            });

            builder.Entity<DocumentDeleted>(entity =>
            {
                entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(ur => ur.CreatedBy)
                  .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasKey(c => new { c.DocumentId, c.UserId });
            });

            builder.Entity<DocumentVersion>(b =>
            {
                b.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<DocumentComment>(b =>
            {
                b.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<VirtualFolderUser>(b =>
            {
                b.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<EmailSMTPSetting>(b =>
            {
                b.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.CreatedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<RecentActivity>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.VirtualFolder)
                   .WithMany()
                   .HasForeignKey(ur => ur.FolderId)
                   .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Document)
                  .WithMany()
                  .HasForeignKey(ur => ur.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserNotification>(entity =>
            {
                entity.HasOne(e => e.Document)
                    .WithMany()
                    .HasForeignKey(ur => ur.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<User>().ToTable("Users");
            builder.Ignore<IdentityUserToken<Guid>>();
            builder.Ignore<IdentityUserLogin<Guid>>();
            builder.DefalutMappingValue();
            builder.DefalutDeleteValueFilter();
        }
    }
}
