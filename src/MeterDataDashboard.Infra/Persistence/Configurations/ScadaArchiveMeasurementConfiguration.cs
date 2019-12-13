using MeterDataDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterDataDashboard.Infra.Persistence.Configurations
{
    public class ScadaArchiveMeasurementConfiguration : IEntityTypeConfiguration<ScadaArchiveMeasurement>
    {
        public void Configure(EntityTypeBuilder<ScadaArchiveMeasurement> builder)
        {
            // Measurement tag is required and just 250 characters
            builder.Property(b => b.MeasTag)
                .IsRequired()
                .HasMaxLength(250);

            // Measurement tag is unique
            builder
            .HasIndex(b => b.MeasTag)
            .IsUnique();

            // Measurement Description is required
            builder.Property(b => b.Description)
                .IsRequired();

            // Measurement type is required
            builder.Property(b => b.MeasType)
                .IsRequired();
        }
    }
}
