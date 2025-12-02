using System;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Data.Services;

namespace ALODWebUtility.Common
{
    public static class AppealSARCUtility
    {
        public static SARCAppealFindings CreateSARCFinding(int appealId)
        {
            SARCAppealFindings cFinding = new SARCAppealFindings();
            var currUser = UserService.CurrentUser();

            cFinding.AppealID = appealId;
            cFinding.SSN = currUser.SSN;
            cFinding.Compo = currUser.Component;
            cFinding.Rank = currUser.Rank.Rank;
            cFinding.Grade = currUser.Rank.Grade;
            cFinding.Name = currUser.FullName;
            cFinding.Pascode = currUser.Unit.PasCode;
            cFinding.ModifiedBy = currUser.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = currUser.Id;
            cFinding.CreatedDate = DateTime.Now;

            return cFinding;
        }

        public static bool HasFinding(SARCAppealFindings finding)
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
