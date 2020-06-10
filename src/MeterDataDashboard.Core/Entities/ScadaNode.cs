using MeterDataDashboard.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Core.Entities
{
    public class ScadaNode : AuditableEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string IpAddress { get; set; }
    }
}