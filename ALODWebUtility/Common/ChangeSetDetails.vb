Imports ALOD.Data

Namespace Common

    Public Class ChangeSetDetails

        Private _actionDate As New Nullable(Of Date)
        Private _firstName As String
        Private _lastName As String
        Private _logId As Integer
        Private _rank As String
        Private _userId As Integer

        Public Property ActionDate() As Nullable(Of Date)
            Get
                Return _actionDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                _actionDate = value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return _firstName
            End Get
            Set(ByVal value As String)
                _firstName = value
            End Set
        End Property

        Public ReadOnly Property FullNameRank() As String
            Get
                If (Not IsNothing(_rank) AndAlso Rank.ToLower() <> "civ") Then
                    Return _rank + " " + _lastName + ", " + _firstName
                End If

                If (Not IsNothing(_lastName) AndAlso Not IsNothing(_firstName)) Then
                    Return _lastName + ", " + _firstName
                End If

                Return Nothing

            End Get
        End Property

        Public Property LastName() As String
            Get
                Return _lastName
            End Get
            Set(ByVal value As String)
                _lastName = value
            End Set
        End Property

        Public Property LogId() As Integer
            Get
                Return _logId
            End Get
            Set(ByVal value As Integer)
                _logId = value
            End Set
        End Property

        Public Property Rank() As String
            Get
                Return _rank
            End Get
            Set(ByVal value As String)
                _rank = value
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

        Public Function GetLastChange(ByVal id As Integer) As ChangeSetDetails
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf LastChagedReader, "core_log_sp_GetLastChange", id)
            Return Me
        End Function

        Protected Sub LastChagedReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            Me.LogId = adapter.GetInt32(reader, 0)
            Me.ActionDate = adapter.GetNullableDate(reader, 1, Nothing)
            Me.UserId = adapter.GetInt32(reader, 2)
            Me.LastName = adapter.GetString(reader, 3)
            Me.FirstName = adapter.GetString(reader, 4)
            Me.Rank = adapter.GetString(reader, 5)
        End Sub

    End Class

End Namespace