Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow

Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports WebSupergoo.ABCpdf8

Namespace Web.Memos

    Partial Class Secure_Shared_Memos_ViewPdf
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim memoId As Integer = 0
            Dim refId As Integer = 0
            Dim moduleId As Byte = ModuleType.LOD

            If (Request.QueryString("memo") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("memo"), memoId)
            End If

            If (Request.QueryString("id") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("id"), refId)
            End If

            If (Request.QueryString("mod") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("mod"), moduleId)
            End If

            If (refId = 0 OrElse memoId = 0) Then
                Throw New ArgumentException("refId is empty")
            End If

            Dim factory As IDaoFactory = New NHibernateDaoFactory()
            Dim dao As IMemoDao = factory.GetMemoDao()
            Dim rdao As IMemoDao2 = New NHibernateDaoFactory().GetMemoDao2()
            Dim pdf As Doc

            If moduleId = 2 Then
                Dim memo As Memorandum = dao.GetById(memoId)
                LogManager.LogAction(moduleId, UserAction.ViewDocument, refId, "Memo: " + memo.Template.Title)
                pdf = memo.ToPdf(New NHibernateDaoFactory())
            ElseIf moduleId = 30 Then
                Dim memo As Memorandum2 = rdao.GetById(memoId)
                LogManager.LogAction(moduleId, UserAction.ViewDocument, refId, "Memo: " + memo.Template.Title)
                pdf = memo.ToPdf(New NHibernateDaoFactory())
            Else
                Dim memo2 As Memorandum2 = rdao.GetById(memoId)
                LogManager.LogAction(moduleId, UserAction.ViewDocument, refId, "Memo: " + memo2.Template.Title)
                pdf = memo2.ToPdf(New NHibernateDaoFactory())
            End If

            'memo.ToPdf().Render(Page.Response)

            If (pdf IsNot Nothing) Then
                pdf.Flatten()
                Dim theData() As Byte = pdf.GetData()
                pdf.Clear()

                Response.ContentType = "application/pdf"
                Response.AddHeader("content-disposition", "inline; filename=output.PDF")
                Response.AddHeader("content-length", theData.Length.ToString())
                Response.BinaryWrite(theData)
                Response.End()
            End If

        End Sub

    End Class

End Namespace