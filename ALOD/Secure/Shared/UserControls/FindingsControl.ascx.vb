'For using this control we need to set the values for Remarks and Findings and Enabled property
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_FindingsControl
        Inherits System.Web.UI.UserControl

        Friend FindingsEnabled As Boolean
        Protected _section As String = ""

        Public Event DecisionSelected(ByVal sender As Object, ByVal e As RadioButtonSelectedEventArgs)

        Public Event FindingSelected(ByVal sender As Object, ByVal e As RadioButtonSelectedEventArgs)

#Region "Properties"

        Public Property AdditionalRemarks() As String
            Get
                Return Server.HtmlEncode(txtAdditionalRemarks.Text)
            End Get
            Set(ByVal value As String)
                txtAdditionalRemarks.Text = Server.HtmlDecode(value)
                lblAdditionalRemarksText.Text = value
            End Set
        End Property

        Public Property AdditionalRemarksLableText() As String
            Get
                Return lblAdditionalRemarks.Text
            End Get
            Set(ByVal value As String)
                lblAdditionalRemarks.Text = value
            End Set
        End Property

        Public Property Adjustlimit() As Integer

            Get
                Return txtRemarks2.MaxLength
            End Get
            Set(value As Integer)
                txtRemarks2.MaxLength = value
            End Set

        End Property

        'For some members decision type is concur/non concur .Default value is concur yes/no
        Public WriteOnly Property ApproveOf() As String

            Set(ByVal value As String)
                rblDecison.Items(0).Text = "Approve  " + value + "  Action"
                rblDecison.Items(1).Text = "Disapprove " + value + "  Action"

            End Set
        End Property

        'For some members decision type is concur/non concur .Default value is concur yes/no
        Public WriteOnly Property ConcurWith() As String

            Set(ByVal value As String)
                rblDecison.Items(0).Text = "Concur with the action of " + value
                rblDecison.Items(1).Text = "Non Concur with the action of " + value

            End Set
        End Property

        'Returns/sets the Decision selected value
        Public Property Decision() As String
            Get
                If (rblDecison.SelectedValue = "") Then
                    Return ""
                Else
                    Return rblDecison.SelectedValue
                End If
            End Get
            Set(ByVal value As String)
                If (value <> "") Then
                    rblDecison.SelectedValue = value
                    lblDecsion.Text = rblDecison.SelectedItem.Text

                End If
            End Set
        End Property

        'For some members decision type is concur/non concur .Default value is concur yes/no
        Public WriteOnly Property DecsionType() As Short

            Set(ByVal value As Short)
                If (value = DecisionType.ApproveDisapprove) Then  'Approve DisApprove
                    rblDecison.Items(0).Text = "Approve"
                    rblDecison.Items(1).Text = "Disapprove"
                End If
            End Set
        End Property

        Public Property DoDecisionAutoPostBack As Boolean
            Get
                Return rblDecison.AutoPostBack
            End Get
            Set(value As Boolean)
                rblDecison.AutoPostBack = value
            End Set
        End Property

        Public Property DoFindingsAutoPostBack As Boolean
            Get
                Return rblFindings.AutoPostBack
            End Get
            Set(value As Boolean)
                rblFindings.AutoPostBack = value
            End Set
        End Property

        Public Property EnableDecision As Boolean
            Get
                Return rblDecison.Enabled
            End Get
            Set(value As Boolean)
                rblDecison.Enabled = value
            End Set
        End Property

        Public Property EnableFindings As Boolean
            Get
                Return FindingsEnabled
            End Get
            Set(value As Boolean)
                FindingsEnabled = value
                rblFindings.Enabled = value
            End Set
        End Property

        'Returns/sets the Findings selected value 'LoadAll needed to get the selected value
        Public Property Findings() As String
            Get
                If rblFindings.SelectedValue = "" Then
                    Return Nothing
                Else
                    Return rblFindings.SelectedValue
                End If
            End Get
            Set(ByVal value As String)
                If (value <> "0" AndAlso Not value Is Nothing AndAlso rblFindings.Items.FindByValue(value) IsNot Nothing) Then
                    rblFindings.SelectedValue = value
                    lblFindings.Text = rblFindings.SelectedItem.Text
                End If
            End Set
        End Property

        Public Property FindingsIndex() As Integer
            Get
                Return rblFindings.SelectedIndex
            End Get
            Set(value As Integer)
                rblFindings.SelectedIndex = value
            End Set
        End Property

        'This property gives the differnt text info to the Findings Label
        Public Property FindingsLableText() As String
            Get
                Return divFindingLabel.Text
            End Get
            Set(ByVal value As String)
                divFindingLabel.Text = value
            End Set
        End Property

        Public Property FindingsList() As IList(Of FindingsLookUp)
            Get
                Return rblFindings.DataSource
            End Get
            Set(value As IList(Of FindingsLookUp))
                rblFindings.DataSource = value
                rblFindings.DataBind()
            End Set
        End Property

        'For some members only findings control is needed and not the decision
        Public Property FindingsOnly() As Boolean
            Get
                Return Not (dvDecision.Visible)
            End Get
            Set(ByVal value As Boolean)
                'Decision Invisible
                dvDecision.Visible = Not (value)
                ' If FindingsOnly is True and SetReadOnly is False, enable rblFindings
                If (value AndAlso Not SetReadOnly) Then
                    EnableFindings = True
                End If
            End Set
        End Property

        Public Property FindingsRequired As Boolean
            Get
                If (divFindingLabel.CssClass.Contains("labelRequired")) Then
                    Return True
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                If (value) Then
                    divFindingLabel.CssClass = "labelRequired"
                Else
                    divFindingLabel.CssClass = ""
                End If
            End Set
        End Property

        'Returns/sets the Findings text as string
        Public ReadOnly Property FindingsText() As String
            Get
                If rblFindings.SelectedValue = "" Then
                    Return ""
                Else
                    Return rblFindings.SelectedItem.Text
                End If
            End Get

        End Property

        ''For formal board members the finding radio button list is not avaliable they just concur or not concur
        'so this property is being used for members like formal medical ,formal legal
        Public Property Formal() As Boolean
            Get
                Return Not (dvFindings.Visible)

            End Get
            Set(ByVal value As Boolean)
                dvFindings.Visible = Not (value)   'If formal findings radio list not visible
            End Set
        End Property

        Public Property FormFindingsText() As String
            Get
                Return Server.HtmlEncode(txtRemarks2.Text)
            End Get
            Set(ByVal value As String)
                txtRemarks2.Text = Server.HtmlDecode(value)
                RemarkTextlbl2.Text = value
            End Set
        End Property

        'This property gives the differnt text info to the previous findings Label (since it will be different for different people in chain
        Public Property PrevFindingsLableText() As String
            Get
                Return PrvFindingLabel.Text
            End Get
            Set(ByVal value As String)
                PrvFindingLabel.Text = value
            End Set
        End Property

        'This property sets the  prev findings text
        Public Property PrevFindingsText() As String
            Get
                Return PrvFindingControl.Text
            End Get
            Set(ByVal value As String)
                PrvFindingControl.Text = value
            End Set
        End Property

        Public Property ReasonsLabelText() As String
            Get
                Return ReasonsLabel.Text
            End Get
            Set(ByVal value As String)
                ReasonsLabel.Text = value
            End Set
        End Property

        Public Property Remarks() As String
            Get
                Return Server.HtmlEncode(txtRemarks.Text)
            End Get
            Set(ByVal value As String)
                txtRemarks.Text = Server.HtmlDecode(value)
                RemarkTextlbl.Text = value
            End Set
        End Property

        'This property gives the differnt text info to the remarks  Label
        Public Property RemarksLableText() As String
            Get
                Return lblRemarks.Text
            End Get
            Set(ByVal value As String)
                lblRemarks.Text = value

            End Set
        End Property

        Public Property RemarksMaxLength As Integer
            Get
                Return txtRemarks.MaxLength
            End Get
            Set(value As Integer)
                If (value < 0) Then
                    txtRemarks.MaxLength = 0
                Else
                    txtRemarks.MaxLength = value
                End If
            End Set
        End Property

        Public Property RemarksVisible() As Boolean
            Get
                Return dvRemarks.Visible
            End Get
            Set(ByVal value As Boolean)
                dvRemarks.Visible = value
            End Set
        End Property

        'Used to log change set
        Public Property Section() As String
            Get
                Return _section

            End Get
            Set(ByVal value As String)
                _section = value
                txtRemarks.Attributes.Add("Section", value)
                txtRemarks2.Attributes.Add("Section", value)
                txtAdditionalRemarks.Attributes.Add("Section", value)
                rblFindings.Attributes.Add("Section", value)
                rblDecison.Attributes.Add("Section", value)

            End Set
        End Property

        Public Property SetAdditionalCommentsReadOnly() As Boolean
            Get
                Return txtAdditionalRemarks.Visible
            End Get
            Set(value As Boolean)
                txtAdditionalRemarks.Visible = Not (value)    'if readonly then text box should not be visible
                lblAdditionalRemarksText.Visible = value      'if readonly then labels should   be visible
            End Set
        End Property

        Public WriteOnly Property SetDecisionToggle() As Boolean
            Set(ByVal value As Boolean)

                Dim decision As String = Me.ClientID + "_" + "rblDecison_1"
                Dim fnd As String = Me.ClientID + "_" + "rblFindings"
                ' Store the IDs in data attributes instead of using onclick on container
                ' This prevents double-firing when clicking radio buttons
                rblDecison.Attributes.Add("data-decision-id", decision)
                rblDecison.Attributes.Add("data-finding-id", fnd)

            End Set

        End Property

        'By default this property is set to true whcich means the control is disabled .When the user has read write
        'access it is set as false through enabled properties on the pages
        Public Property SetReadOnly() As Boolean
            Get
                Return Not txtRemarks.Visible
            End Get
            Set(ByVal value As Boolean)

                'If readonly then the labels should be visible .LoadAll property should have been
                'set to true for the control on initialization otherwise will get error since all rblFindings will not have
                'any selected true
                If (value) Then

                    If (rblFindings.SelectedValue = "") Then
                        lblFindings.Text = " "
                    Else
                        lblFindings.Text = rblFindings.SelectedItem.Text

                    End If
                    If (rblDecison.SelectedValue = "") Then
                        lblDecsion.Text = " "
                    Else
                        lblDecsion.Text = rblDecison.SelectedItem.Text
                    End If
                Else
                    lblFindings.Text = ""     'if not readonly  then labels should   be empty text
                    RemarkTextlbl.Text = ""
                    RemarkTextlbl2.Text = ""
                    lblDecsion.Text = ""

                End If

                txtRemarks.Visible = Not (value)    'if readonly then text box should not be visible
                txtRemarks2.Visible = Not (value)    'if readonly then text box should not be visible
                txtAdditionalRemarks.Visible = Not (value)    'if readonly then text box should not be visible
                rblFindings.Visible = Not (value)    'if readonly then radio button list  should not be visible
                rblDecison.Visible = Not (value)     'if readonly then radio button list  should not be visible

                lblFindings.Visible = value         'if readonly then labels should   be visible
                RemarkTextlbl.Visible = value       'if readonly then labels should   be visible
                RemarkTextlbl2.Visible = value      'if readonly then labels should   be visible
                lblAdditionalRemarksText.Visible = value      'if readonly then labels should   be visible
                lblDecsion.Visible = value          'if readonly then labels should   be visible

                ' If FindingsOnly is True and SetReadOnly is False, enable rblFindings
                If (Not value AndAlso FindingsOnly) Then
                    EnableFindings = True
                End If
            End Set

        End Property

        Public Property ShowAdditionalRemarks() As Boolean
            Get
                Return AdditionalRemarksTable.Visible
            End Get
            Set(ByVal value As Boolean)
                AdditionalRemarksTable.Visible = value
            End Set
        End Property

        Public Property ShowAdditionalRemarksText() As Boolean
            Get
                Return dvAdditionalRemarks.Visible
            End Get
            Set(value As Boolean)
                dvAdditionalRemarks.Visible = value
            End Set
        End Property

        Public Property ShowFormText() As Boolean
            Get
                Return dvFormText.Visible

            End Get
            Set(ByVal value As Boolean)
                dvFormText.Visible = value
            End Set
        End Property

        Public Property ShownOnText() As String
            Get
                Return AppearsLabel.Text
            End Get
            Set(ByVal value As String)
                AppearsLabel.Text = value
            End Set
        End Property

        'This property gives the differnt text info to the previous findings Label (since it will be different for different people in chain
        Public Property ShowPrevFindings() As Boolean
            Get
                Return prevFindings.Visible
            End Get
            Set(ByVal value As Boolean)
                prevFindings.Visible = value
            End Set
        End Property

        Public Property ShowRemarks() As Boolean
            Get
                Return RemarksTable.Visible
            End Get
            Set(ByVal value As Boolean)
                RemarksTable.Visible = value
            End Set
        End Property

        Public Property StartingLabel() As String
            Get
                Dim key As String = "StartingLabel"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = "A"
                End If
                Return CStr(ViewState(key))
            End Get
            Set(ByVal value As String)
                ViewState("StartingLabel") = value
            End Set
        End Property

        Public Property UseRowLabels() As Boolean
            Get
                Dim key As String = "UseRowLabels"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = False
                End If
                Return CBool(ViewState(key))
            End Get
            Set(ByVal value As Boolean)
                ViewState("UseRowLabels") = value
            End Set
        End Property

