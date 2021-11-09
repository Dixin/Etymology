namespace Etymology.Tool;

using Etymology.Common;

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