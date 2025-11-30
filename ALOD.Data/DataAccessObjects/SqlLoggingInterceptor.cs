using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Diagnostics;

namespace ALOD.Data
{
    /// <summary>
    /// Custom NHibernate interceptor for logging SQL with parameter values substituted
    /// WARNING: This is for debugging only - reduces security and performance
    /// </summary>
    public class SqlLoggingInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            // Log the original parameterized SQL
            Debug.WriteLine($"[NHibernate SQL] {sql}");
            
            // You could add logic here to substitute parameters for debugging
            // But this is NOT recommended for production due to security concerns
            
            return base.OnPrepareStatement(sql);
        }
    }
}