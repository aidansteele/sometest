using System;
using System.Linq;

namespace UploadService.Tests.Unit.Factory
{
    public static class FileNameFactory
    {
        public static string GenerateRandomString(this string element, int length)
        {
            return new string(Enumerable.Repeat(element, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static string GenerateFileName(this string fileNameWithoutExtension, string extension)
        {
            return $"{fileNameWithoutExtension}{extension}";
        }
    }
}
