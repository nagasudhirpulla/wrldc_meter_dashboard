using MeterDataDashboard.Core.ScheduleData;
using System;
using System.Collections.Generic;

namespace MeterDataDashboard.Core.ScadaData.Services
{
    public interface IWbesArchiveDataService
    {
        IEnumerable<double> FetchData(string utilName, string schType, DateTime startDate, DateTime endDate);
        IEnumerable<string> FetchSchUtils();
        void PushSchRowsToArchive(List<UtilSchRow> rows);
    }
}
