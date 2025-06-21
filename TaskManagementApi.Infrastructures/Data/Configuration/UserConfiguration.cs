using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity;

namespace TaskManagement.Infrastructures.Data.Configuration
{
    sealed partial class UserConfiguration : IEntityTypeConfiguration<ApplicationUsers>
    {
        public void Configure(EntityTypeBuilder<ApplicationUsers> builder)
        {
            builder.ToTable("User_Type");

            builder.HasKey(x => x.DomainUserId);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired(false);

            builder.HasOne(x => x.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUsers>(x => x.DomainUserId);
               
        }
    }
}
