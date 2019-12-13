using MeterDataDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Infra.Persistence.Configurations
{
    public class FictMeasurementConfiguration : IEntityTypeConfiguration<FictMeasurement>
    {
        public void Configure(EntityTypeBuilder<FictMeasurement> builder)
        {
            // location tag is required and just 10 characters
            builder.Property(b => b.LocationTag)
                .IsRequired()
                .HasMaxLength(10);

            // location tag is unique
            builder
            .HasIndex(b => b.LocationTag)
            .IsUnique();

            // Description is required
            builder.Property(b => b.Description)
                .IsRequired();
        }
    }
}
