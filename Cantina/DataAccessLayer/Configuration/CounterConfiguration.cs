using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configuration;

public class CounterConfiguration : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        //Primary key
        builder.HasKey(p => p.Id);

        //Columns mapping and constraints
        builder.Property(p => p.Up).IsRequired();
        builder.Property(p => p.Down).IsRequired();
        builder.Property(p => p.Difference).IsRequired();
        builder.Property(p => p.Date).IsRequired();
    }
}
