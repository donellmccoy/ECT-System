Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Data.Common
Imports System.Runtime.Remoting.Messaging
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Providers

Namespace Common
    Public Module SessionInfo

        Public Const CRTL_ASYNCH As String = "asynchCrtl"

        Public Const PERMISSION_CREATE_SARC As String = "RSARCCreate"

        Public Const PERMISSION_CREATE_SARC_UNRESTRICTED As String = "SARCUnrestrictedCreate"

        Public Const PERMISSION_DELETE_COMMENTS As String = "commentDelete"

        Public Const PERMISSION_EDIT_EDIPIN_NUMBERS As String = "EditEDIPIN"

        Public Const PERMISSION_EDIT_SARC As String = "myRSARC"

        Public Const PERMISSION_EXECUTE_LOD_POST_COMPLETION As String = "exePostCompletion"

        Public Const PERMISSION_MANAGE_UNITS As String = "unitsEdit"

        Public Const PERMISSION_SARC_ADHOC_REPORTS As String = "SARCAdHocReports"

        Public Const PERMISSION_SARC_LODSARCCASES_REPORT_ALL As String = "LODSARCCasesReport - ALL"

        Public Const PERMISSION_SARC_LODSARCCASES_REPORT_UNRESTRICTED As String = "LODSARCCasesReport - UNRESTRICTED"

        Public Const PERMISSION_SARC_POSTPROCESSING As String = "RSARCPostCompletion"

        Public Const PERMISSION_SARC_VIEW_REDACTED_DOCUMENTS_ONLY As String = "ViewRedactedSARCDocumentsOnly"

        Public Const PERMISSION_SYSTEM_ADMIN As String = "sysAdmin"

        Public Const PERMISSION_VIEW_FORMAL_LOD As String = "ViewFormalLOD"

        Public Const PERMISSION_VIEW_SARC_CASES As String = "RSARCView"

        Public Const SESSIONKEY_ACCESS_SCOPE As String = "AccessScope"

        Public Const SESSIONKEY_COMPO As String = "Compo"

        Public Const SESSIONKEY_DISPLAY_NAME As String = "DisplayName"

        Public Const SESSIONKEY_EDIPIN As String = "EDIPIN"

        Public Const SESSIONKEY_EDIT_MEMBER_SSAN As String = "EDIT_MEMBER_SSN"

        Public Const SESSIONKEY_ERROR_MESSAGE As String = "ERROR_MESSAGE"

        Public Const SESSIONKEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"

        Public Const SESSIONKEY_GROUP_ID As String = "GroupId"

        Public Const SESSIONKEY_GROUP_NAME As String = "Group"

        Public Const SESSIONKEY_LAST_LOGIN As String = "LastLogin"

        Public Const SESSIONKEY_LOCK_AQUIRED As String = "LockAquired"

        Public Const SESSIONKEY_LOCK_ID As String = "LockId"

        Public Const SESSIONKEY_REDIRECT_URL As String = "RedirectUrl"

        Public Const SESSIONKEY_REPORT_VIEW As String = "ReportView"

        Public Const SESSIONKEY_ROLE_ID As String = "RoleId"

        Public Const SESSIONKEY_SARC_PERMSSION As String = "hasSarcPermission"

        Public Const SESSIONKEY_SESSION_DICTIONARY As String = "sessionDictionary"

        Public Const SESSIONKEY_SSN As String = "SSN"

        Public Const SESSIONKEY_UNIT_ID As String = "UnitId"

        Public Const SESSIONKEY_USER_ID As String = "UserId"

        Public Const SESSIONKEY_USERNAME As String = "UserName"

        Public Const SESSIONKEY_WS_ID As String = "wsId"

        Private SessionExcluded() As String = {SESSIONKEY_GROUP_ID, SESSIONKEY_USERNAME, SESSIONKEY_SSN, SESSIONKEY_EDIPIN, SESSIONKEY_COMPO, SESSIONKEY_ROLE_ID,
                                               SESSIONKEY_USER_ID, SESSIONKEY_GROUP_NAME, SESSIONKEY_REPORT_VIEW, SESSIONKEY_UNIT_ID,
                                               SESSIONKEY_ACCESS_SCOPE, SESSIONKEY_LAST_LOGIN, SESSIONKEY_DISPLAY_NAME}

        Public Delegate Function UnitLookUpDelegate(ByVal param As StringDictionary) As List(Of ALOD.Core.Domain.Users.LookUpItem)

        Public Property SESSION_ACCESS_SCOPE() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_ACCESS_SCOPE))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value)
            End Set
        End Property

        Public Property SESSION_COMPO() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_COMPO))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_COMPO, value)
            End Set
        End Property

        Public Property SESSION_DISPLAY_NAME() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_DISPLAY_NAME))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_DISPLAY_NAME, value)
            End Set
        End Property

        Public Property SESSION_EDIPIN() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_EDIPIN))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_EDIPIN, value)
            End Set
        End Property

        Public Property SESSION_GROUP_ID() As Byte
            Get
                Return CInt(GetSessionValue(SESSIONKEY_GROUP_ID))
            End Get
            Set(ByVal value As Byte)
                SetSessionValue(SESSIONKEY_GROUP_ID, value)
            End Set
        End Property

        Public Property SESSION_GROUP_NAME() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_GROUP_NAME))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_GROUP_NAME, value)
            End Set
        End Property

        Public Property SESSION_LAST_LOGIN() As DateTime?
            Get
                Return CDate(GetSessionValue(SESSIONKEY_LAST_LOGIN))
            End Get
            Set(ByVal value As DateTime?)
                SetSessionValue(SESSIONKEY_LAST_LOGIN, value)
            End Set
        End Property

        Public Property SESSION_LOCK_AQUIRED() As Boolean
            Get
                Return CInt(GetSessionValue(SESSIONKEY_LOCK_AQUIRED))
            End Get
            Set(ByVal value As Boolean)
                SetSessionValue(SESSIONKEY_LOCK_AQUIRED, value)
            End Set
        End Property

        Public Property SESSION_LOCK_ID() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_LOCK_ID))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_LOCK_ID, value)
            End Set
        End Property

        Public Property SESSION_REDIRECT_URL() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_REDIRECT_URL))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_REDIRECT_URL, value)
            End Set
        End Property

        Public Property SESSION_REPORT_VIEW() As Byte
            Get
                Return CInt(GetSessionValue(SESSIONKEY_REPORT_VIEW))
            End Get
            Set(ByVal value As Byte)
                SetSessionValue(SESSIONKEY_REPORT_VIEW, value)
            End Set
        End Property

        Public Property SESSION_ROLE_ID() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_ROLE_ID))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_ROLE_ID, value)
            End Set
        End Property

        Public Property SESSION_SSN() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_SSN))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_SSN, value)
            End Set
        End Property

        Public Property SESSION_UNIT_ID() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_UNIT_ID))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_UNIT_ID, value)
            End Set
        End Property

        Public Property SESSION_USER_ID() As Integer
            Get
                Return CInt(GetSessionValue(SESSIONKEY_USER_ID))
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_USER_ID, value)
            End Set
        End Property

        Public Property SESSION_USERNAME() As String
            Get
                Return CStr(GetSessionValue(SESSIONKEY_USERNAME))
            End Get
            Set(ByVal value As String)
                SetSessionValue(SESSIONKEY_USERNAME, value)
            End Set
        End Property

        Public Property SESSION_WS_ID() As Integer 'Pilot
            Get
                Return GetWS_ID()
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value)
            End Set
        End Property

        Public Property SESSION_WS_ID(refId As Integer) As Integer 'By refId
            Get
                Return GetWS_ID(refId)
            End Get
            Set(ByVal value As Integer)
                SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value)
            End Set
        End Property

        '''<summary>
        '''<para>The method creates  UnitLookUpDelegate for  UnitLookUp method and invokes the methos asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncUnitLookup(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult

            Dim asynchDelegate As UnitLookUpDelegate = New UnitLookUpDelegate(AddressOf UnitLookUp)
            Dim param As StringDictionary = CType(state(SESSIONKEY_SESSION_DICTIONARY), StringDictionary)
            Dim r As IAsyncResult = asynchDelegate.BeginInvoke(param, cb, state)
            Return r

        End Function

        Public Function CanUserDeleteUnOwnedDocument() As Boolean
            If (UserHasPermission("DeleteUnOwnedDocuments")) Then
                Return True
            End If

            Return False
        End Function

        Public Sub CleanSession()

            Dim skip As New ArrayList(SessionExcluded)
            Dim remove As New ArrayList

            For Each key As String In HttpContext.Current.Session.Keys
                If (Not skip.Contains(key)) Then
                    remove.Add(key)
                End If
            Next

            For Each key As String In remove
                HttpContext.Current.Session.Remove(key)
            Next

        End Sub

        '''<summary>
        '''<para>The method is the call back method which is called when the  unit look up is complete.
        ''' Upon completion the controls inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncUnitLookup(ByVal a As IAsyncResult)

            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim stateData = CType(a.AsyncState, Dictionary(Of String, Object))
            Dim param As StringDictionary = CType(stateData(SESSIONKEY_SESSION_DICTIONARY), StringDictionary)
            Dim caller As UnitLookUpDelegate = CType(result.AsyncDelegate, UnitLookUpDelegate)
            Dim UnitSelect As DropDownList = CType(stateData(CRTL_ASYNCH), DropDownList)

            Dim lst As List(Of ALOD.Core.Domain.Users.LookUpItem) = caller.EndInvoke(a)
            UnitSelect.DataSource = lst
            UnitSelect.DataBind()
            UnitSelect.Items.Insert(0, New ListItem("-- All --", String.Empty))

        End Sub

        Public Function GetCallerAddress() As String
            If (HttpContext.Current.Request Is Nothing) Then
                Return String.Empty
            End If
            Return HttpContext.Current.Request.UserHostAddress
        End Function

        Public Function GetCurrentSessionId() As String
            If (HttpContext.Current.Session Is Nothing) Then
                Return String.Empty
            End If

            Return HttpContext.Current.Session.SessionID
        End Function

        Public Function GetErrorMessage() As String
            Dim message As String = GetSessionValue(SESSIONKEY_ERROR_MESSAGE)

            If (message IsNot Nothing) Then
                Return CStr(message)
            End If

            Return String.Empty

        End Function

        Public Function GetFeedbackMessage() As String
            Dim message As String = GetSessionValue(SESSIONKEY_FEEDBACK_MESSAGE)

            If (message IsNot Nothing) Then
                Return CStr(message)
            End If

            Return String.Empty

        End Function

        ''' <summary>
        '''Creates a string dictionary object of required session variables.Used by GetStateObject method
        ''' </summary>
        '''<retuns>Return a dictionary of string containing required session variables</retuns>
        Public Function GetSessionDictionary() As StringDictionary

            Dim param As StringDictionary = New StringDictionary()
            param.Add(SESSIONKEY_UNIT_ID, SESSION_UNIT_ID.ToString)
            param.Add(SESSIONKEY_REPORT_VIEW, SESSION_REPORT_VIEW.ToString)
            param.Add(SESSIONKEY_COMPO, SESSION_COMPO)
            param.Add(SESSIONKEY_USER_ID, SESSION_USER_ID.ToString)
            param.Add(SESSIONKEY_SARC_PERMSSION, UserHasPermission(PERMISSION_VIEW_SARC_CASES))

            Return param

        End Function

        Public Function GetSessionValue(ByVal key As String) As Object

            If (HttpContext.Current Is Nothing) OrElse (HttpContext.Current.Session Is Nothing) Then
                Return Nothing
            End If

            Return HttpContext.Current.Session(key)

        End Function

        ''' <summary>
        ''' Creates a dictionary object which can be passed to the BeginAsyncUnitLookup method
        ''' </summary>
        ''' <param name="crtl">The control whose html needs to be updates with the return content</param>
        ''' <retuns>Return a dictionary of string and object</retuns>
        Public Function GetStateObject(ByRef crtl As Control) As Dictionary(Of String, Object)

            Dim param As StringDictionary = GetSessionDictionary()
            Dim stateData As New Dictionary(Of String, Object)
            stateData.Add(SESSIONKEY_SESSION_DICTIONARY, param)
            stateData.Add(CRTL_ASYNCH, crtl)
            Return stateData

        End Function

        Public Function GetWS_ID(refId As Integer)
            Dim dataStore As New SqlDataStore
            Dim dbCommand As DbCommand
            Dim ds As New DataSet
            Dim wsId As Integer
            dbCommand = dataStore.GetStoredProcCommand("core_lod_sp_GetWSID", CObj(refId))
            wsId = dataStore.ExecuteDataSet(dbCommand).Tables(0).Rows(0).ItemArray(0)
            Return wsId
        End Function

        'CodeCleanUp
        Public Function GetWS_ID()
            Select Case SESSION_GROUP_ID
                Case 120
                    Return 330
                Case 2
                    Return 220
                Case Else
                    Return 0
            End Select
        End Function

        Public Sub RemoveSessionValue(ByVal key As String)
            If (HttpContext.Current Is Nothing) OrElse (HttpContext.Current.Session Is Nothing) Then
                Exit Sub
            End If

            HttpContext.Current.Session.Remove(key)
        End Sub

        ''' <summary>
        ''' Redirects a request to a new URL and specifies the new URL. Performs a Response.Redirect(url, False) followed by a CompleteRequest() call to properly terminate code execution.
        ''' </summary>
        ''' <param name="url">The target location.</param>
        ''' <remarks></remarks>
        Public Sub SafeResponseRedirect(ByVal url As String)
            If (String.IsNullOrEmpty(url)) Then
                Exit Sub
            End If

            HttpContext.Current.Response.Redirect(url, False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Sub

        Public Sub SetErrorMessage(ByVal message As String)
            If (message.Length > 0) Then
                SetSessionValue(SESSIONKEY_ERROR_MESSAGE, message)
            Else
                RemoveSessionValue(SESSIONKEY_ERROR_MESSAGE)
            End If
        End Sub

        Public Sub SetFeedbackMessage(ByVal message As String)
            If (message.Length > 0) Then
                SetSessionValue(SESSIONKEY_FEEDBACK_MESSAGE, message)
            Else
                RemoveSessionValue(SESSIONKEY_FEEDBACK_MESSAGE)
            End If
        End Sub

        ''' <summary>
        ''' Fully logs in the user and creates session information. If the user is already logged in in a different session then the function safely redirects to the
        ''' AltSession.aspx page and returns false; otherwise the functions return true.
        ''' </summary>
        ''' <param name="user"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetLogin(ByVal user As ALOD.Core.Domain.Users.AppUser)

            If (Not UpdateUserOnline(user.Id)) Then
                Return False
            End If

            Dim role As UserRole = user.CurrentRole

            HttpContext.Current.Session("UserId") = user.Id
            HttpContext.Current.Session("UserName") = user.Username
            HttpContext.Current.Session("SSN") = user.SSN
            HttpContext.Current.Session("EDIPIN") = user.EDIPIN
            HttpContext.Current.Session("Compo") = user.Component
            HttpContext.Current.Session("RoleId") = role.Id
            HttpContext.Current.Session("GroupId") = role.Group.Id
            HttpContext.Current.Session("Group") = role.Group.Description
            HttpContext.Current.Session("UnitId") = user.CurrentUnitId
            HttpContext.Current.Session("AccessScope") = role.Group.Scope
            HttpContext.Current.Session("LastLogin") = user.LastLoginDate
            HttpContext.Current.Session("DisplayName") = user.Rank.Rank + " " + user.LastName + ", " + user.FirstName

            'if this user has a reporting view set, it overrides the group setting, so use it
            If (user.ReportView.HasValue) Then
                HttpContext.Current.Session("ReportView") = user.ReportView.Value
            Else
                HttpContext.Current.Session("ReportView") = role.Group.ReportView
            End If

            FormsAuthentication.SetAuthCookie(user.Id, False)

            Dim perms As New UserAuthentication(user.Id)
            Dim principle As New LodPrinciple(perms, perms.Permissions)
            HttpContext.Current.User = principle
            Threading.Thread.CurrentPrincipal = principle
            AuthProvider.SetAuthCookie(perms)

            LogManager.LogAction(ModuleType.System, UserAction.UserLogin, "Role: " + role.Group.Description)
            Return True
        End Function

        Public Sub SetSessionValue(ByVal key As String, ByVal value As Object)

            If (HttpContext.Current Is Nothing) OrElse (HttpContext.Current.Session Is Nothing) Then
                Exit Sub
            End If

            HttpContext.Current.Session(key) = value

        End Sub

        '''<summary>
        '''<para>The method calls LookupService's GetChildUnits method.
        ''' It passes current session user's unitid and report view.
        ''' </para>
        '''<param name="param">A string dictionary contining session varaibles</param>
        '''</summary>
        ''' <returns>List of LookUpItems containg units</returns>
        Public Function UnitLookUp(ByVal param As StringDictionary) As List(Of ALOD.Core.Domain.Users.LookUpItem)
            Dim lst As List(Of ALOD.Core.Domain.Users.LookUpItem) = LookupService.GetChildUnits(Integer.Parse(param(SESSIONKEY_UNIT_ID)), Byte.Parse(param(SESSIONKEY_REPORT_VIEW)))
            Return lst
        End Function

        ''' <summary>
        ''' Updates the specified users login information. If the user is already logged in in a different session then the function safely redirects to the
        ''' AltSession.aspx page and returns false; otherwise the functions return true.
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateUserOnline(ByVal userId As Integer)
            Dim adapter As New SqlDataStore
            Dim isValid As Boolean = CBool(adapter.ExecuteScalar("core_user_sp_UpdateLogin", userId, GetCurrentSessionId(), GetCallerAddress()))

            If (Not isValid) Then
                'the user is attempting to login from a different session
                'don't allow this
                If (ConfigurationManager.AppSettings("SingleSession") = "Y") Then
                    'if singe-session is not enabled, let them login (testing)
                    SafeResponseRedirect("~/Public/AltSession.aspx")

                    Return False
                End If

            End If

            Return True

        End Function

        Public Function UserHasPermission(ByVal permission As String) As Boolean
            Return System.Threading.Thread.CurrentPrincipal.IsInRole(permission)
        End Function

    End Module
End Namespace