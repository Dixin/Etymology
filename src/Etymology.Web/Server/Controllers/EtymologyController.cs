namespace Etymology.Web.Server.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Etymology.Common;
    using Etymology.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;

    public class EtymologyController : Controller
    {
        private readonly ILogger<EtymologyController> logger;

        private readonly IMemoryCache memoryCache;

        private readonly EtymologyContext context;

        public EtymologyController(EtymologyContext context, ILogger<EtymologyController> logger, IMemoryCache memoryCache)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.memoryCache = memoryCache;
        }

        [HttpPost]
        [Route(nameof(Etymology))]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> AnalyzeAsync(string chinese)
        {
            (Exception inputException, _) = Chinese.ValidateSingleChineseCharacter(chinese, nameof(chinese));
            if (inputException != null)
            {
                this.logger.LogWarning("Received {chinese} is invalid. {message}", chinese, inputException.Message);
#if DEBUG
                return this.BadRequest(inputException);
#else
                return this.BadRequest();
#endif
            }

            if (!(this.Request.Headers.TryGetValue(nameof(chinese), out StringValues codePointString)
                && int.TryParse(codePointString.ToString(), out int codePoint)
                && codePoint == char.ConvertToUtf32(chinese, 0)))
            {
#if DEBUG
                return this.BadRequest("Received code point is invalid.");
#else
                return this.BadRequest();
#endif
            }

            this.logger.LogInformation("Received {chinese} to analyze.", chinese);

            AnalyzeResult[] results;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                if (this.memoryCache.TryGetValue(codePoint, out AnalyzeResult[] cachedResults))
                {
                    this.logger.LogInformation("Query result is found in cache for {chinese}.", chinese);
                    results = cachedResults;
                }
                else
                {
                    this.logger.LogInformation("Start to query database for {chinese}.", chinese);
                    results = await this.context.AnalyzeAsync(chinese);
                    this.logger.LogInformation("Database query is done successfully for {chinese}.", chinese);
                    this.memoryCache.Set(codePoint, results, Cache.ServerCacheOptions);
                    this.logger.LogInformation("Query result is added to cache for {chinese}.", chinese);
                }
            }
            catch (Exception exception) when (exception.LogErrorWith(this.logger, "Database query fails for {chinese}.", chinese))
            {
                return null; // Never execute because LogErrorWith returns false.
            }
            finally
            {
                stopwatch.Stop();
            }

            return this.View("~/Server/Views/Etymology/Analyze.cshtml", (chinese, stopwatch.Elapsed, results));
        }
    }
}
