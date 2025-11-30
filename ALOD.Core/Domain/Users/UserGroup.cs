using ALOD.Core.Domain.Workflow;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class UserGroup : Entity
    {
        public UserGroup()
        { }

        public UserGroup(int groupId)
        { Id = groupId; }

        public virtual string Abbreviation { get; set; }
        public virtual bool CanBeRequested { get; set; }
        public virtual string Component { get; set; }
        public virtual string Description { get; set; }
        public virtual UserGroupLevel GroupLevel { get; set; }
        public virtual bool HipaaRequired { get; set; }
        public virtual IList<UserGroup> ManagedGroups { get; set; }
        public virtual IList<ALODPermission> Permissions { get; set; }
        public virtual ReportingView ReportView { get; set; }
        public virtual AccessScope Scope { get; set; }
        public virtual bool ShowInfo { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual IList<UserGroup> ViewByGroups { get; set; }
    }
}