using GeoPlanner.Application.AppService;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace GeoPlanner.WebApi.DynamicRouteMapping
{
    public static class DynamicRouteMapper
    {
        public static void RegisterDynamicRoutes(WebApplication app)
        {
            // Scan for all services implementing IAppService
            var appServiceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IAppService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var serviceType in appServiceTypes)
            {
                // Get service methods
                var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    var httpMethod = GetHttpMethod(method.Name); // Determine HTTP method
                    var serviceName = serviceType.Name.Replace("AppService", "");
                    var route = $"/{serviceName}/{method.Name}";

                    // Register the route dynamically based on HTTP method
                    switch (httpMethod)
                    {
                        case "GET":
                            var getDelegate = CreateEndpointDelegate(app, serviceType, method);
                            var getRoute = app.MapGet(route, getDelegate).WithTags(serviceName).WithMetadata(method);
                            break;
                        case "POST":
                            var postDelegate = CreateEndpointDelegate(app, serviceType, method);
                            var postRoute = app.MapPost(route, postDelegate).WithTags(serviceName).WithMetadata(method);
                            break;
                        case "PUT":
                            var putDelegate = CreateEndpointDelegate(app, serviceType, method);
                            var putRoute = app.MapPut(route, putDelegate).WithTags(serviceName).WithMetadata(method);
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported HTTP method for {method.Name}");
                    }
                }
            }
        }

        public static string GetHttpMethod(string methodName)
        {
            // Define naming conventions for HTTP methods
            if (methodName.Contains("Create", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("Insert", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("Post", StringComparison.OrdinalIgnoreCase))
            {
                return "POST";
            }

            if (methodName.Contains("Get", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("Retrieve", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("List", StringComparison.OrdinalIgnoreCase))
            {
                return "GET";
            }

            if (methodName.Contains("Update", StringComparison.OrdinalIgnoreCase) ||
                methodName.Contains("Change", StringComparison.OrdinalIgnoreCase))
            {
                return "PUT";
            }

            return "POST"; // Default to POST
        }

        private static Delegate CreateEndpointDelegate(WebApplication app, Type serviceType, MethodInfo method)
        {
            return async (HttpContext context) =>
            {
                using var scope = app.Services.CreateScope();
                // Resolve the service from DI
                var service = scope.ServiceProvider.GetRequiredService(serviceType);
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(serviceType);
                using var dbcontext = scope.ServiceProvider.GetRequiredService<DbContext>();
                using var transaction = dbcontext.Database.BeginTransaction();

                // Deserialize parameters from the HTTP request
                var parameters = method.GetParameters();
                var args = new List<object>();

                foreach (var param in parameters)
                {
                    if (context.Request.Method == "GET")
                    {
                        // Query string for GET
                        var value = context.Request.Query[param.Name ?? string.Empty].FirstOrDefault();
                        AddQueryParamToMethodArgs(args, param, value);
                    }
                    else
                    {
                        // JSON body for POST/PUT
                        var body = await JsonSerializer.DeserializeAsync(context.Request.Body, param.ParameterType, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        args.Add(body!);
                    }
                }

                try
                {
                    // Invoke the service method
                    var result = method.Invoke(service, args.ToArray());

                    // Handle async methods
                    if (result is Task task)
                    {
                        await task;
                        var returnProperty = task.GetType().GetProperty("Result");
                        result = returnProperty?.GetValue(task);
                    }

                    if (dbcontext.ChangeTracker.HasChanges())
                    {
                        await transaction.CommitAsync();
                    }
                    // Return response
                    await context.Response.WriteAsJsonAsync(new WebApiResult
                    {
                        Data = result,
                        StatusCode = 200,
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    logger.LogError(ex, "An error occurred while processing the request");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new WebApiResult
                    {
                        Message = "An error occurred while processing the request",
                        Success = false,
                        StatusCode = 500
                    });
                }
            };
        }

        private static void AddQueryParamToMethodArgs(List<object> args, ParameterInfo param, string? value)
        {
            if (param.ParameterType == typeof(Guid) || param.ParameterType == typeof(Guid?))
            {
                args.Add(Guid.TryParse(value, out var guidValue) ? guidValue : (param.ParameterType == typeof(Guid?) ? (Guid?)null : Guid.Empty));
            }
            else if (param.ParameterType == typeof(int) || param.ParameterType == typeof(int?))
            {
                args.Add(int.TryParse(value, out var intValue) ? intValue : (param.ParameterType == typeof(int?) ? (int?)null : 0));
            }
            else if (param.ParameterType == typeof(double) || param.ParameterType == typeof(double?))
            {
                args.Add(double.TryParse(value, out var doubleValue) ? doubleValue : (param.ParameterType == typeof(double?) ? (double?)null : 0.0));
            }
            else if (param.ParameterType == typeof(bool) || param.ParameterType == typeof(bool?))
            {
                args.Add(bool.TryParse(value, out var boolValue) ? boolValue : (param.ParameterType == typeof(bool?) ? (bool?)null : false));
            }
            else if (param.ParameterType == typeof(DateTime) || param.ParameterType == typeof(DateTime?))
            {
                args.Add(DateTime.TryParse(value, out var dateTimeValue) ? dateTimeValue : (param.ParameterType == typeof(DateTime?) ? (DateTime?)null : DateTime.MinValue));
            }
            else if (param.ParameterType == typeof(TimeSpan) || param.ParameterType == typeof(TimeSpan?))
            {
                args.Add(TimeSpan.TryParse(value, out var timeValue) ? timeValue : (param.ParameterType == typeof(TimeSpan?) ? (TimeSpan?)null : TimeSpan.MinValue));
            }
            else if (param.ParameterType == typeof(string))
            {
                args.Add(value ?? string.Empty);
            }
            else if (param.ParameterType == typeof(float) || param.ParameterType == typeof(float?))
            {
                args.Add(float.TryParse(value, out var floatValue) ? floatValue : (param.ParameterType == typeof(float?) ? (float?)null : 0f));
            }
            else if (param.ParameterType == typeof(long) || param.ParameterType == typeof(long?))
            {
                args.Add(long.TryParse(value, out var longValue) ? longValue : (param.ParameterType == typeof(long?) ? (long?)null : 0L));
            }
            else if (param.ParameterType == typeof(short) || param.ParameterType == typeof(short?))
            {
                args.Add(short.TryParse(value, out var shortValue) ? shortValue : (param.ParameterType == typeof(short?) ? (short?)null : (short)0));
            }
            else if (param.ParameterType == typeof(byte) || param.ParameterType == typeof(byte?))
            {
                args.Add(byte.TryParse(value, out var byteValue) ? byteValue : (param.ParameterType == typeof(byte?) ? (byte?)null : (byte)0));
            }
            else if (param.ParameterType == typeof(char) || param.ParameterType == typeof(char?))
            {
                args.Add(char.TryParse(value, out var charValue) ? charValue : (param.ParameterType == typeof(char?) ? (char?)null : '\0'));
            }
            else if (param.ParameterType == typeof(decimal) || param.ParameterType == typeof(decimal?))
            {
                args.Add(decimal.TryParse(value, out var decimalValue) ? decimalValue : (param.ParameterType == typeof(decimal?) ? (decimal?)null : 0m));
            }
            else if (param.ParameterType == typeof(uint) || param.ParameterType == typeof(uint?))
            {
                args.Add(uint.TryParse(value, out var uintValue) ? uintValue : (param.ParameterType == typeof(uint?) ? (uint?)null : 0u));
            }
            else if (param.ParameterType == typeof(ulong) || param.ParameterType == typeof(ulong?))
            {
                args.Add(ulong.TryParse(value, out var ulongValue) ? ulongValue : (param.ParameterType == typeof(ulong?) ? (ulong?)null : 0ul));
            }
            else if (param.ParameterType == typeof(ushort) || param.ParameterType == typeof(ushort?))
            {
                args.Add(ushort.TryParse(value, out var ushortValue) ? ushortValue : (param.ParameterType == typeof(ushort?) ? (ushort?)null : (ushort)0));
            }
            else if (param.ParameterType == typeof(sbyte) || param.ParameterType == typeof(sbyte?))
            {
                args.Add(sbyte.TryParse(value, out var sbyteValue) ? sbyteValue : (param.ParameterType == typeof(sbyte?) ? (sbyte?)null : (sbyte)0));
            }
            else
            {
                args.Add(Convert.ChangeType(value, param.ParameterType));
            }
        }
    }
}
