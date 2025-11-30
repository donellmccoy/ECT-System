Imports ALOD.Core.Domain.Common

Namespace Web.UserControls

    Partial Class Secure_Controls_ValidationResults
        Inherits System.Web.UI.UserControl

        Protected _items As New List(Of ValidationItem)
        Protected _values As New Dictionary(Of String, List(Of ValidationItem))

        Public Property DataSource() As List(Of ValidationItem)
            Get
                Return _items
            End Get
            Set(ByVal value As List(Of ValidationItem))
                If (value Is Nothing) Then
                    Exit Property
                End If
                _items = value
                For Each item As ValidationItem In value
                    Add(item)
                Next
                Display()
            End Set
        End Property

        Public Property ShowEmpty() As Boolean
            Get

                If (ViewState("ShowEmpty") Is Nothing) Then
                    Return True

                End If
                Return CBool(ViewState("ShowEmpty"))

            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowEmpty") = value
            End Set
        End Property

        Public Sub Add(ByVal item As ValidationItem)
            If (Not _values.ContainsKey(item.Section)) Then
                _values.Add(item.Section, New List(Of ValidationItem))
            End If

            _values(item.Section).Add(item)

        End Sub

        Public Function GetBySection(ByVal section As String) As IList(Of ValidationItem)
            If (Not _values.ContainsKey(section)) Then
                Return Nothing
            End If

            Return _values(section)
        End Function

        Public Function GetSections() As StringCollection
            Dim keys As New StringCollection

            For Each key As String In _values.Keys
                keys.Add(key)
            Next

            Return keys

        End Function

        Protected Sub Display()

            If (_items Is Nothing) Then
                Me.Visible = False
                Exit Sub
            End If

            If (_items.Count = 0) Then

                If (ShowEmpty) Then
                    pnlEmpty.Visible = True
                    rptItems.Visible = False
                    pnlWarning.Visible = False
                End If

                Exit Sub
            End If
            pnlWarning.Visible = True
            pnlEmpty.Visible = False
            rptItems.Visible = True
            Me.Visible = True

            Dim keys As New StringCollection
            keys = GetSections()
            Dim sections As New List(Of String)()

            For i As Integer = 0 To keys.Count - 1
                sections.Add(keys.Item(i))
            Next

            rptItems.DataSource = sections
            rptItems.DataBind()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                If (Not ShowEmpty) Then
                    pnlEmpty.Visible = False
                End If
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If (Not IsPostBack) Then
                _items = CType(ViewState("Items"), List(Of ValidationItem))
                'Display()
            Else
                ' Display()
            End If
        End Sub

        Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            ViewState("Items") = _items
        End Sub

        Protected Sub rptItems_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptItems.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item) OrElse (e.Item.ItemType = ListItemType.AlternatingItem) Then

                Dim section As Label = CType(e.Item.FindControl("lblSection"), Label)
                Dim rptMessage As Repeater = CType(e.Item.FindControl("rptMessage"), Repeater)
                section.Text = e.Item.DataItem + ":"
                Dim messages As New List(Of ValidationItem)
                messages = GetBySection(e.Item.DataItem)
                rptMessage.DataSource = messages
                rptMessage.DataBind()

            End If

        End Sub

        Protected Sub rptMessage_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim valItem As ValidationItem = CType(e.Item.DataItem, ValidationItem)
            Dim msgLbl As Label = CType(e.Item.FindControl("lblItem"), Label)
            If Not (valItem.IsError) Then
                msgLbl.CssClass = "valWarning"
            End If

        End Sub

    End Class

End Namespace