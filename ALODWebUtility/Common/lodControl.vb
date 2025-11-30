Namespace Common

    <Serializable()>
    Public Class lodControl
        Private _field As String
        Private _IsModified As Boolean
        Private _name As String
        Private _section As String
        Private _value As String

        Public Sub New(ByVal name As String, ByVal val As String, ByVal sec As String, ByVal fld As String)
            _name = name
            _value = val
            _section = sec
            _field = fld
        End Sub

        Public Property Field() As String
            Get
                Return _field
            End Get
            Set(ByVal value As String)
                _field = value
            End Set
        End Property

        Public Property IsModified() As Boolean
            Get
                Return _IsModified
            End Get
            Set(ByVal value As Boolean)
                _IsModified = value
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

        Public Property Section() As String
            Get
                Return _section
            End Get
            Set(ByVal value As String)
                _section = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

        Public Sub ToDataRow(ByVal row As DataSets.ControlLodRow)

            row.name = _name
            row.section = _section
            row.field = _field
            row.value = _value

        End Sub

    End Class

End Namespace