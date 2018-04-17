namespace Etymology.Tool
{
    using CommandLine;
    using System;
    using System.Linq;

    internal class Program
    {
        private static int Main(string[] args) => 
            Parser.Default.ParseArguments<SvgOptions>(args).MapResult(
                svgOptions => Svg.Save(svgOptions),
                errors =>
                    {
                        errors.ForEach(error => Console.WriteLine(error.ToString()));
                        return 1;
                    });
    }
}
