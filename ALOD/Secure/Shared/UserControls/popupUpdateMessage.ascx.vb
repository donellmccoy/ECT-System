Imports ALOD.Core.Domain.Messaging
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_popupUpdateMessage
        Inherits System.Web.UI.UserControl

        Delegate Sub CancelledEvent()

        Delegate Sub SavedEvent()

        Public Event MessageCancelled As CancelledEvent

        Public Event MessageSaved As SavedEvent

        Sub EnableDisable(ByVal value As Boolean)
            lstAvailGroups.Enabled = value
            lstAssignGroups.Enabled = value
            btnAssign.Enabled = value
            btnUnAssign.Enabled = value
        End Sub

        Sub FillListBoxes(ByVal MessageID As Integer)
            Dim coll As New Collection
            Dim list As New MessageList
            list = list.RetrieveMessageGroups(MessageID, CStr(HttpContext.Current.Session("Compo")))
            For Each message As ALODWebUtility.Common.Message In list
                If message.GroupId = 0 Then
                    chkAll.Checked = True
                    EnableDisable(False)
                    Exit For
                End If
                If message.Assigned Then
                    lstAssignGroups.Items.Add(message.GroupId.ToString.PadRight(3) + " " + message.GroupName.Trim)
                Else
                    lstAvailGroups.Items.Add(message.GroupId.ToString.PadRight(3) + " " + message.GroupName.Trim)
                End If
            Next
            ViewState.Add("groups", coll)
            If txtStart.Text = String.Empty Then
                txtStart.Text = Server.HtmlEncode(Now.ToString(DATE_FORMAT))
            End If
            If (txtFrom.Text = String.Empty) Then
                txtFrom.Text = Server.HtmlEncode(CStr(HttpContext.Current.Session("UserName")))
            End If
        End Sub

        '<summary>
        '<mod tfsid=5129 date '02/22/2011'>
        '<change> end date change was not saving </change>
        '</mod>
        '</summary>
        Sub Save()

            Const SECTION_MESSAGE As String = "Message"
            Const FIELD_TITLE As String = "Title"
            Const FIELD_NAME As String = "Name"
            Const FIELD_MESSAGE As String = "Message"
            Const FIELD_START_DATE As String = "Start Date"
            Const FIELD_END_DATE As String = "End Date"
            Const FIELD_POPUP As String = "Popup"

            Dim list As New MessageList
            Dim MessageID As Integer = ViewState("MessageID")

            Dim title As String = txtTitle.Text.Trim
            Dim name As String = txtFrom.Text.Trim
            Dim body As String = txtMessage.Text.Trim
            Dim isPopup As Boolean = chkPopup.Checked
            Dim startDate As Date
            Dim endDate As Date

            Try
                startDate = Date.Parse(txtStart.Text.Trim)
                endDate = Date.Parse(txtEnd.Text.Trim)
                If DateTime.Compare(startDate, endDate) > 0 Then
                    Exit Sub

                End If
            Catch ex As Exception
                Exit Sub
            End Try

            Dim message As ALOD.Core.Domain.Messaging.Message
            Dim dao As IMessageDao = New NHibernateDaoFactory().GetMessageDao()

            If MessageID = 0 Then

                ' MessageID = list.InsertMessage(title, name, DateTime.Parse(startDate), DateTime.Parse(endDate), chkPopup.Checked, body)

                message = New ALOD.Core.Domain.Messaging.Message
                message.CreatedBy = UserService.GetById(SESSION_USER_ID)
                message.Title = title
                message.Body = body
                message.Name = name
                message.IsPopup = isPopup
                message.StartDate = startDate
                message.EndDate = endDate
                message.IsAdminMessage = UserHasPermission(PERMISSION_SYSTEM_ADMIN)

                dao.SaveOrUpdate(message)

                If (message.Id > 0) Then

                    Dim changes As New ChangeSet()
                    changes.Add(SECTION_MESSAGE, FIELD_TITLE, String.Empty, title)
                    changes.Add(SECTION_MESSAGE, FIELD_NAME, String.Empty, name)
                    changes.Add(SECTION_MESSAGE, FIELD_MESSAGE, String.Empty, body)
                    changes.Add(SECTION_MESSAGE, FIELD_START_DATE, String.Empty, startDate)
                    changes.Add(SECTION_MESSAGE, FIELD_POPUP, String.Empty, isPopup.ToString())

                    If (Not String.IsNullOrEmpty(endDate)) Then
                        changes.Add(SECTION_MESSAGE, FIELD_END_DATE, String.Empty, endDate)
                    End If

                    dao.CommitChanges()
                    dao.Evict(message)

                    Dim logId As Integer = LogManager.LogAction(ModuleType.System,
                                            UserAction.MessageAdd, message.Id, "Added Message")

                    If (logId > 0) Then
                        changes.Save(logId)
                    End If

                End If
            Else

                Try
                    message = dao.GetById(MessageID)
                Catch ex As Exception
                    Exit Sub
                End Try

                If (message IsNot Nothing) Then

                    Dim changes As New ChangeSet()

                    If (title <> message.Title) Then
                        changes.Add(SECTION_MESSAGE, FIELD_TITLE, message.Title, title)
                        message.Title = title
                    End If

                    If (body <> message.Body) Then
                        changes.Add(SECTION_MESSAGE, FIELD_MESSAGE, message.Body, body)
                        message.Body = body
                    End If

                    If (name <> message.Name) Then
                        changes.Add(SECTION_MESSAGE, FIELD_NAME, message.Name, name)
                        message.Name = name
                    End If

                    If (startDate <> message.StartDate) Then

                        changes.Add(SECTION_MESSAGE, FIELD_START_DATE,
                                    message.StartDate.ToString(DATE_FORMAT),
                                    startDate.ToString(DATE_FORMAT))

                        message.StartDate = startDate
                    End If

                    If (message.EndDate.HasValue) Then
                        If (endDate <> message.EndDate.Value) Then

                            changes.Add(SECTION_MESSAGE, FIELD_END_DATE,
                                        message.EndDate.Value.ToString(DATE_FORMAT),
                                        endDate.ToString(DATE_FORMAT))
                            message.EndDate = endDate
                        End If
                    Else
                        changes.Add(SECTION_MESSAGE, FIELD_END_DATE, String.Empty, endDate.ToString(DATE_FORMAT))
                    End If

                    If (isPopup <> message.IsPopup) Then
                        changes.Add(SECTION_MESSAGE, FIELD_POPUP, message.IsPopup.ToString(), isPopup.ToString())
                        message.IsPopup = isPopup
                    End If

                    If (changes.Count > 0) Then

                        dao.SaveOrUpdate(message)
                        dao.CommitChanges()
                        dao.Evict(message)

                        Dim logId As Integer = LogManager.LogAction(ModuleType.System,
                                                UserAction.MessageModify, MessageID, "Modified Message")

                        If (logId > 0) Then
                            changes.Save(logId)
                        End If

                    End If

                End If
            End If

            Dim xml As New XMLString("XML_Array")
            If chkAll.Checked Then
                For Each item As ListItem In lstAvailGroups.Items
                    xml.BeginElement("XMLList")
                    xml.WriteAttribute("ID", Mid(item.Text, 1, 3).Trim)
                    xml.EndElement()
                Next
                For Each item As ListItem In lstAssignGroups.Items
                    xml.BeginElement("XMLList")
                    xml.WriteAttribute("ID", Mid(item.Text, 1, 3).Trim)
                    xml.EndElement()
                Next
            Else
                For Each item As ListItem In lstAssignGroups.Items
                    xml.BeginElement("XMLList")
                    xml.WriteAttribute("ID", Mid(item.Text, 1, 3).Trim)
                    xml.EndElement()
                Next
            End If

            list.UpdateMessageGroups(message.Id, xml.ToString)
            RaiseEvent MessageSaved()
        End Sub

        Public Sub Show(ByVal MessageID As Integer)
            Clear()
            ViewState.Add("MessageID", MessageID)
            PopulateControls(MessageID)
            FillListBoxes(MessageID)
            title.Text = "Update Message"
        End Sub

        Public Sub Show()
            '        'We're adding a new record
            ViewState("MessageID") = 0
            Clear()
            FillListBoxes(0)
            title.Text = "Create New Message"
            btnSave.Text = "Create"

        End Sub

        Sub sort(ByRef box As ListBox)
            'sorts listbox descending
            Dim array1 As New ArrayList()
            Dim loop1 As Integer
            For loop1 = 0 To box.Items.Count - 1
                array1.Add(box.Items(loop1).Text)
            Next
            array1.Sort(New sortStringtoIntAsc())
            box.Items.Clear()
            For loop1 = 0 To array1.Count - 1
                box.Items.Add(array1(loop1))
            Next
        End Sub

        Protected Sub btnAssign_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAssign.Click
            Dim DeleteMe As New Collection
            For Each obj As ListItem In lstAvailGroups.Items
                If obj.Selected Then
                    lstAssignGroups.Items.Add(obj)
                    DeleteMe.Add(obj)
                End If
            Next
            For Each obj As ListItem In DeleteMe
                lstAvailGroups.Items.Remove(obj)
            Next
            sort(lstAssignGroups)
        End Sub

        Protected Sub btnUnAssign_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUnAssign.Click
            Dim DeleteMe As New Collection
            For Each obj As ListItem In lstAssignGroups.Items
                If obj.Selected Then
                    lstAvailGroups.Items.Add(obj)
                    DeleteMe.Add(obj)
                End If
            Next
            For Each obj As ListItem In DeleteMe
                lstAssignGroups.Items.Remove(obj)
            Next
            sort(lstAvailGroups)
        End Sub

        Protected Sub chkAll_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAll.CheckedChanged
            EnableDisable(Not sender.checked)
        End Sub

        Protected Sub Clear()
            ViewState.Remove("groups")
            lstAssignGroups.Items.Clear()
            lstAvailGroups.Items.Clear()
            txtEnd.Text = ""
            txtStart.Text = ""
            txtTitle.Text = ""
            txtMessage.Text = ""
            txtFrom.Text = ""
            chkAll.Checked = False
            chkPopup.Checked = False
            EnableDisable(True)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            txtStart.Attributes.Add("readonly", "true")
            txtEnd.Attributes.Add("readonly", "true")

        End Sub

        Protected Sub PopulateControls(ByVal MessageID As Integer)

            Dim message As ALOD.Core.Domain.Messaging.Message
            Dim dao As IMessageDao = New NHibernateDaoFactory().GetMessageDao()

            Try
                message = dao.GetById(MessageID)
            Catch ex As Exception
                Exit Sub
            End Try

            If message IsNot Nothing Then
                txtEnd.Text = message.EndDate.Value.ToString(DATE_FORMAT)
                txtStart.Text = message.StartDate.ToString(DATE_FORMAT)
                txtTitle.Text = message.Title.Trim
                txtMessage.Text = message.Body.Trim
                txtFrom.Text = message.Name.Trim
                chkPopup.Checked = message.IsPopup
            End If

        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Clear()
            RaiseEvent MessageCancelled()
        End Sub

        Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Save()
        End Sub

        Private Class sortStringtoIntAsc : Implements IComparer

            Function Compare(ByVal a As Object, ByVal b As Object) As Integer Implements IComparer.Compare
                Dim c1 As Integer = Mid(a, 1, 2)
                Dim c2 As Integer = Mid(b, 1, 2)

                If (c1 > c2) Then
                    Return 1
                End If

                If (c1 < c2) Then
                    Return -1
                Else
                    Return 0
                End If
            End Function

        End Class

    End Class

End Namespace