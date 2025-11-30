Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.SARC

    Public Class Search
        Inherits System.Web.UI.Page

#Region "Fields..."

        Private _daoFactory As NHibernateDaoFactory
        Private _documentsDao As IDocumentDao
        Private _lodDao As ILineOfDutyDao
        Private _sarcDao As ISARCDAO

#End Region

#Region "Properties..."

        Protected ReadOnly Property DAOFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentsDao Is Nothing) Then
                    _documentsDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentsDao
            End Get
        End Property

        Protected ReadOnly Property LODDao As ILineOfDutyDao
            Get
                If (_lodDao Is Nothing) Then
                    _lodDao = DAOFactory.GetLineOfDutyDao()
                End If

                Return _lodDao
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DAOFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected ReadOnly Property SARCModuleType() As Byte
            Get
                Return ModuleType.SARC
            End Get
        End Property

        Protected Function GetInitURL(ByVal moduleId As Integer) As String
            If (moduleId = ModuleType.SARC) Then
                Return "~/Secure/SARC/init.aspx"
            ElseIf (moduleId = ModuleType.LOD) Then
                Return "~/Secure/lod/init.aspx"
            End If

            Return String.Empty
        End Function

        Protected Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")

                If (parts.Count = 0) Then
                    Exit Sub
                End If

                Dim refId As Integer = CInt(parts(0))
                Dim moduleId As Integer = CInt(parts(1))
                Dim strQuery As New StringBuilder()
                Dim initURL As String = String.Empty

                If (refId <= 0) Then
                    Exit Sub
                End If

                If (moduleId <= 0) Then
                    Exit Sub
                End If

                initURL = GetInitURL(moduleId)

                If (initURL.Equals(String.Empty)) Then
                    Exit Sub
                End If

                strQuery.Append("?refId=" + refId.ToString())

                Response.Redirect(initURL & strQuery.ToString())
            End If
        End Sub

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

                Dim refID As Integer = data("RefId")
                Dim moduleId As Integer = data("ModuleId")

                If (moduleId = ModuleType.SARC) Then
                    InitView348RImageButton(data("RefId"), e)
                ElseIf (moduleId = ModuleType.LOD) Then
                    InitView348ImageButton(data("RefId"), e)
                End If
            End If
        End Sub

        Protected Sub InitInputRestrictions()
            SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
        End Sub

        Protected Sub InitStatusLookupControl()
            StatusSelect.DataSource = LookupService.GetStatusCodesByModule(SARCModuleType)
            StatusSelect.DataValueField = "Id"
            StatusSelect.DataTextField = "Description"
            StatusSelect.DataBind()
            Utility.InsertDropDownListZeroValue(StatusSelect, "-- All --")
        End Sub

        Protected Sub InitUnitLookupControl()
            Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
            RegisterAsyncTask(task)
        End Sub

        Protected Sub InitView348ImageButton(ByVal refId As Integer, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim printImageButton As ImageButton = CType(e.Row.FindControl("PrintImage"), ImageButton)

            If (refId <= 0) Then
                printImageButton.Visible = False
                Exit Sub
            End If

            Dim lod As LineOfDuty = LODDao.GetById(refId)
            Dim _documents As IList(Of ALOD.Core.Domain.Documents.Document) = Nothing

            If (lod.DocumentGroupId.HasValue) Then
                _documents = DocumentDao.GetDocumentsByGroupId(lod.DocumentGroupId)
            End If

            printImageButton.OnClientClick = "" & ViewForms.LinkAttribute348(refId, _documents, "lod") & ""

            If (lod.WorkflowStatus.StatusCodeType.IsFinal) Then
                printImageButton.AlternateText = "Print Final Form"
            Else
                printImageButton.AlternateText = "Print Draft Form"
            End If
        End Sub

        Protected Sub InitView348RImageButton(ByVal refId As Integer, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim printImageButton As ImageButton = CType(e.Row.FindControl("PrintImage"), ImageButton)

            If (refId <= 0) Then
                printImageButton.Visible = False
                Exit Sub
            End If

            Dim sarc As RestrictedSARC = SARCDao.GetById(refId)
            Dim _documents As IList(Of ALOD.Core.Domain.Documents.Document) = Nothing

            If (sarc.DocumentGroupId.HasValue) Then
                _documents = DocumentDao.GetDocumentsByGroupId(sarc.DocumentGroupId)
            End If

            printImageButton.OnClientClick = "" & ViewForms.RestrictedSARCFormPDFLinkAttribute(sarc, _documents) & ""

            If (sarc.WorkflowStatus.StatusCodeType.IsFinal) Then
                printImageButton.AlternateText = "Print Final Form"
            Else
                printImageButton.AlternateText = "Print Draft Form"
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitInputRestrictions()
                InitStatusLookupControl()
                InitUnitLookupControl()
                PreloadSearchFilters()
                gvResults.Sort("DaysCompleted", SortDirection.Descending)
            End If
        End Sub

        Protected Sub SARCData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SARCData.Selecting
            NoFiltersPanel.Visible = False

            If (SsnBox.Text.Trim.Length = 0 AndAlso
                NameBox.Text.Trim.Length = 0 AndAlso
                CaseIdbox.Text.Trim.Length = 0 AndAlso
                StatusSelect.SelectedIndex = 0 AndAlso
                UnitSelect.SelectedIndex < 1) Then
                NoFiltersPanel.Visible = True
                e.Cancel = True
                Exit Sub
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            gvResults.DataBind()
        End Sub

        Private Sub PreloadSearchFilters()
            If (Request.QueryString("data") Is Nothing) Then
                Exit Sub
            End If

            Dim data As String = Request.QueryString("data")

            If (IsNumeric(data)) Then
                If (data.Length = 4) Then
                    SsnBox.Text = data
                Else
                    CaseIdbox.Text = data
                End If
            Else

                If (IsValidCaseId(data)) Then
                    CaseIdbox.Text = data
                Else
                    NameBox.Text = data
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace