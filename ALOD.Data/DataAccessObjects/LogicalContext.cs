using System.Runtime.Remoting.Messaging;
using System.ServiceModel.Channels;
using System.Web;

namespace ALOD.Data
{
    /// <summary>
    /// Provides context-aware session management that works in both web and non-web (e.g., unit testing) contexts.
    /// Uses <see cref="HttpContext"/> for web contexts and <see cref="CallContext"/> for WinForms/testing contexts.
    /// </summary>
    /// <remarks>
    /// This allows the session manager to function correctly in both ASP.NET web applications
    /// and unit testing environments without requiring different implementations.
    /// Discussion: http://forum.springframework.net/showthread.php?t=572
    /// </remarks>
    public class LogicalContext
    {
        /// <summary>
        /// Gets a session object from the appropriate context storage.
        /// Uses <see cref="HttpContext"/> if in a web context, otherwise uses <see cref="CallContext"/>.
        /// </summary>
        /// <param name="key">The key identifying the session object.</param>
        /// <returns>The session object associated with the key, or null if not found.</returns>
        public static object GetSession(string key)
        {
            if (IsInWebContext())
            {
                return (ISession)HttpContext.Current.Items[key];
            }
            else
            {
                return (ISession)CallContext.GetData(key);
            }
        }

        /// <summary>
        /// Determines whether the code is currently executing in a web context.
        /// </summary>
        /// <returns>True if executing in a web context (<see cref="HttpContext.Current"/> is not null); otherwise, false.</returns>
        public static bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        /// <summary>
        /// Stores a session object in the appropriate context storage.
        /// Uses <see cref="HttpContext"/> if in a web context, otherwise uses <see cref="CallContext"/>.
        /// </summary>
        /// <param name="key">The key to associate with the session object.</param>
        /// <param name="value">The session object to store.</param>
        public static void SetSession(string key, object value)
        {
            if (IsInWebContext())
            {
                HttpContext.Current.Items[key] = value;
            }
            else
            {
                CallContext.SetData(key, value);
            }
        }
    }
}