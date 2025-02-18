using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Localization
{
    public interface ILocalizationProvider
    {
        void AddResource(CultureInfo cultureInfo, string key, string? val);
        Dictionary<string, string> GetAllResources(CultureInfo cultureInfo);
        CultureInfo GetCurrentCulture();
        string GetResource(string key, CultureInfo cultureInfo = null);
    }
}
