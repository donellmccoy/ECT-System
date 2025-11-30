using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class ICD9Code : Entity
    {
        public ICD9Code()
        { }

        public ICD9Code(int id)
        {
            this.Id = id;
        }

        public virtual bool Active { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual int DiagnosisLevel { get; set; }
        public virtual bool Has7thCharacter { get; set; }
        public virtual int ICDVersion { get; set; }
        public virtual bool IsDisease { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual int SortOrder { get; set; }

        public virtual string GetFullCode(string seventhChar)
        {
            // If this is a pre-ICD10 code then just return the code as is...
            if (ICDVersion < 10)
            {
                return Code;
            }

            // If null or empty seventh character then just return the code as is...
            if (string.IsNullOrEmpty(seventhChar))
            {
                return Code;
            }

            // If the code itself is null or empty then just return an empty string...
            if (string.IsNullOrEmpty(Code))
            {
                return string.Empty;
            }

            int characterCount = 0;
            int numOfFillChars = 0;
            string fullCode = Code;

            // Determine number of fill characters needed...
            if (fullCode.Contains("."))
            {
                characterCount = fullCode.Length - 1;
            }
            else
            {
                characterCount = fullCode.Length;
                fullCode += ".";
            }

            numOfFillChars = 6 - characterCount;

            // Append fill characters...
            for (int i = 0; i < numOfFillChars; i++)
            {
                fullCode += "x";
            }

            // Append 7th character...
            fullCode += seventhChar;

            return fullCode;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Id.ToString() + " - " + Description;
        }
    }
}