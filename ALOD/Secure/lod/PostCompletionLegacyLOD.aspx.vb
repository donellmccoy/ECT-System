'Imports ALOD.Core.Domain.Workflow
'Imports ALOD.Core.Utils
'Imports ALOD.Data.Services
'Imports ALODWebUtility.Common
'Imports ALODWebUtility.Permission.Search

'Namespace Web.LOD
'    'Partial Class Secure_lod_PostCompletionLegacyLOD
'    '    Inherits System.Web.UI.Page

'    '    Public ReadOnly Property refId() As Integer
'    '        Get
'    '            Return Integer.Parse(Request.QueryString("refId"))
'    '        End Get
'    '    End Property

'    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
'            If (Not IsPostBack) Then
'                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
'                RegisterAsyncTask(task)
'                gvResults.Sort("DaysCompleted", SortDirection.Descending)

'                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
'                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
'                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
'            End If
'        End Sub

'        Private Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand
'            If (e.CommandName = "view") Then

'                Dim parts() As String = e.CommandArgument.ToString().Split(";")
'                Dim strQuery As New StringBuilder()
'                Dim args As New ItemSelectedEventArgs
'                args.RefId = CInt(parts(0))

'                strQuery.Append("refId=" + CType(args.RefId, String))
'                args.Type = ModuleType.LOD

'                Select Case args.Type
'                    Case ModuleType.LOD
'                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
'                        Response.Redirect(args.Url)

'                End Select

'            End If
'        End Sub

'        Protected Sub LodData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles LodData.Selecting

'            ''''LOD_V3'''
'            'If (SESSION_GROUP_ID = 120) Then 'Unit CC (P)
'            '    e.InputParameters("WsId") = 330
'            'End If

'            ''''Displays Legacy cases''''
'            'If (SESSION_GROUP_ID = 2) Then 'Unit Commander
'            e.InputParameters("WsId") = 220
'            'End If

'            e.InputParameters("moduleId") = CByte(ModuleType.LOD)
'            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)

'            NoFiltersPanel.Visible = False
'            If (CheckBoxSearchAllCases.Checked) Then
'                If (SsnBox.Text.Trim.Length = 0 AndAlso NameBox.Text.Trim.Length = 0 AndAlso CaseIdbox.Text.Trim.Length = 0) Then
'                    e.Cancel = True
'                    NoFiltersPanel.Visible = True

'                End If

'            End If
'        End Sub

'        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
'            HeaderRowBinding(sender, e, "CaseId")
'        End Sub
'    End Class
'End Namespace