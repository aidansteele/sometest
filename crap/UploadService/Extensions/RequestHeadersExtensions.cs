using System;
using Microsoft.AspNetCore.Http.Headers;

namespace UploadService.Extensions
{
    public static class RequestHeadersExtensions
    {
        public static Guid? GetGuidValue(this RequestHeaders requestHeaders, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Can't get value from the request header because the given key is null or empty.");
            }

            if (requestHeaders == null)
            {
                return null;
            }

            try
            {
                var val = requestHeaders.Get<Guid>(key);
                if (val == default(Guid))
                {
                    return null;
                }

                return val;
            }
            catch
            {
                return null;
            }
        }
    }
}
