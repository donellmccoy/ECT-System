Imports System.Collections.ObjectModel
Imports AjaxControlToolkit
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALOD.Web.UserControls.UIBuildingBlocks
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PH

    Public Class Form
        Inherits System.Web.UI.Page

        Private _phDao As IPsychologicalHealthDao
        Private _phForm As PHForm
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _uiBuldingBlocks As List(Of IPHFormUIBuildingBlock)
        Private _userDao As IUserDao
        Private _validationErrors As List(Of Integer)
        Private dao As ISpecialCaseDAO
        Private sc As SC_PH = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property CasePHForm As PHForm
            Get
                If (_phForm Is Nothing) Then
                    _phForm = New PHForm(ReferenceId, PHDao)
                End If

                Return _phForm
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
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

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property MasterPage() As SC_PHMaster
            Get
                Dim master As SC_PHMaster = CType(Page.Master, SC_PHMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PH
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
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

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

        Private ReadOnly Property ValidationErrors As List(Of Integer)
            Get
                If (_validationErrors Is Nothing) Then
                    _validationErrors = New List(Of Integer)()
                End If

                Return _validationErrors
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked

            'SetKeyColors()

            ' Add controls dynamically...
            BuildDynamicUI()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/PHForm.css")

            SetKeyColors()

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)

                'Validation
                If (UserCanEdit) Then
                    SpecCase.Validate()
                    If (SpecCase.Validations.Count > 0) Then
                        ShowPageValidationErrors(SpecCase.Validations, Me)
                    End If
                End If

                ' Set form header and key information...
                If (SpecCase.ReportingPeriod.HasValue) Then
                    lblReportingPeriod.Text = SpecCase.ReportingPeriod.Value.ToString("y").Replace(",", String.Empty)
                Else
                    lblReportingPeriod.Text = "UNKNOWN"
                End If

                If (SpecCase.FormLastModified.HasValue) Then
                    lblLastModified.Text = SpecCase.FormLastModified.Value.ToString("m") & ", " & SpecCase.FormLastModified.Value.Year.ToString()
                ElseIf (SpecCase.ModifiedDate.HasValue) Then
                    lblLastModified.Text = SpecCase.ModifiedDate.Value.ToString("m") & ", " & SpecCase.ModifiedDate.Value.Year.ToString()
                Else
                    lblLastModified.Text = "UNKNOWN"
                End If

                ' Set the initial values of each control...
                PopulateDynamicUIFields()

                LogManager.LogAction(ModuleType.SpecCasePH, UserAction.ViewPage, RefId, "Viewed Page: PH Form")
            End If
        End Sub

        Private Sub PopulateDynamicUIFields()
            Dim currentValue As PHFormValue = Nothing

            For Each b As IPHFormUIBuildingBlock In UIBuildingBlocks
                b.Enabled = UserCanEdit
                b.IsReadOnly = (Not UserCanEdit)

                UpdateUIBuildingBlockConfiguration(b)

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

        Private Sub RecurseThroughControlHierarchy(ByVal c As Control)
            'Do whatever it is you need to do with the current control, c
            If (Not String.IsNullOrEmpty(c.ID) AndAlso c.ID.Length > 9 AndAlso c.ID.Substring(0, 10).Equals("formfield_")) Then
                Dim parts() As String = c.ID.Split("_")

                If (parts.Count = 4) Then
                    Dim buildingBlock As IPHFormUIBuildingBlock = CType(c, IPHFormUIBuildingBlock)

                    If (buildingBlock.ValidateInput()) Then
                        Dim rawValue As String = buildingBlock.PrimaryValue
                        Dim formValue As New PHFormValue(ReferenceId, Integer.Parse(parts(1)), Integer.Parse(parts(2)), Integer.Parse(parts(3)), rawValue)
                        CasePHForm.UpdateValue(formValue)
                    Else
                        Dim sectionId As Integer = Integer.Parse(parts(1))
                        If (Not ValidationErrors.Contains(sectionId)) Then
                            ValidationErrors.Add(sectionId)
                        End If
                    End If
                End If
            End If

            'Recurse through c's child controls
            For Each child As Control In c.Controls
                RecurseThroughControlHierarchy(child)
            Next
        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (SpecCase.WorkflowStatus.StatusCodeType.IsFinal) Then
                Exit Sub
            End If

            For Each aacp As AccordionPane In accPHForm.Panes
                For Each c As Control In aacp.Controls
                    RecurseThroughControlHierarchy(c)
                Next
            Next

            bllValidationList.Items.Clear()
            pnlValidationErrors.Visible = False

            If (ValidationErrors.Count > 0) Then
                pnlValidationErrors.Visible = True

                Dim currentParentSection As PHSection = Nothing
                Dim currentListItem As ListItem = Nothing
                For Each sectionId As Integer In ValidationErrors
                    currentParentSection = CasePHForm.GetTopLevelSectionForChildSection(PHDao.GetSectionById(sectionId))

                    If (currentParentSection Is Nothing) Then
                        Continue For
                    End If

                    currentListItem = New ListItem(currentParentSection.Name, currentParentSection.Id)

                    If (Not bllValidationList.Items.Contains(currentListItem)) Then
                        bllValidationList.Items.Add(currentListItem)
                    End If
                Next
            End If

            SpecCase.FormLastModified = DateTime.Now

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

        Private Sub UpdateUIBuildingBlockConfiguration(ByVal buildingBlock As IPHFormUIBuildingBlock)
            ' Update the configuration of the UI Building Block controls based on the Read/Write access of the user...

            ' Update CSVTextBlock controls...
            If (TypeOf buildingBlock Is PHCSVTextBlock) Then
                If (UserCanEdit) Then
                    buildingBlock.CssClass = PHUtility.CSS_FIELD_CSV_BOX
                Else
                    buildingBlock.CssClass = PHUtility.CSS_FIELD_CSV_LABEL
                End If
            End If

            ' Update PHIntegerBlock controls...
            If (TypeOf buildingBlock Is PHIntegerBlock) Then
                If (Not UserCanEdit) Then
                    buildingBlock.ForeColor = Drawing.Color.Black
                End If
            End If
        End Sub

