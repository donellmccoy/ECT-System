Imports System.Text
Imports ALOD.Core.Domain.Users
Imports ALOD.Data

Namespace Common

    <Serializable()>
    Public Class pasCodeList
        Implements IList(Of pascode)
        Protected _pasCodes As List(Of pascode)

#Region "Properties"

        Default Public Property Item(ByVal strpasCode As String) As pascode
            Get
                For Each iter As pascode In _pasCodes
                    If (iter.Value.ToUpper() = strpasCode.ToUpper()) Then
                        Return iter
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal value As pascode)
                For Each iter As pascode In _pasCodes
                    If (iter.Value.ToUpper() = strpasCode.ToUpper()) Then
                        iter = value
                    End If
                Next
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            _pasCodes = New List(Of pascode)
        End Sub

        Public Sub New(ByVal pCodeList As pasCodeList)

            _pasCodes = New List(Of pascode)

            For Each pCode As pascode In pCodeList

                _pasCodes.Add(New pascode(pCode))
            Next
        End Sub

#End Region

#Region "HelperFunctions"

        Public Function GetPasCodeString(ByVal seperator As String) As String

            Dim buffer As New StringBuilder

            For Each item As pascode In _pasCodes
                buffer.Append(item.Value)
                buffer.Append(seperator)
            Next

            If (buffer.Length > 0) Then
                buffer = buffer.Remove(buffer.Length - 1, 1)
            End If

            Return buffer.ToString()

        End Function

        Public Sub ResetStatus()

            For Each pCode As pascode In _pasCodes
                pCode.Status = AccessStatus.Pending
            Next

        End Sub

#End Region

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of pascode).Count
            Get
                Return _pasCodes.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of pascode).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As pascode Implements System.Collections.Generic.IList(Of pascode).Item
            Get
                Return _pasCodes(index)
            End Get
            Set(ByVal value As pascode)
                _pasCodes(index) = value
            End Set
        End Property

        Public Sub Add(ByVal strpasCode As String, ByVal status As AccessStatus)
            _pasCodes.Add(New pascode(strpasCode, status))
        End Sub

        Public Sub Add(ByVal item As pascode) Implements System.Collections.Generic.ICollection(Of pascode).Add
            _pasCodes.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of pascode).Clear
            _pasCodes.Clear()
        End Sub

        Public Function Contains(ByVal item As pascode) As Boolean Implements System.Collections.Generic.ICollection(Of pascode).Contains
            Return _pasCodes.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As pascode, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of pascode).CopyTo
            _pasCodes.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of pascode) Implements System.Collections.Generic.IEnumerable(Of pascode).GetEnumerator
            Return _pasCodes.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _pasCodes.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As pascode) As Integer Implements System.Collections.Generic.IList(Of pascode).IndexOf
            Return _pasCodes.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As pascode) Implements System.Collections.Generic.IList(Of pascode).Insert
            _pasCodes.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As pascode) As Boolean Implements System.Collections.Generic.ICollection(Of pascode).Remove
            Return _pasCodes.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of pascode).RemoveAt
            _pasCodes.RemoveAt(index)
        End Sub

#End Region

#Region "Helper"

        Public Function Contains(ByVal strpasCode As String) As Boolean

            For Each item As pascode In _pasCodes

                If (strpasCode.ToUpper() = item.Value.ToUpper()) Then
                    Return True
                End If

            Next
            Return False

        End Function

#End Region

#Region "Save"

        Public Sub Save(ByVal roleId As Integer, ByVal adapter As SqlDataStore)

            Dim xml As New XMLString("PasCodeList")

            For Each item As pascode In _pasCodes
                xml.BeginElement("PasCode")
                xml.WriteAttribute("id", item.Value.ToUpper())
                xml.WriteAttribute("status", CStr(item.Status))
                xml.EndElement()
            Next

            adapter.ExecuteNonQuery("core_role_sp_UpdatePasCodes", roleId, xml.Value)

        End Sub

#End Region

    End Class

End Namespace