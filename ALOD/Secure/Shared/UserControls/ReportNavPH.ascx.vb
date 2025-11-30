Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_ReportNavPH
        Inherits System.Web.UI.UserControl

#Region "fields/properties"

        Private _phDao As IPsychologicalHealthDao

        Public Event RptClicked(ByVal sender As Object, ByVal e As EventArgs)

        Public ReadOnly Property BeginReportingPeriod As DateTime?
            Get
                ' If a period has not been specified then return the previous priod (i.e. last month)
                If (ddlBeginYear.SelectedIndex <= 0 OrElse ddlBeginMonth.SelectedIndex <= 0) Then
                    Dim previousMonth As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1)
                    ddlBeginYear.SelectedValue = previousMonth.Year
                    ddlBeginMonth.SelectedValue = previousMonth.Month
                    Return previousMonth
                End If

                Return New DateTime(Integer.Parse(ddlBeginYear.SelectedValue), Integer.Parse(ddlBeginMonth.SelectedValue), 1)
            End Get
        End Property

        Public ReadOnly Property Collocated() As Short
            Get
                Return Short.Parse(rblCollocated.SelectedValue)
            End Get
        End Property

        Public ReadOnly Property EndReportingPeriod As DateTime?
            Get
                If (ddlEndYear.SelectedIndex <= 0 OrElse ddlEndMonth.SelectedIndex <= 0) Then
                    Dim previousMonth As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1)
                    ddlEndYear.SelectedValue = previousMonth.Year
                    ddlEndMonth.SelectedValue = previousMonth.Month
                    Return previousMonth
                End If

                Return New DateTime(Integer.Parse(ddlEndYear.SelectedValue), Integer.Parse(ddlEndMonth.SelectedValue), 1)
            End Get
        End Property

        Public ReadOnly Property IncludeComments As Boolean
            Get
                Return chkIncludeComments.Checked
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

        Public ReadOnly Property IncludeTextFields As Boolean
            Get
                Return chkIncludeTextFields.Checked
            End Get
        End Property

        Public ReadOnly Property IsUnitSelected As Boolean
            Get
                If (ddlUnit.SelectedIndex <= 0) Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Public ReadOnly Property OutputFormat As String
            Get
                If (OutputScreen.Checked) Then
                    Return "Browser"
                ElseIf (OutputExcel.Checked) Then
                    Return "Excel"
                Else
                    Return "Browser"
                End If
            End Get
        End Property

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
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

        Public ReadOnly Property UnitType As String
            Get
                Return rblSelect.SelectedItem.Text
            End Get
        End Property

#End Region

        Public Sub AddError(ByVal errorText As String)
            bllErrors.Items.Add(errorText)
        End Sub

        Public Sub ClearErrors()
            bllErrors.Items.Clear()
        End Sub

        Public Sub HideErrors()
            trErrors.Visible = False
        End Sub

        Public Sub ShowErrors()
            trErrors.Visible = True
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If (Not Page.IsPostBack) Then

            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                PopulateUnitDDL("Unit")
                PopulateReportPeriodDDLs()
            End If
        End Sub

        Protected Sub PopulateReportPeriodDDLs()
            ' Populate year ddls...
            Dim smallestYear As Integer = PHDao.GetSmallestReportingPeriodYear()

            ddlBeginYear.Items.Add(New ListItem("-- Select Year --", 0))
            ddlEndYear.Items.Add(New ListItem("-- Select Year --", 0))

            For y As Integer = smallestYear To DateTime.Now.Year
                ddlBeginYear.Items.Add(New ListItem(y, y))
                ddlEndYear.Items.Add(New ListItem(y, y))
            Next

            ' Populate month ddls...
            ddlBeginMonth.Items.Add(New ListItem("-- Select Month --", 0))
            ddlEndMonth.Items.Add(New ListItem("-- Select Month --", 0))

            For i As Integer = 1 To 12
                ddlBeginMonth.Items.Add(New ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i))
                ddlEndMonth.Items.Add(New ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i))
            Next
        End Sub

        Protected Sub PopulateUnitDDL(ByVal itemType As String)
            Select Case itemType
                Case "Unit"
                    PopulateWithUnits(CInt(Session("UnitId")), CInt(Session("ReportView")))

                Case "NAF"
                    PopulateWithNAFs()

            End Select
        End Sub

        Protected Sub PopulateWithNAFs()
            ddlUnit.DataSource = PHDao.GetNumberedAirForcesForPH()
            ddlUnit.DataBind()

            Utility.InsertDropDownListZeroValue(ddlUnit, "-- Select NAF --")

            ddlUnit.SelectedIndex = 0
        End Sub

        Protected Sub PopulateWithUnits(ByVal cs_id As Integer, ByVal viewType As Integer)
            Dim units = From u In LookupService.GetChildUnits(cs_id, viewType)
                        Select u
                        Order By u.Name

            ddlUnit.DataSource = units
            ddlUnit.DataBind()
            SetDropdownByValue(ddlUnit, cs_id.ToString())
        End Sub

        Protected Sub rblSelect_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSelect.SelectedIndexChanged
            If (rblSelect.SelectedIndex = -1) Then
                Exit Sub
            End If

            PopulateUnitDDL(rblSelect.SelectedItem.Text)
        End Sub

        Protected Sub ReportButton_Click(sender As Object, e As EventArgs) Handles ReportButton.Click
            RaiseEvent RptClicked(sender, e)
        End Sub

    End Class

End Namespace