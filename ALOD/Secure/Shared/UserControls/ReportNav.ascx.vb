Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_ReportNav
        Inherits System.Web.UI.UserControl

        Private Const subtractDays As Double = -30

        Public Event RptClicked(ByVal sender As Object, ByVal e As EventArgs)

#Region "fields/properties"

        Private _listOfLabels As New List(Of Label)

        Public Property BeginDate() As Nullable(Of Date)
            Get
                If (rdbSSN.Checked And txtSSN.Text.Length > 0 And txtSSN.Visible) Then
                    txtBeginDate.Text = ""
                    Return Nothing
                ElseIf (rdbName.Checked AndAlso Not IsMemberNameInvalid) Then
                    txtBeginDate.Text = ""
                    Return Nothing
                ElseIf (txtBeginDate.Text.Length = 0) Then
                    txtBeginDate.Text = Now.AddDays(subtractDays).ToShortDateString()
                End If
                Return Server.HtmlEncode(CDate(txtBeginDate.Text))
            End Get
            Set(ByVal value As Nullable(Of Date))
                txtBeginDate.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public Property EndDate() As Nullable(Of Date)
            Get
                If (rdbSSN.Checked And txtSSN.Text.Length > 0 And txtSSN.Visible) Then
                    txtEndDate.Text = ""
                    Return Nothing
                ElseIf (rdbName.Checked AndAlso Not IsMemberNameInvalid) Then
                    txtEndDate.Text = ""
                    Return Nothing
                ElseIf (txtEndDate.Text.Length = 0) Then
                    txtEndDate.Text = Now.ToShortDateString()
                End If
                Return Server.HtmlEncode(CDate(txtEndDate.Text))
            End Get
            Set(ByVal value As Nullable(Of Date))
                txtEndDate.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public ReadOnly Property GetCompo() As String
            Get
                Return CompoSelect.SelectedValue
            End Get
        End Property

        Public Property IncludeSubordinate() As Boolean
            Get
                Return chkSubordinateUnit.Checked
            End Get
            Set(ByVal value As Boolean)
                chkSubordinateUnit.Checked = value
            End Set
        End Property

        Public WriteOnly Property InvalidMemberNameErrorLabelVisibility() As Boolean
            Set(value As Boolean)
                lblInvalidMemberName.Visible = value
            End Set
        End Property

        Public Property IsComplete() As Int16
            Get
                Return rblLodStatus.SelectedValue
            End Get
            Set(ByVal value As Int16)
                SetRadioList(rblLodStatus, value)
            End Set
        End Property

        Public ReadOnly Property IsMemberNameInvalid() As Boolean
            Get
                If (String.IsNullOrEmpty(txtMemberLastName.Text) = True And String.IsNullOrEmpty(txtMemberFirstName.Text) = True And String.IsNullOrEmpty(txtMemberMiddleName.Text) = True) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public Property MemberFirstName() As String
            Get
                Return Server.HtmlEncode(txtMemberFirstName.Text)
            End Get
            Set(value As String)
                txtMemberFirstName.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public Property MemberLastName() As String
            Get
                Return Server.HtmlEncode(txtMemberLastName.Text)
            End Get
            Set(value As String)
                txtMemberLastName.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public Property MemberMiddleName() As String
            Get
                Return Server.HtmlEncode(txtMemberMiddleName.Text)
            End Get
            Set(value As String)
                txtMemberMiddleName.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public ReadOnly Property MemberNameRadioButtonChecked As Boolean
            Get
                Return rdbName.Checked
            End Get
        End Property

        Public WriteOnly Property MemberNotFoundErrorLabelVisibility() As Boolean
            Set(value As Boolean)
                lblMemberNotFound.Visible = value
            End Set
        End Property

        Public Property SortOrder() As String
            Get
                Return SortOrdeDDL.SelectedValue
            End Get
            Set(ByVal value As String)
                SortOrdeDDL.SelectedValue = value
            End Set
        End Property

        Public Property SSN() As String
            Get
                Return Server.HtmlEncode(txtSSN.Text)
            End Get
            Set(ByVal value As String)
                txtSSN.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public ReadOnly Property SSNRadioButtonChecked As Boolean
            Get
                Return rdbSSN.Checked
            End Get
        End Property

        Public Property Unit() As Integer
            Get
                Return ddlUnit.SelectedValue
            End Get
            Set(ByVal value As Integer)
                SetDropdownByValue(ddlUnit, value)
            End Set
        End Property

        Public ReadOnly Property UnitText() As String
            Get
                Return ddlUnit.SelectedItem.Text
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

