using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanBudget.Models.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property("Id");

            builder.Property("Firstname")
                .IsRequired()
                .HasMaxLength(25);

            builder.Property("Lastname")
                .IsRequired()
                .HasMaxLength(25);

            builder.Property("Email")
                .IsRequired()
                .HasMaxLength(39);

            builder.Property("Hash")
                .IsRequired()
                .HasMaxLength(44);

            builder.Property("Salt")
                .IsRequired()
                .HasMaxLength(36);
        }
    }
}
