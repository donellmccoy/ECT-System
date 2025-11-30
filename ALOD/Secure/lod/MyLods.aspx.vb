Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.LOD

    Partial Class Secure_lod_MyLods
        Inherits System.Web.UI.Page

        Private _daoFactory As NHibernateDaoFactory
        Private _lodDao As ILineOfDutyDao

        Protected ReadOnly Property DaoFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LODDao As ILineOfDutyDao
            Get
                If (_lodDao Is Nothing) Then
                    _lodDao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _lodDao
            End Get
        End Property

        Protected Sub gvResults_LODV3_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults_LODV3.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

                If (IsSessionUserABoardTechnician()) Then
                    CType(e.Row.FindControl("lblReceivedFrom"), Label).Text = LODDao.GetFromAndDirection(Integer.Parse(data("refId")))
                End If
            End If
        End Sub

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

                If (IsSessionUserABoardTechnician()) Then
                    CType(e.Row.FindControl("lblReceivedFrom"), Label).Text = LODDao.GetFromAndDirection(Integer.Parse(data("refId")))
                End If
            End If
        End Sub

        Protected Function IsSessionUserABoardTechnician() As Boolean
            If (Session("GroupId") = ALOD.Core.Domain.Users.UserGroups.BoardTechnician) Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Sub LOD_V3()
            Select Case SESSION_GROUP_ID
                Case UserGroups.MedicalTechnician, UserGroups.MedicalOfficer, UserGroups.UnitCommander, UserGroups.WingJudgeAdvocate, UserGroups.WingCommander,
                     UserGroups.WingSarc, UserGroups.BoardLegal, UserGroups.BoardMedical, UserGroups.BoardTechnician, UserGroups.BoardAdministrator, UserGroups.BoardApprovalAuthority
                    resultsUpdatePanel.Visible = False
                    resultsUpdatePanel_IO.Visible = False
                Case UserGroups.InvestigatingOfficer
                    resultsUpdatePanel.Visible = False
                    resultsUpdatePanel_LODV3.Visible = False
                Case Else
                    resultsUpdatePanel_LODV3.Visible = False
                    resultsUpdatePanel_IO.Visible = False

            End Select
        End Sub

        Protected Sub LodData_LODV3_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles LodData_LODV3.Selecting
            e.InputParameters("moduleId") = CByte(ModuleType.LOD)
            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)
        End Sub

        Protected Sub LodData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles LodData.Selecting
            e.InputParameters("moduleId") = CByte(ModuleType.LOD)
            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                'clear any pending locks this user might have
                Dim dao As ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)
                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)
                LOD_V3()
                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtLastName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtFirstName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtMiddleName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
            End If
            If (IsSessionUserABoardTechnician()) Then
                gvResults.Columns(5).Visible = True
            Else
                gvResults.Columns(5).Visible = False
            End If
        End Sub

        Private Sub gvResults_IO_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults_IO.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                Select Case args.Type
                    Case ModuleType.LOD
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                End Select
            End If
        End Sub

        Private Sub gvResults_LODV3_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults_LODV3.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                Select Case args.Type
                    Case ModuleType.LOD
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                End Select
            End If
        End Sub

        Private Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                Select Case args.Type
                    Case ModuleType.LOD
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                End Select
            End If
        End Sub

    End Class

End Namespace