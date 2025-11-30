Imports ALOD.Data

Namespace Worklfow

    Public Class WorkStatusOption

        Protected _active As Boolean

        '07-17-2019 S32019 - Curt Lucas
        Protected _compo As String

        Protected _groupId As Byte
        Protected _groupName As String
        Protected _id As Integer
        Protected _order As Byte
        Protected _statusOut As Integer
        Protected _statusOutText As String
        Protected _template As Byte
        Protected _text As String
        Protected _valid As Boolean
        Protected _visible As Boolean
        Protected _workstatusId As Integer

        Public Property Active() As Boolean
            Get
                Return _active
            End Get
            Set(ByVal value As Boolean)
                _active = value
            End Set
        End Property

        Public Property Compo() As Integer
            Get
                Return _compo
            End Get
            Set(ByVal value As Integer)
                _compo = value
            End Set
        End Property

        Public ReadOnly Property CompoName As String
            Get
                Select Case Compo
                    Case 5
                        Return "ANG"
                    Case 6
                        Return "AFRC"
                    Case 0
                        Return "All"
                    Case Else
                        Return "UNKNOWN"
                End Select
            End Get
        End Property

        Public Property DBSignTemplate() As Byte
            Get
                Return _template
            End Get
            Set(ByVal value As Byte)
                _template = value
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

        Public Property OptionVisible() As Boolean
            Get
                Return _visible
            End Get
            Set(ByVal value As Boolean)
                _visible = value
            End Set
        End Property

        Public Property SortOrder() As Byte
            Get
                Return _order
            End Get
            Set(ByVal value As Byte)
                _order = value
            End Set
        End Property

        Public Property StatusOut() As Integer
            Get
                Return _statusOut
            End Get
            Set(ByVal value As Integer)
                _statusOut = value
            End Set
        End Property

        Public Property StatusOutText() As String
            Get
                Return _statusOutText
            End Get
            Set(ByVal value As String)
                _statusOutText = value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property

        Public Property Valid() As Boolean
            Get
                Return _valid
            End Get
            Set(ByVal value As Boolean)
                _valid = value
            End Set
        End Property

        Public Property WorkStatusId() As Integer
            Get
                Return _workstatusId
            End Get
            Set(ByVal value As Integer)
                _workstatusId = value
            End Set
        End Property

        Public Function Delete() As Boolean
            Dim adapter As New SqlDataStore
            Return CInt(adapter.ExecuteNonQuery("core_workstatus_sp_DeleteOption", _id)) > 0
        End Function

        Public Function Save() As Boolean
            Dim adapter As New SqlDataStore
            _id = CInt(adapter.ExecuteNonQuery("core_workstatus_sp_InsertOption", _id, _workstatusId, _statusOut, _text, _active, _order, _template, _compo))
            Return _id > 0
        End Function

    End Class

End Namespace