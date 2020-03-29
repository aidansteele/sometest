using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using UploadService.Models.Exceptions;
using Newtonsoft.Json;
using Serilog.Context;
using UploadService.Constants;
using UploadService.Converters;
using UploadService.Errors;

namespace UploadService.Middlewares
{
    internal class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (UploadException uploadException)
            {
                LogError(context.Request, uploadException);
                context.Response.StatusCode = uploadException.StatusCode;
                await WriteResponseAsync(context, uploadException);
            }
            catch (Exception e)
            {
                LogError(context.Request, e);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await WriteResponseAsync(context, e);
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var responseFeature = context.Features.Get<IHttpResponseFeature>();
            responseFeature.ReasonPhrase = exception.Message;
            var errorResponseModel = context.CreateErrorResponseModel(exception.Message);
            var bodyData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(errorResponseModel));
            await responseFeature.Body.WriteAsync(bodyData, 0, bodyData.Length);
        }

        private void LogError<T>(HttpRequest request, T e)
        {
            using (LogContext.PushProperty("OutboundResponseErrorHeaders", request.Headers.ToDictionary()))
            using (LogContext.PushProperty("OutboundResponseError", e))
            {
                _logger.LogError("Error while processing inbound upload file request.");
            }
        }
    }
}
