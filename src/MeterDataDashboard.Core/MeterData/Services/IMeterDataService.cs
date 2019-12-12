using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.MeterData.Services
{
    public interface IMeterDataService
    {
        IEnumerable<double> FetchFictData(string tag, DateTime startDate, DateTime endDate);
    }
}
