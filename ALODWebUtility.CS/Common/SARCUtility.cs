using System;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Data.Services;

namespace ALODWebUtility.Common
{
    public static class SARCUtility
    {
        public static RestrictedSARCFindings CreateSARCFinding(int sarcId)
        {
            RestrictedSARCFindings cFinding = new RestrictedSARCFindings();
            var currUser = UserService.CurrentUser();

            cFinding.SARCID = sarcId;
            cFinding.SSN = currUser.SSN;
            cFinding.Compo = currUser.Component;
            cFinding.Rank = currUser.Rank.Rank;
            cFinding.Grade = currUser.Rank.Grade;
            cFinding.Name = currUser.FullName;
            cFinding.ModifiedBy = currUser.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = currUser.Id;
            cFinding.CreatedDate = DateTime.Now;

            return cFinding;
        }

        public static bool HasFinding(RestrictedSARCFindings finding)
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
