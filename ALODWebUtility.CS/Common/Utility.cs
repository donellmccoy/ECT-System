using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.TabNavigation;
using Microsoft.VisualBasic; // For IsNumeric

namespace ALODWebUtility.Common
{
    public static class Utility
    {
        // This is the date in which Form 348 & 261 documents began getting archived into SRXLite
        // when a LOD case is completed.
        public static readonly DateTime ARCHIVE_DATE = new DateTime(2013, 11, 8);

        public const string CSS_FIELD_REQUIRED = "fieldRequired";

        public const string DATE_FORMAT = "MM/dd/yyyy";

        public const string DATE_HOUR_FORMAT = "MM/dd/yyyy HHmm";

        public const string ENGLISH_ALPHABET_UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const string HOUR_FORMAT = "HHmm";

        public const string KEY_NO_SUB_UNITS_MESSAGE = "NO_SUB_UNITS_MESSAGE";

        public const string KEY_SELECTED_CS_ID = "selected_csid";

        public const string NON_PERMITTED_SPECIAL_CHAR_INPUT = "<,>,;,#,^,&,=,{,},[,]";

        public const string PERMITTED_SPECIAL_CHAR_INPUT = "`,~,!,@,$,%,*,(,),-,_,+,',\",.,,,?,/,\\,|,:, ";

        public const string PHONE_NUMBER_CHARACTERS = "(,),-";

        public const int STRLEN_SSN = 9;

        public const string WEBSITE_SPECIAL_CHAR_INPUT = ".,:,/,?,-,_";

        public delegate List<ALOD.Core.Domain.Users.LookUpItem> UnitLookUpDelegate(StringDictionary param);

        public static DeployMode AppMode
        {
            get
            {
                string deployMode = ConfigurationManager.AppSettings["DeployMode"];
                if (deployMode == null) return DeployMode.Development;
                
                switch (deployMode.ToLower())
                {
                    case "prod":
                        return DeployMode.Production;
                    case "demo":
                        return DeployMode.Demo;
                    case "train":
                        return DeployMode.Training;
                    case "test":
                        return DeployMode.Test;
                    default:
                        return DeployMode.Development;
                }
            }
        }

        public static void AddCssClass(WebControl control, string cssClass)
        {
            control.CssClass = control.CssClass + " " + cssClass;
        }

        /// <summary>
        /// Adds a stylesheet to a page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="stylesheet"></param>
        /// <remarks></remarks>
        public static void AddStyleSheet(Page page, string stylesheet)
        {
            HtmlLink link = new HtmlLink();
            link.Href = page.ResolveUrl(stylesheet);
            link.Attributes["rel"] = "stylesheet";
            link.Attributes["text"] = "text/css";

            page.Header.Controls.Add(link);
        }

