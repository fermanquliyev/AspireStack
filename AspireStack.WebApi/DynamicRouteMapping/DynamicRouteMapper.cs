using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace AspireStack.WebApi.DynamicRouteMapping
{
    internal static class DynamicRouteMapper
    {
        /// <summary>
        /// Registers dynamic routes for all services implementing IAppService.
        /// </summary>
        /// <param name="app"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void RegisterDynamicRoutes(this WebApplication app)
        {
            // Scan for all services implementing IAppService
            var appServiceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IAppService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var serviceType in appServiceTypes)
            {
                // Get service methods
                var notMappedAttribute = serviceType.GetCustomAttribute<NotMappedAttribute>();
                if (notMappedAttribute != null)
                {
                    continue;
                }

                var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.DeclaringType == serviceType);
                var serviceAuthAttribute = serviceType.GetCustomAttribute<AppServiceAuthorizeAttribute>();
                var allowAnonymous = serviceAuthAttribute == null || serviceType.GetCustomAttribute<AppServiceAllowAnonymousAttribute>() != null;

                foreach (var method in methods)
                {
                    notMappedAttribute = method.GetCustomAttribute<NotMappedAttribute>();
                    if (notMappedAttribute != null)
                    {
                        continue;
                    }
                    var httpMethodAttribute = method.GetCustomAttribute<AspireHttpMethodAttribute>();
                    var httpMethod = httpMethodAttribute?.Method.Method ?? GetHttpMethod(method.Name); // Determine HTTP method
                    var controllerName = httpMethodAttribute?.ControllerName ?? serviceType.Name.Replace("AppService", "");
                    var actionName = httpMethodAttribute?.ActionName ?? method.Name.Replace("Async", "");
                    var route = $"/{controllerName}/{actionName}";
                    var methodAuthAttribute = method.GetCustomAttribute<AppServiceAuthorizeAttribute>();
                    var methodAllowAnonymous = (allowAnonymous && methodAuthAttribute == null) || method.GetCustomAttribute<AppServiceAllowAnonymousAttribute>() != null;
                    var methodDelegate = CreateEndpointDelegate(app, serviceType, method);
                    RouteHandlerBuilder routeHandler = httpMethod switch
                    {
                        "GET" => app.MapGet(route, methodDelegate).WithTags(controllerName).WithMetadata(method),
                        "POST" => app.MapPost(route, methodDelegate).WithTags(controllerName).WithMetadata(method),
                        "PUT" => app.MapPut(route, methodDelegate).WithTags(controllerName).WithMetadata(method),
                        "DELETE" => app.MapDelete(route, methodDelegate).WithTags(controllerName).WithMetadata(method),
                        _ => throw new InvalidOperationException($"Unsupported HTTP method for {method.Name}"),
                    };
                    if (!methodAllowAnonymous)
                    {
                        var policy = methodAuthAttribute?.Policy ?? serviceAuthAttribute?.Policy;
                        if (policy != null)
                        {
                            routeHandler.RequireAuthorization(policy).WithMetadata("RequireAuthorization");
                        }
                        else
                        {
                            routeHandler.RequireAuthorization().WithMetadata("RequireAuthorization");
                        }
                    }
                    else
                    {
                        routeHandler.AllowAnonymous();
                    }
                }
            }
        }



        /// <summary>
        /// Determines the HTTP method based on the method name.
        /// </summary>
        /// <param name="methodName">The name of the method to evaluate.</param>
        /// <returns>A string representing the HTTP method (GET, POST, PUT, DELETE).</returns>
        internal static string GetHttpMethod(string methodName) => methodName switch
        {
            var name when name.Contains("Create", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Insert", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Post", StringComparison.OrdinalIgnoreCase) => "POST",

            var name when name.Contains("Get", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Retrieve", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("List", StringComparison.OrdinalIgnoreCase) => "GET",

            var name when name.Contains("Update", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Change", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Put", StringComparison.OrdinalIgnoreCase) => "PUT",

            var name when name.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
                          name.Contains("Remove", StringComparison.OrdinalIgnoreCase) => "DELETE",

            _ => "POST"
        };

        private static Delegate CreateEndpointDelegate(WebApplication app, Type serviceType, MethodInfo method)
        {
            var a = async (HttpContext context) =>
            {
                using var scope = app.Services.CreateScope();
                // Resolve the service from DI
                var service = scope.ServiceProvider.GetRequiredService(serviceType);
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(serviceType);
                var dbcontext = scope.ServiceProvider.GetRequiredService<DbContext>();
                var unitOFWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Deserialize parameters from the HTTP request
                var parameters = method.GetParameters();
                var args = new List<object>();


                foreach (var param in parameters)
                {
                    if (context.Request.Method == "GET" || context.Request.Method == "DELETE")
                    {
                        // Query string for GET/DELETE
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
                        // Validate the model
                        var validationResults = new List<ValidationResult>();
                        var validationContext = new ValidationContext(body);
                        if (!Validator.TryValidateObject(body, validationContext, validationResults, true))
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsJsonAsync(new WebApiResult<string[]>
                            {
                                Message = "Validation failed",
                                StatusCode = 400,
                                Success = false,
                                Data = validationResults.Select(x=>$"{x.ErrorMessage}, Member names: {String.Join(",",x.MemberNames)}").ToArray()
                            });
                            return;
                        }
                        args.Add(body!);
                    }
                }

                using var transaction = await dbcontext.Database.BeginTransactionAsync();
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

                    // Save changes and commit transaction if SaveChanges or SaveChangesAsync was called
                    if (dbcontext.ChangeTracker.HasChanges())
                    {
                        await dbcontext.SaveChangesAsync();
                    }
                    await transaction.CommitAsync();

                    // Return response
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsJsonAsync(result);
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
            return a;
        }

        private static void AddQueryParamToMethodArgs(List<object> args, ParameterInfo param, string? value)
        {
            object? parsedValue = param.ParameterType switch
            {
                Type t when t == typeof(Guid) || t == typeof(Guid?) => Guid.TryParse(value?.Replace("\"",""), out var guidValue) ? guidValue : (t == typeof(Guid?) ? (Guid?)null : Guid.Empty),
                Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(value?.Replace("\"", ""), out var intValue) ? intValue : (t == typeof(int?) ? (int?)null : 0),
                Type t when t == typeof(double) || t == typeof(double?) => double.TryParse(value?.Replace("\"", ""), out var doubleValue) ? doubleValue : (t == typeof(double?) ? (double?)null : 0.0),
                Type t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(value?.Replace("\"", ""), out var boolValue) ? boolValue : (t == typeof(bool?) ? (bool?)null : false),
                Type t when t == typeof(DateTime) || t == typeof(DateTime?) => DateTime.TryParse(value?.Replace("\"", ""), out var dateTimeValue) ? dateTimeValue : (t == typeof(DateTime?) ? (DateTime?)null : DateTime.MinValue),
                Type t when t == typeof(DateOnly) || t == typeof(DateOnly?) => DateOnly.TryParse(value?.Replace("\"", ""), out var dateOnlyValue) ? dateOnlyValue : (t == typeof(DateOnly?) ? (DateOnly?)null : DateOnly.MinValue),
                Type t when t == typeof(TimeSpan) || t == typeof(TimeSpan?) => TimeSpan.TryParse(value?.Replace("\"", ""), out var timeSpanValue) ? timeSpanValue : (t == typeof(TimeSpan?) ? (TimeSpan?)null : TimeSpan.MinValue),
                Type t when t == typeof(float) || t == typeof(float?) => float.TryParse(value?.Replace("\"", ""), out var floatValue) ? floatValue : (t == typeof(float?) ? (float?)null : 0f),
                Type t when t == typeof(long) || t == typeof(long?) => long.TryParse(value?.Replace("\"", ""), out var longValue) ? longValue : (t == typeof(long?) ? (long?)null : 0L),
                Type t when t == typeof(short) || t == typeof(short?) => short.TryParse(value?.Replace("\"", ""), out var shortValue) ? shortValue : (t == typeof(short?) ? (short?)null : (short)0),
                Type t when t == typeof(byte) || t == typeof(byte?) => byte.TryParse(value?.Replace("\"", ""), out var byteValue) ? byteValue : (t == typeof(byte?) ? (byte?)null : (byte)0),
                Type t when t == typeof(char) || t == typeof(char?) => char.TryParse(value?.Replace("\"", ""), out var charValue) ? charValue : (t == typeof(char?) ? (char?)null : '\0'),
                Type t when t == typeof(decimal) || t == typeof(decimal?) => decimal.TryParse(value?.Replace("\"", ""), out var decimalValue) ? decimalValue : (t == typeof(decimal?) ? (decimal?)null : 0m),
                Type t when t == typeof(uint) || t == typeof(uint?) => uint.TryParse(value?.Replace("\"", ""), out var uintValue) ? uintValue : (t == typeof(uint?) ? (uint?)null : 0u),
                Type t when t == typeof(ulong) || t == typeof(ulong?) => ulong.TryParse(value?.Replace("\"", ""), out var ulongValue) ? ulongValue : (t == typeof(ulong?) ? (ulong?)null : 0ul),
                Type t when t == typeof(ushort) || t == typeof(ushort?) => ushort.TryParse(value?.Replace("\"", ""), out var ushortValue) ? ushortValue : (t == typeof(ushort?) ? (ushort?)null : (ushort)0),
                Type t when t == typeof(sbyte) || t == typeof(sbyte?) => sbyte.TryParse(value?.Replace("\"", ""), out var sbyteValue) ? sbyteValue : (t == typeof(sbyte?) ? (sbyte?)null : (sbyte)0),
                Type t when t == typeof(string) => value ?? string.Empty,
                _ => JsonSerializer.Deserialize(value ?? "{}", param.ParameterType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
            };

            args.Add(parsedValue);
        }
    }
}
