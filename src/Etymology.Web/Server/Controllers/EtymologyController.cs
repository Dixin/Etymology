namespace Etymology.Web.Server.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Etymology.Common;
    using Etymology.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class EtymologyController : Controller
    {
        private readonly ILogger<EtymologyController> logger;

        private readonly EtymologyContext context;

        public EtymologyController(EtymologyContext context, ILogger<EtymologyController> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route(nameof(Etymology))]
        public async Task<IActionResult> Analyze(string chinese)
        {
            Exception inputException = Chinese.ValidateSingleChineseCharacter(chinese, nameof(chinese));
            if (inputException != null)
            {
                this.logger.LogWarning("Received {chinese} is invalid. {message}", chinese, inputException.Message);
#if DEBUG
                return this.BadRequest(inputException);
#else
                return this.BadRequest();
#endif
            }

            this.logger.LogInformation("Receisved {chinese} to analyze.", chinese);

            AnalyzeResult[] results;
            Stopwatch stopwatch=Stopwatch.StartNew();
            try
            {
                this.logger.LogInformation("Start to query database for {chinese}.", chinese);
                results = await this.context.AnalyzeAsync(chinese);
                this.logger.LogInformation("Database query is done successfully for {chinese}.", chinese);
            }
            catch (Exception exception) when (exception.LogErrorWith(this.logger, "Database query fails for {chinese}.",
                chinese))
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