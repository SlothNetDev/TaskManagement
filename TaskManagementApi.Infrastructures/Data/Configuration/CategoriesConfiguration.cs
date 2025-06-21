using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data.Configuration
{
    internal class CategoriesConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CategoryName)
                .IsRequired()
                .HasColumnType("NVARCHAR(120)");

            builder.Property(X => X.Description)
                .HasColumnType("NVARCHAR(1000)")
                .IsRequired(false);

            //one to many relationships
            //1 user many categories
            builder.HasOne(x => x.User)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.UserId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            //category has many task and if task deleted it should restrict 
            builder.HasMany(x => x.Tasks)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.Id)
                .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
                
        }


    }
}
