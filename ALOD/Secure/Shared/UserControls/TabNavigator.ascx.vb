Imports System.IO
Imports System.Xml
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.TabNavigation

Namespace Web.UserControls

    Partial Class TabNavigator
        Inherits System.Web.UI.UserControl

#Region "Members / Properties"

#Region "TabConfig class"

        <Serializable()>
        Protected Class TabConfig
            Public Access As IList(Of ALOD.Core.Domain.Workflow.PageAccess)
            Public DeleteButtonVisible As Boolean = False
            Public DeleteButtonVisibleDefault As Boolean = False
            Public DeleteButtonVisibleSet As Boolean = False
            Public JumpBoxVisible As Boolean = False
            Public JumpBoxVisibleDefault As Boolean = True
            Public JumpBoxVisibleSet As Boolean = False
            Public PrintButton As Boolean = Nothing
            Public PrintButtonDefault As Boolean = True
            Public PrintButtonset As Boolean = False
            Public PrintButtonText As String = String.Empty
            Public PrintButtonTextDefault As String = "Print"
            Public PrintButtonTextSet As Boolean = False
            Public PrintScript As String = ""
            Public StartPage As String = String.Empty
            Public StartPageDefault As String = String.Empty
            Public StartPageSet As Boolean = False

            Public Sub New()
            End Sub

            Public Sub SetDefaults(ByVal config As TabConfig)
                PrintScript = config.PrintScript
                Access = config.Access
                StartPageDefault = config.StartPageDefault
                PrintButtonDefault = config.PrintButtonDefault
                PrintButtonTextDefault = config.PrintButtonTextDefault
                JumpBoxVisible = config.JumpBoxVisibleDefault
            End Sub

        End Class

