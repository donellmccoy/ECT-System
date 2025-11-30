Imports ALOD.Core.Domain.Workflow

Namespace Worklfow

    Public Class ActiveCase

        Protected _descr As String
        Protected _modDesc As String
        Protected _parentId As Integer
        Protected _refId As Integer
        Protected _sep As String = "|"
        Protected _title As String
        Protected _type As ModuleType

        Public Property Description() As String
            Get
                Return _descr
            End Get
            Set(ByVal value As String)
                _descr = value
            End Set
        End Property

        Public Property ModuleTitle() As String
            Get
                Return _modDesc
            End Get
            Set(ByVal value As String)
                _modDesc = value
            End Set
        End Property

        Public Property ParentId() As Integer
            Get
                Return _parentId
            End Get
            Set(ByVal value As Integer)
                _parentId = value
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

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
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

        Public Sub FromString(ByVal input As String)

            Dim parts() As String = input.Split(_sep)

            If parts.Length <> 3 Then
                Exit Sub
            End If

            Integer.TryParse(parts(0), _refId)
            Integer.TryParse(parts(1), _parentId)
            Byte.TryParse(parts(2), _type)

        End Sub

        Public Overrides Function ToString() As String
            Return _modDesc + " (" + _descr + ")"
        End Function

        Public Function ValueString() As String

            Return _refId.ToString() + _sep + _parentId.ToString() + _sep + CStr(_type)

        End Function

    End Class

End Namespace