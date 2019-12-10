using MeterDataDashboard.Core.Common;

namespace MeterDataDashboard.Core.Entities
{
    public class FictMeasurement : AuditableEntity
    {
        public string LocationTag { get; set; }
        public string Description { get; set; }
    }
}
