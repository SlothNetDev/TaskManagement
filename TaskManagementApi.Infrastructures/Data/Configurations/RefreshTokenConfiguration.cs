using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Infrastructures.Identity.Models;

namespace TaskManagement.Infrastructures.Data.Configurations
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("Refresh Toke"); //table name
            builder.HasKey(x => x.Id); //setting as id

            //relationships
            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(deleteBehavior:DeleteBehavior.Cascade); //delete refresh token too
        }
    }
}
