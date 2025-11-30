Imports ALOD.Core.Domain.Workflow
Imports ALOD.Secure.Shared.UserControls
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.WWD

    Public Class Secure_sc_CaseDialogue
        Inherits System.Web.UI.Page

#Region "Properties"

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseWWD
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

#Region "Load"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then
                CaseDialogue.Initialize(Me, ModuleType, RequestId, Navigator, False, True)
            End If

        End Sub

#End Region

    End Class

End Namespace