#End Region

#Region "Load"

        Public Sub DeselectFinding()
            rblFindings.ClearSelection()
        End Sub

        Public Sub DisableAllFindings()
            For Each item As ListItem In rblFindings.Items
                item.Selected = False
                item.Enabled = False
            Next
        End Sub

        Public Sub DisableAllFindingsExcept(ByVal findingText As String)
            If (String.IsNullOrEmpty(findingText)) Then
                Exit Sub
            End If

            For Each item As ListItem In rblFindings.Items
                If (Not item.Text.Equals(findingText)) Then
                    item.Selected = False
                    item.Enabled = False
                End If
            Next
        End Sub

        Public Sub DisableAllFindingsExcept(ByVal finding As Integer)
            If (finding = 0) Then
                Exit Sub
            End If

            For Each item As ListItem In rblFindings.Items
                If (Not item.Value = finding) Then
                    item.Selected = False
                    item.Enabled = False
                End If
            Next
        End Sub

        Public Sub DisableAllFindingsViaJavaScript()
            If (rblFindings.Attributes.Item("disabled") IsNot Nothing) Then
                rblFindings.Attributes.Item("disabled") = "true"
            Else
                rblFindings.Attributes.Add("disabled", "true")
            End If
        End Sub

        Public Sub DisableFinding(ByVal findingText As String)
            If (String.IsNullOrEmpty(findingText)) Then
                Exit Sub
            End If

            For Each item As ListItem In rblFindings.Items
                If (item.Text.Equals(findingText)) Then
                    item.Enabled = False
                End If
            Next
        End Sub

        ''' <summary>
        ''' Loads all finding options associated with the specified workflow Id.
        ''' </summary>
        ''' <param name="workflowId"></param>
        ''' <remarks></remarks>
        Public Sub LoadFindingOptionsByWorkflow(ByVal workflowId As Integer, ByVal groupId As Integer)
            If (workflowId < 1) Then
                Exit Sub
            End If

            rblFindings.DataSource = New LookupDao().GetWorkflowFindings(workflowId, groupId)

            rblFindings.DataBind()

            If (workflowId = 27) Then
                SetInputFormatRestriction(Page, txtRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestrictionNoReturn(Page, txtRemarks2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtAdditionalRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            Else
                SetInputFormatRestriction(Page, txtRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtRemarks2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtAdditionalRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            End If
        End Sub

        ''' <summary>
        ''' Loads all finding options associated with the specified workflow Id.
        ''' </summary>
        ''' <param name="workflowId"></param>
        ''' <param name="options">The options in the form of the FindingType (use InvestigationDecision enumeration)</param>
        ''' <param name="isInclude">TRUE if the finding options should include only the passed in options (ignores findings not mapped to the specified workflow); FALSE if the finding options should exlcude the passed in options (ignores findings not mapped to the specified workflow).</param>
        ''' <remarks></remarks>
        Public Sub LoadFindingOptionsByWorkflow(ByVal workflowId As Integer, ByVal groupId As Integer, ByVal options As String, ByVal isInclude As Boolean)
            If (workflowId < 1) Then
                Exit Sub
            End If

            If (isInclude) Then
                rblFindings.DataSource = From p In New LookupDao().GetWorkflowFindings(workflowId, groupId) Where options.Split(",").Contains(p.FindingType) Select p
            Else
                rblFindings.DataSource = From p In New LookupDao().GetWorkflowFindings(workflowId, groupId) Where Not options.Split(",").Contains(p.FindingType) Select p
            End If

            rblFindings.DataBind()
        End Sub

        Public Sub RemoveFinding(ByVal findingText As String)
            If (String.IsNullOrEmpty(findingText)) Then
                Exit Sub
            End If

            Dim selection As Integer

            For Each item As ListItem In rblFindings.Items
                If (item.Text.Equals(findingText)) Then
                    selection = rblFindings.Items.IndexOf(item)
                End If
            Next

            rblFindings.Items.RemoveAt(selection)

        End Sub

        Public Sub RenameFindingText(ByVal oldName As String, ByVal newName As String)
            If (String.IsNullOrEmpty(oldName) OrElse String.IsNullOrEmpty(newName)) Then
                Exit Sub
            End If

            For Each item As ListItem In rblFindings.Items
                If (item.Text.Equals(oldName)) Then
                    item.Text = newName
                    Exit Sub
                End If
            Next
        End Sub

        Public Sub SetAccess(ByVal access As ALOD.Core.Domain.Workflow.PageAccessType)
            If (access <> ALOD.Core.Domain.Workflow.PageAccessType.None) Then
                If (access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                    SetReadOnly = False
                End If
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            txtRemarks.Attributes.Add("Field", "Recommendation")
            txtAdditionalRemarks.Attributes.Add("Field", "AdditionalRecommendation")
            txtRemarks2.Attributes.Add("Field", "Form348Text")
            rblFindings.Attributes.Add("Field", "Findings")
            rblDecison.Attributes.Add("Field", "Decision")

            rblFindings.Enabled = FindingsEnabled

            SetMaxLength(txtRemarks2)

            If (txtRemarks.MaxLength > 0) Then
                SetMaxLength(txtRemarks)
            End If
        End Sub

#End Region

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ' Apply FindingsEnabled flag to rblFindings
            rblFindings.Enabled = FindingsEnabled

            ' If FindingsOnly is True and SetReadOnly is False, ensure rblFindings is enabled
            ' This handles cases where properties might be set in different orders
            If (FindingsOnly AndAlso Not SetReadOnly) Then
                EnableFindings = True
            End If

            If (UseRowLabels) Then
                Dim start As Integer = Asc(StartingLabel.Chars(0))

                If (Not FindingsOnly) Then
                    DecisionLabel.Text = Chr(start)
                    start += 1
                End If

                FindingsLabel.Text = Chr(start)
                start += 1

                If (ShowRemarks) Then
                    RemarksNumber.Text = Chr(start)
                    start += 1
                End If

                If (ShowAdditionalRemarks) Then
                    lblAdditionalRemarksNumber.Text = Chr(start)
                    start += 1
                End If

                FindingsNumber.Text = Chr(start)
                start += 1

            End If
        End Sub

        Protected Sub rblDecison_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDecison.SelectedIndexChanged
            ' Disable/Enable findings...this is needed if the control is placed in an update panel which causes connections to JavaScript code break...
            If (rblDecison.AutoPostBack = True AndAlso Decision.Equals("Y")) Then
                EnableFindings = False
                FindingsIndex = -1
            Else
                EnableFindings = True
            End If

            ' Check if the selected decision is "Concur with the action of Unit Commander"
            If rblDecison.SelectedItem IsNot Nothing AndAlso rblDecison.SelectedItem.Text.Contains("Concur with the action of Unit Commander") Then
                DisableFinding("In Line Of Duty (ILOD)")
                DisableFinding("Not ILOD - Due To Own Misconduct")
                DisableFinding("Not ILOD - Not Due To Own Misconduct")
            End If

            ' Raise event...
            Dim evt As New RadioButtonSelectedEventArgs

            If (rblDecison.SelectedItem IsNot Nothing) Then
                evt.ButtonValue = rblDecison.SelectedValue
                evt.ButtonText = rblDecison.SelectedItem.Text
            End If

            RaiseEvent DecisionSelected(Me, evt)
        End Sub

        Protected Sub rblFindings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblFindings.SelectedIndexChanged
            Dim evt As New RadioButtonSelectedEventArgs

            If (rblFindings.SelectedItem IsNot Nothing) Then
                evt.ButtonValue = rblFindings.SelectedValue
                evt.ButtonText = rblFindings.SelectedItem.Text
            End If

            RaiseEvent FindingSelected(Me, evt)

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "text", "initMultiLines()", True)
        End Sub

    End Class

End Namespace