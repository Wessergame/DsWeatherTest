using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class WindDirectionConfiguration : IEntityTypeConfiguration<WindDirection>
{
    public void Configure(EntityTypeBuilder<WindDirection> builder)
    {
        builder.ToTable(nameof(WindDirection));
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => w.Id);
        builder.Property(w => w.Id).HasColumnType("integer").IsRequired().ValueGeneratedOnAdd();

        builder.Property(w => w.Direction).IsRequired().HasColumnType("varchar(2)");
    }
}