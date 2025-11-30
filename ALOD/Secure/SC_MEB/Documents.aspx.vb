Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MEB

    Partial Class Secure_sc_meb_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _dao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _sc As SC_MEB
        Private _special As SpecialCase
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
                Return ModuleType.SpecCaseMEB
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SC() As SC_MEB
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(RequestId)
                End If

                Return _sc
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(RequestId, False)

                End If
                Return _special
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

            End If

        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click

            UpdateMemoList()

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

            If Not (SC.Status = SpecCaseMEBWorkStatus.FinalReview _
                    Or SC.Status = SpecCaseMEBWorkStatus.FinalReviewAFPC _
                    Or SC.Status = SpecCaseMEBWorkStatus.FinalReviewIPEB _
                    Or SC.Status = SpecCaseMEBWorkStatus.FinalReviewFPEB _
                    Or SC.Status = SpecCaseMEBWorkStatus.FinalReviewTransitionPhase) _
                    AndAlso (SC.GetActiveMemoTemplateId().HasValue) Then
                Return False
            End If

            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            Dim template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()

            Return HasPermission(template)

        End Function

        Private Sub CheckMemoList()

            If (Not CanCreateMemos()) Then
                CreateMemo.Visible = False
                CreateMemo2.Visible = False
                Exit Sub
            End If

            ' get memo list
            Dim memoList As IList(Of Memorandum2)
            memoList = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                        Where l.Deleted = False And l.Template.Id = SC.GetActiveMemoTemplateId()
                        Select l).ToList

            Dim memoListRTD As IList(Of Memorandum2)
            memoListRTD = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                           Where l.Deleted = False And (l.Template.Id = MemoType.MEB_4_Non_ALC_C_SAF Or
                                                        l.Template.Id = MemoType.MEB_4_Non_ALC_C_SG Or
                                                        l.Template.Id = MemoType.MEB_RTD_AGR_SAF Or
                                                        l.Template.Id = MemoType.MEB_RTD_AGR_SG Or
                                                        l.Template.Id = MemoType.MEB_RTD_HIV_AGR_SAF Or
                                                        l.Template.Id = MemoType.MEB_RTD_HIV_AGR_SG Or
                                                        l.Template.Id = MemoType.MEB_RTD_HIV_SAF Or
                                                        l.Template.Id = MemoType.MEB_RTD_HIV_SG Or
                                                        l.Template.Id = MemoType.MEB_RTD_SAF Or
                                                        l.Template.Id = MemoType.MEB_RTD_SG)
                           Select l).ToList

            If (memoList.Count > 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Visible = True
            End If

            If (memoListRTD.Count > 0 OrElse SC.Status = SpecCaseMEBWorkStatus.FinalReview) Then
                CreateMemo2.Visible = False
            Else
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

            'get the memo templates
            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            ' get RTD memos; if AFPC disposition
            Dim memoListRTD As IList(Of Memorandum2) = Nothing
            If (SpecCase.Status = SpecCaseMEBWorkStatus.FinalReviewAFPC OrElse SpecCase.Status = SpecCaseMEBWorkStatus.FinalReviewIPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.FinalReviewFPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.FinalReviewTransitionPhase) Then
                memoListRTD = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                               Where l.Deleted = False And (l.Template.Id = MemoType.MEB_4_Non_ALC_C_SAF Or
                                                            l.Template.Id = MemoType.MEB_4_Non_ALC_C_SG Or
                                                            l.Template.Id = MemoType.MEB_RTD_AGR_SAF Or
                                                            l.Template.Id = MemoType.MEB_RTD_AGR_SG Or
                                                            l.Template.Id = MemoType.MEB_RTD_HIV_AGR_SAF Or
                                                            l.Template.Id = MemoType.MEB_RTD_HIV_AGR_SG Or
                                                            l.Template.Id = MemoType.MEB_RTD_HIV_SAF Or
                                                            l.Template.Id = MemoType.MEB_RTD_HIV_SG Or
                                                            l.Template.Id = MemoType.MEB_RTD_SAF Or
                                                            l.Template.Id = MemoType.MEB_RTD_SG)
                               Select l).ToList
            End If

            Dim templateId As Integer = 0
            Dim templateId2 As Integer = 0

            Select Case SpecCase.Status
                Case SpecCaseMEBWorkStatus.FinalReview
                    'grab the proper memo
                    Dim template As MemoTemplate

                    If (SpecCase.GetActiveBoardMedicalFinding() = 1) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create MEB Return to Duty Memo"
                    Else
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create MEB Disqualification Letter"
                    End If

                    If (template IsNot Nothing) Then
                        'now get the user's permissions for this memo
                        Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

                        If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                            'the user has create permissions for this memo, so set it as the one to be created
                            templateId = template.Id

                        End If

                    End If

                Case SpecCaseMEBWorkStatus.FinalReviewAFPC, SpecCaseMEBWorkStatus.FinalReviewIPEB, SpecCaseMEBWorkStatus.FinalReviewFPEB, SpecCaseMEBWorkStatus.FinalReviewTransitionPhase
                    Dim template2 As MemoTemplate
                    template2 = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()

                    If (template2 IsNot Nothing) Then
                        'now get the user's permissions for this memo
                        Dim perms = (From g In template2.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()
                        If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                            'the user has create permissions for this memo, so set it as the one to be created
                            templateId2 = template2.Id
                        End If
                    End If

            End Select

            If (templateId = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + templateId.ToString() + ", " + CInt(ModuleType).ToString() + ");")
            End If

            If (templateId2 = 0) Then
                CreateMemo2.Visible = False
            Else
                CreateMemo2.Visible = True
                CreateMemo2.Attributes.Add("onclick", "showEditor(0," + templateId2.ToString() + ", " + CInt(ModuleType).ToString() + ");")

            End If

        End Sub

#End Region

    End Class

End Namespace