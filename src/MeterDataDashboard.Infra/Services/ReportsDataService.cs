using IdentityModel.Client;
using MeterDataDashboard.Core.ReportsData;
using MeterDataDashboard.Core.ReportsData.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Services
{
    public class ReportsDataService : IReportsDataService
    {
        private readonly ReportsConfig _reportsConfig;
        public ReportsDataService(ReportsConfig reportsConfig)
        {
            _reportsConfig = reportsConfig;
        }

        public async Task<string> GetIdentityServerToken()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _reportsConfig.IdServer,
                Policy =
                {
                    RequireHttps = false
                }
            });
            if (disco.IsError)
            {
                // Console.WriteLine(disco.Error);
                return null;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _reportsConfig.ClientId,
                ClientSecret = _reportsConfig.ClientSecret,

                Scope = "scada_archive"
            });

            if (tokenResponse.IsError)
            {
                // Console.WriteLine(tokenResponse.Error);
                return null;
            }
            // Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }

        public async Task<List<PspMeasurement>> GetAllMeasurements()
        {
            List<PspMeasurement> measList = new List<PspMeasurement>();
            // get token
            string token = await GetIdentityServerToken();

            // call api            
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            var response = await apiClient.GetAsync($"{_reportsConfig.Host}/api/Measurements/getAll");
            if (!response.IsSuccessStatusCode)
            {
                //Console.WriteLine(response.StatusCode);
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                measList = JsonConvert.DeserializeObject<List<PspMeasurement>>(content);
            }

            return measList;
        }

        public async Task<List<double>> GetMeasurementData(string measLabel, DateTime startTime, DateTime endTime)
        {
            List<double> measData = new List<double>();

            // get token
            string token = await GetIdentityServerToken();

            // call api            
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            // api/reports/getMeasData/{label}/{startTimeStr}/{endTimeStr}
            var response = await apiClient.GetAsync($"{_reportsConfig.Host}/api/reports/getMeasData/{measLabel}/{startTime.ToString("yyyy-MM-dd-HH-mm-ss")}/{endTime.ToString("yyyy-MM-dd-HH-mm-ss")}");
            if (response.IsSuccessStatusCode)
            {
                // we get the result in the form of ts1,val1,ts2,val2,...
                string content = await response.Content.ReadAsStringAsync();
                measData = JsonConvert.DeserializeObject<List<double>>(content);
            }
            return measData;
        }
    }
}
