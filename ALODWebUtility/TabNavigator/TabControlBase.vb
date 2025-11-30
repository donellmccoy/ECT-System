Imports System.Web.UI

Namespace TabNavigation

    Public MustInherit Class TabControlBase
        Inherits UserControl

        Public MustOverride Property BackEnabled() As Boolean
        Public MustOverride Property NextEnabled() As Boolean
        Public MustOverride ReadOnly Property Item(ByVal type As ALOD.Core.Domain.Common.NavigatorButtonType) As System.Web.UI.WebControls.Button
    End Class

End Namespace