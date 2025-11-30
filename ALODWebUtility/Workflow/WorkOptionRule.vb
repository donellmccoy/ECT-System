Imports ALOD.Core.Utils
Imports ALOD.Data

Namespace Worklfow

    Public Class WorkOptionRule
        Protected _id As Integer
        Protected _optionId As Integer
        Protected _ruleKey As String
        Protected _ruleType As RuleType
        Protected _ruleValue As String

        Public Sub New()
            _id = 0
            _optionId = 0
            _ruleKey = String.Empty
            _ruleValue = String.Empty
        End Sub

        Public Sub New(ByVal optionid As Integer, ByVal ruleKey As String, ByVal ruleVal As String, ByVal ruleType As RuleType)
            _optionId = optionid
            _ruleKey = ruleKey
            _ruleValue = ruleVal
            _ruleType = ruleType
        End Sub

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property OptionId() As Integer
            Get
                Return _optionId
            End Get
            Set(ByVal value As Integer)
                _optionId = value
            End Set
        End Property

        Public Property RuleKey() As String
            Get
                Return _ruleKey
            End Get
            Set(ByVal value As String)
                _ruleKey = value
            End Set
        End Property

        Public Property RuleTypeID() As RuleType
            Get
                Return _ruleType
            End Get
            Set(ByVal value As RuleType)
                _ruleType = value
            End Set
        End Property

        Public Property RuleValue() As String
            Get
                Return _ruleValue
            End Get
            Set(ByVal value As String)
                _ruleValue = value
            End Set
        End Property

        Public Sub Save()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_workOptionRules_sp_Save", _optionId, _ruleKey, _ruleValue, _ruleType)
        End Sub

    End Class

End Namespace