using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RateLimiter;

namespace WebApi
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RateLimiterMiddleware(RequestDelegate next, ILoggerFactory logFactory)
        {
            _next = next;
            _logger = logFactory.CreateLogger("RateLimiterMiddleware");
            _logger.LogInformation("RateLimiterMiddleware is started");
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation("IP address: " + httpContext.Connection.RemoteIpAddress.ToString());
            RateLimiterManager rateLimiterManager = RateLimiterManager.Instance;
            
            int statusCode = 0;
            rateLimiterManager.CheckIpAddress(httpContext.Connection.RemoteIpAddress.ToString(), ref statusCode);
            
            if (statusCode == 429)
                await httpContext.Response.WriteAsync("429 - Too many requests.");
            else
                await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RateLimiterMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiterMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}
