using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Infrastructures.Data.Seeders;
using TaskManagement.Infrastructures.Identity.Models;
namespace TaskManagement.Infrastructures.Data.Configurations
{
    sealed partial class UserConfiguration : IEntityTypeConfiguration<ApplicationUsers>
    {
        public void Configure(EntityTypeBuilder<ApplicationUsers> builder)
        {
            builder.ToTable("User_Type");

            builder.Property(x => x.CreatedAt)
                .IsRequired(false);

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUsers>(x => x.DomainUserId);
               
        }
    }
}
