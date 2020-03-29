using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UploadService.Constants;
using UploadService.Models.Exceptions;

namespace UploadService.Errors
{
    public static class ErrorResponder
    {
        public static Task RespondWithUnauthorized(this HttpContext ctx, string message)
        {
            return ctx.RespondWithError(message, HttpStatusCode.Unauthorized);
        }

        public static ErrorResponse CreateErrorResponseModel(this HttpContext ctx, string message)
        {
            var correlationId = ctx.Request.Headers[XeroHeaderKeys.CorrelationId].FirstOrDefault();
            return new ErrorResponse()
            {
                ErrorMessage = message,
                ErrorId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId,
            };
        }

        public static async Task RespondWithError(this HttpContext ctx, string message, HttpStatusCode statusCode)
        {
            var responseBody = JsonConvert.SerializeObject(ctx.CreateErrorResponseModel(message));

            ctx.Response.StatusCode = (int)statusCode;
            var data = Encoding.UTF8.GetBytes(responseBody);
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}
