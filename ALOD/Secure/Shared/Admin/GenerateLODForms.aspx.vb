Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.Admin

    Public Class GenerateLODForms
        Inherits System.Web.UI.Page

        Private _docDao As IDocumentDao = Nothing
        Private _lod As LineOfDuty = Nothing
        Private _lodSearchDao As ILodSearchDao = Nothing

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(SESSION_USERNAME)
                End If

                Return _docDao
            End Get
        End Property

        ReadOnly Property LODSearchDao() As ILodSearchDao
            Get
                If (_lodSearchDao Is Nothing) Then
                    _lodSearchDao = New NHibernateDaoFactory().GetLodSearchDao()
                End If

                Return _lodSearchDao
            End Get
        End Property

        Protected Property LOD As LineOfDuty
            Get
                Return _lod
            End Get
            Set(value As LineOfDuty)
                _lod = value
            End Set
        End Property

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
            LOD = LODSearchDao.GetByCaseId(caseId)

            ' Check if the case was found...
            If (LOD Is Nothing) Then
                lblResults.Text = "Forms NOT Generated! Could not find a case with an ID of <b>" & caseId & "</b>."
                Exit Sub
            End If

            ' Check if the case is in progress or has been cancelled...
            If (LOD.CurrentStatusCode <> LodStatusCode.Complete) Then
                If (LOD.CurrentStatusCode = LodStatusCode.Cancelled) Then
                    lblResults.Text = "Forms NOT Generated! The case with an ID of <b>" & caseId & "</b> has been cancelled."
                Else
                    lblResults.Text = "Forms NOT Generated! The case with an ID of <b>" & caseId & "</b> has not yet been completed."
                End If

                Exit Sub
            End If

            ' Generate the new PDF document...
            Dim SavePDF As New PDFCreateFactory()
            Dim doc As PDFDocument = Nothing
            Dim docId As Integer = 0

            doc = SavePDF.GeneratePdf(LOD.Id, LOD.ModuleType, chkIOSig.Checked)

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

            LogManager.LogAction(ModuleType.LOD, UserAction.AddedDocument, LOD.Id, "Generated Form 348 PDF")

            ' Display results
            lblResults.Text = "Forms Successfully Generated and Archived for the case with an ID of <b>" & caseId & "</b>."
            lblResults.CssClass = "highlightSuccess"

            ' Setup case link
            lnkCaseId.Visible = True
            lnkCaseId.Text = caseId
            lnkCaseId.NavigateUrl = "~/Secure/Lod/init.aspx?refId=" & LOD.Id.ToString()

            ' Setup document
            Dim url348 As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" & docId & "&modId=" & ModuleType.LOD)
            imgbCaseDocument.OnClientClick = "viewDoc('" & url348 & "'); return false;"
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

            Dim description As String = ""
            Dim file216 As String = ""
            Dim docId As Long = 0

            Try
                'check for Formal investigation
                If (LOD.Formal) Then
                    '  is261 = True
                    description = "and Form261 "
                    file216 = "_Form261"

                End If

                Dim groupID As Long = LOD.DocumentGroupId
                Dim fileName As String = "Form348" & file216 & "-Case:" & LOD.CaseId.ToString() & "-Generated.pdf"

                Dim docMetaData As Document = New Document()
                docMetaData.DateAdded = DateTime.Now
                docMetaData.DocDate = DateTime.Now
                docMetaData.Description = "Form348 " & description & "Generated for Case Id: " & LOD.CaseId.ToString()
                docMetaData.DocStatus = DocumentStatus.Approved
                docMetaData.Extension = "pdf"
                docMetaData.SSN = LOD.MemberSSN
                docMetaData.DocType = DocumentType.FinalForm348

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
                    "\d{8}-\d{3}(-[A-Z]{1})*",
                    RegexOptions.IgnoreCase _
                    Or RegexOptions.Singleline _
                    Or RegexOptions.CultureInvariant _
                    Or RegexOptions.Compiled)

            Return regex.IsMatch(Server.HtmlEncode(txtCaseId.Text))
        End Function

    End Class

End Namespace