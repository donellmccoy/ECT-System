Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.UserControls

    'For using this control we need to set the values for Review and Concur and Enabled property
    Partial Class Secure_Shared_UserControls_ReviewControl
        Inherits System.Web.UI.UserControl

        Protected _section As String = ""

#Region "Properties"

        Public Property ApproveDisapprove() As Boolean
            Get
                Return (rblDecison.Items(0).Text = "Approved")
            End Get
            Set(ByVal value As Boolean)
                If (value) Then
                    rblDecison.Items(0).Text = "Approved"
                    rblDecison.Items(1).Text = "Disapproved"
                End If
            End Set

        End Property

        Public Property Decision() As String
            Get
                Return rblDecison.SelectedValue
            End Get
            Set(ByVal value As String)
                rblDecison.SelectedValue = value
                lblDecision.Text = Server.HtmlEncode(rblDecison.SelectedItem.Text)
            End Set
        End Property

        Public Property Enabled() As Boolean
            Get
                Return rblDecison.Visible
            End Get
            Set(ByVal value As Boolean)
                rblDecison.Visible = value
                txtReview.Enabled = value

                If (value) Then
                    lblDecision.Text = ""
                End If
                lblDecision.Visible = Not (value)
            End Set

        End Property

        Public Property Review() As String
            Get
                Return Server.HtmlEncode(txtReview.Text)
            End Get
            Set(ByVal value As String)
                txtReview.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public Property Section() As String
            Get
                Return _section

            End Get
            Set(ByVal value As String)
                _section = value
                txtReview.Attributes.Add("Section", value)
                rblDecison.Attributes.Add("Section", value)

            End Set
        End Property

#End Region

        Public Sub Initialize(ByVal strDecision As String, ByVal strReview As String, ByVal IsEnabled As Boolean, Optional ByVal IsApproveDisapprove As Boolean = False)

            Decision = strDecision
            txtReview.Text = Server.HtmlDecode(strReview)
            Enabled = IsEnabled
            ApproveDisapprove = IsApproveDisapprove
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            txtReview.Attributes.Add("Field", "Remarks")
            rblDecison.Attributes.Add("Field", "Findings")
            SetInputFormatRestriction(Page, txtReview, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

        End Sub

    End Class

End Namespace