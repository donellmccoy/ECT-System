using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Abstract base class for data service implementations.
    /// Provides common functionality for managing session state in both web and non-web contexts.
    /// </summary>
    public abstract class DataService
    {
        /// <summary>
        /// Gets a value indicating whether the current execution context is a web context.
        /// </summary>
        protected static bool IsInWebContext
        {
            get { return HttpContext.Current != null; }
        }

        /// <summary>
        /// Retrieves an object from session state, supporting both web (HttpContext) and non-web (CallContext) scenarios.
        /// </summary>
        /// <param name="key">The session key to retrieve.</param>
        /// <returns>The session object, or null if not found.</returns>
        protected static object GetSessionObject(string key)
        {
            if (IsInWebContext)
            {
                return HttpContext.Current.Session[key];
            }
            else
            {
                return CallContext.GetData(key);
            }
        }
    }
}