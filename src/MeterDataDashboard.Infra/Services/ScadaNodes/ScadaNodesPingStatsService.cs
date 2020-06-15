using MeterDataDashboard.Application;
using MeterDataDashboard.Core.ScadaNodes;
using MeterDataDashboard.Core.ScadaNodes.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MeterDataDashboard.Infra.Services.ScadaNodes
{
    public class ScadaNodesPingStatsService : IScadaNodesPingStatsService
    {
        private readonly string _connStr;
        public ScadaNodesPingStatsService(IConfiguration configuration)
        {
            _connStr = configuration["ConnectionStrings:ScadaArchiveConnection"];
        }

        public IEnumerable<NodesPingLiveStatusDTO> FetchNodesPingLiveStatus()
        {
            List<NodesPingLiveStatusDTO> res = new List<NodesPingLiveStatusDTO>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_connStr);
            conn.Open();

            // Define query
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT name, ip, status, data_time, last_toggled_at FROM public.real_node_status 
                                                        order by status desc,name", conn);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.HasRows)
            {
                while (dr.Read())
                {
                    string nodeName = dr.GetString(0);
                    string nodeIp = dr.GetString(1);
                    int nodeStatus = dr.GetInt32(2);
                    DateTime statusTime = dr.GetDateTime(3);
                    DateTime latestToggleTime = dr.GetDateTime(4);
                    res.Add(new NodesPingLiveStatusDTO()
                    {
                        NodeName = nodeName,
                        NodeIp = nodeIp,
                        Status = nodeStatus,
                        StatusTime = statusTime,
                        LatestStatusToggleTime = latestToggleTime
                    });
                }
                dr.NextResult();
            }

            dr.Dispose();

            conn.Close();
            return res;
        }

        public IEnumerable<double> FetchNodePingHist(string nodeName, DateTime startTime, DateTime endTime)
        {
            List<double> res = new List<double>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_connStr);
            conn.Open();

            // Define query
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT data_time, status FROM public.node_status_history 
                                                        where data_time >= 
                                                        (
	                                                        SELECT COALESCE((SELECT max(data_time) FROM public.node_status_history 
	                                                        where data_time <= @startTime and 
	                                                        name=@nodeName), @startTime)
                                                        )
                                                        and data_time <= @endTime
                                                        and name=@nodeName 
                                                        order by data_time", conn);
            command.Parameters.AddWithValue("@nodeName", nodeName);
            command.Parameters.AddWithValue("@startTime", startTime);
            command.Parameters.AddWithValue("@endTime", endTime);

            // Execute the query and obtain a result set
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.HasRows)
            {
                while (dr.Read())
                {
                    DateTime dt = dr.GetDateTime(0);
                    double ts = TimeUtils.ToMillisSinceUnixEpoch(dt);
                    int status = dr.GetInt32(1);
                    res.Add(ts);
                    res.Add(status);
                }
                dr.NextResult();
            }

            dr.Dispose();

            conn.Close();

            // if first sample < startTime, then make it equal to startTime
            double startTimeUnixTs = TimeUtils.ToMillisSinceUnixEpoch(startTime);
            if (res.Count > 0 && res[0] < startTimeUnixTs)
            {
                res[0] = startTimeUnixTs;
            }
            return res;
        }
    }
}