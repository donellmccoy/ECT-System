Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Worklfow

    <Serializable()>
    Public Class Workflow

#Region "Members / Properties"

        Protected _actions As WorkflowActionList
        Protected _active As Boolean
        Protected _compo As String
        Protected _formal As Boolean
        Protected _id As Byte
        Protected _initialStatus As Integer
        Protected _module As ModuleType
        Protected _statusDesc As String
        Protected _statusIn As Integer
        Protected _steps As New WorkflowStepList
        Protected _title As String

        Public Property Actions() As WorkflowActionList
            Get
                Return _actions
            End Get
            Set(ByVal value As WorkflowActionList)
                _actions = value
            End Set
        End Property

        Public Property Active() As Boolean
            Get
                Return _active
            End Get
            Set(ByVal value As Boolean)
                _active = value
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

        Public ReadOnly Property FullTitle() As String
            Get
                Return _title + " [" + GetCompoAbbr(_compo) + "]"
            End Get
        End Property

        Public Property Id() As Byte
            Get
                Return _id
            End Get
            Set(ByVal value As Byte)
                _id = value
            End Set
        End Property

        Public Property InitialStatus() As Integer
            Get
                Return _initialStatus
            End Get
            Set(ByVal value As Integer)
                _initialStatus = value
            End Set
        End Property

        Public Property IsFormal() As Boolean
            Get
                Return _formal
            End Get
            Set(ByVal value As Boolean)
                _formal = value
            End Set
        End Property

        Public Property ModuleId() As Byte
            Get
                Return _module
            End Get
            Set(ByVal value As Byte)
                _module = value
            End Set
        End Property

        Public ReadOnly Property ModuleName() As String
            Get
                Return _module.ToString()
            End Get
        End Property

        Public Property StatusDescription() As String
            Get
                Return _statusDesc
            End Get
            Set(ByVal value As String)
                _statusDesc = value
            End Set
        End Property

        Public Property StatusIn() As Integer
            Get
                Return _statusIn
            End Get
            Set(ByVal value As Integer)
                _statusIn = value
            End Set
        End Property

        Public Property Steps() As WorkflowStepList
            Get
                Return _steps
            End Get
            Set(ByVal value As WorkflowStepList)
                _steps = value
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

#End Region

#Region "Constructors"

        Public Sub New()
            _id = 0
            _compo = String.Empty
            _formal = False
            _title = String.Empty
        End Sub

        Public Sub New(ByVal workflowId As Byte, Optional ByVal loadSteps As Boolean = False)

            _id = workflowId
            _steps = New WorkflowStepList
            GetDetails()

            If (loadSteps) Then
                GetAllSteps()
            End If

        End Sub

        Public Sub New(workflowId As Byte, status As Integer, deathCase As Boolean)

            _id = workflowId
            _statusIn = status
            _steps = New WorkflowStepList
            GetCurrentSteps(deathCase)

        End Sub

#End Region

#Region "Data methods"

        Public Function GetAllSteps() As WorkflowStepList
            _steps.Clear()
            _steps.GetSteps(_id)
            Return _steps
        End Function

        Public Function GetCurrentSteps(ByVal deathCase As Boolean) As WorkflowStepList
            _steps.Clear()
            _steps.GetStepsByStatus(_id, _statusIn, deathCase)
            Return _steps
        End Function

        Public Function GetCurrentSteps(status As Integer, deathCase As Boolean) As WorkflowStepList
            _statusIn = status
            _steps.Clear()
            _steps.GetStepsByStatus(_id, _statusIn, deathCase)
            Return _steps
        End Function

        Public Sub GetDetails()

            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetSqlStringCommand(
            "SELECT title, a.compo, formal, active, cast(initialStatus as int) as initialStatus, b.description " _
            + "FROM core_Workflow a JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE workflowId = @workflowId")
            adapter.AddInParameter(cmd, "@workflowId", Data.DbType.Byte, _id)
            adapter.ExecuteReader(AddressOf DetailsReader, cmd)

        End Sub

        Public Function Insert() As Boolean

            If (_title.Trim.Length = 0) OrElse (_compo.Trim.Length <> 1) Then
                Return False
            End If

            Dim adapter As New SqlDataStore()
            Return CInt(adapter.ExecuteScalar("core_workflow_sp_InsertWorkflow", _title, _module, _compo, _formal, _initialStatus)) > 0

        End Function

        Protected Sub DetailsReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-title, 1-compo, 2-formal, 3-active
            _title = adapter.GetString(reader, 0)
            _compo = adapter.GetString(reader, 1)
            _formal = adapter.GetBoolean(reader, 2)
            _active = adapter.GetBoolean(reader, 3)
            _initialStatus = adapter.GetInt32(reader, 4)
            _statusDesc = adapter.GetString(reader, 5)

        End Sub

#End Region

    End Class

End Namespace