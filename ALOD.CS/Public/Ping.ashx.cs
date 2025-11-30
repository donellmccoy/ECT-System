using System;
using System.Web;

namespace ALOD
{
    /// <summary>
    /// Summary description for Ping
    /// </summary>
    public class Ping : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(DateTime.Now.Ticks);
        }
    }
}
