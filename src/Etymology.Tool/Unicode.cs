namespace Etymology.Tool
{
    using System;
    using CommandLine;
    using Etymology.Common;

    [Verb("unicode", HelpText = "Convert from or to Unicode code point.")]
    internal class UnicodeOptions
    {
        [Option('c', "codepoint", Required = false, HelpText = "Convert from Unicode code point.")]
        public string CodePoint { get; set; } = string.Empty;

        [Option('t', "text", Required = false, HelpText = "Convert from Unicode code point.")]
        public string Text { get; set; } = string.Empty;
    }

    internal static class Unicode
    {
        internal static int Convert(UnicodeOptions unicodeOptions)
        {
            if (!string.IsNullOrEmpty(unicodeOptions.CodePoint))
            {
                Console.WriteLine(unicodeOptions.CodePoint.CodePointToText());
            }

            if (!string.IsNullOrEmpty(unicodeOptions.Text))
            {
                Console.WriteLine(unicodeOptions.Text.TextToCodePoint());
            }

            return 1;
        }
    }
}
