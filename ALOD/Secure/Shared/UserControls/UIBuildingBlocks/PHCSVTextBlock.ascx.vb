Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHCSVTextBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return txtCSV.Attributes("alt")
            End Get
            Set(value As String)
                txtCSV.Attributes.Add("alt", value)
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return txtCSV.BackColor
            End Get
            Set(value As Color)
                txtCSV.BackColor = value
            End Set
        End Property

        Public Property ControlId As String Implements IPHFormUIBuildingBlock.ControlId
            Get
                Return Me.ID
            End Get
            Set(value As String)
                Me.ID = value
            End Set
        End Property

        Public Property CssClass As String Implements IPHFormUIBuildingBlock.CssClass
            Get
                Return txtCSV.CssClass
            End Get
            Set(value As String)
                txtCSV.CssClass = value
                lblCSV.CssClass = value & " fieldWordWrapLabel"
            End Set
        End Property

        Public Property DataSource As String Implements IPHFormUIBuildingBlock.DataSource
            Get
                Return String.Empty
            End Get
            Set(value As String)

            End Set
        End Property

        Public Property Enabled As Boolean Implements IPHFormUIBuildingBlock.Enabled
            Get
                Return txtCSV.Enabled
            End Get
            Set(value As Boolean)
                txtCSV.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return txtCSV.ForeColor
            End Get
            Set(value As Color)
                txtCSV.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return lblCSV.Visible
            End Get
            Set(value As Boolean)
                txtCSV.Visible = (Not value)
                lblCSV.Visible = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return txtCSV.MaxLength
            End Get
            Set(value As Integer)
                txtCSV.MaxLength = value
            End Set
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return txtCSV.Attributes("placeholder")
            End Get
            Set(value As String)
                txtCSV.Attributes.Add("placeholder", value)
            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return GetTextAsCSV()
            End Get
            Set(value As String)
                txtCSV.Text = value
                lblCSV.Text = value.Replace(",", ", ")
            End Set
        End Property

        Public Property ScreenReaderControlEnabled As Boolean Implements IPHFormUIBuildingBlock.ScreenReaderControlEnabled
            Get
                Return lblScreenReaderText.Enabled
            End Get
            Set(value As Boolean)
                lblScreenReaderText.Enabled = value
                lblScreenReaderText.Visible = value
            End Set
        End Property

        Public Property ScreenReaderText As String Implements IPHFormUIBuildingBlock.ScreenReaderText
            Get
                Return lblScreenReaderText.Text
            End Get
            Set(value As String)
                lblScreenReaderText.Text = value
            End Set
        End Property

        Public Property SecondaryValue As String Implements IPHFormUIBuildingBlock.SecondaryValue
            Get
                Return PrimaryValue
            End Get
            Set(value As String)
                PrimaryValue = value
            End Set
        End Property

        Public Property ToolTip As String Implements IPHFormUIBuildingBlock.ToolTip
            Get
                Return txtCSV.ToolTip
            End Get
            Set(value As String)
                txtCSV.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            If (String.IsNullOrEmpty(txtCSV.Text)) Then
                CssClass = PHUtility.CSS_FIELD_CSV_BOX
                Return True
            End If

            Dim isInputValid As Boolean = True

            If (txtCSV.Text.Length > MaxLength) Then
                isInputValid = False
            End If

            ' Check if any non permitted characters are being used...
            If (isInputValid = True AndAlso DoesStringContainNonPermittedCharacters(txtCSV.Text)) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_CSV_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            SetInputFormatRestriction(Page, txtCSV, FormatRestriction.AlphaNumeric, ".,,,/,:,?")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        Private Function GetTextAsCSV() As String
            If (String.IsNullOrEmpty(txtCSV.Text)) Then
                Return String.Empty
            End If

            Dim csv As String = txtCSV.Text

            csv.Trim()
            csv = csv.Replace(" ", ",")
            csv = csv.Replace(Environment.NewLine, ",")

            While (csv.Contains(",,"))
                csv = csv.Replace(",,", ",")
            End While

            Return csv
        End Function

    End Class

End Namespace