        public static bool CheckDate(TextBox val)
        {
            try
            {
                DateTime.Parse(val.Text.Trim());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckTextLength(ref TextBox Box)
        {
            if (Box.Text.Trim().Length > Box.MaxLength)
            {
                AddCssClass(Box, "fieldRequired");
                return false;
            }
            else
            {
                RemoveCssClass(Box, "fieldRequired");
                return true;
            }
        }

        public static ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings CreateFinding(int lodid)
        {
            ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings cFinding = new ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings();
            cFinding.LODID = lodid;
            var currUser = UserService.CurrentUser;
            cFinding.SSN = currUser.SSN;
            cFinding.Compo = currUser.Component;
            cFinding.Rank = currUser.Rank.Rank;
            cFinding.Grade = currUser.Rank.Grade;
            cFinding.Name = currUser.FirstLastName;
            cFinding.ModifiedBy = currUser.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = currUser.Id;
            cFinding.CreatedDate = DateTime.Now;
            return cFinding;
        }

        public static bool DoesStringContainNonPermittedCharacters(string s)
        {
            List<string> nonPermittedCharacters = NON_PERMITTED_SPECIAL_CHAR_INPUT.Split(',').ToList();

            foreach (char c in s)
            {
                if (nonPermittedCharacters.Contains(c.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool DoesStringContainSpecialCharacters(string s)
        {
            List<char> permittedCharacters = new List<char>();

            foreach (char c in ENGLISH_ALPHABET_UPPERCASE)
            {
                permittedCharacters.Add(c);
                permittedCharacters.Add(char.ToLower(c));
            }

            for (int i = 0; i <= 9; i++)
            {
                permittedCharacters.Add(i.ToString()[0]);
            }

            foreach (char c in s)
            {
                if (!permittedCharacters.Contains(c))
                {
                    return true;
                }
            }

            return false;
        }

        public static int FileSizeUploadLimit()
        {
            return IntCheck(ConfigurationManager.AppSettings["InitialFileSizeUploadLimit"]);
        }

        public static object FindOnTab(string name, Page page)
        {
            return page.Master.Master.FindControl("ContentMain").FindControl("ContentNested").FindControl(name);
        }

        public static string FormatFileName(string fileName)
        {
            if (fileName.IndexOf("\\") == -1)
            {
                return fileName;
            }

            return fileName.Substring(fileName.LastIndexOf("\\") + 1);
        }

        public static string FormatSSN(string ssn)
        {
            if (ssn.Length < 9)
            {
                return ssn;
            }

            return ssn.Substring(0, 3) + "-" + ssn.Substring(3, 2) + "-" + ssn.Substring(5, 4);
        }

        public static string FormatSSN(string ssn, bool useMask)
        {
            if (ssn.Length < 9)
            {
                return ssn;
            }

            if (!useMask)
            {
                return FormatSSN(ssn);
            }

            return "XXX-XX-" + ssn.Substring(5, 4);
        }

        public static string GetCalendarImage(ref Page page)
        {
            return page.ResolveClientUrl("~/App_Themes/" + page.Theme + "/Images/Calendar.gif");
        }

        public static string GetCompoAbbr(string compo)
        {
            switch (compo)
            {
                case "1":
                    return "A";
                case "2":
                    return "ARNG";
                case "3":
                    return "USARC";
                case "4":
                    return "AF";
                case "5":
                    return "ANG";
                case "6":
                    return "AFRC";
                case "7":
                    return "N";
                case "8":
                    return "NNG";
                case "9":
                    return "NR";
            }

            return "";
        }

        public static string GetCompoString(string compo)
        {
            switch (compo)
            {
                case "1":
                    return "Active Army";
                case "2":
                    return "Army National Guard";
                case "3":
                    return "Army Reserve";
                case "4":
                    return "Active Air Force";
                case "5":
                    return "Air National Guard";
                case "6":
                    return "Air Force Reserve";
                case "7":
                    return "Active Navy";
                case "8":
                    return "Navy National Guard";
                case "9":
                    return "Navy Reserve";
            }

            return "";
        }

        public static int? GetDropDownListNullableSelectedValue(DropDownList ddl, int nullableValue)
        {
            if (int.Parse(ddl.SelectedValue) == nullableValue)
            {
                return null;
            }
            else
            {
                return int.Parse(ddl.SelectedValue);
            }
        }

        public static double GetFileSizeMB(int fileSizeBytes)
        {
            return Math.Round(fileSizeBytes / 1048576.0, 2);
        }

        public static double GetFileSizeUploadLimitMB()
        {
            return GetFileSizeMB(FileSizeUploadLimit());
        }

        public static string GetHostName()
        {
            return HttpContext.Current.Request.Url.Scheme + Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host;
        }

        public static string GetMultiListSelectedValues(object ctl)
        {
            // Assuming ctl is ListControl
            ListControl listCtl = (ListControl)ctl;
            StringBuilder sb = new StringBuilder(200);
            
            foreach (ListItem lstItem in listCtl.Items)
            {
                if (lstItem.Selected == true)
                {
                    sb.Append(lstItem.Value + ",");
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public static PersonnelTypes GetPersonnelTypeFromGroup(int groupId, bool formal)
        {
            UserGroups group = (UserGroups)groupId;

            switch (group)
            {
                case UserGroups.MedicalTechnician:
                case UserGroups.MedTech_Pilot:
                    return PersonnelTypes.MED_TECH;

                case UserGroups.MedicalOfficer:
                case UserGroups.MedOfficer_Pilot:
                    return PersonnelTypes.MED_OFF;

                case UserGroups.UnitCommander:
                case UserGroups.UnitCC_LODV3:
                    return PersonnelTypes.UNIT_CMDR;

                case UserGroups.WingJudgeAdvocate:
                case UserGroups.WingJA_Pilot:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_WING_JA;
                    }
                    else
                    {
                        return PersonnelTypes.WING_JA;
                    }

                case UserGroups.WingCommander:
                case UserGroups.AppointingAuthority_Pilot:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_APP_AUTH;
                    }
                    else
                    {
                        return PersonnelTypes.APPOINT_AUTH;
                    }

                case UserGroups.BoardTechnician:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_BOARD_RA;
                    }
                    else
                    {
                        return PersonnelTypes.BOARD;
                    }

                case UserGroups.BoardLegal:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_BOARD_JA;
                    }
                    else
                    {
                        return PersonnelTypes.BOARD_JA;
                    }

                case UserGroups.BoardMedical:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_BOARD_SG;
                    }
                    else
                    {
                        return PersonnelTypes.BOARD_SG;
                    }

                case UserGroups.BoardApprovalAuthority:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_BOARD_AA;
                    }
                    else
                    {
                        return PersonnelTypes.BOARD_AA;
                    }

                case UserGroups.MPF:
                    return PersonnelTypes.MPF;

                case UserGroups.InvestigatingOfficer:
                    return PersonnelTypes.IO;

                case UserGroups.LOD_MFP:
                    return PersonnelTypes.LOD_MFP;

                case UserGroups.LOD_PM:
                    return PersonnelTypes.LOD_PM;

                case UserGroups.BoardAdministrator:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_BOARD_A1;
                    }
                    else
                    {
                        return PersonnelTypes.BOARD_A1;
                    }

                case UserGroups.AppellateAuthority:
                    return PersonnelTypes.APPELLATE_AUTH;

                case UserGroups.RSL:
                    return PersonnelTypes.WING_SARC_RSL;

                case UserGroups.SARCAdmin:
                    return PersonnelTypes.SARC_ADMIN;

                case UserGroups.WingSarc:
                    return PersonnelTypes.WING_SARC_RSL;

                case UserGroups.SeniorMedicalReviewer:
                    if (formal)
                    {
                        return PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER;
                    }
                    else
                    {
                        return PersonnelTypes.SENIOR_MEDICAL_REVIEWER;
                    }

                default:
                    return 0; // VB returns Nothing, C# default for enum
            }
        }

        public static string GetReportingViewDescription(byte reportView)
        {
            string description = "Default View";

            switch (reportView)
            {
                case 1:
                    description = "Total View";
                    break;
                case 2:
                    description = "Non Medical Reporting View";
                    break;
                case 3:
                    description = "Medical Reporting View";
                    break;
                case 4:
                    description = "RMU View (Physical Responsibility)";
                    break;
                case 5:
                    description = "JA View";
                    break;
                case 6:
                    description = "MPF View";
                    break;
                case 7:
                    description = "System Administration View";
                    break;
            }

            return description;
        }

        public static string GetSearchPermissionByModuleId(ModuleType moduleId)
        {
            switch (moduleId)
            {
                case ModuleType.LOD:
                    return "lodSearch";
                case ModuleType.ReinvestigationRequest:
                    return "reinvestigateSearch";
                case ModuleType.SpecCaseBCMR:
                    return "BCMRSearch";
                case ModuleType.SpecCaseBMT:
                    return "BMTSearch";
                case ModuleType.SpecCaseMEPS:
                    return "BMTSearch";
                case ModuleType.SpecCaseCMAS:
                    return "CMASSearch";
                case ModuleType.SpecCaseCongress:
                    return "CISearch";
                case ModuleType.SpecCaseFT:
                    return "FTSearch";
                case ModuleType.SpecCaseIncap:
                    return "INCAPSearch";
                case ModuleType.SpecCaseMEB:
                    return "MEBSearch";
                case ModuleType.SpecCasePW:
                    return "PWSearch";
                case ModuleType.SpecCaseWWD:
                    return "WWDSearch";
                case ModuleType.SpecCaseMH:
                    return "MHSearch";
                case ModuleType.SpecCaseNE:
                    return "NESearch";
                case ModuleType.SpecCaseDW:
                    return "DWSearch";
                case ModuleType.SpecCaseMO:
                    return "MOSearch";
                case ModuleType.SpecCasePEPP:
                    return "PEPPSearch";
                case ModuleType.SpecCaseRS:
                    return "RSSearch";
                case ModuleType.SpecCaseRW:
                    return "RWSearch";
                case ModuleType.SpecCasePH:
                    return "PHSearch";
                case ModuleType.AppealRequest:
                    return "APSearch";
                case ModuleType.SARCAppeal:
                    return "RSARCAppealSearch";
                case ModuleType.SpecCaseAGR:
                    return "AGRCertSearch";
                case ModuleType.SpecCasePSCD:
                    return "APSearch";
                default:
                    return string.Empty;
            }
        }

        public static string GetUserName(int userId)
        {
            if (userId == 0)
            {
                return "";
            }

            return UserService.GetById(userId).FullName;
        }

        public static string GetViewPermissionByModuleId(ModuleType moduleId)
        {
            switch (moduleId)
            {
                case ModuleType.LOD:
                    return "lodView";
                case ModuleType.ReinvestigationRequest:
                    return "RRView";
                case ModuleType.SpecCaseBCMR:
                    return "BCMRView";
                case ModuleType.SpecCaseBMT:
                    return "BMTView";
                case ModuleType.SpecCaseMEPS:
                    return "MEPSView";
                case ModuleType.SpecCaseCMAS:
                    return "CMASView";
                case ModuleType.SpecCaseCongress:
                    return "CIView";
                case ModuleType.SpecCaseFT:
                    return "IRILOView";
                case ModuleType.SpecCaseIncap:
                    return "INCAPView";
                case ModuleType.SpecCaseMEB:
                    return "MEBView";
                case ModuleType.SpecCasePW:
                    return "PWView";
                case ModuleType.SpecCaseWWD:
                    return "WWDView";
                case ModuleType.SpecCaseMH:
                    return "MHView";
                case ModuleType.SpecCaseNE:
                    return "NEView";
                case ModuleType.SpecCaseDW:
                    return "DWView";
                case ModuleType.SpecCaseMO:
                    return "MOView";
                case ModuleType.SpecCasePEPP:
                    return "PEPPView";
                case ModuleType.SpecCaseRS:
                    return "RSView";
                case ModuleType.SpecCaseRW:
                    return "RWView";
                case ModuleType.SpecCasePH:
                    return "PHView";
                case ModuleType.AppealRequest:
                    return "APView";
                case ModuleType.SARCAppeal:
                    return "RSARCAppealView";
                case ModuleType.SpecCaseAGR:
                    return "AGRCertView";
                default:
                    return string.Empty;
            }
        }

        public static string GetWorkflowInitPageURL(ModuleType moduleId, int refId)
        {
            string url = string.Empty;
            string reference = "refId=";

            switch (moduleId)
            {
                case ModuleType.LOD:
                    url = "~/Secure/lod/init.aspx?";
                    break;
                case ModuleType.ReinvestigationRequest:
                    url = "~/Secure/ReinvestigationRequests/init.aspx?";
                    reference = "requestId=";
                    break;
                case ModuleType.AppealRequest:
                    url = "~/Secure/AppealRequest/init.aspx?";
                    reference = "requestId=";
                    break;
                case ModuleType.SARC:
                    url = "~/Secure/SARC/init.aspx?";
                    break;
                case ModuleType.SARCAppeal:
                    url = "~/Secure/SARC_Appeal/init.aspx?";
                    reference = "requestId=";
                    break;
                case ModuleType.SpecCaseBCMR:
                    url = "~/Secure/SC_BCMR/init.aspx?";
                    break;
                case ModuleType.SpecCaseBMT:
                    url = "~/Secure/SC_BMT/init.aspx?";
                    break;
                case ModuleType.SpecCaseCMAS:
                    url = "~/Secure/SC_CMAS/init.aspx?";
                    break;
                case ModuleType.SpecCaseCongress:
                    url = "~/Secure/SC_Congress/init.aspx?";
                    break;
                case ModuleType.SpecCaseFT:
                    url = "~/Secure/SC_FastTrack/init.aspx?";
                    break;
                case ModuleType.SpecCaseIncap:
                    url = "~/Secure/SC_Incap/init.aspx?";
                    break;
                case ModuleType.SpecCaseMEB:
                    url = "~/Secure/SC_MEB/init.aspx?";
                    break;
                case ModuleType.SpecCaseMEPS:
                    url = "~/Secure/SC_MEPS/init.aspx?";
                    break;
                case ModuleType.SpecCasePW:
                    url = "~/Secure/SC_PWaivers/init.aspx?";
                    break;
                case ModuleType.SpecCaseWWD:
                    url = "~/Secure/SC_WWD/init.aspx?";
                    break;
                case ModuleType.SpecCaseMH:
                    url = "~/Secure/SC_MH/init.aspx?";
                    break;
                case ModuleType.SpecCaseNE:
                    url = "~/Secure/SC_NE/init.aspx?";
                    break;
                case ModuleType.SpecCaseDW:
                    url = "~/Secure/SC_DW/init.aspx?";
                    break;
                case ModuleType.SpecCaseMO:
                    url = "~/Secure/SC_MO/init.aspx?";
                    break;
                case ModuleType.SpecCasePEPP:
                    url = "~/Secure/SC_PEPP/init.aspx?";
                    break;
                case ModuleType.SpecCaseRS:
                    url = "~/Secure/SC_RS/init.aspx?";
                    break;
                case ModuleType.SpecCaseRW:
                    url = "~/Secure/SC_RW/init.aspx?";
                    break;
                case ModuleType.SpecCasePH:
                    url = "~/Secure/SC_PH/init.aspx?";
                    break;
                case ModuleType.SpecCaseAGR:
                    url = "~/Secure/SC_AGRCert/init.aspx?";
                    break;
                case ModuleType.SpecCaseMMSO:
                    url = "~/Secure/SC_MMSO/init.aspx?";
                    break;
                default:
                    return string.Empty;
            }

            return (url + reference + refId.ToString());
        }

        public static void HeaderRowBinding(GridView grid, GridViewRowEventArgs e, string defaultColumn)
        {
            if (e.Row.RowType != DataControlRowType.Header)
            {
                return;
            }

            int cellIndex = -1;
            string SortColumn = defaultColumn;

            if (grid.SortExpression.Length > 0)
            {
                SortColumn = grid.SortExpression;
            }

            foreach (DataControlField field in grid.Columns)
            {
                if (field.SortExpression == SortColumn)
                {
                    cellIndex = grid.Columns.IndexOf(field);
                }
            }

            if (cellIndex > -1)
            {
                if (grid.SortDirection == SortDirection.Ascending)
                {
                    e.Row.Cells[cellIndex].CssClass = "gridViewHeader sort-asc";
                }
                else
                {
                    e.Row.Cells[cellIndex].CssClass = "gridViewHeader sort-desc";
                }
            }
        }

        public static void HighlightInvalidField(WebControl oCtrl, bool isValid)
        {
            if (oCtrl != null)
            {
                if (isValid)
                {
                    oCtrl.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    AddCssClass(oCtrl, "fieldRequired");
                }
            }
        }

        public static string HTMLDecodeNulls(string RawString, bool ReturnNothing = false)
        {
            string DecodedString;
            if (!string.IsNullOrEmpty(RawString))
            {
                DecodedString = HttpContext.Current.Server.HtmlDecode(RawString);
            }
            else
            {
                if (ReturnNothing)
                {
                    DecodedString = null;
                }
                else
                {
                    DecodedString = "";
                }
            }
            return DecodedString;
        }

        public static string HTMLEncodeNulls(string RawString, bool ReturnNothing = false)
        {
            string EncodedString;
            if (!string.IsNullOrEmpty(RawString))
            {
                EncodedString = HttpContext.Current.Server.HtmlEncode(RawString);
            }
            else
            {
                if (ReturnNothing)
                {
                    EncodedString = null;
                }
                else
                {
                    EncodedString = "";
                }
            }
            return EncodedString;
        }

        public static string ICDHierarchy(int id)
        {
            if (id == 0)
            {
                return null;
            }

            StringBuilder items = new StringBuilder();

            if (items == null)
            {
                return null;
            }

            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();

            if (icdDao == null)
            {
                return null;
            }

            ICD9Code code = icdDao.GetById(id);

            if (code == null)
            {
                return null;
            }

            items.Insert(0, code.Id.ToString());

            while (code.ParentId.HasValue)
            {
                // Get the parent code...
                code = icdDao.GetById(code.ParentId.Value);

                if (code == null)
                {
                    break;
                }

                items.Insert(0, code.Id.ToString() + ",");
            }

            return items.ToString(); // returns chapter,section,dl1,dl2,dl3,dl4
        }

        public static void InsertDropDownListEmptyValue(DropDownList ddl, string listItemTitle)
        {
            if (ddl == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(listItemTitle))
            {
                return;
            }

            ListItem firstItem = new ListItem();

            firstItem.Text = listItemTitle;
            firstItem.Value = "";

            ddl.Items.Insert(0, firstItem);
        }

        public static void InsertDropDownListZeroValue(DropDownList ddl, string listItemTitle)
        {
            if (ddl == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(listItemTitle))
            {
                return;
            }

            ListItem firstItem = new ListItem();

            firstItem.Text = listItemTitle;
            firstItem.Value = "0";

            ddl.Items.Insert(0, firstItem);
        }

        public static int IntCheck(object value, int defaultValue = 0)
        {
            if (!Information.IsNumeric(value)) return defaultValue;
            return Convert.ToInt32(value);
        }

        public static bool IsFileSizeValid(int length)
        {
            return length <= FileSizeUploadLimit();
        }

        /// <summary>
        /// Function to verify if the "current user/logged user" belongs to the Board.
        /// </summary>
        /// <param name="userGroupId">Id to verify.</param>
        /// <returns>Boolean - Is the user a has a Board Role.</returns>
        /// <remarks>Currently taking in consideration Board Technician(7), Board Legal(8), Board Medical(9), Approving Authority(88) and HQ AFRC Technician.</remarks>
        public static bool IsUserBelongsToTheBoard(int userGroupId, bool includeSA, bool includeNF)
        {
            List<int> boardList = new List<int>();

            boardList.Add((int)UserGroups.BoardLegal);
            boardList.Add((int)UserGroups.BoardMedical);
            boardList.Add((int)UserGroups.BoardApprovalAuthority);
            boardList.Add((int)UserGroups.AFRCHQTechnician);
            boardList.Add((int)UserGroups.BoardAdministrator);
            boardList.Add((int)UserGroups.HQAFRCDPH);

            if (includeNF) // Include members who do not make findings (Non-Finders)
            {
                boardList.Add((int)UserGroups.BoardTechnician);
            }

            if (includeSA)
            {
                boardList.Add((int)UserGroups.SystemAdministrator);
            }

            return boardList.Contains(userGroupId);
        }

        public static ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings NewFinding(int lodid)
        {
            ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings cFinding = new ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings();
            cFinding.LODID = lodid;
            cFinding.ModifiedBy = SessionInfo.SESSION_USER_ID;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = SessionInfo.SESSION_USER_ID;
            cFinding.CreatedDate = DateTime.Now;
            return cFinding;
        }

        public static string NullStringToEmptyString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string OptionToString(ALOD.Core.Domain.Workflow.WorkflowStatusOption item)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(item.Id.ToString() + ";");
            buffer.Append(item.wsStatusOut.ToString() + ";");
            buffer.Append(item.Template.ToString());

            return buffer.ToString();
        }

        public static object ParseDateAndTime(string inputDate)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            return DateTime.ParseExact(inputDate, DATE_HOUR_FORMAT, culture);
        }

