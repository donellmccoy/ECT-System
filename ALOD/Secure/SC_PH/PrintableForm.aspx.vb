Imports System.Collections.ObjectModel
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Web.UserControls.UIBuildingBlocks
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PH

    Public Class PrintableForm
        Inherits System.Web.UI.Page

        Private _phDao As IPsychologicalHealthDao
        Private _phForm As PHForm
        Private _sc As SC_PH = Nothing
        Private _scId As Integer = 0
        Private _specCaseDao As ISpecialCaseDAO
        Private _uiBuldingBlocks As List(Of IPHFormUIBuildingBlock)
        Private _userDao As IUserDao

        Public ReadOnly Property CasePHForm As PHForm
            Get
                If (_phForm Is Nothing) Then
                    _phForm = New PHForm(ReferenceId, PHDao)
                End If

                Return _phForm
            End Get
        End Property

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Public ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PH
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(ReferenceId)
                End If

                Return _sc
            End Get
        End Property

        Protected ReadOnly Property UserDao() As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Private ReadOnly Property UIBuildingBlocks As List(Of IPHFormUIBuildingBlock)
            Get
                If (_uiBuldingBlocks Is Nothing) Then
                    _uiBuldingBlocks = New List(Of IPHFormUIBuildingBlock)()
                End If

                Return _uiBuldingBlocks
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ' Add controls dynamically...
            BuildDynamicUI()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + Page.ResolveClientUrl(Page.Request.ApplicationPath) & "/Styles/PHPrintableForm.css")

            If (Not Page.IsPostBack) Then
                PopulateHeaderFields()
                SetKeyColors()
                PopulateDynamicUIFields()
            End If
        End Sub

        Private Sub PopulateDynamicUIFields()
            Dim currentValue As PHFormValue = Nothing

            For Each b As IPHFormUIBuildingBlock In UIBuildingBlocks
                b.Enabled = True
                b.IsReadOnly = True
                b.ScreenReaderControlEnabled = False

                Dim parts() As String = b.ControlId.Split("_")

                If (parts.Count <> 4) Then
                    Continue For
                End If

                currentValue = CasePHForm.GetValue(Integer.Parse(parts(1)), Integer.Parse(parts(2)), Integer.Parse(parts(3)))

                If (currentValue Is Nothing) Then
                    Continue For
                End If

                If (String.IsNullOrEmpty(currentValue.RawValue)) Then
                    Continue For
                End If

                b.PrimaryValue = currentValue.RawValue
            Next

            UIBuildingBlocks.Clear()
        End Sub

        Private Sub PopulateHeaderFields()
            lblDPHName.Text = SpecCase.DPHUser.FullName
            lblWingName.Text = SpecCase.DPHUser.CurrentUnitName
            lblReportingPeriod.Text = SpecCase.ReportingPeriod.Value.ToString("y").Replace(",", String.Empty)

            If (SpecCase.IsDelinquent.HasValue AndAlso SpecCase.IsDelinquent.Value) Then
                lblDelinquent.Text = "YES"
            Else
                lblDelinquent.Text = "NO"
            End If
        End Sub

