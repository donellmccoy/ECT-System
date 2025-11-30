Imports System.Web
Imports System.Web.UI.WebControls

Namespace Common
    Public Module WebControlSetters

        Public Sub SetCheckBox(ByVal checkbox As CheckBox, ByVal val As Boolean?)
            If (val Is Nothing OrElse Not val.HasValue) Then
                Exit Sub
            End If

            checkbox.Checked = val
        End Sub

        Public Sub SetDateLabel(ByVal label As Label, ByVal theDate As Nullable(Of DateTime))
            If (label Is Nothing OrElse Not theDate.HasValue) Then
                Exit Sub
            End If

            SetLabelText(label, theDate.Value.ToString(DATE_FORMAT))
        End Sub

        Public Sub SetDateTextbox(ByVal textbox As TextBox, ByVal theDate As Nullable(Of DateTime))
            If (textbox Is Nothing OrElse Not theDate.HasValue) Then
                Exit Sub
            End If

            SetTextboxText(textbox, theDate.Value.ToString(DATE_FORMAT))
        End Sub

        Public Sub SetDateTimeLabel(ByVal label As Label, ByVal dateTime As Nullable(Of DateTime))
            If (label Is Nothing OrElse Not dateTime.HasValue) Then
                Exit Sub
            End If

            SetLabelText(label, dateTime.Value.ToString(DATE_HOUR_FORMAT))
        End Sub

        Public Sub SetDropdownByValue(ByVal list As DropDownList, ByVal value As String)
            If (list Is Nothing OrElse String.IsNullOrEmpty(value)) Then
                Exit Sub
            End If

            If (list.Items.FindByValue(value) IsNot Nothing) Then
                list.SelectedValue = value
            End If
        End Sub

        Public Sub SetLabelText(ByVal label As Label, ByVal text As String)
            If (label Is Nothing OrElse String.IsNullOrEmpty(text)) Then
                Exit Sub
            End If

            label.Text = HttpContext.Current.Server.HtmlDecode(text)
        End Sub

        Public Sub SetMaxLength(ByVal input As TextBox)
            input.Attributes.Add("maxLength", input.MaxLength.ToString())
            AddCssClass(input, "textLimit")
        End Sub

        Public Sub SetMaxLength(ByVal input As TextBox, ByVal charRemaining As Boolean)
            input.Attributes.Add("maxLength", input.MaxLength.ToString())
            If (charRemaining) Then
                AddCssClass(input, "textLimit")
            End If

        End Sub

        Public Sub SetTextboxText(ByVal textbox As TextBox, ByVal text As String)
            If (textbox Is Nothing OrElse String.IsNullOrEmpty(text)) Then
                Exit Sub
            End If

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text)
        End Sub

        Public Sub SetTextboxText(ByVal textbox As TextBox, ByVal text As Nullable(Of Decimal))
            If (textbox Is Nothing OrElse Not text.HasValue) Then
                Exit Sub
            End If

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text)
        End Sub

        Public Sub SetTextboxText(ByVal textbox As TextBox, ByVal text As Nullable(Of Integer))
            If (textbox Is Nothing OrElse Not text.HasValue) Then
                Exit Sub
            End If

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text)
        End Sub

        Public Sub SetTimeTextbox(ByVal textbox As TextBox, ByVal timeAsDateTime As Nullable(Of DateTime))
            If (textbox Is Nothing OrElse Not timeAsDateTime.HasValue) Then
                Exit Sub
            End If

            SetTextboxText(textbox, timeAsDateTime.Value.ToString(HOUR_FORMAT))
        End Sub

    End Module
End Namespace