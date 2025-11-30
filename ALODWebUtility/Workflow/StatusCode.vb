Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Worklfow

    <Serializable()>
    Public Class StatusCode

        Protected _accessScope As Byte = 0
        Protected _canAppeal As Boolean
        Protected _compo As String = String.Empty
        Protected _compoDescr As String = String.Empty
        Protected _description As String = String.Empty
        Protected _displayOrder As Byte = 0
        Protected _fullDescr As String = String.Empty
        Protected _groupId As Byte
        Protected _groupName As String = String.Empty
        Protected _isApproved As Boolean
        Protected _isDisposition As Byte = 0
        Protected _isFinal As Boolean
        Protected _isFormal As Byte = 0
        Protected _moduleId As ModuleType
        Protected _moduleName As String = String.Empty
        Protected _statusId As Integer = 0

        Public Sub New()

        End Sub

        Public Property AccessScope() As Byte
            Get
                Return _accessScope
            End Get
            Set(ByVal value As Byte)
                _accessScope = value
            End Set
        End Property

        Public Property CanAppeal() As Boolean
            Get
                Return _canAppeal
            End Get
            Set(ByVal value As Boolean)
                _canAppeal = value
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

        Public Property CompoDescr() As String
            Get
                Return _compoDescr
            End Get
            Set(ByVal value As String)
                _compoDescr = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property DisplayOrder() As Byte
            Get
                Return _displayOrder
            End Get
            Set(ByVal value As Byte)
                _displayOrder = value
            End Set
        End Property

        Public Property FullDescription() As String
            Get
                Return _fullDescr
            End Get
            Set(ByVal value As String)
                _fullDescr = value
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

        Public Property IsApproved() As Boolean
            Get
                Return _isApproved
            End Get
            Set(ByVal value As Boolean)
                _isApproved = value
            End Set
        End Property

        Public Property IsDisposition() As Boolean
            Get
                Return _isDisposition
            End Get
            Set(ByVal value As Boolean)
                _isDisposition = value
            End Set
        End Property

        Public Property IsFinal() As Boolean
            Get
                Return _isFinal
            End Get
            Set(ByVal value As Boolean)
                _isFinal = value
            End Set
        End Property

        Public Property IsFormal() As Boolean
            Get
                Return _isFormal
            End Get
            Set(ByVal value As Boolean)
                _isFormal = value
            End Set
        End Property

        Public Property ModuleId() As ModuleType
            Get
                Return _moduleId
            End Get
            Set(ByVal value As ModuleType)
                _moduleId = value
            End Set
        End Property

        Public Property ModuleName() As String
            Get
                Return _moduleName
            End Get
            Set(ByVal value As String)
                _moduleName = value
            End Set
        End Property

        Public Property StatusId() As Integer
            Get
                Return _statusId
            End Get
            Set(ByVal value As Integer)
                _statusId = value
            End Set
        End Property

        Public Function Insert() As Boolean

            If (_description.Trim.Length = 0) OrElse (_compo.Trim.Length = 0) OrElse (_moduleId = 0) Then
                Return False
            End If

            Dim adapter As New SqlDataStore()
            _statusId = adapter.ExecuteScalar("core_workflow_sp_InsertStatusCode", _description, _isFinal, _isApproved, _canAppeal, IIf(_groupId <> 0, _groupId, DBNull.Value), _moduleId, _compo, _isDisposition, _isFormal)

            Return _statusId > 0

        End Function

        Public Sub SetAccessScope()
            Dim adapter As New SqlDataStore()
            _accessScope = adapter.ExecuteScalar("core_workflow_sp_GetStatusCodeScope", _statusId)
        End Sub

        Public Sub ToDataRow(ByRef row As DataSets.StatusCodeRow)
            row.canAppeal = _canAppeal
            row.compo = _compo
            row.description = _description
            row.groupId = _groupId
            row.isApproved = _isApproved
            row.isFinal = _isFinal
            row.moduleId = _moduleId
            row.statusId = _statusId
            row.groupName = _groupName
            row.moduleName = _moduleName
            row.compoDescr = _compoDescr
            row.FullDescription = _fullDescr
            row.displayOrder = _displayOrder
            row.isDisposition = _isDisposition
            row.isFormal = _isFormal
        End Sub

    End Class

End Namespace