#End Region

        Protected _config As New TabConfig
        Protected _curIndex As Short = 0
        Protected _inited As Boolean = False
        Protected _navControls As TabControlBase = Nothing
        Protected _sourceFile As String = String.Empty
        Protected _steps As TabItemList

        'get a little circular reference going here.  Huzzah!
        Private _associatedCaseDao As IAssociatedCaseDao

        Private _daoFactory As IDaoFactory
        Private _specCaseDao As ISpecialCaseDAO

        Public Event ButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public ReadOnly Property CurrentStep() As TabItem
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return _steps(_curIndex)
            End Get
        End Property

        Public Property CurrentStepId() As Integer
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return GetCurrentPageIndex() + 1
                Return _curIndex + 1
            End Get
            Set(ByVal Value As Integer)

                Dim index As Integer = Value - 1

                If (index < 0) Or (index > MaxIndex) Then
                    Exit Property
                End If

                _curIndex = index
            End Set
        End Property

        Public Property DeleteButtonVisible() As Boolean
            Get
                CheckInit()

                If (Not _config.DeleteButtonVisibleSet) Then
                    Return _config.DeleteButtonVisibleDefault
                End If

                Return _config.DeleteButtonVisible

            End Get
            Set(ByVal value As Boolean)
                _config.DeleteButtonVisible = value
                _config.DeleteButtonVisibleSet = True
            End Set
        End Property

        Public Property JumpBoxVisible() As Boolean
            Get
                CheckInit()

                If (Not _config.JumpBoxVisibleSet) Then
                    Return _config.JumpBoxVisibleDefault
                End If

                Return _config.JumpBoxVisible

            End Get
            Set(ByVal value As Boolean)
                _config.JumpBoxVisible = value
                _config.JumpBoxVisibleSet = True
            End Set
        End Property

        Public ReadOnly Property MaxIndex() As Integer
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return _steps.Count - 1
            End Get
        End Property

        Public Property NavControls() As TabControlBase
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return _navControls
            End Get
            Set(ByVal Value As TabControlBase)
                _navControls = Value
            End Set
        End Property

        Public ReadOnly Property NextButtonEnabled() As Boolean
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                'if the current step is the last one then we are at the end
                If (CurrentStepId = NumSteps) Then
                    Return False
                End If

                'otherwise, we need to determine if we are the last visible step
                For i As Short = CurrentStepId - 1 To NumSteps - 1
                    If (_steps(i).Visible) Then
                        Return True 'we have at least one more visible step past the current one
                    End If
                Next

                Return False 'we are the last visible step
            End Get
        End Property

        Public ReadOnly Property NextPageUrl() As String
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (_curIndex = MaxIndex) Then
                    Return String.Empty
                End If

                For i As Short = _curIndex + 1 To MaxIndex
                    If (_steps(i).Visible) Then
                        Return _steps(i).Page
                    End If
                Next

                Return String.Empty

            End Get
        End Property

        Public ReadOnly Property NumSteps() As Short
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return _steps.Count
            End Get

        End Property

        Public ReadOnly Property PrevButtonEnabled() As Boolean
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (CurrentStepId = 1) Then
                    Return False 'we are the first step, no need to check further
                End If

                'otherwise, we need to determine if we are the first visible step
                'For i As Short = CurrentStepId - 2 To 1 Step -1
                For i As Short = CurrentStepId - 1 To 1 Step -1
                    If (_steps(i).Visible) Then
                        Return True 'we have at least one more visible step before the current one
                    End If
                Next

                Return False 'we are the first visible step
            End Get
        End Property

        Public ReadOnly Property PrevPageUrl() As String
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (_curIndex = 0) Then
                    Return String.Empty
                End If

                For i As Short = _curIndex - 1 To 0 Step -1
                    If (_steps(i).Visible) Then
                        Return _steps(i).Page
                    End If
                Next

                Return String.Empty

            End Get
        End Property

        Public Property PrintButtonText() As String
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (Not _config.PrintButtonTextSet) Then
                    Return _config.PrintButtonTextDefault
                End If

                Return _config.PrintButtonText

            End Get
            Set(ByVal Value As String)
                _config.PrintButtonTextSet = True
                _config.PrintButtonText = Value
            End Set
        End Property

        Public Property PrintButtonVisible() As Boolean
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (Not _config.PrintButtonset) Then
                    Return _config.PrintButtonDefault
                End If
                Return _config.PrintButton
            End Get
            Set(ByVal Value As Boolean)
                _config.PrintButtonset = True
                _config.PrintButton = Value
            End Set
        End Property

        Public Property PrintScript() As String
            Get
                CheckInit()
                Return _config.PrintScript
            End Get
            Set(ByVal value As String)
                _config.PrintScript = value
            End Set
        End Property

        Public Property SourceFile() As String
            Get
                Return _sourceFile
            End Get

            Set(ByVal Value As String)
                _sourceFile = Value
            End Set
        End Property

        Public Property StartPage() As String
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                If (Not _config.StartPageSet) Then
                    Return _config.StartPageDefault
                End If

                Return _config.StartPage
            End Get

            Set(ByVal Value As String)
                _config.StartPageSet = True
                _config.StartPage = Value
            End Set
        End Property

        Public ReadOnly Property Steps() As TabItemList
            Get
                If (Not _inited) Then
                    InitControl()
                End If

                Return _steps
            End Get
        End Property

        Protected ReadOnly Property Associated() As IAssociatedCaseDao
            Get
                If (_associatedCaseDao Is Nothing) Then
                    _associatedCaseDao = DaoFactory.GetAssociatedCaseDao()
                End If
                Return _associatedCaseDao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

#End Region

