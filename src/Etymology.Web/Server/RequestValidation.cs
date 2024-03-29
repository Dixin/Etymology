﻿namespace Etymology.Web.Server;

using Etymology.Common;
using Etymology.Data.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

internal static class RequestValidation
{
    private const string CookieName = nameof(Oracle);

    private const string FormFieldName = nameof(Bronze);

    private const string HeaderName = nameof(Seal);

    internal static MvcOptions AddAntiforgery(this MvcOptions options)
    {
        options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
        return options;
    }

    internal static IServiceCollection AddAntiforgery(this IServiceCollection services, Settings settings) =>
        services.AddAntiforgery(options =>
            {
                options.Cookie.Name = CookieName;
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = settings.SameSiteMode; // Default same site mode is Lax, which make the cookie not readable in 360 browser.
                options.FormFieldName = FormFieldName;
                options.HeaderName = HeaderName;
            });

    internal static IApplicationBuilder UseAntiforgery(this IApplicationBuilder application, Settings settings, IAntiforgery antiforgery, ILogger logger) =>
        application.Use(async (context, next) =>
            {
                HttpRequest request = context.Request;
                try
                {
                    string path = request.Path.Value ?? string.Empty;
                    if (settings.IndexPagePaths.Contains(path, StringComparer.OrdinalIgnoreCase))
                    {
                        // Requesting index page.
                        antiforgery.SendTokenToContext(context, settings);
                    } // Not requesting index page.
                    else if (!settings.PublicPaths.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                    {
                        (bool isValid, string message) = request.IsValid(settings);
                        if (!isValid)
                        {
                            logger.LogError("Request {method} {uri} from {ipAddress} is invalid. {message}", request.Method, request.GetDisplayUrl(), context.GetIPAddress(), message);
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
#if DEBUG
                            await context.Response.WriteAsync(message);
#endif
                            return;
                        }
                    }

                    await next();
                }
                catch (Exception exception) when (exception.IsNotCritical())
                {
                    logger.LogError("Request {method} {uri} from {ipAddress} fails. {exception}", request.Method, request.GetDisplayUrl(), context.GetIPAddress(), exception);
#if DEBUG
                    await context.Response.WriteAsync(exception.ToString());
#endif
                    throw;
                }
            });

    private static void SendTokenToContext(this IAntiforgery antiforgery, HttpContext context, Settings settings)
    {
        AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append(
            tokens.FormFieldName,
            tokens.RequestToken ?? string.Empty,
            new CookieOptions() { HttpOnly = false, SameSite = settings.SameSiteMode, Secure = settings.IsHttpsOnly }); // Default same site mode is Lax, which make the cookie not readable in 360 browser.
    }

    private static (bool IsValid, string Message) IsValid(this HttpRequest request, Settings settings)
    {
        // Referrer.
        IHeaderDictionary headers = request.Headers;
        if (!headers.TryGetValue("Referer", out StringValues rawReferrer))
        {
            return (false, "Header's referrer is missing.");
        }

        string referrer = rawReferrer.ToString();
        if (!Uri.TryCreate(referrer, UriKind.Absolute, out Uri? referrerUri))
        {
            return (false, $"Header's referrer {referrer} is not valid.");
        }

        string referrerHost = referrerUri.Host;
        if (!settings.AllowedHosts.Contains(referrerHost, StringComparer.OrdinalIgnoreCase))
        {
            return (false, $"Header's referrer {referrer} host {referrerHost} is not allowed.");
        }

        // User agent.
        if (!headers.TryGetValue("User-Agent", out StringValues rawUserAgent) || string.IsNullOrWhiteSpace(rawUserAgent.ToString()))
        {
            return (false, "Header's user agent is missing.");
        }

        // Anti forgery token.
        // The public APIs validate both cookie and request. The APIs to validate only cookie is not public.
        IRequestCookieCollection cookies = request.Cookies;
        if (!cookies.TryGetValue(CookieName, out string? cookie) || string.IsNullOrWhiteSpace(cookie))
        {
            return (false, $"Cookie {CookieName} is missing.");
        }

        if (!cookies.TryGetValue(FormFieldName, out string? formField) || string.IsNullOrWhiteSpace(formField))
        {
            return (false, $"Cookie {FormFieldName} is missing.");
        }

        return (true, string.Empty);
    }

    private static string? GetIPAddress(this HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString()
        ?? context.Features.Get<IHttpConnectionFeature>().RemoteIpAddress?.ToString()
        ?? (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwarded) ? forwarded.ToString() : null)
        ?? (context.Request.Headers.TryGetValue("REMOTE_ADDR", out StringValues remote) ? remote.ToString() : null);
}