using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODSARCCasesReportResultItem : IExtractedEntity
    {
        public LODSARCCasesReportResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        private LODSARCCasesReportResultItem(DataRow row)
        { }

        public virtual string CaseId { get; set; }
        public virtual bool IsComplete { get; set; }
        public virtual bool IsRestricted { get; set; }
        public virtual string MemberName { get; set; }

        public virtual string MemberPartialSSN
        {
            get
            {
                if (String.IsNullOrEmpty(MemberSSN) || MemberSSN.Length < 4)
                    return string.Empty;

                return MemberSSN.Substring(MemberSSN.Length - 4, 4);
            }
        }

        public virtual string MemberUnit { get; set; }
        public virtual int MemberUnitId { get; set; }
        public virtual int ModuleId { get; set; }
        public virtual int RefId { get; set; }
        public virtual string StatusCodeName { get; set; }
        public virtual int WorkflowId { get; set; }
        public virtual int WorkStatusId { get; set; }
        protected virtual string MemberSSN { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RefId = DataHelpers.GetIntFromDataRow("RefId", row);
                this.ModuleId = DataHelpers.GetIntFromDataRow("ModuleId", row);
                this.WorkflowId = DataHelpers.GetIntFromDataRow("WorkflowId", row);
                this.CaseId = DataHelpers.GetStringFromDataRow("CaseId", row);
                this.IsRestricted = DataHelpers.GetBoolFromDataRow("IsRestricted", row);
                this.IsComplete = DataHelpers.GetBoolFromDataRow("IsComplete", row);
                this.WorkStatusId = DataHelpers.GetIntFromDataRow("WorkStatusId", row);
                this.StatusCodeName = DataHelpers.GetStringFromDataRow("StatusCodeName", row);
                this.MemberUnit = DataHelpers.GetStringFromDataRow("MemberUnit", row);
                this.MemberUnitId = DataHelpers.GetIntFromDataRow("MemberUnitId", row);
                this.MemberName = DataHelpers.GetStringFromDataRow("MemberName", row);
                this.MemberSSN = DataHelpers.GetStringFromDataRow("MemberSSN", row);

                return true;
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                return ExtractFromDataRow(row);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.RefId = 0;
            this.ModuleId = 0;
            this.WorkflowId = 0;
            this.CaseId = string.Empty;
            this.IsRestricted = false;
            this.IsComplete = false;
            this.WorkStatusId = 0;
            this.StatusCodeName = string.Empty;
            this.MemberUnit = string.Empty;
            this.MemberUnitId = 0;
            this.MemberName = string.Empty;
            this.MemberSSN = string.Empty;
        }
    }
}