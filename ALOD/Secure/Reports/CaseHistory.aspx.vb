Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search
Imports ALODWebUtility.Printing

Namespace Web.Reports

    Partial Class Secure_Reports_CaseHistory
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY_LOd As String = "_LODSortExp_"
        Private Const SORT_COLUMN_KEY_SC As String = "_SCSortExp_"
        Private Const SORT_DIR_KEY_LOD As String = "_LODSortDirection_"
        Private Const SORT_DIR_KEY_SC As String = "_SCSortDirection_"
        Private _dao As IDocumentDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _workflowDao As IWorkflowDao
        Private instance As LineOfDuty

#End Region

#Region "Properites..."

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

        Protected Property SortColumnLOD() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_LOd)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_LOd) = value
            End Set
        End Property

        Protected Property SortColumnSC() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_SC)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_SC) = value
            End Set
        End Property

        Protected Property SortDirectionLOD() As String
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

        Protected Property SortDirectionSC() As String
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

        Protected ReadOnly Property SortExpressionLOD() As String
            Get
                Return SortColumnLOD + " " + SortDirectionLOD
            End Get
        End Property

        Protected ReadOnly Property SortExpressionSC() As String
            Get
                Return SortColumnSC + " " + SortDirectionSC
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected Sub GetData(ByVal ssn As String)
            GetPALHistoryData(ssn)
            GetLODHistoryData(ssn)
            GetSpecialCaseHistoryData(ssn)
        End Sub

        Protected Sub GetLODHistoryData(ByVal ssn As String)
            Dim history As DataSet = LodService.GetLodsBySM(Server.HtmlEncode(ssn), UserHasPermission(PERMISSION_VIEW_SARC_CASES))

            If (history.Tables(0).Rows.Count < 1) Then
                HistoryGrid.Visible = False
                NoHistoryLbl.Visible = True
            Else
                HistoryGrid.Visible = True
                NoHistoryLbl.Visible = False
            End If

            HistoryGrid.DataSource = history
            HistoryGrid.DataBind()
        End Sub

        Protected Sub GetPALHistoryData(ByVal ssn As String)
            Dim palHistory As DataSet = LodService.GetPALDataByMemberSSN(Server.HtmlEncode(ssn), "")

            If (palHistory.Tables.Count = 0) Then
                HistoryGridPAL.Visible = False
                NoHistoryPALLbl.Visible = True
            Else
                HistoryGridPAL.Visible = True
                NoHistoryPALLbl.Visible = False

                HistoryGridPAL.DataSource = palHistory
                HistoryGridPAL.DataBind()
            End If
        End Sub

        Protected Function GetSelectedMemberSSN() As String
            If (rdbSSN.Checked) Then
                Return SSNText.Text
            ElseIf (rdbName.Checked) Then
                If (String.IsNullOrEmpty(txtMemberLastName.Text) = True And String.IsNullOrEmpty(txtMemberFirstName.Text) = True And String.IsNullOrEmpty(txtMemberMiddleName.Text) = True) Then
                    Return String.Empty
                End If

                If (Session("rowIndex") Is Nothing) Then
                    Return String.Empty
                End If

                Dim resultsTable As DataTable = LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)

                If (resultsTable.Rows.Count > 0) Then
                    Return resultsTable.Rows(Session("rowIndex")).Field(Of String)("SSN")
                Else
                    Return String.Empty
                End If
            End If

            Return String.Empty
        End Function

        Protected Sub GetSpecialCaseHistoryData(ByVal ssn As String)
            Dim scHistory As DataSet = LodService.GetSpecialCasesByMemberSSN(Server.HtmlEncode(ssn), SESSION_USER_ID)

            If (scHistory.Tables(0).Rows.Count < 1) Then
                SCHistoryGrid.Visible = False
                NoSCHistoryLbl.Visible = True
            Else
                SCHistoryGrid.Visible = True
                NoSCHistoryLbl.Visible = False
            End If

            SCHistoryGrid.DataSource = scHistory
            SCHistoryGrid.DataBind()
        End Sub

        Protected Sub HistoryGrid_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles HistoryGrid.PageIndexChanging
            Dim memberSSN As String = GetSelectedMemberSSN()

            If (Not String.IsNullOrEmpty(memberSSN)) Then
                HistoryGrid.PageIndex = e.NewPageIndex
                Dim dsSort As DataSet = LodService.GetLodsBySM(Server.HtmlEncode(memberSSN), UserHasPermission(PERMISSION_VIEW_SARC_CASES))
                Dim dvSort As DataView = dsSort.Tables(0).DefaultView
                HistoryGrid.DataSource = dvSort
                HistoryGrid.DataBind()
            End If
        End Sub

        Protected Sub HistoryGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles HistoryGrid.RowDataBound
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

                If (wsStatus.StatusCodeType.IsFinal) Then
                    completeDateLbl = CType(e.Row.FindControl("completeDateLbl"), Label)
                    completeDateLbl.Text = data("ReceiveDate")
                End If

                ViewFinal(data("RefId"), data, e)
            End If
        End Sub

        Protected Sub HistoryGrid_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles HistoryGrid.Sorting
            Dim memberSSN As String = GetSelectedMemberSSN()

            If (Not String.IsNullOrEmpty(memberSSN)) Then
                HistoryGrid.PageIndex = 0
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
                Dim dsSort As DataSet = LodService.GetLodsBySM(Server.HtmlEncode(memberSSN), UserHasPermission(PERMISSION_VIEW_SARC_CASES))
                Dim dvSort As DataView = dsSort.Tables(0).DefaultView
                dvSort.Sort = SortExpressionLOD
                HistoryGrid.DataSource = dvSort
                HistoryGrid.DataBind()
            End If
        End Sub

        Protected Sub HistoryGridSC_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles SCHistoryGrid.PageIndexChanging
            Dim memberSSN As String = GetSelectedMemberSSN()

            If (Not String.IsNullOrEmpty(memberSSN)) Then
                SCHistoryGrid.PageIndex = e.NewPageIndex
                Dim dsSort As DataSet = LodService.GetSpecialCasesByMemberSSN(Server.HtmlEncode(memberSSN), SESSION_USER_ID)
                Dim dvSort As DataView = dsSort.Tables(0).DefaultView
                SCHistoryGrid.DataSource = dvSort
                SCHistoryGrid.DataBind()
            End If
        End Sub

        Protected Sub HistoryGridSC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles SCHistoryGrid.RowDataBound
            Dim caseId As String
            Dim data As System.Data.DataRowView
            Dim wsDao As WorkStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                data = e.Row.DataItem
                caseId = data("Case_Id")
            End If
        End Sub

        Protected Sub HistoryGridSC_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles SCHistoryGrid.Sorting
            Dim memberSSN As String = GetSelectedMemberSSN()

            If (Not String.IsNullOrEmpty(memberSSN)) Then
                SCHistoryGrid.PageIndex = 0
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
                Dim dsSort As DataSet = LodService.GetSpecialCasesByMemberSSN(Server.HtmlEncode(SSNText.Text), SESSION_USER_ID)
                Dim dvSort As DataView = dsSort.Tables(0).DefaultView
                dvSort.Sort = SortExpressionSC
                SCHistoryGrid.DataSource = dvSort
                SCHistoryGrid.DataBind()
            End If
        End Sub

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)
            Dim resultsTable As DataTable = LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)
            Dim member As ServiceMember = Nothing
            Dim ssn As String = String.Empty

            Session("rowIndex") = e.SelectedRowIndex
            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")
            member = LookupService.GetServiceMemberBySSN(ssn)

            ProcessSelectedMember(member, ssn)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            SetInputFormatRestriction(Page, SSNText, FormatRestriction.Numeric, "-")

            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected

            If (Not IsPostBack) Then
                trSSN.Visible = True
                trName.Visible = False
            End If
        End Sub

        Protected Sub ProcessSelectedMember(ByRef member As ServiceMember, ByVal ssn As String)
            If (member IsNot Nothing) Then
                MemberSelectionPanel.Visible = False
                lblInvalidMemberName.Visible = False
                lblMemberNotFound.Visible = False

                lblName.Text = member.FullName
                lblRank.Text = member.Rank.Rank
                lbldob.Text = Format(member.DateOfBirth, "yyyyMMdd")
                UnitTextBox.Text = member.Unit.Name
                ResultsPanel.Visible = True
                GetData(ssn)
            Else
                ErrorLabel.Text = "No service member found."
                MemberSelectionPanel.Visible = False
                lblInvalidMemberName.Visible = False
                lblMemberNotFound.Visible = False
            End If
        End Sub

        Protected Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles HistoryGrid.RowCommand
            If (e.CommandName = "view") Then
                ' Dim query As New SecureQueryString()
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

        Protected Sub ResultGridSC_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles SCHistoryGrid.RowCommand
            If (e.CommandName = "view") Then
                ' Dim query As New SecureQueryString()
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Session("RefId") = parts(0)
                Response.Redirect(GetWorkflowInitPageURL(parts(1), parts(0)))
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            If Page.IsValid Then
                Dim SSN As String = String.Empty
                Dim member As ServiceMember = Nothing

                If (rdbSSN.Checked) Then
                    SSN = Server.HtmlEncode(SSNText.Text.Trim())
                    member = LookupService.GetServiceMemberBySSN(SSN)
                ElseIf (rdbName.Checked) Then
                    lblInvalidMemberName.Visible = False
                    lblMemberNotFound.Visible = False

                    If (String.IsNullOrEmpty(txtMemberLastName.Text) = True And String.IsNullOrEmpty(txtMemberFirstName.Text) = True And String.IsNullOrEmpty(txtMemberMiddleName.Text) = True) Then
                        lblInvalidMemberName.Visible = True
                        Exit Sub
                    End If

                    Dim resultsTable As DataTable = LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)

                    If (resultsTable.Rows.Count > 1) Then
                        ucMemberSelectionGrid.Initialize(resultsTable)
                        MemberSelectionPanel.Visible = True
                        ResultsPanel.Visible = False
                        Exit Sub
                    ElseIf (resultsTable.Rows.Count = 1) Then
                        Session("rowIndex") = 0
                        SSN = resultsTable.Rows(0).Field(Of String)("SSN")
                        member = LookupService.GetServiceMemberBySSN(SSN)
                    Else
                        lblMemberNotFound.Visible = True
                        Exit Sub
                    End If
                End If

                ProcessSelectedMember(member, SSN)
            End If
        End Sub

        Protected Sub SearchSelectionRadioButton_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbSSN.CheckedChanged, rdbName.CheckedChanged
            If (rdbSSN.Checked) Then
                trSSN.Visible = True
                trName.Visible = False
            Else
                trSSN.Visible = False
                trName.Visible = True
            End If
        End Sub

        Protected Sub ViewFinal(ByVal refID As String, ByVal data As DataRowView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim form348ID As String = 0
            Dim form261ID As String = 0
            Dim printImageBtn As ImageButton = CType(e.Row.FindControl("PrintImage"), ImageButton)

            If (printImageBtn Is Nothing) Then
                Exit Sub
            End If

            instance = LodService.GetById(CInt(data("RefId")))

            ' ckeck for GroupID; some cases were cancled
            If (instance.DocumentGroupId) Then
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)
            End If

            printImageBtn.OnClientClick = ViewForms.LinkAttribute348(refID, _documents, "lod")

            If (data("IsFinal").ToString() = "True") Then
                printImageBtn.AlternateText = "Print Final Forms"
            Else
                printImageBtn.AlternateText = "Print Draft Forms"
            End If
        End Sub

#End Region

    End Class

End Namespace