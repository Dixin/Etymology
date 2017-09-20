namespace ChineseEtymology.Tests.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Etymology.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public async Task AnalyzeAsyncTest()
        {
            using (EtymologyContext database = new EtymologyContext(new DbContextOptionsBuilder<EtymologyContext>().UseSqlServer(
                new ConfigurationBuilder()
                    .AddJsonFile("settings.json")
                    .Build()
                    .GetConnectionString(nameof(Etymology)),
                options => options
                    .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)).Options))
            {
                var (a, b, c, d, e) = await database.AnalyzeAsync('车');
                Assert.IsNotNull(a);
                Assert.IsTrue(a.Any());
                Assert.IsNotNull(a.First());
                Assert.IsNotNull(b);
                Assert.IsTrue(b.Any());
                Assert.IsNotNull(b.First());
                Assert.IsNotNull(c);
                Assert.IsTrue(c.Any());
                Assert.IsNotNull(c.First());
                Assert.IsNotNull(d);
                Assert.IsTrue(d.Any());
                Assert.IsNotNull(d.First());
                Assert.IsNotNull(e);
                Assert.IsTrue(e.Any());
                Assert.IsNotNull(e.First());
            }
        }
    }
}
