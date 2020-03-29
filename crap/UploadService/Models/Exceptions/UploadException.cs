using System;
using System.Net;

namespace UploadService.Models.Exceptions
{
    public class UploadException : Exception
    {
        public UploadException(int httpStatusCode)
        {
            this.StatusCode = httpStatusCode;
        }

        public UploadException(HttpStatusCode httpStatusCode)
        {
            this.StatusCode = (int)httpStatusCode;
        }

        public UploadException(int httpStatusCode, string message)
            : base(message)
        {
            this.StatusCode = httpStatusCode;
        }

        public UploadException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            this.StatusCode = (int)httpStatusCode;
        }

        public UploadException(int httpStatusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = httpStatusCode;
        }

        public UploadException(HttpStatusCode httpStatusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = (int)httpStatusCode;
        }

        public int StatusCode { get; }
    }
}
