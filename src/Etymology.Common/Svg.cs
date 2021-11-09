namespace Etymology.Common;

public static class Svg
{
    public static string ConvertToBase64(this string svgFile)
    {
        XDocument svgDocument = XDocument.Parse(svgFile);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(svgDocument.Root?.ToString(SaveOptions.DisableFormatting) ?? string.Empty));
    }

    public static bool TryConvertToBase64(this string svgFile, out string base64)
    {
        try
        {
            base64 = ConvertToBase64(svgFile);
            return true;
        }
        catch (Exception exception) when (exception.IsNotCritical())
        {
            base64 = string.Empty;
            return false;
        }
    }
}