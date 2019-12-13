using MeterDataDashboard.Core.Common;

namespace MeterDataDashboard.Core.Entities
{
    public class ScadaArchiveMeasurement : AuditableEntity
    {
        public string MeasTag { get; set; }
        public string Description { get; set; }
        public string MeasType { get; set; }
    }
}