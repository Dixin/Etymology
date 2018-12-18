namespace Etymology.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static partial class Chinese
    {
        private static readonly List<(int Min, int Max)> BasicRanges = new List<(int Min, int Max)>();

        private static readonly List<(int Min, int Max)> SurrogateRanges = new List<(int Min, int Max)>();

        private static readonly List<(int Min, int Max)> StrokeRanges = new List<(int Min, int Max)>();

        private static void Add(this List<(int Min, int Max)> ranges, params (string Min, string Max)[] hexCodePoints) =>
            ranges.AddRange(hexCodePoints.Select(hexCodePoint =>
                (int.Parse(hexCodePoint.Min, NumberStyles.HexNumber), int.Parse(hexCodePoint.Max, NumberStyles.HexNumber))));

        static Chinese()
        {
            BasicRanges.Add(
                ("4E00", "9FFF"), // 一 CJK Unified Ideograms Unified Ideographs: (U+4E00 to U+9FFF).
                ("3400", "4DBF"), // 㐦 CJK Ideographs Extension A: (U+3400 to U+4DBF).
                ("2E80", "2EFF"), // ⺀ CJK Radicals Supplement: (U+2E80 to U+2EFF).
                ("2F00", "2FDF"), // ⼀ Kangxi Radicals: (U+2F00 to U+2FDF).
                ("2FF0", "2FFF"), // “⿰” Ideographic Description Characters: (U+2FF0 to U+2FFF).
                ("3000", "303F"), // 〥 CJK Symbols and Punctuation: (U+3000 to U+303F).
                ("3040", "309F"), // い Hiragana: (U+3040 to U+309F).
                ("30A0", "30FF"), // ア Katakana: (U+30A0 to U+30FF).
                ("3100", "312F"), // ㄆ Bopomofo: (U+3100 to U+312F).
                ("31A0", "31BF"), // ㆡ Bopomofo Extended: (U+31A0 to U+31BF).
                ("31C0", "31EF"), // ㇏ CJK Strokes: (U+31C0 to U+31EF) Legitimize.
                ("31F0", "31FF"), // ㇰ Katakana Phonetic Extensions: (U+31F0 to U+31FF).
                ("F900", "FAFF"), // 豈 CJK Compatibility Ideographs: (U+F900 to U+FAFF).
                ("FE30", "FE4F"), // ︽ CJK Compatibility Forms: (U+FE30 to U+FE4F).
                ("FF00", "FFEF")); // ｬ Half width and Full width Forms: (U+FF00 to U+FFEF).

            SurrogateRanges.Add(
                ("20000", "2A6DF"), // 𠀀 CJK Ideographs Extension B: (U+20000 to U+2A6DF).
                ("2A700", "2B73F"), // 𪜀 CJK Ideographs Extension C: (U+2A700 to U+2B73F).
                ("2B740", "2B81F"), // 𫝀 CJK Ideographs Extension D: (U+2B740 to U+2B81F).
                ("2B820", "2CEAF"), // 𫢸 CJK Ideographs Extension E: (U+2B820 to U+2CEAF).
                ("2CEB0", "2EBEF"), // 𬺰 CJK Ideographs Extension F: (U+2CEB0 to U+2EBEF).
                ("2F800", "2FA1F")); // “丽” CJK Comparability Ideographs Supplement: (U+2F800 to U+2FA1F).

            StrokeRanges.Add(
                ("31C0", "31E3")); // Strokes are not handled by SQL Server collation.
        }

        public static (Exception Exception, bool IsSingleSurrogatePair) ValidateSingleCharacter(string text, string argument = null)
        {
            // Equivalent to: !string.IsNullOrEmpty(text) && new StringInfo(text).LengthInTextElements == 1.
            if (string.IsNullOrEmpty(text))
            {
                return (new ArgumentNullException(argument, "Input is null or empty."), false);
            }

            int length = text.Length;
            if (length > 2)
            {
                return (new ArgumentOutOfRangeException(argument, "Input is more than a single character."), false);
            }

            // length is either 1 or 2.
            bool isSurrogate = char.IsHighSurrogate(text, 0);
            if (isSurrogate)
            {
                // length must be 2.
                if (length != 2)
                {
                    return (new ArgumentException(argument, "Input is a single surrogate character and missing another character in the pair."), false);
                }
            }
            else
            {
                // length must be 1.
                if (length != 1)
                {
                    return (new ArgumentOutOfRangeException(argument, "Input is more than a single character."), false);
                }
            }

            return (null, isSurrogate);
        }

        public static (Exception Exception, bool IsSingleSurrogatePair) ValidateSingleChineseCharacter(string text, string argument = null)
        {
            (Exception exception, bool isSurrogate) = ValidateSingleCharacter(text, argument);
            if (exception != null)
            {
                return (exception, isSurrogate);
            }

            int codePoint = isSurrogate ? char.ConvertToUtf32(text, 0) : text[0];
            // Equivalent to:
            // byte[] bytes = isSurrogate
            //    ? text.TextToBytes().Convert(Encoding.Unicode, Encoding.UTF32).FormatForUtf32()
            //    : text.TextToBytes().Adjust();
            // int codePoint = bytes.BytesToInt32();
            List<(int Min, int Max)> ranges = isSurrogate ? SurrogateRanges : BasicRanges;
            return ranges.Any(range => range.Min <= codePoint && range.Max >= codePoint)
                ? (null, isSurrogate)
                : (new ArgumentOutOfRangeException(argument, "Input is a single character but not Chinese."), isSurrogate);
        }
    }
}
