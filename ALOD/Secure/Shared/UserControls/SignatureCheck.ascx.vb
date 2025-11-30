Imports ALOD.Core.Domain.DBSign
Imports ALOD.Data.Services

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_SigntureCheck
        Inherits System.Web.UI.UserControl

        Public Const KEY_PRIMARY As String = "PrimaryKey"
        Public Const KEY_SECONDARY As String = "SecondaryKey"
        Public Const KEY_SIGSTATUS As String = "SigStatus"
        Public Const KEY_TEMPLATE As String = "DbSignTemplate"

        Public Property CssClass() As String
            Get
                Dim key As String = "CssClass"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = ""
                End If
                Return CType(ViewState(key), String)
            End Get
            Set(ByVal value As String)
                Dim key As String = "CssClass"
                ViewState(key) = value
            End Set
        End Property

        Public Property PrimaryKey() As Integer
            Get
                If (ViewState(KEY_PRIMARY) Is Nothing) Then
                    ViewState(KEY_PRIMARY) = 0
                End If
                Return CType(ViewState(KEY_PRIMARY), Integer)
            End Get
            Set(ByVal value As Integer)
                ViewState(KEY_PRIMARY) = value
            End Set
        End Property

        Public Property SecondaryKey() As Integer
            Get
                If (ViewState(KEY_SECONDARY) Is Nothing) Then
                    ViewState(KEY_SECONDARY) = 0
                End If
                Return CType(ViewState(KEY_SECONDARY), Integer)
            End Get
            Set(ByVal value As Integer)
                ViewState(KEY_SECONDARY) = value
            End Set
        End Property

        Public Property SignatureStatus() As DBSignResult
            Get
                If (ViewState(KEY_SIGSTATUS) Is Nothing) Then
                    ViewState(KEY_SIGSTATUS) = DBSignResult.Unknown
                End If
                Return CType(ViewState(KEY_SIGSTATUS), DBSignResult)
            End Get
            Set(ByVal value As DBSignResult)
                ViewState(KEY_SIGSTATUS) = value
            End Set
        End Property

        Public Property Template() As DBSignTemplateId
            Get
                If (ViewState(KEY_TEMPLATE) Is Nothing) Then
                    ViewState(KEY_TEMPLATE) = DBSignTemplateId.SignOnly
                End If
                Return CType(ViewState(KEY_TEMPLATE), DBSignTemplateId)
            End Get
            Set(ByVal value As DBSignTemplateId)
                ViewState(KEY_TEMPLATE) = value
            End Set
        End Property

        Public Sub VerifySignature()
            VerifySignature(Me.PrimaryKey, Me.SecondaryKey)
        End Sub

        Public Sub VerifySignature(ByVal primaryKey As Integer)
            VerifySignature(primaryKey, Me.SecondaryKey)
        End Sub

        Public Sub VerifySignature(ByVal primaryKey As Integer, ByVal secondaryKey As Integer)

            If (primaryKey = 0 OrElse Template = DBSignTemplateId.SignOnly) Then
                'we can't verify sign only, or if the primary key hasn't been set
                Exit Sub
            End If

            Me.PrimaryKey = primaryKey
            Me.SecondaryKey = secondaryKey

            Dim service As New DBSignService(Template, primaryKey, secondaryKey)

            service.VerifySignature()

            Select Case service.Result
                Case DBSignResult.SignatureValid
                    StatusLabel.Text = "Signature Verified"
                    StatusLabel.ForeColor = Drawing.Color.Green
                    StatusImage.ImageUrl = "~/images/sig_valid.gif"

                    StatusImage.Attributes.Add("onclick", "getDbSignInfo(" +
                                               primaryKey.ToString() + "," +
                                               secondaryKey.ToString() + "," +
                                               CStr(Template) + ");")

                    StatusImage.Style("cursor") = "pointer"
                    StatusImage.ToolTip = "View Signature Info"
                    Me.Visible = True

                Case DBSignResult.SignatureInvalid
                    StatusLabel.Text = "Unable to verify digital signature"
                    StatusLabel.ForeColor = Drawing.Color.Red
                    StatusImage.ImageUrl = "~/images/sig_error.gif"
                    Me.Visible = True
                Case DBSignResult.NoSignature
                    StatusLabel.Text = "No signature found"
                    Me.Visible = False
                Case Else
                    StatusLabel.Text = "Temporarily unable to verify signature"
                    Me.Visible = False
            End Select

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            WrapperPanel.CssClass = CssClass
        End Sub

    End Class

End Namespace