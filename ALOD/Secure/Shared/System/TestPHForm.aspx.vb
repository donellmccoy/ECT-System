Imports System.Collections.ObjectModel
Imports AjaxControlToolkit
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Web.UserControls.UIBuildingBlocks
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class TestPHForm
        Inherits System.Web.UI.Page

        Private _phDao As IPsychologicalHealthDao
        Private _phForm As PHForm

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

        Public ReadOnly Property PlaceHolderControl As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                'Return Integer.Parse(Request.QueryString("refId"))
                Return 999999
            End Get
        End Property

        Protected Sub btnTestSubmit_Click(sender As Object, e As EventArgs) 'Handles btnTestSubmit.Click
            ' METHOD 1 - Might be more efficient because we only traverse the control hierarchy onces and getting the individual PHFormValues is a O(1) operation...
            'For Each aacp As AccordionPane In accPHForm.Panes
            '    For Each c As Control In aacp.Controls
            '        RecurseThroughControlHierarchy(c)
            '    Next
            'Next

            ' METHOD 2 - Might be less efficient because we are doing multiple traversals fo the control hierarchy via the FindControl function...
            Dim currentValue As PHFormValue = Nothing
            Dim currentControlId As String = Nothing
            Dim newRawValue As String = Nothing

            For Each ff As PHFormField In CasePHForm.Fields
                currentValue = CasePHForm.GetValue(ff.Section.Id, ff.Field.Id, ff.FieldType.Id)
                currentControlId = PHUtility.GenerateFieldControlId(ff.Section.Id, ff.Field.Id, ff.FieldType.Id)

                If (ff.FieldType.Id = 1 OrElse ff.FieldType.Id = 2 OrElse ff.FieldType.Id = 3 OrElse ff.FieldType.Id = 5) Then
                    newRawValue = CType(accPHForm.FindControl(currentControlId), TextBox).Text
                End If

                If (currentValue Is Nothing) Then
                    If (Not String.IsNullOrEmpty(newRawValue)) Then
                        'CasePHForm.InsertValue(New PHFormValue(CasePHForm.RefId, ff.Section.Id, ff.Field.Id, ff.FieldType.Id, newRawValue))
                    End If

                    Continue For
                End If

                ' WE know that currentValue IS NOT NULL
                If (String.IsNullOrEmpty(newRawValue)) Then
                    ' Remove/Delete the value becuase the value exist but it is being set to an empty value
                    Continue For
                End If

                If (Not currentValue.RawValue.Equals(newRawValue)) Then
                    'CasePHForm.UpdateValue(New PHFormValue(CasePHForm.RefId, ff.Section.Id, ff.Field.Id, ff.FieldType.Id, newRawValue))
                    Continue For
                End If
            Next
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ' Add controls dynamically...
            BuildDynamicUI()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'AddStyleSheet(Page, "~/Styles/PHForm.css")
            AddStyleSheet(Page, Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host + Page.ResolveClientUrl(Page.Request.ApplicationPath) & "/Styles/PHForm.css")

            If (Not Page.IsPostBack) Then
                ' Set the initial values of each control...
                PopulatedDynamicUIFields()
            End If
        End Sub

        Private Sub BuildDynamicUI()
            Dim topLevelSections As List(Of PHSection) = CasePHForm.GetTopLevelSections()

            For Each s As PHSection In topLevelSections
                Dim accPane As New AccordionPane()
                accPane.ID = "accp_PHSection_" + s.Id.ToString()
                accPane.HeaderContainer.Controls.Add(New LiteralControl(s.Name))

                For Each c As PHSection In s.Children
                    RecursiveProcessChildren(c, accPane.ContentContainer)
                Next

                ' TO DO: Check if this section has any FormFields to display...if not then don't display panel
                If (CasePHForm.DoFieldsExistForSection(s.Id)) Then
                    Dim innerPanel = PHUtility.CreateSectionInnerPanel(s)
                    BuildFields_v2(s, innerPanel)
                    accPane.ContentContainer.Controls.Add(innerPanel)
                End If

                accPHForm.Panes.Add(accPane)
            Next

        End Sub

        Private Sub BuildFieldGroupsInOneColumn(ByVal fieldPanels As List(Of Panel), ByVal sectionPanel As Panel)
            Dim fieldGroup1Panel As Panel = PHUtility.CreateFieldGroupPanel(1, String.Empty)

            For i As Integer = 0 To (fieldPanels.Count - 1)
                fieldGroup1Panel.Controls.Add(fieldPanels(i))
            Next

            sectionPanel.Controls.Add(fieldGroup1Panel)
        End Sub

        Private Sub BuildFieldGroupsInThreeColumns(ByVal fieldPanels As List(Of Panel), ByVal sectionPanel As Panel)
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

            sectionPanel.Controls.Add(fieldGroup1Panel)
            sectionPanel.Controls.Add(fieldGroup2Panel)
            sectionPanel.Controls.Add(fieldGroup3Panel)
        End Sub

        Private Sub BuildFieldGroupsInTwoColumns(ByVal fieldPanels As List(Of Panel), ByVal sectionPanel As Panel)
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

            sectionPanel.Controls.Add(fieldGroup1Panel)
            sectionPanel.Controls.Add(fieldGroup2Panel)
        End Sub

        Private Sub BuildFields_v2(ByVal section As PHSection, ByVal sectionPanel As Panel)

            Dim formFields As ReadOnlyCollection(Of PHFormField) = CasePHForm.GetFieldsBySection(section.Id)
            Dim fieldPanels As List(Of Panel) = New List(Of Panel)()
            'Dim currentFieldPanel As Panel = Nothing
            Dim currentFieldComponentsPanel As Panel = Nothing
            Dim currentFieldId As Integer = 0

            For Each f As PHFormField In formFields
                If (f.Field.Id <> currentFieldId) Then
                    Dim fieldPanel = PHUtility.CreateFieldPanel()
                    Dim isEmptyFieldLabel As Boolean = True

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
                    currentFieldComponentsPanel.Controls.Add(GetFieldControl_V2(f))
                End If
            Next

            If (section.FieldColumns = 1) Then
                BuildFieldGroupsInOneColumn(fieldPanels, sectionPanel)
            ElseIf (section.FieldColumns = 2) Then
                BuildFieldGroupsInTwoColumns(fieldPanels, sectionPanel)
            Else
                BuildFieldGroupsInThreeColumns(fieldPanels, sectionPanel)
            End If
        End Sub

        Private Function GetFieldControl_V2(ByVal formField As PHFormField) As Control
            Dim controlId As String = PHUtility.GenerateFieldControlId(formField.Section.Id, formField.Field.Id, formField.FieldType.Id)
            Dim control As UserControl = Nothing
            Dim cssClass As String = String.Empty

            ' TO DO: Find a better way of referencing the different field types...enums if necessary

            'Select Case formField.FieldType.Id
            '    Case 1, 2, 3, 6, 7, 8, 9, 10
            '        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHIntegerBlock.ascx"), PHIntegerBlock)
            '        cssClass = "fieldNumbersTextbox"

            '    Case 4
            '        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHOptionBlock.ascx"), PHOptionBlock)
            '        cssClass = "fieldOptionsBox"

            '    Case 5
            '        control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHSmallTextBlock.ascx"), PHSmallTextBlock)
            '        cssClass = "fieldSmallTextbox"

            '    Case Else
            '        Return PHUtility.CreateFieldLabel("UNKNOWN")
            'End Select

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

                'Case DataType.DT_String_50
                '    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHSmallTextBlock.ascx"), PHSmallTextBlock)
                '    cssClass = PHUtility.CSS_FIELD_SMALLTEXT_BOX

                'Case DataType.DT_String_255
                '    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHLargeTextBlock.ascx"), PHLargeTextBlock)
                '    cssClass = PHUtility.CSS_FIELD_LARGETEXT_BOX

                'Case DataType.DT_String_500
                '    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHGiantTextBlock.ascx"), PHGiantTextBlock)
                '    cssClass = PHUtility.CSS_FIELD_GIANTTEXT_BOX

                Case DataType.DT_CSV
                    control = CType(LoadControl("~/Secure/Shared/UserControls/UIBuildingBlocks/PHCSVTextBlock.ascx"), PHCSVTextBlock)
                    cssClass = PHUtility.CSS_FIELD_CSV_BOX

                Case Else
                    Return PHUtility.CreateFieldLabel("UNKNOWN")
            End Select

            control.ID = controlId
            PHUtility.ConfigureUserControl(formField, control, cssClass)

            Return control

        End Function

        Private Sub PopulatedDynamicUIFields()

            ' METHOD 1 - Recursive version - See btnTestSubmit_Click METHOD 1

            ' METHOD 2
            Dim currentControl As Control = Nothing

            'For Each fv As PHFormValue In CasePHForm.Values
            '    currentControl = accPHForm.FindControl(GenerateFieldControlId(fv.SectionId, fv.FieldId, fv.FieldTypeId))
            'Next

            For Each fv As PHFormValue In PHUtility.GetTestFormValues()
                currentControl = accPHForm.FindControl(PHUtility.GenerateFieldControlId(fv.SectionId, fv.FieldId, fv.FieldTypeId))

                If (currentControl Is Nothing) Then
                    Continue For
                End If

                CType(currentControl, IPHFormUIBuildingBlock).PrimaryValue = fv.RawValue

                ' TO DO: This is uncecessary if the user controls implement a common interface...
                'If (fv.FieldTypeId = 1 OrElse fv.FieldTypeId = 2 OrElse fv.FieldTypeId = 3) Then
                '    CType(currentControl, PHIntegerBlock).Value = fv.RawValue
                'ElseIf (fv.FieldTypeId = 4) Then
                '    CType(currentControl, PHOptionBlock).Value = fv.RawValue
                'ElseIf (fv.FieldTypeId = 5) Then
                '    CType(currentControl, PHShortTextBlock).Value = fv.RawValue
                'End If
            Next
        End Sub

        Private Sub RecurseThroughControlHierarchy(ByVal c As Control)
            'Do whatever it is you need to do with the current control, c
            If (Not String.IsNullOrEmpty(c.ID) AndAlso c.ID.Substring(0, 10).Equals("formfield_")) Then
                Dim rawValue As String = String.Empty
                Dim parts() As String = c.ID.Split("_")

                If (parts.Count = 4) Then

                    Dim fieldType As Integer = Integer.Parse(parts(3))

                    If (fieldType = 1 OrElse fieldType = 2 OrElse fieldType = 3 OrElse fieldType = 5) Then
                        'CType(accPHForm.FindControl(PHUtility.GenerateFieldControlId(FV.SectionId, FV.FieldId, FV.FieldTypeId)), TextBox).Text = FV.RawValue
                        rawValue = CType(c, TextBox).Text
                    End If

                    Dim formValue As New PHFormValue(0, Integer.Parse(parts(1)), Integer.Parse(parts(2)), Integer.Parse(parts(3)), rawValue)
                    'CasePHForm.UpdateValue(formValue)
                End If
            End If

            'Recurse through c's children controls
            For Each child As Control In c.Controls
                RecurseThroughControlHierarchy(child)
            Next
        End Sub

        Private Sub RecursiveProcessChildren(ByVal section As PHSection, ByVal parentPanel As Panel)
            Dim sectionPanel As Panel = PHUtility.CreateSectionPanel(section)
            Dim innerPanel = Nothing

            For Each c As PHSection In section.Children
                RecursiveProcessChildren(c, sectionPanel)
            Next

            ' TO DO: Check if this section has any FormFields to display...if not then don't display panel

            If (section.Children.Count > 0) Then
                innerPanel = PHUtility.CreateSectionInnerPanel(section)
                BuildFields_v2(section, innerPanel)
                sectionPanel.Controls.Add(innerPanel)
            Else
                BuildFields_v2(section, sectionPanel)
            End If

            If (section.Children.Count > 0 OrElse CasePHForm.DoFieldsExistForSection(section.Id)) Then
                parentPanel.Controls.Add(sectionPanel)
            End If
        End Sub

    End Class

End Namespace