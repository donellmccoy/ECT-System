using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class PersonnelData
    {
        public PersonnelData(string nm, string ssn, string gr, string pscd, string descr, string br, string com)
        {
            Name = nm;
            SSN = ssn;
            Grade = gr;
            PasCode = pscd;
            PasCodeDescription = descr;
            Branch = br;
            Compo = com;
        }

        public PersonnelData()
        {
            Name = "";
            SSN = "";
            Grade = "";
            PasCode = "";
            PasCodeDescription = "";
            Branch = "";
            Compo = "";
        }

        public string Branch { get; set; }
        public string Compo { get; set; }
        public string Grade { get; set; }
        public bool InvestigationMade { get; set; }
        public string Name { get; set; }
        public string PasCode { get; set; }
        public string PasCodeDescription { get; set; }
        public string SSN { get; set; }
    }
}