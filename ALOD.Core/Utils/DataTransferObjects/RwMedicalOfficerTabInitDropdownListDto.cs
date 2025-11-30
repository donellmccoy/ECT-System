using System.Web.UI.WebControls;

namespace ALOD.Core.Utils.DataTransferObjects
{
    public class RwMedicalOfficerTabInitDropdownListDto
    {
        public int? AlcLetterType { get; set; }
        public DropDownList AssignmentLimitationCodeDropDownList { get; set; }
        public RadioButtonList DecisionRadioButtonList { get; set; }
        public DropDownList MemosDropDownList { get; set; }

        public int? MemoTemplateId { get; set; }
        public int? ProcessAs { get; set; }
        public DropDownList ProcessAsDropDownList { get; set; }
    }
}