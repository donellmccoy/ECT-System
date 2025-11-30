using System;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_Incap_Findings : Entity
    {
        //Appeal
        public virtual int AP_Id { get; set; }

        public virtual bool? CAFR_AppealApproval { get; set; }

        public virtual bool? CCR_AppealApproval { get; set; }

        public virtual bool? DOP_AppealApproval { get; set; }

        public virtual bool? DOS_AppealApproval { get; set; }

        public virtual bool? Fin_IncomeLost { get; set; }

        public virtual bool? Fin_SelfEmployed { get; set; }

        public virtual bool? IC_Recommendation { get; set; }

        public virtual bool? Init_AppealOrComplete { get; set; }

        public virtual DateTime? Init_EndDate { get; set; }

        public virtual bool? Init_ExtOrComplete { get; set; }

        public virtual bool? Init_LateSubmission { get; set; }

        //public virtual string Case_Id { get; set; }
        public virtual DateTime? Init_StartDate { get; set; }

        public virtual bool? Med_AbilityToPreform { get; set; }

        public virtual string Med_ReportType { get; set; }

        public virtual bool? OCR_AppealApproval { get; set; }

        public virtual bool? OPR_AppealApproval { get; set; }

        //Initial
        public virtual int SC_Id { get; set; }

        public virtual bool? VCR_AppealApproval { get; set; }
        public virtual bool? WCC_AppealApproval { get; set; }
        public virtual bool? WCC_InitApproval { get; set; }
        public virtual bool? Wing_Ja_Concur { get; set; }
        //EXT
        //public virtual int EXT_Id { get; set; }
        //public virtual int EXT_Number { get; set; }
        //public virtual DateTime? EXT_StartDate { get; set; }
        //public virtual DateTime? EXT_EndDate { get; set; }
        //public virtual bool? MED_ExtRecommendation { get; set; }
        //public virtual bool? IC_ExtRecommendation { get; set; }
        //public virtual bool? WJA_ConcurWithIC { get; set; }
        //public virtual bool? FIN_ExtIncomeLost { get; set; }
        //public virtual bool? WCC_ExtApproval { get; set; }
        //public virtual bool? OPR_ExtApproval { get; set; }
        //public virtual bool? OCR_ExtApproval { get; set; }
        //public virtual bool? DOS_ExtApproval { get; set; }
        //public virtual bool? CCR_ExtApproval { get; set; }
        //public virtual bool? VCR_ExtApproval { get; set; }
        //public virtual bool? DOP_ExtApproval { get; set; }
        //public virtual bool? CAFR_ExtApproval { get; set; }

        //public virtual string TMTNumber { get; set; }

        //public virtual DateTime? TMTReceiveDate { get; set; }

        //public virtual DateTime? SuspenseDate { get; set; }
    }
}