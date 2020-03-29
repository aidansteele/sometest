using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace UploadService.Converters
{
    public static class HttpRequestMessageConverter
    {
        public static IDictionary<string, object> ConvertToDictionary(this HttpRequest request)
        {
            var uri = request.GetUri();

            return new Dictionary<string, object>
            {
                ["Method"] = request.Method,
                ["Path"] = uri.AbsolutePath,
                ["Host"] = uri.Host,
                ["Uri"] = uri.AbsoluteUri,
                ["Headers"] = request.Headers?.ToDictionary()
            };
        }
    }
}
