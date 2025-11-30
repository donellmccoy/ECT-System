using ALOD.Core.Domain.Common;
using System;

namespace ALOD.Core.Domain.Modules.SARC
{
    [Serializable]
    public class RestrictedSARCOtherICDCode : Entity
    {
        public RestrictedSARCOtherICDCode()
        {
            this.SARCId = 0;
            this.ICDCode = null;
            this.ICD7thCharacter = string.Empty;
        }

        public RestrictedSARCOtherICDCode(int sarcId, ICD9Code icdCode, string icd7thChar)
        {
            this.SARCId = sarcId;
            this.ICDCode = icdCode;
            this.ICD7thCharacter = icd7thChar;
        }

        public virtual string ICD7thCharacter { get; set; }
        public virtual ICD9Code ICDCode { get; set; }
        public virtual int SARCId { get; set; }
    }
}