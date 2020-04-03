using System;
using System.Text;

namespace MeterDataDashboard.Core.ScheduleData
{
    public class UtilSchRow
    {
        public DateTime SchDate { get; set; }
        public int Block { get; set; }
        public string UtilName { get; set; }
        public string SchType { get; set; }
        public double SchVal { get; set; }
    }
}
