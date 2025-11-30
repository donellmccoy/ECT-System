Imports ALOD.Core.Domain.DBSign
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports Newtonsoft.Json

Namespace Web.DBSign

    Partial Class Signing
        Inherits System.Web.UI.Page

        Public ReadOnly Property DbsAppSigPutUrl As String
            Get
                Return "receivesignature.aspx?refId=" + Request.QueryString("id") + "&workflow=" + Request.QueryString("workflow") + "&ptype=" + Request.QueryString("ptype")
            End Get
        End Property

        Public ReadOnly Property DbsDataUrl As String
            Get
                Return ConfigurationManager.AppSettings("DbSignUrl")
                'Return "https://alodtest.afrc.af.mil/dbsign/server"
            End Get
        End Property

        Public ReadOnly Property DbsFailureUrl As String
            Get
                Return "failure.aspx?&sgn=" + Request.QueryString("sgn")
            End Get
        End Property

        Public ReadOnly Property DbsInstanceName As String
            Get
                Return ConfigurationManager.AppSettings("DbSignDatabase")
                'Return "ALOD_DB"
            End Get
        End Property

        Public ReadOnly Property DbsMobileLicenseId As String
            Get
                Return "4FCD0B2CC37EDF3F3C5FDEA651957D9C937EF666"
            End Get
        End Property

        Public ReadOnly Property DbsSuccessUrl As String
            Get
                Return "successful.aspx?&sgn=" + Request.QueryString("sgn")
            End Get
        End Property

        Public ReadOnly Property SGN As String
            Get
                Return Request.QueryString("sgn")
            End Get
        End Property

        Protected ReadOnly Property RefId As Integer
            Get
                If (Request.QueryString("id") Is Nothing) Then
                    Return 0
                End If

                Return CInt(Request.QueryString("id"))
            End Get
        End Property

        Protected ReadOnly Property SecId As Integer
            Get
                If (Request.QueryString("secId") Is Nothing) Then
                    Return 0
                End If

                Return CInt(Request.QueryString("secId"))
            End Get
        End Property

        'Protected ReadOnly Property BaseURL As String
        '    Get
        '        Return ConfigurationManager.AppSettings("DbSignUrl")
        '    End Get
        'End Property

        'Protected ReadOnly Property AppPool As String
        '    Get
        '        Return ConfigurationManager.AppSettings("DbSignDatabase")
        '    End Get
        'End Property

        'Protected ReadOnly Property RefId As Integer
        '    Get
        '        If (Request.QueryString("id") Is Nothing) Then
        '            Return 0
        '        End If

        '        Return CInt(Request.QueryString("id"))
        '    End Get
        'End Property

        'Protected ReadOnly Property SecId As Integer
        '    Get
        '        If (Request.QueryString("secId") Is Nothing) Then
        '            Return 0
        '        End If

        '        Return CInt(Request.QueryString("secId"))
        '    End Get
        'End Property

        'Protected ReadOnly Property SuccessURL As String
        '    Get
        '        Return "SUCCESSFUL.ASPX?sgn=" + Request.QueryString("sgn")
        '    End Get
        'End Property

        'Protected ReadOnly Property FailureURL As String
        '    Get
        '        Return "FAILURE.ASPX?&sgn=" + Request.QueryString("sgn")
        '    End Get
        'End Property

        'Protected ReadOnly Property SignatureUrl As String
        '    Get
        '        Return "receivesignature.aspx?refId=" + Request.QueryString("id") + "&workflow=" + Request.QueryString("workflow") + "&ptype=" + Request.QueryString("ptype")
        '    End Get
        'End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            WriteHostName(Page)

            'Dim refId As Integer = CInt(Request.QueryString("id"))
            Dim templateId As DBSignTemplateId = CType(CByte(Request.QueryString("mode")), DBSignTemplateId)
            Dim service As New DBSignService(templateId, RefId, 0)
            'Dim secId As Integer = 0

            'If (Request.QueryString("secId") IsNot Nothing) Then
            '    Integer.TryParse(Request.QueryString("secId"), secId)
            'End If

            Dim signOnly As Boolean = IsNothing(templateId) OrElse templateId = DBSignTemplateId.SignOnly

            RegisterDBsignCommonParams()

            If signOnly Then

                Dim rand As New Random
                Dim dtbs As String = rand.Next().ToString()

                RegisterDBsignSigningParams(dtbs)
            Else

                Dim templatePrimaryKeys As Dictionary(Of String, String) = service.Template.GetPrimaryKeys(RefId.ToString(), SecId.ToString())
                RegisterDBsignSigningParams(service.TemplateTableName, templatePrimaryKeys)

            End If

            'Dim sbObject As StringBuilder = New StringBuilder("")
            'Dim sbEmbed As New StringBuilder()

            '' Common parameters
            'sbObject.Append(GetCommonIEParameters())
            'sbEmbed.Append(GetCommonFirefoxParameters())

            'If (templateId = DBSignTemplateId.SignOnly) Then
            '    ' PKI verifiaction only
            '    sbObject.Append(GetSignOnlyIEParameters())
            '    sbEmbed.Append(GetSignOnlyFirefoxParameters())
            'Else
            '    ' Template signing
            '    sbObject.Append(GetTemplateSigningIEParameters(service))
            '    sbEmbed.Append(GetTemplateSigningFirefoxParameters(service))
            'End If

            ''litParamsObject.Text = Server.HtmlEncode(sbObject.ToString)
            ''litParamsEmbed.Text = Server.HtmlEncode(sbEmbed.ToString())

            ''litParamsObject.Text = sbObject.ToString()

        End Sub

        Private Sub RegisterDBsignCommonParams()

            Dim csname As String = "RegisterDBsignCommonParams"
            Dim cstype As Type = Page.GetType()
            Dim cs As ClientScriptManager = Page.ClientScript

            If Not cs.IsStartupScriptRegistered(cstype, csname) Then

                Dim sb As New StringBuilder
                sb.AppendLine(String.Format("var DBS_DATA_URL = '{0}';", DbsDataUrl))
                sb.AppendLine(String.Format("var DBS_DATA_URL_SAS = '{0}';", DbsDataUrl))
                sb.AppendLine(String.Format("var DBS_MOBILE_LICENSE_ID = '{0}';", DbsMobileLicenseId))
                sb.AppendLine(String.Format("var DBS_INSTANCE_NAME = '{0}';", DbsInstanceName))
                sb.AppendLine(String.Format("var DBS_SUCCESS_URL = '{0}';", DbsSuccessUrl))
                sb.AppendLine(String.Format("var DBS_FAILURE_URL = '{0}';", DbsFailureUrl))
                sb.AppendLine(String.Format("var SGN = '{0}';", SGN))

                cs.RegisterStartupScript(Page.GetType(), csname, sb.ToString(), True)

            End If

        End Sub

        Private Sub RegisterDBsignSigningParams(dtbs As String)

            Dim csname As String = "RegisterDBsignSigningParams"
            Dim cstype As Type = Page.GetType()
            Dim cs As ClientScriptManager = Page.ClientScript

            If Not cs.IsStartupScriptRegistered(cstype, csname) Then

                Dim sb As New StringBuilder
                sb.AppendLine("var SIGN_MODE = 'SignOnly';")
                sb.AppendLine(String.Format("var DBS_DTBS = '{0}';", dtbs))
                sb.AppendLine("var DBS_DTBS_FMT = 'TEXT';")
                sb.AppendLine(String.Format("var DBS_APP_SIG_PUT_URL = '{0}';", DbsAppSigPutUrl))

                cs.RegisterStartupScript(Page.GetType(), csname, sb.ToString(), True)

            End If

        End Sub

        Private Sub RegisterDBsignSigningParams(templateName As String, templatePrimaryKeys As Dictionary(Of String, String))

            Dim csname As String = "RegisterDBsignSigningParams"
            Dim cstype As Type = Page.GetType()
            Dim cs As ClientScriptManager = Page.ClientScript

            If Not cs.IsStartupScriptRegistered(cstype, csname) Then

                Dim sb As New StringBuilder
                sb.AppendLine("var SIGN_MODE = 'Template';")
                sb.AppendLine("var DBS_DTBS_FMT = 'TEXT';")
                sb.AppendLine(String.Format("var DBS_APP_SIG_PUT_URL = '{0}';", DbsAppSigPutUrl))

                sb.AppendLine(String.Format("var DBS_TEMPLATE_NAME = '{0}';", templateName))
                sb.AppendLine(String.Format("var DBS_PK_LIST = {0};", JsonConvert.SerializeObject(templatePrimaryKeys)))

                cs.RegisterStartupScript(Page.GetType(), csname, sb.ToString(), True)

            End If

        End Sub

        'Protected Function GetCommonIEParameters() As String
        '    Dim parameters As StringBuilder = New StringBuilder("")

        '    parameters.Append("<param name='DATA_URL' value='" + BaseURL + "' />" & vbCrLf)
        '    parameters.Append("<param name='INSTANCE_NAME' value='" + AppPool + "' />" & vbCrLf)
        '    parameters.Append("<param name='SCRIPTED' value='false' />" & vbCrLf)
        '    parameters.Append("<param name='SUCCESS_URL' value='" + SuccessURL + "' />" & vbCrLf)
        '    parameters.Append("<param name='FAILURE_URL' value='" + FailureURL + "' />" & vbCrLf)

        '    Return parameters.ToString()
        'End Function

        'Protected Function GetCommonFirefoxParameters() As String
        '    Dim parameters As StringBuilder = New StringBuilder("")

        '    parameters.Append("DATA_URL=" + BaseURL + vbCrLf)
        '    parameters.Append("INSTANCE_NAME=" + AppPool + vbCrLf)
        '    parameters.Append("SCRIPTED=false" + vbCrLf)
        '    parameters.Append("SUCCESS_URL=" + SuccessURL & vbCrLf)
        '    parameters.Append("FAILURE_URL=" + FailureURL + vbCrLf)

        '    Return parameters.ToString()
        'End Function

        'Protected Function GetSignOnlyIEParameters() As String
        '    Dim parameters As StringBuilder = New StringBuilder("")
        '    Dim ran As Random = New Random()

        '    parameters.Append("<param name='APP_SIG_PUT_INCLUDES_SIGNER_DN' value='true' />" & vbCrLf)
        '    parameters.Append("<param name='SIG_INCLUDE_SIGNER_CERT' value='true' />" & vbCrLf)
        '    parameters.Append("<param name='CONTENT_TYPE' value='APP_SIGN' />" & vbCrLf)
        '    'sbObject.Append("<param name='APP_SIG_PUT_URL' value='" + successUrl + "' />" & vbCrLf)
        '    'sbObject.Append("<param name='APP_SIG_PUT_NAVIGATES_BROWSER' value='1' />" & vbCrLf)
        '    'this call in dbsign 3 navigated to the app_sig_put_url instead of success url, which
        '    'doesn't seem to make sense here because the successurl isn't set and the app sig is the success
        '    parameters.Append("<param name='APP_DTBS' value='" + ran.Next().ToString() + "' />" + vbCrLf)
        '    parameters.Append("<param name='APP_DTBS_FMT' value='TEXT' />" & vbCrLf)
        '    parameters.Append("<param name='APP_SIG_PUT_URL' value='" + SignatureUrl + "' />" & vbCrLf)

        '    Return parameters.ToString()
        'End Function

        'Protected Function GetSignOnlyFirefoxParameters() As String
        '    Dim parameters As StringBuilder = New StringBuilder("")
        '    Dim ran As Random = New Random()

        '    parameters.Append("SIG_INCLUDE_SIGNER_CERT=true" & vbCrLf)
        '    parameters.Append("CONTENT_TYPE=APP_SIGN" & vbCrLf)
        '    'sbEmbed.Append("APP_SIG_PUT_URL=" + successUrl + vbCrLf)
        '    'sbEmbed.Append("APP_SIG_PUT_NAVIGATES_BROWSER=1" & vbCrLf)
        '    parameters.Append("APP_DTBS=" + ran.Next().ToString() + vbCrLf)
        '    parameters.Append("APP_DTBS_FMT=TEXT" + vbCrLf)

        '    Return parameters.ToString()
        'End Function

        'Protected Function GetTemplateSigningIEParameters(ByVal service As DBSignService) As String
        '    Dim parameters As StringBuilder = New StringBuilder("")

        '    parameters.Append("<param name='CONTENT_TYPE' value='SIGN' />" & vbCrLf)
        '    parameters.Append("<param name='SIG_PUT_URL' value='" + BaseURL + "' />" & vbCrLf)
        '    parameters.Append("<param name='Template_Name' value='" + service.TemplateTableName + "' />" & vbCrLf)
        '    parameters.Append("<param name='APP_SIG_PUT_URL' value='" + SuccessURL + "' />" & vbCrLf)

        '    parameters.Append(service.Template.GetKeyListParameter(DBSignSupportedBrowsers.InternetExplorer))
        '    parameters.Append(service.Template.GetPrimaryKeyParameter(DBSignSupportedBrowsers.InternetExplorer, RefId.ToString()))
        '    parameters.Append(service.Template.GetSecondaryKeyParameter(DBSignSupportedBrowsers.InternetExplorer, SecId.ToString()))

        '    Return parameters.ToString()
        'End Function

        'Protected Function GetTemplateSigningFirefoxParameters(ByVal service As DBSignService) As String
        '    Dim parameters As StringBuilder = New StringBuilder("")

        '    parameters.Append("CONTENT_TYPE=SIGN" + vbCrLf)
        '    parameters.Append("SIG_PUT_URL=" + BaseURL + vbCrLf)
        '    parameters.Append("Template_Name=" + service.TemplateTableName + vbCrLf)
        '    parameters.Append("APP_SIG_PUT_URL=" + SuccessURL + vbCrLf)

        '    parameters.Append(service.Template.GetKeyListParameter(DBSignSupportedBrowsers.Firefox))
        '    parameters.Append(service.Template.GetPrimaryKeyParameter(DBSignSupportedBrowsers.Firefox, RefId.ToString()))
        '    parameters.Append(service.Template.GetSecondaryKeyParameter(DBSignSupportedBrowsers.Firefox, SecId.ToString()))

        '    Return parameters.ToString()
        'End Function
    End Class

End Namespace