        public static void RemoveCssClass(WebControl control, string cssClass)
        {
            control.CssClass = control.CssClass.Replace(cssClass, "");
        }

        public static void RemoveDropDownListValue(DropDownList ddl, string listItemTitle)
        {
            if (ddl == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(listItemTitle))
            {
                return;
            }

            short index = 0;
            bool isInList = false;
            foreach (ListItem x in ddl.Items)
            {
                if (x.Text.Equals(listItemTitle))
                {
                    isInList = true;
                    index = Convert.ToInt16(x.Value);
                    break;
                }
            }
            if (isInList)
            {
                ddl.Items.RemoveAt(index);
            }
        }

        public static void RunStartupScript(Page Page, string key, string script)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("<SCRIPT language=\"javascript\">\r\n");
            buffer.Append(script + "\r\n");
            buffer.Append("</SCRIPT>\r\n");

            Page.ClientScript.RegisterClientScriptBlock(Page.GetType().BaseType, key, buffer.ToString());
        }

        public static void SetDefaultButton(TextBox input, Button defaultButton)
        {
            input.Attributes.Add("onkeydown", "defaultButton('" + defaultButton.ClientID + "');");
        }

        /// <summary>
        /// Restricts the allowed input in a text box
        /// </summary>
        /// <param name="Page">The page the control belongs to</param>
        /// <param name="input">the control to limit</param>
        /// <param name="type">Type of restriction</param>
        /// <remarks></remarks>
        public static void SetInputFormatRestriction(ref Page Page, TextBox input, FormatRestriction type, string specialChars = "")
        {
            input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" + type.ToString() + "','" + specialChars + "');");
        }

