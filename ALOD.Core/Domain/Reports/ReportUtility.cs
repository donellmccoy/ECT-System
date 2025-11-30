using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Reports
{
    public static class ReportUtility
    {
        public const int FORMAL_BOARD_THRESHOLD = 20;

        public const int FORMAL_IO_THRESHOLD = 30;

        public const int FORMAL_TOTAL_THRESHOLD = 160;

        public const int FORMAL_UNITCC_THRESHOLD = 30;

        public const int FORMAL_WINGCC_THRESHOLD = 10;

        public const int FORMAL_WINGJA_THRESHOLD = 30;

        public const int INFORMAL_BOARD_THRESHOLD = 15;

        public const int INFORMAL_MEDOFF_THRESHOLD = 46;

        public const int INFORMAL_MEDTECH_THRESHOLD = 6;

        // TO DO: Place these threshold totals in a database table and allow htem to be edited by admins from a UI page...
        public const int INFORMAL_TOTAL_THRESHOLD = 85;

        public const int INFORMAL_UNITCC_THRESHOLD = 30;
        public const int INFORMAL_WINGCC_THRESHOLD = 10;
        public const int INFORMAL_WINGJA_THRESHOLD = 30;
        // Combined total
        // Combined total

        public static ProcessingTimeThresholdStatus GetThresholdStatus(double? itemValue, int threshold, int tolerance)
        {
            if (!itemValue.HasValue || threshold == -1)
                return ProcessingTimeThresholdStatus.Under;

            if (itemValue.Value >= threshold)
            {
                return ProcessingTimeThresholdStatus.Over;
            }
            else if (tolerance > 0 && itemValue.Value >= (threshold - tolerance))
            {
                return ProcessingTimeThresholdStatus.Near;
            }
            else
            {
                return ProcessingTimeThresholdStatus.Under;
            }
        }

        public static int GetUserStartingUnitId(UserGroups groupId, int unitId, IUnitDao unitDao)
        {
            // ** NOTES ** '
            //   - Group permissions keep users who are not Board Members, LOD PMs, and System Admins out of this report...
            //   - LOD PMs start at their Wing...some LOD PMs are assigned to the Wing and some are assigned to subordinate units of the Wing...
            //   - Board Members & System Admins start at the HQ AFRCMD unit (ie. they start by seeing the NAFs that report to the HQ AFRCMD unit)...
            //   - Board Members are already assigned to the HQ AFRCMD (__) unit..
            //   - System Admins are assigned to the System Administration (__) unit...

            if (groupId == UserGroups.LOD_PM)
            {
                // Find and return the proper unit ID for the LOD PM...
                // Priority of unit types: (1) Wing --> (2) Group --> ?(3) NAF (Numbered Air Force)? --> (4) Assigned Unit

                Unit pmUnit = unitDao.GetById(unitId);
                Unit wingUnit = null;
                Unit groupUnit = null;
                Unit nafUnit = null;
                int depth = 0;
                int maxDepth = 20;

                // Check if this unit is a Wing level unit or reports to itself...
                pmUnit.ReportingStructure = unitDao.GetUnitReportingStructure(pmUnit.Id);
                if (pmUnit.UnitType.Equals("WG") || pmUnit.ReportingStructure["PHA_NMREPORT"] == pmUnit.Id)
                    return unitId;

                // Traverse up the Non-Medical Reporting heirarchy structure finding the Wing, Group, and NAF untis...
                Unit currentUnit = unitDao.GetById(pmUnit.ReportingStructure["PHA_NMREPORT"]);
                currentUnit.ReportingStructure = unitDao.GetUnitReportingStructure(currentUnit.Id);

                while (currentUnit != null && !currentUnit.PasCode.Equals("AAAA"))
                {
                    if (currentUnit.UnitType.Equals("WG") && wingUnit == null)  // Save first Wing unit found...
                    {
                        wingUnit = currentUnit;
                        break; // Wing has first prioirty so break out of loop...
                    }

                    if (currentUnit.UnitType.Equals("GP")) // Save the group found highest up in the heirarchy...
                        groupUnit = currentUnit;

                    if (currentUnit.UnitType.Equals("AF") && nafUnit == null)   // Save first NAF unit found...
                        nafUnit = currentUnit;

                    // Avoid infinite loop...
                    if (currentUnit.ReportingStructure["PHA_NMREPORT"] == currentUnit.Id || depth >= maxDepth)
                        break;

                    currentUnit = unitDao.GetById(currentUnit.ReportingStructure["PHA_NMREPORT"]);
                    currentUnit.ReportingStructure = unitDao.GetUnitReportingStructure(currentUnit.Id);

                    depth++;
                }

                if (wingUnit != null)
                    return wingUnit.Id;
                else if (groupUnit != null)
                    return groupUnit.Id;
                else if (nafUnit != null)
                    return nafUnit.Id;
                else
                    return unitId;  // No Wing, Group, or NAF unit found...just return the users unit...
            }
            else if (groupId == UserGroups.SystemAdministrator)
            {
                // Find and return the HQ AFRCMD (__) unit for the System Admins...
                Unit cmdUnit = unitDao.GetByNameAndPASCode("HQ AFRCMD", "__");

                if (cmdUnit != null)
                    return cmdUnit.Id;

                return unitId;
            }
            else
            {
                return unitId;
            }
        }
    }
}