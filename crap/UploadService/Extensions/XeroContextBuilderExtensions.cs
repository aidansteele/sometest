using Microsoft.AspNetCore.Builder;
using UploadService.Middlewares.XeroContext;

namespace UploadService.Extensions
{
    public static class XeroContextBuilderExtensions
    {
        public static IApplicationBuilder UseXeroContextBuilder(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<XeroContextBuilder>();
            return appBuilder;
        }
    }
}
