using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace UploadService.Extensions
{
    public static class HttpContextExtensions
    {
        public static string ExtractRequestId(this HttpContext httpContext)
        {
            var requestIdFeature = httpContext.Features.Get<IHttpRequestIdentifierFeature>();
            var requestId = string.Empty;
            if (requestIdFeature?.TraceIdentifier != null)
            {
                requestId = requestIdFeature.TraceIdentifier;
            }

            return requestId;
        }
    }
}
