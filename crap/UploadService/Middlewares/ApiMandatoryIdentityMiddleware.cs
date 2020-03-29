using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UploadService.Constants;
using UploadService.Errors;
using UploadService.Extensions;
using UploadService.Models;
using UploadService.Models.Exceptions;

namespace UploadService.Middlewares
{
    internal class ApiMandatoryIdentityMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiMandatoryIdentityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx, CurrentUserContext currentUserContext)
        {
            var userId = ctx.Request.GetTypedHeaders().GetGuidValue(XeroHeaderKeys.UserId);
            var orgId = ctx.Request.GetTypedHeaders().GetGuidValue(XeroHeaderKeys.TenantId);

            if (userId == null || userId == Guid.Empty)
            {
                await ctx.RespondWithUnauthorized($"UserId is mandatory. The header {XeroHeaderKeys.UserId} must be set.");
                return;
            }

            if (orgId == null || orgId == Guid.Empty)
            {
                await ctx.RespondWithUnauthorized($"OrganisationId is mandatory. The header {XeroHeaderKeys.TenantId} must be set.");
                return;
            }

            currentUserContext.UserId = (Guid)userId;
            currentUserContext.OrganisationId = (Guid)orgId;

            await _next(ctx);
        }
    }
}
