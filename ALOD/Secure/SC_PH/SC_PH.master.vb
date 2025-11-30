Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.PH

    Partial Class SC_PHMaster
        Inherits System.Web.UI.MasterPage

        Dim _dao As ISpecialCaseDAO

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

            Dim requestId As Integer = 0
            Integer.TryParse(Request.QueryString("requestId"), requestId)

            If (requestId = 0) Then
                Integer.TryParse(Request.QueryString("RefId"), requestId)
            End If

            If (requestId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("RequestId"), sessionId)

            If (sessionId = 0) Then
                Integer.TryParse(Session("RefId"), sessionId)
            End If

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (requestId <> sessionId) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            ModHeader.ModuleHeader = "PH"
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked

            RaiseEvent TabClick(Me, e)

        End Sub

    End Class

End Namespace