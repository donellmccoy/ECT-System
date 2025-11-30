Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PSCD

    Partial Class Secure_PSCD_Documents
        Inherits System.Web.UI.Page

        Private _caseTypeDao As ICaseTypeDao
        Private _context As HttpContext
        Private _dao As IDocumentDao
        Private _factory As IDaoFactory
        Private _indexIncrement As Integer = 3
        Private _memoSource As MemoDao2
        Private _sc As SC_PSCD
        Private _special As SpecialCase
        Private _Specialdao As ISpecialCaseDAO

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("Username")))
                End If

                Return _dao
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_Specialdao Is Nothing) Then
                    _Specialdao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _Specialdao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property memoId() As Integer
            Get
                Return MemoStore.GetMemoTemplateId("PSC Determination")
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCasePSCD
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SC() As SC_PSCD
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(refId)
                End If

                Return _sc
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(refId, False)
                End If

                Return _special
            End Get
        End Property

        Protected ReadOnly Property worksheetId() As Integer
            Get
                Return MemoStore.GetMemoTemplateId("PSC Commentary Worksheet")
            End Get
        End Property

        Private ReadOnly Property CaseTypeDao() As ICaseTypeDao
            Get
                If (_caseTypeDao Is Nothing) Then
                    _caseTypeDao = DaoFactory.GetCaseTypeDao()
                End If

                Return _caseTypeDao
            End Get
        End Property

        Private ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao2()
                End If

                Return _memoSource
            End Get
        End Property

        Public Sub GetCommentary()
            'to display Commentary Worksheet

        End Sub

        Public Function GetURL(ByVal context As HttpContext, Optional ByVal relativePath As String = "", Optional ByVal query As String = "") As String
            Dim currentUri As Uri = context.Request.Url
            Dim uriBuild As New UriBuilder
            With uriBuild
                .Scheme = currentUri.Scheme
                .Host = currentUri.Host
                .Port = currentUri.Port
                .Path = context.Request.ApplicationPath & relativePath
                .Query = query
            End With

            Return uriBuild.Uri.AbsoluteUri
        End Function

        Protected Sub CommentaryRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles CommentaryRepeater.ItemCommand
            If (e.CommandName = "DeleteWorksheet") Then
                Dim worksheetId As Integer = CInt(e.CommandArgument)
                Dim worksheet As Memorandum2 = MemoStore.GetById(worksheetId)

                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, refId, "Memo: " + worksheet.Template.Title)

                worksheet.Deleted = True
                MemoStore.SaveOrUpdate(worksheet)
                MemoStore.CommitChanges()
                MemoStore.Evict(worksheet)
                UpdateWorksheetList()

                'If (IsDeterminationMemo(memo.Template.Id)) Then
                ' LOD.UpdateIsPostProcessingComplete(DaoFactory)
                'LodService.SaveUpdate(LOD)
                'End If
            End If
            If (e.CommandName = "CreateMemo") Then
                Dim x As Int16 = 1
            End If
        End Sub

        Protected Sub CommentaryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CommentaryRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim workSheet As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

            Dim link As HyperLink = CType(e.Item.FindControl("CommentaryLink"), HyperLink)
            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(workSheet.Id) + "'); return false;")

            Dim perm = (From g In workSheet.Template.GroupPermissions
                        Where g.Group.Id = CInt(Session("groupId"))
                        Select g).FirstOrDefault()

            If (perm Is Nothing OrElse Not perm.CanView) Then
                'if they can't view it, they can't do anything else with it so just exit
                e.Item.Visible = False

                Exit Sub

            End If

            If (Documents.UserCanEdit OrElse perm IsNot Nothing) Then

                Dim edit As Image = CType(e.Item.FindControl("EditWorksheet"), Image)
                Dim delete As Image = CType(e.Item.FindControl("DeleteWorksheet"), Image)

                edit.Visible = perm.CanEdit
                delete.Visible = perm.CanDelete

                If (perm.CanEdit) Then
                    edit.Attributes.Add("onClick", "showEditor(" + workSheet.Id.ToString() + "," + workSheet.Template.Id.ToString() + ");")
                End If
            Else
                'this is read only
                CType(e.Item.FindControl("EditWorksheet"), Image).Visible = False
                CType(e.Item.FindControl("DeleteWorksheet"), Image).Visible = False
            End If

        End Sub

        Protected Sub MemoRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MemoRepeater.ItemCommand
            If (e.CommandName = "DeleteMemo") Then
                Dim memoId As Integer = CInt(e.CommandArgument)
                Dim memo As Memorandum2 = MemoStore.GetById(memoId)

                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, refId, "Memo: " + memo.Template.Title)

                memo.Deleted = True
                MemoStore.SaveOrUpdate(memo)
                MemoStore.CommitChanges()
                MemoStore.Evict(memo)
                UpdateMemoList()

                'If (IsDeterminationMemo(memo.Template.Id)) Then
                ' LOD.UpdateIsPostProcessingComplete(DaoFactory)
                'LodService.SaveUpdate(LOD)
                'End If
            End If
            If (e.CommandName = "CreateMemo") Then
                Dim x As Int16 = 1
            End If
        End Sub

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim memo As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

            Dim perm = (From g In memo.Template.GroupPermissions
                        Where g.Group.Id = CInt(Session("groupId"))
                        Select g).FirstOrDefault()

            If (perm Is Nothing OrElse Not perm.CanView) Then
                'if they can't view it, they can't do anything else with it so just exit
                e.Item.Visible = False

                Exit Sub

            End If

            If (Documents.UserCanEdit OrElse perm IsNot Nothing) Then

                Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
                Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)

                edit.Visible = perm.CanEdit
                delete.Visible = perm.CanDelete

                If (perm.CanEdit) Then
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + ");")
                End If
            Else
                'this is read only
                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Document.css")
            Dim memoTemplateId As Int16
            Dim worksheetTemplateId As Int16

            'just in case memo and worksheet templates are not created yet
            Try
                memoTemplateId = MemoStore.GetMemoTemplateId("PSC Determination")
            Catch

            End Try

            Try
                worksheetTemplateId = MemoStore.GetMemoTemplateId("PSC Commentary Worksheet")
            Catch

            End Try
            If (Not IsPostBack) Then
                GetDocuments()
            Else
                ReloadMemoList()
                ReloadWorksheetList()
            End If

        End Sub

        Protected Sub ReloadMemoList()

            If (Not Documents.UserCanEdit) Then
                ViewPSCDMemo.Visible = False
            End If
            If (Documents.UserCanEdit AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDMemo.Visible = True
            ElseIf (SESSION_GROUP_ID = 7 AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDMemo.Visible = True
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            MemoRepeater.DataSource = From m In MemoStore.GetByRefId(refId)
                                      Where m.Deleted = False AndAlso m.Template.Id = memoId
                                      Select m
                                      Order By m.CreatedDate

            If (MemoRepeater.Items.Count >= 1) Then
                ViewPSCDMemo.Visible = False
            Else
                MemoRepeater.DataBind()
                If (MemoRepeater.Items.Count >= 1) Then
                    ViewPSCDMemo.Visible = False
                End If
            End If
        End Sub

        Protected Sub ReloadWorksheetList()

            If (Not Documents.UserCanEdit) Then
                ViewPSCDWorksheet.Visible = False
            End If
            If (Documents.UserCanEdit AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDWorksheet.Visible = True
            ElseIf (SESSION_GROUP_ID = 7 AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDWorksheet.Visible = True
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            CommentaryRepeater.DataSource = From m In MemoStore.GetByRefId(refId)
                                            Where m.Deleted = False AndAlso m.Template.Id = worksheetId
                                            Select m
                                            Order By m.CreatedDate

            If (CommentaryRepeater.Items.Count >= 1) Then
                ViewPSCDWorksheet.Visible = False
            Else
                CommentaryRepeater.DataBind()
                If (CommentaryRepeater.Items.Count >= 1) Then
                    ViewPSCDWorksheet.Visible = False
                End If
            End If
        End Sub

        Protected Sub UpdateMemoList()

            If (Not Documents.UserCanEdit) Then
                ViewPSCDMemo.Visible = False
            End If
            If (Documents.UserCanEdit AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDMemo.Visible = True
            ElseIf (SESSION_GROUP_ID = 7 AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDMemo.Visible = True
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefId(refId)
                                      Where m.Deleted = False AndAlso m.Template.Id = memoId
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()

            If (MemoRepeater.Items.Count >= 1) Then
                ViewPSCDMemo.Visible = False
            End If
        End Sub

        Protected Sub UpdateWorksheetList()

            If (Not Documents.UserCanEdit) Then
                ViewPSCDWorksheet.Visible = False
            End If
            If (Documents.UserCanEdit AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDWorksheet.Visible = True
            ElseIf (SESSION_GROUP_ID = 7 AndAlso (SC.CurrentStatusCode = SpecCasePSCDStatusCode.BoardTechComplete OrElse SC.CurrentStatusCode = 305)) Then
                ViewPSCDWorksheet.Visible = True
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)

            CommentaryRepeater.DataSource = From m In MemoStore.GetByRefId(refId)
                                            Where m.Deleted = False AndAlso m.Template.Id = worksheetId
                                            Select m
                                            Order By m.CreatedDate

            CommentaryRepeater.DataBind()

            If (CommentaryRepeater.Items.Count >= 1) Then
                ViewPSCDWorksheet.Visible = False
            End If
        End Sub

        Private Sub GetDocuments()
            Dim isDeterminationTemplate As Boolean = True

            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SCDao.SaveOrUpdate(SpecCase)
                SCDao.CommitChanges()
            End If
            ViewPSCDMemo.ToolTip = "Create PSC-D Memo"
            ViewPSCDWorksheet.ToolTip = "Create Commentary Worksheet"
            ViewPSCDMemo.Attributes.Add("onclick", "showEditor(0," + memoId.ToString() + "," + isDeterminationTemplate.ToString().ToLower() + ");")
            ViewPSCDWorksheet.Attributes.Add("onclick", "showEditor(0," + worksheetId.ToString() + "," + isDeterminationTemplate.ToString().ToLower() + ");")
            UpdateMemoList()
            UpdateWorksheetList()
            SpecCase.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SpecCase, _indexIncrement, DaoFactory))
        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + refId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

        End Function

    End Class

End Namespace