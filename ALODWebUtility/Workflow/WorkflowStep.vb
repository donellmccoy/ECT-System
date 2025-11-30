Imports ALOD.Core.Domain.DBSign
Imports ALOD.Data

Namespace Worklfow

    <Serializable()>
    Public Class WorkflowStep

#Region "Members/Properties"

        Private Const _sep As String = ";"
        Private _actionCount As Integer = 0
        Private _active As Boolean
        Dim _adapter As SqlDataStore
        Private _dbSignTemplate As DBSignTemplateId
        Private _deathStatus As Char
        Private _displayOrder As Byte = 0
        Private _groupInDescr As String = String.Empty
        Private _groupInId As Byte
        Private _groupOutDescr As String = String.Empty
        Private _groupOutId As Byte
        Private _id As Short = 0
        Private _memoTemplate As Byte = 0
        Private _memoTemplateName As String = String.Empty
        Private _statusIn As Integer = 0
        Private _statusInDescr As String = String.Empty
        Private _statusOut As Integer = 0
        Private _statusOutDescr As String = String.Empty
        Private _text As String = String.Empty
        Private _workflow As Byte = 0
        Private _workflowDescr As String = String.Empty

        Public Property ActionCount() As Integer
            Get
                Return _actionCount
            End Get
            Set(ByVal value As Integer)
                _actionCount = value
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

        Public Property DBSignTemplate() As DBSignTemplateId
            Get
                Return _dbSignTemplate
            End Get
            Set(ByVal value As DBSignTemplateId)
                _dbSignTemplate = value
            End Set
        End Property

        Public Property DeathStatus() As Char
            Get
                Return _deathStatus
            End Get
            Set(ByVal value As Char)
                _deathStatus = value
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

        Public Property GroupInDescr() As String
            Get
                Return _groupInDescr
            End Get
            Set(ByVal value As String)
                _groupInDescr = value
            End Set
        End Property

        Public Property GroupInId() As Byte
            Get
                Return _groupInId
            End Get
            Set(ByVal value As Byte)
                _groupInId = value
            End Set
        End Property

        Public Property GroupOutDescr() As String
            Get
                Return _groupOutDescr
            End Get
            Set(ByVal value As String)
                _groupOutDescr = value
            End Set
        End Property

        Public Property GroupOutId() As Byte
            Get
                Return _groupOutId
            End Get
            Set(ByVal value As Byte)
                _groupOutId = value
            End Set
        End Property

        Public Property Id() As Short
            Get
                Return _id
            End Get
            Set(ByVal value As Short)
                _id = value
            End Set
        End Property

        Public Property MemoTemplate() As Byte
            Get
                Return _memoTemplate
            End Get
            Set(ByVal value As Byte)
                _memoTemplate = value
            End Set
        End Property

        Public Property MemoTemplateName() As String
            Get
                Return _memoTemplateName
            End Get
            Set(ByVal value As String)
                _memoTemplateName = value
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

        Public Property StatusInDescription() As String
            Get
                Return _statusInDescr
            End Get
            Set(ByVal value As String)
                _statusInDescr = value
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

        Public Property StatusOutDescription() As String
            Get
                Return _statusOutDescr
            End Get
            Set(ByVal value As String)
                _statusOutDescr = value
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

        Public Property Workflow() As Byte
            Get
                Return _workflow
            End Get
            Set(ByVal value As Byte)
                _workflow = value
            End Set
        End Property

        Protected ReadOnly Property Adapter() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal value As String)

            Dim parts() As String = value.Split(_sep)

            If (parts.Length <> 4) Then
                Exit Sub
            End If

            Integer.TryParse(parts(0), _id)
            Integer.TryParse(parts(1), _statusOut)
            Byte.TryParse(parts(2), _dbSignTemplate)
            Byte.TryParse(parts(3), _memoTemplate)

        End Sub

#End Region

        Public Sub Delete()

            If (_id = 0) Then
                Exit Sub
            End If

            Dim cmd As System.Data.Common.DbCommand = Adapter.GetSqlStringCommand(
                "DELETE FROM core_workflowSteps WHERE stepId = @stepId")
            Adapter.AddInParameter(cmd, "@stepId", Data.DbType.Int16, _id)
            Adapter.ExecuteNonQuery(cmd)

        End Sub

        Public Function Save() As Boolean
            If (_id = 0) Then
                Return Insert()
            Else
                Return Update()
            End If
        End Function

        Public Sub ToDataRow(ByVal row As DataSets.WorkflowStepsRow)
            row.id = _id
            row.active = _active
            row.displayOrder = _displayOrder
            row.statusIn = _statusIn
            row.statusInDescription = _statusInDescr
            row.statusOut = _statusOut
            row.statusOutDescription = _statusOutDescr
            row.text = _text
            row.workflow = _workflow
            row.workflowDescription = _workflowDescr
            row.groupInId = _groupInId
            row.groupInDescr = _groupInDescr
            row.groupOutId = _groupOutId
            row.groupOutDescr = _groupOutDescr
            row.dbSignTemplate = _dbSignTemplate
            row.actionCount = _actionCount
            row.deathStatus = _deathStatus
            row.memoTemplate = _memoTemplate
        End Sub

        Public Function ToValueString() As String
            Return ToValueString(";")
        End Function

        Public Function ToValueString(ByVal seperator As String) As String
            Return _id.ToString() + seperator + _statusOut.ToString() + seperator + CStr(_dbSignTemplate) + seperator + _memoTemplate.ToString()
        End Function

        Protected Function Insert() As Boolean
            Return (CInt(Adapter.ExecuteScalar("core_workflow_sp_InsertStep",
                _workflow, _statusIn, _statusOut, _text,
                0, _active, _displayOrder, _dbSignTemplate, _deathStatus,
                IIf(_memoTemplate <> 0, _memoTemplate, DBNull.Value))) > 0)
        End Function

        Protected Function Update() As Boolean
            Return (CInt(Adapter.ExecuteNonQuery("core_workflow_sp_UpdateStep",
                _id, _workflow, _statusIn, _statusOut, _text,
                0, _active, _displayOrder, _dbSignTemplate, _deathStatus,
                IIf(_memoTemplate <> 0, _memoTemplate, DBNull.Value))) > 0)
        End Function

    End Class

End Namespace