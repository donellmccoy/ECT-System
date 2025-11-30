Imports ALOD.Data

Namespace Worklfow

    Public Class WorkOptionRuleList
        Implements IList(Of WorkOptionRule)

        Protected _list As List(Of WorkOptionRule)

        Dim _adapter As SqlDataStore

        Public Sub New()
            _list = New List(Of WorkOptionRule)
        End Sub

        Protected ReadOnly Property Adapter() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        ''' <summary>
        ''' Get list of rules by workflowid and status in
        ''' </summary>
        ''' <param name="workflowId"></param>
        ''' <param name="statusIn"></param>
        ''' <remarks></remarks>
        Public Function GetListOfRules(ByVal workflowId As Integer, ByVal statusIn As Integer) As WorkOptionRuleList
            Adapter.ExecuteReader(AddressOf RuleReader, "core_workflowRules_sp_GetRules", workflowId, statusIn)
            Return Me
        End Function

        Protected Sub RuleReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            Dim _rules As New WorkOptionRule

            _rules.OptionId = adapter.GetInt32(reader, 0)
            _rules.RuleKey = adapter.GetString(reader, 1)
            _rules.RuleValue = adapter.GetString(reader, 2)
            _rules.RuleTypeID = adapter.GetByte(reader, 3)

            _list.Add(_rules)
        End Sub

#Region "Ilist"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkOptionRule).Count
            Get
                Return _list.Count()
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkOptionRule).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As WorkOptionRule Implements System.Collections.Generic.IList(Of WorkOptionRule).Item
            Get
                Return _list.Item(index)
            End Get
            Set(ByVal value As WorkOptionRule)
                _list.Item(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As WorkOptionRule) Implements System.Collections.Generic.ICollection(Of WorkOptionRule).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkOptionRule).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkOptionRule) As Boolean Implements System.Collections.Generic.ICollection(Of WorkOptionRule).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkOptionRule, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkOptionRule).CopyTo
            _list.CopyTo(array)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkOptionRule) Implements System.Collections.Generic.IEnumerable(Of WorkOptionRule).GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As WorkOptionRule) As Integer Implements System.Collections.Generic.IList(Of WorkOptionRule).IndexOf
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As WorkOptionRule) Implements System.Collections.Generic.IList(Of WorkOptionRule).Insert
            _list.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As WorkOptionRule) As Boolean Implements System.Collections.Generic.ICollection(Of WorkOptionRule).Remove
            Return _list.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of WorkOptionRule).RemoveAt
            _list.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace