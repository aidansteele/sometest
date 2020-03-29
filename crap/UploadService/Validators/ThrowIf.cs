using System.Net;
using UploadService.Models.Exceptions;

namespace UploadService.Validators
{
    public static class ThrowIf
    {
        public static bool ThrowUploadExceptionIfTrue(this bool b, string msg, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            if (b)
            {
                throw new UploadException(statusCode, msg);
            }

            return b;
        }

        public static bool ThrowUploadExceptionIfFalse(this bool b, string msg, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return ThrowUploadExceptionIfTrue(!b, msg, statusCode);
        }

        public static T ThrowUploadExceptionIfNull<T>(
            this T t,
            string msg,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            if (t == null)
            {
                throw new UploadException(statusCode, msg);
            }

            return t;
        }
    }
}
