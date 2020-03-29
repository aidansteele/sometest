using Microsoft.AspNetCore.Builder;
using UploadService.Middlewares;

namespace UploadService.Extensions
{
    public static class GlobalExceptionStartup
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder application)
        {
            return application.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
