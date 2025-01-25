using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.Security
{
    /// <summary>
    /// Specifies that the class or method that this attribute is applied to does not require authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DebuggerDisplay("{ToString(),nq}")]
    public class AppServiceAllowAnonymousAttribute : Attribute
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return "AllowAnonymous";
        }
    }
}
