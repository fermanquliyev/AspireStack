using Microsoft.OpenApi.Any;
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
            var methodInfo = methodInfos.FirstOrDefault(x => x.ToString().Contains("AspireStack")) ?? methodInfos.FirstOrDefault();

            if (methodInfo != null)
            {
                AddSwaggerParameters(operation, methodInfo, DynamicRouteMapper.GetHttpMethod(methodInfo.Name));
            }
        }

        private static OpenApiOperation AddSwaggerParameters(OpenApiOperation op, MethodInfo method, string httpMethod)
        {
            // Add method parameters to Swagger
            var parameters = method.GetParameters();
            if (parameters.Length == 0) return op;
            if (httpMethod == "GET")
            {

                foreach (var param in parameters)
                {
                    var openApiParam = new OpenApiParameter()
                    {
                        Name = param.Name,
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new OpenApiSchema()
                        {
                            Type = GetOpenApiType(param.ParameterType),
                            Default = GetOpenApiTypeDefault(param.ParameterType)
                        }
                    };
                    op.Parameters.Add(openApiParam);
                }
            }
            else
            {
                op.RequestBody = new OpenApiRequestBody()
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties = parameters.GetType().GetProperties().ToDictionary(p => p.Name, p => new OpenApiSchema()
                                {
                                    Type = GetOpenApiType(p.PropertyType),
                                    Default = GetOpenApiTypeDefault(p.PropertyType)
                                })
                            }
                        }
                    }
                };
            }

            var returnType = method.ReturnType;

            // If the return type is Task<T>, set the return type to T
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            else if (returnType == typeof(Task))
            {
                returnType = typeof(object);
            }


            op.Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                                {
                                    ["message"] = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        Default = new OpenApiString("Success")
                                    },
                                    ["data"] = new OpenApiSchema()
                                    {
                                        Type = "object",
                                        Properties = returnType.GetProperties().ToDictionary(p => p.Name, p => new OpenApiSchema()
                                        {
                                            Type = GetOpenApiType(p.PropertyType),
                                            Default = GetOpenApiTypeDefault(p.PropertyType)
                                        })
                                    },
                                    ["statuscode"] = new OpenApiSchema()
                                    {
                                        Type = "integer",
                                        Default = new OpenApiInteger(200)
                                    },
                                    ["success"] = new OpenApiSchema()
                                    {
                                        Type = "boolean",
                                        Default = new OpenApiBoolean(true)
                                    }
                                }
                            }
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
                            Schema = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                                {
                                    ["message"] = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        Default = new OpenApiString("Internal Server Error")
                                    },
                                    ["data"] = new OpenApiSchema()
                                    {
                                        Type = "object",
                                        Default = new OpenApiObject()
                                    },
                                    ["statuscode"] = new OpenApiSchema()
                                    {
                                        Type = "integer",
                                        Default = new OpenApiInteger(500)
                                    },
                                    ["success"] = new OpenApiSchema()
                                    {
                                        Type = "boolean",
                                        Default = new OpenApiBoolean(false)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return op;
        }

        private static string GetOpenApiType(Type type)
        {
            // List of valid JSON schema types
            var validTypes = new Dictionary<Type, string>
            {
                { typeof(string), "string" },
                { typeof(int), "integer" },
                { typeof(int?), "integer" },
                { typeof(long), "integer" },
                { typeof(long?), "integer" },
                { typeof(byte), "integer" },
                { typeof(byte?), "integer" },
                { typeof(float), "number" },
                { typeof(float?), "number" },
                { typeof(double), "number" },
                { typeof(double?), "number" },
                { typeof(bool), "boolean" },
                { typeof(bool?), "boolean" },
                { typeof(Guid), "string" },
                { typeof(Guid?), "string" },
                { typeof(DateTime), "string" },
                { typeof(DateTime?), "string" },
                { typeof(TimeSpan), "string" },
                { typeof(TimeSpan?), "string" },
                { typeof(object), "object" }
            };

            // Return the corresponding JSON schema type or "object" for others
            return validTypes.TryGetValue(type, out var schemaType) ? schemaType : "object";
        }

        private static IOpenApiAny GetOpenApiTypeDefault(Type type)
        {
            // List of valid JSON schema types
            var validTypes = new Dictionary<Type, IOpenApiAny>
            {
                { typeof(string), new OpenApiString("string") },
                { typeof(int), new OpenApiInteger(1) },
                { typeof(int?), new OpenApiInteger(1) },
                { typeof(long), new OpenApiLong(1L) },
                { typeof(long?), new OpenApiLong(1L) },
                { typeof(byte), new OpenApiByte(1) },
                { typeof(byte?), new OpenApiByte(1) },
                { typeof(float), new OpenApiFloat(1.0f) },
                { typeof(float?), new OpenApiFloat(1.0f) },
                { typeof(double), new OpenApiDouble(1.0) },
                { typeof(double?), new OpenApiDouble(1.0) },
                { typeof(bool), new OpenApiBoolean(false) },
                { typeof(bool?), new OpenApiBoolean(false) },
                { typeof(Guid), new OpenApiString("8227e1f9-8f13-4325-85a8-1068fe0771b7") },
                { typeof(Guid?), new OpenApiString("8227e1f9-8f13-4325-85a8-1068fe0771b7") },
                { typeof(DateTime), new OpenApiDateTime(DateTime.Now) },
                { typeof(DateTime?), new OpenApiDateTime(DateTime.Now) },
                { typeof(TimeSpan), new OpenApiString(TimeSpan.MaxValue.ToString()) },
                { typeof(TimeSpan?), new OpenApiString(TimeSpan.MaxValue.ToString()) },
                { typeof(object), new OpenApiObject() }
            };
            // Return the corresponding JSON schema type or a new OpenApiObject for others
            return validTypes.TryGetValue(type, out var schemaType) ? schemaType : new OpenApiObject();
        }
    }
}