        /// <summary>
        /// Restricts the allowed input in a text box
        /// </summary>
        /// <param name="Page">The page the control belongs to</param>
        /// <param name="input">the control to limit</param>
        /// <param name="type">Type of restriction</param>
        /// <remarks></remarks>
        public static void SetInputFormatRestriction(ref Page Page, HtmlInputText input, FormatRestriction type, string specialChars = "")
        {
            input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" + type.ToString() + "','" + specialChars + "');");
        }

        /// <summary>
        /// Restricts the allowed input in a text box
        /// </summary>
        /// <param name="Page">The page the control belongs to</param>
        /// <param name="input">the control to limit</param>
        /// <param name="type">Type of restriction</param>
        /// <remarks></remarks>
        public static void SetInputFormatRestrictionNoReturn(ref Page Page, TextBox input, FormatRestriction type, string specialChars = "")
        {
            input.Attributes.Add("onKeyPress", "return (event.keyCode != 13 && checkFormat(this,event,'" + type.ToString() + "','" + specialChars + "'));");
        }

        public static void SetRadioList(RadioButtonList oRadio, object DBValue)
        {
            // Pass the RadioButtonList object and the Database value and the correct value will be selected
            if (DBValue is DBNull) return;
            if (DBValue == null) return;
            if (DBValue.ToString().Trim().Length == 0) return;
            try
            {
                oRadio.SelectedValue = DBValue.ToString().Trim();
            }
            catch (Exception)
            {
            }
        }

