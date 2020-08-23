namespace Etymology.Tool
{
    using CommandLine;

    [Verb("svgtodb", HelpText = "Add SVG files to database.")]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class SvgOptions
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        [Option('d', "directory", Required = false, HelpText = "Save all SVG files in the directory to database.")]
        public string Directory { get; set; } = string.Empty;

        [Option('c', "connection", Required = false, HelpText = "Connection string to database.")]
        public string Connection { get; set; } = string.Empty;
    }
}