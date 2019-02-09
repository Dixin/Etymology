namespace Etymology.Web.Server
{
    using System.Collections.Generic;

    public class Settings
    {
        public Dictionary<string, string> Connections { get; } = new Dictionary<string, string>();

        public List<string> RefererHosts { get; } = new List<string>();

        public List<string> IndexPageUrls { get; } = new List<string>();

        public List<string> ExposedPaths { get; } = new List<string>();
    }
}