#Region "Navigation methods"

        Public Sub EnableAllSteps()

            If (Not _inited) Then
                InitControl()
            End If

            For Each item As TabItem In _steps
                item.Enabled = True
            Next

        End Sub

        Public Sub EnableStep(ByVal stepNum As Integer)

            If (Not _inited) Then
                InitControl()
            End If

            If (Not StepInRange(stepNum)) Then
                Exit Sub
            End If

            _steps(stepNum - 1).Enabled = True

        End Sub

        Public Sub EnableSteps(ByVal steps() As Integer)

            If (Not _inited) Then
                InitControl()
            End If

            For Each index As Integer In steps
                EnableStep(index)
            Next

        End Sub

        Public Sub EnableSteps(ByVal firstStep As Integer, ByVal lastStep As Integer)

            If (Not _inited) Then
                InitControl()
            End If

            For i As Integer = firstStep To lastStep
                EnableStep(i)
            Next

        End Sub

        Public Function GetPageUrl(ByVal pageTitle As String) As String

            'locate this step and redirect to it
            For Each row As TabItem In Me._steps
                If (row.Title.ToLower() = pageTitle.ToLower()) Then
                    Return row.Page
                End If
            Next

            Return String.Empty

        End Function

        Public Function GetStep(ByVal title As String) As TabItem
            For Each row As TabItem In Me._steps
                If (row.Title.ToLower() = title.ToLower()) Then
                    Return row
                End If
            Next
            Return Nothing
        End Function

        Public Sub MoveToNextPage()
            Response.Redirect(NextPageUrl + Request.Url.Query)
        End Sub

        Public Sub MoveToPage(ByVal pageTitle As String)

            Dim item = (From s In _steps Where s.Title = pageTitle Select s).SingleOrDefault()

            If (item IsNot Nothing) Then
                Response.Redirect(item.Page + Request.Url.Query, True)
            End If

            ''locate this step and redirect to it
            'For Each row As TabItem In Me._steps
            '    If (row.Title.ToLower() = pageTitle.ToLower()) Then
            '        Response.Redirect(row.Page + Request.Url.Query)
            '    End If
            'Next

        End Sub

        Public Sub MoveToPage(ByVal pageStep As Integer)

            If (pageStep >= _steps.Count) Then
                Exit Sub
            End If

            Response.Redirect(_steps(pageStep).Page + Request.Url.Query)

        End Sub

        Public Sub MoveToPrevPage()
            Response.Redirect(PrevPageUrl + Request.Url.Query)
        End Sub

        Public Sub MoveToStartPage()
            Dim url As String = StartPage
            ClearSession()
            Response.Redirect(url + Request.Url.Query)
        End Sub

        Public Sub MoveToStep(ByVal item As TabItem)
            MoveToPage(item.Title)
        End Sub

        Public Sub ShowStep(ByVal stepName As String, ByVal show As Boolean)

            If (Not _inited) Then
                InitControl()
            End If

            Dim item As TabItem = GetStep(stepName)

            If (item IsNot Nothing) Then
                item.Visible = show
            End If

        End Sub

        Public Sub ShowStep(ByVal stepNum As Integer, ByVal show As Boolean)

            If (Not _inited) Then
                InitControl()
            End If

            If (Not StepInRange(stepNum)) Then
                Exit Sub
            End If

            _steps(stepNum - 1).Visible = show

        End Sub

        Public Sub StartFromPage(ByVal page As Integer)

            'convert from page count to index by subtracting 1
            'obviously we only want to do this if we actually have a step working
            'i.e. page > 0

            If (page > 0) Then
                page = page - 1
            End If

            If (page > _steps.Count) Then
                Exit Sub
            End If

            'since we are starting at some arbitrary step, we need to assume that
            'the steps prior to it have been completed, so we need to enable them
            For i As Int16 = 0 To page - 1
                _steps(i).Completed = True
                _steps(i).Enabled = True
            Next

            'enable the step we are moving to
            _steps(page).Enabled = True

            'now move to that page
            Dim url As String = _steps(page).Page
            Response.Redirect(_steps(page).Page + Request.Url.Query)

        End Sub

        Public Sub StepCompleted()
            StepCompleted(CurrentStepId)
        End Sub

        Public Sub StepCompleted(ByVal index As Integer)

            If (index <= 0) Or (index >= _steps.Count) Then
                Exit Sub
            End If

            _steps(index - 1).Completed = True

            If (index <> _steps.Count) Then
                _steps(index).Enabled = True
            End If

        End Sub

        Public Function StepInRange(ByVal stepNum As Integer) As Boolean
            Return (stepNum > 0 And stepNum < _steps.Count)
        End Function

#End Region

