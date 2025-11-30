using System;

namespace ALOD.Core.Domain.Lookup
{
    public class CancelReasons
    {
        public CancelReasons()
        {
            Id = 0;
            Description = String.Empty;
            DisplayOrder = 0;
        }

        public CancelReasons(int id, string Description, int DisplayOrder)
        {
            this.Id = id;
            this.Description = Description;
            this.DisplayOrder = DisplayOrder;
        }

        public virtual string Description { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int Id { get; set; }
    }
}