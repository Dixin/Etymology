namespace ChineseEtymology.Tests.Models
{
    using System;
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
                AnalyzeResult[] results = await database.AnalyzeAsync("车");
                Assert.AreEqual(1, results.Length);
                var (chinese, etymology, oracles, bronzes, seals, liushutongs) = results.Single();
                Assert.IsNotNull(etymology);
                Assert.IsNotNull(etymology);
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
        //[Ignore]
        public void AnalyzeAllImagesTest()
        {
            Etymology[] etymologies = null;
            using (EtymologyContext database = CreateDatabase())
            {
                etymologies = database.Etymology
                    .Where(entity =>
                        database.Oracle.Count(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null) > 0
                        && database.Bronze.Count(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null) > 0
                        && database.Seal.Count(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null) > 0
                        && database.Liushutong.Count(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null) > 0)
                    .ToArray();
            }
            Trace.WriteLine(string.Concat(etymologies.Select(entity => entity.Traditional)));

            etymologies = etymologies.Where(entity => entity.Simplified.Length < 3).ToArray();
            Trace.WriteLine(string.Concat(etymologies.Select(entity => entity.Traditional)));
            Trace.WriteLine(string.Concat(etymologies.Select(entity => entity.SimplifiedInitial)));
        }

        [TestMethod]
        public async Task ExtensionBTest()
        {
            using (EtymologyContext database = CreateDatabase())
            {
                const string extensionB = "𠂇";
                var (chinese, etymology, oracles, bronzes, seals, liushutongs) = (await database.AnalyzeAsync(extensionB)).Single();
                Assert.IsNotNull(etymology);
                Assert.IsNotNull(etymology);
                Assert.AreEqual(extensionB, etymology.Traditional);
                Trace.WriteLine($"{chinese} {oracles.Length} {bronzes.Length} {seals.Length} {liushutongs.Length}");
            }
        }

        [TestMethod]
        public void CommonCharactersTest()
        {
            const string CommonCharacters = "寶貝兵車徹鬯厂長曹倉采步不卜亳帛伯并秉丙楚賓畢妣比次匕保邦般敗百白臣辰晨曾成承乘遲齒赤沖舂對丑出初畜傳春祠此朿丹北逢單旦得八安帝典奠父鼎冬東動斗豆斷隊多而兒耳二伐弓反匚方分封大酆缶夫弗服稻登福斧簠黼复丁婦復干剛高羔告戈格厚乎鬲工公攻畫妃古谷鼓豐官觀盥光歸甫癸國果亥蒿好禾合何河后呼壺庚降虎化淮黃今惠會或鑊基箕及九吉龜即亟鬼耤己季行考既祭家甲寇斝監侯姜彊角教巾斤進盡京兢獲井競六酒舊沮句絕疾君畯可克客口牢老見醴立利麗解良林敏陵令柳龍盧魯陸鹿爵洛內旅年率馬買麥來卯枚眉每媚門蒙盟皿名明鳴命莫母前遣羌聾妾侵秦卿丘牡區曲取去犬人壬日戎木入若卅三散目牧乃南逆廿鳥涉宁寧牛農奴諾女旁轡彭辟品僕圃七戚祈齊杞啟气千且如山男麓上少紹射申沈生省師十石食時史矢豕使始士氏事室視首受獸唯未位畏衛文聞問我吳五午武舞勿戊夕兮昔析奚襲徙喜系下先咸獻相叔蜀嗇戍束商水絲死巳四祀綏歲尸孫它唐天田聽同土退屯豚鼉外亡王韋黍司宋祥饗向象小效辛新星興兄休羞宿戌須徐宣旋學旬亞言炎衍甗央羊昜陽揚夭爻頁一伊衣依匜夷宜疑彝乙亦育聿玉圉雨羽漁虞魚雩盂余于幼又酉有友昃增游猶幽攸用永尹寅禋因義肄異易邑昱御元員曰月戉樂龠允宰載再在朱葬乍宅旃折貞朕郢征正鄭姪祖執旨黹至爰豸彘中終仲帚逐祝追召茲子自宗足卒之族俎尊止舟州周貯作";
            Trace.WriteLine(CommonCharacters.Length);

            Trace.WriteLine("Start queries.");
            var testedResults = CommonCharacters
                .Select(chinese =>
                {
                    using (EtymologyContext database = CreateDatabase())
                    {
                        return database.AnalyzeAsync(new string(chinese, 1)).Result;
                    }
                })
                .Where(results =>
                {
                    if (!results.Any())
                    {
                        return false;
                    }

                    foreach (AnalyzeResult result in results)
                    {
                        var (chinese, etymology, oracles, bronzes, seals, liushutongs) = result;
                        if (!(etymology != null && etymology.HasSimplified() && oracles.Any() && bronzes.Any() && seals.Any() && liushutongs.Any()))
                        {
                            Trace.WriteLine($"{chinese} {etymology.Traditional} {oracles.Length} {bronzes.Length} {seals.Length} {liushutongs.Length}");
                            return false;
                        }
                    }

                    return true;
                })
                .ToList();
            Assert.AreEqual(CommonCharacters.Length, testedResults.Count);
            Trace.WriteLine(string.Concat(testedResults.Select(result => result.First().Chinese)));
        }

        private static EtymologyContext CreateDatabase()
        {
            string connection = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddJsonFile($"settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
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
