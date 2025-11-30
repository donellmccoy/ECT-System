using System.Collections.Generic;
using System.Linq;
using System.Web;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Data.Services;

namespace ALODWebUtility.Common
{
    public static class UserCanEdit
    {
        public static bool GetAccess(IList<PageAccess> Pages, bool CheckLock)
        {

            //if we don't have a case lock aquired, this will be read-only regardless of pageacess
            if (CheckLock && !SessionInfo.SESSION_LOCK_AQUIRED)
            {
                return false;
            }

            //otherwise, we have a case lock, so check page access
            var access = (from t in Pages where t.PageTitle == SiteMap.CurrentNode.Title select t).SingleOrDefault();

            if (access == null)
            {
                return false;
            }
            else
            {
                if (access.Access == ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public static bool GetAccessLOD(IList<PageAccess> Pages, bool CheckLock, LineOfDuty LOD)
        {

            //if we don't have a case lock aquired, this will be read-only regardless of pageacess
            if (CheckLock && !SessionInfo.SESSION_LOCK_AQUIRED)
            {
                return false;
            }

            //otherwise, we have a case lock, so check page access
            var access = (from t in Pages where t.PageTitle == SiteMap.CurrentNode.Title select t).SingleOrDefault();

            if (access == null)
            {
                return false;
            }
            else
            {
                if (access.Access == ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite)
                {
                    //Check is the user is a Board role
                    if (Utility.IsUserBelongsToTheBoard(SessionInfo.SESSION_GROUP_ID, true, true))
                    {
                        return true;
                    }
                    else //Continue Normal Verification for the Attach PASSCODE
                    {
                        //Need to verify if the Case has an attach PASSCODE
                        //If yes and reached this point, means the user has Read/Write "Medical".
                        //Need to evaluate if the user belongs to the Attach PASSCODE
                        //If yes, user can edit, If no, user Can not edit
                        //UserCanEdit = True

                        //Dim lod As LineOfDuty = LodService.GetById(refId)

                        if (LOD.isAttachPas)
                        {
                            //First , evaluate if the user is part of the Attach PASSCODE
                            if (UserService.IsMemberPartOfAttachUnit(SessionInfo.SESSION_USER_ID, LOD.MemberAttachedUnitId))
                            {
                                //User is from the Attach PASSCODE, can have write access
                                return true;
                            }
                            else
                            {
                                //User is from the Member PASSCODE, only read access
                                return false;

                            }
                        }
                        else //Continue normal Execution since the LOD does not have an Attach PASSCODE
                        {
                            return true;
                        }

                    }
                    if (LOD.WorkflowStatus.StatusCodeType.IsFinal)
                    {
                        if (LOD.LODMedical.PhysicianCancelReason != 0)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false; // Added default return
        }
    }
}