#Region "Private methods (init, etc)"

        Public Property PageAccess() As IList(Of ALOD.Core.Domain.Workflow.PageAccess)
            Get
                CheckInit()
                Return _config.Access
            End Get
            Set(ByVal value As IList(Of ALOD.Core.Domain.Workflow.PageAccess))
                SetPageAccess(value)
            End Set
        End Property

        Default Public Property Item(ByVal title As String) As TabItem
            Get
                CheckInit()

                For Each row As TabItem In _steps
                    If (row.Title = title) Then
                        Return row
                    End If
                Next

                Return New TabItem(title)

            End Get
            Set(ByVal value As TabItem)
                CheckInit()

                For Each row As TabItem In _steps
                    If (row.Title = title) Then
                        row = value
                    End If
                Next
            End Set
        End Property

        Public Sub Cancel(Optional ByVal url As String = "")

            If (Not _inited) Then
                InitControl()
            End If

            If (url.Length = 0) Then
                url = StartPage
            End If

            ClearSession()
            Response.Redirect("~/Secure/Welcome.aspx")
            'Response.Redirect(url)
        End Sub

        Public Sub ClearSession()
            _steps = Nothing
            _config = Nothing
            Session.Remove("NavigatorSteps")
            Session.Remove("NavConfig")
            Session.Remove("Access")
        End Sub

        Public Sub Commit()

            If (Not _inited) Then
                Exit Sub
            End If

            Session("NavigatorSteps") = _steps
            Session("NavConfig") = _config
            Session("Access") = _config.Access

        End Sub

        Public Function Find(ByVal title As String) As TabItem
            Return Item(title)
        End Function

        Public Function GetCurrentPageIndex() As Short

            If (Not _inited) Then
                InitControl()
            End If

            'set the current page, based on the request
            Dim file As String = Request.FilePath
            file = Path.GetFileName(file)
            Dim index As Short = 0

            For Each row As TabItem In _steps
                If (row.Page.ToLower() = file.ToLower()) Then
                    index = row.Order - 1
                    Exit For
                End If
            Next

            Return index

        End Function

        Public Sub InitControl()

            If (_inited) Then
                Exit Sub
            End If

            If (_steps Is Nothing) Then

                If (Session("NavigatorSteps") Is Nothing) Then
                    'read our step data from the xml file

                    If (SourceFile.Trim().Length = 0) Then
                        Throw New ArgumentNullException("Required attribute SourceFile was not provided")
                    End If

                    _steps = New TabItemList
                    Dim stream As StreamReader = Nothing
                    Dim reader As XmlTextReader = Nothing
                    Dim ds As New DataSet

                    Try

                        stream = New StreamReader(Server.MapPath(SourceFile), Encoding.UTF8)
                        reader = New XmlTextReader(stream)
                        ds.ReadXml(reader, XmlReadMode.Auto)

                        _config = New TabConfig
                        Dim tbConfig As DataTable = ds.Tables("Configuration")
                        Dim tbSteps As DataTable = ds.Tables("Step")

                        If (tbConfig Is Nothing) OrElse (tbSteps Is Nothing) Then
                            Throw New ConfigurationErrorsException("Configuration not found in SourceFile")
                        End If

                        If (tbConfig.Rows.Count = 0) Then
                            Throw New ConfigurationErrorsException("Configuration not found in SourceFile")
                        End If

                        'read configuration
                        _config.StartPageDefault = tbConfig.Rows(0)("StartPage")
                        _config.PrintButtonDefault = Boolean.Parse(tbConfig.Rows(0)("PrintButtonVisible"))
                        _config.PrintButtonTextDefault = tbConfig.Rows(0)("PrintButtonText")
                        _config.PrintScript = tbConfig.Rows(0)("PrintScript")

                        If (tbConfig.Columns.Contains("DeleteButtonVisible")) Then
                            _config.DeleteButtonVisibleDefault = Boolean.Parse(tbConfig.Rows(0)("DeleteButtonVisible"))
                        End If

                        If (tbConfig.Columns.Contains("JumpBoxVisible")) Then
                            _config.JumpBoxVisibleDefault = Boolean.Parse(tbConfig.Rows(0)("JumpBoxVisible"))
                        End If

                        'read our steps
                        Dim wizStep As TabItem
                        Dim i As Short = 1

                        'Dim sc = SpecCaseDao.GetById()
                        'Dim cases As IList(Of AssociatedCase) = New List(Of AssociatedCase)
                        'cases = cases.Concat(Associated.GetAssociatedCasesLOD(sc.Id, sc.Workflow)).ToList()

                        For Each row As DataRow In tbSteps.Rows

                            'read it from the dataset
                            wizStep = New TabItem(row("Title"), i, row("Page"))

                            wizStep.Visible = Boolean.Parse(row("Visible"))
                            wizStep.Enabled = Boolean.Parse(row("Enabled"))

                            If (row.Table.Columns.Contains("ClientScript")) Then
                                If (row("ClientScript") IsNot Nothing) Then
                                    wizStep.ClientScript = row("ClientScript").ToString()
                                End If
                            End If

                            'automatically enable our first step
                            If (i = 1) Then
                                wizStep.Enabled = True
                            End If

                            'add it to the list
                            _steps.Add(wizStep)
                            i += 1

                        Next
                    Catch ae As ArgumentException

                        Dim aeInner As New ConfigurationErrorsException("The supplied SourceFile contains invalid data or is in an incorrect format")
                        LogManager.LogError(aeInner, "InitControl")
                        Throw aeInner
                    Catch ice As InvalidCastException

                        Dim iceInner As New ConfigurationErrorsException("The supplied SourceFile contains invalid data or is in an incorrect format")
                        LogManager.LogError(iceInner, "InitControl")
                        Throw iceInner
                    Finally

                        reader.Close()
                        stream.Close()

                    End Try
                Else
                    _steps = Session("NavigatorSteps")
                    Dim conf As TabConfig = Session("NavConfig")
                    _config.SetDefaults(conf)
                    _config.Access = Session("Access")
                End If

                _inited = True

            End If

        End Sub

        Public Sub SetPageAccess(ByVal accessList As IList(Of ALOD.Core.Domain.Workflow.PageAccess))

            _config.Access = accessList

            Dim access As ALOD.Core.Domain.Workflow.PageAccess

            For Each item As TabItem In _steps

                Dim currentItem As TabItem = item
                access = (From a In accessList Where a.PageTitle = currentItem.Title Select a).SingleOrDefault()

                If (access IsNot Nothing) Then
                    item.Access = access.Access
                    If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.None) Then
                        item.Visible = False
                    Else
                        item.Visible = True
                        item.IsReadOnly = (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadOnly)
                    End If
                Else
                    item.Visible = False
                End If
            Next

        End Sub

        Public Sub SetPageAccess(ByVal accessList As IList(Of ALOD.Core.Domain.Workflow.PageAccess), ByVal associatedWorkFlow As Int64)

            _config.Access = accessList

            Dim access As ALOD.Core.Domain.Workflow.PageAccess

            For Each item As TabItem In _steps

                Dim currentItem As TabItem = item
                access = (From a In accessList Where a.PageTitle = currentItem.Title Select a).SingleOrDefault()

                If (access IsNot Nothing) Then
                    item.Access = access.Access
                    If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.None) Then
                        item.Visible = False
                    Else
                        item.Visible = True
                        item.IsReadOnly = (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadOnly)
                    End If
                Else
                    item.Visible = False
                End If

                If (associatedWorkFlow = 11 AndAlso (item.Title.Equals("PSC IR Documents") OrElse item.Title.Equals("PSC RW Documents"))) Then
                    item.Visible = False
                ElseIf (associatedWorkFlow = 15 AndAlso (item.Title.Equals("PSC WD Documents") OrElse item.Title.Equals("PSC RW Documents"))) Then
                    item.Visible = False
                ElseIf (associatedWorkFlow = 30 AndAlso (item.Title.Equals("PSC WD Documents") OrElse item.Title.Equals("PSC IR Documents"))) Then
                    item.Visible = False
                End If

            Next

        End Sub

        Private Sub CheckInit()
            If (Not _inited) Then
                InitControl()
            End If
        End Sub

        Private Sub rptLinks_ItemDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLinks.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Then

                Dim link As LinkButton = CType(e.Item.FindControl("hlink"), LinkButton)

                If (link Is Nothing) Then
                    Exit Sub
                End If

                Dim curStep As TabItem = CType(e.Item.DataItem, TabItem)
                link.Text = curStep.Title

                e.Item.Visible = curStep.Visible

                'If (link.Text.Equals("Audit")) Then 'Hard Coded need to change
                '    e.Item.Visible = True
                'End If

                link.Attributes.Add("onclick", "navigateTab('" + curStep.Title + "'); return false;")

                If (curStep.Completed) Then
                    link.Enabled = True
                Else
                    link.Enabled = curStep.Enabled
                End If

                If (curStep.Order = CurrentStepId) Then
                    CType(e.Item.FindControl("Tab"), HtmlControls.HtmlControl).Attributes("class") = "active"
                    link.CssClass = "active"
                Else
                    CType(e.Item.FindControl("Tab"), HtmlControls.HtmlControl).Attributes("class") = "normal"
                End If

            End If
        End Sub

        Private Sub SetCurrentPage()

            If (Not _inited) Then
                InitControl()
            End If

            'set the current page, based on the request
            Dim file As String = Request.FilePath
            file = Request.FilePath.Substring(Request.FilePath.LastIndexOf("/") + 1)

            For Each row As TabItem In _steps
                If (row.Page.ToLower() = file.ToLower()) Then
                    _curIndex = row.Order - 1
                    Exit For
                End If
            Next

        End Sub

        ''' <summary>
        ''' When a page is loaded for the first time, this makes sure that this step
        ''' is actually enables.  Ensuring the user can't manually type in a url
        ''' and bypass steps
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub VerifyStepIsActive()

            If (Not _inited) Then
                InitControl()
            End If

            If (Not _steps(_curIndex).Enabled) Then

                If (_curIndex = 0) Then
                    'we are on the first page (which should always be enabled) so
                    'just set the current page to enabled and we're good
                    _steps(_curIndex).Enabled = True
                    Exit Sub
                End If

                'we somehow got to a page that is not yet enabled, so redirect to the last
                'enabled step
                For i As Int16 = _curIndex - 1 To 0 Step -1
                    If (_steps(i).Enabled) Then
                        'set our active step to this one
                        _curIndex = i
                        'and redirect to it
                        Response.Redirect(_steps(i).Page + Request.Url.Query)
                    End If
                Next

            End If

        End Sub

