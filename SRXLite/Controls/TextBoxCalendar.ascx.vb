Imports SRXLite.Modules

Namespace Web.Controls

    Partial Class Controls_TextBoxCalendar
        Inherits System.Web.UI.UserControl

        Private _selectedDate As Date
        Private _textBoxOnChange As String
        Private _textBoxValue As String

#Region " Properties "
        Public Property SelectedDate() As Date
            Get
                Return DateCheck(txtDate.Text, Nothing)
            End Get
            Set(ByVal value As Date)
                txtDate.Text = value.ToShortDateString
            End Set
        End Property

        Public ReadOnly Property TextBox() As TextBox
            Get
                Return txtDate
            End Get
        End Property

        Public Property TextBoxOnChange() As String
            Get
                Return _textBoxOnChange
            End Get
            Set(ByVal value As String)
                _textBoxOnChange = value
                txtDate.Attributes.Add("onchange", _textBoxOnChange)
            End Set
        End Property

        Public Property Value() As String
            Get
                Return txtDate.Text.Trim
            End Get
            Set(ByVal value As String)
                txtDate.Text = value
            End Set
        End Property
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        Protected Sub txtDate_TextChanged(sender As Object, e As EventArgs)

        End Sub
    End Class

End Namespace