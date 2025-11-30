Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.RR

    Partial Class Secure_rr_SMData
        Inherits System.Web.UI.Page

        Private _dao As ILineOfDutyDao
        Private _lod As LineOfDuty
        Private _rdao As ILODReinvestigateDAO
        Private _reinvestigation As LODReinvestigation

#Region "Properties"

        Protected ReadOnly Property Dao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetLineOfDutyDao()
                End If

                Return _dao
            End Get
        End Property

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = Dao.GetById(reinvestigationRequest.InitialLodId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.ReinvestigationRequest
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property reinvestigationRequest() As LODReinvestigation
            Get
                If (_reinvestigation Is Nothing) Then
                    _reinvestigation = RRDao.GetById(RequestId, False)

                End If
                Return _reinvestigation
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("requestId"))
            End Get
        End Property

        Protected ReadOnly Property RRDao() As ILODReinvestigateDAO
            Get
                If (_rdao Is Nothing) Then
                    _rdao = New NHibernateDaoFactory().GetLODReinvestigationDao()
                End If

                Return _rdao
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

        Protected Sub GetData()
            SetLabelText(lblName, reinvestigationRequest.MemberName)
            SetLabelText(lblCompo, Utility.GetCompoString(reinvestigationRequest.MemberCompo))
            SetLabelText(lblUnit, reinvestigationRequest.MemberUnit)
            SetDateLabel(lbldob, LOD.MemberDob)

            If (reinvestigationRequest.MemberRank IsNot Nothing) Then
                SetLabelText(lblRank, reinvestigationRequest.MemberRank.Title)
            End If
        End Sub

        Protected Sub InitSigCheckControl()
            If (Not reinvestigationRequest.IsNonDBSignCase) Then
                SigCheck.VerifySignature(RequestId)
                SigCheck.Visible = True
            Else
                SigCheck.Visible = False
            End If
        End Sub

        '''<summary>
        '''<mod date="2011-02-28">The member case history grid is being filled by using asynchronlus delegate .
        '''</mod>
        '''</summary>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitSigCheckControl()
                GetData()
                TabControl.Item(NavigatorButtonType.Save).Visible = False

                CaseHistory.Initialize(Me, reinvestigationRequest.MemberSSN, reinvestigationRequest.CaseId, False)

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RequestId, "Viewed Page: Member")
            End If
        End Sub

    End Class

End Namespace