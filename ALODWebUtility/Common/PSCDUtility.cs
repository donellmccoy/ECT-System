using System;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Data.Services;

namespace ALODWebUtility.Common
{
    public static class PSCDUtility
    {
        public static SC_PSCD_Findings CreatePSCDFinding(int PSCDID)
        {
            SC_PSCD_Findings cFinding = new SC_PSCD_Findings();
            var currUser = UserService.CurrentUser;

            cFinding.PSCDId = PSCDID;
            cFinding.Name = currUser.FullName;
            cFinding.ModifiedBy = currUser.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = currUser.Id;
            cFinding.CreatedDate = DateTime.Now;

            return cFinding;
        }

        public static bool HasFinding(SC_PSCD_Findings finding)
        {
            if (finding == null)
            {
                return false;
            }

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsTimeToSaveViaNavigator(NavigatorButtonType buttonType)
        {
            if (buttonType == NavigatorButtonType.Save ||
                buttonType == NavigatorButtonType.NavigatedAway ||
                buttonType == NavigatorButtonType.NextStep ||
                buttonType == NavigatorButtonType.PreviousStep)
            {
                return true;
            }

            return false;
        }
    }
}
