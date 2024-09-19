using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configuration;

public class OrderItemsConfiguration : IEntityTypeConfiguration<OrderItems>
{
    public void Configure(EntityTypeBuilder<OrderItems> builder)
    {
        //Primary key
        builder.HasKey(p => p.Id);

        //Columns mapping and constraints
        builder.Property(p => p.Quantity).IsRequired();
        builder.Property(p => p.ProductId).IsRequired();
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.UserId).IsRequired();


        //Relationships
        builder.HasOne(p => p.Product).WithMany().HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Order).WithMany().HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);



    }
}
