Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Data.Services

Namespace Common
    Public Module PSIDUtility
        Public Function HasFinding(ByVal finding As SC_PSCD_Findings) As Boolean
            If (finding Is Nothing) Then
                Return False
            End If

            If (Not finding.Finding.HasValue OrElse finding.Finding.Value = 0) Then
                Return False
            End If

            Return True
        End Function

        Public Function IsTimeToSaveViaNavigator(ByVal buttonType As NavigatorButtonType) As Boolean
            If (buttonType = NavigatorButtonType.Save OrElse
                buttonType = NavigatorButtonType.NavigatedAway OrElse
                buttonType = NavigatorButtonType.NextStep OrElse
                buttonType = NavigatorButtonType.PreviousStep) Then
                Return True
            End If

            Return False
        End Function

        Public Function CreatePSIDFinding(ByVal psidID As Integer) As SC_PSCD_Findings
            Dim cFinding As SC_PSCD_Findings = New SC_PSCD_Findings()
            Dim currUser = UserService.CurrentUser

            cFinding.PSIDId = psidID
            cFinding.Name = currUser.FullName
            cFinding.ModifiedBy = currUser.Id
            cFinding.ModifiedDate = DateTime.Now
            cFinding.CreatedBy = currUser.Id
            cFinding.CreatedDate = DateTime.Now

            Return cFinding
        End Function
    End Module
End Namespace
