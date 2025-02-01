using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService
{

    /// <summary>
    /// Attribute to define the HTTP method of an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DebuggerDisplay("{ToString(),nq}")]
    public class AspireHttpMethodAttribute: Attribute
    {

        public HttpMethod Method { get; set; }
        public string? ActionName { get; }
        public string? ControllerName { get; }

        public AspireHttpMethodAttribute(HttpMethod method, string? actionName = null, string? controllerName = null)
        {
            Method = method;
            ActionName = actionName;
            ControllerName = controllerName;
        }
        public override string ToString()
        {
            return $"{Method} {ControllerName}/{ActionName}";
        }
    }
}
