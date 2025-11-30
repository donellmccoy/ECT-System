Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Special_Case

    Partial Class Secure_sc_MyCases
        Inherits System.Web.UI.Page

        Private _workflowDao As IWorkflowDao

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Protected Sub gvResults10_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults10.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults11_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults11.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults12_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults12.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults13_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults13.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults14_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults14.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults16_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvResults16.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                Select Case args.Type
                    Case ModuleType.SpecCaseMMSO
                        args.Url = "~/Secure/SC_MMSO/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case Else
                        Throw New ApplicationException("Incorrect case type")
                End Select

            End If
        End Sub

        Protected Sub gvResults17_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults17.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults18_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults18.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults19_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults19.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults20_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults20.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults21_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults21.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults22_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults22.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults23_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults23.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults28_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults28.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults6_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults6.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If
        End Sub

        Protected Sub gvResults7_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults7.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults8_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults8.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub gvResults9_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults9.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                'Dim lockId As Integer = 0
                'Integer.TryParse(data("lockId"), lockId)

                'If (lockId > 0) Then
                '    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                'End If
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                'clear any pending locks this user might have

                Dim dao As ALOD.Core.Interfaces.DAOInterfaces.ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)
                'Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                'RegisterAsyncTask(task)
            End If

            If Not UserHasPermission("MMSOSearch") Then
                MMSODiv.Visible = False
            End If
        End Sub

        Protected Sub SCData_Module10_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module10.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCasePW
            e.InputParameters("userId") = SESSION_USER_ID
        End Sub

        Protected Sub SCData_Module11_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module11.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseMEB
        End Sub

        Protected Sub SCData_Module12_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module12.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseBCMR
        End Sub

        Protected Sub SCData_Module13_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module13.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseFT
            e.InputParameters("userId") = SESSION_USER_ID
        End Sub

        Protected Sub SCData_Module14_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module14.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseCMAS
        End Sub

        Protected Sub SCData_Module16_Selecting(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module16.Selecting

            e.InputParameters("moduleId") = ModuleType.SpecCaseMMSO
        End Sub

        Protected Sub SCData_Module17_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module17.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseMH
        End Sub

        Protected Sub SCData_Module18_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module18.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseNE
        End Sub

        Protected Sub SCData_Module19_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module19.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseDW
        End Sub

        Protected Sub SCData_Module20_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module20.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseMO
        End Sub

        Protected Sub SCData_Module21_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module21.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCasePEPP
        End Sub

        Protected Sub SCData_Module22_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module22.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseRS
        End Sub

        Protected Sub SCData_Module23_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module23.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCasePH
        End Sub

        Protected Sub SCData_Module28_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module28.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseAGR
        End Sub

        Protected Sub SCData_Module6_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module6.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseIncap
        End Sub

        Protected Sub SCData_Module7_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module7.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseCongress
        End Sub

        Protected Sub SCData_Module8_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module8.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseBMT
        End Sub

        Protected Sub SCData_Module9_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module9.Selecting
            e.InputParameters("moduleId") = ModuleType.SpecCaseWWD
        End Sub

        Private Sub gvResults10_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults10.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults11_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults11.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults12_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults12.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults13_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults13.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults14_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults14.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults17_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults17.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                'args.Url = GetCaseHistoryURL(args.Type, strQuery.ToString())
                'Session("RefId") = args.RefId
                'Response.Redirect(args.Url)

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults18_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults18.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults19_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults19.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        ' gvResults18 for NE goes here.
        Private Sub gvResults20_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults20.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults21_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults21.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults22_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults22.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults23_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults23.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)
            End If

        End Sub

        Private Sub gvResults28_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults28.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)
            End If

        End Sub

        Private Sub gvResults6_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults6.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)
            End If
        End Sub

        Private Sub gvResults7_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults7.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)
            End If
        End Sub

        Private Sub gvResults8_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults8.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

        Private Sub gvResults9_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults9.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Response.Redirect(args.Url)

            End If

        End Sub

    End Class

End Namespace