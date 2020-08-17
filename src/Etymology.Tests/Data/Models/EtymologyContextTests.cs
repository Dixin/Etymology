namespace Etymology.Tests.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Etymology.Data.Cache;
    using Etymology.Data.Models;
    using Etymology.Web.Server;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EtymologyContextTests
    {
        [TestMethod]
        public async Task AnalyzeAsyncTest()
        {
            await using EtymologyContext database = CreateDatabase();
            const string chinese = "车";
            CharacterCache characterCache = new CharacterCache();
            await characterCache.Initialize(database);
            IEnumerable<(string Traditional, int CodePoint)> allTraditional = characterCache.AllTraditional(char.ConvertToUtf32(chinese, 0));
            AnalyzeResult[] results = await database.AnalyzeAsync(chinese, allTraditional);
            Assert.AreEqual(1, results.Length);
            var (queriedChinese, etymology, oracles, bronzes, seals, liushutongs) = results.Single();
            Assert.AreEqual(chinese, queriedChinese);
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

        [TestMethod]
        [Ignore]
        public void AnalyzeAllImagesTest()
        {
            Etymology[] etymologies = null;
            using (EtymologyContext database = CreateDatabase())
            {
                etymologies = database.Etymology
                    .Where(entity =>
                        database.Oracle.Any(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null)
                        && database.Bronze.Any(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null)
                        && database.Seal.Any(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null)
                        && database.Liushutong.Any(character => character.Traditional == entity.Traditional && character.ImageVectorBase64 != null))
                    .ToArray();
            }
            Trace.WriteLine(string.Concat(etymologies.Select(entity => entity.Traditional)));
            etymologies.Where(entity => entity.Simplified.Length >= 3).ToList().ForEach(entity => Trace.WriteLine(entity.Simplified));
            etymologies = etymologies.Where(entity => entity.Simplified.Length < 3).ToArray();
            Trace.WriteLine(string.Concat(etymologies.Select(entity => entity.Traditional)));
        }

        [TestMethod]
        public async Task ExtensionBTest()
        {
            await using EtymologyContext database = CreateDatabase();
            const string extensionB = "𠂇";
            CharacterCache characterCache = new CharacterCache();
            await characterCache.Initialize(database);
            IEnumerable<(string Traditional, int CodePoint)> allTraditional = characterCache.AllTraditional(char.ConvertToUtf32(extensionB, 0));
            var (chinese, etymology, oracles, bronzes, seals, liushutongs) = (await database.AnalyzeAsync(extensionB, allTraditional)).Single();
            Assert.IsNotNull(etymology);
            Assert.IsNotNull(etymology);
            Assert.AreEqual(extensionB, etymology.Traditional);
            Trace.WriteLine($"{chinese} {oracles.Length} {bronzes.Length} {seals.Length} {liushutongs.Length}");
        }

        [TestMethod]
        public void CommonCharactersTest()
        {
            const string CommonCharacters = "安八白百敗般邦保寶北貝匕比妣畢賓兵丙秉并伯帛亳卜不步采倉曹曾長厂鬯車徹臣辰晨成承乘遲齒赤沖舂丑出初楚畜傳春祠此次大丹單旦稻得登帝典奠丁鼎冬東動斗豆斷隊對多而兒耳二伐反方妃分封豐酆逢缶夫弗服福甫斧簠黼父复婦復干剛高羔告戈格鬲庚工弓公攻古谷鼓官觀盥光龜歸鬼癸國果亥行蒿好禾合何河侯后厚乎呼壺虎化畫淮黃惠會或獲鑊基箕及吉即亟疾己季既祭家甲斝監見姜彊降角教解巾斤今進盡京兢井競九酒舊沮句絕爵君畯考可克客口寇來牢老醴立利麗良林陵令柳六龍聾盧魯陸鹿麓洛旅率馬買麥卯枚眉每媚門蒙盟皿敏名明鳴命莫母牡木目牧乃男南內逆年廿鳥寧牛農奴諾女旁轡彭辟品僕圃七戚祈齊杞啟气千前遣羌且妾侵秦卿丘區曲取去犬人壬日戎如入若卅三散嗇山商上少紹射涉申沈生省尸師十石食時史矢豕使始士氏事室視首受獸叔黍蜀戍束水司絲死巳四祀宋綏歲孫它唐天田聽同土退屯豚鼉外亡王韋唯未位畏衛文聞問我吳五午武舞勿戊夕兮昔析奚襲徙喜系下先咸獻相祥饗向象小辛新星興兄休羞宿戌須徐宣旋學旬亞言炎衍甗央羊昜陽揚夭爻頁一伊衣依匜夷宜疑彝乙亦邑易異肄義因禋寅尹郢永用攸幽猶游友有酉又幼于余盂雩魚虞漁羽雨圉玉聿育昱御禦元爰員曰月樂龠允宰載再在葬昃增乍宅旃召折貞朕征正鄭之執止旨黹至豸彘中終仲舟州周帚朱逐祝貯追茲子自宗足卒族俎祖尊作";
            Trace.WriteLine(CommonCharacters.Length);

            List<(char chinese, AnalyzeResult[] Result)> testedResults = CommonCharacters
                .Select(chinese =>
                {
                    using EtymologyContext database = CreateDatabase();
                    CharacterCache characterCache = new CharacterCache();
                    characterCache.Initialize(database).Wait();
                    IEnumerable<(string Traditional, int CodePoint)> allTraditional = characterCache.AllTraditional(char.ConvertToUtf32(new string(chinese, 1), 0));
                    return (chinese, database.AnalyzeAsync(new string(chinese, 1), allTraditional).Result);
                })
                .Where(item =>
                {
                    var (text, results) = item;
                    if (!results.Any())
                    {
                        Trace.WriteLine($"Error: {text}");
                        return false;
                    }

                    bool hasCharacters = results.Any(result =>
                    {
                        var (chinese, etymology, oracles, bronzes, seals, liushutongs) = result;
                        return etymology != null && etymology.HasSimplified() && oracles.Any() && bronzes.Any() && seals.Any() && liushutongs.Any();
                    });
                    if (!hasCharacters)
                    {
                        Trace.WriteLine($"Error: {text}");
                    }

                    return hasCharacters;
                })
                .ToList();
            Assert.AreEqual(CommonCharacters.Length, testedResults.Count);
            Trace.WriteLine(string.Concat(testedResults.Select(result => result.chinese)));
        }

        internal static EtymologyContext CreateDatabase()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddJsonFile($"settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .Build();
            Settings settings = configuration.Get<Settings>();
            return new EtymologyContext(new DbContextOptionsBuilder<EtymologyContext>().UseSqlServer(
                settings.Connections[nameof(Etymology)],
                options => options
                    .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)).Options);
        }
    }
}
