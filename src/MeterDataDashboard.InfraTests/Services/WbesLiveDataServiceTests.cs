using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeterDataDashboard.Infra.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using MeterDataDashboard.Web;
using System.Reflection;
using System.Threading.Tasks;
using MeterDataDashboard.Core.ScheduleData;

namespace MeterDataDashboard.Infra.Services.Tests
{
    [TestClass()]
    public class WbesLiveDataServiceTests
    {
        private readonly IConfiguration _config;

        public WbesLiveDataServiceTests()
        {
            _config = new ConfigurationBuilder().AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly).Build();
        }

        [TestMethod()]
        public async Task GetMaxRevForDateTestAsync()
        {
            //IConfiguration config = new ConfigurationBuilder().AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly).Build();
            WbesLiveDataService service = new WbesLiveDataService(_config);
            int maxRev = await service.GetMaxRevForDate(DateTime.Now);
            Assert.IsTrue(maxRev != -1);
        }

        [TestMethod()]
        public async Task GetSellerFullSchForDateTestAsync()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            UtilSchData schData = await service.GetSellerFullSchForDate("6477e23c-660e-4587-92d2-8e3488bc8262", DateTime.Now);
            Assert.IsTrue(schData != null);
            Assert.IsTrue(schData.SchVals.Count == 96);
        }

        [TestMethod()]
        public void GetAllThermalIsgsUtilsTest()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            List<(string, string)> utils = service.GetAllThermalIsgsUtils();
            Assert.IsTrue(utils != null);
            Assert.IsTrue(utils.Count > 0);
        }

        [TestMethod()]
        public void GetOnbarInstalledCapacityForDatesTest()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            UtilSchData schData = service.GetOnbarInstalledCapacityForDates("6477e23c-660e-4587-92d2-8e3488bc8262", DateTime.Now, DateTime.Now);
            Assert.IsTrue(schData != null);
            Assert.IsTrue(schData.SchVals.Count == 96);
        }

        [TestMethod()]
        public async Task GetDownMarginsForDateTestAsync()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            UtilSchData schData = await service.GetDownMarginsForDate("6477e23c-660e-4587-92d2-8e3488bc8262", DateTime.Now);
            Assert.IsTrue(schData != null);
            Assert.IsTrue(schData.SchVals.Count == 96);
        }

        [TestMethod()]
        public async Task GetDownMarginsForDatesTestAsync()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            int numDays = 3;
            UtilSchData schData = await service.GetDownMarginsForDates("6477e23c-660e-4587-92d2-8e3488bc8262", DateTime.Now.AddDays(-1 * (numDays - 1)), DateTime.Now);
            Assert.IsTrue(schData != null);
            Assert.IsTrue(schData.SchVals.Count == 96 * numDays);
        }

        [TestMethod()]
        public async Task GetIsgsThermalDownMarginsForDatesTestAsync()
        {
            WbesLiveDataService service = new WbesLiveDataService(_config);
            int numDays = 3;
            IsgsMarginsDTO schData = await service.GetIsgsThermalDownMarginsForDates(DateTime.Now.AddDays(-1 * (numDays - 1)), DateTime.Now);
            Assert.IsTrue(schData != null);
            Assert.IsTrue(schData.GenNames.Count > 10);
            Assert.IsTrue(schData.Timestamps.Count == 96 * (numDays));
            Assert.IsTrue(schData.Margins.Keys.Count == schData.GenNames.Count);
        }
    }
}