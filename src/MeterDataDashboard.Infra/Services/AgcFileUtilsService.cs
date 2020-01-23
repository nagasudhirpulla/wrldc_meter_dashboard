using MeterDataDashboard.Core.ScadaData.Services;
using MeterDataDashboard.Core.ScheduleData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MeterDataDashboard.Infra.Services
{
    public class AgcFileUtilsService : IAgcFileUtilsService
    {
        public AgcFileUtilsService()
        { }
        public static Dictionary<string, string> StationAliasDict_ = new Dictionary<string, string> { { "MAUDA2", "MOUDA_II" }, { "MAUDA", "MOUDA" } };
        public List<UtilSchRow> ParseAgcCsv(StreamReader sreader)
        {
            string dateFormat = "d/M/yyyy";
            List<UtilSchRow> rows = new List<UtilSchRow>();
            string[] headers = sreader.ReadLine().Split(',');
            // there should be atleast 99 headers
            if (headers.Length < 99)
            {
                //throw new Exception("There csv headers are less than 99");
                return null;
            }
            // first 3 headers to be DATE,STATION,STATE
            if ((headers[0].ToUpper() != "DATE") || (headers[1].ToUpper() != "STATION") || (headers[2].ToUpper() != "STATE"))
            {
                return null;
            }
            // iterate through each row for daywise 96 block schedule data
            while (!sreader.EndOfStream)
            {
                string[] cells = sreader.ReadLine().Split(',');
                // check if we have sufficient cells
                if (cells.Length < 99)
                {
                    continue;
                }
                //check schedule type
                string schType = cells[2].ToUpper();
                if (schType != "AGC")
                {
                    continue;
                }
                //get csv genName
                string csvGen = cells[1];
                // check if csvGen is present in generator alias dictionary
                if (!AgcFileUtilsService.StationAliasDict_.ContainsKey(csvGen))
                {
                    continue;
                }
                string agcGen = StationAliasDict_[csvGen];
                // parse date
                DateTime schDate;
                if (!DateTime.TryParseExact(cells[0], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out schDate))
                {
                    continue;
                }
                // iterate from col index 3 to 98
                for (int blkIter = 1; blkIter <= 96; blkIter++)
                {
                    int colInd = blkIter + 2;
                    // check if valid numeric string is supplied
                    if (double.TryParse(cells[colInd], out double val))
                    {
                        double schVal = val;
                        if (schType == "AGC")
                        {
                            schVal = val * 4;
                        }
                        rows.Add(new UtilSchRow()
                        {
                            SchDate = schDate,
                            SchVal = schVal,
                            Block = blkIter,
                            UtilName = agcGen,
                            SchType = schType
                        });
                    }
                }
            }
            return rows;
        }
    }
}
