Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces

Namespace Common
    Public Module PHUtility
        Public Const CSS_FIELD_CSV_BOX As String = "fieldCSVTextbox"
        Public Const CSS_FIELD_CSV_LABEL As String = "fieldCSVLabel"
        Public Const CSS_FIELD_GIANTTEXT_BOX As String = "fieldGiantTextbox textLimit"
        Public Const CSS_FIELD_LARGETEXT_BOX As String = "fieldLargeTextbox"
        Public Const CSS_FIELD_MULTIPLEOPTIONS_BOX As String = "fieldMultipleOptionsBox"
        Public Const CSS_FIELD_NUMBERSTEXT_BOX As String = "fieldNumbersTextbox"
        Public Const CSS_FIELD_OPTIONS_BOX As String = "fieldOptionsBox"
        Public Const CSS_FIELD_SMALLTEXT_BOX As String = "fieldSmallTextbox"
        Public Const CSS_FIELD_VALIDATION_ERROR As String = "fieldValidationError"
        Public Const EMPTY_FIELD_NAME As String = "{EMPTY}"

        Public Sub ConfigureUserControl(ByVal formField As PHFormField, ByVal userControl As UserControl, ByVal cssClass As String)
            Dim uc As IPHFormUIBuildingBlock = CType(userControl, IPHFormUIBuildingBlock)

            uc.CssClass = cssClass
            uc.Placeholder = formField.FieldType.Placeholder
            uc.DataSource = formField.FieldType.Datasource
            uc.ToolTip = formField.ToolTip

            If (Not formField.Field.Name.Equals(EMPTY_FIELD_NAME)) Then
                uc.ScreenReaderText = formField.Field.Name & " " & formField.FieldType.Name
            Else
                uc.ScreenReaderText = formField.FieldType.Name
            End If

            If (formField.FieldType.Color.HasValue) Then
                uc.BackColor = formField.FieldType.Color.Value
            End If

            If (formField.FieldType.Length.HasValue) Then
                uc.MaxLength = formField.FieldType.Length
            End If
        End Sub

        Public Function CreateFieldComponentsPanel(ByVal columns As Integer, ByVal isEmptyFieldLabel As Boolean) As Panel
            Dim p As New Panel()

            If (isEmptyFieldLabel) Then
                p.CssClass = "fieldComponentsPanel-NoLabel"
            ElseIf (columns = 1) Then
                p.CssClass = "fieldComponentsPanel-1"
            ElseIf (columns = 2) Then
                p.CssClass = "fieldComponentsPanel-2"
            Else
                p.CssClass = "fieldComponentsPanel-3"
            End If

            Return p
        End Function

        Public Function CreateFieldGroupPanel(ByVal isLeft As Boolean) As Panel
            Dim p As New Panel()

            If (isLeft) Then
                p.CssClass = "fieldGroupPanelLeft"
            Else
                p.CssClass = "fieldGroupPanelRight"
            End If

            Return p
        End Function

        Public Function CreateFieldGroupPanel(ByVal c As Integer, ByVal h As String) As Panel
            Dim p As New Panel()

            If (c = 1) Then
                p.CssClass = "fieldGroupPanel-1"
            ElseIf (c = 2) Then
                If (h.Equals("L")) Then
                    p.CssClass = "fieldGroupPanelLeft-2"
                ElseIf (h.Equals("R")) Then
                    p.CssClass = "fieldGroupPanelRight-2"
                End If
            Else
                If (h.Equals("L")) Then
                    p.CssClass = "fieldGroupPanelLeft-3"
                ElseIf (h.Equals("R")) Then
                    p.CssClass = "fieldGroupPanelRight-3"
                ElseIf (h.Equals("C")) Then
                    p.CssClass = "fieldGroupPanelCenter-3"
                End If
            End If

            Return p
        End Function

        Public Function CreateFieldLabel(ByVal text As String) As Label
            Dim lbl As New Label()

            lbl.Text = text
            lbl.CssClass = "fieldLabel"

            Return lbl
        End Function

        Public Function CreateFieldLabelPanel(ByVal columns As Integer) As Panel
            Dim p As New Panel()

            If (columns = 1) Then
                p.CssClass = "fieldLabelPanel-1"
            ElseIf (columns = 2) Then
                p.CssClass = "fieldLabelPanel-2"
            Else
                p.CssClass = "fieldLabelPanel-3"
            End If

            Return p
        End Function

        Public Function CreateFieldPanel() As Panel
            Dim p As New Panel()

            p.CssClass = "fieldPanel"

            Return p
        End Function

        Public Function CreateSectionHeader(ByVal section As PHSection) As HtmlGenericControl
            Dim h As New HtmlGenericControl("h2")

            h.InnerHtml = section.Name

            If (section.HasPageBreak) Then
                h.Attributes.Add("class", "sectionHeader mainSectionBreak")
            Else
                h.Attributes.Add("class", "sectionHeader")
            End If

            Return h
        End Function

        Public Function CreateSectionHeaderLink(ByVal section As PHSection) As HyperLink
            Dim l As New HyperLink

            l.Text = section.Name
            l.NavigateUrl = "#"
            l.Attributes.Add("onclick", "return false;")

            Return l
        End Function

        Public Function CreateSectionInnerFieldset(ByVal section As PHSection) As HtmlGenericControl
            Dim f As New HtmlGenericControl("fieldset")

            f.ID = "pnlSection_Inner_" & section.Id
            f.Attributes.Add("class", "sectionPanelNoTitle")

            Return f
        End Function

        Public Function CreateSectionInnerPanel(ByVal section As PHSection) As Panel
            Dim p As New Panel()

            p.ID = "pnlSection_Inner_" & section.Id
            p.CssClass = "sectionPanelNoTitle"

            Return p
        End Function

        Public Function CreateSectionPanel(ByVal section As PHSection) As Panel
            Dim p As New Panel()

            p.ID = "pnlSection_" & section.Id
            p.CssClass = "sectionPanel"
            p.GroupingText = "<label>" + section.Name + "</label>"

            Return p
        End Function

        Public Function CreateSectionPanel_v2(ByVal section As PHSection) As Panel
            Dim headerPanel As New Panel()
            Dim innerPanel As New Panel()

            headerPanel.ID = "pnlSection_" & section.Id
            headerPanel.CssClass = "sectionPanel"
            headerPanel.GroupingText = section.Name
            headerPanel.Font.Bold = True

            Return headerPanel
        End Function

        Public Function GenerateFieldControlId(ByVal sectionId As Integer, ByVal fieldId As Integer, ByVal fieldTypeId As Integer) As String
            Dim controlId As String = "formfield_" + sectionId.ToString() + "_" + fieldId.ToString() + "_" + fieldTypeId.ToString()

            Return controlId
        End Function

        Public Function GetTestFormValues() As IList(Of PHFormValue)
            Dim testValues As New List(Of PHFormValue)()

            ' Human Performance Improvement/Outreach
            testValues.Add(New PHFormValue(0, 1, 21, 1, 85))

            ' Walkabout/Unit Visits
            testValues.Add(New PHFormValue(0, 6, 1, 1, 5))
            testValues.Add(New PHFormValue(0, 6, 1, 2, 15))

            testValues.Add(New PHFormValue(0, 6, 2, 1, 55))
            testValues.Add(New PHFormValue(0, 6, 2, 3, 515))

            testValues.Add(New PHFormValue(0, 6, 133, 1, 75))
            testValues.Add(New PHFormValue(0, 6, 133, 1, 715))
            testValues.Add(New PHFormValue(0, 6, 133, 5, "Test Value"))

            ' Abuse
            testValues.Add(New PHFormValue(0, 10, 29, 1, 1313))

            ' Suidice Method
            testValues.Add(New PHFormValue(0, 8, 49, 4, "2,4,1,5,3"))

            testValues.Add(New PHFormValue(0, 3, 92, 14, "www.google.com,www.stanfieldsystems.com"))

            testValues.Add(New PHFormValue(0, 17, 140, 12, "jfklaflkdsajfsafjslajfkslfjlksjfklsjfls"))

            Return testValues
        End Function

    End Module
End Namespace