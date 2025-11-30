using System;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_IncapAppeal_Findings : Entity
    {
        public virtual int AP_Id { get; set; }
        public virtual bool? CAFR_AppealApproval { get; set; }
        public virtual bool? CCR_AppealApproval { get; set; }
        public virtual bool? DOP_AppealApproval { get; set; }
        public virtual bool? DOS_AppealApproval { get; set; }
        public virtual int EXT_Id { get; set; }
        public virtual bool? OCR_AppealApproval { get; set; }
        public virtual bool? OPR_AppealApproval { get; set; }
        public virtual int SC_Id { get; set; }
        public virtual bool? VCR_AppealApproval { get; set; }
        public virtual bool? WCC_AppealApproval { get; set; }
    }
}