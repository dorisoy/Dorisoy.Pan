using Dorisoy.Pan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using Dorisoy.Pan.Data.Entities;
using System.Collections.Generic;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Dorisoy.Pan.Common;


namespace Dorisoy.Pan.Domain
{
    public class DocumentContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, IdentityUserLogin<Guid>, RoleClaim, IdentityUserToken<Guid>>
    {
        //public DocumentContext(DbContextOptions options) : base(options)
        //{

        //}

        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options)
        {

        }

        /// <summary>
        /// 重写Users实体
        /// </summary>
        public override DbSet<User> Users { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public override DbSet<UserClaim> UserClaims { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public override DbSet<RoleClaim> RoleClaims { get; set; }

        public DbSet<Operate> Actions { get; set; }
        public DbSet<Page> Pages { get; set; }

        public DbSet<NLog> NLog { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<Audit> AuditTrails { get; set; }


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

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Department> Departments { get; set; }


        public DbSet<Events> Events { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<Connections> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<UserDocumentClaim>()
      .ToTable("UserPermissions")
      .HasKey(c => new { c.DocumentId, c.UserId });
            builder.Entity<UserDocumentClaim>()
                .HasIndex(c => new { c.DocumentId, c.UserId });



            builder.Entity<Operate>()
              .ToTable("Actions")
              .HasKey(c => new { c.Id });

            builder.Entity<Operate>(b =>
            {
                b.HasOne(x => x.Page)
                  .WithMany(x => x.Actions)
                  .HasForeignKey(x => x.PageId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Page>()
                .ToTable("Pages")
                .HasKey(c => new { c.Id });


            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            builder.Entity<User>(b =>
            {
                b.HasMany(e => e.UserClaims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserDocumentClaims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<User>()
                .HasData(new User()
                {
                    Id = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                    Email = "admin@dorisoy.com",
                    UserName = "administrator",
                    PhoneNumber = "13002929017",
                    IsAdmin = true,
                    Sex = Sex.Male,
                    RaleName = "Dorisoy"
                }, new User()
                {
                    Id = Guid.Parse("03CD9F6A-CADB-4AD9-97DC-C94B7F8A273B"),
                    Email = "test@dorisoy.com",
                    UserName = "测试",
                    PhoneNumber = "13002929018",
                    IsAdmin = false,
                    Sex = Sex.Male,
                    RaleName = "王思聪"
                });


            builder.Entity<Role>()
                .ToTable("Roles");
            builder.Entity<Role>(b =>
            {
                b.HasIndex("NormalizedName")
                    .IsUnique(false)
                    .HasDatabaseName("RoleNameIndex");

                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();

                b.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(ur => ur.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(e => e.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(rc => rc.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

            });
            builder.Entity<Role>()
               .HasData(new Role()
               {
                   Id = Guid.Parse("0358FE78-3F71-4CAB-BC9F-D1BF38E86708"),
                   Name = "超级管理员",
                   NormalizedName = "Administrator",
                   IsSystem = true,
                   IsDeleted = false,
                   Description = "表示拥有超级管理权限的用户",
                   CreatedDate = DateTime.Now,

               }, new Role()
               {
                   Id = Guid.Parse("B12A5896-DDCA-4434-94C4-58F0FA420A0B"),
                   Name = "医生",
                   NormalizedName = "Docter",
                   IsSystem = true,
                   IsDeleted = false,
                   Description = "表示拥有医生管理权限的用户",
                   CreatedDate = DateTime.Now,
               }, new Role()
               {
                   Id = Guid.Parse("0E954F0A-80F3-4794-81F2-452AA625F460"),
                   Name = "员工",
                   NormalizedName = "Employe",
                   IsSystem = true,
                   IsDeleted = false,
                   Description = "表示拥有平台平台权限的用户",
                   CreatedDate = DateTime.Now,
               }, new Role()
               {
                   Id = Guid.Parse("D7356BD4-EB59-4459-9A52-E0689FB20178"),
                   Name = "其它",
                   NormalizedName = "Other",
                   IsSystem = false,
                   IsDeleted = false,
                   Description = "自定义其它用户权限",
                   CreatedDate = DateTime.Now,
               });

            //超级管理员初始角色
            builder.Entity<UserRole>()
                .ToTable("UserRoles")
                .HasData(new UserRole()
                {
                    UserId = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                    RoleId = Guid.Parse("0358FE78-3F71-4CAB-BC9F-D1BF38E86708"),
                });

            builder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims");
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //超级管理员初始角色权限
            var allPermissions = new List<RoleClaimResponse>();
            allPermissions.GetAllPermissions();
            var seeds = new List<RoleClaim>();
            var parimValue = 0;
            foreach (var seed in allPermissions)
            {
                parimValue++;
                seeds.Add(new RoleClaim()
                {
                    Id = parimValue,
                    Description = seed.Description,
                    Group = seed.Group,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                    IsDeleted = false,
                    ClaimType = seed.Type,
                    ClaimValue = seed.Value,
                    RoleId = Guid.Parse("0358FE78-3F71-4CAB-BC9F-D1BF38E86708")
                });
            }
            builder.Entity<RoleClaim>().HasData(seeds);


            builder.Entity<DocumentUserPermission>().HasKey(c => new { c.DocumentId, c.UserId });
            builder.Entity<DocumentUserPermission>().HasIndex(c => new { c.DocumentId, c.UserId });
            builder.Entity<PhysicalFolderUser>().HasKey(c => new { c.FolderId, c.UserId });
            builder.Entity<VirtualFolderUser>().HasKey(c => new { c.FolderId, c.UserId });
            builder.Entity<DocumentStarred>().HasKey(c => new { c.DocumentId, c.UserId });

            builder.Entity<Department>()
            .ToTable("Departments")
            .HasKey(c => new { c.Id });
            builder.Entity<Department>(entity =>
            {
                entity.HasIndex(c => new { c.Name, c.IsDeleted, c.ParentId });
            });

            builder.Entity<Audit>()
             .ToTable("Audits")
             .HasKey(c => new { c.Id });

            builder.Entity<Patient>()
             .ToTable("Patients")
             .HasKey(c => new { c.Id });



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



            builder.Entity<PhysicalFolder>()
          .HasData(new PhysicalFolder()
          {
              Id = Guid.Parse("79073EC1-51E2-4772-95E6-9B06075A174B"),
              Name = "全部",
              SystemFolderName = 1,
              ParentId = null,
              Size = "",
              CreatedBy = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
              CreatedDate = DateTime.Now,
              IsDeleted = false
          });

            builder.Entity<PhysicalFolderUser>()
                .HasData(new PhysicalFolderUser()
                {
                    FolderId = Guid.Parse("79073EC1-51E2-4772-95E6-9B06075A174B"),
                    UserId = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                });


            builder.Entity<VirtualFolder>()
            .HasData(new VirtualFolder()
            {
                Id = Guid.Parse("A4D06132-D76C-49B5-8472-2BF78AC4147E"),
                Name = "全部",
                ParentId = null,
                Size = "",
                IsShared = false,
                PhysicalFolderId = Guid.Parse("79073EC1-51E2-4772-95E6-9B06075A174B"),
                CreatedBy = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                CreatedDate = DateTime.Now,
                IsDeleted = false
            });

            builder.Entity<VirtualFolderUser>()
                .HasData(new VirtualFolderUser()
                {
                    CreatedBy = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                    IsStarred = true,
                    ModifiedBy = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                    ModifiedDate = DateTime.Now,
                    FolderId = Guid.Parse("A4D06132-D76C-49B5-8472-2BF78AC4147E"),
                    UserId = Guid.Parse("115CE6FB-EAEB-49C9-9842-583ACE34AA91"),
                });


            builder.Ignore<IdentityUserToken<Guid>>();
            builder.Ignore<IdentityUserLogin<Guid>>();

            builder.DefalutMappingValue();
            builder.DefalutDeleteValueFilter();

        }
    }
}
