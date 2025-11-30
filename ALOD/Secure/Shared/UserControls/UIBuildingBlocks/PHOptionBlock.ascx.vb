Imports System.Drawing
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.UserControls.UIBuildingBlocks

    Public Class PHOptionBlock
        Inherits System.Web.UI.UserControl
        Implements IPHFormUIBuildingBlock

        Private _dataSource As String
        Private _phDao As IPsychologicalHealthDao

        Public Property AltAttribute As String Implements IPHFormUIBuildingBlock.AltAttribute
            Get
                Return ddlOptions.Attributes("alt")
            End Get
            Set(value As String)
                ddlOptions.Attributes.Add("alt", value & " Selection")
            End Set
        End Property

        Public Property BackColor As Color Implements IPHFormUIBuildingBlock.BackColor
            Get
                Return ddlOptions.BackColor
            End Get
            Set(value As Color)
                ddlOptions.BackColor = value
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
                Return ddlOptions.CssClass
            End Get
            Set(value As String)
                ddlOptions.CssClass = value
            End Set
        End Property

        Public Property DataSource As String Implements IPHFormUIBuildingBlock.DataSource
            Get
                Return _dataSource
            End Get
            Set(value As String)
                _dataSource = value
            End Set
        End Property

        Public Property Enabled As Boolean Implements IPHFormUIBuildingBlock.Enabled
            Get
                Return ddlOptions.Enabled
            End Get
            Set(value As Boolean)
                ddlOptions.Enabled = value
            End Set
        End Property

        Public Property ForeColor As Color Implements IPHFormUIBuildingBlock.ForeColor
            Get
                Return ddlOptions.ForeColor
            End Get
            Set(value As Color)
                ddlOptions.ForeColor = value
            End Set
        End Property

        Public Property IsReadOnly As Boolean Implements IPHFormUIBuildingBlock.IsReadOnly
            Get
                Return lblOptions.Visible
            End Get
            Set(value As Boolean)
                ddlOptions.Visible = (Not value)
                lblOptions.Visible = value
            End Set
        End Property

        Public Property MaxLength As Integer Implements IPHFormUIBuildingBlock.MaxLength
            Get
                Return 0
            End Get
            Set(value As Integer)

            End Set
        End Property

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Public Property Placeholder As String Implements IPHFormUIBuildingBlock.Placeholder
            Get
                Return String.Empty
            End Get
            Set(value As String)

            End Set
        End Property

        Public Property PrimaryValue As String Implements IPHFormUIBuildingBlock.PrimaryValue
            Get
                Return ddlOptions.SelectedValue
            End Get
            Set(value As String)
                ddlOptions.SelectedValue = value
                lblOptions.Text = ddlOptions.SelectedItem.Text
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
                Return ddlOptions.ToolTip
            End Get
            Set(value As String)
                ddlOptions.ToolTip = value
            End Set
        End Property

        Public Function ValidateInput() As Boolean Implements IPHFormUIBuildingBlock.ValidateInput
            Dim isInputValid As Boolean = True

            If (ddlOptions.SelectedIndex < 0 OrElse ddlOptions.SelectedIndex > ddlOptions.Items.Count) Then
                isInputValid = False
            End If

            ' Apply CSS styling based on whether the input is valid or not...
            If (isInputValid = False) Then
                CssClass = CssClass & " " & PHUtility.CSS_FIELD_VALIDATION_ERROR
            Else
                CssClass = PHUtility.CSS_FIELD_OPTIONS_BOX
            End If

            Return isInputValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            BindDDL()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then

            End If
        End Sub

        Private Sub BindDDL()
            If (Not String.IsNullOrEmpty(DataSource)) Then
                ddlOptions.DataSource = PHDao.GetDataSetFromProcedure(DataSource)
                ddlOptions.DataValueField = "Id"
                ddlOptions.DataTextField = "Name"
                ddlOptions.DataBind()

                InsertDropDownListZeroValue(ddlOptions, "-- Choose an Item --")
            End If
        End Sub

    End Class

End Namespace