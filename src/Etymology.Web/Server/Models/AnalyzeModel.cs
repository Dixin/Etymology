namespace Etymology.Web.Server.Models
{
    using System;
    using Etymology.Data.Models;

    public record AnalyzeModel(string Chinese, TimeSpan Elapsed, AnalyzeResult[] Results);
}
