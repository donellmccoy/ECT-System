Namespace Printing

    Public Class PrintDocument
        Protected _compo As Integer
        Protected _docid As Integer
        Protected _docname As String
        Protected _filename As String
        Protected _filetype As String
        Protected _formFieldParserId As Integer
        Protected _sbDelimiter As String = " ~ "
        Protected _sp_name As String

#Region "Constructors"

        Public Sub New()
            _docid = 0
            _docname = String.Empty
        End Sub

        Public Sub New(ByVal docId As Integer)
            _docid = docId
            LoadDocumentData()
        End Sub

#End Region

#Region "Properties"

        Public Property Compo() As Integer
            Get
                Return _compo
            End Get
            Set(ByVal Value As Integer)
                _compo = Value
            End Set
        End Property

        Public Property DocID() As Integer
            Get
                Return _docid
            End Get
            Set(ByVal Value As Integer)
                _docid = Value
            End Set
        End Property

        Public Property DocName() As String
            Get
                Return _docname
            End Get
            Set(ByVal Value As String)
                _docname = Value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return _filename
            End Get
            Set(ByVal Value As String)
                _filename = Value
            End Set
        End Property

        Public Property FileType() As String
            Get
                Return _filetype
            End Get
            Set(ByVal Value As String)
                _filetype = Value
            End Set
        End Property

        Public Property SpName() As String
            Get
                Return _sp_name
            End Get
            Set(ByVal Value As String)
                _sp_name = Value
            End Set
        End Property

#End Region

#Region "Database/Loading"

        Public Sub LoadDocumentData()
            Try
                Dim documentDetailsTable As DTPrinting.PrintDocumentDetailsDataTable
                Dim documentDetailsStore As New PrintStore()
                documentDetailsTable = documentDetailsStore.DocumentGetDetails(_docid)
                Dim row As DTPrinting.PrintDocumentDetailsRow = documentDetailsTable.Rows(0)
                _docname = row.doc_name
                _filename = row.filename
                _filetype = row.filetype
                _compo = row.compo
                _sp_name = row.sp_getdata
                _formFieldParserId = row.FormFieldParserId
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Methods"

        'Used to retrieve the Document specific data for assembly.
        Protected Function GetDocumentData(ByVal keyId As Integer) As DataSet

            Dim printStore As New PrintStore()
            Try
                Return printStore.DocumentGetData(_sp_name, _docid, keyId)
            Catch ex As Exception
                Throw New DataException("Unable to acquire data.")
            End Try

        End Function

#End Region

    End Class

End Namespace