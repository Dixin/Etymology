using System;
using System.Linq;
using CommandLine;
using Etymology.Tool;

Parser
    .Default
    .ParseArguments<SvgOptions, UnicodeOptions>(args)
    .MapResult<SvgOptions, UnicodeOptions, int>(
        Svg.Save,
        Unicode.Convert,
        errors =>
        {
            errors.ForEach(error => Console.WriteLine(error.ToString()));
            return 1;
        });