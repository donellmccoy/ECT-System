using System.Collections.Generic;

namespace ALOD.Core.Domain.Documents
{
    public class DocCategoryView : Entity
    {
        public virtual IList<DocumentCategory2> Categories { get; protected set; }
        public virtual int ViewOrder { get; set; }
        public virtual IList<DocumentView> Views { get; protected set; }
    }
}