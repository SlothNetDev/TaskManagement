using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data.Configurations
{
    internal class TasksConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.Property(x => x.Title)
                .IsRequired();

            builder.Property(x => x.Priority)
                .HasMaxLength(15)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(15)
                .HasConversion<string>()
                .IsRequired(false);

            builder.Property(x => x.DueDate)
                .IsRequired(false);


            //relationships for user 1: many

            builder.HasOne(x => x.User)
                .WithMany(x => x.Tasks)
                .HasForeignKey(X => X.UserId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            //relationShips with Category one:many

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);


        }
    }
}
