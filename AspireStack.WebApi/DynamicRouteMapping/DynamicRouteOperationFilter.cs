using AspireStack.WebApi.Host.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace AspireStack.WebApi.DynamicRouteMapping
{
    public class DynamicRouteOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the method info for the current operation
            var methodInfos = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<MethodInfo>().ToList();
            var methodInfo = methodInfos.FirstOrDefault(x => x.DeclaringType?.FullName?.Contains("AspireStack.Application") ?? false);
            
            var requireAuthorization = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<string>().ToList().Any(x => x == "RequireAuthorization");


            if (methodInfo != null)
            {
                AddSwaggerParameters(operation, methodInfo, DynamicRouteMapper.GetHttpMethod(methodInfo.Name), true, context);
            }
        }

        private static OpenApiOperation AddSwaggerParameters(OpenApiOperation op, MethodInfo method, string httpMethod, bool requireAuthorization, OperationFilterContext context)
        {
            // Add method parameters to Swagger
            var parameters = method.GetParameters();
            if (parameters is not null && parameters.Length > 0)
            {
                if (httpMethod == "GET")
                {
                    foreach (var param in parameters)
                    {
                        var isRequired = !param.ParameterType.IsGenericType || param.ParameterType.GetGenericTypeDefinition() != typeof(Nullable<>);
                        if (param.HasDefaultValue)
                        {
                            isRequired = false;
                        }

                        var openApiParam = new OpenApiParameter()
                        {
                            Name = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(param.Name),
                            In = ParameterLocation.Query,
                            Required = isRequired,
                            Schema = context.SchemaGenerator.GenerateSchema(param.ParameterType, context.SchemaRepository)
                        };
                        op.Parameters.Add(openApiParam);
                    }
                }
                else
                {
                    var parameter = parameters.FirstOrDefault();
                    if (parameter != null)
                    {
                        op.RequestBody = new OpenApiRequestBody()
                        {
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType()
                                {
                                    Schema = context.SchemaGenerator.GenerateSchema(parameter.ParameterType, context.SchemaRepository)
                                }
                            }
                        };
                    }
                }
            }
            if (requireAuthorization)
            {
                op.Description += " This operation requires authorization. ASPSTK.";
            }

            var returnType = method.ReturnType;

            // If the return type is Task<T>, set the return type to T
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            else if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }

            op.Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Description = "OkResult",
                    Content = returnType != typeof(void) ? new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(returnType, context.SchemaRepository)
                        }
                    }: null
                },
                ["400"] = new OpenApiResponse()
                {
                    Description = "Bad Request",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(WebApiResult<string[]>), context.SchemaRepository),
                        }
                    }
                },
                ["500"] = new OpenApiResponse()
                {
                    Description = "Internal Server Error",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(WebApiResult<string>), context.SchemaRepository)
                        }
                    }
                }
            };

            if (requireAuthorization)
            {
                op.Responses["401"] = new OpenApiResponse()
                {
                    Description = "Unauthorized",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(WebApiResult<string>), context.SchemaRepository)
                        }
                    }
                };
            }

            return op;
        }
    }
}
