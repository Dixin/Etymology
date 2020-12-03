namespace Etymology.Tests.Web.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Etymology.Common;
    using Etymology.Data.Cache;
    using Etymology.Data.Models;
    using Etymology.Tests.Common;
    using Etymology.Tests.Data.Models;
    using Etymology.Web.Server.Controllers;
    using Etymology.Web.Server.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EtymologyControllerTests
    {
        [TestMethod]
        public async Task AnalyzeAsyncTest()
        {
            IEnumerable<Task> testTasks = ChineseTests.ChineseCharacters.Select(async item =>
            {
                EtymologyController controller = CreateController();
                controller.ControllerContext.HttpContext.Request.Headers.Add(nameof(Chinese), new StringValues(char.ConvertToUtf32(item.Text, 0).ToString("D", CultureInfo.InvariantCulture)));
                ViewResult? view;
                try
                {
                    view = await controller.AnalyzeAsync(item.Text) as ViewResult;
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                    throw;
                }

                Assert.IsNotNull(view);
                Assert.IsInstanceOfType(view!.Model, typeof(EtymologyAnalyzeModel));
                (string chinese, TimeSpan duration, AnalyzeResult[] results) = (EtymologyAnalyzeModel)view.Model;
                Assert.AreEqual(item.Text, chinese);
                Assert.IsTrue(duration > TimeSpan.Zero);
                Assert.IsNotNull(results);
            });
            await Task.WhenAll(testTasks);
        }

        [TestMethod]
        public async Task AnalyzeAsyncErrorTest()
        {
            IEnumerable<Task> testTasks = ChineseTests.NonChineseCharacters.Select(async item =>
            {
                EtymologyController controller = CreateController();
                try
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    controller.ControllerContext.HttpContext.Request.Headers.Add(nameof(Chinese), new StringValues(char.ConvertToUtf32(item, 0).ToString("D", CultureInfo.InvariantCulture)));
#pragma warning restore CS8604 // Possible null reference argument.
                }
                catch (Exception exception) when (exception.IsNotCritical())
                {
                    Trace.WriteLine(exception);
                }

                Trace.WriteLine(item);
#if DEBUG
                BadRequestObjectResult badRequest = (BadRequestObjectResult)await controller.AnalyzeAsync(item!);
#else
                BadRequestResult badRequest = (BadRequestResult)await controller.AnalyzeAsync(item!);
#endif
                Assert.IsNotNull(badRequest);
#if DEBUG
                Assert.IsInstanceOfType(badRequest.Value, typeof(ArgumentException));
#endif
                Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
            });
            await Task.WhenAll(testTasks);
        }

        private static EtymologyController CreateController()
        {
            return new(
#pragma warning disable CA2000 // Dispose objects before losing scope
                EtymologyContextTests.CreateDatabase(),
                new NullLogger<EtymologyController>(),
                new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions())),
                new CharacterCache(EtymologyContextTests.CreateDatabase()))
#pragma warning restore CA2000 // Dispose objects before losing scope
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };
        }
    }
}
