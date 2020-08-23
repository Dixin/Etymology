namespace Etymology.Tool
{
    using System;
    using System.Linq;
    using CommandLine;

    internal static class Program
    {
        private static int Main(string[] args) =>
            Parser.Default.ParseArguments<SvgOptions, UnicodeOptions>(args).MapResult<SvgOptions, UnicodeOptions, int>(
                Svg.Save,
                Unicode.Convert,
                errors =>
                {
                    errors.ForEach(error => Console.WriteLine(error.ToString()));
                    return 1;
                });
    }
}
