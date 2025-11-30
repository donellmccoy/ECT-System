Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IN

    Partial Class Secure_sc_in_SMData
        Inherits System.Web.UI.Page

#Region "IncapProperty"

        Private _dao As ISpecialCaseDAO
        Private sc As SC_Incap

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _dao
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseIncap
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_Incap
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(refId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

        Protected Sub GetData()

            SetLabelText(lblName, SpecCase.MemberName)
            If SpecCase.MemberRank IsNot Nothing Then
                SetLabelText(lblRank, SpecCase.MemberRank.Title)
            End If
            SetLabelText(lblCompo, Utility.GetCompoString(SpecCase.MemberCompo))
            SetLabelText(lblUnit, SpecCase.MemberUnit)
            SetDateLabel(lbldob, SpecCase.MemberDOB)

        End Sub

        '''<summary>
        '''<mod date="2011-02-28">The member case history grid is being filled by using asynchronlus delegate .
        '''</mod>
        '''</summary>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                SigCheck.VerifySignature(refId)
                GetData()
                TabControl.Item(NavigatorButtonType.Save).Visible = False

                If (UserHasPermission("sysAdmin")) Then
                    ChangeUnitButton.Attributes.Add("onclick", "showSearcher('Select New Unit'); return false;")
                Else
                    ChangeUnitButton.Visible = False
                End If

                CaseHistory.Initialize(Me, SpecCase.MemberSSN, SpecCase.CaseId, True)

                LogManager.LogAction(ModuleType, UserAction.ViewPage, refId, "Viewed Page: Member")

            End If

        End Sub

        Protected Sub SaveUnitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveUnitButton.Click
            If (Not UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                Exit Sub
            End If

            Dim newUnitId As Integer = 0

            If (Not Integer.TryParse(newUnitIDLabel.Text.Trim, newUnitId)) Then
                Exit Sub
            End If

            Dim newUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(newUnitId)

            If (newUnit Is Nothing) Then
                Exit Sub
            End If

            If (SpecCase Is Nothing) Then
                Exit Sub
            End If

            If (SpecCase.MemberUnitId <> newUnit.Id) Then

                LogManager.LogAction(ModuleType, UserAction.UpdateCaseUnit, SpecCase.Id, "Unit changed from: " + SpecCase.MemberUnit + " to " + newUnit.Name, SpecCase.Status)

                SpecCase.MemberUnitId = newUnit.Id
                SpecCase.MemberUnit = newUnit.Name
                lblUnit.Text = newUnit.Name

                SCDao.SaveOrUpdate(SpecCase)

                SigBlock.StartSignature(refId, SpecCase.Workflow, 0, "Updated Member unit", SpecCase.Status, SpecCase.Status, 0, DBSignTemplateId.SignOnly, "")

            End If
        End Sub

    End Class

End Namespace