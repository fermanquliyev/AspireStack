using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspireStack.Domain.Localization
{
    public class LocalizationInitializer
    {
        public static void Initialize(ILocalizationProvider localizationProvider, string appName= "AspireStack")
        {
            var assembly = typeof(LocalizationInitializer).Assembly;
            var resourceNames = assembly.GetManifestResourceNames().Where(f=>f.EndsWith("json"));
            foreach (var file in resourceNames)
            {
                var jsonString = assembly.GetManifestResourceStream(file);
                var jsonDocument = JsonDocument.Parse(jsonString);
                var fileCulture = jsonDocument.RootElement.GetProperty("culture").GetString();
                var values = jsonDocument.RootElement.GetProperty("values").EnumerateObject();

                foreach (var value in values)
                {
                    var key = value.Name;
                    var val = value.Value.GetString();
                    localizationProvider.AddResource(new CultureInfo(fileCulture), key, val);
                }
            }
        }
    }
}
