using System.Collections.Generic;
using System.Reflection;

namespace ALOD.Core.Domain
{
    /// <summary>
    /// This serves as a base interface for <see cref="EntityWithTypedId"/> and
    /// <see cref="Entity"/>.
    /// </summary>
    internal interface IEntityWithTypedId<Tid>
    {
        Tid Id { get; }

        IEnumerable<PropertyInfo> GetSignatureProperties();

        bool IsTransient();
    }
}