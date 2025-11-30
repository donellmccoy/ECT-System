using ALOD.Core.Utils;
using System.Data;

namespace ALOD.Core.Domain.Lookup
{
    public class ReserveMedicalUnit
    {
        public ReserveMedicalUnit(DataRow row)
        {
            Id = DataHelpers.GetIntFromDataRow("Id", row);
            CSId = DataHelpers.GetIntFromDataRow("cs_id", row);
            Name = DataHelpers.GetStringFromDataRow("RMU", row);
        }

        public int CSId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}