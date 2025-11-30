Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class ICDCodeModalPopupControl
        Inherits System.Web.UI.UserControl

        Public Event ICDCodeSubmitted(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)

        Public Sub Hide()
            mpeICDCodeModalPopup.Hide()
        End Sub

        Public Sub Show()
            mpeICDCodeModalPopup.Show()
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
            ResetErrorControl()
            mpeICDCodeModalPopup.Hide()
        End Sub

        Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If (Not ucICDCodeControl.IsICDCodeSelected()) Then
                lblErrorMessages.Text = "Must select an ICD Code in order to submit!"
                lblErrorMessages.Visible = True
                Exit Sub
            End If

            ResetErrorControl()

            Dim eventArgs As ICDCodeSelectedEventArgs = New ICDCodeSelectedEventArgs()

            eventArgs.SelectedICDCodeId = ucICDCodeControl.SelectedICDCodeID

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                eventArgs.SelectedICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                eventArgs.SelectedICD7thCharacter = Nothing
            End If

            mpeICDCodeModalPopup.Hide()

            RaiseEvent ICDCodeSubmitted(Me, eventArgs)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ucICDCodeControl.Initialilze(Me.Page)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (IsPostBack) Then
                mpeICDCodeModalPopup.Show()
            Else
                ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
            End If
        End Sub

        Protected Sub ResetErrorControl()
            lblErrorMessages.Text = String.Empty
            lblErrorMessages.Visible = False
        End Sub

    End Class

End Namespace