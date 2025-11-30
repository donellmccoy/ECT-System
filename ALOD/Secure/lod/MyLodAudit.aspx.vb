Imports System.Data.Common
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.LOD

    Partial Class Secure_lod_MyLodAudit
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

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            Dim dataStore As New SqlDataStore
            Dim dbCommand As DbCommand
            Dim ds As New DataSet
            Dim wsId As Int32

            If (SESSION_GROUP_ID = 9) Then 'Board Medical
                wsId = 333
            End If
            If (SESSION_GROUP_ID = 8) Then 'Board Legal
                wsId = 334
            End If
            If (SESSION_GROUP_ID = 11) Then 'Approving Authority
                wsId = 335
            End If
            If (SESSION_GROUP_ID = 97) Then 'Board Administrator
                wsId = 330
            End If
            dbCommand = dataStore.GetStoredProcCommand("form348_sp_PilotCaseSearch", 330, SESSION_COMPO)
            ds = dataStore.ExecuteDataSet(dbCommand)
            Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)

        End Sub

        Protected Sub LodData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles LodData.Selecting

            If (SESSION_GROUP_ID = 9) Then 'Board Medical
                e.InputParameters("wsId") = 333
            End If
            If (SESSION_GROUP_ID = 8) Then 'Board Legal
                e.InputParameters("wsId") = 334
            End If
            If (SESSION_GROUP_ID = 11) Then 'Approving Authority
                e.InputParameters("wsId") = 335
            End If
            If (SESSION_GROUP_ID = 97) Then 'Board Administrator
                e.InputParameters("wsId") = 330
            End If
            e.InputParameters("compo") = SESSION_COMPO
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                'clear any pending locks this user might have
                Dim dao As ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)
                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)

                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
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