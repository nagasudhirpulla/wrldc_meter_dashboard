using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMUDataAdapter
{
    public class PmuAdapter
    {

        public PmuAdapter()
        {
           
        }

        public async Task<List<double>> FetchData(int measId, DateTime startTime, DateTime endTime)
        {
            List<double> res = new List<double>();
            //List<(long, double)> fetchedData = await HistoryDataAdapter_.GetSingleMeasDataAsync(startTime, endTime, measId, DataRate_, RefMeasId_);
            //foreach ((long, double) dataPnt in fetchedData)
            //{
            //    // add timestamp
            //    res.Add(dataPnt.Item1);
            //    // add value
            //    res.Add(dataPnt.Item2);
            //}
            return res;
        }
    }
}