Imports System.Collections.Specialized
Imports System.Text
Imports ALOD.Core.Utils

Namespace Reports

    Public Class QuerySource

        Private _alias As String = ""
        Private _fields As New StringDictionary
        Private _join As String = ""
        Private _joinType As JoinType
        Private _orderFields As New StringCollection
        Private _table As String = ""

        Public ReadOnly Property FieldList() As String
            Get
                Dim buffer As New StringBuilder

                For Each pair As DictionaryEntry In _fields
                    If (pair.Value.ToString().Length > 0) Then
                        buffer.Append(_alias + "." + pair.Key.ToString() + " AS '" + pair.Value.ToString() + "', ")
                    Else
                        buffer.Append(_alias + "." + pair.Key.ToString() + ", ")
                    End If
                Next

                Return buffer.ToString()

            End Get
        End Property

        Public Property Fields() As StringDictionary
            Get
                Return _fields
            End Get
            Set(ByVal value As StringDictionary)
                _fields = value
            End Set
        End Property

        Public ReadOnly Property IsJoin() As Boolean
            Get
                Return _join.Length > 0
            End Get
        End Property

        Public Property JoinCondition() As String
            Get
                Return _join
            End Get
            Set(ByVal value As String)
                _join = value
            End Set
        End Property

        Public Property JoinType() As JoinType
            Get
                Return _joinType
            End Get
            Set(ByVal value As JoinType)
                _joinType = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _table
            End Get
            Set(ByVal value As String)
                _table = value
            End Set
        End Property

        Public ReadOnly Property OrderFieldList() As String
            Get
                Dim buffer As New StringBuilder

                For Each pair As String In _orderFields
                    buffer.Append(_alias + "." + pair + ", ")
                Next

                Return buffer.ToString()

            End Get
        End Property

        Public Property OrderFields() As StringCollection
            Get
                Return _orderFields
            End Get
            Set(ByVal value As StringCollection)
                _orderFields = value
            End Set
        End Property

        Public Property TableAlias() As String
            Get
                Return _alias
            End Get
            Set(ByVal value As String)
                _alias = value
            End Set
        End Property

        Public Sub AddField(ByVal name As String)
            _fields.Add(name, "")
        End Sub

        Public Sub AddField(ByVal name As String, ByVal display As String)
            _fields.Add(name, display)
        End Sub

        Public Sub AddFields(ByVal ParamArray name As String())
            For Each item As String In name
                AddField(item)
            Next
        End Sub

        Public Sub AddOrderField(ByVal name As String)
            _orderFields.Add(name)
        End Sub

        Public Sub AddOrderFields(ByVal ParamArray name As String())
            For Each item As String In name
                AddOrderField(item)
            Next
        End Sub

    End Class

End Namespace