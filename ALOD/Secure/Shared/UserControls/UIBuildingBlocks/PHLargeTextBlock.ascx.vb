Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHLargeTextBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        'Private Const MaxLength As Integer = 255

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return txtLongText.Attributes("alt")
            End Get
            Set(value As String)
                txtLongText.Attributes.Add("alt", value)
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return txtLongText.BackColor
            End Get
            Set(value As Color)
                txtLongText.BackColor = value
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
                Return txtLongText.CssClass
            End Get
            Set(value As String)
                txtLongText.CssClass = value
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
                Return txtLongText.Enabled
            End Get
            Set(value As Boolean)
                txtLongText.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return txtLongText.ForeColor
            End Get
            Set(value As Color)
                txtLongText.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return lblLongText.Visible
            End Get
            Set(value As Boolean)
                txtLongText.Visible = (Not value)
                lblLongText.Visible = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return txtLongText.MaxLength
            End Get
            Set(value As Integer)
                txtLongText.MaxLength = value
            End Set
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return txtLongText.Attributes("placeholder")
            End Get
            Set(value As String)
                txtLongText.Attributes.Add("placeholder", value)
            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return txtLongText.Text
            End Get
            Set(value As String)
                txtLongText.Text = value
                lblLongText.Text = value
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
                Return txtLongText.ToolTip
            End Get
            Set(value As String)
                txtLongText.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            If (String.IsNullOrEmpty(txtLongText.Text)) Then
                CssClass = PHUtility.CSS_FIELD_LARGETEXT_BOX
                Return True
            End If

            Dim isInputValid As Boolean = True

            If (txtLongText.Text.Length > MaxLength) Then
                isInputValid = False
            End If

            ' Check if any non permitted characters are being used...
            If (isInputValid = True AndAlso DoesStringContainNonPermittedCharacters(txtLongText.Text)) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_LARGETEXT_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            SetInputFormatRestriction(Page, txtLongText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            'txtLongText.MaxLength = MaxLength
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace