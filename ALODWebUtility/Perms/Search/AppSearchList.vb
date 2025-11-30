Imports System.Data.Common
Imports System.Web
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Permission.Search

    Public Class AppSearchList
        Implements IList(Of AppSearch)

        Protected _adapter As SqlDataStore

        Private _maxResults As Integer = 0

        Dim _search As New List(Of AppSearch)

        Public Property MaxResults() As Integer
            Get
                Return _maxResults
            End Get
            Set(ByVal value As Integer)
                _maxResults = value
            End Set
        End Property

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Function GetActiveWorkOrders(ByVal status As Short, ByVal workflow As Byte, ByVal formal As Short, ByVal overdue As Short) As AppSearchList
            Dim cmd As DbCommand = DataStore.GetStoredProcCommand("form2173_sp_SearchActiveWorkOrders")

            DataStore.AddInParameter(cmd, "@userId", DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            DataStore.AddInParameter(cmd, "@status", DbType.Int16, status)
            DataStore.AddInParameter(cmd, "@workflow", DbType.Byte, IIf(workflow <> 0, workflow, DBNull.Value))
            DataStore.AddInParameter(cmd, "@formal", DbType.Int16, IIf(formal <> -1, formal, DBNull.Value))
            DataStore.AddInParameter(cmd, "@overdue", DbType.Int16, IIf(overdue <> -1, overdue, DBNull.Value))

            DataStore.ExecuteReader(AddressOf CodeReader, cmd)
            Return Me

        End Function

        Public Function GetActiveWorkOrdersDataSet(ByVal status As Short, ByVal workflow As Byte, ByVal formal As Short, ByVal overdue As Short) As DataSets.SearchResultDataTable
            Return GetActiveWorkOrders(status, workflow, formal, overdue).ToDataSet()
        End Function

        '************Temp Function to be deleted**********************
        Public Function GetAllLodsAsDataSet(ByVal type As ModuleType, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal workflow As String = "") As DataSets.SearchResultDataTable
            Dim scriptName As String = String.Empty
            Dim reader As SqlDataStore.RowReader = AddressOf ResultReader

            Select Case type
                Case ModuleType.LOD
                    scriptName = "form348_sp_Search"

            End Select

            Return GetAllLods(reader, scriptName, caseId, ssn, name, statusId, workflow).ToDataSet()

        End Function

        Public Function GetSearchResultsForGroup(ByVal type As ModuleType, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal AOR As String = "", Optional ByVal Tricare As String = "", Optional ByVal workflow As String = "") As AppSearchList

            'New parameter workflow is used in the group search for LOD and so  is added in form2173_GroupSearch stored procs only
            Dim scriptName As String = String.Empty
            Dim reader As SqlDataStore.RowReader = AddressOf CodeReader

            Select Case type
                Case ModuleType.LOD
                    scriptName = "form2173_GroupSearch"
                Case Else
            End Select

            Return GetSearchResult(reader, scriptName, caseId, ssn, name, statusId, AOR, Tricare, workflow)
        End Function

        Public Function GetSearchResultsForGroupAsDataSet(ByVal type As ModuleType, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal AOR As String = "", Optional ByVal Tricare As String = "", Optional ByVal workflow As String = "") As DataSets.SearchResultDataTable
            Return GetSearchResultsForGroup(type, caseId, ssn, name, statusId, AOR, Tricare, workflow).ToDataSet()
        End Function

        Public Function GetSearchResultsForUserAsDataSet(ByVal type As ModuleType, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal AOR As String = "", Optional ByVal Tricare As String = "", Optional ByVal workflow As String = "") As DataSets.SearchResultDataTable

            Dim scriptName As String = String.Empty
            Dim reader As SqlDataStore.RowReader = AddressOf CodeReader
            'New parameter workflow is used in the group search for LOD and so  is added in form2173_Search stored procs only

            Select Case type
                Case ModuleType.LOD
                    scriptName = "form2173_Search"
                Case Else
            End Select

            Return GetSearchResult(reader, scriptName, caseId, ssn, name, statusId, AOR, Tricare, workflow).ToDataSet()

        End Function

        Public Function SearchForEligibleMMSO(ByVal searchTerm As String) As AppSearchList
            DataStore.ExecuteReader(AddressOf CodeReader, "mmso_sp_newCase", searchTerm, CInt(HttpContext.Current.Session("UserId")))
            Return Me
        End Function

        Public Function ToDataSet() As DataSets.SearchResultDataTable

            Dim data As New DataSets.SearchResultDataTable
            Dim row As DataSets.SearchResultRow

            For Each search As AppSearch In _search
                row = data.NewSearchResultRow()
                search.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Private Sub CodeReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            'The following TSQLs use this reader:
            '1. form2173_Search
            '2. form2173_GroupSearch
            '2. mmso_sp_searchMMSO
            '3. mmso_sp_search
            '4. Incap_sp_search
            '5. Incap_sp_groupSearch
            '6. Incap_extension_sp_search
            '7. Incap_extension_sp_groupSearch()
            '8. appeals_sp_search
            '9. appeals_sp_Group_search
            '10. PDHRA_sp_search
            '11. PDHRA_sp_groupSearch
            '12. form2173_sp_SearchActiveWorkOrders
            '13. mmso_sp_newCase

            Dim listSearch As New AppSearch()

            listSearch.refID = adapter.GetInt32(reader, 0)
            listSearch.recID = adapter.GetInt32(reader, 1)
            listSearch.parentID = adapter.GetInt32(reader, 2)

            listSearch.CaseId = adapter.GetString(reader, 3)
            listSearch.WorkflowId = adapter.GetByte(reader, 11)
            listSearch.Workflow = adapter.GetString(reader, 12)
            listSearch.Name = adapter.GetString(reader, 7)
            listSearch.Uic = adapter.GetString(reader, 5)
            listSearch.Region = adapter.GetString(reader, 10)
            listSearch.Status = adapter.GetString(reader, 6)
            listSearch.Compo = adapter.GetString(reader, 13)
            listSearch.DateCreated = adapter.GetDateTime(reader, 14, Now)
            listSearch.CanView = adapter.GetBoolean(reader, 15)
            listSearch.CanEdit = adapter.GetBoolean(reader, 16)
            listSearch.ModuleId = adapter.GetByte(reader, 17)
            listSearch.BaseType = adapter.GetByte(reader, 18)
            listSearch.Returned = adapter.GetBoolean(reader, 19)
            listSearch.IsFormal = adapter.GetBoolean(reader, 20)
            listSearch.DateReceived = adapter.GetDateTime(reader, 21)
            listSearch.IsFinal = adapter.GetBoolean(reader, 22)

            _search.Add(listSearch)

        End Sub

        Private Function GetAllLods(ByVal rowReader As SqlDataStore.RowReader, ByVal scriptName As String, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal workFlow As String = "") As AppSearchList

            Dim adapter As SqlDataStore = DataStore
            Dim cmd As DbCommand = adapter.GetStoredProcCommand(scriptName)
            'IIf works because there are default values, so the objects are set
            adapter.AddInParameter(cmd, "@caseID", DbType.String, IIf(Not IsNothing(caseId), caseId, DBNull.Value))
            adapter.AddInParameter(cmd, "@ssn", DbType.String, IIf(Not IsNothing(ssn), ssn, DBNull.Value))
            adapter.AddInParameter(cmd, "@name", DbType.String, IIf(Not IsNothing(name), name, DBNull.Value))
            adapter.AddInParameter(cmd, "@status", DbType.Int32, IIf(statusId <> 0, statusId, DBNull.Value))
            adapter.AddInParameter(cmd, "@userId", DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            adapter.AddInParameter(cmd, "@compo", DbType.String, CStr(HttpContext.Current.Session("Compo")))
            adapter.AddInParameter(cmd, "@maxCount", DbType.Int32, _maxResults)

            adapter.ExecuteReader(rowReader, cmd)

            Return Me

        End Function

        Private Function GetSearchResult(ByVal rowReader As SqlDataStore.RowReader, ByVal scriptName As String, Optional ByVal caseId As String = "", Optional ByVal ssn As String = "", Optional ByVal name As String = "", Optional ByVal statusId As Integer = 0, Optional ByVal AOR As String = "", Optional ByVal Tricare As String = "", Optional ByVal workFlow As String = "") As AppSearchList

            Dim adapter As SqlDataStore = DataStore
            Dim cmd As DbCommand = adapter.GetStoredProcCommand(scriptName)
            'IIf works because there are default values, so the objects are set
            adapter.AddInParameter(cmd, "@caseID", DbType.String, IIf(Not IsNothing(caseId), caseId, DBNull.Value))
            adapter.AddInParameter(cmd, "@ssn", DbType.String, IIf(Not IsNothing(ssn), ssn, DBNull.Value))
            adapter.AddInParameter(cmd, "@name", DbType.String, IIf(Not IsNothing(name), name, DBNull.Value))
            adapter.AddInParameter(cmd, "@status", DbType.Int32, IIf(statusId <> 0, statusId, DBNull.Value))
            adapter.AddInParameter(cmd, "@userId", DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            adapter.AddInParameter(cmd, "@compo", DbType.String, CStr(HttpContext.Current.Session("Compo")))
            adapter.AddInParameter(cmd, "@maxCount", DbType.Int32, _maxResults)

            adapter.AddInParameter(cmd, "@aor", DbType.String, IIf(AOR <> String.Empty, AOR, DBNull.Value))
            adapter.AddInParameter(cmd, "@tricare", DbType.String, IIf(Tricare <> String.Empty, Tricare, DBNull.Value))

            If ((scriptName = "form2173_GroupSearch") Or (scriptName = "form2173_Search")) Then
                adapter.AddInParameter(cmd, "@workflow", DbType.Byte, IIf(workFlow <> "0", workFlow, DBNull.Value))
            End If
            adapter.ExecuteReader(rowReader, cmd)

            Return Me

        End Function

        Private Sub ResultReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            Dim listSearch As New AppSearch()

            listSearch.refID = adapter.GetInt32(reader, 0)
            listSearch.recID = adapter.GetInt32(reader, 1)
            listSearch.parentID = adapter.GetInt32(reader, 2)
            listSearch.CaseId = adapter.GetString(reader, 3)
            listSearch.Name = adapter.GetString(reader, 4)
            listSearch.Compo = adapter.GetString(reader, 5)
            listSearch.Status = adapter.GetString(reader, 7)
            listSearch.IsFinal = adapter.GetBoolean(reader, 8)
            listSearch.WorkflowId = adapter.GetByte(reader, 9)
            listSearch.Workflow = adapter.GetString(reader, 10)
            listSearch.ModuleId = adapter.GetByte(reader, 11)
            listSearch.IsFormal = adapter.GetBoolean(reader, 12)
            listSearch.Uic = ""
            listSearch.Region = ""
            listSearch.DateCreated = adapter.GetDateTime(reader, 13, Now)
            listSearch.CanView = adapter.GetBoolean(reader, 14)
            listSearch.CanEdit = adapter.GetBoolean(reader, 15)

            _search.Add(listSearch)

        End Sub

        '****************End Temp Function*****************

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of AppSearch).Count
            Get
                Return _search.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of AppSearch).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As AppSearch Implements System.Collections.Generic.IList(Of AppSearch).Item
            Get
                Return _search.Item(index)
            End Get
            Set(ByVal value As AppSearch)
                _search.Item(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As AppSearch) Implements System.Collections.Generic.ICollection(Of AppSearch).Add
            _search.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of AppSearch).Clear
            _search.Clear()
        End Sub

        Public Function Contains(ByVal item As AppSearch) As Boolean Implements System.Collections.Generic.ICollection(Of AppSearch).Contains
            Return _search.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As AppSearch, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of AppSearch).CopyTo
            _search.CopyTo(array)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of AppSearch) Implements System.Collections.Generic.IEnumerable(Of AppSearch).GetEnumerator
            Return _search.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _search.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As AppSearch) As Integer Implements System.Collections.Generic.IList(Of AppSearch).IndexOf
            Return _search.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As AppSearch) Implements System.Collections.Generic.IList(Of AppSearch).Insert
            _search.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As AppSearch) As Boolean Implements System.Collections.Generic.ICollection(Of AppSearch).Remove
            Return _search.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of AppSearch).RemoveAt
            _search.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace