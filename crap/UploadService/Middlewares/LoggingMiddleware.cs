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
    internal class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                LogRequest(context.Request);
                await _next.Invoke(context);
                LogResponse(context.Response, watch);
            }
            catch
            {
                await context.RespondWithError("Server error while logging.", HttpStatusCode.InternalServerError);
            }
        }

        private void LogResponse(HttpResponse response, Stopwatch watch)
        {
            using (LogContext.PushProperty("OutboundResponse", response.Headers.ToDictionary()))
            {
                _logger.LogInformation($"Processed inbound upload file request. Execution time {watch.ElapsedMilliseconds}ms.");
            }
        }

        private void LogRequest(HttpRequest request)
        {
            using (LogContext.PushProperty("InboundRequest", request.ConvertToDictionary()))
            {
                _logger.LogInformation("Serving inbound upload file request.");
            }
        }
    }
}
