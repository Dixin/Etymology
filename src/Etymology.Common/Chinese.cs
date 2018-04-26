namespace Etymology.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static partial class Chinese
    {
        public static Encoding GB18030 { get; } = Encoding.GetEncoding(nameof(GB18030));

        public static string ConvertText(this string value, Encoding from, Encoding to) =>
            to.GetString(Encoding.Convert(from, to, from.GetBytes(value)));

        public static byte[] ConvertBytes(this byte[] bytes, Encoding from, Encoding to) =>
            Encoding.Convert(from, to, bytes);

        public static int BytesToInt32(this byte[] bytes) =>
            bytes.Aggregate(0, (result, @byte) => result * 256 + @byte);

        public static byte[] TextToBytes(this string text, Encoding encoding = null) =>
            (encoding ?? Encoding.Unicode).GetBytes(text);

        public static string BytesToText(this byte[] bytes, Encoding encoding = null) =>
            (encoding ?? Encoding.Unicode).GetString(bytes);

        private static bool IsBasicCodePoint(this string hexCodePoint) => // Basic Multilingual Plane.
            !string.IsNullOrWhiteSpace(hexCodePoint) && hexCodePoint.Length == 4;

        private static bool IsSurrogateCodePoint(this string hexCodePoint) => // Surrogate pair.
            !string.IsNullOrWhiteSpace(hexCodePoint) && hexCodePoint.Length == 5;

        // "4E00" => 一
        public static byte[] BasicCodePointToBytes(this string hexCodePoint) =>
            hexCodePoint.IsBasicCodePoint()
                ? hexCodePoint.HexToBytes().FormatBasicBytes()
                : throw new ArgumentOutOfRangeException(nameof(hexCodePoint));

        private static byte[] FormatBasicBytes(this byte[] bytes)
        {
            if (bytes.Length % 2 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            for (int index = 0; index < bytes.Length; index += 2)
            {
                (bytes[index], bytes[index + 1]) = (bytes[index + 1], bytes[index]);
            }

            return bytes;
        }

        private static byte[] FormatSurrogateBytes(this byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        private const string SurrogateCodePointPrefix = "000";

        // "2B740" => 𫝀
        public static byte[] SurrogateCodePointToUtf16Bytes(this string hexCodePoint) =>
            !hexCodePoint.IsSurrogateCodePoint()
                ? throw new ArgumentOutOfRangeException(nameof(hexCodePoint))
                : Encoding.Convert(Encoding.UTF32, Encoding.Unicode, hexCodePoint.SurrogateCodePointToUtf32Bytes());

        // "2B740" => 𫝀
        public static byte[] SurrogateCodePointToUtf32Bytes(this string hexCodePoint) =>
            hexCodePoint.IsSurrogateCodePoint()
                ? $"{SurrogateCodePointPrefix}{hexCodePoint}".HexToBytes().FormatSurrogateBytes()
                : throw new ArgumentOutOfRangeException(nameof(hexCodePoint));

        // "4E00", "2B740"  => 一, 𫝀
        public static byte[] CodePointToBytes(this string hexCodePoint)
        {
            if (hexCodePoint.IsBasicCodePoint())
            {
                return hexCodePoint.BasicCodePointToBytes();
            }

            if (hexCodePoint.IsSurrogateCodePoint())
            {
                return hexCodePoint.SurrogateCodePointToUtf16Bytes();
            }

            throw new ArgumentOutOfRangeException(nameof(hexCodePoint));
        }

        // "4E00", "020000" =>
        public static byte[] HexToBytes(this string hex)
        {
            hex = Regex.Replace(hex, @"\s+", string.Empty);
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hex));
            }

            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int index = 0; index < length; index += 2)
            {
                bytes[index / 2] = byte.Parse(hex.Substring(index, 2), NumberStyles.HexNumber);
            }

            return bytes;
        }

        // 一, 𫝀 => "4E00", "D86DDF40"
        public static string BytesToUtf16CodePoint(this byte[] bytes) =>
            BitConverter.ToString(bytes.FormatBasicBytes()).Replace("-", string.Empty);

        // 一, 𫝀 => "4E00", "2B740"
        public static string BytesToUtf32CodePoint(this byte[] bytes)
        {
            byte[] utf32Bytes = bytes.ConvertBytes(Encoding.Unicode, Encoding.UTF32);
            return BitConverter.ToString(utf32Bytes.FormatSurrogateBytes()).Replace("-", string.Empty).Substring(SurrogateCodePointPrefix.Length);
        }

        public static bool IsBasiBytes(this byte[] bytes) => bytes?.Length == 2;

        // 一, 𫝀 => "4E00", "2B740"
        public static string BytesToCodePoint(this byte[] bytes) =>
            bytes.IsBasiBytes()
                ? bytes.BytesToUtf16CodePoint()
                : bytes.BytesToUtf32CodePoint();

        public static string CodePointToText(this string hexCodePoint, Encoding encoding = null) =>
            hexCodePoint.CodePointToBytes().BytesToText(encoding);

        public static string TextToCodePoint(this string text, Encoding encoding = null) =>
            text.TextToBytes(encoding).BytesToCodePoint();
    }

    public static partial class Chinese
    {
        private static readonly List<(int Min, int Max)> BasicRanges = new List<(int Min, int Max)>();

        private static readonly List<(int Min, int Max)> SurrogateRanges = new List<(int Min, int Max)>();

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
                ("2F800", "2FA1F")); // “丽” CJK Comparability Ideographs Supplement: (U+2F800 to U+2FA1F).
        }

        public static Exception ValidateSingleChineseCharacter(string text, string argument = null)
        {
            // Equivalent to: !string.IsNullOrEmpty(text) && new StringInfo(text).LengthInTextElements == 1.
            if (string.IsNullOrEmpty(text))
            {
                return new ArgumentNullException(argument, "Input is null or empty.");
            }

            int length = text.Length;
            if (length > 2)
            {
                return new ArgumentOutOfRangeException(argument, "Input is more than a single character.");
            }

            // length is either 1 or 2.
            bool isSurrogatePair = char.IsHighSurrogate(text, 0);
            if (isSurrogatePair)
            {
                // length must be 2.
                if (length != 2)
                {
                    return new ArgumentException(argument, "Input is a surrogate character and missing another character in the pair.");
                }
            }
            else
            {
                // length must be 1.
                if (length != 1)
                {
                    return new ArgumentOutOfRangeException(argument, "Input is more than a single character.");
                }
            }

            int codePoint = isSurrogatePair ? char.ConvertToUtf32(text, 0) : text[0];
            // Equivalent to:
            // byte[] bytes = isSurrogate
            //    ? text.TextToBytes().Convert(Encoding.Unicode, Encoding.UTF32).FormatForUtf32()
            //    : text.TextToBytes().Adjust();
            // int codePoint = bytes.BytesToInt32();
            List<(int Min, int Max)> ranges = isSurrogatePair ? SurrogateRanges : BasicRanges;
            return ranges.Any(range => range.Min <= codePoint && range.Max >= codePoint)
                ? null
                : new ArgumentOutOfRangeException(argument, "Input is a single character but not Chinese.");
        }
    }
}
