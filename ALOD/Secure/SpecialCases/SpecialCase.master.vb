Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces
Imports ALOD.Web.UserControls

Namespace Web.Special_Case

    Partial Class SpecialCaseMaster
        Inherits System.Web.UI.MasterPage

        Dim _dao As DAOInterfaces.ISpecialCaseDAO

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        Public ReadOnly Property ModHeader() As ModuleHeader
            Get
                Return ModuleHeader1
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return TabNavigator1
            End Get
        End Property

        Public ReadOnly Property NestedHolder() As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain").FindControl("ContentNested"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return TabControls1
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ModHeader.ModuleHeader = "SC"
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked

            RaiseEvent TabClick(Me, e)

        End Sub

    End Class

End Namespace