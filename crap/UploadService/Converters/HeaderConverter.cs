using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace UploadService.Converters
{
    public static class HeaderConverter
    {
        public static IDictionary<string, string> ToDictionary(this IHeaderDictionary headerDictionary)
        {
            var newDict = new Dictionary<string, string>();

            foreach (var keyValuePair in headerDictionary)
            {
                newDict.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return newDict;
        }
    }
}
