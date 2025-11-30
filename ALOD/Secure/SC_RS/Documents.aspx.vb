Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.RS

    Partial Class Secure_sc_rs_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _caseTypeDao As ICaseTypeDao
        Private _dao As IDocumentDao
        Private _dispositionDao As ILookupDispositionDao
        Private _factory As IDaoFactory
        Private _indexIncrement As Integer = 0
        Private _memoSource As MemoDao2
        Private _sc As SC_RS
        Private _special As SpecialCase
        Private _Specialdao As ISpecialCaseDAO
        Private _stampDao As ICertificationStampDao

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

        Protected ReadOnly Property IndexIncrement() As Integer
            Get
                If (_indexIncrement = 0) Then
                    If (GetAreMemosNeeded() OrElse SCDao.GetIsReassessmentCase(SpecCase.Id)) Then
                        _indexIncrement = 2
                    Else
                        _indexIncrement = 1
                    End If
                End If

                Return _indexIncrement
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseRS
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SC() As SC_RS
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

        Private ReadOnly Property CaseTypeDao() As ICaseTypeDao
            Get
                If (_caseTypeDao Is Nothing) Then
                    _caseTypeDao = DaoFactory.GetCaseTypeDao()
                End If

                Return _caseTypeDao
            End Get
        End Property

        Private ReadOnly Property DispositionDao() As ILookupDispositionDao
            Get
                If (_dispositionDao Is Nothing) Then
                    _dispositionDao = DaoFactory.GetLookupDispositionDao()
                End If

                Return _dispositionDao
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

                If (GetAreMemosNeeded()) Then
                    pnlMemos.Visible = True
                    InitMemoCreationControls()
                    UpdateMemoList()
                ElseIf (SCDao.GetIsReassessmentCase(SpecCase.Id)) Then
                    pnlMemos.Visible = True
                    UpdateMemoList()
                End If

            End If

        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click

            UpdateMemoList()
        End Sub

        Protected Sub UpdateMemoList()

            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                                      Where (m.Deleted = False)
                                      Select m
                                      Order By m.CreatedDate
            MemoRepeater.DataBind()
            CheckMemoList()

        End Sub

        Private Sub ApplySupplementalMemoImageVisibilityConditions(ByVal rsMemos As IList(Of Memorandum2))
            'To generate the RS ALC and RS Non-RLC memos the renewal date must be filled out on the medifcal officer page
            If (SC.MemoTemplateID.HasValue AndAlso (SC.MemoTemplateID.Value = MemoType.RSALCMemo OrElse SC.MemoTemplateID.Value = MemoType.RSNonALCMemo)) Then
                If (Not SC.ExpirationDate.HasValue) Then
                    CreateMemo.Visible = False
                End If
            End If

        End Sub

        Private Sub CheckMemoList()
            If (Not Documents.UserCanEdit) Then
                CreateMemo.Visible = False
                Exit Sub
            End If

            ' Get RS memo(s) if they exist...
            Dim rsMemos As IList(Of Memorandum2)
            rsMemos = (From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                       Where m.Deleted = False AndAlso m.CreatedDate >= SpecCase.CreatedDate
                       Select m).ToList()

            If ((rsMemos IsNot Nothing AndAlso rsMemos.Count > 0) OrElse Not SC.MemoTemplateID.HasValue) Then
                CreateMemo.Visible = False
            Else
                'CreateMemo.Visible = True
            End If

            ApplySupplementalMemoImageVisibilityConditions(rsMemos)
        End Sub

        Private Function GetAreMemosNeeded() As Boolean
            If (Not SC.CaseType.HasValue OrElse SC.CaseType.Value < 1) Then
                Return False
            End If

            Dim ct As CaseType = CaseTypeDao.GetById(SC.CaseType.Value)

            If (ct Is Nothing) Then
                Return False
            End If

            If (Not ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) AndAlso Not ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)) Then
                Return False
            End If

            Return True
        End Function

        Private Sub GetDocuments()

            'make sure we have a groupId
            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                SCDao.CommitChanges()
            End If

            SpecCase.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SpecCase, IndexIncrement, DaoFactory))

        End Sub

        Private Function GetMemoToolTipText(ByVal templateId As Integer) As String
            If (templateId = MemoType.PalaceChaseAFI) Then
                Return "Create Palace Chase AFI Memo"
            ElseIf (templateId = MemoType.PalaceChaseMSD) Then
                Return "Create Palace Chase MSD Memo"
            ElseIf (templateId = MemoType.PalaceChaseQualified) Then
                Return "Create Palace Chase Qualified Memo"
            ElseIf (templateId = MemoType.RSALCMemo) Then
                Return "Create RS ALC Memo"
            ElseIf (templateId = MemoType.RSNonALCMemo) Then
                Return "Create RS Non-ALC Memo"
            Else
                Return "Create RS Memo"
            End If
        End Function

        Private Function GetMemoURL(memoId As Integer) As String

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

        End Function

        Private Sub InitMemoCreationControls()
            Dim rsTemplateId As Integer = 0
            Dim rsTemplate As MemoTemplate = Nothing

            ' Get all RS memo templates...
            Dim templates = From t In MemoStore.GetAllTemplates()
                            Where t.Component = Session("Compo") And (t.ModuleType = ModuleType)
                            Select t

            ' Get RS memo(s) if they exist...
            Dim rsMemos As IList(Of Memorandum2)
            rsMemos = (From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                       Where m.Deleted = False AndAlso m.CreatedDate >= SpecCase.CreatedDate
                       Select m).ToList()

            ' Get which memo(s) have been selected...
            Select Case SpecCase.Status
                Case SpecCaseRSWorkStatus.InitiateCase
                    ' Allow HQ Tech to generate the non ALC memos
                    If (SC.MemoTemplateID.HasValue AndAlso SC.MemoTemplateID.Value > 0 AndAlso SC.MemoTemplateID.Value <> MemoType.RSALCMemo AndAlso SC.MemoTemplateID.Value <> MemoType.RSNonALCMemo) Then
                        rsTemplate = (From t In templates Select t Where t.Id = SC.MemoTemplateID.Value).FirstOrDefault()
                    End If

                Case SpecCaseRSWorkStatus.FinalReview, SpecCaseRSWorkStatus.RecruiterComments
                    ' If the disposition is qualified AND an ALC code is selected then get the ALC memo template; otherwise, get the RS memo template selected on the HQ Tech tab...
                    If (SC.GetActiveDispositionValue(DispositionDao) = 1 AndAlso SC.GetActiveALC() > 0) Then
                        rsTemplate = (From t In templates Select t Where t.Id = MemoType.RSALCMemo).FirstOrDefault()
                    ElseIf (SC.MemoTemplateID.HasValue AndAlso SC.MemoTemplateID.Value > 0) Then
                        rsTemplate = (From t In templates Select t Where t.Id = SC.MemoTemplateID.Value).FirstOrDefault()
                    End If
            End Select

            ' Determine if user has permission to create the memos...
            ' Check the RS template's group permissions against  the user's group Id...
            If (rsTemplate IsNot Nothing) Then
                Dim perms = (From g In rsTemplate.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

                If ((perms IsNot Nothing AndAlso perms.CanCreate AndAlso SpecCase.Status = SpecCaseRSWorkStatus.FinalReview) Or UserHasPermission("RSCanGenerateMemo")) Then
                    rsTemplateId = rsTemplate.Id
                End If
            End If

            ' If the user does not have permission to create the memos OR the memos have already been created then hide the create button...otherwise initialize the create button...
            If (rsTemplateId = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + rsTemplateId.ToString() + ", " + CInt(ModuleType).ToString() + ");")
                CreateMemo.ToolTip = GetMemoToolTipText(rsTemplateId)
            End If

        End Sub

#End Region

    End Class

End Namespace