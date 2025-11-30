Imports System.IO
Imports System.Xml

Namespace Common

    Public Class XMLString
        Implements IDisposable

        Private _sections As New Stack(Of String)
        Private _stream As MemoryStream
        Private _writer As XmlTextWriter

        Private disposedValue As Boolean = False

        Public Sub New(ByVal title As String)

            _stream = New MemoryStream()
            _writer = New XmlTextWriter(_stream, System.Text.Encoding.ASCII)

            'start the document
            BeginElement(title)

        End Sub

        Public ReadOnly Property Value() As String
            Get
                Return Me.ToString()
            End Get
        End Property

        Public Sub BeginElement(ByVal name As String)
            _sections.Push(name)
            _writer.WriteStartElement(name)
        End Sub

        Public Sub Close()

            'write the end elements for any that weren't closed
            While (_sections.Count > 0)
                EndElement()
            End While

        End Sub

        Public Sub EndElement()
            _sections.Pop()
            _writer.WriteEndElement()
        End Sub

        Public Overrides Function ToString() As String

            Close()

            _writer.Flush()
            Dim buffer(_stream.Length) As Byte
            _stream.Seek(0, SeekOrigin.Begin)

            Dim reader As New StreamReader(_stream)
            Return reader.ReadToEnd()
        End Function

        Public Sub WriteAttribute(ByVal name As String, ByVal value As String)
            _writer.WriteAttributeString(name, value)
        End Sub

        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free managed resources when explicitly called
                    Try
                        _writer.Close()
                    Catch
                    End Try
                End If

                ' TODO: free shared unmanaged resources
            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class

End Namespace