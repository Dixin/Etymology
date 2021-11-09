namespace Etymology.Web.Server.Models;

using Etymology.Data.Models;

public record EtymologyAnalyzeModel(string Chinese, TimeSpan Elapsed, AnalyzeResult[] Results);