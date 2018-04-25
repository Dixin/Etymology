namespace Etymology.Tool
{
    using System;
    using CommandLine;
    using Etymology.Common;

    [Verb("unicode", HelpText = "Convert from or to Unicode code point.")]
    internal class UnicodeOptions
    {
        [Option('f', "from", Required = false, HelpText = "Convert from Unicode code point.")]
        public string From { get; set; } = string.Empty;

        [Option('t', "to", Required = false, HelpText = "Convert from Unicode code point.")]
        public string To { get; set; } = string.Empty;
    }

    internal static class Unicode
    {
        internal static int Convert(UnicodeOptions unicodeOptions)
        {
            if (!string.IsNullOrWhiteSpace(unicodeOptions.From))
            {
                Console.WriteLine(unicodeOptions.From.FromUnicodeCodePoint());
            }

            if (!string.IsNullOrWhiteSpace(unicodeOptions.To))
            {
                Console.WriteLine(unicodeOptions.To.ToUnicodeCodePoint());
            }

            return 1;
        }
    }
}
