namespace Etymology.Web.Server
{
    using System;
    using System.Linq;
    using System.Net;
    using Etymology.Data.Models;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    internal static class RequestValidation
    {
        internal static MvcOptions AddAntiforgery(this MvcOptions options)
        {
            options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
            return options;
        }

        internal static IServiceCollection AddAntiforgery(this IServiceCollection services) =>
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = nameof(Oracle);
                options.FormFieldName = nameof(Bronze);
                options.HeaderName = nameof(Seal);
            });

        internal static IApplicationBuilder UseAntiforgery(this IApplicationBuilder applicationBuilder, Settings settings, IAntiforgery antiforgery) =>
            applicationBuilder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;
                if (settings.IndexPageUrls.Contains(path, StringComparer.OrdinalIgnoreCase))
                {
                    antiforgery.SendTokenToContext(context);
                }
                else if (!settings.ExposedPaths.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) 
                    && !context.Request.IsValid(settings))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }
                await next();
            });

        private static void SendTokenToContext(this IAntiforgery antiforgery, HttpContext context)
        {
            AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append(tokens.FormFieldName, tokens.RequestToken, new CookieOptions() { HttpOnly = false });
        }

        private static bool IsValid(this HttpRequest request, Settings settings)
        {
            IHeaderDictionary headers = request.Headers;
            IRequestCookieCollection cookies = request.Cookies;
            return 
                // Referer.
                Uri.TryCreate(headers["Referer"], UriKind.Absolute, out Uri refererUri)
                && settings.RefererHosts.Contains(refererUri.Host, StringComparer.OrdinalIgnoreCase)
                // Host.
                && settings.RefererHosts.Contains(request.Host.Host, StringComparer.OrdinalIgnoreCase)
                // User agent.
                && !string.IsNullOrEmpty(headers["User-Agent"])
                // Anti forgery token.
                // The public APIs validate both cookie and request. The APIs to validate cookie is not public.
                && !string.IsNullOrEmpty(cookies[nameof(Oracle)])
                && !string.IsNullOrEmpty(cookies[nameof(Bronze)]);
        }
    }
}