#Region "Dynamic UI Generation..."

        Private Sub BuildDynamicUI()
            Dim topLevelSections As List(Of PHSection) = CasePHForm.GetTopLevelSections()

            For Each s As PHSection In topLevelSections
                pnlPHForm.Controls.Add(PHUtility.CreateSectionHeader(s))

                For Each c As PHSection In s.Children
                    RecursiveProcessChildren(c, pnlPHForm)
                Next

                ' Check if this section has any FormFields to display...if not then don't display panel
                If (CasePHForm.DoFieldsExistForSection(s.Id)) Then
                    Dim innerPanel = PHUtility.CreateSectionInnerFieldset(s) 'PHUtility.CreateSectionInnerPanel(s)
                    BuildFields(s, innerPanel)
                    pnlPHForm.Controls.Add(innerPanel)
                End If
            Next
        End Sub

        Private Sub BuildDynamicUI_OLD()
            Dim topLevelSections As List(Of PHSection) = CasePHForm.GetTopLevelSections()
            'Dim isFirst As Boolean = True

            For Each s As PHSection In topLevelSections
                Dim sectionPanel As Panel = PHUtility.CreateSectionPanel(s)

                For Each c As PHSection In s.Children
                    RecursiveProcessChildren(c, sectionPanel)
                Next

                ' Check if this section has any FormFields to display...if not then don't display panel
                If (CasePHForm.DoFieldsExistForSection(s.Id)) Then
                    Dim innerPanel = PHUtility.CreateSectionInnerPanel(s)
                    BuildFields(s, innerPanel)
                    sectionPanel.Controls.Add(innerPanel)
                End If

                'If (Not isFirst) Then
                '    sectionPanel.CssClass = sectionPanel.CssClass & " mainSectionBreak"
                'Else
                '    isFirst = False
                'End If
                'If (s.Name = "Deaths/Near Deaths" OrElse s.Name = "RMU Interaction") Then
                '    sectionPanel.CssClass = sectionPanel.CssClass & " mainSectionBreak"
                'End If

                pnlPHForm.Controls.Add(sectionPanel)
            Next
        End Sub

        Private Sub BuildFieldGroupsInOneColumn(ByVal fieldPanels As List(Of Panel), ByVal sectionControl As Control)
            Dim fieldGroup1Panel As Panel = PHUtility.CreateFieldGroupPanel(1, String.Empty)

            For i As Integer = 0 To (fieldPanels.Count - 1)
                fieldGroup1Panel.Controls.Add(fieldPanels(i))
            Next

            sectionControl.Controls.Add(fieldGroup1Panel)
        End Sub

        Private Sub BuildFieldGroupsInThreeColumns(ByVal fieldPanels As List(Of Panel), ByVal sectionControl As Control)
            Dim fieldGroup1Panel As Panel = PHUtility.CreateFieldGroupPanel(3, "L")
            Dim fieldGroup2Panel As Panel = PHUtility.CreateFieldGroupPanel(3, "C")
            Dim fieldGroup3Panel As Panel = PHUtility.CreateFieldGroupPanel(3, "R")
            Dim oneThirds As Integer = 0

            oneThirds = Math.Ceiling(fieldPanels.Count / 3)

            Dim twoThirds As Integer = oneThirds * 2 '(fieldPanels.Count * 2) / 3

            For i As Integer = 0 To (fieldPanels.Count - 1)
                If (i < oneThirds) Then
                    fieldGroup1Panel.Controls.Add(fieldPanels(i))
                ElseIf (i >= oneThirds AndAlso i < twoThirds) Then
                    fieldGroup2Panel.Controls.Add(fieldPanels(i))
                Else
                    fieldGroup3Panel.Controls.Add(fieldPanels(i))
                End If
            Next

            sectionControl.Controls.Add(fieldGroup1Panel)
            sectionControl.Controls.Add(fieldGroup2Panel)
            sectionControl.Controls.Add(fieldGroup3Panel)
        End Sub

        Private Sub BuildFieldGroupsInTwoColumns(ByVal fieldPanels As List(Of Panel), ByVal sectionControl As Control)
            Dim fieldGroup1Panel As Panel = PHUtility.CreateFieldGroupPanel(2, "L")
            Dim fieldGroup2Panel As Panel = PHUtility.CreateFieldGroupPanel(2, "R")
            Dim half As Integer = 0

            half = Math.Ceiling(fieldPanels.Count / 2)

            For i As Integer = 0 To (fieldPanels.Count - 1)
                If (i < half) Then
                    fieldGroup1Panel.Controls.Add(fieldPanels(i))
                Else
                    fieldGroup2Panel.Controls.Add(fieldPanels(i))
                End If
            Next

            sectionControl.Controls.Add(fieldGroup1Panel)
            sectionControl.Controls.Add(fieldGroup2Panel)
        End Sub

        Private Sub BuildFields(ByVal section As PHSection, ByVal sectionControl As Control)
            Dim formFields As ReadOnlyCollection(Of PHFormField) = CasePHForm.GetFieldsBySection(section.Id)
            Dim fieldPanels As List(Of Panel) = New List(Of Panel)()
            Dim currentFieldComponentsPanel As Panel = Nothing
            Dim currentFieldId As Integer = 0

            For Each f As PHFormField In formFields
                If (f.Field.Id <> currentFieldId) Then
                    Dim fieldPanel = PHUtility.CreateFieldPanel()
                    Dim isEmptyFieldLabel As Boolean = True

                    ' Check if a label and label panel need to be created...
                    If (Not f.Field.Name.Equals(PHUtility.EMPTY_FIELD_NAME)) Then
                        Dim labelPanel = PHUtility.CreateFieldLabelPanel(section.FieldColumns)
                        labelPanel.Controls.Add(PHUtility.CreateFieldLabel(f.Field.Name))
                        fieldPanel.Controls.Add(labelPanel)
                        isEmptyFieldLabel = False
                    End If

                    currentFieldComponentsPanel = PHUtility.CreateFieldComponentsPanel(section.FieldColumns, isEmptyFieldLabel)

                    fieldPanel.Controls.Add(currentFieldComponentsPanel)
                    fieldPanels.Add(fieldPanel)
                    currentFieldId = f.Field.Id
                End If

                If (currentFieldComponentsPanel IsNot Nothing) Then
                    currentFieldComponentsPanel.Controls.Add(GetFieldControl(f))
                End If
            Next

            If (section.FieldColumns = 1) Then
                BuildFieldGroupsInOneColumn(fieldPanels, sectionControl)
            ElseIf (section.FieldColumns = 2) Then
                BuildFieldGroupsInTwoColumns(fieldPanels, sectionControl)
            Else
                BuildFieldGroupsInThreeColumns(fieldPanels, sectionControl)
            End If
        End Sub

        Private Function GetFieldControl(ByVal formField As PHFormField) As Control
            Dim controlId As String = PHUtility.GenerateFieldControlId(formField.Section.Id, formField.Field.Id, formField.FieldType.Id)
            Dim control As UserControl = Nothing
            Dim cssClass As String = String.Empty

            Select Case formField.FieldType.DataTypeId
                Case DataType.DT_Integer
                    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHIntegerBlock.ascx"), PHIntegerBlock)
                    cssClass = PHUtility.CSS_FIELD_NUMBERSTEXT_BOX

                Case DataType.DT_Option
                    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHOptionBlock.ascx"), PHOptionBlock)
                    cssClass = PHUtility.CSS_FIELD_OPTIONS_BOX

                Case DataType.DT_MultiOption
                    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHMultipleOptionsBlock.ascx"), PHMultipleOptionsBlock)
                    cssClass = PHUtility.CSS_FIELD_MULTIPLEOPTIONS_BOX

                Case DataType.DT_String
                    If (formField.FieldType.Name.Contains("Small")) Then
                        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHSmallTextBlock.ascx"), PHSmallTextBlock)
                        cssClass = PHUtility.CSS_FIELD_SMALLTEXT_BOX
                    ElseIf (formField.FieldType.Name.Contains("Large")) Then
                        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHLargeTextBlock.ascx"), PHLargeTextBlock)
                        cssClass = PHUtility.CSS_FIELD_LARGETEXT_BOX
                    ElseIf (formField.FieldType.Name.Contains("Giant")) Then
                        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHGiantTextBlock.ascx"), PHGiantTextBlock)
                        cssClass = PHUtility.CSS_FIELD_GIANTTEXT_BOX
                    End If

                Case DataType.DT_CSV
                    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHCSVTextBlock.ascx"), PHCSVTextBlock)
                    cssClass = PHUtility.CSS_FIELD_CSV_BOX

                Case Else
                    Return PHUtility.CreateFieldLabel("UNKNOWN")
            End Select

            If (control Is Nothing) Then
                Return PHUtility.CreateFieldLabel("UNKNOWN")
            End If

            control.ID = controlId
            PHUtility.ConfigureUserControl(formField, control, cssClass)
            CType(control, IPHFormUIBuildingBlock).Placeholder = String.Empty

            UIBuildingBlocks.Add(CType(control, IPHFormUIBuildingBlock))

            Return control
        End Function

        Private Sub RecursiveProcessChildren(ByVal section As PHSection, ByVal parentPanel As Panel)
            Dim sectionPanel As Panel = PHUtility.CreateSectionPanel(section)
            Dim innerPanel = Nothing

            For Each c As PHSection In section.Children
                RecursiveProcessChildren(c, sectionPanel)
            Next

            If (section.Children.Count > 0) Then
                innerPanel = PHUtility.CreateSectionInnerPanel(section)
                BuildFields(section, innerPanel)
                sectionPanel.Controls.Add(innerPanel)
            Else
                BuildFields(section, sectionPanel)
            End If

            ' Check if this section has any FormFields to display...if not then don't display panel
            If (section.Children.Count > 0 OrElse CasePHForm.DoFieldsExistForSection(section.Id)) Then
                'If (section.Name = "Deaths/Near Deaths" OrElse section.Name = "Marital Status") Then
                '    sectionPanel.CssClass = sectionPanel.CssClass & " mainSectionBreak"
                'End If

                parentPanel.Controls.Add(sectionPanel)
            End If
        End Sub

        Private Sub SetKeyColors()
            Dim fieldTypes As IList(Of PHFieldType) = PHDao.GetAllFieldTypes()

            txtFrequencyKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Frequency")).First.Color.Value
            txtMembersSeenKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Members Seen")).First.Color.Value
            txtFollowUpKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Follow-Ups")).First.Color.Value
            txtArmyKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Army")).First.Color.Value
            txtNavyKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Navy")).First.Color.Value
            txtAirForceKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Air Force")).First.Color.Value
            txtCoastGuardKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Coast Guard")).First.Color.Value
            txtMarineCorpsKey.BackColor = fieldTypes.Where(Function(x) x.Name.Equals("Marine Corps")).First.Color.Value
        End Sub

#End Region

    End Class

End Namespace