Imports ALOD.Core.Domain.Common
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class ICDCodeControl
        Inherits System.Web.UI.UserControl

        Private _icdHierarchyCSV As String  'chapter,section,diagnosis level 1,diagnosis level 2,diagnosis level 3,diagnosis level 4

        Private isTransitioning As Boolean = False

        Public Event ICDCodeSelectionChanged(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)

        Public Event ICDDiagnosisChanged(ByVal sender As Object, ByVal e As EventArgs)

        Public ReadOnly Property ICDCodeDiagnosisLabelText As String
            Get
                Return lblICDDiagnosis.Text
            End Get
        End Property

        Public ReadOnly Property ICDContentDropDownClientID As String
            Get
                Return ddlICDDiagnosisLevel1.ClientID
            End Get
        End Property

        Public ReadOnly Property ICDSectionDropDownClientID As String
            Get
                Return ddlICDSection.ClientID
            End Get
        End Property

        Public ReadOnly Property SelectedICDCodeID As Integer
            Get
                Dim ddl As DropDownList = GetMostRecentlyUsedDDL()

                If (ddl Is Nothing) Then
                    Return 0
                Else
                    Return CInt(ddl.SelectedValue)
                End If
            End Get
        End Property

        Public ReadOnly Property SelectedICDCodeText As String
            Get
                Dim ddl As DropDownList = GetMostRecentlyUsedDDL()

                If (ddl Is Nothing) Then
                    Return String.Empty
                Else
                    Return ddl.SelectedItem.Text
                End If
            End Get
        End Property

        Protected Property IsReadOnly() As Boolean
            Get
                Return CInt(ViewState("IsReadOnly"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("IsReadOnly") = value
            End Set
        End Property

        Public Sub DisableLastLevel(ByVal hideDDL As Boolean)
            ddlICDDiagnosisLevel4.Enabled = False
            'cddDiagnosisLevel4.Enabled = False
            ddlICDDiagnosisLevel4.Visible = (Not hideDDL)
        End Sub

        ''' <summary>
        ''' Disables the ICD Code dropdown listboxes. Whether or not the listboxes are visible can be specified.
        ''' </summary>
        ''' <param name="hideDropDowns"></param>
        ''' <remarks></remarks>
        Public Sub DisplayReadOnly(ByVal hideDropDowns As Boolean)
            IsReadOnly = True

            ' Disable dropdowns...
            ddlICDChapter.Enabled = False
            ddlICDSection.Enabled = False
            ddlICDDiagnosisLevel1.Enabled = False
            ddlICDDiagnosisLevel2.Enabled = False
            ddlICDDiagnosisLevel3.Enabled = False
            ddlICDDiagnosisLevel4.Enabled = False

            ' Hide the dropdowns and ajax controls...
            If (hideDropDowns) Then
                ddlICDChapter.Visible = False
                ddlICDSection.Visible = False
                ddlICDDiagnosisLevel1.Visible = False
                ddlICDDiagnosisLevel2.Visible = False
                ddlICDDiagnosisLevel3.Visible = False
                ddlICDDiagnosisLevel4.Visible = False
            End If

            lblICDDiagnosis.Visible = True
            lblICDDiagnosis.Enabled = True

        End Sub

        ''' <summary>
        ''' Enables and makes visible the ICE Code dropdown listboxes.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DisplayReadWrite(ByVal hideICDLabel As Boolean)
            IsReadOnly = False

            ' Enable all dropdowns...
            ddlICDChapter.Enabled = True
            ddlICDSection.Enabled = True
            ddlICDDiagnosisLevel1.Enabled = True
            ddlICDDiagnosisLevel2.Enabled = True
            ddlICDDiagnosisLevel3.Enabled = True
            ddlICDDiagnosisLevel4.Enabled = True

            ' Determine which dropdowns need to be disabled...
            Select Case GetDropDownLevel(GetMostRecentlyUsedDDL())
                Case 0
                    DisableChapterChildrenDDLs()
                Case 1
                    DisableChapterChildrenDDLs()
                Case 2
                    DisableSectionChildrenDDLs()
                Case 3
                    DisableDiagnosisLevel1ChildrenDDLs()
                Case 4
                    DisableDiagnosisLevel2ChildrenDDLs()
                Case 5
                    DisableDiagnosisLevel3ChildrenDDLs()
                Case Else
                    DisableChapterChildrenDDLs()
            End Select

            ' Show the dropdowns...
            ddlICDChapter.Visible = True
            ddlICDSection.Visible = True
            ddlICDDiagnosisLevel1.Visible = True
            ddlICDDiagnosisLevel2.Visible = True
            ddlICDDiagnosisLevel3.Visible = True
            ddlICDDiagnosisLevel4.Visible = True

            ' Hide ICD label
            If (hideICDLabel) Then
                lblICDDiagnosis.Visible = False
                lblICDDiagnosis.Enabled = False
            End If
        End Sub

        Public Sub ForceFullRebind(ByVal icdCodeId As Integer)
            BindChapterDDL()

            If (icdCodeId > 0) Then
                InitializeHierarchy(icdCodeId)

                If (IsHierarchySet()) Then
                    InitializeDDLHierarchy()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Hooks up the necessary components to allow the ICD Code control to work within host pages as a async postback control.
        ''' </summary>
        ''' <param name="hostPage"></param>
        ''' <remarks></remarks>
        Public Sub Initialilze(ByVal hostPage As Page)
            ' Register self as an asyn postback control...
            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)
        End Sub

        ''' <summary>
        ''' Initializes the ICD hierarchy for the specified bottom level ICD code ID.
        ''' </summary>
        ''' <param name="icdCodeId"></param>
        ''' <remarks></remarks>
        Public Sub InitializeHierarchy(ByVal icdCodeId As Integer)
            If (Not IsNothing(icdCodeId) AndAlso GetICDCodeVersion(icdCodeId) > 9) Then
                _icdHierarchyCSV = ICDHierarchy(icdCodeId)
            End If
        End Sub

        ''' <summary>
        ''' Returns whether or not the ICD hierarchy variable has been set.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsHierarchySet() As Boolean
            Return (Not String.IsNullOrEmpty(_icdHierarchyCSV))
        End Function

        ''' <summary>
        ''' Returns whether or not an ICD code has been selected in the last ICD dropdown listbox.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsICDCodeSelected() As Boolean
            Dim ddl As DropDownList = GetMostRecentlyUsedDDL()

            ' Check if nothing has been selected yet...
            If (ddl Is Nothing) Then
                Return False
            End If

            ' The values in first two drop down lists do not contain codes...
            If (ddl.ID.Equals(ddlICDChapter.ID) OrElse
                ddl.ID.Equals(ddlICDSection.ID)) Then
                Return False
            End If

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()

            ' Check if the selected value has children which means the lowest child has not been selected...
            If (icdDao.HasChildren(CInt(ddl.SelectedValue), True)) Then
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Returns whether or not the specific bottom level ICD code id is a valid ICD Code.
        ''' </summary>
        ''' <param name="codeId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsValidICDCode(ByVal codeId As Integer) As Boolean
            Dim code As ICD9Code = LookupService.GetIcd9CodeById(codeId)

            If (code Is Nothing) Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Updates the ICD code label to display the ICD value of the specified bottom level code.
        ''' </summary>
        ''' <param name="icdCodeId"></param>
        ''' <remarks></remarks>
        Public Sub UpdateICDCodeDiagnosisLabel(ByVal icdCodeId As Integer)
            UpdateICDCodeDiagnosisLabel(icdCodeId, True)
        End Sub

        ''' <summary>
        ''' Updates the ICD code label to display the ICD value of the specified bottom level code.
        ''' </summary>
        ''' <param name="icdCodeId"></param>
        ''' <param name="includeHRTag"></param>
        ''' <remarks></remarks>
        Public Sub UpdateICDCodeDiagnosisLabel(ByVal icdCodeId As Integer, ByVal includeHRTag As Boolean)
            If (IsNothing(icdCodeId)) Then
                Exit Sub
            End If

            Dim code As ICD9Code = LookupService.GetIcd9CodeById(icdCodeId)
            If (code IsNot Nothing) Then
                lblICDDiagnosis.Text = code.Description + " (ICD" + GetICDCodeVersion(icdCodeId) + ": " + code.Code + ")"
                lblPostHTMLTags.Text = "<br />"

                If (includeHRTag) Then
                    lblPostHTMLTags.Text = lblPostHTMLTags.Text + "<hr />"
                End If
            Else
                lblICDDiagnosis.Text = String.Empty
                lblPostHTMLTags.Text = String.Empty
            End If
        End Sub

        Protected Sub ddlICDDiagnosis_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDDiagnosisLevel1.SelectedIndexChanged, ddlICDDiagnosisLevel2.SelectedIndexChanged, ddlICDDiagnosisLevel3.SelectedIndexChanged, ddlICDDiagnosisLevel4.SelectedIndexChanged
            RaiseEvent ICDDiagnosisChanged(sender, e)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                BindChapterDDL()

                If (IsHierarchySet()) Then
                    InitializeDDLHierarchy()
                End If

                'PreviousControlLevel = 0
            End If
        End Sub

        Private Function GetDropDownLevel(ByVal ddl As DropDownList) As Integer
            If (ddl Is Nothing) Then
                Return 0
            End If

            Select Case ddl.ID
                Case ddlICDChapter.ID
                    Return 1
                Case ddlICDSection.ID
                    Return 2
                Case ddlICDDiagnosisLevel1.ID
                    Return 3
                Case ddlICDDiagnosisLevel2.ID
                    Return 4
                Case ddlICDDiagnosisLevel3.ID
                    Return 5
                Case ddlICDDiagnosisLevel4.ID
                    Return 6
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' Returns the ICD version of a specified ICD code ID as a string.
        ''' </summary>
        ''' <param name="codeId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetICDCodeVersion(ByVal codeId As Integer) As String
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Return icdDao.GetCodeVersion(codeId)
        End Function

        Private Function GetMostRecentlyUsedDDL() As DropDownList
            If (Not String.IsNullOrEmpty(ddlICDDiagnosisLevel4.SelectedValue)) Then
                Return ddlICDDiagnosisLevel4
            ElseIf (Not String.IsNullOrEmpty(ddlICDDiagnosisLevel3.SelectedValue)) Then
                Return ddlICDDiagnosisLevel3
            ElseIf (Not String.IsNullOrEmpty(ddlICDDiagnosisLevel2.SelectedValue)) Then
                Return ddlICDDiagnosisLevel2
            ElseIf (Not String.IsNullOrEmpty(ddlICDDiagnosisLevel1.SelectedValue)) Then
                Return ddlICDDiagnosisLevel1
            ElseIf (Not String.IsNullOrEmpty(ddlICDSection.SelectedValue)) Then
                Return ddlICDSection
            ElseIf (Not String.IsNullOrEmpty(ddlICDChapter.SelectedValue)) Then
                Return ddlICDChapter
            Else
                Return Nothing
            End If
        End Function

        Private Sub InitializeDDLHierarchy()
            If (Not String.IsNullOrEmpty(_icdHierarchyCSV)) Then
                Dim items() As String
                items = _icdHierarchyCSV.ToString.Split(",")

                If (items.Count > 0) Then
                    ddlICDChapter.SelectedValue = items(0)
                    BindSectionDDL()
                End If

                If (items.Count > 1) Then
                    ddlICDSection.SelectedValue = items(1)
                    BindDiagnosisLevel1DDL()
                End If

                If (items.Count > 2) Then
                    ddlICDDiagnosisLevel1.SelectedValue = items(2)
                    BindDiagnosisLevel2DDL()
                End If

                If (items.Count > 3) Then
                    ddlICDDiagnosisLevel2.SelectedValue = items(3)
                    BindDiagnosisLevel3DDL()
                End If

                If (items.Count > 4) Then
                    ddlICDDiagnosisLevel3.SelectedValue = items(4)
                    BindDiagnosisLevel4DDL()
                End If

                If (items.Count > 5) Then
                    ddlICDDiagnosisLevel4.SelectedValue = items(5)
                End If
            End If
        End Sub

        Private Sub RaiseICDCodeSelectionChangedEvent(ByVal ddl As DropDownList)
            Dim eventArgs As ICDCodeSelectedEventArgs = New ICDCodeSelectedEventArgs()

            If (String.IsNullOrEmpty(ddl.SelectedValue)) Then
                eventArgs.SelectedICDCodeId = 0
            Else
                eventArgs.SelectedICDCodeId = CInt(ddl.SelectedValue)
            End If

            eventArgs.SelectedDropDownLevel = GetDropDownLevel(ddl)

            RaiseEvent ICDCodeSelectionChanged(ddl, eventArgs)
        End Sub

#Region "DDL Management"

        Protected Sub ddlICDChapter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDChapter.SelectedIndexChanged
            If (String.IsNullOrEmpty(ddlICDChapter.SelectedValue)) Then
                DisableChapterChildrenDDLs()
            Else
                BindSectionDDL()
            End If

            RaiseICDCodeSelectionChangedEvent(ddlICDChapter)
        End Sub

        Protected Sub ddlICDDiagnosisLevel1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDDiagnosisLevel1.SelectedIndexChanged
            If (String.IsNullOrEmpty(ddlICDDiagnosisLevel1.SelectedValue)) Then
                DisableDiagnosisLevel1ChildrenDDLs()
            Else
                BindDiagnosisLevel2DDL()
            End If

            RaiseICDCodeSelectionChangedEvent(ddlICDDiagnosisLevel1)
        End Sub

        Protected Sub ddlICDDiagnosisLevel2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDDiagnosisLevel2.SelectedIndexChanged
            If (String.IsNullOrEmpty(ddlICDDiagnosisLevel2.SelectedValue)) Then
                DisableDiagnosisLevel2ChildrenDDLs()
            Else
                BindDiagnosisLevel3DDL()
            End If

            RaiseICDCodeSelectionChangedEvent(ddlICDDiagnosisLevel2)
        End Sub

        Protected Sub ddlICDDiagnosisLevel3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDDiagnosisLevel3.SelectedIndexChanged
            If (String.IsNullOrEmpty(ddlICDDiagnosisLevel3.SelectedValue)) Then
                DisableDiagnosisLevel3ChildrenDDLs()
            Else
                BindDiagnosisLevel4DDL()
            End If

            RaiseICDCodeSelectionChangedEvent(ddlICDDiagnosisLevel3)
        End Sub

        Protected Sub ddlICDDiagnosisLevel4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDDiagnosisLevel4.SelectedIndexChanged
            RaiseICDCodeSelectionChangedEvent(ddlICDDiagnosisLevel4)
        End Sub

        Protected Sub ddlICDSection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlICDSection.SelectedIndexChanged
            If (String.IsNullOrEmpty(ddlICDSection.SelectedValue)) Then
                DisableSectionChildrenDDLs()
            Else
                BindDiagnosisLevel1DDL()
            End If

            RaiseICDCodeSelectionChangedEvent(ddlICDSection)
        End Sub

        Private Sub BindChapterDDL()
            ddlICDChapter.DataValueField = "Value"
            ddlICDChapter.DataTextField = "Text"
            ddlICDChapter.DataSource = GetICDCodeChapterValues()
            ddlICDChapter.DataBind()

            ddlICDChapter.Items.Insert(0, New ListItem("-- Select ICD Chapter --", ""))
            ddlICDChapter.SelectedIndex = 0

            ' Set editability of the control...
            SetDDLEditability(ddlICDChapter)

            ' Disable child ddls...
            DisableChapterChildrenDDLs()
        End Sub

        Private Sub BindChildDDL(ByVal childDDL As DropDownList, ByVal parentDDL As DropDownList, ByVal selectText As String)
            If (String.IsNullOrEmpty(parentDDL.SelectedValue)) Then
                Exit Sub
            End If

            childDDL.DataValueField = "Value"
            childDDL.DataTextField = "Text"
            childDDL.DataSource = GetICDCodeChildrenValues(CInt(parentDDL.SelectedValue))
            childDDL.DataBind()

            childDDL.Items.Insert(0, New ListItem("-- Select ICD " & selectText & " --", ""))
            childDDL.SelectedIndex = 0
        End Sub

        Private Sub BindDiagnosisLevel1DDL()
            ' Populate Diagnosis Level 1 ddl
            BindChildDDL(ddlICDDiagnosisLevel1, ddlICDSection, "Diagnosis Level 1")

            ' Set editability of the control...
            SetDDLEditability(ddlICDDiagnosisLevel1)

            ' Disable child ddls...
            DisableDiagnosisLevel1ChildrenDDLs()
        End Sub

        Private Sub BindDiagnosisLevel2DDL()
            ' Populate Diagnosis Level 2 ddl
            BindChildDDL(ddlICDDiagnosisLevel2, ddlICDDiagnosisLevel1, "Diagnosis Level 2")

            ' Set editability of the control...
            SetDDLEditability(ddlICDDiagnosisLevel2)

            ' Disable child ddls...
            DisableDiagnosisLevel2ChildrenDDLs()
        End Sub

        Private Sub BindDiagnosisLevel3DDL()
            ' Populate Diagnosis Level 3 ddl
            BindChildDDL(ddlICDDiagnosisLevel3, ddlICDDiagnosisLevel2, "Diagnosis Level 3")

            ' Set editability of the control...
            SetDDLEditability(ddlICDDiagnosisLevel3)

            ' Disable child ddls...
            DisableDiagnosisLevel3ChildrenDDLs()
        End Sub

        Private Sub BindDiagnosisLevel4DDL()
            ' Populate Diagnosis Level 4 ddl
            BindChildDDL(ddlICDDiagnosisLevel4, ddlICDDiagnosisLevel3, "Diagnosis Level 4")

            ' Set editability of the control...
            SetDDLEditability(ddlICDDiagnosisLevel4)
        End Sub

        Private Sub BindSectionDDL()
            ' Populate Section ddl
            BindChildDDL(ddlICDSection, ddlICDChapter, "Section")

            ' Set editability of the control...
            SetDDLEditability(ddlICDSection)

            ' Disable child ddls...
            DisableSectionChildrenDDLs()
        End Sub

        Private Sub DisableChapterChildrenDDLs()
            DisableDDL(ddlICDSection, "Section")
            DisableDDL(ddlICDDiagnosisLevel1, "Diagnosis Level 1")
            DisableDDL(ddlICDDiagnosisLevel2, "Diagnosis Level 2")
            DisableDDL(ddlICDDiagnosisLevel3, "Diagnosis Level 3")
            DisableDDL(ddlICDDiagnosisLevel4, "Diagnosis Level 4")
        End Sub

        Private Sub DisableDDL(ByVal ddl As DropDownList, ByVal selectText As String)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            ddl.Items.Clear()
            ddl.Items.Insert(0, New ListItem("-- Select ICD " & selectText & " --", ""))
            ddl.SelectedIndex = 0
            ddl.Enabled = False
        End Sub

        Private Sub DisableDiagnosisLevel1ChildrenDDLs()
            DisableDDL(ddlICDDiagnosisLevel2, "Diagnosis Level 2")
            DisableDDL(ddlICDDiagnosisLevel3, "Diagnosis Level 3")
            DisableDDL(ddlICDDiagnosisLevel4, "Diagnosis Level 4")
        End Sub

        Private Sub DisableDiagnosisLevel2ChildrenDDLs()
            DisableDDL(ddlICDDiagnosisLevel3, "Diagnosis Level 3")
            DisableDDL(ddlICDDiagnosisLevel4, "Diagnosis Level 4")
        End Sub

        Private Sub DisableDiagnosisLevel3ChildrenDDLs()
            DisableDDL(ddlICDDiagnosisLevel4, "Diagnosis Level 4")
        End Sub

        Private Sub DisableSectionChildrenDDLs()
            DisableDDL(ddlICDDiagnosisLevel1, "Diagnosis Level 1")
            DisableDDL(ddlICDDiagnosisLevel2, "Diagnosis Level 2")
            DisableDDL(ddlICDDiagnosisLevel3, "Diagnosis Level 3")
            DisableDDL(ddlICDDiagnosisLevel4, "Diagnosis Level 4")
        End Sub

        Private Sub SetDDLEditability(ByVal ddl As DropDownList)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (Not IsReadOnly AndAlso ddl.Items.Count > 1) Then
                ddl.Enabled = True
            Else
                ddl.Enabled = False
            End If
        End Sub

#End Region

#Region "Data Access"

        Public Function GetChapters() As IEnumerable(Of ICD9Code)
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim query = From r In icdDao.GetAll() Where r.Code Is Nothing And r.ParentId Is Nothing And r.Active Select r Order By r.SortOrder
            Return query
        End Function

        Public Function GetICDChildren(ByVal parentId As Integer) As IEnumerable(Of ICD9Code)
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim query = From r In icdDao.GetAll() Where r.ParentId = parentId And r.Active Select r
            Return query
        End Function

        Public Function GetICDCodeChapterValues() As List(Of ListItem)
            Dim children As IEnumerable(Of ICD9Code) = GetChapters()
            Dim values As New List(Of ListItem)
            Dim text As String = String.Empty

            For Each item As ICD9Code In children
                If (String.IsNullOrEmpty(item.Code)) Then
                    text = item.Description
                Else
                    text = item.Description & " - " & item.Code
                End If

                values.Add(New ListItem(text, item.Id.ToString()))
            Next

            Return values
        End Function

        Public Function GetICDCodeChildrenValues(ByVal selectedParentId As Integer) As List(Of ListItem)
            Dim children As IEnumerable(Of ICD9Code) = GetICDChildren(selectedParentId)
            Dim values As New List(Of ListItem)
            Dim text As String = String.Empty

            For Each item As ICD9Code In children
                If (String.IsNullOrEmpty(item.Code)) Then
                    text = item.Description
                Else
                    text = item.Description & " - " & item.Code
                End If

                values.Add(New ListItem(text, item.Id.ToString()))
            Next

            Return values

        End Function

#End Region

    End Class

End Namespace