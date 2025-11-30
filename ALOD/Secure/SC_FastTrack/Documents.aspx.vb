Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IRILO

    Partial Class Secure_sc_FastTrack_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _dao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _specialCase As SC_FastTrack
        Private _Specialdao As ISpecialCaseDAO

#End Region

#Region "Properties"

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
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
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

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseFT
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_FastTrack
            Get
                If (_specialCase Is Nothing) Then
                    _specialCase = SCDao.GetById(RequestId, False)

                End If

                Return _specialCase
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

#End Region

#Region "Page Methods"

        Protected Sub MemoRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MemoRepeater.ItemCommand
            If (e.CommandName = "DeleteMemo") Then
                Dim memoId As Integer = CInt(e.CommandArgument)
                Dim memo As Memorandum2 = MemoStore.GetById(memoId)

                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, RequestId, "Memo: " + memo.Template.Title)

                memo.Deleted = True
                MemoStore.SaveOrUpdate(memo)
                MemoStore.CommitChanges()
                MemoStore.Evict(memo)
                UpdateMemoList()
            End If
        End Sub

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            Dim memo As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

            Dim perm = (From g In memo.Template.GroupPermissions
                        Where g.Group.Id = CInt(Session("groupId"))
                        Select g).FirstOrDefault()

            If (perm Is Nothing OrElse Not perm.CanView) Then
                e.Item.Visible = False
                Exit Sub
            End If

            Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
            Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)

            If (perm IsNot Nothing) Then
                edit.Visible = perm.CanEdit
                delete.Visible = perm.CanDelete

                If (perm.CanEdit) Then
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + ");")
                End If
            Else
                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then
                GetDocuments()
                InitMemoCreation()
                UpdateMemoList()
                SetFastTrackType()
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateMemoList()
        End Sub

        Protected Sub SetFastTrackType()
            If (SpecCase.FastTrackType.HasValue) Then
                If Not String.IsNullOrEmpty(SpecCase.ICD9Description) Then
                    FastTrackTypeLabel.Text = SpecCase.ICD9Description
                Else
                    FastTrackTypeLabel.Text = "IRILO Not Eligible"
                End If
            End If
        End Sub

        Protected Sub UpdateMemoList()
            Dim modType As Integer = ModuleType
            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, modType)
                                      Where (m.Deleted = False)
                                      Select m
                                      Order By m.CreatedDate
            MemoRepeater.DataBind()
            CheckMemoList()
        End Sub

        Private Function CanCreateMemos() As Boolean
            If Not (SpecCase.Status = SpecCaseFTWorkStatus.FinalReview Or SpecCase.Status = SpecCaseFTWorkStatus.AdminLOD) AndAlso SpecCase.GetActiveMemoTemplateId().HasValue Then
                Return False
            End If

            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            Dim template = (From t In templates Select t Where t.Id = SpecCase.GetActiveMemoTemplateId()).FirstOrDefault()

            Return HasPermission(template)
        End Function

        Private Sub CheckMemoList()
            If (Not CanCreateMemos()) Then
                CreateMemo.Visible = False
                CreateMemo2.Visible = False
                Exit Sub
            End If

            Dim memoList As IList(Of Memorandum2)
            memoList = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                        Where l.Deleted = False And l.Template.Id = SpecCase.GetActiveMemoTemplateId()
                        Select l).ToList

            Dim memoDisqual As IList(Of Memorandum2)
            memoDisqual = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                           Where l.Deleted = False And l.Template.Id = MemoType.IRILO_MEB_Disqualification_Letter
                           Select l).ToList

            If (Not Documents.UserCanEdit Or memoList.Count > 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Visible = True
            End If
            If (Not Documents.UserCanEdit Or memoDisqual.Count > 0) Then
                CreateMemo2.Visible = False
            ElseIf (SpecCase.GetActiveBoardMedicalFinding() = 2) Or (SpecCase.GetActiveMemoTemplateId() = MemoType.IRILO_Admin_LOD_AFPC) Then
                CreateMemo2.Visible = True
            End If
        End Sub

        Private Sub GetDocuments()
            'make sure we have a groupId
            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                SCDao.CommitChanges()
            End If

            SpecCase.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SpecCase, 2, DaoFactory))
        End Sub

        Private Function GetMemoURL(memoId As Integer) As String
            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()
        End Function

        Private Function HasPermission(template As MemoTemplate) As Boolean
            If (template IsNot Nothing) Then
                Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()
                If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                    Return True
                End If
            End If

            Return False
        End Function

        Private Sub InitMemoCreation()
            If (Not CanCreateMemos()) Then
                CreateMemo.Visible = False
                CreateMemo2.Visible = False
                Exit Sub
            End If

            'grab the proper memo
            Dim template2 As MemoType = 0
            Dim template As MemoTemplate = (From m In MemoStore.GetAllTemplates()
                                            Where m.Component = Session("Compo") _
                                            And (m.ModuleType = ModuleType) _
                                            And m.Id = SpecCase.GetActiveMemoTemplateId()
                                            Select m).FirstOrDefault

            If (SpecCase.GetActiveBoardMedicalFinding() = 1) Then
                CreateMemo.ToolTip = "Create IRILO Return to Duty Memo"
            ElseIf (SpecCase.GetActiveBoardMedicalFinding() = 2) Or (SpecCase.GetActiveMemoTemplateId() = MemoType.IRILO_Admin_LOD_AFPC) Then
                CreateMemo.ToolTip = "Create IRILO Admin LOD Letter"

                template2 = MemoType.IRILO_MEB_Disqualification_Letter
                CreateMemo2.ToolTip = "Create MEB Disqualificatoin Letter"
            ElseIf (SpecCase.GetActiveMemoTemplateId() = MemoType.IRILO_DQ_WWD) Then
                CreateMemo.ToolTip = "Create IRILO DQ WWD"
            ElseIf (SpecCase.GetActiveMemoTemplateId() = MemoType.IRILO_DQ_MEB) Then
                CreateMemo.ToolTip = "Create IRILO DQ MEB"
            ElseIf (SpecCase.GetActiveMemoTemplateId() = MemoType.IRILO_DQ_PENDING) Then
                CreateMemo.ToolTip = "Create IRILO DQ LOD Pending"
            Else
                CreateMemo.ToolTip = "Create MEB Disqualification Letter"
            End If

            If (Not HasPermission(template)) Then
                Exit Sub
            End If

            'if the user doesn't have access, hide the create button
            If (template.Id = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + template.Id.ToString() + ", " + CInt(ModuleType).ToString() + ");")
            End If

            If (template.Id = 0 Or template2 = 0) Then
                CreateMemo2.Visible = False
            Else
                CreateMemo2.Attributes.Add("onclick", "showEditor(0," + CInt(template2).ToString() + ", " + CInt(ModuleType).ToString() + ");")
            End If
        End Sub

#End Region

    End Class

End Namespace