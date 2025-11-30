Imports ALOD.Core.Domain.Workflow
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.BMT

    Partial Class UnitComments
        Inherits System.Web.UI.Page

#Region "Properties"

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseBMT
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

            CaseComment.Initialize(Me, ModuleType, RequestId, Navigator, False)

        End Sub

#End Region

    End Class

End Namespace