        public static void ShowPageValidationErrors(IList<ValidationItem> items, ref Page lodPage)
        {
            string InValidFields;

            foreach (ValidationItem Item in items)
            {
                InValidFields = Item.Field;

                if (InValidFields != null)
                {
                    string[] strFields = InValidFields.Trim().Split(',');
                    for (int i = 0; i <= strFields.Length - 1; i++)
                    {
                        WebControl ctl = (WebControl)FindOnTab(strFields[i].Trim(), lodPage);
                        if (ctl != null)
                        {
                            AddCssClass(ctl, "fieldRequired");
                        }
                    }
                }
            }
        }

        public static ALOD.Core.Domain.Workflow.WorkflowStatusOption StringToOption(string input)
        {
            ALOD.Core.Domain.Workflow.WorkflowStatusOption item;

            string[] parts = input.Split(';');
            item = WorkFlowService.GetOptionById(Convert.ToInt32(parts[0]));

            return item;
        }

        public static bool ValidateRequiredField(TextBox control)
        {
            if (control.Text.Trim().Length == 0)
            {
                AddCssClass(control, CSS_FIELD_REQUIRED);
                return false;
            }
            else
            {
                RemoveCssClass(control, CSS_FIELD_REQUIRED);
                return true;
            }
        }

        /// <summary>
        /// Makes the current hostname available to javascript
        /// </summary>
        /// <param name="Page"></param>
        /// <remarks>Useful for Ajax calls</remarks>
        public static void WriteHostName(ref Page Page)
        {
            // build our hostname
            string host = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + Page.ResolveClientUrl(Page.Request.ApplicationPath);

            // Use the actual port from the request instead of hardcoded localhost port
            if (host.Contains("localhost") && Page.Request.Url.Port != 80)
            {
                host = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + ":" + Page.Request.Url.Port.ToString() + Page.ResolveClientUrl(Page.Request.ApplicationPath);
            }
            StringBuilder script = new StringBuilder();

            script.Append("<SCRIPT language=\"javascript\">\r\n");
            script.Append("$_HOSTNAME = '" + host + "';\r\n");
            script.Append("</SCRIPT>\r\n");

            Page.ClientScript.RegisterClientScriptBlock(Page.GetType().BaseType, "HostName", script.ToString());
        }

