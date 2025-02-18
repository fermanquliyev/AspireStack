using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Localization
{
    public class LocalizationProvider : ILocalizationProvider
    {
        private readonly ConcurrentDictionary<CultureInfo, Dictionary<string, string>> _resources = new();
        public void AddResource(CultureInfo cultureInfo, string key, string? val)
        {
            if(!_resources.TryGetValue(cultureInfo, out var resources))
            {
                resources = new Dictionary<string, string>();
                _resources.TryAdd(cultureInfo, resources);
            }
            resources[key] = val;
        }

        public CultureInfo GetCurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        public string GetResource(string key, CultureInfo cultureInfo = default)
        {
            if (cultureInfo == default)
            {
                cultureInfo = CultureHelper.DefaultCulture;
            }
            if (_resources.TryGetValue(cultureInfo, out var resources))
            {
                if (resources.TryGetValue(key, out var val))
                {
                    return val;
                }
            }
            return key;
        }

        public Dictionary<string,string> GetAllResources(CultureInfo cultureInfo)
        {
            if (_resources.TryGetValue(cultureInfo, out var resources))
            {
                return resources;
            }
            return null;
        }
    }
}
