Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Web.UI.WebControls
Imports System.Xml
Imports AjaxControlToolkit
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Data

Namespace LookUps

    Public Class LookUp
        Inherits LookUpTable

        'Public Function GetMemberTypes() As LookUp
        '    Dim cmd As DbCommand = DataStore.GetSqlStringCommand("select  memberType, memberDescr from core_lkupMemberType Order By id")
        '    GetCollection(DataStore, "memberType", cmd)
        '    Return Me
        'End Function

        Public Function GetCategory(ByVal headingId As Integer) As IEnumerable(Of ICD9Code)

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim query = From r In icdDao.GetAll() Where r.ParentId = headingId And r.Active Select r
            Return query

        End Function

        Public Function GetChapter() As IEnumerable(Of ICD9Code)

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim query = From r In icdDao.GetAll() Where r.Code Is Nothing And r.ParentId Is Nothing And r.Active Select r Order By r.SortOrder

            Return query

        End Function

        Public Function GetGroupsByCompo(ByVal compo As String) As LookUp
            Dim cmd As DbCommand = DataStore.GetStoredProcCommand("core_group_sp_GetByCompo")
            DataStore.AddInParameter(cmd, "@compo", DbType.String, compo)
            GetCollection(DataStore, "groups_compo_" + compo, cmd)
            Return Me
        End Function

        Public Function GetGroupsByCompoAsDataSet(ByVal compo As String) As DataSets.LookupDataTable
            Return GetGroupsByCompo(compo).ToDataSet()
        End Function

        Public Function GetICDChildren(ByVal parentId As Integer) As IEnumerable(Of ICD9Code)
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim query = From r In icdDao.GetAll() Where r.ParentId = parentId And r.Active Select r
            Return query
        End Function

        Public Function GetICDCodeCDDValues(ByVal categoryValues As StringDictionary, ByVal parentCategory As String) As CascadingDropDownNameValue()

            If (Not categoryValues.ContainsKey(parentCategory)) Then
                Return Nothing
            End If

            Dim selectedParentId As Integer = CType(categoryValues(parentCategory), Integer)
            Dim children As IEnumerable(Of ICD9Code) = GetICDChildren(selectedParentId)
            Dim values As New List(Of CascadingDropDownNameValue)
            Dim text As String = String.Empty

            For Each item As ICD9Code In children
                If (String.IsNullOrEmpty(item.Code)) Then
                    text = item.Description
                Else
                    text = item.Description & " - " & item.Code
                End If

                values.Add(New CascadingDropDownNameValue(text, item.Id))
            Next

            Return values.ToArray()

        End Function

        Public Function GetICDIncident(ByVal codeId As Integer) As List(Of ListItem)
            If (IsNothing(codeId) Or codeId = 0) Then
                Return Nothing
            End If

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim code As ICD9Code = icdDao.GetById(codeId)
            Dim values As New List(Of ListItem)

            Dim incidentValues As List(Of NatureOfIncident) = icdDao.GetAssociatedNatureOfIncidentValues(codeId)

            If (incidentValues IsNot Nothing AndAlso incidentValues.Count > 0) Then
                For Each v As NatureOfIncident In incidentValues
                    values.Add(New ListItem(v.Text, v.Value))
                Next
            Else
                values.Add(New ListItem("Illness", "Illness"))
                values.Add(New ListItem("Injury", "Injury"))
                values.Add(New ListItem("Injury MVA", "Injury-MVA"))
                values.Add(New ListItem("Disease", "Disease"))
                values.Add(New ListItem("Death", "Death"))
            End If

            'If code.Id < 467 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 466 And code.Id < 475 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 474 And code.Id < 485 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id = 485 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 485 And code.Id < 493 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id = 493 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 493 And code.Id < 780 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 779 And code.Id < 800 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 799 And code.Id < 812 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 811 And code.Id < 919 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 918 And code.Id < 925 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 924 And code.Id < 982 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 981 And code.Id < 1043 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf code.Id > 1042 And code.Id < 1045 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'Else
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'End If

            'If codeId < 467 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 466 And codeId < 475 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 474 And codeId < 485 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId = 485 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 485 And codeId < 493 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId = 493 Then
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 493 And codeId < 780 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 779 And codeId < 800 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 799 And codeId < 812 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 811 And codeId < 919 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 918 And codeId < 925 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 924 And codeId < 982 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 981 And codeId < 1043 Then
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'ElseIf codeId > 1042 And codeId < 1045 Then
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Death", "Death"))
            'Else
            '    values.Add(New ListItem("Illness", "Illness"))
            '    values.Add(New ListItem("Injury", "Injury"))
            '    values.Add(New ListItem("Injury MVA", "Injury-MVA"))
            '    values.Add(New ListItem("Disease", "Disease"))
            '    values.Add(New ListItem("Death", "Death"))
            'End If

            Return values

        End Function

        Public Function GetManagedGroups(ByVal groupId As Integer) As DataSet
            Return DataStore.ExecuteDataSet("core_group_sp_GetManaged", groupId)
        End Function

        Public Function GetManagedGroupsDropDown(ByVal groupId As Integer) As DataSet
            Return DataStore.ExecuteDataSet("core_group_sp_GetManagedDropDown", groupId)
        End Function

        Public Function GetStates() As DataSet
            Dim cmd As DbCommand = DataStore.GetSqlStringCommand("select state, state_name from core_lkupStates order by country desc, state_name")
            Return DataStore.ExecuteDataSet(cmd)
        End Function

        Public Function GetViewedGroups(ByVal groupId As Integer) As DataSet
            Return DataStore.ExecuteDataSet("core_group_sp_GetViewBy", groupId)
        End Function

        Public Function GetWorkflowActionTypes() As LookUp
            Dim cmd As DbCommand = DataStore.GetSqlStringCommand("SELECT type, text FROM core_lkupWorkflowAction ORDER BY text")
            GetCollection(DataStore, "workflowActions", cmd)
            Return Me
        End Function

        Public Function GetWorkflowActionTypesAsDataSet() As DataSets.LookupDataTable
            Return GetWorkflowActionTypes().ToDataSet()
        End Function

        Public Function SearchUsers(ByVal userId As Integer, ByVal ssn As String, ByVal name As String, ByVal status As Byte, ByVal role As Integer, ByVal unitId As Integer, ByVal showAllUsers As Boolean) As DataSet
            Return DataStore.ExecuteDataSet("core_user_sp_GetManagedUsers", userId, ssn, name, status, role, unitId, showAllUsers)
        End Function

