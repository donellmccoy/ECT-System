Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports WebSupergoo.ABCpdf8

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_PrintTracking
        Inherits System.Web.UI.Page

        Protected ReadOnly Property userId() As String
            Get
                Return CInt(Request.QueryString("id"))
            End Get
        End Property

        Public Overrides Sub VerifyRenderingInServerForm(ByVal ctrl As Control)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim changes As New ChangeSet()
            changes.GetByUserID(userId)

            Dim pdf As New Doc()

            'we need to be in landscape to allow as much width as possible
            'apply a rotation transform
            Dim w As Double = pdf.MediaBox.Width
            Dim h As Double = pdf.MediaBox.Height
            Dim l As Double = pdf.MediaBox.Left
            Dim b As Double = pdf.MediaBox.Bottom
            pdf.Transform.Rotate(90, l, b)
            pdf.Transform.Translate(w, 0)

            ' rotate our rectangle
            pdf.Rect.Width = h
            pdf.Rect.Height = w

            'rotate the page
            Dim docId As Integer = pdf.GetInfoInt(pdf.Root, "Pages")
            pdf.SetInfo(docId, "/Rotate", "90")

            pdf.FontSize = 10
            pdf.Rect.Inset(20, 40)

            Dim table As New PdfTable(pdf, 6)
            table.CellPadding = 5
            table.RepeatHeader = True

            Dim i As Integer = 0
            Dim page As Integer = 1
            Dim shade As Boolean = False

            'header row
            table.NextRow()
            Dim header As String() = {"Date", "Action", "Setting", "Old Value", "New Value", "User"}
            table.AddHtml(header)

            For Each row As ChangeRow In changes

                table.NextRow()

                Dim cols(5) As String
                cols(0) = row.ActionDate.ToString(DATE_HOUR_FORMAT)
                cols(1) = row.ActionName
                cols(2) = row.Field
                cols(3) = row.OldVal
                cols(4) = row.NewVal
                cols(5) = row.UserName
                table.AddHtml(cols)

                If (pdf.PageNumber > page) Then
                    page = pdf.PageNumber
                    shade = True
                End If

                If (shade) Then
                    table.FillRow("216 216 255", table.Row)
                End If

                shade = Not shade
                i = i + 1

            Next

            pdf.VPos = 0.5

            Dim userDao As IUserDao = New NHibernateDaoFactory().GetUserDao()
            Dim user As AppUser = userDao.GetById(userId)

            Dim userName As String = user.LastName + ", " + user.FirstName

            If (Not String.IsNullOrEmpty(user.MiddleName)) Then
                userName += " " + user.MiddleName
            End If

            If (user.SSN IsNot Nothing AndAlso user.SSN.Trim.Length = 9) Then
                userName += " (" + user.SSN.Substring(5) + ")"
            End If

            For ct As Integer = 1 To pdf.PageCount

                pdf.PageNumber = ct

                'left side
                pdf.HPos = 0.0
                pdf.Rect.SetRect(20, 580, 280, 20)
                pdf.AddText(userName)

                'middle
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 580, 750, 20)
                pdf.AddText("User Account History")

                'right
                pdf.HPos = 1.0
                pdf.Rect.SetRect(500, 580, 270, 20)
                pdf.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT))

                'page number
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 20, 750, 20)
                pdf.AddText("Page " + ct.ToString() + " of " + pdf.PageCount.ToString())

                'table header
                pdf.AddLine(20, 552, 772, 552)

            Next

            pdf.Flatten()
            Dim data() As Byte = pdf.GetData()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "inline; filename=Tracking.PDF")
            Response.AddHeader("content-length", data.Length.ToString())
            Response.BinaryWrite(data)
            Response.End()

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        End Sub

    End Class

End Namespace