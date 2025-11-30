Imports ALOD.Data

Namespace Worklfow

    Public Class WorkStatus

        Protected _descr As String
        Protected _groupId As Byte
        Protected _groupName As String
        Protected _id As Integer
        Protected _sortOrder As Byte
        Protected _status As Integer
        Protected _workflow As Byte

        Public Property Description() As String
            Get
                Return _descr
            End Get
            Set(ByVal value As String)
                _descr = value
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

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property SortOrder() As Byte
            Get
                Return _sortOrder
            End Get
            Set(ByVal value As Byte)
                _sortOrder = value
            End Set
        End Property

        Public Property Status() As Integer
            Get
                Return _status
            End Get
            Set(ByVal value As Integer)
                _status = value
            End Set
        End Property

        Public Property Workflow() As Byte
            Get
                Return _workflow
            End Get
            Set(ByVal value As Byte)
                _workflow = value
            End Set
        End Property

        ''' <summary>
        ''' Saves the status code to the provided workflow
        ''' </summary>
        ''' <param name="workflow"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Save(ByVal workflow As Byte) As Boolean
            _workflow = workflow
            Return Save()
        End Function

        Public Function Save() As Boolean
            Dim adapter As New SqlDataStore
            Return CInt(adapter.ExecuteNonQuery("core_workstatus_sp_InsertStatus", _workflow, _id)) > 0
        End Function

        Public Function SetOrder(ByVal order As Byte) As Boolean

            Dim adapter As New SqlDataStore
            If CInt(adapter.ExecuteNonQuery("core_workstatus_sp_UpdateOrder", _id, order)) > 0 Then
                _sortOrder = order
                Return True
            End If

            Return False

        End Function

    End Class

End Namespace