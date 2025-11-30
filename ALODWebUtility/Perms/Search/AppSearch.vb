Imports ALOD.Core.Domain.Workflow

Namespace Permission.Search

    Public Class AppSearch

        Private _baseType As ModuleType
        Private _canEdit As Boolean
        Private _canView As Boolean
        Private _caseId As String
        Private _compo As String
        Private _dateCreated As Date
        Private _dateFinal As Date
        Private _dateReceived As Date

        'used by appeals
        Private _isFinal As Boolean

        Private _isFormal As Byte
        Private _name As String
        Private _parentId As Integer
        Private _recId As Integer
        Private _refId As Integer
        Private _region As String
        Private _regionId As Integer
        Private _returned As Boolean
        Private _ssn As String
        Private _status As String
        Private _statusId As Integer
        Private _type As ModuleType
        Private _uic As String
        Private _workflow As String
        Private _workflowId As Short

        Public Sub New()
            _caseId = String.Empty
            _workflow = String.Empty
            _workflowId = 0
            _name = String.Empty
            _uic = String.Empty
            _region = String.Empty
            _statusId = 0
            _status = String.Empty
            _ssn = String.Empty
            _type = 0
            _recId = 0
            _refId = 0
            _parentId = 0
            _compo = String.Empty
            _canView = False
            _canEdit = False
            _isFinal = False

        End Sub

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

        Public Property CanView() As Boolean
            Get
                Return _canView
            End Get
            Set(ByVal value As Boolean)
                _canView = value
            End Set
        End Property

        Public Property CaseId() As String
            Get
                Return _caseId
            End Get
            Set(ByVal value As String)
                _caseId = value
            End Set
        End Property

        Public Property Compo() As String
            Get
                Return Me._compo
            End Get
            Set(ByVal value As String)
                Me._compo = value
            End Set
        End Property

        Public Property DateCreated() As Date
            Get
                Return _dateCreated
            End Get
            Set(ByVal value As Date)
                _dateCreated = value
            End Set
        End Property

        Public Property DateFinal() As Date
            Get
                Return _dateFinal
            End Get
            Set(ByVal value As Date)
                _dateFinal = value
            End Set
        End Property

        Public Property DateReceived() As Date
            Get
                Return _dateReceived
            End Get
            Set(ByVal value As Date)
                _dateReceived = value
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

        Public Property IsFormal() As Byte
            Get
                Return _isFormal
            End Get
            Set(ByVal value As Byte)
                _isFormal = value
            End Set
        End Property

        Public Property ModuleId() As Byte
            Get
                Return Me._type
            End Get
            Set(ByVal value As Byte)
                Me._type = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return Me._name
            End Get
            Set(ByVal value As String)
                Me._name = value
            End Set
        End Property

        Public ReadOnly Property Overdue() As Boolean
            Get
                Dim elapsed As TimeSpan = Now.Subtract(_dateCreated)

                Select Case _type
                    Case ModuleType.LOD
                        If (Not _isFinal) Then
                            If (_isFormal) Then
                                Return (elapsed.TotalDays > 70)
                            Else
                                Return (elapsed.TotalDays > 45)
                            End If
                        End If
                    Case Else
                        Return False
                End Select
                Return False
            End Get
        End Property

        Public Property parentID() As Integer
            Get
                Return Me._parentId
            End Get
            Set(ByVal value As Integer)
                Me._parentId = value
            End Set
        End Property

        Public Property recID() As Integer
            Get
                Return Me._recId
            End Get
            Set(ByVal value As Integer)
                Me._recId = value
            End Set
        End Property

        Public Property refID() As Integer
            Get
                Return Me._refId
            End Get
            Set(ByVal value As Integer)
                Me._refId = value
            End Set
        End Property

        Public Property Region() As String
            Get
                Return Me._region
            End Get
            Set(ByVal value As String)
                Me._region = value
            End Set
        End Property

        Public Property RegionId() As Integer
            Get
                Return _regionId
            End Get
            Set(ByVal value As Integer)
                _regionId = value
            End Set
        End Property

        Public Property Returned() As Boolean
            Get
                Return _returned
            End Get
            Set(ByVal value As Boolean)
                _returned = value
            End Set
        End Property

        Public Property SSN() As String
            Get
                Return Me._ssn
            End Get
            Set(ByVal value As String)
                Me._ssn = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return Me._status
            End Get
            Set(ByVal value As String)
                Me._status = value
            End Set
        End Property

        Public Property StatusId() As Integer
            Get
                Return Me._statusId
            End Get
            Set(ByVal value As Integer)
                Me._statusId = value
            End Set
        End Property

        Public Property Uic() As String
            Get
                Return Me._uic
            End Get
            Set(ByVal value As String)
                Me._uic = value
            End Set
        End Property

        Public Property Workflow() As String
            Get
                Return Me._workflow
            End Get
            Set(ByVal value As String)
                Me._workflow = value
            End Set
        End Property

        Public Property WorkflowId() As Short
            Get
                Return _workflowId
            End Get
            Set(ByVal value As Short)
                _workflowId = value
            End Set
        End Property

        Public Sub ToDataRow(ByRef row As DataSets.SearchResultRow)

            row.caseId = _caseId
            row.recId = _recId
            row.parentId = _parentId
            row.workflow = _workflow
            row.workflowId = _workflowId
            row.name = _name
            row.uic = _uic
            row.regions = _region
            row.status = _status
            row.refID = _refId
            row.compo = _compo
            row.canEdit = _canEdit
            row.canView = _canView
            row.moduleId = _type
            row.dateFinal = _dateFinal
            row.baseType = _baseType
            row.baseType = _baseType
            row.dateCreated = DateCreated
            row.returned = _returned
            row.overdue = Overdue
            row.isFormal = _isFormal
            row.dateReceived = _dateReceived
            row.isFinal = _isFinal
        End Sub

    End Class

End Namespace