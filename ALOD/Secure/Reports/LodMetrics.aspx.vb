Namespace Web.Reports

    Partial Class Secure_Reports_LodMetrics
        Inherits System.Web.UI.Page

        '    Dim ds As ALodRptList

        '#Region "fields/property"

        'Private _rowTotal As Decimal = 0
        'Private _columnAllTotal As Decimal = 0
        'Private _medUnitCount As Decimal = 0
        'Private _medOfficerCount As Decimal = 0
        'Private _unit As Decimal = 0
        'Private _wingJACount As Decimal = 0
        'Private _wingCCCount As Decimal = 0
        'Private _lodBoardCount As Decimal = 0
        'Private _columnTotal As Decimal = 0
        'Private _formalInvCount As Decimal = 0

        'Public Property RowTotal() As Decimal
        '    Get
        '        Return _rowTotal
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _rowTotal = value
        '    End Set
        'End Property

        'Public Property MedUnitCount() As Decimal
        '    Get
        '        Return _medUnitCount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _medUnitCount = value
        '    End Set
        'End Property

        'Public Property MedOfficerCount() As Decimal
        '    Get
        '        Return _medOfficerCount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _medOfficerCount = value
        '    End Set
        'End Property

        'Public Property Unit() As Decimal
        '    Get
        '        Return _unit
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _unit = value
        '    End Set
        'End Property

        'Public Property WingJACount() As Decimal
        '    Get
        '        Return _wingJACount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _wingJACount = value
        '    End Set
        'End Property

        'Public Property WingCCCount() As Decimal
        '    Get
        '        Return _wingCCCount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _wingCCCount = value
        '    End Set
        'End Property

        'Public Property LodBoardCount() As Decimal
        '    Get
        '        Return _lodBoardCount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _lodBoardCount = value
        '    End Set
        'End Property

        'Public Property ColumnTotal() As Decimal
        '    Get
        '        Return _columnTotal
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _columnTotal = value
        '    End Set
        'End Property

        'Public Property FormalInvCount() As Decimal
        '    Get
        '        Return _formalInvCount
        '    End Get
        '    Set(ByVal value As Decimal)
        '        _formalInvCount = value
        '    End Set
        'End Property

        '#End Region

        '    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '        If (Not Page.IsPostBack) Then

        '            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SortOrder, False)
        '            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SSN, False)

        '        End If

        '        AddHandler rptNav.RptClicked, AddressOf RunReport
        '    End Sub

        '    Protected Sub MetricsList_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MetricsList.ItemCommand
        '        If (e.CommandName.Equals("drill")) Then

        '            Dim cs_id As Integer = e.CommandArgument
        '            ds = New ALodRptList()

        '            MetricsList.DataSource = ALodRptList.GetCommandByUnit(cs_id, CInt(Session("ReportView")))
        '            MetricsList.DataBind()
        '        End If
        '    End Sub

        '    Protected Sub MetricsList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MetricsList.ItemDataBound
        '        If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
        '            Exit Sub
        '        End If

        '        Dim unitData As System.Data.DataRowView = e.Item.DataItem
        '        Dim metricsRptr As Repeater = CType(e.Item.FindControl("metricsDetail"), Repeater)
        '        Dim metricsCalculation As System.Data.DataSet = ALodRptList.GetMetricRpt(unitData(0), CInt(Session("ReportView")), rptNav.SSN, rptNav.BeginDate, rptNav.EndDate, rptNav.IsComplete, rptNav.IncludeSubordinate)

        '        Dim lnkBtn As LinkButton = CType(e.Item.FindControl("LinkButton1"), LinkButton)
        '        Dim lnkLabel As Label = CType(e.Item.FindControl("UnitLabel"), Label)

        '        _rowTotal = 0.0
        '        _columnAllTotal = 0.0      'To check for the complete total of a row
        '        For colNum As Integer = 0 To metricsCalculation.Tables(0).Columns.Count - 1
        '            For Each row In metricsCalculation.Tables(0).Rows
        '                _rowTotal += row(colNum)
        '                _columnAllTotal += row(colNum)
        '                Select Case metricsCalculation.Tables(0).Columns(colNum).ColumnName
        '                    Case "MedicalUnit"
        '                        _medUnitCount += row(colNum)
        '                    Case "MedicalOfficer"
        '                        _medOfficerCount += row(colNum)
        '                    Case "Unit"
        '                        _unit += row(colNum)
        '                    Case "WingJA"
        '                        _wingJACount += row(colNum)
        '                    Case "WingCC"
        '                        _wingCCCount += row(colNum)
        '                    Case "LODBoard"
        '                        _lodBoardCount += row(colNum)
        '                    Case "FormalInv"
        '                        _formalInvCount += row(colNum)
        '                        _rowTotal -= row(colNum)
        '                End Select
        '            Next
        '        Next

        '        lnkLabel.Visible = _columnAllTotal = 0.0
        '        lnkBtn.Visible = _columnAllTotal <> 0.0

        '        _columnTotal += _rowTotal

        '        metricsRptr.DataSource = metricsCalculation
        '        metricsRptr.DataBind()

        '    End Sub

        '    Public Sub RunReport(ByVal sender As Object, ByVal e As System.EventArgs)
        '        Dim ds As System.Data.DataSet = ALodRptList.GetCommandByUnit(rptNav.Unit, CInt(Session("ReportView")))
        '        MetricsList.DataSource = ALodRptList.FillterDataSet(ds, "cs_id = " + rptNav.Unit.ToString())
        '        MetricsList.DataBind()
        '    End Sub

    End Class

End Namespace