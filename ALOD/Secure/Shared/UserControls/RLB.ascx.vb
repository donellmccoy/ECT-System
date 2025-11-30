Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Utils

Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_RLB
        Inherits System.Web.UI.UserControl

        Public Property Enabled() As Boolean
            Get
                Return txtMedTechComments.Enabled
            End Get
            Set(ByVal value As Boolean)
                txtMedTechComments.Visible = value
                UnitCommentsLbl.Visible = Not (value)
            End Set
        End Property

        Public Property MedTechComments() As String
            Get
                Return Server.HtmlEncode(txtMedTechComments.Text.Trim)
            End Get
            Set(ByVal value As String)
                txtMedTechComments.Text = Server.HtmlDecode(value)
            End Set
        End Property

        Public Sub Initialize(ByRef rwRec As ALOD.Core.Domain.Modules.Lod.Rwoa)

            If (rwRec IsNot Nothing) Then
                Dim reason As Short = rwRec.ReasonSentBack
                RLBExplantionLbl.Text = rwRec.ExplantionSendingBack
                UnitCommentsLbl.Text = rwRec.CommentsBackToSender
                lblComments.Text = "Comments from Board:"

                If reason <> 0 Then
                    lblRwoaReason.Text = LookupService.GetRwoaReasonDescription(CType(reason, Int16))
                    txtMedTechComments.Text = rwRec.CommentsBackToSender
                End If
            End If
        End Sub

        Public Sub Initialize(ByRef rwRec As ALOD.Core.Domain.Modules.Lod.Return)

            If (rwRec IsNot Nothing) Then
                Dim reason As Short = rwRec.ReasonSentBack
                RLBExplantionLbl.Text = rwRec.ExplantionSendingBack
                UnitCommentsLbl.Text = rwRec.CommentsBackToSender
                lblComments.Text = "Return Comments:"

                If reason <> 0 Then
                    lblRwoaReason.Text = LookupService.GetRwoaReasonDescription(CType(reason, Int16))
                    txtMedTechComments.Text = rwRec.CommentsBackToSender
                End If
            End If
        End Sub

        Public Sub Initialize(ByRef _iLod As LineOfDuty)
            If _iLod.RwoaReason IsNot Nothing Then
                Dim reason As Short = _iLod.RwoaReason
                RLBExplantionLbl.Text = _iLod.RwoaExplanation
                UnitCommentsLbl.Text = _iLod.MedTechComments
                lblComments.Text = "Comments from Board:"
                If reason <> 0 Then
                    lblRwoaReason.Text = LookupService.GetRwoaReasonDescription(CType(reason, Int16))
                    txtMedTechComments.Text = Server.HtmlDecode(_iLod.MedTechComments)
                End If
            End If
        End Sub

        Public Sub SaveMedTechComments(ByRef _iLod As LineOfDuty)
            _iLod.MedTechComments = MedTechComments
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            SetMaxLength(txtMedTechComments)
            SetInputFormatRestriction(Page, txtMedTechComments, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

    End Class

End Namespace