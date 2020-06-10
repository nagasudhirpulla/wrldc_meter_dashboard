using MeterDataDashboard.Application.Common;
using MeterDataDashboard.Core.Common;
using MeterDataDashboard.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }
        public DbSet<FictMeasurement> FictMeasurements { get; set; }
        public DbSet<ScadaArchiveMeasurement> ScadaArchiveMeasurements { get; set; }
        public DbSet<ScadaNode> ScadaNodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
