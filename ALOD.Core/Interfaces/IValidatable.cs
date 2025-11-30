using ALOD.Core.Domain.Common;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces
{
    /// <summary>
    /// Provides an interface for validating the data in an entity
    /// </summary>
    public interface IValidatable
    {
        IList<ValidationItem> ValidationItems { get; }

        bool Validate(int userid);
    }
}