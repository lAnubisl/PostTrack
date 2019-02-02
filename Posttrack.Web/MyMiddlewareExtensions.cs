using Microsoft.AspNetCore.Builder;

namespace Posttrack.Web
{
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseTraceId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TraceIdMiddleware>();
        }
    }
}