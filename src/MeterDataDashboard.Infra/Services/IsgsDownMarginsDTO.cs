using System;
using System.Collections.Generic;

namespace MeterDataDashboard.Infra.Services
{
    public class IsgsDownMarginsDTO
    {
        public List<string> GenNames { get; set; } = new List<string>();
        public List<DateTime> Timestamps { get; set; } = new List<DateTime>();
        public Dictionary<string, List<double>> DownMargins = new Dictionary<string, List<double>>();
    }
}