#Region "Caching"

        Public Function ToDataSet() As DataSets.LookupDataTable

            Dim data As New DataSets.LookupDataTable
            Dim row As DataSets.LookupRow

            For Each item As LookUpRow In _rows
                row = data.NewLookupRow
                row.key = item.text
                row.value = item.value
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Public Sub ToXml(ByRef parentNode As XmlNode)

            Dim doc As XmlDocument = parentNode.OwnerDocument

            Dim section As String = ""
            Dim currentId As Integer = 0

            For Each row As LookUpRow In _rows

                Dim node As XmlNode = doc.CreateElement("Row")

                node.Attributes.Append(doc.CreateAttribute("Text"))
                node.Attributes("Text").Value = row.text

                node.Attributes.Append(doc.CreateAttribute("Value"))
                node.Attributes("Value").Value = row.value

                parentNode.AppendChild(node)

            Next

        End Sub

        Protected Sub GetCollection(ByVal key As String, ByVal procName As String, ByVal ParamArray parameterValues As Object())
            GetCollection(DataStore, key, DataStore.GetStoredProcCommand(procName, parameterValues))
        End Sub

        Protected Sub GetCollection(ByVal adapter As SqlDataStore, ByVal key As String, ByVal cmd As DbCommand)

            If (Cache(key) IsNot Nothing) Then
                'we have this item in cache
                _rows = CType(Cache(key), List(Of LookUpRow))
            Else
                'get it from the db
                adapter.ExecuteReader(AddressOf LookUpRowReader, cmd)
                Cache.Insert(key, _rows)
            End If

        End Sub

#End Region

    End Class

End Namespace