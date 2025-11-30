Imports System.Web
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data.Services

Namespace Common
    Public Module UserCanEdit

        Public Function GetAccess(ByVal Pages As IList(Of PageAccess), ByVal CheckLock As Boolean) As Boolean

            'if we don't have a case lock aquired, this will be read-only regardless of pageacess
            If (CheckLock AndAlso Not SESSION_LOCK_AQUIRED) Then
                Return False
            End If

            'otherwise, we have a case lock, so check page access
            Dim access = (From t In Pages Where t.PageTitle = SiteMap.CurrentNode.Title Select t).SingleOrDefault()

            If (access Is Nothing) Then
                Return False
            Else
                If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite) Then
                    Return True
                Else
                    Return False
                End If
            End If

        End Function

        Public Function GetAccessLOD(ByVal Pages As IList(Of PageAccess), ByVal CheckLock As Boolean, ByVal LOD As LineOfDuty) As Boolean

            'if we don't have a case lock aquired, this will be read-only regardless of pageacess
            If (CheckLock AndAlso Not SESSION_LOCK_AQUIRED) Then
                Return False
            End If

            'otherwise, we have a case lock, so check page access
            Dim access = (From t In Pages Where t.PageTitle = SiteMap.CurrentNode.Title Select t).SingleOrDefault()

            If (access Is Nothing) Then
                Return False
            Else
                If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite) Then
                    'Check is the user is a Board role
                    If (Utility.IsUserBelongsToTheBoard(SESSION_GROUP_ID, True, True)) Then
                        Return True
                    Else 'Continue Normal Verification for the Attach PASSCODE
                        'Need to verify if the Case has an attach PASSCODE
                        'If yes and reached this point, means the user has Read/Write "Medical".
                        'Need to evaluate if the user belongs to the Attach PASSCODE
                        'If yes, user can edit, If no, user Can not edit
                        'UserCanEdit = True

                        'Dim lod As LineOfDuty = LodService.GetById(refId)

                        If (LOD.isAttachPas) Then
                            'First , evaluate if the user is part of the Attach PASSCODE
                            If (UserService.IsMemberPartOfAttachUnit(SESSION_USER_ID, LOD.MemberAttachedUnitId)) Then
                                'User is from the Attach PASSCODE, can have write access
                                Return True
                            Else
                                'User is from the Member PASSCODE, only read access
                                Return False

                            End If
                        Else 'Continue normal Execution since the LOD does not have an Attach PASSCODE
                            Return True
                        End If

                    End If
                    If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                        If (LOD.LODMedical.PhysicianCancelReason <> 0) Then
                            Return False
                        End If
                    End If
                Else
                    Return False
                End If
            End If
        End Function

    End Module

End Namespace