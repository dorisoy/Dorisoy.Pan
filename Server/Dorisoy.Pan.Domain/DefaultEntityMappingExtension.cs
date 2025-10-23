using Dorisoy.Pan.Data;
using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.Domain
{
    public static class DefaultEntityMappingExtension
    {
        public static void DefalutMappingValue(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.ModifiedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }

        public static void DefalutDeleteValueFilter(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<EmailTemplate>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<EmailSMTPSetting>()
            .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<PhysicalFolder>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<VirtualFolder>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<VirtualFolderUser>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<DocumentDeleted>()
                .HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
