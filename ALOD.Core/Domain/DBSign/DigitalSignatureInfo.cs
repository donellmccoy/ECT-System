using System;

namespace ALOD.Core.Domain.DBSign
{
    public class DigitalSignatureInfo
    {
        public DateTime DateSigned { get; set; }
        public string Name { get; set; }
        public string Signature { get; set; }
    }
}