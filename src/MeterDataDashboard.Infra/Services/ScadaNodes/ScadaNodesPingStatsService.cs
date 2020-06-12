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
        private readonly IConfiguration _configuration;
        public ScadaNodesPingStatsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<NodesPingLiveStatusDTO> FetchNodesPingLiveStatus()
        {
            List<NodesPingLiveStatusDTO> res = new List<NodesPingLiveStatusDTO>();
            // Connect to a PostgreSQL database
            NpgsqlConnection conn = new NpgsqlConnection(_configuration["ConnectionStrings:ScadaArchiveConnection"]);
            conn.Open();

            // Define a query
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

    }
}