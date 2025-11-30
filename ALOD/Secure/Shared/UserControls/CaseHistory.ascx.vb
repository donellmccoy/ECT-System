Imports System.Runtime.Remoting.Messaging
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_CaseHistory
        Inherits System.Web.UI.UserControl
        Protected Const KEY_REFID As String = "refId"

        Protected Const KEY_REFSTATUS As String = "refStatus"

        Private Const ASCENDING As String = "Asc"

        Private Const DESCENDING As String = "Desc"

        Private Const SORT_COLUMN_KEY_LOd As String = "_LODSortExp_"

        Private Const SORT_COLUMN_KEY_SC As String = "_SCSortExp_"

        Private Const SORT_DIR_KEY_LOD As String = "_LODSortDirection_"

        Private Const SORT_DIR_KEY_SC As String = "_SCSortDirection_"

        Private case_id As String = ""

        Private MemberSSN As String = ""

        Public Delegate Function GetPALDataDelegate(ByVal param As StringDictionary) As DataSet

        Public Delegate Function GetSpecialCasesDelegate(ByVal param As StringDictionary) As DataSet

        Public Delegate Function GetUnDeletedLodsDelegate(ByVal param As StringDictionary) As DataSet

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Private Property SortColumnLOD() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_LOd)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_LOd) = value
            End Set
        End Property

        Private Property SortColumnSC() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_SC)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_SC) = value
            End Set
        End Property

        Private Property SortDirectionLOD() As String
            Get
                If (ViewState(SORT_DIR_KEY_LOD) Is Nothing) Then
                    ViewState(SORT_DIR_KEY_LOD) = ASCENDING
                End If
                Return ViewState(SORT_DIR_KEY_LOD)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY_LOD) = value
            End Set
        End Property

        Private Property SortDirectionSC() As String
            Get
                If (ViewState(SORT_DIR_KEY_SC) Is Nothing) Then
                    ViewState(SORT_DIR_KEY_SC) = ASCENDING
                End If
                Return ViewState(SORT_DIR_KEY_SC)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY_SC) = value
            End Set
        End Property

        Private ReadOnly Property SortExpressionLOD() As String
            Get
                Return SortColumnLOD + " " + SortDirectionLOD
            End Get
        End Property

        Private ReadOnly Property SortExpressionSC() As String
            Get
                Return SortColumnSC + " " + SortDirectionSC
            End Get
        End Property

        '''<summary>
        '''<para>The method creates  GetSpecialCasesDelegate for  GetSpecialCases method and invokes it asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncOperation(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult
            Dim asynchDelegate As GetSpecialCasesDelegate = New GetSpecialCasesDelegate(AddressOf GetSpecialCases)
            Dim param As StringDictionary = CType(state, StringDictionary)
            Dim r As IAsyncResult = asynchDelegate.BeginInvoke(param, cb, state)
            Return r
        End Function

        '''<summary>
        '''<para>The method creates  GetUndeletedLODsDelegate for GetUndeletedLODs method and invokes it asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncOperationLOD(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult
            Dim asynchDelegateLOD As GetUnDeletedLodsDelegate = New GetUnDeletedLodsDelegate(AddressOf GetUnDeletedLods)
            Dim param As StringDictionary = CType(state, StringDictionary)
            Dim r As IAsyncResult = asynchDelegateLOD.BeginInvoke(param, cb, state)
            Return r
        End Function

        '''<summary>
        '''<para>The method creates  GetPALDataDelegate for GetPALData method and invokes it asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncOperationPAL(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult
            Dim asynchDelegatePAL As GetPALDataDelegate = New GetPALDataDelegate(AddressOf GetPALData)
            Dim param As StringDictionary = CType(state, StringDictionary)
            Dim r As IAsyncResult = asynchDelegatePAL.BeginInvoke(param, cb, state)
            Return r
        End Function

        '''<summary>
        '''<para>The method is the call back method which is called when the  GetSpecialCases  is complete.
        ''' Upon completion History Grid's inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncOperation(ByVal a As IAsyncResult)
            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim caller As GetSpecialCasesDelegate = CType(result.AsyncDelegate, GetSpecialCasesDelegate)
            Dim history As DataSet = caller.EndInvoke(a)
            If history.Tables.Count = 0 Then
                HistoryGrid.Visible = False
                NoHistoryLbl.Visible = True
            Else
                HistoryGrid.Visible = True
                NoHistoryLbl.Visible = False
                HistoryGrid.DataSource = history
                HistoryGrid.DataBind()
            End If

            Session("HistoryGridSC") = history
        End Sub

        '''<summary>
        '''<para>The method is the call back method which is called when the  GetUnDeletedLods is complete.
        ''' Upon completion History Grid's inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncOperationLOD(ByVal a As IAsyncResult)
            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim caller As GetUnDeletedLodsDelegate = CType(result.AsyncDelegate, GetUnDeletedLodsDelegate)
            Dim history As DataSet = caller.EndInvoke(a)
            If history.Tables.Count = 0 Then
                HistoryGridLOD.Visible = False
                NoHistoryLODLbl.Visible = True
            Else
                HistoryGridLOD.Visible = True
                NoHistoryLODLbl.Visible = False
                HistoryGridLOD.DataSource = history
                HistoryGridLOD.DataBind()
            End If

            Session("HistoryGridLOD") = history
        End Sub

        '''<summary>
        '''<para>The method is the call back method which is called when the  GetPALData is complete.
        ''' Upon completion History Grid's inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncOperationPAL(ByVal a As IAsyncResult)
            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim caller As GetPALDataDelegate = CType(result.AsyncDelegate, GetPALDataDelegate)
            Dim history As DataSet = caller.EndInvoke(a)
            If history.Tables.Count = 0 Then
                HistoryGridPAL.Visible = False
                NoHistoryPALLbl.Visible = True
            Else
                HistoryGridPAL.Visible = True
                NoHistoryPALLbl.Visible = False
                HistoryGridPAL.DataSource = history
                HistoryGridPAL.DataBind()
            End If
        End Sub

        Public Function GetPALData(ByVal param As StringDictionary) As DataSet
            Return LodService.GetPALDataByMemberSSN(MemberSSN, "")
        End Function

        Public Function GetSpecialCases(ByVal param As StringDictionary) As DataSet
            Return LodService.GetMemberSpecialCaseHistory(MemberSSN, Integer.Parse(param(SESSIONKEY_USER_ID)))
        End Function

        Public Function GetUnDeletedLods(ByVal param As StringDictionary) As DataSet
            Return LodService.GetCaseHistory(MemberSSN, Integer.Parse(param(SESSIONKEY_USER_ID)), Byte.Parse(param(SESSIONKEY_REPORT_VIEW)), param(SESSIONKEY_COMPO), Integer.Parse(param(SESSIONKEY_UNIT_ID)), Boolean.Parse(param(SESSIONKEY_SARC_PERMSSION)), True)
        End Function

        Public Sub Initialize(ByVal hostPage As Page, ByVal SSN As String, ByVal caseID As String, ByVal PALdocuments As Boolean)
            MemberSSN = SSN
            If (Not String.IsNullOrEmpty(caseID)) Then
                case_id = caseID
            End If

            Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncOperation), New EndEventHandler(AddressOf EndAsyncOperation), Nothing, GetSessionDictionary)
            hostPage.RegisterAsyncTask(task)

            Dim taskLOD As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncOperationLOD), New EndEventHandler(AddressOf EndAsyncOperationLOD), Nothing, GetSessionDictionary)
            hostPage.RegisterAsyncTask(taskLOD)

            If (PALdocuments) Then
                Dim taskPAL As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncOperationPAL), New EndEventHandler(AddressOf EndAsyncOperationPAL), Nothing, GetSessionDictionary)
                hostPage.RegisterAsyncTask(taskPAL)

                If (Session("groupid") = UserGroups.UnitCommander) Then
                    HistoryPanelPAL.Visible = False
                End If
            Else
                HistoryPanelPAL.Visible = False
            End If

            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)
        End Sub

        Protected Sub HistoryGrid_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles HistoryGrid.PageIndexChanging
            HistoryGrid.PageIndex = e.NewPageIndex
            Dim dsSort As DataSet = Session("HistoryGridSC")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            HistoryGrid.DataSource = dvSort
            HistoryGrid.DataBind()
        End Sub

        Protected Sub HistoryGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles HistoryGrid.RowDataBound
            Dim caseId As String
            Dim data As System.Data.DataRowView
            Dim wsDao As WorkStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                data = e.Row.DataItem
                caseId = data("Case_Id")

                If caseId = case_id Then
                    e.Row.Visible = False
                End If
            End If
        End Sub

        Protected Sub HistoryGrid_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles HistoryGrid.Sorting
            HistoryGrid.PageIndex = 0
            If (SortColumnSC <> "") Then
                If (SortColumnSC = e.SortExpression) Then
                    If SortDirectionSC = ASCENDING Then
                        SortDirectionSC = DESCENDING
                    Else
                        SortDirectionSC = ASCENDING
                    End If
                Else
                    SortDirectionSC = ASCENDING
                End If
            End If

            SortColumnSC = e.SortExpression
            Dim dsSort As DataSet = Session("HistoryGridSC")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            dvSort.Sort = SortExpressionSC
            HistoryGrid.DataSource = dvSort
            HistoryGrid.DataBind()
        End Sub

        Protected Sub HistoryGridLOD_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles HistoryGridLOD.PageIndexChanging
            HistoryGridLOD.PageIndex = e.NewPageIndex
            Dim dsSort As DataSet = Session("HistoryGridLOD")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            HistoryGridLOD.DataSource = dvSort
            HistoryGridLOD.DataBind()
        End Sub

        Protected Sub HistoryGridLOD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles HistoryGridLOD.RowDataBound
            Dim caseId As String
            Dim data As System.Data.DataRowView
            Dim status As Integer
            Dim wsStatus As ALOD.Core.Domain.Workflow.WorkStatus
            Dim completeDateLbl As Label
            Dim wsDao As WorkStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                data = e.Row.DataItem
                status = data("WorkStatusId")
                wsStatus = wsDao.GetById(CInt(status))
                caseId = data("CaseId")
                If wsStatus.StatusCodeType.IsFinal Then
                    completeDateLbl = CType(e.Row.FindControl("completeDateLbl"), Label)
                    completeDateLbl.Text = data("ReceiveDate")
                End If

                If caseId = case_id Then
                    e.Row.Visible = False
                End If
            End If
        End Sub

        Protected Sub HistoryGridLOD_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles HistoryGridLOD.Sorting
            HistoryGridLOD.PageIndex = 0
            If (SortColumnLOD <> "") Then
                If (SortColumnLOD = e.SortExpression) Then
                    If SortDirectionLOD = ASCENDING Then
                        SortDirectionLOD = DESCENDING
                    Else
                        SortDirectionLOD = ASCENDING
                    End If
                Else
                    SortDirectionLOD = ASCENDING
                End If
            End If

            SortColumnLOD = e.SortExpression
            Dim dsSort As DataSet = Session("HistoryGridLOD")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            dvSort.Sort = SortExpressionLOD
            HistoryGridLOD.DataSource = dvSort
            HistoryGridLOD.DataBind()
        End Sub

        Private Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles HistoryGrid.RowCommand
            If (e.CommandName = "view") Then
                ' Dim query As New SecureQueryString()
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Session("RefId") = parts(0)
                Session("RequestId") = -1
                Response.Redirect(GetWorkflowInitPageURL(parts(1), parts(0)))
            End If
        End Sub

        Private Sub ResultGridLOD_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles HistoryGridLOD.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = parts(0)

                args.Type = parts(1)

                Select Case args.Type
                    Case ModuleType.LOD
                        strQuery.Append("refId=" + CType(args.RefId, String))
                        args.Url = "~/Secure/lod/init.aspx?" + strQuery.ToString()
                        Session("RefId") = args.RefId
                        Session("RequestId") = -1
                        Response.Redirect(args.Url)
                    Case ModuleType.AppealRequest
                        strQuery.Append("requestId=" + CType(args.RefId, String))
                        args.Url = "~/Secure/AppealRequests/init.aspx?" + strQuery.ToString()
                        Session("requestId") = args.RefId
                        Response.Redirect(args.Url)
                End Select
            End If
        End Sub

    End Class

End Namespace