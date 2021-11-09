namespace Etymology.Common;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static partial class Chinese
{
    private const string SurrogateCodePointPrefix = "000";

    public static Encoding GB18030 { get; } = Encoding.GetEncoding(nameof(GB18030));

    public static string ConvertText(this string value, Encoding from, Encoding to)
    {
        if (from is null)
        {
            throw new ArgumentNullException(nameof(from));
        }

        if (to is null)
        {
            throw new ArgumentNullException(nameof(to));
        }

        return to.GetString(Encoding.Convert(from, to, from.GetBytes(value)));
    }

    public static byte[] ConvertBytes(this byte[] bytes, Encoding from, Encoding to) =>
        Encoding.Convert(from, to, bytes);

    public static string CodePointToHex(this int codePoint) =>
        codePoint.ToString("X4", CultureInfo.InvariantCulture);

    public static int HexToCodePoint(string hex) =>
        int.Parse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

    public static int BytesToInt32(this byte[] bytes) =>
        bytes.Aggregate(0, (result, @byte) => result * 256 + @byte);

    public static byte[] TextToBytes(this string text, Encoding? encoding = null) =>
        (encoding ?? Encoding.Unicode).GetBytes(text);

    public static string BytesToText(this byte[] bytes, Encoding? encoding = null) =>
        (encoding ?? Encoding.Unicode).GetString(bytes);

    // "4E00" => 一
    public static byte[] BasicCodePointToBytes(this string hexCodePoint) =>
        hexCodePoint.IsBasicCodePoint()
            ? hexCodePoint.HexToBytes().FormatBasicBytes()
            : throw new ArgumentOutOfRangeException(nameof(hexCodePoint));

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

    // "4E00" => [一], "020000" => [𫝀]
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
            bytes[index / 2] = byte.Parse(hex.Substring(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return bytes;
    }

    // "4E00" => [一], "2B740" => [𫝀]
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

    // [一] => "4E00", [𫝀] => "D86DDF40"
    public static string BytesToUtf16CodePoint(this byte[] bytes)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        return BitConverter.ToString(bytes.FormatBasicBytes()).Replace("-", string.Empty, StringComparison.InvariantCulture);
    }

    // [一] => "4E00", [𫝀] => "2B740"
    public static string BytesToUtf32CodePoint(this byte[] bytes)
    {
        byte[] utf32Bytes = bytes.ConvertBytes(Encoding.Unicode, Encoding.UTF32);
        return BitConverter.ToString(utf32Bytes.FormatSurrogateBytes()).Replace("-", string.Empty, StringComparison.InvariantCulture).Substring(SurrogateCodePointPrefix.Length);
    }

    public static bool IsBasicBytes(this byte[] bytes) => bytes.Length == 2;

    public static bool IsSurrogateBytes(this byte[] bytes) => bytes.Length == 4;

    // [一] => "4E00", [𫝀] => "2B740"
    public static string BytesToCodePoint(this byte[] bytes)
    {
        if (bytes.IsBasicBytes())
        {
            return bytes.BytesToUtf16CodePoint();
        }

        if (bytes.IsSurrogateBytes())
        {
            return bytes.BytesToUtf32CodePoint();
        }

        throw new ArgumentOutOfRangeException(nameof(bytes));
    }

    // "3400" => "㐀", "20000" => "𠀀"
    public static string CodePointToText(this string hexCodePoint) =>

        // Equivalent to: hexCodePoint.CodePointToBytes().BytesToText(encoding);
        char.ConvertFromUtf32(int.Parse(hexCodePoint, NumberStyles.HexNumber, CultureInfo.InvariantCulture));

    // "㐀" => "3400", "𠀀" => "20000"
    public static string TextToCodePoint(this string text) =>

        // Equivalent to: text.TextToBytes(encoding).BytesToCodePoint();
        char.ConvertToUtf32(text, 0).ToString("X4", CultureInfo.InvariantCulture);

    public static IEnumerable<string> Characters(this string text)
    {
        StringInfo parsed = new(text);
        for (int index = 0; index < parsed.LengthInTextElements; index++)
        {
            yield return parsed.SubstringByTextElements(index, 1);
        }
    }

    private static bool IsBasicCodePoint(this string hexCodePoint) => // Basic Multilingual Plane.
        !string.IsNullOrWhiteSpace(hexCodePoint) && hexCodePoint.Length == 4;

    private static bool IsSurrogateCodePoint(this string hexCodePoint) => // Surrogate pair.
        !string.IsNullOrWhiteSpace(hexCodePoint) && hexCodePoint.Length == 5;

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
}