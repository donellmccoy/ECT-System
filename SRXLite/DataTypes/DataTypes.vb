Option Strict On

Namespace DataTypes

    Public Module DataTypes

#Region " ActionType "

        Public Enum ActionType
            AddDocument = 1
            DeleteDocument = 2
            UpdateDocumentStatus = 3
            AddDocumentPage = 4
            DeleteDocumentPage = 5
            AddGroup = 6
            DeleteGroup = 7
            AddGroupDocument = 8
            DeleteGroupDocument = 9
            AddGroupCopy = 10
            UpdateDocumentKeys = 11
            AddBatch = 12
            GenerateDocumentPDF = 13
        End Enum

#End Region

#Region " BatchType "

        Public Enum BatchType As Byte
            Entity = 1
            DocType = 2
        End Enum

#End Region

#Region " DocumentStatus "

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum DocumentStatus As Byte
            Pending = 1
            Approved = 2
            Deleted = 3
            Test = 4
            Batch = 5
        End Enum

#End Region

#Region " BatchDocumentType "

        Public Enum BatchDocumentType As Integer
            BatchImport = 2
        End Enum

#End Region

#Region " InputType "

        Public Enum InputType As Byte
            Upload = 1
            Scan = 2
            WebServiceUpload = 3
        End Enum

#End Region

#Region " Category "

        Public Enum Category
            Document = 1
            Group = 2
            Batch = 3
        End Enum

#End Region

#Region " UploadKeys "

        Public Class UploadKeys
            Public DocDate As Date
            Public DocDescription As String
            Public DocStatus As DocumentStatus
            Public DocTypeID As Integer
            Public EntityName As String
            Public FileName As String
            Public InputType As InputType
        End Class

#End Region

#Region " DocumentKeys "

        Public Class DocumentKeys
            Public DocDate As Date
            Public DocDescription As String
            Public DocStatus As DocumentStatus
            Public DocTypeID As Integer
            Public EntityName As String
        End Class

#End Region

#Region " DocumentData "

        Public Class DocumentData
            Public DocDate As Date
            Public DocDescription As String
            Public DocID As Long
            Public DocStatus As DocumentStatus
            Public DocTypeID As Integer
            Public DocTypeName As String
            Public EntityName As String
            Public FileExt As String
            Public IconUrl As String
            Public IsAppendable As Boolean
            Public OriginalFileName As String
            Public PageCount As Short
            Public UploadDate As Date
            Public UploadedBySubuserName As String
        End Class

#End Region

#Region " DocumentPageData "

        Public Class DocumentPageData
            Public DocPageID As Long
            Public PageNumber As Short
            Public PageUrl As String
        End Class

#End Region

#Region " BatchUploadKeys "

        Public Class BatchUploadKeys
            Public BatchType As BatchType
            Public DocTypeID As Integer
            Public EntityName As String
            Public Location As String
        End Class

#End Region

#Region " BatchData "

        Public Class BatchData
            Public BatchID As Integer
            Public BatchType As BatchType
            Public DateCreated As Date
            Public DocDescription As String
            Public DocTypeID As Integer
            Public DocTypeName As String
            Public EntityName As String
            Public Location As String
            Public PageCount As Integer
            Public UploadedBySubuserName As String
        End Class

#End Region

    End Module

End Namespace