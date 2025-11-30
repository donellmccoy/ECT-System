Option Explicit On
Option Strict On

Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.DBSign

    Partial Class Failure
        Inherits System.Web.UI.Page

        Const KEY_ERROR_DESCRIPTION As String = "ERROR_DESCRIPTION"
        Const KEY_ERROR_VAL As String = "DBS_ERROR_VAL"
        Const KEY_SIGN_RESULT As String = ""
        Const KEY_SIGRAN As String = ""

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'indicate that the signature failed
            Session.Remove(KEY_SIGRAN)
            Session(KEY_SIGN_RESULT) = "0"

            Dim errorMessage As String = String.Empty
            Dim errorCode As String = String.Empty

            If (Request.Params(KEY_ERROR_DESCRIPTION) IsNot Nothing) Then
                errorMessage = Server.HtmlEncode(Request.Params(KEY_ERROR_DESCRIPTION))
            End If

            If (Request.Params(KEY_ERROR_VAL) IsNot Nothing) Then
                errorCode = Request.Params(KEY_ERROR_VAL)
            End If

            Dim logMessage As String = String.Format("Signature Failed" + Environment.NewLine +
                                                "User: {0} " + Environment.NewLine +
                                                "UserId: {1} " + Environment.NewLine +
                                                "Error Code: {2}" + Environment.NewLine +
                                                "Error Msg: {3}",
                                                SESSION_USERNAME, SESSION_USER_ID,
                                                errorCode, errorMessage)

            LogManager.LogError(logMessage)
            lblErrorMsg.Text = errorMessage

        End Sub

    End Class

End Namespace