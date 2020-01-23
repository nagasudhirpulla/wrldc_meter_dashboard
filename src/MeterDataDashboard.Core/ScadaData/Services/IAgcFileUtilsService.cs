using MeterDataDashboard.Core.ScheduleData;
using System.Collections.Generic;
using System.IO;

namespace MeterDataDashboard.Core.ScadaData.Services
{
    public interface IAgcFileUtilsService
    {
        List<UtilSchRow> ParseAgcCsv(StreamReader sreader);
    }
}
