namespace Etymology.Web
{
    using System.Collections.Generic;

    public class Settings
    {
        public Dictionary<string, string> ConnectionStrings { get; } = new Dictionary<string, string>();

        public List<string> Referers { get; } = new List<string>();

        public List<string> IndexPageUrls { get; } = new List<string>();

        public List<string> IgnoreRequestValidation { get; } = new List<string>();
    }
}