#End Region

        Protected Sub btnTabClick_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTabClick.Click

            Dim args As New TabNavigationEventArgs(NavigatorButtonType.NavigatedAway, GetPageUrl(txtTabName.Text))

            'raise a button event so the current page can save if it needs to
            If (RaiseNavigationEvent(args)) Then
                'we only navigate away of the above event handler returned true
                Response.Redirect(args.TargetUrl + Request.Url.Query)
                'MoveToPage(link.Text)
            End If

            'If (item IsNot Nothing) Then
            'MoveToStep(item)
            'End If

        End Sub

        Protected Sub WriteScript()

            Dim buffer As New StringBuilder()

            buffer.Append("<script type='text/javascript'>" + vbCrLf)
            buffer.Append("function navigateTab(cmd) {" + vbCrLf)

            Dim item As TabItem = CurrentStep()

            'if we have a validation routine for this page, add it here
            If (item.ClientScript.Length > 0) Then
                buffer.Append(" if (!" + item.ClientScript + "()) { return false; }" + vbCrLf)
            End If

            buffer.Append("element('" + Me.ID + "_txtTabName').value = cmd;" + vbCrLf)
            buffer.Append("element('" + Me.ID + "_btnTabClick').click();" + vbCrLf)
            buffer.Append("$.blockUI();" + vbCrLf)
            buffer.Append("}" + vbCrLf)
            buffer.Append("</script>" + vbCrLf)

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType().BaseType, "navigatorscript", buffer.ToString())

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            InitControl()
            SetCurrentPage()
            'WriteScript()

            If (Not IsPostBack) Then

                VerifyStepIsActive()

            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender

            rptLinks.Visible = True
            rptLinks.DataSource = _steps
            rptLinks.DataBind()

        End Sub

        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
            Commit()
        End Sub

