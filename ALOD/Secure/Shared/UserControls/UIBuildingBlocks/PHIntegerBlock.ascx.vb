Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHIntegerBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        'Private Const MaxLength As Integer = 5

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return txtInteger.Attributes("alt")
            End Get
            Set(value As String)
                txtInteger.Attributes.Add("alt", value)
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return txtInteger.BackColor
            End Get
            Set(value As Color)
                txtInteger.BackColor = value
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
                Return txtInteger.CssClass
            End Get
            Set(value As String)
                txtInteger.CssClass = value
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
                Return txtInteger.Enabled
            End Get
            Set(value As Boolean)
                'txtInteger.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return txtInteger.ForeColor
            End Get
            Set(value As Color)
                txtInteger.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return False
            End Get
            Set(value As Boolean)
                txtInteger.ReadOnly = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return txtInteger.MaxLength
            End Get
            Set(value As Integer)
                txtInteger.MaxLength = value
            End Set
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return txtInteger.Attributes("placeholder")
            End Get
            Set(value As String)
                txtInteger.Attributes.Add("placeholder", value)
            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return txtInteger.Text
            End Get
            Set(value As String)
                txtInteger.Text = value
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
                Return txtInteger.ToolTip
            End Get
            Set(value As String)
                txtInteger.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            If (String.IsNullOrEmpty(txtInteger.Text)) Then
                CssClass = PHUtility.CSS_FIELD_NUMBERSTEXT_BOX
                Return True
            End If

            Dim isInputValid As Boolean = True

            If (txtInteger.Text.Length > MaxLength) Then
                isInputValid = False
            End If

            If (isInputValid = True AndAlso Not IsNumeric(txtInteger.Text)) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_NUMBERSTEXT_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            SetInputFormatRestriction(Page, txtInteger, FormatRestriction.Numeric)
            'txtInteger.MaxLength = MaxLength
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace