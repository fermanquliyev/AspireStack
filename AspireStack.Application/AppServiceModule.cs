using AspireStack.Application.AppService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application
{
    public static class AppServiceModule
    {
        public static void RegisterAppServices(this IServiceCollection services)
        {
            // Register app services here
            var appServiceTypes = typeof(AspireAppService).Assembly.GetTypes()
                    .Where(t => typeof(AspireAppService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var appServiceType in appServiceTypes)
            {
                services.AddScoped(appServiceType);
            }
        }
    }
}
