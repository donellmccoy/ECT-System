Imports System.Text
Imports System.Xml
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Common

    Public Class ChangeSet
        Implements IList(Of ChangeRow)

        Protected _details As New Dictionary(Of Integer, ChangeSetDetails)
        Protected _sets As New List(Of ChangeRow)

        Public Sub Add(ByVal section As String, ByVal field As String, ByVal oldVal As String, ByVal newVal As String)

            Dim change As New ChangeRow(section, field, oldVal, newVal)
            _sets.Add(change)

        End Sub

        Public Sub GetByLogId(ByVal logId As Integer)
            _sets.Clear()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf SetReader, "core_log_sp_GetChangeSetByLogId", logId)
            GetDetails()
        End Sub

        Public Sub GetByReferenceId(ByVal refId As Integer, ByVal type As ModuleType)
            _sets.Clear()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf SetReader, "core_log_sp_GetChangeSetsByRefId", refId, type)
            GetDetails()
        End Sub

        Public Sub GetByUserID(ByVal userID As Integer)
            _sets.Clear()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf SetReader, "core_log_sp_GetChangeSetByUserId", userID)
            GetDetialsByUserId(userID)
        End Sub

        Public Sub GetDetails()

            If (_sets.Count = 0) Then
                Exit Sub
            End If

            'get a list of unique log ids
            Dim list As New List(Of Integer)

            For Each row As ChangeRow In _sets
                If (Not list.Contains(row.Id)) Then
                    list.Add(row.Id)
                End If
            Next

            Dim buffer As New StringBuilder

            For Each id As Integer In list
                buffer.Append(id.ToString() + ",")
            Next

            buffer = buffer.Remove(buffer.Length - 1, 1)

            'now get the details for this
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf DetailsReader, "core_log_sp_GetChangeSetDetails", buffer.ToString())

        End Sub

        Public Sub Save(ByVal id As Integer)

            Dim xml As New XMLString("XML_Array")

            For Each item As ChangeRow In _sets
                xml.BeginElement("XMLList")
                xml.WriteAttribute("ID", id.ToString())
                xml.WriteAttribute("section", item.Section)
                xml.WriteAttribute("field", item.Field)
                xml.WriteAttribute("oldVal", item.OldVal)
                xml.WriteAttribute("newVal", item.NewVal)
                xml.EndElement()
            Next

            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_log_sp_InsertChangeSet", xml.Value)

        End Sub

        Public Sub ToXml(ByRef parentNode As XmlNode, Optional ByVal repeatHeading As Boolean = True)

            Dim doc As XmlDocument = parentNode.OwnerDocument

            Dim section As String = ""
            Dim currentId As Integer = 0
            Dim noRepeat As Boolean = True
            Dim groupNode As XmlNode = doc.CreateElement("ChangeSet")

            For Each row As ChangeRow In _sets

                If (currentId <> row.Id And noRepeat) Then

                    currentId = row.Id
                    groupNode = doc.CreateElement("ChangeSet")

                    If (_details.ContainsKey(row.Id)) Then

                        Dim details As ChangeSetDetails = _details(row.Id)
                        noRepeat = repeatHeading
                        groupNode.Attributes.Append(doc.CreateAttribute("logId"))
                        groupNode.Attributes("logId").Value = details.LogId.ToString()

                        groupNode.Attributes.Append(doc.CreateAttribute("ActionDate"))
                        groupNode.Attributes("ActionDate").Value = details.ActionDate.ToString()

                        groupNode.Attributes.Append(doc.CreateAttribute("LastName"))
                        groupNode.Attributes("LastName").Value = details.LastName

                        groupNode.Attributes.Append(doc.CreateAttribute("FirstName"))
                        groupNode.Attributes("FirstName").Value = details.FirstName

                        groupNode.Attributes.Append(doc.CreateAttribute("Rank"))
                        groupNode.Attributes("Rank").Value = details.Rank

                        groupNode.Attributes.Append(doc.CreateAttribute("UserId"))
                        groupNode.Attributes("UserId").Value = details.UserId.ToString()

                    End If

                    parentNode.AppendChild(groupNode)

                End If

                Dim node As XmlNode = doc.CreateElement("ChangeRow")

                node.Attributes.Append(doc.CreateAttribute("Section"))
                node.Attributes("Section").Value = row.Section

                node.Attributes.Append(doc.CreateAttribute("Field"))
                node.Attributes("Field").Value = row.Field

                node.Attributes.Append(doc.CreateAttribute("OldValue"))
                node.Attributes("OldValue").Value = row.OldVal

                node.Attributes.Append(doc.CreateAttribute("NewValue"))
                node.Attributes("NewValue").Value = row.NewVal

                groupNode.AppendChild(node)

            Next

        End Sub

        Private Sub DetailsReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-logId, 1-actionDate, 2-userId, 3-lastName, 4-firstName, 5-rank

            Dim details As New ChangeSetDetails

            details.LogId = adapter.GetInteger(reader, 0)
            details.ActionDate = adapter.GetDateTime(reader, 1)
            details.UserId = adapter.GetInteger(reader, 2)
            details.LastName = adapter.GetString(reader, 3)
            details.FirstName = adapter.GetString(reader, 4)
            details.Rank = adapter.GetString(reader, 5)

            _details.Add(details.LogId, details)

        End Sub

        Private Sub GetDetialsByUserId(ByVal userId As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf DetailsReader, "core_log_sp_GetLastChange", userId)
        End Sub

        Private Sub SetReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-logId, 1-section, 2-field, 3-old, 4-new, 5-actionId, 6-actionName
            Dim change As New ChangeRow

            change.Id = adapter.GetInteger(reader, 0)
            change.Section = adapter.GetString(reader, 1)
            change.Field = adapter.GetString(reader, 2)
            change.OldVal = adapter.GetString(reader, 3)
            change.NewVal = adapter.GetString(reader, 4)
            change.ActionId = adapter.GetNumber(reader, 5)
            change.ActionName = adapter.GetString(reader, 6)
            change.UserId = adapter.GetNumber(reader, 7)
            change.UserName = adapter.GetString(reader, 8)
            change.ActionDate = adapter.GetDateTime(reader, 9)

            _sets.Add(change)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of ChangeRow).Count
            Get
                Return _sets.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of ChangeRow).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As ChangeRow Implements System.Collections.Generic.IList(Of ChangeRow).Item
            Get
                Return _sets(index)
            End Get
            Set(ByVal value As ChangeRow)
                _sets(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As ChangeRow) Implements System.Collections.Generic.ICollection(Of ChangeRow).Add
            _sets.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of ChangeRow).Clear
            _sets.Clear()
        End Sub

        Public Function Contains(ByVal item As ChangeRow) As Boolean Implements System.Collections.Generic.ICollection(Of ChangeRow).Contains
            Return _sets.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As ChangeRow, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of ChangeRow).CopyTo
            _sets.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ChangeRow) Implements System.Collections.Generic.IEnumerable(Of ChangeRow).GetEnumerator
            Return _sets.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _sets.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As ChangeRow) As Integer Implements System.Collections.Generic.IList(Of ChangeRow).IndexOf
            Return _sets.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As ChangeRow) Implements System.Collections.Generic.IList(Of ChangeRow).Insert
            _sets.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As ChangeRow) As Boolean Implements System.Collections.Generic.ICollection(Of ChangeRow).Remove
            Return _sets.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of ChangeRow).RemoveAt
            _sets.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace