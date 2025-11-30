Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MMSO

    Partial Class Secure_sc_mmso_MedTech
        Inherits System.Web.UI.Page

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

#Region "MMSOProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

        Protected ReadOnly Property MasterPage() As SC_MMSOMaster
            Get
                Dim master As SC_MMSOMaster = CType(Page.Master, SC_MMSOMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub DisplayDataReadOnly()
            Panel1.Enabled = False
            Panel2.Enabled = False
            Panel3.Enabled = False
        End Sub

        Protected Sub DisplayDataReadWrite()
            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            If (SpecCase.Status = SpecCaseMMSOWorkStatus.InitiateCase) Then
                access = SectionList(MMSectionNames.MM_INPUT.ToString())
                Panel1.Enabled = True
                Panel2.Enabled = True
                Panel3.Enabled = False
            ElseIf SpecCase.Status = SpecCaseMMSOWorkStatus.SMRInput Then
                access = SectionList(MMSectionNames.MM_UCINPUT.ToString())
                Panel1.Enabled = False
                Panel2.Enabled = False
                Panel3.Enabled = True
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()

                SigCheck.VerifySignature(ReferenceId)

                LogManager.LogAction(ModuleType.SpecCaseMMSO, UserAction.ViewPage, RefId, "Viewed Page: Med Tech")

            End If

        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, StreetTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, cityTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, zipTB, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, phoneTB, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, Injury_Illness_DateTB, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DateInTB, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DateOutTB, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DiagnosisTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, FollowupCareTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ProviderTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ProviderPOCTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ProviderPOCPhoneTB, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, MTFNameTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, MTFDateTB, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, ProfileInfoTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, NearestMTFNameTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DistanceTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitOfAssignmentNameTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UIC_OFPAC_TB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitAddressTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitAddress2TB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitCityTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitStateTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitZipTB, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, UnitPOCNameTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitPOCTitleTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, UnitPOCPhoneTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            SetTextBoxFromString(StreetTB, Server.HtmlDecode(SpecCase.Member_Address_Street))
            SetTextBoxFromString(cityTB, Server.HtmlDecode(SpecCase.Member_Address_City))
            SetTextBoxFromString(zipTB, Server.HtmlDecode(SpecCase.Member_Address_Zip))
            SetTextBoxFromString(phoneTB, Server.HtmlDecode(SpecCase.Member_Home_Phone))
            If (Not SpecCase.Member_Address_State Is Nothing) Then
                stateDDL.SelectedValue = SpecCase.Member_Address_State
            End If
            If (Not SpecCase.Member_Tricare_Region Is Nothing) Then
                tricareDDL.SelectedValue = SpecCase.Member_Tricare_Region
            End If
            If (Not SpecCase.DateIn Is Nothing) Then
                DateInTB.Text = Server.HtmlDecode(SpecCase.DateIn.Value)
            End If
            If (Not SpecCase.DateOut Is Nothing) Then
                DateOutTB.Text = Server.HtmlDecode(SpecCase.DateOut.Value)
            End If
            SetTextBoxFromString(DiagnosisTB, Server.HtmlDecode(SpecCase.Medical_Diagnosis))
            SetTextBoxFromString(FollowupCareTB, Server.HtmlDecode(SpecCase.Follow_Up_Care))
            SetTextBoxFromString(ProviderTB, Server.HtmlDecode(SpecCase.Medical_Provider))
            SetTextBoxFromString(ProviderPOCTB, Server.HtmlDecode(SpecCase.Provider_POC))
            SetTextBoxFromString(ProviderPOCPhoneTB, Server.HtmlDecode(SpecCase.Provider_POC_Phone))
            If (SpecCase.InjuryIllnessDate.HasValue) Then
                SetTextBoxFromString(Injury_Illness_DateTB, Server.HtmlDecode(SpecCase.InjuryIllnessDate.Value.ToShortDateString()))
            End If
            SetTextBoxFromString(MTFNameTB, Server.HtmlDecode(SpecCase.Military_Treatment_Facility_Initial))
            If SpecCase.MTF_Initial_Treatment_Date.HasValue Then
                SetTextBoxFromString(MTFDateTB, Server.HtmlDecode(SpecCase.MTF_Initial_Treatment_Date))
            End If
            SetTextBoxFromString(ProfileInfoTB, Server.HtmlDecode(SpecCase.Medical_Profile_Info))

            If (Not SpecCase.ICD9Id Is Nothing) Then
                ICD9TB.Text = SpecCase.ICD9Id
            End If

            SetTextBoxFromString(NearestMTFNameTB, Server.HtmlDecode(SpecCase.Military_Treatment_Facility_Suggested))
            If (Not SpecCase.MTF_Suggested_Distance Is Nothing) Then
                DistanceTB.Text = Server.HtmlDecode(SpecCase.MTF_Suggested_Distance.Value.ToString())
            End If
            If (Not SpecCase.MTF_Suggested_Choice Is Nothing) Then
                If SpecCase.MTF_Suggested_Choice > 0 Then
                    MTFLocationType.SelectedValue = SpecCase.MTF_Suggested_Choice.Value.ToString()
                End If
            End If
            SetTextBoxFromString(UnitOfAssignmentNameTB, Server.HtmlDecode(SpecCase.UnitOfAssignment))
            SetTextBoxFromString(UnitAddressTB, Server.HtmlDecode(SpecCase.Unit_Address1))
            SetTextBoxFromString(UnitAddress2TB, Server.HtmlDecode(SpecCase.Unit_Address2))
            SetTextBoxFromString(UnitCityTB, Server.HtmlDecode(SpecCase.Unit_City))
            SetTextBoxFromString(UnitStateTB, Server.HtmlDecode(SpecCase.Unit_State))
            SetTextBoxFromString(UnitZipTB, Server.HtmlDecode(SpecCase.Unit_Zip))
            SetTextBoxFromString(UnitPOCNameTB, Server.HtmlDecode(SpecCase.Unit_POC_name))
            SetTextBoxFromString(UnitPOCTitleTB, Server.HtmlDecode(SpecCase.Unit_POC_title))
            SetTextBoxFromString(UnitPOCPhoneTB, Server.HtmlDecode(SpecCase.Unit_POC_Phone))
            SetTextBoxFromString(UIC_OFPAC_TB, Server.HtmlDecode(SpecCase.Unit_UIC))

            UnitPOCRankDDL.DataSource = Services.LookupService.GetRanksAndGrades()
            UnitPOCRankDDL.DataTextField = "Title"
            UnitPOCRankDDL.DataValueField = "Id"
            UnitPOCRankDDL.DataBind()
            UnitPOCRankDDL.Items.Insert(0, New ListItem("-Select-", ""))
            If SpecCase.Unit_POC_rank Is Nothing Then
                UnitPOCRankDDL.SelectedValue = ""
            Else
                UnitPOCRankDDL.SelectedValue = SpecCase.Unit_POC_rank.Value.ToString
            End If
            DisplayDataReadWrite()

            If UserCanEdit Then
                DateInTB.CssClass = "datePicker"
                DateOutTB.CssClass = "datePicker"
                Injury_Illness_DateTB.CssClass = "datePicker"
            Else
                DisplayDataReadOnly()
            End If
        End Sub

        Private Sub SaveFindings()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SpecCase.Member_Address_Street = Server.HtmlEncode(StreetTB.Text)
            SpecCase.Member_Address_City = Server.HtmlEncode(cityTB.Text)
            SpecCase.Member_Address_State = stateDDL.SelectedValue
            SpecCase.Member_Address_Zip = Server.HtmlEncode(zipTB.Text)
            SpecCase.Member_Home_Phone = Server.HtmlEncode(phoneTB.Text)
            SpecCase.Member_Tricare_Region = Convert.ToInt32(tricareDDL.SelectedValue)
            SpecCase.Medical_Diagnosis = Server.HtmlEncode(DiagnosisTB.Text)
            SpecCase.Follow_Up_Care = Server.HtmlEncode(FollowupCareTB.Text)
            SpecCase.Medical_Provider = Server.HtmlEncode(ProviderTB.Text)
            SpecCase.Provider_POC = Server.HtmlEncode(ProviderPOCTB.Text)
            SpecCase.Provider_POC_Phone = Server.HtmlEncode(ProviderPOCPhoneTB.Text)

            'Date Field
            Try
                If (CheckDate(Injury_Illness_DateTB)) Then
                    If (Injury_Illness_DateTB.Text.Trim.Length > 0) Then
                        SpecCase.InjuryIllnessDate = Server.HtmlEncode(DateTime.Parse(Injury_Illness_DateTB.Text.Trim))
                    End If
                Else
                    SpecCase.InjuryIllnessDate = Nothing
                End If
            Catch ex As Exception
                SpecCase.InjuryIllnessDate = Nothing
            End Try

            'Date Field
            Try
                If (CheckDate(MTFDateTB)) Then
                    If (MTFDateTB.Text.Trim.Length > 0) Then
                        SpecCase.MTF_Initial_Treatment_Date = Server.HtmlEncode(DateTime.Parse(MTFDateTB.Text.Trim))
                    End If
                Else
                    SpecCase.MTF_Initial_Treatment_Date = Nothing
                End If
            Catch ex As Exception
                SpecCase.MTF_Initial_Treatment_Date = Nothing
            End Try

            SpecCase.Military_Treatment_Facility_Initial = Server.HtmlEncode(MTFNameTB.Text)
            If MTFLocationType.SelectedValue = "1" Or MTFLocationType.SelectedValue = "2" Then
                SpecCase.MTF_Suggested_Choice = Convert.ToInt32(MTFLocationType.SelectedValue)
            End If
            SpecCase.Medical_Profile_Info = Server.HtmlEncode(ProfileInfoTB.Text)

            SpecCase.Military_Treatment_Facility_Suggested = Server.HtmlEncode(NearestMTFNameTB.Text)
            If Not DistanceTB.Text = "" Then
                SpecCase.MTF_Suggested_Distance = Server.HtmlEncode(Convert.ToInt32(DistanceTB.Text))
            End If
            SpecCase.Unit_POC_name = Server.HtmlEncode(UnitPOCNameTB.Text)
            SpecCase.Unit_POC_Phone = Server.HtmlEncode(UnitPOCPhoneTB.Text)
            SpecCase.Unit_POC_title = Server.HtmlEncode(UnitPOCTitleTB.Text)
            If UnitPOCRankDDL.SelectedValue <> "" Then
                SpecCase.Unit_POC_rank = Convert.ToInt32(UnitPOCRankDDL.SelectedValue)
            End If
            'Date Field
            Try
                If (CheckDate(DateInTB)) Then
                    If (DateInTB.Text.Trim.Length > 0) Then
                        SpecCase.DateIn = Server.HtmlEncode(DateTime.Parse(DateInTB.Text.Trim))
                    End If
                Else
                    SpecCase.DateIn = Nothing
                End If
            Catch ex As Exception
                SpecCase.DateIn = Nothing
            End Try

            'Date Field
            Try
                If (CheckDate(DateOutTB)) Then
                    If (DateOutTB.Text.Trim.Length > 0) Then
                        SpecCase.DateOut = Server.HtmlEncode(DateTime.Parse(DateOutTB.Text.Trim))
                    End If
                Else
                    SpecCase.DateOut = Nothing
                End If
            Catch ex As Exception
                SpecCase.DateOut = Nothing
            End Try

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

        End Sub

        Private Sub SetTextBoxFromString(ByVal tb As TextBox, ByVal DataVal As String)
            If Not (DataVal Is Nothing) Then
                tb.Text = DataVal
            End If
        End Sub

#Region "TabEvent"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

#End Region

    End Class

End Namespace