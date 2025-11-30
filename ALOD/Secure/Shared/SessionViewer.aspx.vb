Imports ALOD.Core.Utils
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web

    Partial Class SessionViewer
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Protected even As String = "#FFFFFF"
        Protected isEven As Boolean = False
        Protected odd As String = "#e6e6fa"

        Protected ReadOnly Property BGColor() As String
            Get
                isEven = Not isEven
                Return IIf(isEven, even, odd)
            End Get
        End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If (Not IsPostBack) Then

                If (AppMode <> DeployMode.Development) Then

                End If

                lnkRefresh.Attributes.Add("onClick", "DoRefresh();")
            End If

            Dim buffer As New System.Text.StringBuilder
            Dim count As Integer = 0

            For Each key As Object In Session.Keys

                buffer.Append("<tr bgcolor='" & BGColor.ToString() & "'>" & vbCrLf)
                buffer.Append("<td>&nbsp;" & key & "</td>" & vbCrLf)
                buffer.Append("<td>&nbsp;</td>" & vbCrLf)

                If (key Is Nothing) Or Session(key) Is Nothing Then
                    buffer.Append("<td style='color: blue;'>Nothing</td>" & vbCrLf)
                Else
                    buffer.Append("<td style='color: blue;'>" & Session(key).GetType().ToString() & "</td>" & vbCrLf)
                End If

                buffer.Append("<td>&nbsp;</td>" & vbCrLf)

                If (Session(key) Is Nothing) Then
                    buffer.Append("<td style='color: blue;'>Nothing</td>" & vbCrLf)
                    count += 1
                Else
                    buffer.Append("<td>" & Session(key).ToString() & "</td>" & vbCrLf)
                End If

                buffer.Append("</tr>")

            Next

            ltSummary.Text = buffer.ToString()
            lblCount.Text = Server.HtmlEncode(Session.Count.ToString())
            lblNulls.Text = Server.HtmlEncode(count.ToString())
            lblMode.Text = Server.HtmlEncode(Session.Mode.ToString())

            Dim auth As UserAuthentication = CType(User.Identity, UserAuthentication)
            lblPerms.Text = Server.HtmlEncode(auth.Roles.Replace(",", ", "))

        End Sub

    End Class

End Namespace