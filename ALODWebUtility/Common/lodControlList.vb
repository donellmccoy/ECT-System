Imports System.Text
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Logging

Namespace Common

    <Serializable()>
    Public Class lodControlList
        Implements IList(Of lodControl)
        Protected _lodcontrols As List(Of lodControl)

        Public Sub New()
            _lodcontrols = New List(Of lodControl)
            _lodcontrols.Clear()
        End Sub

        Public Sub Create(ByVal ctls As System.Web.UI.ControlCollection)
            ' Set the Initial Hash Value
            Dim tmpVal As String
            tmpVal = ControlValues.GetKeyValues(ctls, Me, "")

        End Sub

        Public Function LogChanges(newList As lodControlList, modtype As ModuleType, actionType As UserAction, refId As Integer, comment As String, status As Integer) As Boolean
            Dim i As Integer
            Dim name As String = ""
            Dim oldldc As lodControl
            Dim changes As New ChangeSet
            i = 0

            If (newList.Count <> _lodcontrols.Count) Then
                'There is some error since the control count is not same
                Return False
            End If

            For Each newldc As lodControl In newList
                oldldc = _lodcontrols.Item(i)
                If (newldc.Name <> oldldc.Name()) Then
                    'There is some error since the control names are  not same so do not record the change set
                    Return False
                End If
                i = i + 1
            Next

            i = 0
            For Each newldc As lodControl In newList
                oldldc = _lodcontrols.Item(i)
                If (newldc.Value <> oldldc.Value) Then
                    _lodcontrols.Item(i).IsModified = True
                    changes.Add(oldldc.Section, oldldc.Field, oldldc.Value, newldc.Value)
                End If
                i = i + 1
            Next
            If (changes.Count > 0) Then
                Dim actionId As Integer = LogManager.LogAction(modtype, actionType, refId, comment, status)
                changes.Save(actionId)
            End If
            Return True

        End Function

        Public Sub Read(ByVal strList As String)
            Dim i As Integer
            Dim crtls() As String
            Dim crtlDef() As String
            Dim ldc As lodControl
            crtls = strList.Split(Chr(9)) 'Tab is used as a seperator character since control values can contain this character
            Dim ncrtls As Integer

            _lodcontrols.Clear()
            ncrtls = crtls.Length
            If ncrtls > 0 Then
                For i = 0 To ncrtls - 1
                    crtlDef = crtls(i).Split(Chr(14)) 'Character -Control+T

                    ldc = New lodControl(crtlDef(0), crtlDef(1), crtlDef(2), crtlDef(3)) 'name ,val,sec,field
                    _lodcontrols.Add(ldc)
                Next
            End If

        End Sub

        Public Function ToDataSet() As DataSets.ControlLodDataTable

            Dim data As New DataSets.ControlLodDataTable
            Dim row As DataSets.ControlLodRow

            For Each ldc As lodControl In _lodcontrols
                row = data.NewControlLodRow()
                ldc.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Public Function Write() As String

            Dim buffer As New StringBuilder

            For Each ldc As lodControl In _lodcontrols
                'name ,val,sec,field
                '   buffer.Append(ldc.Name + "|$|" + ldc.Value + "|$|" + ldc.Section + "|$|" + ldc.Field)
                buffer.Append(ldc.Name + Chr(14) + ldc.Value + Chr(14) + ldc.Section + Chr(14) + ldc.Field)

                buffer.Append(Chr(9)) 'Tab is used as a seperator character since control values can contain this character
                'buffer.Append(";")
            Next

            If (buffer.Length > 0) Then
                buffer = buffer.Remove(buffer.Length - 1, 1)
            End If

            Return buffer.ToString()

        End Function

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of lodControl).Count
            Get
                Return _lodcontrols.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of lodControl).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As lodControl Implements System.Collections.Generic.IList(Of lodControl).Item
            Get
                Return _lodcontrols(index)
            End Get
            Set(ByVal value As lodControl)
                _lodcontrols(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As lodControl) Implements System.Collections.Generic.ICollection(Of lodControl).Add
            _lodcontrols.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of lodControl).Clear
            _lodcontrols.Clear()
        End Sub

        Public Function Contains(ByVal item As lodControl) As Boolean Implements System.Collections.Generic.ICollection(Of lodControl).Contains
            Return _lodcontrols.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As lodControl, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of lodControl).CopyTo
            _lodcontrols.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of lodControl) Implements System.Collections.Generic.IEnumerable(Of lodControl).GetEnumerator
            Return _lodcontrols.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _lodcontrols.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As lodControl) As Integer Implements System.Collections.Generic.IList(Of lodControl).IndexOf
            Return _lodcontrols.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As lodControl) Implements System.Collections.Generic.IList(Of lodControl).Insert
            _lodcontrols.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As lodControl) As Boolean Implements System.Collections.Generic.ICollection(Of lodControl).Remove
            Return _lodcontrols.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of lodControl).RemoveAt
            _lodcontrols.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace