Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Data.Services

Namespace Common
    Public Module SARCUtility

        Public Function CreateSARCFinding(ByVal sarcId As Integer) As RestrictedSARCFindings
            Dim cFinding As RestrictedSARCFindings = New RestrictedSARCFindings()
            Dim currUser = UserService.CurrentUser

            cFinding.SARCID = sarcId
            cFinding.SSN = currUser.SSN
            cFinding.Compo = currUser.Component
            cFinding.Rank = currUser.Rank.Rank
            cFinding.Grade = currUser.Rank.Grade
            cFinding.Name = currUser.FullName
            cFinding.ModifiedBy = currUser.Id
            cFinding.ModifiedDate = DateTime.Now
            cFinding.CreatedBy = currUser.Id
            cFinding.CreatedDate = DateTime.Now

            Return cFinding
        End Function

        Public Function HasFinding(ByVal finding As RestrictedSARCFindings) As Boolean
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

    End Module
End Namespace