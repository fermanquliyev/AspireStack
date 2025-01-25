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
            var methodInfo = methodInfos.FirstOrDefault(x => x.DeclaringType?.FullName?.Contains("AspireStack.Application") ?? false);
            methodInfo ??= methodInfos.FirstOrDefault();
            var requireAuthorization = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<string>().ToList().Any(x => x == "RequireAuthorization");

            if (methodInfo != null)
            {
                AddSwaggerParameters(operation, methodInfo, DynamicRouteMapper.GetHttpMethod(methodInfo.Name), true);
            }
        }

        private static OpenApiOperation AddSwaggerParameters(OpenApiOperation op, MethodInfo method, string httpMethod, bool requireAuthorization)
        {
            // Add method parameters to Swagger
            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                if (httpMethod == "GET")
                {

                    foreach (var param in parameters)
                    {
                        var openApiParam = new OpenApiParameter()
                        {
                            Name = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(param.Name),
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
                        Reference = new OpenApiReference()
                        {
                            Id = "requestBody",
                            Type = ReferenceType.RequestBody
                        },
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType()
                            {
                                Schema = new OpenApiSchema()
                                {
                                    Type = "object",
                                    Properties = parameters.GetType().GetProperties().ToDictionary(p => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(p.Name), p => new OpenApiSchema()
                                    {
                                        Type = GetOpenApiType(p.PropertyType),
                                        Default = GetOpenApiTypeDefault(p.PropertyType)
                                    })
                                }
                            }
                        }
                    };
                }
            }
            if (requireAuthorization)
            {
                op.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString("Bearer <token>")
                    }
                });
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
                                        Type = IsCollectionType(returnType) ? "array" : "object",
                                        Items = IsCollectionType(returnType) ? new OpenApiSchema()
                                        {
                                            Type = GetOpenApiType(returnType.GetGenericArguments().FirstOrDefault() ?? returnType.GetElementType()),
                                            Default = GetOpenApiTypeDefault(returnType.GetGenericArguments().FirstOrDefault() ?? returnType.GetElementType()),
                                            Properties = (returnType.GetGenericArguments().FirstOrDefault() ?? returnType.GetElementType()).GetProperties().ToDictionary(p => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(p.Name), p => new OpenApiSchema()
                                            {
                                                Type = GetOpenApiType(p.PropertyType),
                                                Default = GetOpenApiTypeDefault(p.PropertyType)
                                            })
                                        } : null,
                                        Properties = IsCollectionType(returnType) ? null : returnType.GetProperties().ToDictionary(p => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(p.Name), p => new OpenApiSchema()
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

        private static bool IsCollectionType(Type returnType)
        {
            return returnType.IsGenericType && (returnType.GetGenericTypeDefinition() == typeof(List<>) || returnType.GetGenericTypeDefinition() == typeof(IList<>) || returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || returnType.IsArray;
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
