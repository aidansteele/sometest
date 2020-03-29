using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using UploadService.Constants;

namespace UploadService.Middlewares
{
    public class SwaggerAuthoriseOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = XeroHeaderKeys.TenantId,
                In = "header",
                Description = "Xero Organisation ID",
                Required = true,
                Type = "string"
            });

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = XeroHeaderKeys.UserId,
                In = "header",
                Description = "Xero User ID",
                Required = true,
                Type = "string"
            });
        }
    }
}
