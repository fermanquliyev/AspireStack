using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService
{
    /// <summary>
    /// Attribute to prevent this service/method to be mapped as route in API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DebuggerDisplay("{ToString(),nq}")]
    public class NotMappedAttribute: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotMappedAttribute"/> class.
        /// </summary>
        public NotMappedAttribute()
        {
            
        }
        public override string ToString()
        {
            return "NotMapped";
        }
    }
}
