using System.Web.UI.WebControls;

namespace ALOD.Core.Utils.DataTransferObjects
{
    public class IRILOMedicalOfficerTabInitMemosDropdownListDto
    {
        public int? BoardMedicalFindingId { get; set; }
        public DropDownList MemosDropDownList { get; set; }
        public int? MemoTemplateId { get; set; }

        public int? ProcessAs { get; set; }
    }
}