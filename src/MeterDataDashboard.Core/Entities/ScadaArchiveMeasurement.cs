using MeterDataDashboard.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Core.Entities
{
    public class ScadaArchiveMeasurement : AuditableEntity
    {
        [Required]
        public string MeasTag { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string MeasType { get; set; }
    }
}