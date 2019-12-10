using System;
using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Web.Models
{
    public class FictDataFetchFormModel
    {
        [Required]
        public DateTime LocationTag { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}