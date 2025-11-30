<%@ Application Language="VB" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="ALOD.Web" %>
<%@ Import Namespace="ALOD.Core" %>
<%@ Import Namespace="ALOD.Core.Interfaces" %>
<%@ Import Namespace="ALOD.Core.Interfaces.DAOInterfaces" %>
<%@ Import Namespace="ALOD.Core.Utils" %>
<%@ Import Namespace="ALOD.Data" %>
<%@ Import Namespace="ALOD.Data.Services" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.CORE.Utils" %>
<%@ Import Namespace="WebSupergoo.ABCpdf12" %>


<script runat="server">


    Private Const EmailReminderCache As String = "EmailReminderCache"
    Private Const AppWarmupCacheKey As String = "DailyAutomaticProcessesCache"

    'Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Code that runs on application startup
    '    'start the NHibernate session manager
    '    NHibernateSessionManager.Instance.GetSession()


    '    'Register Cache Entry That Will Send Reminder Emails When the Cache is removed
    '    'If ConfigurationManager.AppSettings("ReminderEmailIsEnabled") = "Y" Then
    '    '    RegisterCache()
    '    'End If
    '    RegisterCache()
    'End Sub


    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
        
        ' Register script and style bundles for optimization
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        
        If HttpContext.Current IsNot Nothing Then
            NHibernateSessionManager.Instance.GetSession()

            'Register Cache Entry That Will Send Reminder Emails When the Cache is removed
            'If ConfigurationManager.AppSettings("ReminderEmailIsEnabled") = "Y" Then
            '    RegisterCache()
            'End If
            RegisterCache()

            ' Initialize ABCpdf
            'If Not XSettings.InstallLicense("") Then
            '    Response.Write("Could not install trial license. ")
            '    Response.Write("You have: " + XSettings.LicenseDescription)
            'End If

        End If
    End Sub


    Private Function RegisterCache() As Boolean
        'If (Not HttpContext.Current.Cache(EmailReminderCache) = Nothing) Then
        '    Return False
        'End If

        If (Not HttpContext.Current.Cache(AppWarmupCacheKey) = Nothing) Then
            Return False
        End If

        Dim sendTime As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 1, 0).AddDays(1)
        Dim sendTimeFromNow As TimeSpan = TimeSpan.FromTicks(sendTime.Ticks - DateTime.Now.Ticks)

        'HttpContext.Current.Cache.Add(EmailReminderCache, "Test", Nothing, sendTime, System.Web.Caching.Cache.NoSlidingExpiration,
        '                         CacheItemPriority.NotRemovable, New CacheItemRemovedCallback(AddressOf ReminderCacheItemRemovedCallback))

        HttpContext.Current.Cache.Add(AppWarmupCacheKey, "ECTWarmupCache", Nothing, sendTime, System.Web.Caching.Cache.NoSlidingExpiration,
                                 CacheItemPriority.NotRemovable, New CacheItemRemovedCallback(AddressOf DailyAutomaticProcessesCacheItemRemovedCallback))

        Return True
    End Function

    'Sub ReminderCacheItemRemovedCallback(key As String, value As Object, reason As CacheItemRemovedReason)
    Sub DailyAutomaticProcessesCacheItemRemovedCallback(key As String, value As Object, reason As CacheItemRemovedReason)

        If reason = CacheItemRemovedReason.Expired Then
            'Hit a Dummy Page to reinsert the Cache
            HitPage()

            Dim appWarmupDao As IApplicationWarmupProcessDao = New NHibernateDaoFactory().GetApplicationWarmupProcessDao()
            Dim hostname As String = ConfigurationManager.AppSettings("Hostname")
            Dim reminder As New ReminderEmailsService()

            'Call to send email reminders
            If (ConfigurationManager.AppSettings("ReminderEmailIsEnabled") = "Y" AndAlso appWarmupDao.IsProcessActive("ReminderEmails") = True) Then
                reminder.SendEmailReminders(hostname)
            End If

            If (appWarmupDao.IsProcessActive("InactiveAccounts") = True) Then
                reminder.DisableInactiveAccounts()
            End If

            PsychologicalHealthService.ExecuteApplicationWarmupProcesses(DateTime.Now, hostname)
            ReportsService.ExecuteApplicationWarmupProcesses(DateTime.Now, hostname)
        End If

    End Sub

    Sub HitPage()
        Dim client As New WebClient()
        client.DownloadData(ConfigurationManager.AppSettings("KeepAliveURL"))
    End Sub



    Sub Application_BeginRequest(sender As Object, e As EventArgs)
        'If ConfigurationManager.AppSettings("ReminderEmailIsEnabled") = "Y" Then
        '    'If the dummy page is hit, then it means we want to add another item in cache        
        '    If HttpContext.Current.Request.Url.ToString().ToLower = ConfigurationManager.AppSettings("KeepAliveURL") Or _
        '       HttpContext.Current.Request.Url.ToString().ToLower = ConfigurationManager.AppSettings("KeepAliveURL2") Then

        '        'Add the item in cache and when succesful, do the work.

        '        RegisterCache()

        '        Return
        '    End If

        'End If
        'If the dummy page is hit, then it means we want to add another item in cache        
        If HttpContext.Current.Request.Url.ToString().ToLower = ConfigurationManager.AppSettings("KeepAliveURL") Or
           HttpContext.Current.Request.Url.ToString().ToLower = ConfigurationManager.AppSettings("KeepAliveURL2") Then

            'Add the item in cache and when succesful, do the work.

            RegisterCache()

            Return
        End If
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim ErrorDescription As String = ""
        Dim ErrMsg As String = Server.GetLastError.ToString

        ' Remove extra message up to "---->"
        Dim Loc As Integer = InStr(ErrMsg, "--->")

        ErrMsg = Mid(ErrMsg, Loc + 4)
        Loc = InStr(ErrMsg, "--- End of inner exception stack trace ---")
        If Loc > 2 Then ErrMsg = Left(ErrMsg, Loc - 2)
        ErrorDescription &= ErrMsg

        ErrorDescription &= vbCrLf & "========================="

        ErrorDescription &= vbCrLf & "URL=" & Request.Url.ToString
        ErrorDescription &= vbCrLf & "REFERER=" & Request.ServerVariables("HTTP_REFERER")
        ErrorDescription &= vbCrLf & "BROWSER=" & Request.ServerVariables("HTTP_USER_AGENT")
        ErrorDescription &= vbCrLf & "--------------------------------------------------"

        ' Save passed values to aid reconstruction
        ErrorDescription &= vbCrLf & ">> QUERY STRING"
        Dim Key As String
        For Each Key In Request.QueryString.AllKeys
            ErrorDescription &= vbCrLf & Key & "=" & Request.QueryString(Key)
        Next

        ErrorDescription &= vbCrLf & ">> FORM COLLECTION"
        For Each Key In Request.Form.AllKeys
            If Not ("__" = Left(Key, 2)) Then
                ErrorDescription &= vbCrLf & Key & "=" & Request.Form(Key)
            End If
        Next

        Try
            ALOD.Logging.LogManager.LogError(ErrorDescription)
        Catch ex As Exception
            'since we are the error handler, let's ignore this one
        End Try

        'since we encountered an error, rollback any pending database actions
        NHibernateSessionManager.Instance.RollbackTransaction()

        If AppMode <> DeployMode.Development Then

            If TypeOf Server.GetLastError() Is HttpRequestValidationException Then
                HandleRequestValidationException()
            Else
                Server.ClearError()
                Response.Redirect("~/Public/AppError.htm")
            End If
        End If

        If TypeOf Server.GetLastError() Is HttpRequestValidationException Then
            HandleRequestValidationException()
        End If

    End Sub

    Private Sub HandleRequestValidationException()
        Server.ClearError()
        If HttpContext.Current.Request.Url.ToString().ToLower.Contains("logout.aspx") <> True Then
            Response.Redirect("~/Public/AppValidationError.htm")
        Else
            Response.Redirect(HttpContext.Current.Request.Url.ToString())
        End If
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
        If (Not Request.Cookies(Web.Security.FormsAuthentication.FormsCookieName) Is Nothing) Then
            'session has timed out
            '   Response.Redirect("~/Public/Logout.aspx")
        End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub

</script>