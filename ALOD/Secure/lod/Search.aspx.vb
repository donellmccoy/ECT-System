Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.LOD

    Partial Class Secure_lod_Search
        Inherits System.Web.UI.Page

#Region "Properites to set Print button to view Final 348/216 snapshot "

        Private _dao As IDocumentDao

        ' Added by skennedy 6/29/2013
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)

        Private _lod As LineOfDuty = Nothing
        Private instance As LineOfDuty

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

#End Region

        Protected Sub ddlLodStatus_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusSelect.DataBound
            StatusSelect.Items.Insert(0, New ListItem("-- All --", String.Empty))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)
                PreloadSearchFilters()
                ResultGrid.Sort("CaseId", SortDirection.Ascending)

                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtLastName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtFirstName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtMiddleName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
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

                Dim refID As Integer = data("RefId")
                ViewFinal(refID, data, e)
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            CaseIdBox.Text = CaseIdBox.Text.Trim
            SsnBox.Text = SsnBox.Text.Trim
            NameBox.Text = NameBox.Text.Trim

            ResultGrid.DataBind()
        End Sub

        Protected Sub SearchData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SearchData.Selecting
            NoFiltersPanel.Visible = False
            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)
            If (SsnBox.Text.Trim.Length = 0 AndAlso
                NameBox.Text.Trim.Length = 0 AndAlso
                CaseIdBox.Text.Trim.Length = 0 AndAlso
                StatusSelect.SelectedIndex = 0 AndAlso
                FormalSelect.SelectedIndex = 0 AndAlso
                UnitSelect.SelectedIndex < 1) Then
                NoFiltersPanel.Visible = True
                e.Cancel = True
                Exit Sub

            End If

            'txtLastName.Text.Trim.Length = 0 AndAlso _
            'txtFirstName.Text.Trim.Length = 0 AndAlso _
            'txtMiddleName.Text.Trim.Length = 0 AndAlso _
        End Sub

        ' Handles Printing 384/261 forms:
        ' If IsFinal then get pdf from database, else build pdf on the fly
        Protected Sub ViewFinal(ByVal refID As String, ByVal data As DataRowView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)

            Dim form348ID As String = 0
            Dim form261ID As String = 0
            instance = LodService.GetById(CInt(data("RefId")))

            ' ckeck for GroupID; some cases were cancled
            If (instance.DocumentGroupId) Then
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)
            End If

            If (data("IsFinal")) Then

                Dim isDoc As Boolean = False
                If (Not _documents Is Nothing) Then

                    ' fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                    ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                    Dim fileSubString As String = instance.CaseId & "-Generated"

                    For Each docItem In _documents
                        If (docItem.DocType.ToString() = "FinalForm348" AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                            form348ID = docItem.Id.ToString()
                            isDoc = True
                        End If
                    Next
                End If

                If (isDoc) Then
                    Dim url348 As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" & form348ID & "&modId=" & ModuleType.LOD)
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "viewDoc('" & url348 & "'); return false;"
                    CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Final Forms"
                Else
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refID & "', 'lod'); return false;"
                    CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Final Forms"

                End If
            Else
                CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refID & "', 'lod'); return false;"
                CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Draft Forms"

            End If

        End Sub

        Private Sub PreloadSearchFilters()

            If (Request.QueryString("data") Is Nothing) Then
                Exit Sub
            End If

            Dim data As String = Request.QueryString("data")

            If (IsNumeric(data)) Then

                If (data.Length = 4) Then
                    Dim num As Integer = Integer.Parse(data)
                    If (num >= 2005 And num <= 2026) Then
                        'assumes it is the year portion of the caseID
                        CaseIdBox.Text = data
                    Else
                        'assume SSN
                        SsnBox.Text = data
                    End If
                Else
                    'assume caseId
                    CaseIdBox.Text = data
                End If
            Else
                'could be either a name, or caseid

                If (IsValidCaseId(data)) Then
                    CaseIdBox.Text = data
                Else
                    'txtLastName.Text = data
                    NameBox.Text = data
                End If

            End If

        End Sub

        Private Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles ResultGrid.RowCommand
            If (e.CommandName = "view") Then
                ' Dim query As New SecureQueryString()
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                If (UserHasPermission(GetViewPermissionByModuleId(args.Type))) Then
                    Select Case args.Type
                        Case ModuleType.LOD
                            args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
                    End Select
                Else
                    args.Url = "~/Secure/Shared/SecureAccessDenied.aspx?deniedType=1"
                End If

                Response.Redirect(args.Url)
            End If
        End Sub

    End Class

End Namespace