using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class WitnessData
    {
        public WitnessData(string name, string address, string phonenumber)
        {
            Name = name;
            Address = address;
            PhoneNumber = phonenumber;
        }

        public WitnessData()
        {
            Name = "";
            Address = "";
            PhoneNumber = "";
        }

        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}