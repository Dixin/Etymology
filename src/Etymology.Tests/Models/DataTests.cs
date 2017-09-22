namespace ChineseEtymology.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            using (EtymologyContext database = CreateDatabase())
            {
                var (chinese, etymologies, oracles, bronzes, seals, liushutongs) = await database.AnalyzeAsync('车');
                Assert.IsNotNull(etymologies);
                Assert.IsTrue(etymologies.Any());
                Assert.IsNotNull(etymologies.First());
                Assert.IsNotNull(oracles);
                Assert.IsTrue(oracles.Any());
                Assert.IsNotNull(oracles.First());
                Assert.IsNotNull(bronzes);
                Assert.IsTrue(bronzes.Any());
                Assert.IsNotNull(bronzes.First());
                Assert.IsNotNull(seals);
                Assert.IsTrue(seals.Any());
                Assert.IsNotNull(seals.First());
                Assert.IsNotNull(liushutongs);
                Assert.IsTrue(liushutongs.Any());
                Assert.IsNotNull(liushutongs.First());
                Trace.WriteLine($"{chinese} {oracles.Length} {bronzes.Length} {seals.Length} {liushutongs.Length}");
            }
        }

        [TestMethod]
        public void CommonCharactersTest()
        {
            const string CommonCharacters =
                "的一了是我不在人们有来他这上着个地到大里说就去子得也和那要下看天时过出小么起你都把好还多没为又可家学只以主会样年想能生同老中十从自面前头道它后然走很像见两用她国动进成回什边作对开而己些现山民候经发工向事命给长水几义三声于高正妈手知理眼志点心战二问但身方实吃做叫当住听革打呢真党全才四已所敌之最光产情路分总条白话东席次亲如被花口放儿常西气五第使写军吧文运再果怎定许快明行因别飞外树物活部门无往船望新带队先力完间却站代员机更九您每风级跟笑啊孩万少直意夜比阶连车重便斗马哪化太指变社似士者干石满日决百原拿群究各六本思解立河爸村八难早论吗根共让相研今其书坐接应关信觉死步反处记将千找争领或师结块跑谁草越字加脚紧爱等习阵怕月青半火法题建赶位唱海七女任件感准张团屋爷离色脸片科倒睛利世病刚且由送切星导晚表够整认响雪流未场该并底深刻平伟忙提确近亮轻讲农古黑告界拉名呀土清阳照办史改历转画造嘴此治北必服雨穿父内识验传业菜爬睡兴形量咱观苦体众通冲合破友度术饭公旁房极南枪读沙岁线线野坚空收算至政城劳落钱特围弟胜教热展包歌类渐强数乡呼性音答哥际旧神座章帮啦受系令跳非何牛取入岸敢掉忽种装顶急林停息句娘区衣般报叶压母慢叔背细";
            Trace.WriteLine(CommonCharacters.Length);

            Trace.WriteLine("Start queries.");
            List<AnalyzeResult> results = CommonCharacters
                .Select(chinese =>
                {
                    using (EtymologyContext database = CreateDatabase())
                    {
                        return database.AnalyzeAsync(chinese).Result;
                    }
                })
                .Where(result =>
                {
                    var (chinese, etymologies, oracles, bronzes, seals, liushutongs) = result;
                    // Trace.WriteLine($"{chinese} {etymologies.Length} {oracles.Length} {bronzes.Length} {seals.Length} {liushutongs.Length}");
                    return etymologies.Length > 0 && oracles.Length > 0 && bronzes.Length > 0 && seals.Length > 0 && liushutongs.Length > 0;
                })
                .ToList();
            Assert.IsTrue(results.Any());
            Trace.WriteLine(new string(results.Select(result=>result.Chinese).ToArray()));
        }

        private static EtymologyContext CreateDatabase()
        {
            string connection = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddJsonFile("settings.Development.json", optional: true)
                .Build()
                .GetConnectionString(nameof(Etymology));
            return new EtymologyContext(new DbContextOptionsBuilder<EtymologyContext>().UseSqlServer(
                connection,
                options => options
                    .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)).Options);
        }
    }
}
