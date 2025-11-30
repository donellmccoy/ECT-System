Imports System.Configuration
Imports System.IO
Imports System.Web
Imports ALOD.Core.Utils
Imports WebSupergoo.ABCpdf8

Namespace Printing

    Public Class PDFDocument
        Implements IDisposable

        Private _doc As Doc = Nothing
        Private _includeFOUOWatermark As Boolean
        Private _includeWSCompleteWaterMark As Boolean
        Private _nullWatermarkBody As List(Of PDFString) = Nothing
        Private _watermark As String = String.Empty

        Public Sub New()

            _doc = New Doc()
            _doc.SaveOptions.Linearize = True
            _watermark = ConfigurationManager.AppSettings("WaterMark")
            _nullWatermarkBody = New List(Of PDFString)()
            _includeFOUOWatermark = True

        End Sub

        Public Property IncludeFOUOWatermark As Boolean
            Get
                Return _includeFOUOWatermark
            End Get
            Set(value As Boolean)
                _includeFOUOWatermark = value
            End Set
        End Property

        Public Property IncludeWSCompleteWaterMark As Boolean
            Get
                Return _includeWSCompleteWaterMark
            End Get
            Set(value As Boolean)
                _includeWSCompleteWaterMark = value
            End Set
        End Property

        Public Property WaterMark() As String
            Get
                Return _watermark
            End Get
            Set(ByVal value As String)
                _watermark = value
            End Set
        End Property

        Public Sub AddForm(ByVal form As PDFForm)

            _doc.Append(form.Document)

        End Sub

        Public Sub AddNullWatermarkString(ByVal str As PDFString)
            If (_nullWatermarkBody Is Nothing OrElse str Is Nothing) Then
                Exit Sub
            End If

            _nullWatermarkBody.Add(str)
        End Sub

        Public Sub AddPageNumbers(ByVal widthPositionRatio As Double, ByVal heightPositionRatio As Double)
            ' Apply text to each page...
            For i As Integer = 1 To _doc.PageCount
                _doc.PageNumber = i
                _doc.Pos.X = (_doc.MediaBox.Width * widthPositionRatio)
                _doc.Pos.Y = (_doc.MediaBox.Height * heightPositionRatio)
                _doc.AddText("Page " & i)
                _doc.Flatten()
            Next
        End Sub

        Public Sub AddStamp(ByVal stampLines As List(Of PDFString), ByVal firstPageOnly As Boolean, ByVal widthRatio As Double, ByVal heightRatio As Double, ByVal tabSize As Double)
            If (stampLines Is Nothing OrElse stampLines.Count = 0) Then
                Exit Sub
            End If

            Dim numPages As Integer = 0

            If (firstPageOnly) Then
                numPages = 1
            Else
                numPages = _doc.PageCount
            End If

            For i As Short = 1 To numPages
                _doc.PageNumber = i

                _doc.Pos.Y = _doc.MediaBox.Height * heightRatio

                Dim tabAmount As Integer = 0

                For Each s In stampLines
                    If (Not String.IsNullOrEmpty(s.Text)) Then
                        _doc.Pos.X = (_doc.MediaBox.Width * widthRatio) + tabAmount

                        _doc.AddHtml(s.GetHTML())

                        tabAmount = tabAmount + tabSize
                    End If
                Next
            Next
        End Sub

        Public Sub AddWebPage(ByVal server As System.Web.HttpServerUtility, ByVal url As String, ByVal startPage As Integer)
            If (startPage < 1 OrElse startPage > _doc.PageCount) Then
                startPage = 1
            End If

            Dim renderedPageContents As String = String.Empty

            Using writer As New StringWriter()
                server.Execute(url, writer, False)
                renderedPageContents = writer.ToString()
            End Using

            Dim chainId As Integer = 0

            chainId = _doc.AddImageHtml(renderedPageContents)

            While True
                _doc.FrameRect()

                If (Not _doc.Chainable(chainId)) Then
                    Exit While
                End If

                _doc.Page = _doc.AddPage()
                chainId = _doc.AddImageToChain(chainId)
            End While

            For i As Integer = startPage To _doc.PageCount
                _doc.PageNumber = i
                _doc.Flatten()
            Next
        End Sub

        Public Sub Close()
            _nullWatermarkBody.Clear()
            _doc.Clear()
            Dispose()
        End Sub

        Public Function GetBuffer() As Byte()

            Return _doc.GetData()

        End Function

        Public Function Read(ByVal fileData As Byte()) As Boolean
            If (fileData Is Nothing) Then
                Return False
            End If

            Try
                _doc.Read(fileData)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Render the final PDF document to the client response stream
        ''' </summary>
        ''' <param name="response"></param>
        ''' <remarks></remarks>
        Public Sub Render(ByVal response As HttpResponse)

            If (IncludeFOUOWatermark) Then
                AddBRBlockNotUsedWatermark()
                AddAABlockNotUsedWatermark()
                AddWJABlockNotUsedWatermark()
                AddFOUOWatermark()
            End If

            _doc.Form.Stamp()
            _doc.Flatten()

            AddNullDocumentWatermark()

            Dim theData() As Byte = _doc.GetData()

            response.ContentType = "application/pdf"
            response.AddHeader("content-disposition", "inline; filename=output.PDF")
            response.AddHeader("content-length", theData.Length.ToString())
            response.BinaryWrite(theData)
            response.End()

        End Sub

        ''' <summary>
        ''' Render the final PDF document to a file
        ''' </summary>
        ''' <param name="fileName">The name of the file to write to</param>
        ''' <remarks></remarks>
        Public Sub Render(ByVal fileName As String)

            If (IncludeFOUOWatermark) Then
                AddBRBlockNotUsedWatermark()
                AddAABlockNotUsedWatermark()
                AddWJABlockNotUsedWatermark()
                AddFOUOWatermark()
            End If

            _doc.Form.Stamp()
            _doc.Flatten()

            AddNullDocumentWatermark()

        End Sub

        Public Sub SetRenderingEngine(ByVal engine As EngineType)
            _doc.HtmlOptions.Engine = engine
        End Sub

        Private Sub AddAABlockNotUsedWatermark()

            If (IncludeWSCompleteWaterMark) Then
                _doc.FontSize = 40

                _doc.PageNumber = 3
                _doc.Pos.X = 0
                _doc.Pos.Y = _doc.Rect.Bottom + 330 '_doc.Rect.Top / 2
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>")
            End If

        End Sub

        Private Sub AddBRBlockNotUsedWatermark()

            If (IncludeWSCompleteWaterMark) Then
                _doc.FontSize = 40

                _doc.PageNumber = 3
                _doc.Pos.X = 0
                _doc.Pos.Y = _doc.Rect.Bottom + 460 '_doc.Rect.Top / 2
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>")
                _doc.Pos.Y = _doc.Rect.Bottom + 600 '_doc.Rect.Top / 2
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>")
            End If

        End Sub

        Private Sub AddFOUOWatermark()

            _doc.FontSize = 22

            For i As Short = 1 To _doc.PageCount
                _doc.PageNumber = i
                _doc.Pos.X = 0
                _doc.Pos.Y = _doc.Rect.Bottom + 24 '_doc.Rect.Top / 2
                _doc.AddHtml("<p align=center><StyleRun rotate=0><Font face='arial-bold' color=#FF0000/90>" + ConfigurationManager.AppSettings("WaterMark") + "</Font></StyleRun></p>")
            Next

        End Sub

        Private Sub AddNullDocumentWatermark()
            If (_nullWatermarkBody Is Nothing OrElse _nullWatermarkBody.Count = 0) Then
                Exit Sub
            End If

            For i As Short = 1 To _doc.PageCount
                _doc.PageNumber = i

                ' Add grey overlay
                _doc.Color.String = "128 128 128 a64"
                _doc.Color.Alpha = 255 / 5
                _doc.FillRect()

                If (Helpers.IsOdd(i)) Then
                    ' Fill in grey rectangle
                    _doc.Rect.Inset(38.25, 297)
                    _doc.Color.String = "128 128 128"
                    _doc.Color.Alpha = 255 / 5
                    _doc.FillRect()
                    _doc.Rect.Inset(-38.25, -297)

                    ' Fill in smaller white rectangle over the grey rectangle
                    _doc.Rect.Inset(41.25, 300.5)
                    _doc.Color.String = "255 255 255"
                    _doc.Color.Alpha = 255 / 5
                    _doc.FillRect()
                    _doc.Rect.Inset(-41.25, -300.5)

                    ' Add text via HTML
                    _doc.Rect.Inset(41.25, 0)
                    _doc.Pos.X = 0
                    _doc.Pos.Y = (_doc.MediaBox.Height / 2) + 75

                    For Each s In _nullWatermarkBody
                        If (Not String.IsNullOrEmpty(s.Text)) Then
                            _doc.AddHtml(s.GetHTML())
                        End If
                    Next
                    _doc.Rect.Inset(-41.25, 0)
                End If
            Next
        End Sub

        Private Sub AddWJABlockNotUsedWatermark()

            If (IncludeWSCompleteWaterMark) Then
                _doc.FontSize = 40

                _doc.PageNumber = 2
                _doc.Pos.X = 0
                _doc.Pos.Y = _doc.Rect.Bottom + 143 '_doc.Rect.Top / 2
                _doc.AddHtml("<p align=center><StyleRun rotate=4><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>")
            End If

        End Sub

#Region "IDisposable"

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    _nullWatermarkBody.Clear()
                    _doc.Clear()
                End If

                ' TODO: free shared unmanaged resources

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace