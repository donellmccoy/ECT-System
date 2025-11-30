Namespace Common

    Public Class childunit

        Private _CHAIN_TYPE As String = String.Empty
        Private _childName As String = String.Empty
        Private _childPasCode As String = String.Empty
        Private _cs_id As Integer = -1
        Private _Level As Integer = -1
        Private _parentCS_ID As Integer = -1
        Private _userUnit As Integer = -1

        Private in_Active As Boolean = False

        Public Sub New()

        End Sub

        Public Property CHAIN_TYPE() As String
            Get
                Return Me._CHAIN_TYPE
            End Get
            Set(ByVal value As String)

                Me._CHAIN_TYPE = value

            End Set
        End Property

        Public Property childName() As String
            Get
                Return Me._childName
            End Get
            Set(ByVal value As String)

                Me._childName = value

            End Set
        End Property

        Public Property childPasCode() As String
            Get
                Return Me._childPasCode
            End Get
            Set(ByVal value As String)

                Me._childPasCode = value

            End Set
        End Property

        Public Property cs_id() As Integer
            Get
                Return Me._cs_id
            End Get
            Set(ByVal value As Integer)

                Me._cs_id = value

            End Set
        End Property

        Public Property InActive() As Boolean
            Get
                Return Me.in_Active
            End Get
            Set(ByVal value As Boolean)

                Me.in_Active = value

            End Set
        End Property

        Public Property Level() As Integer
            Get
                Return Me._Level
            End Get
            Set(ByVal value As Integer)

                Me._Level = value

            End Set
        End Property

        Public Property parentCS_ID() As Integer
            Get
                Return Me._parentCS_ID
            End Get
            Set(ByVal value As Integer)

                Me._parentCS_ID = value

            End Set
        End Property

        Public Property userUnit() As Integer
            Get
                Return Me._userUnit
            End Get
            Set(ByVal value As Integer)

                Me._userUnit = value

            End Set
        End Property

    End Class

End Namespace