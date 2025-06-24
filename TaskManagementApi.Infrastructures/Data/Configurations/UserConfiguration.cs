using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Infrastructures.Identity.Models;
namespace TaskManagement.Infrastructures.Data.Configuration
{
    sealed partial class UserConfiguration : IEntityTypeConfiguration<ApplicationUsers>
    {
        public void Configure(EntityTypeBuilder<ApplicationUsers> builder)
        {
            builder.ToTable("User_Type");

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired(false);

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUsers>(x => x.DomainUserId);
               
        }
    }
}
