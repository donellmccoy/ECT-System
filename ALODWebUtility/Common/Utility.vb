Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.TabNavigation

Namespace Common
    Public Module Utility

        ' This is the date in which Form 348 & 261 documents began getting archived into SRXLite
        ' when a LOD case is completed.
        Public Const ARCHIVE_DATE As Date = #11/8/2013#

        Public Const CSS_FIELD_REQUIRED As String = "fieldRequired"

        Public Const DATE_FORMAT As String = "MM/dd/yyyy"

        Public Const DATE_HOUR_FORMAT As String = "MM/dd/yyyy HHmm"

        Public Const ENGLISH_ALPHABET_UPPERCASE As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        Public Const HOUR_FORMAT As String = "HHmm"

        Public Const KEY_NO_SUB_UNITS_MESSAGE As String = "NO_SUB_UNITS_MESSAGE"

        Public Const KEY_SELECTED_CS_ID As String = "selected_csid"

        Public Const NON_PERMITTED_SPECIAL_CHAR_INPUT As String = "<,>,;,#,^,&,=,{,},[,]"

        Public Const PERMITTED_SPECIAL_CHAR_INPUT As String = "`,~,!,@,$,%,*,(,),-,_,+,\',"",.,,,?,/,\\,|,:, "

        Public Const PHONE_NUMBER_CHARACTERS As String = "(,),-"

        Public Const STRLEN_SSN As Integer = 9

        Public Const WEBSITE_SPECIAL_CHAR_INPUT As String = ".,:,/,?,-,_"

        Public Delegate Function UnitLookUpDelegate(ByVal param As StringDictionary) As List(Of ALOD.Core.Domain.Users.LookUpItem)

        Public ReadOnly Property AppMode() As DeployMode
            Get
                Select Case ConfigurationManager.AppSettings("DeployMode").ToLower()
                    Case "prod"
                        Return DeployMode.Production
                    Case "demo"
                        Return DeployMode.Demo
                    Case "train"
                        Return DeployMode.Training
                    Case "test"
                        Return DeployMode.Test
                    Case Else
                        Return DeployMode.Development
                End Select
            End Get
        End Property

        Public Sub AddCssClass(ByVal control As WebControl, ByVal cssClass As String)
            control.CssClass = control.CssClass + " " + cssClass
        End Sub

        ''' <summary>
        ''' Adds a stylesheet to a page
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="stylesheet"></param>
        ''' <remarks></remarks>
        Public Sub AddStyleSheet(ByVal page As Page, ByVal stylesheet As String)

            Dim link As New HtmlLink
            link.Href = page.ResolveUrl(stylesheet)
            link.Attributes("rel") = "stylesheet"
            link.Attributes("text") = "text/css"

            page.Header.Controls.Add(link)

        End Sub

        Public Function CheckDate(ByVal val As TextBox) As Boolean

            Try
                DateTime.Parse(val.Text.Trim)
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        Public Function CheckTextLength(ByRef Box As TextBox) As Boolean

            If (Box.Text.Trim.Length > Box.MaxLength) Then
                AddCssClass(Box, "fieldRequired")
                Return False
            Else
                RemoveCssClass(Box, "fieldRequired")
                Return True
            End If

        End Function

        Public Function CreateFinding(ByVal lodid As Integer) As ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings

            Dim cFinding As ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings = New ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings()
            cFinding.LODID = lodid
            Dim currUser = UserService.CurrentUser
            cFinding.SSN = currUser.SSN
            cFinding.Compo = currUser.Component
            cFinding.Rank = currUser.Rank.Rank
            cFinding.Grade = currUser.Rank.Grade
            cFinding.Name = currUser.FirstLastName
            cFinding.ModifiedBy = currUser.Id
            cFinding.ModifiedDate = DateTime.Now
            cFinding.CreatedBy = currUser.Id
            cFinding.CreatedDate = DateTime.Now
            Return cFinding

        End Function

        Public Function DoesStringContainNonPermittedCharacters(ByVal s As String)
            'Dim permittedCharacters As List(Of Char) = New List(Of Char)()

            'For Each c As Char In ENGLISH_ALPHABET_UPPERCASE
            '    permittedCharacters.Add(c)
            '    permittedCharacters.Add(Char.ToLower(c))
            'Next

            'For Each c As Char In PERMITTED_SPECIAL_CHAR_INPUT_T.Split("T")
            '    permittedCharacters.Add(c)
            'Next

            'For i As Integer = 0 To 9
            '    permittedCharacters.Add(i.ToString())
            'Next

            'For Each c As Char In s
            '    If (Not permittedCharacters.Contains(c)) Then
            '        Return True
            '    End If
            'Next
            Dim nonPermittedCharacters As List(Of String) = NON_PERMITTED_SPECIAL_CHAR_INPUT.Split(",").ToList()

            For Each c As Char In s
                If (nonPermittedCharacters.Contains(c)) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function DoesStringContainSpecialCharacters(ByVal s As String)
            Dim permittedCharacters As List(Of Char) = New List(Of Char)()

            For Each c As Char In ENGLISH_ALPHABET_UPPERCASE
                permittedCharacters.Add(c)
                permittedCharacters.Add(Char.ToLower(c))
            Next

            For i As Integer = 0 To 9
                permittedCharacters.Add(i.ToString())
            Next

            For Each c As Char In s
                If (Not permittedCharacters.Contains(c)) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function FileSizeUploadLimit() As Integer
            Return IntCheck(ConfigurationManager.AppSettings("InitialFileSizeUploadLimit"))
        End Function

        Public Function FindOnTab(ByVal name As String, ByVal page As Page) As Object
            Return page.Master.Master.FindControl("ContentMain").FindControl("ContentNested").FindControl(name)
        End Function

        Public Function FormatFileName(ByVal fileName As String) As String
            If fileName.IndexOf("\") = -1 Then
                Return fileName
            End If

            Return fileName.Substring(fileName.LastIndexOf("\") + 1)
        End Function

        Public Function FormatSSN(ByVal ssn As String) As String

            If (ssn.Length < 9) Then
                Return ssn
            End If

            Return ssn.Substring(0, 3) + "-" + ssn.Substring(3, 2) + "-" + ssn.Substring(5, 4)

        End Function

        Public Function FormatSSN(ByVal ssn As String, ByVal useMask As Boolean) As String

            If (ssn.Length < 9) Then
                Return ssn
            End If

            If (Not useMask) Then
                Return FormatSSN(ssn)
            End If

            Return "XXX-XX-" + ssn.Substring(5, 4)

        End Function

        Public Function GetCalendarImage(ByRef page As Page) As String
            Return page.ResolveClientUrl("~/App_Themes/" + page.Theme + "/Images/Calendar.gif")
        End Function

        Public Function GetCompoAbbr(ByVal compo As String) As String
            Select Case compo
                Case "1"
                    Return "A"
                Case "2"
                    Return "ARNG"
                Case "3"
                    Return "USARC"
                Case "4"
                    Return "AF"
                Case "5"
                    Return "ANG"
                Case "6"
                    Return "AFRC"
                Case "7"
                    Return "N"
                Case "8"
                    Return "NNG"
                Case "9"
                    Return "NR"
            End Select

            Return ""
        End Function

        Public Function GetCompoString(ByVal compo As String) As String
            Select Case compo
                Case "1"
                    Return "Active Army"
                Case "2"
                    Return "Army National Guard"
                Case "3"
                    Return "Army Reserve"
                Case "4"
                    Return "Active Air Force"
                Case "5"
                    Return "Air National Guard"
                Case "6"
                    Return "Air Force Reserve"
                Case "7"
                    Return "Active Navy"
                Case "8"
                    Return "Navy National Guard"
                Case "9"
                    Return "Navy Reserve"
            End Select

            Return ""
        End Function

        Public Function GetDropDownListNullableSelectedValue(ByVal ddl As DropDownList, ByVal nullableValue As Integer) As Nullable(Of Integer)
            If (ddl.SelectedValue = nullableValue) Then
                Return Nothing
            Else
                Return ddl.SelectedValue
            End If
        End Function

        Public Function GetFileSizeMB(ByVal fileSizeBytes As Integer) As Double
            Return Math.Round(fileSizeBytes / 1048576, 2)
        End Function

        Public Function GetFileSizeUploadLimitMB() As Double
            Return GetFileSizeMB(FileSizeUploadLimit())
        End Function

        Public Function GetHostName() As String
            Return HttpContext.Current.Request.Url.Scheme + Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host '+ HttpContext.Current.Request.ApplicationPath
        End Function

        Public Function GetMultiListSelectedValues(ByVal ctl As Object) As String

            Dim sb As New System.Text.StringBuilder(200)
            Dim lstItem As ListItem
            Dim i As Integer
            ' Build a concatenated list of selected indices
            For Each lstItem In ctl.Items
                If lstItem.Selected = True Then
                    i = ctl.Items.IndexOf(lstItem)
                    sb.Append(lstItem.Value + ",")
                End If
            Next

            If (sb.Length > 0) Then
                sb = sb.Remove(sb.Length - 1, 1)
            End If

            Return sb.ToString()
        End Function

        Public Function GetPersonnelTypeFromGroup(ByVal groupId As Integer, ByVal formal As Boolean) As PersonnelTypes

            Dim group As UserGroups = CType(groupId, UserGroups)

            Select Case group

                Case UserGroups.MedicalTechnician, UserGroups.MedTech_Pilot
                    Return PersonnelTypes.MED_TECH

                Case UserGroups.MedicalOfficer, UserGroups.MedOfficer_Pilot
                    Return PersonnelTypes.MED_OFF

                Case UserGroups.UnitCommander, UserGroups.UnitCC_LODV3
                    Return PersonnelTypes.UNIT_CMDR

                Case UserGroups.WingJudgeAdvocate, UserGroups.WingJA_Pilot
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_WING_JA
                    Else
                        Return PersonnelTypes.WING_JA
                    End If

                Case UserGroups.WingCommander, UserGroups.AppointingAuthority_Pilot

                    If (formal) Then
                        Return PersonnelTypes.FORMAL_APP_AUTH
                    Else
                        Return PersonnelTypes.APPOINT_AUTH
                    End If

                Case UserGroups.BoardTechnician
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_BOARD_RA
                    Else
                        Return PersonnelTypes.BOARD
                    End If

                Case UserGroups.BoardLegal
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_BOARD_JA
                    Else
                        Return PersonnelTypes.BOARD_JA
                    End If

                Case UserGroups.BoardMedical
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_BOARD_SG
                    Else
                        Return PersonnelTypes.BOARD_SG
                    End If

                Case UserGroups.BoardApprovalAuthority
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_BOARD_AA
                    Else
                        Return PersonnelTypes.BOARD_AA
                    End If

                Case UserGroups.MPF
                    Return PersonnelTypes.MPF

                Case UserGroups.InvestigatingOfficer
                    Return PersonnelTypes.IO

                Case UserGroups.LOD_MFP
                    Return PersonnelTypes.LOD_MFP

                Case UserGroups.LOD_PM
                    Return PersonnelTypes.LOD_PM

                Case UserGroups.BoardAdministrator
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_BOARD_A1
                    Else
                        Return PersonnelTypes.BOARD_A1
                    End If

                Case UserGroups.AppellateAuthority
                    Return PersonnelTypes.APPELLATE_AUTH

                Case UserGroups.RSL
                    Return PersonnelTypes.WING_SARC_RSL

                Case UserGroups.SARCAdmin
                    Return PersonnelTypes.SARC_ADMIN

                Case UserGroups.WingSarc
                    Return PersonnelTypes.WING_SARC_RSL

                Case UserGroups.SeniorMedicalReviewer
                    If (formal) Then
                        Return PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER
                    Else
                        Return PersonnelTypes.SENIOR_MEDICAL_REVIEWER
                    End If

                Case Else
                    Return Nothing

            End Select

        End Function

        Public Function GetReportingViewDescription(ByVal reportView As Byte) As String

            Dim description As String = "Default View"

            Select Case reportView
                Case 1
                    description = "Total View"
                Case 2
                    description = "Non Medical Reporting View"
                Case 3
                    description = "Medical Reporting View"
                Case 4
                    description = "RMU View (Physical Responsibility)"
                Case 5
                    description = "JA View"
                Case 6
                    description = "MPF View"
                Case 7
                    description = "System Administration View"

            End Select

            Return description

        End Function

        Public Function GetSearchPermissionByModuleId(moduleId As ModuleType) As String
            Select Case moduleId
                Case ModuleType.LOD
                    Return "lodSearch"
                Case ModuleType.ReinvestigationRequest
                    Return "reinvestigateSearch"
                Case ModuleType.SpecCaseBCMR
                    Return "BCMRSearch"
                Case ModuleType.SpecCaseBMT
                    Return "BMTSearch"
                Case ModuleType.SpecCaseMEPS
                    Return "BMTSearch"
                Case ModuleType.SpecCaseCMAS
                    Return "CMASSearch"
                Case ModuleType.SpecCaseCongress
                    Return "CISearch"
                Case ModuleType.SpecCaseFT
                    Return "FTSearch"
                Case ModuleType.SpecCaseIncap
                    Return "INCAPSearch"
                Case ModuleType.SpecCaseMEB
                    Return "MEBSearch"
                Case ModuleType.SpecCasePW
                    Return "PWSearch"
                Case ModuleType.SpecCaseWWD
                    Return "WWDSearch"
                Case ModuleType.SpecCaseMH
                    Return "MHSearch"
                Case ModuleType.SpecCaseNE
                    Return "NESearch"
                Case ModuleType.SpecCaseDW
                    Return "DWSearch"
                Case ModuleType.SpecCaseMO
                    Return "MOSearch"
                Case ModuleType.SpecCasePEPP
                    Return "PEPPSearch"
                Case ModuleType.SpecCaseRS
                    Return "RSSearch"
                Case ModuleType.SpecCaseRW
                    Return "RWSearch"
                Case ModuleType.SpecCasePH
                    Return "PHSearch"
                Case ModuleType.AppealRequest
                    Return "APSearch"
                Case ModuleType.SARCAppeal
                    Return "RSARCAppealSearch"
                Case ModuleType.SpecCaseAGR  ' Darel 04/29/2020
                    Return "AGRCertSearch"
                Case ModuleType.SpecCasePSCD
                    Return "APSearch"
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function GetUserName(ByVal userId As Integer) As String

            If (userId = Nothing) Then
                Return ""
            End If

            Return UserService.GetById(userId).FullName
        End Function

        Public Function GetViewPermissionByModuleId(ByVal moduleId As ModuleType) As String
            Select Case moduleId
                Case ModuleType.LOD
                    Return "lodView"
                Case ModuleType.ReinvestigationRequest
                    Return "RRView"
                Case ModuleType.SpecCaseBCMR
                    Return "BCMRView"
                Case ModuleType.SpecCaseBMT
                    Return "BMTView"
                Case ModuleType.SpecCaseMEPS
                    Return "MEPSView"
                Case ModuleType.SpecCaseCMAS
                    Return "CMASView"
                Case ModuleType.SpecCaseCongress
                    Return "CIView"
                Case ModuleType.SpecCaseFT
                    Return "IRILOView"
                Case ModuleType.SpecCaseIncap
                    Return "INCAPView"
                Case ModuleType.SpecCaseMEB
                    Return "MEBView"
                Case ModuleType.SpecCasePW
                    Return "PWView"
                Case ModuleType.SpecCaseWWD
                    Return "WWDView"
                Case ModuleType.SpecCaseMH
                    Return "MHView"
                Case ModuleType.SpecCaseNE
                    Return "NEView"
                Case ModuleType.SpecCaseDW
                    Return "DWView"
                Case ModuleType.SpecCaseMO
                    Return "MOView"
                Case ModuleType.SpecCasePEPP
                    Return "PEPPView"
                Case ModuleType.SpecCaseRS
                    Return "RSView"
                Case ModuleType.SpecCaseRW
                    Return "RWView"
                Case ModuleType.SpecCasePH
                    Return "PHView"
                Case ModuleType.AppealRequest
                    Return "APView"
                Case ModuleType.SARCAppeal
                    Return "RSARCAppealView"
                Case ModuleType.SpecCaseAGR
                    Return "AGRCertView"    ' Darel 06/06/2020
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function GetWorkflowInitPageURL(ByVal moduleId As ModuleType, ByVal refId As Integer) As String
            Dim url As String = String.Empty
            Dim reference As String = "refId="

            Select Case moduleId
                Case ModuleType.LOD
                    url = "~/Secure/lod/init.aspx?"
                Case ModuleType.ReinvestigationRequest
                    url = "~/Secure/ReinvestigationRequests/init.aspx?"
                    reference = "requestId="
                Case ModuleType.AppealRequest
                    url = "~/Secure/AppealRequest/init.aspx?"
                    reference = "requestId="
                Case ModuleType.SARC
                    url = "~/Secure/SARC/init.aspx?"
                Case ModuleType.SARCAppeal
                    url = "~/Secure/SARC_Appeal/init.aspx?"
                    reference = "requestId="
                Case ModuleType.SpecCaseBCMR
                    url = "~/Secure/SC_BCMR/init.aspx?"
                Case ModuleType.SpecCaseBMT
                    url = "~/Secure/SC_BMT/init.aspx?"
                Case ModuleType.SpecCaseCMAS
                    url = "~/Secure/SC_CMAS/init.aspx?"
                Case ModuleType.SpecCaseCongress
                    url = "~/Secure/SC_Congress/init.aspx?"
                Case ModuleType.SpecCaseFT
                    url = "~/Secure/SC_FastTrack/init.aspx?"
                Case ModuleType.SpecCaseIncap
                    url = "~/Secure/SC_Incap/init.aspx?"
                Case ModuleType.SpecCaseMEB
                    url = "~/Secure/SC_MEB/init.aspx?"
                Case ModuleType.SpecCaseMEPS
                    url = "~/Secure/SC_MEPS/init.aspx?"
                Case ModuleType.SpecCasePW
                    url = "~/Secure/SC_PWaivers/init.aspx?"
                Case ModuleType.SpecCaseWWD
                    url = "~/Secure/SC_WWD/init.aspx?"
                Case ModuleType.SpecCaseMH
                    url = "~/Secure/SC_MH/init.aspx?"
                Case ModuleType.SpecCaseNE
                    url = "~/Secure/SC_NE/init.aspx?"
                Case ModuleType.SpecCaseDW
                    url = "~/Secure/SC_DW/init.aspx?"
                Case ModuleType.SpecCaseMO
                    url = "~/Secure/SC_MO/init.aspx?"
                Case ModuleType.SpecCasePEPP
                    url = "~/Secure/SC_PEPP/init.aspx?"
                Case ModuleType.SpecCaseRS
                    url = "~/Secure/SC_RS/init.aspx?"
                Case ModuleType.SpecCaseRW
                    url = "~/Secure/SC_RW/init.aspx?"
                Case ModuleType.SpecCasePH
                    url = "~/Secure/SC_PH/init.aspx?"
                Case ModuleType.SpecCaseAGR
                    url = "~/Secure/SC_AGRCert/init.aspx?"
                Case ModuleType.SpecCaseMMSO
                    url = "~/Secure/SC_MMSO/init.aspx?"
                Case Else
                    Return String.Empty
            End Select

            Return (url + reference + refId.ToString())
        End Function

        Public Sub HeaderRowBinding(ByVal grid As GridView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs, ByVal defaultColumn As String)

            If (e.Row.RowType <> DataControlRowType.Header) Then
                Exit Sub
            End If

            Dim cellIndex As Integer = -1
            Dim SortColumn As String = defaultColumn

            If (grid.SortExpression.Length > 0) Then
                SortColumn = grid.SortExpression
            End If

            For Each field As DataControlField In grid.Columns
                If (field.SortExpression = SortColumn) Then
                    cellIndex = grid.Columns.IndexOf(field)
                End If
            Next

            If (cellIndex > -1) Then
                If (grid.SortDirection = SortDirection.Ascending) Then
                    e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-asc"
                Else
                    e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-desc"
                End If

            End If
        End Sub

        Public Sub HighlightInvalidField(ByVal oCtrl As WebControl, ByVal isValid As Boolean)
            If Not (oCtrl Is Nothing) Then
                If isValid Then
                    oCtrl.BackColor = Nothing
                Else
                    AddCssClass(oCtrl, "fieldRequired")
                End If
            End If
        End Sub

        Public Function HTMLDecodeNulls(ByVal RawString As String, Optional ByVal ReturnNothing As Boolean = False) As String
            Dim DecodedString As String
            If Not String.IsNullOrEmpty(RawString) Then
                DecodedString = HttpContext.Current.Server.HtmlDecode(RawString)
            Else
                If ReturnNothing Then
                    DecodedString = Nothing
                Else
                    DecodedString = ""
                End If
            End If
            Return DecodedString
        End Function

        Public Function HTMLEncodeNulls(ByVal RawString As String, Optional ByVal ReturnNothing As Boolean = False) As String
            Dim EncodedString As String
            If Not String.IsNullOrEmpty(RawString) Then
                EncodedString = HttpContext.Current.Server.HtmlEncode(RawString)
            Else
                If ReturnNothing Then
                    EncodedString = Nothing
                Else
                    EncodedString = ""
                End If
            End If
            Return EncodedString
        End Function

        Public Function ICDHierarchy(ByVal id As Integer) As String

            If (IsNothing(id)) Then
                Return Nothing
            End If

            Dim items As New StringBuilder

            If (items Is Nothing) Then
                Return Nothing
            End If

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()

            If (icdDao Is Nothing) Then
                Return Nothing
            End If

            Dim code As ICD9Code = icdDao.GetById(id)

            If (code Is Nothing) Then
                Return Nothing
            End If

            items.Insert(0, code.Id.ToString())

            While (Not IsNothing(code.ParentId))
                ' Get the parent code...
                code = icdDao.GetById(code.ParentId)

                If (code Is Nothing) Then
                    Exit While
                End If

                items.Insert(0, code.Id.ToString() & ",")
            End While

            Return items.ToString() 'returns chapter,section,dl1,dl2,dl3,dl4

        End Function

        Public Sub InsertDropDownListEmptyValue(ByVal ddl As DropDownList, ByVal listItemTitle As String)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(listItemTitle)) Then
                Exit Sub
            End If

            Dim firstItem = New ListItem()

            firstItem.Text = listItemTitle
            firstItem.Value = ""

            ddl.Items.Insert(0, firstItem)
        End Sub

        Public Sub InsertDropDownListZeroValue(ByVal ddl As DropDownList, ByVal listItemTitle As String)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(listItemTitle)) Then
                Exit Sub
            End If

            Dim firstItem = New ListItem()

            firstItem.Text = listItemTitle
            firstItem.Value = 0

            ddl.Items.Insert(0, firstItem)
        End Sub

        Public Function IntCheck(ByVal value As Object, Optional ByVal defaultValue As Integer = 0) As Integer
            If Not IsNumeric(value) Then Return defaultValue
            Return CInt(value)
        End Function

        Public Function IsFileSizeValid(ByVal length As Integer) As Boolean
            Return length <= FileSizeUploadLimit()
        End Function

        ''' <summary>
        ''' Function to verify if the "current user/logged user" belongs to the Board.
        ''' </summary>
        ''' <param name="userGroupId">Id to verify.</param>
        ''' <returns>Boolean - Is the user a has a Board Role.</returns>
        ''' <remarks>Currently taking in consideration Board Technician(7), Board Legal(8), Board Medical(9), Approving Authority(88) and HQ AFRC Technician.</remarks>
        Public Function IsUserBelongsToTheBoard(ByVal userGroupId As Integer, ByVal includeSA As Boolean, ByVal includeNF As Boolean) As Boolean

            Dim boardList As New List(Of Integer)

            boardList.Add(UserGroups.BoardLegal)
            boardList.Add(UserGroups.BoardMedical)
            boardList.Add(UserGroups.BoardApprovalAuthority)
            boardList.Add(UserGroups.AFRCHQTechnician)
            boardList.Add(UserGroups.BoardAdministrator)
            boardList.Add(UserGroups.HQAFRCDPH)

            'boardList.Add(UserGroups.ANGBoardLegal)
            'boardList.Add(UserGroups.ANGBoardMedical)
            'boardList.Add(UserGroups.ANGBoardApprovalAuthority)
            'boardList.Add(UserGroups.ANGAFRCHQTechnician)
            'boardList.Add(UserGroups.ANGBoardAdministrator)
            'boardList.Add(UserGroups.ANGHQAFRCDPH)

            If (includeNF) Then  'Include members who do not make findings (Non-Finders)
                boardList.Add(UserGroups.BoardTechnician)
                'boardList.Add(UserGroups.ANGBoardTechnician)
            End If

            If (includeSA) Then
                boardList.Add(UserGroups.SystemAdministrator)
                'boardList.Add(UserGroups.ANGSystemAdministrator)
            End If

            Return boardList.Contains(userGroupId)

        End Function

        Public Function NewFinding(ByVal lodid As Integer) As ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings

            Dim cFinding As ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings = New ALOD.Core.Domain.Modules.Lod.LineOfDutyFindings()
            cFinding.LODID = lodid
            cFinding.ModifiedBy = SESSION_USER_ID
            cFinding.ModifiedDate = DateTime.Now
            cFinding.CreatedBy = SESSION_USER_ID
            cFinding.CreatedDate = DateTime.Now
            Return cFinding

        End Function

        Public Function NullStringToEmptyString(ByVal value As String) As String
            If Not String.IsNullOrEmpty(value) Then
                Return value
            Else
                Return String.Empty
            End If
        End Function

        Public Function OptionToString(ByVal item As ALOD.Core.Domain.Workflow.WorkflowStatusOption) As String
            Dim buffer As New StringBuilder
            buffer.Append(item.Id.ToString() + ";")
            buffer.Append(item.wsStatusOut.ToString() + ";")
            buffer.Append(item.Template.ToString())

            Return buffer.ToString()
        End Function

        Public Function ParseDateAndTime(ByVal inputDate As String) As Object
            Dim culture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo("en-US")
            Return DateTime.ParseExact(inputDate, DATE_HOUR_FORMAT, culture)
        End Function

        Public Sub RemoveCssClass(ByVal control As WebControl, ByVal cssClass As String)
            control.CssClass = control.CssClass.Replace(cssClass, "")
        End Sub

        Public Sub RemoveDropDownListValue(ByVal ddl As DropDownList, ByVal listItemTitle As String)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(listItemTitle)) Then
                Exit Sub
            End If

            Dim index As Int16
            Dim isInList As Boolean
            For Each x As ListItem In ddl.Items
                If (x.Text.Equals(listItemTitle)) Then
                    isInList = True
                    index = x.Value
                    Exit For
                End If
            Next
            If (isInList) Then
                ddl.Items.RemoveAt(index)
            End If
        End Sub

        Public Sub RunStartupScript(ByVal Page As System.Web.UI.Page, ByVal key As String, ByVal script As String)

            Dim buffer As New StringBuilder
            buffer.Append("<SCRIPT language=""javascript"">" & vbCrLf)
            buffer.Append(script + vbCrLf)
            buffer.Append("</SCRIPT>" & vbCrLf)

            Page.ClientScript.RegisterClientScriptBlock(Page.GetType().BaseType, key, buffer.ToString())

        End Sub

        Public Sub SetDefaultButton(ByVal input As TextBox, ByVal defaultButton As Button)
            input.Attributes.Add("onkeydown", "defaultButton('" + defaultButton.ClientID + "');")
        End Sub

        ''' <summary>
        ''' Restricts the allowed input in a text box
        ''' </summary>
        ''' <param name="Page">The page the control belongs to</param>
        ''' <param name="input">the control to limit</param>
        ''' <param name="type">Type of restriction</param>
        ''' <remarks></remarks>
        Public Sub SetInputFormatRestriction(ByRef Page As System.Web.UI.Page, ByVal input As System.Web.UI.WebControls.TextBox, ByVal type As FormatRestriction, Optional ByVal specialChars As String = "")
            'input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" & type.ToString() & "');")
            input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" & type.ToString() & "','" & specialChars & "');")

        End Sub

        ''' <summary>
        ''' Restricts the allowed input in a text box
        ''' </summary>
        ''' <param name="Page">The page the control belongs to</param>
        ''' <param name="input">the control to limit</param>
        ''' <param name="type">Type of restriction</param>
        ''' <remarks></remarks>
        Public Sub SetInputFormatRestriction(ByRef Page As System.Web.UI.Page, ByVal input As System.Web.UI.HtmlControls.HtmlInputText, ByVal type As FormatRestriction, Optional ByVal specialChars As String = "")
            'input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" & type.ToString() & "');")
            input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" & type.ToString() & "','" & specialChars & "');")

        End Sub

        ''' <summary>
        ''' Restricts the allowed input in a text box
        ''' </summary>
        ''' <param name="Page">The page the control belongs to</param>
        ''' <param name="input">the control to limit</param>
        ''' <param name="type">Type of restriction</param>
        ''' <remarks></remarks>
        Public Sub SetInputFormatRestrictionNoReturn(ByRef Page As System.Web.UI.Page, ByVal input As System.Web.UI.WebControls.TextBox, ByVal type As FormatRestriction, Optional ByVal specialChars As String = "")
            'input.Attributes.Add("onKeyPress", "return checkFormat(this,event,'" & type.ToString() & "');")
            input.Attributes.Add("onKeyPress", "return (event.keyCode != 13 && checkFormat(this,event,'" & type.ToString() & "','" & specialChars & "'));")

        End Sub

        Public Sub SetRadioList(ByVal oRadio As RadioButtonList, ByVal DBValue As Object)
            ' Pass the RadioButtonList object and the Database value and the correct value will be selected
            If (DBValue Is System.DBNull.Value) Then Exit Sub
            If (DBValue Is Nothing) Then Exit Sub
            If 0 = DBValue.ToString.Trim.Length Then Exit Sub
            Try
                oRadio.SelectedValue = DBValue.ToString.Trim
            Catch e As Exception
            End Try
        End Sub

        Public Sub ShowPageValidationErrors(ByVal items As IList(Of ValidationItem), ByRef lodPage As Page)

            Dim InValidFields As String

            For Each Item As ValidationItem In items

                InValidFields = Item.Field

                If Not (InValidFields Is Nothing) Then
                    Dim strFields() As String = InValidFields.Trim.Split(",")
                    Dim i As Integer
                    For i = 0 To strFields.Length - 1
                        Dim ctl As WebControl = FindOnTab(strFields(i).Trim(), lodPage)
                        If (ctl IsNot Nothing) Then
                            AddCssClass(ctl, "fieldRequired")
                        End If
                    Next
                End If
            Next

        End Sub

        Public Function StringToOption(ByVal input As String) As ALOD.Core.Domain.Workflow.WorkflowStatusOption
            Dim item As ALOD.Core.Domain.Workflow.WorkflowStatusOption

            Dim parts() As String = input.Split(";")
            item = WorkFlowService.GetOptionById(CInt(parts(0)))

            Return item
        End Function

        Public Function ValidateRequiredField(ByVal control As TextBox) As Boolean

            If (control.Text.Trim.Length = 0) Then
                AddCssClass(control, CSS_FIELD_REQUIRED)
                Return False
            Else
                RemoveCssClass(control, CSS_FIELD_REQUIRED)
                Return True
            End If

        End Function

        ''' <summary>
        ''' Makes the current hostname available to javascript
        ''' </summary>
        ''' <param name="Page"></param>
        ''' <remarks>Useful for Ajax calls</remarks>
        Public Sub WriteHostName(ByRef Page As System.Web.UI.Page)

            'build our hostname
            Dim host As String = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + Page.ResolveClientUrl(Page.Request.ApplicationPath)

            ' Use the actual port from the request instead of hardcoded localhost port
            If host.Contains("localhost") AndAlso Not Page.Request.Url.Port = 80 Then 
                host = Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + ":" + Page.Request.Url.Port.ToString() + Page.ResolveClientUrl(Page.Request.ApplicationPath)
            End If
            Dim script As New StringBuilder

            script.Append("<SCRIPT language=""javascript"">" & vbCrLf)
            script.Append("$_HOSTNAME = '" + host + "';" + vbCrLf)
            script.Append("</SCRIPT>" & vbCrLf)

            Page.ClientScript.RegisterClientScriptBlock(Page.GetType().BaseType, "HostName", script.ToString())

        End Sub

#Region "Workflow Page Utilities..."

        Public Sub InitSeniorMedicalReviewerTabVisibility(args As SeniorMedicalReviewerTabVisibilityArgs)
            Dim lookupDao As ILookupDao = New NHibernateDaoFactory().GetLookupDao()
            Dim trackingData = lookupDao.GetStatusTracking(args.RefId, args.ModuleId)

            If (Not trackingData Is Nothing AndAlso trackingData.Count > 1) Then
                For Each wst As WorkStatusTracking In trackingData
                    If (args.WorkStatusIds.Contains(wst.WorkflowStatus.Id)) Then
                        Exit Sub
                    End If
                Next
            End If

            For Each item As TabItem In args.Steps
                If (item.Title.Equals(args.TabTitle)) Then
                    item.Visible = False
                End If
            Next
        End Sub

        Public Sub UpdateCaseLock(ByVal userAccess As PageAccess.AccessLevel, ByVal refId As Integer, ByVal caseModuleType As ModuleType)
            Dim lockDao As ICaseLockDao = New NHibernateDaoFactory().GetCaseLockDao()

            lockDao.ClearLocksForUser(SESSION_USER_ID)

            If (userAccess = PageAccess.AccessLevel.ReadWrite) Then
                Dim lock As CaseLock = lockDao.GetByReferenceId(refId, caseModuleType)

                If (lock Is Nothing) Then
                    lock = New CaseLock()
                    lock.UserId = SESSION_USER_ID
                    lock.ReferenceId = refId
                    lock.ModuleType = caseModuleType
                    lock.LockTime = DateTime.Now

                    lockDao.Save(lock)
                    lockDao.CommitChanges()
                    SESSION_LOCK_ID = lock.Id
                    SESSION_LOCK_AQUIRED = True
                Else
                    SESSION_LOCK_ID = lock.Id

                    If (lock.UserId = SESSION_USER_ID) Then
                        SESSION_LOCK_AQUIRED = True
                    Else
                        SESSION_LOCK_AQUIRED = False
                    End If
                End If
            Else
                ' No need to check lock, since it will be read-only anyway
                SESSION_LOCK_ID = 0
                SESSION_LOCK_AQUIRED = False
            End If
        End Sub

        Public Sub VerifyUserAccess(ByVal userAccess As PageAccess.AccessLevel, ByVal errorMessage As String, ByVal redirectPage As String)
            If (userAccess = PageAccess.AccessLevel.None) Then
                SetErrorMessage(errorMessage)
                HttpContext.Current.Response.Redirect(redirectPage, True)
            End If
        End Sub

#End Region

    End Module
End Namespace