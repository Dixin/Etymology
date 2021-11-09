namespace Etymology.Web.Server.Models;

using System;
using Etymology.Data.Models;

public record EtymologyAnalyzeModel(string Chinese, TimeSpan Elapsed, AnalyzeResult[] Results);