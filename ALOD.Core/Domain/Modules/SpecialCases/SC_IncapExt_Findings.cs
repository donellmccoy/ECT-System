using System;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_IncapExt_Findings : Entity
    {
        public virtual bool? CAFR_ExtApproval { get; set; }
        public virtual bool? CCR_ExtApproval { get; set; }
        public virtual bool? DOP_ExtApproval { get; set; }
        public virtual bool? DOS_ExtApproval { get; set; }
        public virtual DateTime? EXT_EndDate { get; set; }
        public virtual int EXT_Id { get; set; }
        public virtual int EXT_Number { get; set; }
        public virtual DateTime? EXT_StartDate { get; set; }
        public virtual bool? FIN_ExtIncomeLost { get; set; }
        public virtual bool? IC_ExtRecommendation { get; set; }
        public virtual string med_AMRODisposition { get; set; }
        public virtual DateTime? MED_AMROEndDate { get; set; }
        public virtual DateTime? MED_AMROStartDate { get; set; }
        public virtual bool? MED_ExtRecommendation { get; set; }
        public virtual string MED_IRILOStatus { get; set; }
        public virtual DateTime? MED_NextAMROEndDate { get; set; }
        public virtual DateTime? MED_NextAMROStartDate { get; set; }
        public virtual bool? OCR_ExtApproval { get; set; }
        public virtual bool? OPR_ExtApproval { get; set; }
        public virtual bool? VCR_ExtApproval { get; set; }
        public virtual bool? WCC_ExtApproval { get; set; }
        public virtual bool? WJA_ConcurWithIC { get; set; }
    }
}