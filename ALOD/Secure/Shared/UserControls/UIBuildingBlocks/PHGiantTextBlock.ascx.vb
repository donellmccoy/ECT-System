Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHGiantTextBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return txtGiantText.Attributes("alt")
            End Get
            Set(value As String)
                txtGiantText.Attributes.Add("alt", value)
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return txtGiantText.BackColor
            End Get
            Set(value As Color)
                txtGiantText.BackColor = value
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
                Return txtGiantText.CssClass
            End Get
            Set(value As String)
                txtGiantText.CssClass = value
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
                Return txtGiantText.Enabled
            End Get
            Set(value As Boolean)
                txtGiantText.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return txtGiantText.ForeColor
            End Get
            Set(value As Color)
                txtGiantText.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return lblGiantText.Visible
            End Get
            Set(value As Boolean)
                txtGiantText.Visible = (Not value)
                lblGiantText.Visible = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return txtGiantText.MaxLength
            End Get
            Set(value As Integer)
                txtGiantText.MaxLength = value
            End Set
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return txtGiantText.Attributes("placeholder")
            End Get
            Set(value As String)
                txtGiantText.Attributes.Add("placeholder", value)
            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return txtGiantText.Text
            End Get
            Set(value As String)
                txtGiantText.Text = value
                lblGiantText.Text = value
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
                Return txtGiantText.ToolTip
            End Get
            Set(value As String)
                txtGiantText.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            If (String.IsNullOrEmpty(txtGiantText.Text)) Then
                CssClass = PHUtility.CSS_FIELD_GIANTTEXT_BOX
                Return True
            End If

            Dim isInputValid As Boolean = True

            If (txtGiantText.Text.Length > MaxLength) Then
                isInputValid = False
            End If

            ' Check if any non permitted characters are being used...
            If (isInputValid = True AndAlso DoesStringContainNonPermittedCharacters(txtGiantText.Text)) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_GIANTTEXT_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            SetInputFormatRestriction(Page, txtGiantText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                txtGiantText.Attributes.Add("maxLength", txtGiantText.MaxLength.ToString())
            End If
        End Sub

    End Class

End Namespace