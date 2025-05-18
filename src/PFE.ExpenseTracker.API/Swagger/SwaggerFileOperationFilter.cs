using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PFE.ExpenseTracker.API.Swagger
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody?.Content.Any(x => x.Key.Equals(fileUploadMime)) == true)
            {
                var fileParams = context.MethodInfo.GetParameters()
                    .Where(p => p.ParameterType == typeof(IFormFile));
                
                operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                    fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "binary"
                    });
            }
        }
    }
}