        #region Workflow Page Utilities...

        public static void InitSeniorMedicalReviewerTabVisibility(SeniorMedicalReviewerTabVisibilityArgs args)
        {
            ILookupDao lookupDao = new NHibernateDaoFactory().GetLookupDao();
            var trackingData = lookupDao.GetStatusTracking(args.RefId, args.ModuleId);

            if (trackingData != null && trackingData.Count > 1)
            {
                foreach (WorkStatusTracking wst in trackingData)
                {
                    if (args.WorkStatusIds.Contains(wst.WorkflowStatus.Id))
                    {
                        return;
                    }
                }
            }

            foreach (TabItem item in args.Steps)
            {
                if (item.Title.Equals(args.TabTitle))
                {
                    item.Visible = false;
                }
            }
        }

        public static void UpdateCaseLock(PageAccess.AccessLevel userAccess, int refId, ModuleType caseModuleType)
        {
            ICaseLockDao lockDao = new NHibernateDaoFactory().GetCaseLockDao();

            lockDao.ClearLocksForUser(SessionInfo.SESSION_USER_ID);

            if (userAccess == PageAccess.AccessLevel.ReadWrite)
            {
                CaseLock @lock = lockDao.GetByReferenceId(refId, caseModuleType);

                if (@lock == null)
                {
                    @lock = new CaseLock();
                    @lock.UserId = SessionInfo.SESSION_USER_ID;
                    @lock.ReferenceId = refId;
                    @lock.ModuleType = caseModuleType;
                    @lock.LockTime = DateTime.Now;

                    lockDao.Save(@lock);
                    lockDao.CommitChanges();
                    SessionInfo.SESSION_LOCK_ID = @lock.Id;
                    SessionInfo.SESSION_LOCK_AQUIRED = true;
                }
                else
                {
                    SessionInfo.SESSION_LOCK_ID = @lock.Id;

                    if (@lock.UserId == SessionInfo.SESSION_USER_ID)
                    {
                        SessionInfo.SESSION_LOCK_AQUIRED = true;
                    }
                    else
                    {
                        SessionInfo.SESSION_LOCK_AQUIRED = false;
                    }
                }
            }
            else
            {
                // No need to check lock, since it will be read-only anyway
                SessionInfo.SESSION_LOCK_ID = 0;
                SessionInfo.SESSION_LOCK_AQUIRED = false;
            }
        }

        public static void VerifyUserAccess(PageAccess.AccessLevel userAccess, string errorMessage, string redirectPage)
        {
            if (userAccess == PageAccess.AccessLevel.None)
            {
                SessionInfo.SetErrorMessage(errorMessage);
                HttpContext.Current.Response.Redirect(redirectPage, true);
            }
        }

        #endregion
    }
}
