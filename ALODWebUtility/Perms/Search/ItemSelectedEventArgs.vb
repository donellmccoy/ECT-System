Imports ALOD.Core.Domain.Workflow

Namespace Permission.Search

    Public Class ItemSelectedEventArgs
        Inherits EventArgs

        Private _baseType As ModuleType
        Private _canEdit As Boolean

        'used by appeals
        Private _compo As String

        Private _parentId As Integer
        Private _recId As Integer
        Private _redirect As Boolean = True
        Private _refId As Integer
        Private _requestId As Integer
        Private _status As String = ""
        Private _type As ModuleType
        Private _url As String = ""
        Private _workFlowId As Short

        ''' <summary>
        ''' For appeals this is the the base module type being appealed
        ''' For non-appeals this will be the same as Type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BaseType() As ModuleType
            Get
                Return _baseType
            End Get
            Set(ByVal value As ModuleType)
                _baseType = value
            End Set
        End Property

        Public Property CanEdit() As Boolean
            Get
                Return _canEdit
            End Get
            Set(ByVal value As Boolean)
                _canEdit = value
            End Set
        End Property

        Public Property Compo() As String
            Get
                Return _compo
            End Get
            Set(ByVal value As String)
                _compo = value
            End Set
        End Property

        Public Property ParentID() As Integer
            Get
                Return Me._parentId
            End Get
            Set(ByVal value As Integer)
                Me._parentId = value
            End Set
        End Property

        Public Property RecID() As Integer
            Get
                Return Me._recId
            End Get
            Set(ByVal value As Integer)
                Me._recId = value
            End Set
        End Property

        Public Property Redirect() As Boolean
            Get
                Return _redirect
            End Get
            Set(ByVal value As Boolean)
                _redirect = value
            End Set
        End Property

        Public Property RefId() As Integer
            Get
                Return _refId
            End Get
            Set(ByVal value As Integer)
                _refId = value
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

        Public Property Status() As String
            Get
                Return _status
            End Get
            Set(ByVal value As String)
                _status = value
            End Set
        End Property

        Public Property Type() As ModuleType
            Get
                Return _type
            End Get
            Set(ByVal value As ModuleType)
                _type = value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

        Public Property WorkFlowId() As Short
            Get
                Return _workFlowId
            End Get
            Set(ByVal value As Short)
                _workFlowId = value
            End Set
        End Property

    End Class

End Namespace