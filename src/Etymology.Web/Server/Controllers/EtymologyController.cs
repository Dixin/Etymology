namespace Etymology.Web.Server.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
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
            this.logger.LogInformation("Received {chinese} to analyze.", chinese);
            if (string.IsNullOrEmpty(chinese))
            {
                this.logger.LogWarning("Received {chinese} is null or empty.", chinese);
                return this.BadRequest(new ArgumentNullException(nameof(chinese)));
            }
            if (chinese.Length != 1)
            {
                this.logger.LogWarning("Received {chinese} is not a single character.", chinese);
                return this.BadRequest(new ArgumentOutOfRangeException(nameof(chinese)));
            }
            if (char.GetUnicodeCategory(chinese, 0) != UnicodeCategory.OtherLetter)
            {
                this.logger.LogWarning("Received {chinese} is not chinese character.", chinese);
                return this.BadRequest(new ArgumentOutOfRangeException(nameof(chinese)));
            }

            try
            {
                this.logger.LogInformation("Start to query database for {chinese}.", chinese);
                AnalyzeResult result = await this.context.AnalyzeAsync(chinese.Single());
                this.logger.LogInformation("Database query is done successfully for {chinese}.", chinese);
                if (!result.Etymologies.Any())
                {
                    this.logger.LogCritical("The etymology for {chinese} is not found.", chinese);
                    return this.View("~/Server/Views/Etymology/NotFound.cshtml", result);
                }

                return this.View("~/Server/Views/Etymology/Analyze.cshtml", result);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Database query fails for {chinese}.", chinese);
                throw;
            }
        }
    }
}