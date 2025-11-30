Imports ALOD.Data

Namespace Common

    <Serializable()>
    Public Class Message

#Region "Members"

        Protected _rows As Byte
        Private _assigned As Boolean = False
        Private _endTime As New Date
        Private _groupId As Byte = 0
        Private _groupName As String = String.Empty
        Private _isAdmin As Boolean = False
        Private _message As String = String.Empty
        Private _messageId As Int16 = 0
        Private _name As String = String.Empty
        Private _popup As Boolean = False
        Private _startTime As New Date
        Private _title As String = String.Empty

#End Region

#Region "Properties"

        Public Property Assigned() As Boolean
            Get
                Return _assigned
            End Get
            Set(ByVal value As Boolean)
                _assigned = value
            End Set
        End Property

        Public Property EndTime() As Date
            Get
                Return _endTime
            End Get
            Set(ByVal value As Date)
                _endTime = value
            End Set
        End Property

        Public Property GroupId() As Byte
            Get
                Return _groupId
            End Get
            Set(ByVal value As Byte)
                _groupId = value
            End Set
        End Property

        Public Property GroupName() As String
            Get
                Return _groupName
            End Get
            Set(ByVal value As String)
                _groupName = value
            End Set
        End Property

        Public Property IsAdmin() As Boolean
            Get
                Return _isAdmin
            End Get
            Set(ByVal value As Boolean)
                _isAdmin = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return _message
            End Get
            Set(ByVal value As String)
                _message = value
            End Set
        End Property

        Public Property MessageId() As Int16
            Get
                Return _messageId
            End Get
            Set(ByVal value As Int16)
                _messageId = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Popup() As Boolean
            Get
                Return _popup
            End Get
            Set(ByVal value As Boolean)
                _popup = value
            End Set
        End Property

        Public Property StartTime() As Date
            Get
                Return _startTime
            End Get
            Set(ByVal value As Date)
                _startTime = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

#End Region

        Public Sub DeleteMessage(ByVal messageId As Int16)
            Dim adapter As New SqlDataStore
            Dim cmd As Data.Common.DbCommand
            cmd = adapter.GetStoredProcCommand("core_messages_sp_DeleteMessage")
            adapter.AddInParameter(cmd, "@messageId", Data.DbType.Int16, messageId)
            adapter.ExecuteNonQuery(cmd)
        End Sub

        ''' <summary>
        ''' Returns a Message details row
        ''' </summary>
        ''' <param name="messageId">The ID of the Message</param>
        ''' <returns>A data row containing the results.</returns>
        ''' <remarks></remarks>
        Public Function RetrieveMessageDetails(ByVal messageId As Integer) As Message
            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_messages_sp_GetMessagesDetails", messageId)
            adapter.ExecuteReader(AddressOf RetrieveMessageDetailsReader, cmd)
            Return Me

        End Function

        Protected Sub RetrieveMessageDetailsReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            _rows += 1

            '0-Title, 1-Message, 2-Name, 3-Popup, 4-StartTime,5-EndTime
            'Dim message As New Message
            _title = adapter.GetString(reader, 0)
            _message = adapter.GetString(reader, 1)
            _name = adapter.GetString(reader, 2)
            _popup = adapter.GetBoolean(reader, 3)
            _startTime = adapter.GetDateTime(reader, 4, Now).ToShortDateString
            _endTime = adapter.GetDateTime(reader, 5, Now).ToShortDateString

        End Sub

    End Class

End Namespace