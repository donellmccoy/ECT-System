Imports ALOD.Data

Namespace Permission

    Public Class OnlineUserList

        Private _users As DataSets.OnlineUsersDataTable

        Public Function GetUsers() As DataSets.OnlineUsersDataTable

            _users = New DataSets.OnlineUsersDataTable()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf UserReader, "core_user_sp_GetUsersOnline")
            Return _users

        End Function

        Private Sub UserReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-userid, 1-userName, 2-roleName, 3-regionName, 4-loginTime
            Dim row As DataSets.OnlineUsersRow = _users.NewOnlineUsersRow()

            row.userId = adapter.GetInteger(reader, 0)
            row.userName = adapter.GetString(reader, 1)
            row.roleName = adapter.GetString(reader, 2)
            row.regionName = adapter.GetString(reader, 3)
            row.loginTime = adapter.GetDateTime(reader, 4)
            row.timeOnline = Now.Subtract(row.loginTime)

            _users.Rows.Add(row)

        End Sub

    End Class

End Namespace