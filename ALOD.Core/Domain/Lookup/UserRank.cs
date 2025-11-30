using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Lookup
{
    [Serializable]
    public class UserRank : Entity, IHasAssignedId<int>
    {
        public virtual int DisplayOrder { get; set; }

        public virtual string FormattedGrade
        {
            get
            {
                return FormatGrade();
            }
        }

        public virtual string Grade { get; set; }
        public virtual string Rank { get; set; }
        public virtual string Title { get; set; }

        #region IHasAssignedId<int> Members

        /// <inheritdoc/>
        public virtual void SetId(int assignedId)
        {
            Id = assignedId;
        }

        #endregion IHasAssignedId<int> Members

        protected virtual string FormatGrade()
        {
            string prefix = string.Empty;
            int startIndex = -1;

            if (Grade.Contains("E"))        // Enlisted
            {
                startIndex = Grade.IndexOf("E") + 1;
                prefix = "E";
            }
            else if (Grade.Contains("0"))    // Officer
            {
                startIndex = Grade.IndexOf("0") + 1;
                prefix = "O";
            }

            if (startIndex == -1)
                return Grade;

            return (prefix + "-" + Grade.Substring(startIndex, Grade.Length - startIndex));
        }
    }
}