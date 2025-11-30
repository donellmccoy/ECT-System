Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHSmallTextBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        'Private Const MaxLength As Integer = 50

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return txtShortText.Attributes("alt")
            End Get
            Set(value As String)
                txtShortText.Attributes.Add("alt", value)
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return txtShortText.BackColor
            End Get
            Set(value As Color)
                txtShortText.BackColor = value
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
                Return txtShortText.CssClass
            End Get
            Set(value As String)
                txtShortText.CssClass = value
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
                Return txtShortText.Enabled
            End Get
            Set(value As Boolean)
                txtShortText.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return txtShortText.ForeColor
            End Get
            Set(value As Color)
                txtShortText.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return lblShortText.Visible
            End Get
            Set(value As Boolean)
                txtShortText.Visible = (Not value)
                lblShortText.Visible = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return txtShortText.MaxLength
            End Get
            Set(value As Integer)
                txtShortText.MaxLength = value
            End Set
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return txtShortText.Attributes("placeholder")
            End Get
            Set(value As String)
                txtShortText.Attributes.Add("placeholder", value)
            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return txtShortText.Text
            End Get
            Set(value As String)
                txtShortText.Text = value
                lblShortText.Text = value
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
                Return txtShortText.ToolTip
            End Get
            Set(value As String)
                txtShortText.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            If (String.IsNullOrEmpty(txtShortText.Text)) Then
                CssClass = PHUtility.CSS_FIELD_SMALLTEXT_BOX
                Return True
            End If

            Dim isInputValid As Boolean = True

            If (txtShortText.Text.Length > MaxLength) Then
                isInputValid = False
            End If

            ' Check if any non permitted characters are being used...
            If (isInputValid = True AndAlso DoesStringContainNonPermittedCharacters(txtShortText.Text)) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_SMALLTEXT_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            SetInputFormatRestriction(Page, txtShortText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            'txtShortText.MaxLength = MaxLength
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace