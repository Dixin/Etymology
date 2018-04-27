namespace Etymology.Tests.Web.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Etymology.Data.Models;
    using Etymology.Tests.Common;
    using Etymology.Tests.Data.Models;
    using Etymology.Web.Server.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EtymologyControllerTests
    {
        private static EtymologyController CreateController() =>
            new EtymologyController(EtymologyContextTests.CreateDatabase(), new NullLogger<EtymologyController>());

        [TestMethod]
        public async Task AnalyzeAsyncTest()
        {
            IEnumerable<Task> testTasks = ChineseTests.ChineseCharacters.Select(async item =>
            {
                EtymologyController controller = CreateController();
                ViewResult view = await controller.AnalyzeAsync(item.Text) as ViewResult;
                Assert.IsNotNull(view);
                Assert.IsInstanceOfType(view.Model, typeof((string, TimeSpan, AnalyzeResult[])));
                var (chinese, duration, results) = ((string, TimeSpan, AnalyzeResult[]))view.Model;
                Assert.AreEqual(item.Text, chinese);
                Assert.IsTrue(duration > TimeSpan.Zero);
                Assert.IsNotNull(results);
            });
            await Task.WhenAll(testTasks);
        }

        [TestMethod]
        public async Task AnalyzeAsyncErrorTest()
        {
            IEnumerable<Task> testTasks = ChineseTests.OtherCharacters.Select(async item =>
            {
                EtymologyController controller = CreateController();
                BadRequestObjectResult badRequest = await controller.AnalyzeAsync(item) as BadRequestObjectResult;
                Assert.IsNotNull(badRequest);
                Assert.IsInstanceOfType(badRequest.Value, typeof(ArgumentException));
                Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
            });
            await Task.WhenAll(testTasks);
        }
    }
}
