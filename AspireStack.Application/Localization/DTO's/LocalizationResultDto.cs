using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.Localization.DTO_s
{
    public class LocalizationResultDto
    {
        public required List<string> SupportedCultures { get; set; }
        public required string Culture { get; set; }
        public required Dictionary<string,string> Resources { get; set; }
    }
}
