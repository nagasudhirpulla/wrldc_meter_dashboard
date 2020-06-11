using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.ScadaData.Services
{
    public interface IScadaDataService
    {
        IEnumerable<double> FetchScadaData(string tag, DateTime startDate, DateTime endDate);
    }
}
