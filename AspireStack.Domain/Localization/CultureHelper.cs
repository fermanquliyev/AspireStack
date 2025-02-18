using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Localization
{
    public static class CultureHelper
    {
        public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        public static CultureInfo GetCultureInfoByChecking(string name)
        {
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                return DefaultCulture;
            }
        }

        public static CultureInfo[] GetSupportedCultures()
        {
            var supportedCultures = new[] { "en-US", "fr-FR", "tr-TR", "ru-RU", "az-AZ" };
            return supportedCultures.Select(c => new CultureInfo(c)).ToArray();
        }

        public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("en-US");
    }
}
