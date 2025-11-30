using ALOD.Core.Domain.Workflow;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class CommandStruct : Entity
    {
        private IList<CommandStruct> children = new List<CommandStruct>();

        public CommandStruct()
        { }

        public CommandStruct(int id)
        { Id = id; }

        public CommandStruct(int id, ReportingView rpt)
        { Id = id; ReportView = rpt; }

        public virtual IList<CommandStruct> ChildUnits
        {
            get { return children; }
            private set { children = value; }
        }

        public virtual Unit Details { get; set; }

        public virtual AppUser ModifiedBy { get; set; }

        public virtual DateTime? ModifiedDate { get; set; }

        public virtual CommandStruct Parent { get; set; }

        [DomainSignature]
        public virtual ReportingView ReportView { get; set; }
    }
}