using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class WeatherConfiguration : IEntityTypeConfiguration<Weather>
{
    public void Configure(EntityTypeBuilder<Weather> builder)
    {
        builder.ToTable(nameof(Weather));
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => w.Id);
        builder.Property(w => w.Id).HasColumnType("integer").IsRequired().ValueGeneratedOnAdd();

        builder.Property(w => w.Date).HasColumnType("date").IsRequired();
        builder.Property(w => w.Time).HasColumnType("time").IsRequired();
        builder.Property(w => w.Temperature).HasColumnType("double precision").IsRequired();
        builder.Property(w => w.RelativeHumidity).HasColumnType("double precision");
        builder.Property(w => w.DewPoint).HasColumnType("double precision");
        builder.Property(w => w.AtmosphericPressure).HasColumnType("double precision");
        builder.Property(w => w.WindSpeed).HasColumnType("double precision");
        builder.Property(w => w.Cloudiness).HasColumnType("double precision");
        builder.Property(w => w.LowerLimitCloud).HasColumnType("integer");
        builder.Property(w => w.Visibility).HasColumnType("integer");
        builder.Property(w => w.WeatherPhenomena).HasColumnType("text");

        builder.HasMany(w => w.WindDirections)
            .WithMany()
            .UsingEntity(j => j.ToTable("WeatherWindDirection"));
    }
}
