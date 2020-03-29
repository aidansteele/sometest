using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UploadService.Extensions;
using UploadService.Models;

namespace UploadService.Middlewares.XeroContext
{
    public class XeroContextBuilder
    {
        private readonly RequestDelegate _next;

        public XeroContextBuilder(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var xeroContext = new Models.XeroContext
            {
                RequestId = httpContext.ExtractRequestId()
            };

            var xeroContextProvider = httpContext.RequestServices.GetService<IXeroContextProvider>();
            xeroContextProvider.Save(xeroContext);
            await _next.Invoke(httpContext);
        }
    }
}