#End Region

        Public Enum CtrlList As Byte
            Unit
            subordinate
            BeginDate
            EndDate
            SSN
            LODStatus
            SortOrder
            CompoSelect
        End Enum

        Public Sub ControlVisibility(ByVal ctrl As CtrlList, ByVal visible As Boolean)
            Select Case True
                Case ctrl = CtrlList.Unit
                    tr1.Visible = visible
                Case ctrl = CtrlList.subordinate
                    tr2.Visible = visible
                Case ctrl = CtrlList.BeginDate
                    tr3.Visible = visible
                Case ctrl = CtrlList.EndDate
                    tr4.Visible = visible
                Case ctrl = CtrlList.SSN
                    tr5.Visible = visible
                    trRadioButtons.Visible = visible
                    trName.Visible = visible
                Case ctrl = CtrlList.LODStatus
                    tr6.Visible = visible
                Case ctrl = CtrlList.SortOrder
                    tr7.Visible = visible
                Case ctrl = CtrlList.CompoSelect
                    tr8.Visible = visible
                Case Else
            End Select
            RearrangeDisplayLettering()
        End Sub

        Public Sub FillUnitLookup(ByVal cs_id As Integer, ByVal viewType As Integer)

            Dim units = From u In LookupService.GetChildUnits(cs_id, viewType)
                        Select u
                        Order By u.Name

            ddlUnit.DataSource = units
            ddlUnit.DataBind()

            SetDropdownByValue(ddlUnit, cs_id.ToString())
        End Sub

        Protected Sub FillCompoLookup()
            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()
            CompoSelect.DataSource = From n In lkupDAO.GetCompos()
            CompoSelect.DataTextField = "Name"
            CompoSelect.DataValueField = "Value"
            CompoSelect.DataBind()
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If (Not Page.IsPostBack) Then
                FillUnitLookup(CInt(Session("UnitId")), CInt(Session("ReportView")))
                FillCompoLookup()
                SetDropdownByValue(ddlUnit, CInt(Session("UnitId")))
                _listOfLabels.Add(ReportLabel1)
                _listOfLabels.Add(ReportLabel2)
                _listOfLabels.Add(ReportLabel3)
                _listOfLabels.Add(ReportLabel4)
                _listOfLabels.Add(lblMemberNameRadioButtons)
                _listOfLabels.Add(ReportLabel5)
                _listOfLabels.Add(lblReportMemberName)
                _listOfLabels.Add(ReportLabel6)
                _listOfLabels.Add(ReportLabel7)
                _listOfLabels.Add(CompoLabel)
                _listOfLabels.Add(ReportLabel8)

                tr5.Visible = True
                trName.Visible = False
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                SetInputFormatRestriction(Page, txtBeginDate, FormatRestriction.AlphaNumeric, "/")
                SetInputFormatRestriction(Page, txtEndDate, FormatRestriction.AlphaNumeric, "/")
                SetInputFormatRestriction(Page, txtSSN, FormatRestriction.Numeric)
            End If
        End Sub

        Protected Sub ReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReportButton.Click
            RaiseEvent RptClicked(sender, e)
        End Sub

        Protected Sub SearchSelectionRadioButton_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbSSN.CheckedChanged, rdbName.CheckedChanged
            If (rdbSSN.Checked) Then
                tr5.Visible = True
                trName.Visible = False
            Else
                tr5.Visible = False
                trName.Visible = True
            End If
        End Sub

        Private Sub RearrangeDisplayLettering()
            Dim i As Byte = 0
            Dim letterIndex As Byte = 0

            For i = 0 To (_listOfLabels.Count - 1)
                If (_listOfLabels(i).Visible AndAlso letterIndex < Utility.ENGLISH_ALPHABET_UPPERCASE.Length) Then

                    _listOfLabels(i).Text = Utility.ENGLISH_ALPHABET_UPPERCASE(letterIndex)

                    ' Check if the current label is either the Member SSN label or the Member Name label.
                    ' If so then give the two labels the same Text value. These two labels exist on the
                    ' same row; therefore, they need to have the same letter value.
                    If (_listOfLabels(i).Equals(ReportLabel5)) Then
                        lblReportMemberName.Text = Utility.ENGLISH_ALPHABET_UPPERCASE(letterIndex)
                    ElseIf (_listOfLabels(i).Equals(lblReportMemberName)) Then
                        ReportLabel5.Text = Utility.ENGLISH_ALPHABET_UPPERCASE(letterIndex)
                    End If

                    letterIndex = letterIndex + 1

                End If
            Next
        End Sub

    End Class

End Namespace