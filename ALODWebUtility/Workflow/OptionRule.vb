Imports ALOD.Data

Namespace Worklfow

    Public Class OptionRule

#Region "Members"

        Protected _chkAll As Boolean = True
        Protected _ruleData As String
        Protected _ruleId As Byte
        Protected _wsoId As Int32
        Protected _wsrId As Byte = 0

#End Region

#Region "Property"

        Public Property CheckAll() As Boolean
            Get
                Return _chkAll
            End Get
            Set(ByVal value As Boolean)
                _chkAll = value
            End Set
        End Property

        Public Property RuleData() As String
            Get
                Return _ruleData
            End Get
            Set(ByVal value As String)
                _ruleData = value
            End Set
        End Property

        Public Property RuleId() As Byte
            Get
                Return _ruleId
            End Get
            Set(ByVal value As Byte)
                _ruleId = value
            End Set
        End Property

        Public Property WSOId() As Int32
            Get
                Return _wsoId
            End Get
            Set(ByVal value As Int32)
                _wsoId = value
            End Set
        End Property

        Public Property WSRId() As Byte
            Get
                Return _wsrId
            End Get
            Set(ByVal value As Byte)
                _wsrId = value
            End Set
        End Property

#End Region

#Region "Save"

        Public Sub Save()
            Dim adapter As New SqlDataStore
            adapter.ExecuteScalar("core_rules_sp_InsertOptionRule", WSOId, RuleId, RuleData, CheckAll)

        End Sub

#End Region

    End Class

End Namespace