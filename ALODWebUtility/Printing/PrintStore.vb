Imports ALOD.Data

Namespace Printing

    Public Class PrintStore

        Protected _adapter As SqlDataStore
        Private _data As DTPrinting.LetterContentsDataTable
        Private _letterTemplates As DTPrinting.LetterTemplateDataTable
        Dim _memoList As DTPrinting.MemoListDataTable
        Private _printingDocumentsTable As DTPrinting.PrintDocumentDetailsDataTable

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If
                Return _adapter
            End Get
        End Property

        Public Function GetMemoList(ByVal pk2173 As Integer, ByVal secondaryId As Integer, ByVal workFlowCode As String) As DTPrinting.MemoListDataTable
            _memoList = New DTPrinting.MemoListDataTable
            DataStore.ExecuteReader(AddressOf MemoListReader, "core_memo_sp_GetListByRefId", pk2173)
            Return _memoList
        End Function

        Public Function GetTemplateData(ByVal dataProc As String, ByVal ParamArray parameters() As Object) As DTPrinting.LetterContentsDataTable
            _data = New DTPrinting.LetterContentsDataTable
            DataStore.ExecuteReader(AddressOf TemplateDataReader, dataProc, parameters)
            Return _data
        End Function

        Private Sub MemoListReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim row As DTPrinting.MemoListRow = _memoList.NewMemoListRow

            '0-LetterID, 1-TemplateID, 2-OriginationDate, 3-Description, 4-AccessLevelID
            '5-ViewRestrictedByAccessLevel, 6-UserRole, 7-AccessLevel

            row.LetterId = DataStore.GetInt32(reader, 0)
            row.TemplateId = DataStore.GetByte(reader, 1)
            row.CreatedDate = DataStore.GetDateTime(reader, 2, DateTime.Now)
            row.Description = DataStore.GetString(reader, 3)

            _memoList.Rows.Add(row)

        End Sub

#Region "Get Document Dataset"

        Public Function DocumentGetData(ByVal procName As String, ByVal docId As Integer, ByVal keyId As Integer) As DataSet
            Dim ds As DataSet = Nothing
            Try
                ds = DataStore.ExecuteDataSet(procName, docId, keyId)
            Catch ex As Exception
                Throw ex
            End Try
            Return ds
        End Function

#End Region

#Region "Print Document Details"

        Public Function DocumentGetDetails(ByVal documentId As Integer) As DTPrinting.PrintDocumentDetailsDataTable
            _printingDocumentsTable = New DTPrinting.PrintDocumentDetailsDataTable()
            DataStore.ExecuteReader(AddressOf HandleDocumentDetailsRowRead, "print_sp_GetDocumentDetails", documentId)
            Return _printingDocumentsTable
        End Function

        Private Sub HandleDocumentDetailsRowRead(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim row As DTPrinting.PrintDocumentDetailsRow = _printingDocumentsTable.NewPrintDocumentDetailsRow()

            row.docid = DataStore.GetInt16(reader, 0)
            row.doc_name = DataStore.GetString(reader, 1)
            row.filename = DataStore.GetString(reader, 2)
            row.filetype = DataStore.GetString(reader, 3)
            row.compo = DataStore.GetInt16(reader, 4)
            row.sp_getdata = DataStore.GetString(reader, 5)
            row.FormFieldParserId = DataStore.GetInt32(reader, 6)

            _printingDocumentsTable.Rows.Add(row)

        End Sub

#End Region

#Region "Templates"

        Public Function GetLetterTemplate(ByVal refId As Integer, ByVal templateId As Short, ByVal userGroup As Byte) As DTPrinting.LetterTemplateDataTable
            _letterTemplates = New DTPrinting.LetterTemplateDataTable
            DataStore.ExecuteReader(AddressOf LetterTemplateReader, "core_memo_sp_GetTemplateById", refId, templateId, userGroup)
            Return _letterTemplates
        End Function

        Private Sub LetterTemplateReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim row As DTPrinting.LetterTemplateRow = _letterTemplates.NewLetterTemplateRow

            '0-Description, 1-Content, 2-MailingAddress, 3-addSignature
            '4-dataSource, 5-officeSymbol, 6-addDate, 7-addSuspenseDate, 8-ApprovalRequired
            '9-sigblock

            row.Description = DataStore.GetString(reader, 0)
            row.Content = DataStore.GetString(reader, 1)
            row.MailingAddress = DataStore.GetString(reader, 2)
            row.addSignature = DataStore.GetBoolean(reader, 3, False)
            row.dataProc = DataStore.GetString(reader, 4)
            row.officeSymbol = DataStore.GetString(reader, 5)
            row.addDate = DataStore.GetBoolean(reader, 6, True)
            row.addSuspenseDate = DataStore.GetBoolean(reader, 7, False)
            row.ApprovalRequired = DataStore.GetBoolean(reader, 8, False)
            row.sigBlock = DataStore.GetString(reader, 9)

            _letterTemplates.Rows.Add(row)

        End Sub

#End Region

        Private Sub TemplateDataReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim row As DTPrinting.LetterContentsRow = _data.NewLetterContentsRow

            row.Key = DataStore.GetString(reader, 0)
            row.Value = DataStore.GetString(reader, 1)

            _data.Rows.Add(row)

        End Sub

    End Class

End Namespace