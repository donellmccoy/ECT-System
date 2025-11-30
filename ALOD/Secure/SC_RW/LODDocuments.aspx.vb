Imports AjaxControlToolkit
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.RW

    Partial Class Secure_sc_rw_lod_Documents
        Inherits System.Web.UI.Page

        Private _assocaiatedCaseDao As IAssociatedCaseDao
        Private _daoFactory As IDaoFactory
        Private _documents As LODDocuments
        Private _lod As LineOfDuty = Nothing
        Private _sc As SC_RW = Nothing
        Private _scId As Integer = 0
        Private _specCaseDao As ISpecialCaseDAO
        Private ControlList As IList(Of LODDocuments) = New List(Of LODDocuments)

        Protected ReadOnly Property associated() As IAssociatedCaseDao
            Get
                If (_assocaiatedCaseDao Is Nothing) Then
                    _assocaiatedCaseDao = DaoFactory.GetAssociatedCaseDao()
                End If

                Return _assocaiatedCaseDao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LODCase() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow).First.associated_RefId)
                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property LODDocumentsForm As LODDocuments
            Get
                If (_documents Is Nothing) Then
                    _documents = New LODDocuments
                End If

                Return _documents
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_RWMaster
            Get
                Dim master As SC_RWMaster = CType(Page.Master, SC_RWMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_RW
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return _sc
            End Get
        End Property

        Protected Sub BuildDynamicUI()
            Dim AssociatedLODs = associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow)

            For Each s In AssociatedLODs
                Dim accPane As New AccordionPane()
                accPane.ID = "accp_AssociatedLOD_" + s.associated_RefId.ToString()
                accPane.HeaderContainer.Controls.Add(CreateSectionHeaderLink(s.associated_caseId, s.associated_RefId))
                accPane.HeaderContainer.ToolTip = "Click to expand..."

                accPane.ContentContainer.Controls.Add(CreateSectionInnerPanel(s.associated_RefId))
                accLODDocuments.Panes.Add(accPane)
            Next
        End Sub

        Protected Function CreateSectionHeaderLink(ByVal name As String, ByVal id As Integer) As HyperLink
            Dim l As New HyperLink

            l.Text = name
            l.Attributes.Add("onclick", "redirect(" + CType(id, String) + "); return false;")

            Return l
        End Function

        Protected Function CreateSectionInnerPanel(ByVal id As Integer) As UpdatePanel
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
            Utility.AddStyleSheet(Page, "~/Styles/AssociatedDocuments.css")

            If (ControlList.Count = 0) Then
                LoadNoLODsControls()
            Else
                LoadLODDocumentsControls()
            End If
        End Sub

    End Class

End Namespace