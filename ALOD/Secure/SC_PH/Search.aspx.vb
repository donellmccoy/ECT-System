Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search
Imports ALODWebUtility.Printing

Namespace Web.Special_Case.PH

    Partial Class Search
        Inherits System.Web.UI.Page

        Private _documentDao As IDocumentDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)

        Private _specCaseDao As ISpecialCaseDAO
        Private _workflowDao As IWorkflowDao

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _specCaseDao
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

        Protected Function CanShowPrintImage(ByVal specCase As SpecialCase) As Boolean
            If (specCase Is Nothing OrElse Not specCase.DocumentGroupId.HasValue OrElse specCase.DocumentGroupId.Value < 1) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub ddlPHStatus_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusSelect.DataBound
            StatusSelect.Items.Insert(0, New ListItem("-- All --", String.Empty))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, txtYear, FormatRestriction.Numeric)

                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)

                PreloadSearchFilters()

                PopulateReportingPeriodDDLs()

                ResultGrid.Sort("Reporting_Period", SortDirection.Ascending)

                SetInputFormatRestriction(Page, CaseIdBox, FormatRestriction.AlphaNumeric, "-")
            End If
        End Sub

        Protected Sub ResultGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ResultGrid.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

                SetPrintImageEventHandler(data, e)
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            ResultGrid.DataBind()
        End Sub

        Protected Sub SearchData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SearchData.Selecting
            NoFiltersPanel.Visible = False

            'If (CaseIdBox.Text.Trim.Length = 0 AndAlso _
            '    UnitSelect.SelectedIndex < 1) Then
            '    NoFiltersPanel.Visible = True
            '    e.Cancel = True
            '    Exit Sub
            'End If
        End Sub

        Protected Sub SetPrintImageEventHandler(ByVal data As DataRowView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim specCase As SpecialCase = SpecCaseDao.GetById(CInt(data("RefId")))

            If (Not CanShowPrintImage(specCase)) Then
                CType(e.Row.FindControl("PrintImage"), ImageButton).Visible = False
                CType(e.Row.FindControl("lblDocumentNotFound"), Label).Visible = True
                Exit Sub
            End If

            _documents = DocumentDao.GetDocumentsByGroupId(specCase.DocumentGroupId)

            CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = ViewForms.PHFormPDFLinkAttribute(specCase, _documents)
            CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print PH Form"
        End Sub

        Private Sub PopulateReportingPeriodDDLs()
            ddlMonth.Items.Add(New ListItem("-- Select Month --", 0))

            For i As Integer = 1 To 12
                ddlMonth.Items.Add(New ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i))
            Next
        End Sub

        Private Sub PreloadSearchFilters()
            If (Request.QueryString("data") Is Nothing) Then
                Exit Sub
            End If

            Dim data As String = Request.QueryString("data")

            If (IsNumeric(data)) Then
                CaseIdBox.Text = data
            Else
                'could be either a name, or caseid
                If (IsValidCaseId(data)) Then
                    CaseIdBox.Text = data
                End If
            End If
        End Sub

        Private Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles ResultGrid.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                If (UserHasPermission(GetViewPermissionByModuleId(args.Type))) Then
                    args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                    Response.Redirect(args.Url)
                Else
                    args.Url = "~/Secure/Shared/SecureAccessDenied.aspx?deniedType=1"
                End If

                Response.Redirect(args.Url)
            End If
        End Sub

    End Class

End Namespace