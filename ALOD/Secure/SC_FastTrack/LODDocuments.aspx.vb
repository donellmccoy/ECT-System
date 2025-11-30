Imports AjaxControlToolkit
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IRILO

    Partial Class Secure_sc__ft_lod_Documents
        Inherits System.Web.UI.Page

        Private _assocaiated As IAssociatedCaseDao
        Private _documents As LODDocuments
        Private _factory As IDaoFactory
        Private _workflowDao As IWorkflowDao
        Private ControlList As IList(Of LODDocuments) = New List(Of LODDocuments)
        Private dao As ISpecialCaseDAO
        Private sc As SC_FastTrack
        Private scId As Integer = 0

        ReadOnly Property associated() As IAssociatedCaseDao
            Get
                If (_assocaiated Is Nothing) Then
                    _assocaiated = factory.GetAssociatedCaseDao()
                End If

                Return _assocaiated
            End Get
        End Property

        ReadOnly Property factory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = factory.GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property LODDocumentsForm As LODDocuments
            Get
                If (_documents Is Nothing) Then
                    _documents = New LODDocuments
                End If

                Return _documents
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_FastTrackMaster
            Get
                Dim master As SC_FastTrackMaster = CType(Page.Master, SC_FastTrackMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_FastTrack
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = factory.GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Public Function CreateSectionHeaderLabel(name As String) As Label

            Dim l As New Label

            l.Text = name

            Return l
        End Function

        Public Function CreateSectionHeaderLink(name As String, id As Integer) As HyperLink

            Dim l As New HyperLink

            l.Text = name
            l.Attributes.Add("onclick", "redirect(" + CType(id, String) + "); return false;")

            Return l
        End Function

        Public Function CreateSectionInnerPanel(ByVal id As Integer) As UpdatePanel
            Dim p As New UpdatePanel()
            Dim lod As LODDocuments

            p.ID = "pnlSection_Inner_" & id

            lod = LoadControl("~/secure/shared/UserControls/LODDocuments.ascx")
            lod.ID = "ucLODDocuments_" & id
            lod.Lod_Id = id

            ControlList.Add(lod)

            p.ContentTemplateContainer.Controls.Add(lod)
            Return p
        End Function

        Protected Function GetIsFormal(refId As Integer, workflowId As Integer) As Boolean
            Dim moduleId As Integer = WorkflowDao.GetModuleFromWorkflow(workflowId)

            If (moduleId = ModuleType.LOD) Then

                Dim LOD = LodService.GetById(refId)

                Return LOD.Formal

            End If

            Return False

        End Function

        Protected Sub LoadLODDocumentsControls()
            For Each lodControl In ControlList
                lodControl.Initialize(Me, lodControl.Lod_Id, False, Navigator)
            Next
        End Sub

        Protected Sub LoadNoLODsControls()
            accLODDocuments.Visible = False
            pnlNoLODs.Visible = True

            If (SpecCase.HasAdminLOD.HasValue AndAlso SpecCase.HasAdminLOD.Value) Then
                lblNoLODsMessage.Text = "Admin LOD"
            Else
                lblNoLODsMessage.Text = "No associated LODs"
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            BuildDynamicUI()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/AssociatedDocuments.css")

            If (ControlList.Count = 0) Then
                LoadNoLODsControls()
            Else
                LoadLODDocumentsControls()
            End If
        End Sub

        Private Sub BuildDynamicUI()
            Dim AssociatedLODs = associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow)

            For Each s In AssociatedLODs
                Dim accPane As New AccordionPane()
                accPane.ID = "accp_AssociatedLOD_" + s.associated_RefId.ToString()
                If (WorkflowDao.CanViewWorkflow(SESSION_GROUP_ID, s.associated_workflowId, GetIsFormal(s.associated_RefId, s.associated_workflowId))) Then
                    accPane.HeaderContainer.Controls.Add(CreateSectionHeaderLink(s.associated_caseId, s.associated_RefId))
                Else
                    accPane.HeaderContainer.Controls.Add(CreateSectionHeaderLabel(s.associated_caseId))
                End If
                accPane.HeaderContainer.ToolTip = "Click to expand..."

                accPane.ContentContainer.Controls.Add(CreateSectionInnerPanel(s.associated_RefId))
                accLODDocuments.Panes.Add(accPane)
            Next
        End Sub

    End Class

End Namespace