using AspireStack.Application.AppService;
using AspireStack.Application.Localization.DTO_s;
using AspireStack.Application.Security;
using AspireStack.Domain.Localization;
using System.Globalization;

namespace AspireStack.Application.Localization
{
    [AppServiceAllowAnonymous]
    public class LocalizationAppService: AspireAppService
    {
        private readonly ILocalizationProvider localizationProvider;

        public LocalizationAppService(ILocalizationProvider localizationProvider)
        {
            this.localizationProvider = localizationProvider;
        }

        public LocalizationResultDto GetLocalization(string culture)
        {
            var localization = localizationProvider.GetAllResources(new System.Globalization.CultureInfo(culture));
            var supportedCultures = CultureHelper.GetSupportedCultures().Select(culture => culture.Name).ToList();

            return new LocalizationResultDto
            {
                Culture = culture,
                SupportedCultures = supportedCultures,
                Resources = localization
            };
        }

        public LocalizationResultDto GetCurrentLocalization()
        {
            var localization = localizationProvider.GetAllResources(CultureInfo.CurrentCulture);
            var supportedCultures = CultureHelper.GetSupportedCultures().Select(culture => culture.Name).ToList();

            return new LocalizationResultDto
            {
                Culture = CultureInfo.CurrentCulture.Name,
                SupportedCultures = supportedCultures,
                Resources = localization
            };
        }
    }
}
