Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.Admin

    Public Class GenerateSARCForms
        Inherits System.Web.UI.Page

        Private _docDao As IDocumentDao = Nothing
        Private _sarc As RestrictedSARC = Nothing
        Private _sarcDao As ISARCDAO = Nothing

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(SESSION_USERNAME)
                End If

                Return _docDao
            End Get
        End Property

        Protected Property SARC As RestrictedSARC
            Get
                Return _sarc
            End Get
            Set(value As RestrictedSARC)
                _sarc = value
            End Set
        End Property

        Protected ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = New NHibernateDaoFactory().GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        ' Stores the PDF document into SRXLite
        Protected Function ArchiveDocument(ByVal doc As PDFDocument) As Long
            Try
                If (doc Is Nothing) Then
                    Return 0
                End If

                Dim groupID As Long = SARC.DocumentGroupId
                Dim fileName As String = "Form348-R_Case:" & SARC.CaseId.ToString() & "-Generated.pdf"
                Dim docMetaData As Document = New Document()
                Dim docId As Long = 0

                docMetaData.DateAdded = DateTime.Now
                docMetaData.DocDate = DateTime.Now
                docMetaData.Description = "Form348-R Generated for Case Id: " & SARC.CaseId.ToString()
                docMetaData.DocStatus = DocumentStatus.Approved
                docMetaData.Extension = "pdf"
                docMetaData.SSN = SARC.DocumentEntityId
                docMetaData.DocType = DocumentType.Form348R

                doc.Render(fileName)

                Return DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)
            Catch ex As Exception
                LogManager.LogError(ex)
                Return 0
            End Try
        End Function

        Protected Sub btnSubmin_Click(sender As Object, e As EventArgs) Handles btnSubmin.Click

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
            SARC = SARCDao.GetByCaseId(caseId)

            ' Check if the case was found...
            If (SARC Is Nothing) Then
                lblResults.Text = "Forms NOT Generated! Could not find a case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            ' Check if the case is in progress or has been cancelled...
            If (SARC.Status <> SARCRestrictedWorkStatus.SARCComplete) Then
                If (SARC.Status = SARCRestrictedWorkStatus.SARCCancelled) Then
                    lblResults.Text = "Forms NOT Generated! The case with an ID of <b>" & caseId & "</b> has been cancelled."
                Else
                    lblResults.Text = "Forms NOT Generated! The case with an ID of <b>" & caseId & "</b> has not yet been completed."
                End If

                Exit Sub
            End If

            ' Generate the new PDF document...
            Dim pdfFactory As New PDFCreateFactory()
            Dim doc As PDFDocument = Nothing
            Dim docId As Integer = 0

            doc = pdfFactory.GenerateRestrictedSARCDocument(SARC.Id)

            If (doc Is Nothing) Then
                lblResults.Text = "Forms NOT Generated! Failed to create the PDF document for the case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            ' Archive the document in SRXLite...
            docId = ArchiveDocument(doc)

            If (docId = 0) Then
                lblResults.Text = "Forms NOT Generated! Failed to archive the PDF document for the case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            LogManager.LogAction(ModuleType.SARC, UserAction.AddedDocument, SARC.Id, "Generated Form 348 PDF")

            ' Display results
            lblResults.Text = "Forms Successfully Generated and Archived for the case with an ID of <b>" & caseId & "</b>."
            lblResults.CssClass = "highlightSuccess"

            ' Setup case link
            lnkCaseId.Visible = True
            lnkCaseId.Text = caseId
            lnkCaseId.NavigateUrl = "~/Secure/SARC/init.aspx?refId=" & SARC.Id.ToString()

            ' Setup document
            Dim docViewerURL As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" & docId & "&modId=" & ModuleType.SARC)
            imgbCaseDocument.OnClientClick = "viewDoc('" & docViewerURL & "'); return false;"
            imgbCaseDocument.AlternateText = "Print Final Forms"

            If (AppMode = DeployMode.Production) Then
                imgbCaseDocument.Visible = False 'System Admins can't view the form...only redacted documents
            Else
                imgbCaseDocument.Visible = True
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        ' Performs server-side input validation
        Private Function ValidateInput() As Boolean
            ' Check if the case ID is in the proper format...
            Dim regex As Regex = New Regex(
                    "\d{8}-\d{3}-SA",
                    RegexOptions.IgnoreCase _
                    Or RegexOptions.Singleline _
                    Or RegexOptions.CultureInvariant _
                    Or RegexOptions.Compiled)

            Return regex.IsMatch(Server.HtmlEncode(txtCaseId.Text))
        End Function

    End Class

End Namespace