#Region "Dynamic UI Generation..."

        Private Sub BuildDynamicUI()
            Dim topLevelSections As List(Of PHSection) = CasePHForm.GetTopLevelSections()

            For Each s As PHSection In topLevelSections
                Dim accPane As New AccordionPane()
                accPane.ID = "accp_PHSection_" + s.Id.ToString()
                accPane.HeaderContainer.Controls.Add(PHUtility.CreateSectionHeaderLink(s))
                accPane.HeaderContainer.ToolTip = "Click to expand..."

                For Each c As PHSection In s.Children
                    RecursiveProcessChildren(c, accPane.ContentContainer)
                Next

                ' Check if this section has any FormFields to display...if not then don't display panel
                If (CasePHForm.DoFieldsExistForSection(s.Id)) Then
                    Dim innerPanel = PHUtility.CreateSectionInnerPanel(s)
                    BuildFields(s, innerPanel)
                    accPane.ContentContainer.Controls.Add(innerPanel)
                End If

                accPHForm.Panes.Add(accPane)
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
                    If (UserCanEdit) Then
                        cssClass = PHUtility.CSS_FIELD_CSV_BOX
                    Else
                        cssClass = PHUtility.CSS_FIELD_CSV_LABEL
                    End If

                Case Else
                    Return PHUtility.CreateFieldLabel("UNKNOWN")
            End Select

            If (control Is Nothing) Then
                Return PHUtility.CreateFieldLabel("UNKNOWN")
            End If

            control.ID = controlId
            PHUtility.ConfigureUserControl(formField, control, cssClass)

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

#Region "TabEvent"

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If
        End Sub

#End Region

    End Class

End Namespace