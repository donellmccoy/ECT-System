Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports ALODWebUtility.PrintingUtil

Public Class Form348
    Inherits System.Web.UI.Page
    Implements IDocumentCreate

    Protected Const EpochDate As Date = #1/29/2010#
    Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
    Private Const SIGNED_TEXT As String = "//SIGNED//"
    Private _sigDao As ISignatueMetaDateDao
    Private _wsdao As WorkStatusDao
    Private concurredFinding As Short
    Private lodid As Integer
    Private signatureService As DBSignService

    ReadOnly Property SigDao() As SignatureMetaDataDao
        Get
            If (_sigDao Is Nothing) Then
                _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
            End If

            Return _sigDao
        End Get
    End Property

    Protected ReadOnly Property wsdao As WorkStatusDao
        Get
            If (_wsdao Is Nothing) Then
                _wsdao = New WorkStatusDao()
            End If

            Return _wsdao
        End Get
    End Property

    Private Property VerifySource() As DBSignService Implements IDocumentCreate.VerifySource
        Get
            Return signatureService
        End Get
        Set(value As DBSignService)
            signatureService = value
        End Set
    End Property

    Public Function GeneratePDFForm(ByVal refId As Integer, replaceIOsig As Boolean) As Printing.PDFForm Implements IDocumentCreate.GeneratePDFForm
        Dim strComments As String = "Generated Form 348 PDF"
        Dim lod As LineOfDuty = LodService.GetById(refId)
        lodid = lod.Id
        Dim isRLod As Boolean = False

        If (lod.SarcCase AndAlso Not UserHasPermission(PERMISSION_VIEW_SARC_CASES)) Then
            If (lod.IsRestricted OrElse (Not lod.IsRestricted AndAlso Not UserHasPermission("SARCUnrestricted"))) Then
                Throw New Exception("User is attempting to view SARC PDF without permission refId:" + lod.Id.ToString)
            End If
        End If

        Dim form348 As New PDFForm(PrintDocuments.FormARFC348)

        SetFormField(form348, "lodCaseNumberP1", lod.CaseId)
        SetFormField(form348, "lodCaseNumberP2", lod.CaseId)

        'set member/unit info
        SetFormField(form348, "to", lod.ToUnit)
        SetFormField(form348, "from", lod.FromUnit)
        SetFormField(form348, "memberName", lod.MemberName)
        SetFormField(form348, "memberSSN", FormatSSN(lod.MemberSSN))
        SetFormField(form348, "memberGrade", lod.MemberGrade)
        SetFormField(form348, "memberUnit", lod.MemberUnit)

        SetMedicalInfo(form348, lod)

        SetUnitInfo(form348, lod)

        SetWingJudgeAdvocateInfo(form348, lod)

        SetWingCommanderInfo(form348, lod)

        '*************************************
        'The board section only gets added
        'to the 348 for informal cases
        '*************************************

        '*************************************
        ''If form 348 is not "Complete", we suppress the whole second page, Print-Out/PDF
        ''*************************************
        If (lod.WorkflowStatus.Id <> LodWorkStatus.Complete) Then
            'Suppress the page
            form348.SuppressSecondPage()
        Else
            '    'Continue normal execution

            If (lod.Formal) Then
                'this is a formal case, so add the 261 text
                SetFormField(form348, "medicalReview", "(See DD Form 261 for Formal investigation )")
            Else

                SetBoardMedicalInfo(form348, lod)

                SetBoardLegalInfo(form348, lod)

                SetBoardTechInfo(form348, lod)

                SetBoardApprovalInfo(form348, lod)

                If CheckForPriorStatus(lod.CurrentStatusCode, refId) Then
                    SetFormField(form348, "medicalReview", "This Block Not Used")
                    SetFormField(form348, "legalReview", "This Block Not Used")
                End If

            End If

        End If

        LogManager.LogAction(ModuleType.LOD, UserAction.ViewDocument, lodid, strComments)

        Return form348
    End Function

    Protected Function AddBoardFinding(ByVal doc As PDFForm, ByVal boardFinding As LineOfDutyFindings, ByVal concurField As String, ByVal findingField As String) As Boolean
        If (boardFinding Is Nothing) Then
            SetFormField(doc, concurField, String.Empty)
            SetFormField(doc, findingField, String.Empty)
            Return False
        End If

        Dim concurText As String = ""
        Dim newFinding As String = ""

        If (boardFinding.DecisionYN = "Y") Then
            concurText = "Concur with Appointing Authority"

            If (findingField = "medicalReview") Then
                Dim message As String = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their  pathophysiology and prognosis, as related to causation, the following determination was found in this case: "

                SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding)
                SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText))
                Return True
            Else
                SetFormField(doc, concurField, concurText + newFinding)
                SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText))
                Return True
            End If
        Else
            concurText = "Non Concur with Appointing Authority. Recommended new finding: "

            If (boardFinding.Finding.HasValue) Then
                newFinding = GetFindingFormText(boardFinding.Finding.Value)

                If (findingField = "medicalReview") Then
                    Dim message As String = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their  pathophysiology and prognosis, as related to causation, the following determination was found in this case: "

                    SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding)
                    SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText))
                    Return True
                Else
                    SetFormField(doc, concurField, concurText + newFinding)
                    SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText))
                    Return True
                End If
            Else
                Return False
            End If
        End If

        Return False

    End Function

    Protected Function AddSignatureToForm(ByVal doc As PDFForm, ByVal signature As SignatureMetaData, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal titleField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean
        If (signature Is Nothing) Then
            SetFormField(doc, dateField, String.Empty)
            SetFormField(doc, nameField, String.Empty)
            SetFormField(doc, sigField, String.Empty)
            SetFormField(doc, titleField, String.Empty)
            Return False
        End If

        SetFormField(doc, nameField, signature.NameAndRank)

        Dim dateSigned As Date = signature.date

        If (dateSigned < EpochDate) Then

            'use the old style signature
            SetFormField(doc, sigField, SIGNED_TEXT)

            'use the passed in date
            SetFormField(doc, dateField, dateSigned.ToString(DATE_FORMAT))

            'and we're done
            Return True

        End If

        'this signature occured after the epoch, so verify it
        VerifySource = New DBSignService(template, lodid, ptype)

        Dim valid As Boolean = False

        Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()

        If (signatureStatus = DBSignResult.SignatureValid) Then
            'if it's valid, add the signing info to the form
            Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()

            Dim sigLine As String = "Digitally signed by " _
                + signInfo.Signature + Environment.NewLine _
                + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT)

            SetFormField(doc, sigField, sigLine)
            SetFormField(doc, dateField, signInfo.DateSigned.ToString(DATE_FORMAT))
            valid = True
        Else
            'otherwise, clear those fields
            SetFormField(doc, sigField, SIGNED_TEXT)
            SetFormField(doc, dateField, signature.date.ToString())
            valid = False
        End If

        'finally set the title field
        SetFormField(doc, titleField, signature.Title)

        Return valid
    End Function

    Private Sub SetBoardApprovalInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus = wsdao.GetById(lod.Status)

        If (lodCurrStatus.StatusCodeType.IsFinal) Then

            Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.ApprovingAuthorityAction)

            If (Not sig Is Nothing) Then

                Dim approvingfinding As LineOfDutyFindings

                If (lod.Formal) Then
                    approvingfinding = lod.FindByType(PersonnelTypes.FORMAL_APP_AUTH)
                Else
                    approvingfinding = lod.FindByType(PersonnelTypes.BOARD_AA)
                End If

                If approvingfinding IsNot Nothing Then

                    If approvingfinding.Finding IsNot Nothing Then
                        Select Case approvingfinding.Finding
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348, "approvingILOD", "Yes")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348, "approvingRecommendFormal", "Yes")
                            Case Finding.Epts_Service_Aggravated
                                SetFormField(form348, "approvingEptsServiceAggravated", "Yes")
                            Case Finding.Nlod_Due_To_Own_Misconduct
                                SetFormField(form348, "approvingNotILodDom", "Yes")
                            Case Finding.Epts_Lod_Not_Applicable
                                SetFormField(form348, "approvingEptsNotApplicable", "Yes")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348, "approvingNotILodNotDom", "Yes")
                        End Select

                    End If

                    Dim approver As String = ""
                    'approval authority is slightly different

                    'we need to know if the board signed for the general
                    If (lod.BoardForGeneral = "Y") Then

                        Dim Techsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.BoardReview)

                        'add the board members name to the signature
                        If (Not Techsig Is Nothing) Then
                            approver += Techsig.NameAndRank + " for " + vbCrLf
                        End If
                    Else

                        'if the General signed it, add their signature
                        AddSignatureToForm(form348, sig,
                                                    "approvingDate", "approvingSignature",
                                                    "approvingRankName", "",
                                                    DBSignTemplateId.Form348Findings,
                                                    PersonnelTypes.BOARD_AA)
                    End If

                    approver += sig.NameAndRank _
                                + vbCrLf + sig.Title

                    SetFormField(form348, "approvingRankName", approver)

                End If
            End If

        End If
    End Sub

    Private Sub SetBoardLegalInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.BoardLegalReview)

        If (Not sig Is Nothing) Then

            Dim legalfinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.BOARD_JA)
            If (AddBoardFinding(form348, legalfinding, "legalSubstitutedFindings", "legalReview")) Then

                AddSignatureToForm(form348, sig,
                                                "legalReviewDate", "legalReviewSignature",
                                                "legalReviewName", "legalReviewRank",
                                            DBSignTemplateId.Form348Findings,
                                            PersonnelTypes.BOARD_JA)
            End If

        End If
    End Sub

    Private Sub SetBoardMedicalInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.BoardMedicalReview)

        If (Not sig Is Nothing) Then

            Dim medicalfinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.BOARD_SG)
            If (AddBoardFinding(form348, medicalfinding, "medicalSubstitutedFindings", "medicalReview")) Then

                If (Not AddSignatureToForm(form348, sig,
                                                "medicalReviewlDate", "medicalReviewSignature",
                                                "medicalReviewName", "medicalReviewRank",
                                                DBSignTemplateId.Form348Findings,
                                                PersonnelTypes.BOARD_SG)) Then

                    'we failed to add a signature
                    'either it wasn't signed or the signature is not valid
                    'either way, clear the findings fields

                    ClearFormField(form348, "medicalSubstitutedFindings")
                    ClearFormField(form348, "medicalReview")
                    ClearFormField(form348, "medicalReviewlDate")
                    ClearFormField(form348, "medicalReviewSignature")
                    ClearFormField(form348, "medicalReviewName")
                    ClearFormField(form348, "medicalReviewRank")

                End If
            End If

        End If
    End Sub

    Private Sub SetBoardTechInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus = wsdao.GetById(lod.Status)

        If (lodCurrStatus.StatusCodeType.IsFinal) Then

            If (lod.FinalFindings IsNot Nothing AndAlso lod.FinalFindings.HasValue) Then

                Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.BoardReview)

                If (Not sig Is Nothing) Then

                    Select Case lod.FinalFindings.Value
                        Case Finding.In_Line_Of_Duty
                            SetFormField(form348, "boardILod", "Yes")
                        Case Finding.Recommend_Formal_Investigation
                            SetFormField(form348, "boardRecommendFormal", "Yes")
                        Case Finding.Epts_Service_Aggravated
                            SetFormField(form348, "boardEptsServiceAggravated", "Yes")
                    End Select

                    AddSignatureToForm(form348, sig,
                                    "boardReviewDate", "boardReviewSignature",
                                    "boardReviewRankName", "boardReviewRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.BOARD)

                    'if the board signed for the general, add the board findings and signature for the AA as well
                    If (lod.BoardForGeneral = "Y") Then

                        Select Case lod.FinalFindings.Value
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348, "approvingILOD", "Yes")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348, "approvingRecommendFormal", "Yes")
                            Case Finding.Epts_Service_Aggravated
                                SetFormField(form348, "approvingEptsServiceAggravated", "Yes")
                            Case Finding.Nlod_Due_To_Own_Misconduct
                                SetFormField(form348, "approvingNotILodDom", "Yes")
                            Case Finding.Epts_Lod_Not_Applicable
                                SetFormField(form348, "approvingEptsNotApplicable", "Yes")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348, "approvingNotILodNotDom", "Yes")
                        End Select

                        Dim Appsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.ApprovingAuthorityAction)

                        AddSignatureToForm(form348, Appsig,
                                    "approvingDate", "approvingSignature",
                                    "approvingRankName", "",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.BOARD)

                        Dim approver As String = ""

                        approver += sig.NameAndRank + " for " + vbCrLf

                        ' Check if the Approval Authority actually signed the case...
                        If (Not Appsig Is Nothing) Then
                            ' Use the approval authority signature information stored in the case record...
                            approver += Appsig.NameAndRank _
                                        + vbCrLf + Appsig.Title
                        Else
                            ' Use the Approval Authority selected by the Board Tech...
                            Dim approvalAuthority As ALOD.Core.Domain.Users.AppUser = Nothing
                            Dim title As String = String.Empty

                            If (lod.ApprovingAuthorityUserId.HasValue) Then
                                approvalAuthority = UserService.GetById(lod.ApprovingAuthorityUserId.Value)
                            End If

                            If (approvalAuthority IsNot Nothing) Then
                                title = UserService.GetUserAlternateTitle(approvalAuthority.Id, ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority)

                                Dim strSig As String = "USAF"
                                If SESSION_COMPO = "5" Then
                                    strSig = "ANG"
                                End If

                                If (Not String.IsNullOrEmpty(approvalAuthority.AlternateSignatureName)) Then
                                    approver += approvalAuthority.AlternateSignatureName + ", " + strSig + vbCrLf + title
                                Else
                                    approver += approvalAuthority.SignatureName + ", " + strSig + vbCrLf + title
                                End If
                            Else
                                approver += "UNKNOWN Approving Authority"
                            End If
                        End If

                        SetFormField(form348, "approvingRankName", approver)

                    End If

                End If

            End If

        End If
    End Sub

    Private Sub SetMedicalInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        'medical info
        If (lod.LODMedical.TreatmentDate IsNot Nothing) Then
            If (lod.LODMedical.TreatmentDate.HasValue) Then
                SetFormField(form348, "treatmentDate", Server.HtmlDecode(lod.LODMedical.TreatmentDate.Value.ToString(DATE_HOUR_FORMAT)))
            End If
        End If

        'start our diagnosis wit the nature of incident
        Dim natureOfIncident As String = lod.LODMedical.NatureOfIncidentDescription

        'add EPTS
        If (lod.LODMedical.Epts IsNot Nothing) AndAlso (lod.LODMedical.Epts.HasValue) Then
            Select Case lod.LODMedical.Epts.Value
                Case 0 'no EPTS
                    natureOfIncident += "    EPTS No"
                Case 1 'EPTS Yes - Service Aggrivated
                    natureOfIncident += "    EPTS Yes/SA"
                Case 2 'EPTS Yes - Not Service Aggrivated
                    natureOfIncident += "    EPTS Yes"
            End Select
        End If

        'add ICD9 info
        Dim code As ICD9Code = Nothing

        If (lod.LODMedical.ICD9Id IsNot Nothing) AndAlso (lod.LODMedical.ICD9Id.HasValue) Then
            code = LookupService.GetIcd9CodeById(lod.LODMedical.ICD9Id.Value)

            If (code IsNot Nothing) Then
                natureOfIncident += "    " + code.GetFullCode(lod.LODMedical.ICD7thCharacter) + " - " + code.Description
            End If

        End If

        'the incidentType field holds: Incident type - EPTS - ICD9 code - ICD Diagnosis
        SetFormField(form348, "incidentType", natureOfIncident)

        'set the free-form diagnosis and physician info
        Dim diagnosis As String = lod.LODMedical.DiagnosisText

        If (Not String.IsNullOrEmpty(lod.LODMedical.ApprovalComments)) Then
            diagnosis += Environment.NewLine
            diagnosis += Server.HtmlDecode(lod.LODMedical.ApprovalComments)
        End If

        SetFormField(form348, "diagnosis", Server.HtmlDecode(diagnosis))
        SetFormField(form348, "treatmentInfo", Server.HtmlDecode(lod.LODMedical.MedicalFacility))

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.MedicalOfficerReview)

        AddSignatureToForm(form348, sig,
                            "medicalDate", "medicalSignature",
                            "medicalName", "medicalRank",
                            DBSignTemplateId.Form348Medical,
                            PersonnelTypes.MED_OFF)
    End Sub

    Private Sub SetUnitInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        If lod.SarcCase AndAlso lod.IsRestricted Then
            Dim message As String = "This Block not Used"

            SetFormField(form348, "unitName", message)
            SetFormField(form348, "judgeAdvocateRankName", message)
            SetFormField(form348, "appointingName", message)
        Else

            '*************************************
            'Unit Commander
            '*************************************
            If lod.LODUnit IsNot Nothing Then

                Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.UnitCommanderReview)

                'we only display data if the UC has signed it
                If (Not sig Is Nothing) Then

                    If Not String.IsNullOrEmpty(lod.LODUnit.DutyStatusDescription) Then
                        If lod.LODUnit.DutyDetermination = DutyStatus.Active_Duty_Status Then
                            SetFormField(form348, "unitActiveDutyStatus", "Yes")
                            If lod.LODUnit.DutyFrom IsNot Nothing Then
                                If lod.LODUnit.DutyFrom.HasValue Then
                                    SetFormField(form348, "unitActiveDutyStartDate", Server.HtmlDecode(lod.LODUnit.DutyFrom.Value.ToString(DATE_FORMAT)))
                                End If
                            End If
                            If lod.LODUnit.DutyTo IsNot Nothing Then
                                If lod.LODUnit.DutyTo.HasValue Then
                                    SetFormField(form348, "unitActiveDutyEndDate", Server.HtmlDecode(lod.LODUnit.DutyTo.Value.ToString(DATE_FORMAT)))
                                End If
                            End If
                        ElseIf lod.LODUnit.DutyDetermination IsNot Nothing Then
                            SetFormField(form348, "unitInactiveDutyStatus", "Yes")
                            Select Case lod.LODUnit.DutyDetermination
                                Case DutyStatus.UTA
                                    SetFormField(form348, "unitInactiveDutyStatusUTA", "Yes")
                                Case DutyStatus.AFTP
                                    SetFormField(form348, "unitInactiveDutyStatusAFTP", "Yes")
                                Case DutyStatus.Saturday_night_rule
                                    SetFormField(form348, "unitInactiveDutyStatusSaturdayNightRule", "Yes")
                                Case DutyStatus.Travel_to_from_duty
                                    SetFormField(form348, "unitInactiveDutyStatusTravelDuty", "Yes")
                                Case DutyStatus.Unit_sponsored_event
                                    SetFormField(form348, "unitInactiveDutyStatusUnitSponsoredEvent", "Yes")
                                Case DutyStatus.Other
                                    SetFormField(form348, "unitInactiveDutyStatusOther", "Yes")
                                    SetFormField(form348, "unitOtherInfo", Server.HtmlDecode(lod.LODUnit.OtherDutyStatus))
                            End Select
                        End If

                    End If ' end (if signed)

                End If

                SetFormField(form348, "unitFindings", Server.HtmlDecode(lod.LODUnit.AccidentDetails))

                Dim unitFinding As LineOfDutyFindings
                unitFinding = lod.FindByType(PersonnelTypes.UNIT_CMDR)
                If unitFinding IsNot Nothing Then

                    If unitFinding.Finding IsNot Nothing Then
                        concurredFinding = unitFinding.Finding

                        Select Case concurredFinding
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348, "unitILod", "Yes")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348, "unitRecommendFormal", "Yes")
                            Case Finding.Nlod_Due_To_Own_Misconduct
                                SetFormField(form348, "unitNotILodDom", "Yes")
                            Case Finding.Epts_Lod_Not_Applicable
                                SetFormField(form348, "unitEptsNotApplicable", "Yes")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348, "unitNotILodNotDom", "Yes")
                            Case Finding.Epts_Service_Aggravated
                                SetFormField(form348, "unitEptsServiceAggravated", "Yes")
                        End Select
                    End If

                    'add the Unit Command signature
                    AddSignatureToForm(form348, sig,
                                "unitDate", "unitSignature",
                                "unitName", "unitRank",
                                DBSignTemplateId.Form348Unit,
                                PersonnelTypes.UNIT_CMDR)

                End If 'unitFinding <> nothing

            End If

        End If
    End Sub

    Private Sub SetWingCommanderInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.AppointingAutorityReview)

        If (Not sig Is Nothing) Then

            Dim appointingfinding As LineOfDutyFindings
            appointingfinding = lod.FindByType(PersonnelTypes.APPOINT_AUTH)
            If appointingfinding IsNot Nothing Then

                If appointingfinding.Finding IsNot Nothing Then
                    If appointingfinding.Finding.HasValue Then
                        Select Case appointingfinding.Finding
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348, "appointingILod", "Yes")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348, "appointingRecommendFormal", "Yes")
                            Case Finding.Nlod_Due_To_Own_Misconduct
                                SetFormField(form348, "appointingNotILodDom", "Yes")
                            Case Finding.Epts_Lod_Not_Applicable
                                SetFormField(form348, "appointingEptsNotApplicable", "Yes")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348, "appointingNotILodNotDom", "Yes")
                            Case Finding.Epts_Service_Aggravated
                                SetFormField(form348, "appointingEptsServiceAggravated", "Yes")

                        End Select
                    End If

                    Dim nextWorkStatus As WorkStatus = wsdao.GetById(LodService.GetInitialNextStep(lod.Id, LodWorkStatus.AppointingAutorityReview))

                    If (nextWorkStatus.StatusCodeType.Id = LodStatusCode.BoardReview) Then
                        SetFormField(form348, "appointingForwardHQ", "Yes")
                    End If

                    AddSignatureToForm(form348, sig,
                                    "appointingDate", "appointingSignature",
                                    "appointingName", "appointingRank",
                                    DBSignTemplateId.WingCC,
                                    PersonnelTypes.APPOINT_AUTH)

                End If
            End If
        End If
    End Sub

    Private Sub SetWingJudgeAdvocateInfo(ByVal form348 As PDFForm, ByVal lod As LineOfDuty)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus.WingJAReview)

        'if not signed, we don't show anything
        If (Not sig Is Nothing) Then

            Dim jaFinding As LineOfDutyFindings
            jaFinding = lod.FindByType(PersonnelTypes.WING_JA)

            If jaFinding IsNot Nothing Then

                If jaFinding.DecisionYN = "Y" Then
                    SetFormField(form348, "judgeAdvocateConcur", "Yes")
                ElseIf jaFinding.DecisionYN = "N" Then
                    SetFormField(form348, "judgeAdvocateNonConcur", "Yes")
                    If jaFinding.Finding IsNot Nothing Then
                        If jaFinding.Finding.HasValue Then
                            concurredFinding = jaFinding.Finding
                        End If
                    End If
                End If

                If (jaFinding.DecisionYN = "N") Then
                    Select Case concurredFinding
                        Case Finding.In_Line_Of_Duty
                            SetFormField(form348, "judgeAdvocateILod", "Yes")
                        Case Finding.Recommend_Formal_Investigation
                            SetFormField(form348, "judgeAdvocateRecommendFormal", "Yes")
                        Case Finding.Nlod_Due_To_Own_Misconduct
                            SetFormField(form348, "judgeAdvocateNotILodDom", "Yes")
                        Case Finding.Epts_Lod_Not_Applicable
                            SetFormField(form348, "judgeAdvocateEptsNotApplicable", "Yes")
                        Case Finding.Nlod_Not_Due_To_OwnMisconduct
                            SetFormField(form348, "judgeAdvocateNotILodNotDom", "Yes")
                        Case Finding.Epts_Service_Aggravated
                            SetFormField(form348, "judgeAdvocateEptsServiceAggravated", "Yes")
                    End Select
                End If

                AddSignatureToForm(form348, sig,
                                    "judgeAdvocateDate", "judgeAdvocateSignature",
                                    "judgeAdvocateName", "judgeAdvocateRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.WING_JA)

            End If
        End If 'end (if signed)
    End Sub

End Class