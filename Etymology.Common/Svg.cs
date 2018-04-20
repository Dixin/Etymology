namespace Etymology.Common
{
    using System;
    using System.Text;
    using System.Xml.Linq;

    public static class Svg
    {
        public static string ConvertToBase64(this string svg)
        {
            XDocument svgDocument = XDocument.Parse(svg);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(svgDocument.Root.ToString(SaveOptions.DisableFormatting)));
        }

        public static bool TryConvertToBase64(this string svg, out string base64)
        {
            try
            {
                base64 = ConvertToBase64(svg);
                return true;
            }
            catch (Exception exception) when (exception.IsNotCritical())
            {
                base64 = string.Empty;
                return false;
            }
        }
    }
}
