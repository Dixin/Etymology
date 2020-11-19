namespace Etymology.Web.Server
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;

    public record Settings
    {
        public List<string> AllowedHosts { get; } = new();

        public Dictionary<string, string> Connections { get; } = new();

        public string ErrorPagePath { get; init; } = string.Empty;

        public List<string> IndexPagePaths { get; } = new();

        public bool IsHttpsOnly { get; init; }

        public List<string> PublicPaths { get; } = new();

        public Dictionary<string, string> Routes { get; } = new();

        public SameSiteMode SameSiteMode { get; init; }
    }
}
