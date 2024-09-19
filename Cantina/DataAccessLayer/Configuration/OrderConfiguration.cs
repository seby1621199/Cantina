using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderDate).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.Location).IsRequired().HasMaxLength(15);
            builder.Property(p => p.Status).IsRequired().HasDefaultValue("Pending");
            builder.Property(p => p.VerificationCode).IsRequired();
            builder.Property(p => p.DeliveryPersonId).IsRequired(false);

            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(p => p.DeliveryPerson)
                   .WithMany()
                   .HasForeignKey(p => p.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
