Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_SignatureBlock
        Inherits System.Web.UI.UserControl

        Const URL_CLEAR As String = "~/Secure/Shared/DBsign/clear.htm#"
        Const URL_SIGNING As String = "~/Secure/Shared/DBSign/signing.aspx"
        Const URL_SUCCESS As String = "~/Secure/Shared/DBSign/successful.aspx"

        Public Event SignCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs)

        Public Event SignStarted(ByVal sender As Object, ByVal e As SignStartedEventArgs)

        Public Property SignButton() As String
            Get
                Dim key As String = "SignButton"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = ""
                End If
                Return CType(ViewState(key), String)
            End Get
            Set(ByVal value As String)
                Dim key As String = "SignButton"
                ViewState(key) = value
            End Set
        End Property

        Public Sub ClearSignatureFrame()
            DBSignConversation.Attributes("src") = Page.ResolveUrl(URL_CLEAR)
            DBSignConversation.Attributes("height") = "0px"
        End Sub

        Public Sub StartSignature(ByVal refId As Integer, ByVal workflow As Integer, ByVal secId As Integer, ByVal text As String, ByVal statusIn As Integer, ByVal statusOut As Integer, ByVal optionId As Short, ByVal dbsign As DBSignTemplateId, ByVal comments As String)

            ViewState("actionText") = text
            ViewState("optionId") = optionId
            ViewState("statusIn") = statusIn
            ViewState("statusOut") = statusOut
            ViewState("refId") = refId
            ViewState("workflow") = workflow
            ViewState("secId") = secId
            ViewState("comments") = comments

            Dim args As New SignStartedEventArgs
            args.OptionId = optionId
            args.DBSign = dbsign
            args.StatusOut = statusOut
            args.StatusIn = statusIn
            args.Cancel = False
            args.RefId = refId
            args.SecondaryId = secId
            args.Comments = comments

            RaiseEvent SignStarted(Me, args)

            If (args.Cancel) Then
                Exit Sub
            End If

            'we generate a random number here and store it in the session
            'Dim number As Integer = New Random().Next()
            Dim number As Long = Now.ToFileTime()
            Session("SigRan") = number

            'we then also pass it to the signing page.
            'after the signature is complete we compare the numbers
            'this prevents users from calling the successful.aspx page directly
            'which would bypass the signing process

            Dim url As New StringBuilder
            url.Append(Page.Request.Url.Scheme + Uri.SchemeDelimiter + Page.Request.Url.Host)
            url.Append(":" + Page.Request.Url.Port.ToString())  'Add Port in case Application/DBSign does not use default port

            If (AppMode = DeployMode.Production OrElse ConfigurationManager.AppSettings("UseDBSign") = "Y") Then
                'use DBSign
                url.Append(Me.ResolveUrl("~/Secure/Shared/DBSign/signing.aspx"))
                url.Append("?id=" + refId.ToString())
                url.Append("&mode=" + CStr(dbsign))
                url.Append("&sgn=" + number.ToString())
                url.Append("&workflow=" + workflow.ToString())
                url.Append("&ptype=" + secId.ToString())

                If (secId > 0) Then
                    url.Append("&secId=" + secId.ToString())
                End If
            Else
                'bypass BBSign
                url.Append(Me.ResolveUrl("~/Secure/Shared/DBSign/successful.aspx"))
                url.Append("?id=" + refId.ToString())
                url.Append("&mode=" + CStr(dbsign))
                url.Append("&sgn=" + number.ToString())

                If (secId > 0) Then
                    url.Append("&secId=" + secId.ToString())
                End If
            End If

            DBSignConversation.Attributes("src") = url.ToString()
            DBSignConversation.Attributes("height") = "100px"

        End Sub

        ''' <summary>
        ''' This is called when the signature is completed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub btnProceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProceedButton.Click

            Dim signed As Boolean = False

            If (Session("SignResult") IsNot Nothing) Then
                If (Session("SignResult") = "1") Then
                    signed = True
                End If

                Session.Remove("SignResult")
            End If

            Dim args As New SignCompletedEventArgs
            args.SignaturePassed = signed
            args.OptionId = ViewState("optionId")
            args.StatusIn = ViewState("statusIn")
            args.StatusOut = ViewState("statusOut")
            args.Text = ViewState("actionText")
            args.RefId = ViewState("refId")
            args.Comments = ViewState("comments")

            RaiseEvent SignCompleted(Me, args)

            ClearSignatureFrame()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                DBSignConversation.Attributes("src") = Page.ResolveUrl(URL_CLEAR)
            End If
        End Sub

    End Class

End Namespace