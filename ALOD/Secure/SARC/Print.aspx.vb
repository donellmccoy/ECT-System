Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.SARC

    Public Class Print
        Inherits System.Web.UI.Page

        Private _documentDao As IDocumentDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _sarcDao As ISARCDAO
        Private _viewDocURL As String = String.Empty

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Public Property ViewDocURL As String
            Get
                Return _viewDocURL
            End Get
            Private Set(value As String)
                _viewDocURL = value
            End Set
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        Protected ReadOnly Property SarcDao() As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = New NHibernateDaoFactory().GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' Determine if the case's PDF should be grabbed from SRXLite or dynamically generated...
            Dim sarc As RestrictedSARC = SarcDao.GetById(ReferenceId)

            If (sarc Is Nothing) Then
                Exit Sub
            End If

            If (sarc.DocumentGroupId.HasValue) Then
                _documents = DocumentDao.GetDocumentsByGroupId(sarc.DocumentGroupId)
            End If

            If (Not UserHasPermission(PERMISSION_VIEW_SARC_CASES)) Then
                Exit Sub
            End If

            Dim javaScriptCall As String = ViewForms.RestrictedSARCFormPDFLinkAttribute(sarc, _documents)

            If (javaScriptCall.Contains("viewDoc")) Then
                ' Grab from SRXLite...
                ViewDocURL = Me.ResolveClientUrl(ExtractViewDocURL(javaScriptCall))
            Else
                ' Generate dynamically...
                Dim pdfFactory As PDFCreateFactory = New PDFCreateFactory()

                Dim doc As PDFDocument = pdfFactory.GeneratePdf(ReferenceId, ModuleType.SARC)

                If (doc Is Nothing) Then
                    Exit Sub
                End If

                doc.Render(Page.Response)
                doc.Close()

            End If
        End Sub

        Private Function ExtractViewDocURL(ByVal javaScriptCall As String) As String
            Dim url As String = String.Empty

            Dim startIndex As Integer = javaScriptCall.IndexOf("(") + 1
            Dim length As Integer = javaScriptCall.LastIndexOf(")") - startIndex

            Return javaScriptCall.Substring(startIndex, length)
        End Function

    End Class

End Namespace