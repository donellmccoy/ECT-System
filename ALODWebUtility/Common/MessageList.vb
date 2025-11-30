Imports System.Web
Imports ALOD.Core.Domain.Users
Imports ALOD.Data

Namespace Common

    Public Class MessageList
        Implements IList(Of Message)

        Protected _message As New List(Of Message)

#Region "Messages"

        ''' <summary>
        ''' Insert Message into Table
        ''' </summary>
        ''' <param name="title">The Message's Title</param>
        ''' <param name="name"></param>
        ''' <param name="startDate">StartDate of the Message</param>
        ''' <param name="endDate">EndDate of the Message</param>
        ''' <param name="popUp">If message is a popup</param>
        ''' <param name="message">Message</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertMessage(ByVal title As String, ByVal name As String, ByVal startDate As Date, ByVal endDate As Date, ByVal popUp As Boolean, ByVal message As String) As Integer
            Dim adapter As New SqlDataStore()
            Dim userId As Integer = CInt(HttpContext.Current.Session("UserId"))
            Return CInt(adapter.ExecuteScalar("core_messages_sp_InsertMessages", title, name, startDate, endDate, popUp, message, userId))
        End Function

        ''' <summary>
        ''' Returns a list of all messages for editing in the MsgAdmin page.
        ''' </summary>
        ''' <param name="compo"></param>
        ''' <returns>A data table containing the results.</returns>
        ''' <remarks></remarks>
        Public Function RetrieveAllMessages(ByVal compo As Integer, ByVal isAdmin As Boolean) As MessageList
            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_messages_sp_GetAllMessages", compo, isAdmin)
            adapter.ExecuteReader(AddressOf RetrieveAllMessagesReader, cmd)
            Return Me
        End Function

        ''' <summary>
        ''' Returns a list of UserGroups assigned and unassigned.
        ''' </summary>
        ''' <param name="messageid"></param>
        ''' <param name="compo"></param>
        ''' <returns>A data table containing the results.</returns>
        ''' <remarks></remarks>
        Public Function RetrieveMessageGroups(ByVal messageId As Int16, ByVal compo As String) As MessageList
            Dim adapter As New SqlDataStore()
            Dim user As AppUser = ALOD.Data.Services.UserService.CurrentUser()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_messages_sp_GetMessagesGroups", messageId, compo, user.CurrentRole.Id)
            adapter.ExecuteReader(AddressOf RetrieveMessageGroupsReader, cmd)
            Return Me
        End Function

        ''' <summary>
        ''' Returns a list of new messages.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <param name="groupId"></param>
        ''' <param name="popup"></param>
        ''' <returns>A data table containing the results.</returns>
        ''' <remarks></remarks>
        Public Function RetrieveMessages(ByVal userId As Integer, ByVal groupId As Integer, ByVal popup As Boolean) As MessageList

            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_messages_sp_GetMessages", userId, groupId, popup)
            adapter.ExecuteReader(AddressOf RetrieveMessagesReader, cmd)

            Return Me
        End Function

        Public Sub SetMessagesRead(ByVal userId As Integer, ByVal groupId As Byte)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_messages_sp_UpdateMessagesRead", userId, groupId)
        End Sub

        ''' <summary>
        ''' Updates the Message Details.
        ''' </summary>
        ''' <param name="messageId">ID of the Message</param>
        ''' <param name="title">The Message's Title</param>
        ''' <param name="name">Name to show the Message is From</param>
        ''' <param name="startdate">StartDate of the Message</param>
        ''' <param name="enddate">EndDate of the Message</param>
        ''' <param name="popup">If message is a popup</param>
        ''' <param name="message">Message</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateMessage(ByVal messageId As Integer, ByVal title As String, ByVal name As String, ByVal startdate As Date, ByVal enddate As Date, ByVal popup As Boolean, ByVal message As String) As Boolean
            Dim adapter As New SqlDataStore()
            Return CInt(adapter.ExecuteNonQuery("core_messages_sp_UpdateMessages", messageId, title, name, startdate, enddate, popup, message)) > 0
        End Function

        ''' <summary>
        ''' Remove all associates groups with a message and repopulate them using an XML string.
        ''' </summary>
        ''' <param name="messageId">ID of the Message</param>
        ''' <param name="xmlGroups">XML String of the Groups now associated with the Message</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateMessageGroups(ByVal messageId As Integer, ByVal xmlGroups As String) As Boolean
            Dim adapter As New SqlDataStore()
            Return adapter.ExecuteNonQuery("core_messages_sp_UpdateMessageGroups", messageId, xmlGroups) > 0
        End Function

        Protected Sub RetrieveAllMessagesReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '1-MessageId, 1-Title, 2-StartTime, 3-EndTime, 4-Popup
            Dim message As New Message
            message.MessageId = adapter.GetInt16(reader, 0)
            message.Title = adapter.GetString(reader, 1)
            message.StartTime = adapter.GetDateTime(reader, 2, Now).ToShortDateString
            message.EndTime = adapter.GetDateTime(reader, 3, Now).ToShortDateString
            message.Popup = adapter.GetBoolean(reader, 4)

            _message.Add(message)

        End Sub

        Protected Sub RetrieveMessageGroupsReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-Title, 1-Message, 2-Name, 3-Popup, 4-StartTime,5-EndTime
            Dim message As New Message

            message.GroupId = adapter.GetByte(reader, 0)
            message.GroupName = adapter.GetString(reader, 1)
            message.Assigned = adapter.GetInteger(reader, 2)
            _message.Add(message)

        End Sub

        Protected Sub RetrieveMessagesReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-Message, 1-Name, 2-StartTime, 3-Title
            Dim message As New Message
            message.Message = adapter.GetString(reader, 0)
            message.Name = adapter.GetString(reader, 1)
            message.StartTime = adapter.GetDateTime(reader, 2, Now).ToShortDateString
            message.Title = adapter.GetString(reader, 3)
            message.IsAdmin = adapter.GetBoolean(reader, 4)

            _message.Add(message)

        End Sub

#End Region

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of Message).Count
            Get
                Return _message.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of Message).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Message Implements System.Collections.Generic.IList(Of Message).Item
            Get
                Return _message(index)
            End Get
            Set(ByVal value As Message)
                _message(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As Message) Implements System.Collections.Generic.ICollection(Of Message).Add
            _message.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of Message).Clear
            _message.Clear()
        End Sub

        Public Function Contains(ByVal item As Message) As Boolean Implements System.Collections.Generic.ICollection(Of Message).Contains
            Return _message.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As Message, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of Message).CopyTo
            _message.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Message) Implements System.Collections.Generic.IEnumerable(Of Message).GetEnumerator
            Return _message.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _message.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As Message) As Integer Implements System.Collections.Generic.IList(Of Message).IndexOf
            Return _message.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As Message) Implements System.Collections.Generic.IList(Of Message).Insert
            _message.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As Message) As Boolean Implements System.Collections.Generic.ICollection(Of Message).Remove
            Return _message.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of Message).RemoveAt
            _message.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace