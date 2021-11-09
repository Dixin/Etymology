namespace Etymology.Tool;

using CommandLine;

[Verb("unicode", HelpText = "Convert from or to Unicode code point.")]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal class UnicodeOptions
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
    [Option('c', "codepoint", Required = false, HelpText = "Convert from Unicode code point.")]
    public string CodePoint { get; set; } = string.Empty;

    [Option('t', "text", Required = false, HelpText = "Convert from Unicode code point.")]
    public string Text { get; set; } = string.Empty;
}