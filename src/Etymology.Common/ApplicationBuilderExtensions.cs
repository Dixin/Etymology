namespace Etymology.Common;

using Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEncodings(this IApplicationBuilder application)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return application;
    }
}