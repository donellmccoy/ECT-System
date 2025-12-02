using System;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Data.Services;

namespace ALODWebUtility.Common
{
    public static class PSIDUtility
    {
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

        public static SC_PSCD_Findings CreatePSIDFinding(int psidID)
        {
            SC_PSCD_Findings cFinding = new SC_PSCD_Findings();
            var currUser = UserService.CurrentUser();

            cFinding.PSCDId = psidID;
            cFinding.Name = currUser.FullName;
            cFinding.ModifiedBy = currUser.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = currUser.Id;
            cFinding.CreatedDate = DateTime.Now;

            return cFinding;
        }
    }
}
