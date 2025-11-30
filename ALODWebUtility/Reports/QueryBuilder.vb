Imports System.Data.Common
Imports System.Text
Imports ALOD.Core.Utils
Imports ALOD.Data

Namespace Reports

    Public Class QueryBuilder

        Protected _adapter As SqlDataStore
        Protected _cmd As DbCommand
        Protected _distinct As Boolean = False
        Protected _params As List(Of QueryCondition)
        Protected _query As StringBuilder
        Protected _sources As Dictionary(Of String, QuerySource)
        Protected _where As StringBuilder

        'these are for the source aliasing
        Private Shared _suffix As New StringBuilder()

        Private Shared _suffixChar As Integer = 64
        Private Shared _suffixIndex As Integer = 0

        Public Sub New()

            _adapter = New SqlDataStore()
            _query = New StringBuilder
            _where = New StringBuilder
            _sources = New Dictionary(Of String, QuerySource)
            _params = New List(Of QueryCondition)

        End Sub

        Public Property Distinct() As Boolean
            Get
                Return _distinct
            End Get
            Set(ByVal value As Boolean)
                _distinct = value
            End Set
        End Property

        Public Sub AddCondition(ByVal field As String, ByVal value As Object, ByVal valueType As DbType, ByVal filter As String)

            If (value IsNot Nothing) AndAlso (value.ToString().Length > 0) Then
                _where.Append(" AND " + filter)
                _params.Add(New QueryCondition(field, value, valueType, filter))
            End If

        End Sub

        Public Sub AddCondition(ByVal field As String, ByVal filter As String)

            _where.Append(" AND " + filter)
            _params.Add(New QueryCondition(field, filter))

        End Sub

        Public Sub AddDefaultOrder(ByVal source As String, ByVal ParamArray orderFields As String())
            _sources(source).AddOrderFields(orderFields)
        End Sub

        Public Sub AddFields(ByVal source As String, ByVal ParamArray fields As String())
            _sources(source).AddFields(fields)
        End Sub

        Public Function AddSource(ByVal sourceName As String) As QuerySource

            Dim source As New QuerySource()
            source.Name = sourceName
            source.TableAlias = GetNextAlias()
            source.JoinType = JoinType.None

            _sources.Add(sourceName, source)
            Return source

        End Function

        Public Function AddSource(ByVal sourceName As String, ByVal sourceKey As String, ByVal joinName As String, ByVal joinKey As String, ByVal join As JoinType) As QuerySource

            Dim source As New QuerySource()
            source.Name = sourceName
            source.TableAlias = GetNextAlias()
            source.JoinType = join

            Dim target As QuerySource = _sources(joinName)

            If (target Is Nothing) Then
                Return Nothing
            End If

            source.JoinCondition = source.TableAlias + "." + sourceKey + " = " + target.TableAlias + "." + joinKey

            _sources.Add(sourceName, source)
            Return source

        End Function

        Public Function AddSource(ByVal sourceName As String, ByVal joinName As String, ByVal joinCondition As String, ByVal join As JoinType) As QuerySource

            Dim source As New QuerySource()
            source.Name = sourceName
            source.TableAlias = GetNextAlias()
            source.JoinType = join

            Dim target As QuerySource = _sources(joinName)

            If (target Is Nothing) Then
                Return Nothing
            End If

            source.JoinCondition = joinCondition

            _sources.Add(sourceName, source)
            Return source

        End Function

        Public Function GetData() As DataSet

            BuildQuery()

            _cmd = _adapter.GetSqlStringCommand(_query.ToString())

            For Each condition As QueryCondition In _params
                _adapter.AddInParameter(_cmd, condition.Name, condition.ValueType, condition.Value)
            Next

            Return _adapter.ExecuteDataSet(_cmd, 180)

        End Function

        ''' <summary>
        ''' Gets the next available source alias
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Starts with [A-Z] then [AA-ZZ], etc</remarks>
        Private Shared Function GetNextAlias() As String

            _suffixChar += 1

            If _suffixChar = 65 Then
                'this is the first source
                _suffix.Append(ChrW(_suffixChar))
            ElseIf _suffixChar > 90 Then
                'we've wrapped
                _suffixIndex += 1
                _suffixChar = 65
                'reset to A
                _suffix.Append(ChrW(_suffixChar))
            End If

            _suffix(_suffixIndex) = ChrW(_suffixChar)

            Return _suffix.ToString()

        End Function

        Private Sub BuildQuery()

            Dim query As New StringBuilder
            Dim from As New StringBuilder
            Dim orderBy As New StringBuilder

            'iterate our sources building our query as we go
            For Each source As QuerySource In _sources.Values

                'append the fields for this source
                query.Append(source.FieldList)

                'append order fields
                orderBy.Append(source.OrderFieldList)

                'append the source
                If (source.JoinType = JoinType.None) Then
                    from.Append(" FROM " + source.Name + " AS " + source.TableAlias)
                Else
                    from.Append(" " + source.JoinType.ToString().ToUpper() _
                        + " JOIN " + source.Name + " AS " + source.TableAlias + " ON " + source.JoinCondition)
                End If

            Next

            If (query.Length > 0) Then
                query = query.Remove(query.Length - 2, 1) 'remove the trailing ,
            End If

            If (orderBy.Length > 0) Then
                orderBy = orderBy.Remove(orderBy.Length - 2, 1) 'remove the trailing ,
            End If

            'now join our parts
            _query = New StringBuilder
            _query.Append("SELECT ")
            If Distinct Then
                _query.Append("DISTINCT ")
            End If
            _query.Append(query.ToString())
            _query.Append(" ")
            _query.Append(from.ToString())
            _query.Append(" WHERE 1=1 ")

            If (_where.Length > 0) Then
                _query.Append(_where.ToString())
            End If

            If (orderBy.ToString.Length > 0) Then
                _query.Append(" ORDER BY ")
                _query.Append(orderBy.ToString)
            End If

        End Sub

    End Class

End Namespace