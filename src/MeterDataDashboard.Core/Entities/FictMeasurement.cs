﻿using MeterDataDashboard.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Core.Entities
{
    public class FictMeasurement : AuditableEntity
    {
        [Required]
        public string LocationTag { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public FictMeasType MeasType { get; set; }
    }

    public enum FictMeasType
    {
        Fict,
        Real
    }
}
