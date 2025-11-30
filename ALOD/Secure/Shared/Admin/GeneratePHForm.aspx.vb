Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports WebSupergoo.ABCpdf8

Namespace Web.Admin

    Public Class GeneratePHForm
        Inherits System.Web.UI.Page

        Private _docDao As IDocumentDao = Nothing
        Private _specCase As SpecialCase = Nothing
        Private _specCaseDao As ISpecialCaseDAO = Nothing

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(SESSION_USERNAME)
                End If

                Return _docDao
            End Get
        End Property

        ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected Property SpecCase As SpecialCase
            Get
                Return _specCase
            End Get
            Set(value As SpecialCase)
                _specCase = value
            End Set
        End Property

        Protected Sub btnSubmin_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

            ' Display results panel
            pnlResults.Visible = True

            lblResults.CssClass = "highlightFailure"

            ' Validate the input...
            If (Not ValidateInput()) Then
                lblResults.Text = "Invalid Input! Please enter a valid Case ID."
                Exit Sub
            End If

            Dim caseId As String = Server.HtmlEncode(txtCaseId.Text)

            ' Load the case using the case id
            SpecCase = SpecCaseDao.GetSpecialCaseByCaseId(caseId)

            ' Check if the case was found...
            If (SpecCase Is Nothing) Then
                lblResults.Text = "Forms NOT Generated! Could not find a case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            ' Check if the case is in progress or has been cancelled...
            If (SpecCase.Status <> ALOD.Core.Domain.Modules.Lod.SpecCasePHWorkStatus.Complete) Then
                If (SpecCase.Status = ALOD.Core.Domain.Modules.Lod.SpecCasePHWorkStatus.Cancelled) Then
                    lblResults.Text = "Form NOT Generated! The case with an ID of <b>" & caseId & "</b> has been cancelled."
                Else
                    lblResults.Text = "Form NOT Generated! The case with an ID of <b>" & caseId & "</b> has not yet been completed."
                End If

                Exit Sub
            End If

            ' Generate the new PDF document...
            Dim doc As PDFDocument = New PDFDocument()
            Dim docId As Integer = 0

            If (doc Is Nothing) Then
                lblResults.Text = "Forms NOT Generated! Failed to create the PDF document for the case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            doc.SetRenderingEngine(EngineType.Gecko)
            doc.AddWebPage(Server, "~/Secure/SC_PH/PrintableForm.aspx?refId=" & SpecCase.Id, 1)
            doc.AddPageNumbers(0.47, 0.015)
            doc.IncludeFOUOWatermark = False

            ' Archive the document in SRXLite...
            docId = ArchiveDocument(doc)

            If (docId = 0) Then
                lblResults.Text = "Forms NOT Generated! Failed to archive the PDF document for the case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            LogManager.LogAction(ModuleType.SpecCasePH, UserAction.AddedDocument, SpecCase.Id, "Generated PH Form PDF")

            ' Display results
            lblResults.Text = "Form Successfully Generated and Archived for the case with an ID of <b>" & caseId & "</b>."
            lblResults.CssClass = "highlightSuccess"

            ' Setup case link
            lnkCaseId.Visible = True
            lnkCaseId.Text = caseId
            lnkCaseId.NavigateUrl = "~/Secure/SC_PH/init.aspx?refId=" & SpecCase.Id.ToString()

            ' Setup document
            Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx") +
                                                    "?docId=" + docId.ToString() +
                                                    "&modId=" + SpecCase.moduleId.ToString() +
                                                    "&refId=" + SpecCase.Id.ToString()

            imgbCaseDocument.OnClientClick = "viewDoc('" & url & "'); return false;"
            imgbCaseDocument.AlternateText = "Print Final Forms"
            imgbCaseDocument.Visible = True
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        ' Stores the PDF document into SRXLite
        Private Function ArchiveDocument(ByVal doc As PDFDocument) As Integer
            If (doc Is Nothing) Then
                Return 0
            End If

            Try
                Dim description As String = ""
                Dim groupID As Long = SpecCase.DocumentGroupId
                Dim fileName As String = "PH Form-Case:" & SpecCase.CaseId.ToString() & "-Generated.pdf"

                Dim docMetaData As Document = New Document()
                docMetaData.DateAdded = DateTime.Now
                docMetaData.DocDate = DateTime.Now
                docMetaData.Description = "PH Form" & description & "Generated for Case Id: " & SpecCase.CaseId.ToString()
                docMetaData.DocStatus = DocumentStatus.Approved
                docMetaData.Extension = "pdf"
                docMetaData.SSN = SpecCase.DocumentEntityId
                docMetaData.DocType = DocumentType.PHForm

                doc.Render(fileName)

                Return DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)
            Catch ex As Exception
                LogManager.LogError(ex)
                Return 0
            End Try
        End Function

        ' Performs server-side input validation
        Private Function ValidateInput() As Boolean
            ' Check if the case ID is in the proper format...
            Dim regex As Regex = New Regex(
                    "\d{8}-\d{3}-PH",
                    RegexOptions.IgnoreCase _
                    Or RegexOptions.Singleline _
                    Or RegexOptions.CultureInvariant _
                    Or RegexOptions.Compiled)

            Return regex.IsMatch(Server.HtmlEncode(txtCaseId.Text))
        End Function

    End Class

End Namespace