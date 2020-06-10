using MeterDataDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterDataDashboard.Infra.Persistence.Configurations
{
    public class ScadaNodeConfiguration : IEntityTypeConfiguration<ScadaNode>
    {
        public void Configure(EntityTypeBuilder<ScadaNode> builder)
        {
            // Node Name is required and just 250 characters
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(250);

            // Measurement tag is unique
            builder
            .HasIndex(b => b.Name)
            .IsUnique();

            // Node Ip is required and just 50 characters
            builder.Property(b => b.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            // Node Ip is unique
            builder
            .HasIndex(b => b.IpAddress)
            .IsUnique();
        }
    }
}
