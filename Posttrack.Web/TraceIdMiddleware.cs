using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Posttrack.Web
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;
        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            await _next.Invoke(context);
        }
    }

    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseTraceId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TraceIdMiddleware>();
        }
    }
}