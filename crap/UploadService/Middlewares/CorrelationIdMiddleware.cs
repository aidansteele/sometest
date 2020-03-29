using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UploadService.Constants;
using UploadService.Extensions;
using UploadService.Models;

namespace UploadService.Middlewares
{
    internal class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var correlationId = ctx.Request.GetTypedHeaders().GetGuidValue(XeroHeaderKeys.CorrelationId);
            if (correlationId == null || correlationId == Guid.Empty)
            {
                ctx.Request.Headers.Add(XeroHeaderKeys.CorrelationId, Guid.NewGuid().ToString());
            }

            await _next(ctx);
        }
    }
}
