namespace Etymology.Web.Server
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;

    public class Settings
    {
        public List<string> AllowedHosts { get; } = new List<string>();

        public Dictionary<string, string> Connections { get; } = new Dictionary<string, string>();

        public string ErrorPagePath { get; set; } = string.Empty;

        public List<string> IndexPagePaths { get; } = new List<string>();

        public bool IsHttpsOnly { get; set; }

        public List<string> PublicPaths { get; } = new List<string>();

        public Dictionary<string, string> Routes { get; } = new Dictionary<string, string>();

        public SameSiteMode SameSiteMode { get; set; }
    }
}