#Region "Navigation button events"

        Public Function RaiseNavigationEvent(ByVal type As NavigatorButtonType) As Boolean

            If (type = NavigatorButtonType.Print) AndAlso (_config.PrintScript.Length > 0) Then
                RunStartupScript(Me.Page, "TabPrintScript", _config.PrintScript)
                Return False
            End If

            Dim args As New TabNavigationEventArgs(type)
            RaiseEvent ButtonClicked(Me, args)
            Return Not args.Cancel        'we not the cancel value to make this function more logical on return
        End Function

        Public Function RaiseNavigationEvent(ByVal args As TabNavigationEventArgs) As Boolean
            RaiseEvent ButtonClicked(Me, args)
            Return Not args.Cancel       'we not the cancel value to make this function more logical on return
        End Function

        Public Sub RenameAllSteps(sKeyWord As String, sReplacement As String)

            If (Not _inited) Then
                InitControl()
            End If

            For Each ti As TabItem In _steps
                ti.Title = ti.Title.Replace(sKeyWord, sReplacement)
            Next

        End Sub

        Protected Sub NavHeaderClicked(ByVal sender As Object, ByVal e As EventArgs)

            'our sender should be the link that was clicked
            Dim link As LinkButton = CType(sender, LinkButton)

            If (link Is Nothing) Then
                Exit Sub
            End If

            Dim args As New TabNavigationEventArgs(NavigatorButtonType.NavigatedAway, GetPageUrl(link.Text))

            'raise a button event so the current page can save if it needs to
            If (RaiseNavigationEvent(args)) Then
                'we only navigate away of the above event handler returned true
                Response.Redirect(args.TargetUrl + Request.Url.Query)
                'MoveToPage(link.Text)
            End If

            'otherwise, just stay where we are, any message to the effect that we didn't navigate away will be handled
            'by the displaying page, so we don't need to mess with it here.

        End Sub

#End Region

    End Class

End Namespace