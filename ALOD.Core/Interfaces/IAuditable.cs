using System;

namespace ALOD.Core.Interfaces
{
    public interface IAuditable
    {
        DateTime ModifiedDate { get; set; }
    }
}