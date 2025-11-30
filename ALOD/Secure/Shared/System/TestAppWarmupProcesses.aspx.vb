Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class TestAppWarmupProcesses
        Inherits System.Web.UI.Page

        Private _appWarmupProcessDao As IApplicationWarmupProcessDao

        Protected ReadOnly Property AppWarmupProcessDao As IApplicationWarmupProcessDao
            Get
                If (_appWarmupProcessDao Is Nothing) Then
                    _appWarmupProcessDao = New NHibernateDaoFactory().GetApplicationWarmupProcessDao()
                End If

                Return _appWarmupProcessDao
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected Sub btnExecuteTest_Click(sender As Object, e As EventArgs) Handles btnExecuteTest.Click
            If (AppMode = DeployMode.Production) Then
                Exit Sub
            End If

            If (ddlTypeOfProcess.SelectedValue = 0) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtTestExecutionDate.Text)) Then
                Exit Sub
            End If

            Dim hostname As String = ConfigurationManager.AppSettings("Hostname")

            Select Case Integer.Parse(ddlTypeOfProcess.SelectedValue)
                Case 1
                    PsychologicalHealthService.ExecuteApplicationWarmupProcesses(Convert.ToDateTime(txtTestExecutionDate.Text), hostname)

                Case 2
                    ReportsService.ExecuteApplicationWarmupProcesses(Convert.ToDateTime(txtTestExecutionDate.Text), hostname)

                Case Else
                    ' Do nothing...
            End Select

            BindDataLogs()
        End Sub

        Protected Sub ddlTypeOfProcess_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTypeOfProcess.SelectedIndexChanged
            If (Integer.Parse(ddlTypeOfProcess.SelectedValue) = 1) Then
                tblPHProcessInfo.Visible = True
                tblReportProcessInfo.Visible = False
            ElseIf (Integer.Parse(ddlTypeOfProcess.SelectedValue) = 2) Then
                tblPHProcessInfo.Visible = False
                tblReportProcessInfo.Visible = True
            Else
                tblPHProcessInfo.Visible = False
                tblReportProcessInfo.Visible = False
            End If
        End Sub

        Protected Sub gdvProcessLogs_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvProcessLogs.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 1) Then
                Exit Sub
            End If

            If (e.CommandName = "DeleteLog") Then
                AppWarmupProcessDao.DeleteLogById(Integer.Parse(parts(0)))
                BindDataLogs()
            End If
        End Sub

        Protected Sub gdvProcessLogs_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvProcessLogs.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim isProductionMode As Boolean = False

            If (AppMode = DeployMode.Production) Then
                isProductionMode = True
            End If

            CType(e.Row.FindControl("btnDeleteLog"), LinkButton).Visible = (Not isProductionMode)
        End Sub

        Protected Sub gdvResults_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvProcessLogs.PageIndexChanging
            gdvProcessLogs.PageIndex = e.NewPageIndex
            BindDataLogs()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Private Sub BindDataLogs()
            gdvProcessLogs.DataSource = AppWarmupProcessDao.GetAllLogs()
            gdvProcessLogs.DataBind()
        End Sub

        Private Sub InitControls()
            SetInputFormatRestriction(Page, txtTestExecutionDate, FormatRestriction.Numeric, "/")
            txtTestExecutionDate.CssClass = "datePickerAll"

            InitProcessTypeDDL()
            BindDataLogs()

            If (AppMode = DeployMode.Production) Then
                tblInput2.Visible = False
                txtTestExecutionDate.Enabled = False
                btnExecuteTest.Enabled = False
                lblDisabled.Visible = True
            End If
        End Sub

        Private Sub InitProcessTypeDDL()
            ddlTypeOfProcess.Items.Add(New ListItem("-- Select Process Type --", 0))
            ddlTypeOfProcess.Items.Add(New ListItem("PH Process", 1))
            ddlTypeOfProcess.Items.Add(New ListItem("Reports Process", 2))
        End Sub

    End Class

End Namespace