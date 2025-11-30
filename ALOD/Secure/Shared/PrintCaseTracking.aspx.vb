Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports WebSupergoo.ABCpdf8

Public Class PrintCaseTracking1
    Inherits System.Web.UI.Page

    Private _daoFactory As IDaoFactory
    Private _workflowDao As IWorkflowDao

    Protected ReadOnly Property DaoFactory As IDaoFactory
        Get
            If (_daoFactory Is Nothing) Then
                _daoFactory = New NHibernateDaoFactory()
            End If

            Return _daoFactory
        End Get
    End Property

    Protected ReadOnly Property WorkflowDao As IWorkflowDao
        Get
            If (_workflowDao Is Nothing) Then
                _workflowDao = DaoFactory.GetWorkflowDao()
            End If

            Return _workflowDao
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim case_module As Byte = CByte(Request.QueryString("module"))
        Dim refId As Integer = CInt(Request.QueryString("refId"))
        Dim showAll As Boolean = Boolean.Parse(Request.QueryString("showAll"))
        Dim CaseId As String = Request.QueryString("CaseId")

        'get the data
        Dim data As IEnumerable(Of ALOD.Core.Domain.Workflow.WorkStatusTracking) = LookupService.GetWorkStatusTracking(refId, case_module)

        'set up our pdf document
        Dim doc As New Doc()

        'apply a rotation transform
        Dim w As Double = doc.MediaBox.Width
        Dim h As Double = doc.MediaBox.Height
        Dim l As Double = doc.MediaBox.Left
        Dim b As Double = doc.MediaBox.Bottom
        doc.Transform.Rotate(90, l, b)
        doc.Transform.Translate(w, 0)

        ' rotate our rectangle
        doc.Rect.Width = h
        doc.Rect.Height = w

        'rotate the page
        Dim docId As Integer = doc.GetInfoInt(doc.Root, "Pages")
        doc.SetInfo(docId, "/Rotate", "90")

        doc.FontSize = 10
        doc.Rect.Inset(20, 40)

        Dim table As New PdfTable(doc, 5)
        table.CellPadding = 5
        table.RepeatHeader = True

        Dim i As Integer = 0
        Dim page As Integer = 1
        Dim shade As Boolean = False

        'header row
        table.NextRow()
        Dim header As String() = {"Process Name", "Start Date", "End Date", "Days in Process", "Completed By"}
        table.AddHtml(header)

        For Each row As ALOD.Core.Domain.Workflow.WorkStatusTracking In data

            table.NextRow()

            Dim cols(4) As String
            cols(0) = row.WorkflowStatus.Description
            cols(1) = row.StartDate.ToString(DATE_HOUR_FORMAT)

            If (Not row.EndDate Is Nothing) Then
                cols(2) = row.EndDate.Value.ToString(DATE_HOUR_FORMAT)
            Else
                cols(2) = ""
            End If

            cols(3) = row.DaysInStep.TotalDays.ToString("N2")

            If (Not row.CompletedBy Is Nothing) Then
                cols(4) = GetUserName(row.CompletedBy)
            Else
                cols(4) = ""
            End If
            table.AddHtml(cols)

            If (doc.PageNumber > page) Then
                page = doc.PageNumber
                shade = True
            End If

            If (shade) Then
                table.FillRow("216 216 255", table.Row)
            End If

            shade = Not shade
            i = i + 1

        Next

        Dim title As String = WorkflowDao.GetCaseType(case_module)

        For ct As Integer = 1 To doc.PageCount

            doc.PageNumber = ct

            'left side
            doc.HPos = 0.0
            doc.Rect.SetRect(20, 580, 280, 20)
            doc.AddText(CaseId + " - " + title)

            'middle
            doc.HPos = 0.5
            doc.Rect.SetRect(20, 580, 750, 20)
            doc.AddText("Case Tracking")

            'right
            doc.HPos = 1.0
            doc.Rect.SetRect(500, 580, 270, 20)
            doc.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT))

            'page number
            doc.HPos = 0.5
            doc.Rect.SetRect(20, 20, 750, 20)
            doc.AddText("Page " + ct.ToString() + " of " + doc.PageCount.ToString())

            'table header
            doc.AddLine(20, 552, 772, 552)

        Next

        doc.Flatten()

        'send the output to the client
        Dim theData() As Byte = doc.GetData()

        Response.ContentType = "application/pdf"
        Response.AddHeader("content-disposition", "inline; filename=Tracking.PDF")
        Response.AddHeader("content-length", theData.Length.ToString())
        Response.BinaryWrite(theData)
        Response.End()

    End Sub

End Class