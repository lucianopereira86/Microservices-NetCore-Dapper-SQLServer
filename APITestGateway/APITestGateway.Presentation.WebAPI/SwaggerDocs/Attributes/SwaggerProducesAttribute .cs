
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace APITestGateway.Presentation.WebAPI.SwaggerDocs.Attributes
{
    /// <summary>
    /// SwaggerResponseContentTypeAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerProducesAttribute : Attribute
    {
        public SwaggerProducesAttribute(params string[] contentTypes)
        {
            this.ContentTypes = contentTypes;
        }

        public IEnumerable<string> ContentTypes { get; }
    }

    public class ProducesOperatioFilter : IOperationFilter
    {

        public void Apply(Operation operation, OperationFilterContext context)
        {


            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor != null)
            {
                var actionAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true).Where(e => e.GetType() == typeof(SwaggerProducesAttribute)).Select(x => (SwaggerProducesAttribute)x).FirstOrDefault();

                if (actionAttributes == null)
                {
                    return;
                }

                operation.Produces.Clear();
                operation.Produces = actionAttributes.ContentTypes.ToList();
            }



        }
    }
}
