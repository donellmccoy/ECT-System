Imports ALOD.Data

Namespace Permission

    Public Class PermRequest

        Protected _akoId As String
        Protected _dateReq As Date
        Protected _grant As Boolean = False
        Protected _permId As Integer
        Protected _requestId As Integer
        Protected _userId As Integer
        Protected _userName As String

        Public Property AkoId() As String
            Get
                Return _akoId
            End Get
            Set(ByVal value As String)
                _akoId = value
            End Set
        End Property

        Public Property DateRequested() As Date
            Get
                Return _dateReq
            End Get
            Set(ByVal value As Date)
                _dateReq = value
            End Set
        End Property

        Public Property Granted() As Boolean
            Get
                Return _grant
            End Get
            Set(ByVal value As Boolean)
                _grant = value
            End Set
        End Property

        Public Property PermissionId() As Integer
            Get
                Return _permId
            End Get
            Set(ByVal value As Integer)
                _permId = value
            End Set
        End Property

        Public Property RequestId() As Integer
            Get
                Return _requestId
            End Get
            Set(ByVal value As Integer)
                _requestId = value
            End Set
        End Property

        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal value As Integer)
                _userId = value
            End Set
        End Property

        Public Property UserName() As String
            Get
                Return _userName
            End Get
            Set(ByVal value As String)
                _userName = value
            End Set
        End Property

        Public Sub GetByUserId(ByVal userId As Integer, ByVal permId As Short)

            _permId = permId
            _userId = userId

            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf RequestReader, "core_permissions_sp_getRequestForUser", userId, permId)

        End Sub

        Public Function Grant(ByVal userId As Integer) As Boolean

            Dim adapter As New SqlDataStore
            Return CInt(adapter.ExecuteNonQuery("core_permissions_sp_GrantUserPermission", userId, _permId)) > 0

        End Function

        Public Function Save(ByVal userId As Integer) As Boolean

            If (_permId = 0) Then
                Return False
            End If

            Dim adapter As New SqlDataStore()
            Return CInt(adapter.ExecuteNonQuery("core_permissions_sp_insertRequest", userId, _permId)) > 0

        End Function

        Protected Sub RequestReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            UserId = adapter.GetInt32(reader, 0)
            AkoId = adapter.GetString(reader, 1)
            UserName = adapter.GetString(reader, 2) + " " + adapter.GetString(reader, 3) + ", " + adapter.GetString(reader, 4)
            DateRequested = adapter.GetDateTime(reader, 5)
            RequestId = adapter.GetInt32(reader, 6)
            PermissionId = adapter.GetInt16(reader, 7)

        End Sub

    End Class

End Namespace