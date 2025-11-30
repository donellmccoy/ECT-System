Option Strict On

Imports System.IO
Imports WebSupergoo.ABCpdf8
Imports SRXLite.Modules

Namespace Classes

    Public Class PDF
        Implements IDisposable

        Private _doc As Doc
        Private _officeDoc As Doc

#Region " Constructors "

        Public Sub New()
            _doc = New Doc()
            _officeDoc = New Doc()
        End Sub

#End Region

#Region " PageType enum "

        Public Enum PageType
            Image
            MSWord
            PDF
        End Enum

#End Region

#Region " AddBookmark "

        Public Sub AddBookmark(ByVal Path As String, ByVal Expanded As Boolean)
            _doc.AddBookmark(Path, False)
        End Sub

#End Region

#Region " AddCoverPage "

        Public Sub AddCoverPage(ByVal EntityID As String)
            _doc.Page = _doc.AddPage(1)

            With _doc
                .HPos = 0.5 'Center text
                .Font = .AddFont("Helvetica")
                .TextStyle.ParaSpacing = 20
                .FontSize = 36
                .AddText(vbCrLf & vbCrLf & "Document Archive" & vbCrLf)

                .FontSize = 18
                .AddText("for" & vbCrLf)
                .AddText(EntityID & vbCrLf)

                .VPos = 0.98
                .HPos = 0.5
                .AddHtml("<font color='gray'>Generated: " & Now().ToString("yyyyMMdd HH:mm ET") & "</font>")
            End With

            AddWatermark()
        End Sub

#End Region

#Region " AddPage "

        ''' <summary>
        ''' Adds a page and bookmark to the PDF file.
        ''' </summary>
        ''' <param name="fileBytes"></param>
        ''' <param name="type"></param>
        ''' <param name="DocTypeName"></param>
        ''' <param name="DocDate"></param>
        ''' <param name="PageNum"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddPage(
         ByVal fileBytes As Byte(),
         ByVal type As PageType,
         ByVal docTypeName As String,
         ByVal docDate As Date,
         ByVal pageNum As Integer) As Boolean

            Dim pagePath As String = docTypeName & "\" & docDate
            Dim bookmarkPath As String = pagePath & "\Page "

            Dim isAdded As Boolean = True
            _doc.Transform.Reset()

            Select Case type
                Case PageType.Image
                    Dim image As New XImage
                    Try
                        image.SetData(fileBytes)
                        _doc.MediaBox.String = "0 0 612 792"
                        _doc.Rect.String = _doc.MediaBox.String

                        If image.Width > image.Height Then
                            Dim AspectRatio As Double = image.Width / image.Height
                            Dim W As Integer = Math.Min(792, image.Width)
                            Dim H As Integer = Math.Min(612, CInt(Math.Floor(W / AspectRatio))) 'Scaled height

                            _doc.MediaBox.String = "0 0 792 612"
                            _doc.Rect.String = "0 0 " & W & " " & H
                        End If

                        _doc.Page = _doc.AddPage()  'Add new page
                        _doc.AddImageObject(image, False)
                        _doc.AddBookmark(bookmarkPath & pageNum, False)
                        AddWatermark()
                    Catch ex As Exception
                        isAdded = False
                    Finally
                        image.Dispose()
                    End Try

                Case PageType.PDF
                    Dim pdfDoc As New Doc

                    Try
                        Dim lastPageNumber As Integer = _doc.PageCount
                        pdfDoc.Read(fileBytes)

                        'Remove existing bookmarks
                        While pdfDoc.Bookmark.Count > 0
                            pdfDoc.Bookmark.RemoveAt(0)
                        End While

                        pdfDoc.Form.Stamp() 'Stamp form fields into document
                        pdfDoc.Flatten() 'Combine and compress layers
                        AddWatermarkToDoc(pdfDoc)

                        _doc.Append(pdfDoc) 'Append pages

                        _doc.PageNumber = lastPageNumber + 1
                        _doc.AddBookmark(pagePath, False)

                        'Add bookmark for each page
                        For i As Integer = 1 To pdfDoc.PageCount
                            _doc.PageNumber = lastPageNumber + i
                            _doc.AddBookmark(bookmarkPath & i, False)
                        Next
                    Catch ex As Exception
                        isAdded = False
                    Finally
                        pdfDoc.Clear()
                        pdfDoc.Dispose()
                    End Try

                Case Else
                    isAdded = False
            End Select

            Return isAdded
        End Function

#End Region

#Region " AddWatermark "

        Public Sub AddWatermark(ByRef pdfDoc As Doc, ByVal text As String)
            'Add watermark to current page
            With pdfDoc
                .VPos = 0
                .HPos = 0
                .Font = .AddFont("Helvetica")
                .Pos.X = .MediaBox.Width
                .Pos.Y = (.MediaBox.Height / 2) - 175
                .FontSize = 62
                .TextStyle.Bold = True
                .TextStyle.Italic = True
                .AddHtml("<font color='#FFC0CB'>" & text & "</font>")
                .Transform.Reset()
            End With
        End Sub

        Private Sub AddWatermark()
            AddWatermark(_doc)
        End Sub

        Private Sub AddWatermark(ByRef pdfDoc As Doc)
            If AppSettings.Environment.IsProd() Then Exit Sub
            If ConfigurationManager.AppSettings("WaterMark") <> "" Then
                AddWatermark(pdfDoc, ConfigurationManager.AppSettings("WaterMark"))
            Else
            End If
        End Sub

#End Region

#Region " AddWatermarkToDoc "

        Public Sub AddWatermarkToDoc(ByVal text As String)
            For i As Integer = 1 To _doc.PageCount
                _doc.PageNumber = i
                AddWatermark(_doc, text)
            Next
        End Sub

        Public Sub AddWatermarkToDoc(ByRef pdfDoc As Doc)
            For i As Integer = 1 To pdfDoc.PageCount
                pdfDoc.PageNumber = i
                AddWatermark(pdfDoc)
            Next
        End Sub

#End Region

#Region " GetBytes "

        Public Function GetBytes() As Byte()
            Return _doc.GetData
        End Function

#End Region

#Region " Render "

        ''' <summary>
        ''' Render the final PDF document to the client response stream
        ''' </summary>
        ''' <param name="response"></param>
        ''' <remarks></remarks>
        Public Sub Render(ByVal response As HttpResponse)
            response.ClearHeaders()
            response.ClearContent()
            response.ContentType = "application/pdf"
            response.OutputStream.Write(_doc.GetData, 0, _doc.GetData.Length)
            response.Flush()
            response.Close()
        End Sub

        ''' <summary>
        ''' Render the final PDF document to a file
        ''' </summary>
        ''' <param name="fileName">The name of the file to write to</param>
        ''' <remarks></remarks>
        Public Sub Render(ByVal fileName As String)
            Using outStream As New FileStream(fileName, FileMode.Create)
                outStream.Write(_doc.GetData, 0, _doc.GetData.Length())
            End Using
        End Sub

#End Region

#Region " IDisposable Support "

        Private disposedValue As Boolean = False 'To detect redundant calls

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free managed resources when explicitly called
                End If

                ' TODO: free shared unmanaged resources
                If _doc IsNot Nothing Then _doc.Dispose()
                If _officeDoc IsNot Nothing Then _officeDoc.Dispose()

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace