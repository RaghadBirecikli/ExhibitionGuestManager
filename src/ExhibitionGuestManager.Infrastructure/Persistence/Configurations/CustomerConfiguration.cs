using ExhibitionGuestManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExhibitionGuestManager.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(customer => customer.MobileNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(customer => customer.City)
            .HasMaxLength(100);

        builder.Property(customer => customer.CompanyName)
            .HasMaxLength(200);

        builder.Property(customer => customer.Department)
            .HasMaxLength(150);

        builder.Property(customer => customer.GeneralNotes)
            .HasMaxLength(1000);

        builder.Property(customer => customer.InternalNotes)
            .HasMaxLength(1000);

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.CreatedBy)
            .HasMaxLength(450);

        builder.Property(customer => customer.UpdatedBy)
            .HasMaxLength(450);

        builder.Property(customer => customer.DeletedBy)
            .HasMaxLength(450);

        builder.Property(customer => customer.Status)
            .HasConversion<int>();

        builder.HasQueryFilter(customer => !customer.IsDeleted);

        builder.HasIndex(customer => customer.FullName);
        builder.HasIndex(customer => customer.MobileNumber);
        builder.HasIndex(customer => customer.City);
        builder.HasIndex(customer => customer.CompanyName);
        builder.HasIndex(customer => customer.Department);
        builder.HasIndex(customer => customer.Status);
        builder.HasIndex(customer => customer.CreatedAt);
    }
}
