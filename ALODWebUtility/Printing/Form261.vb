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

Public Class Form261
    Inherits System.Web.UI.Page
    Implements IDocumentCreate

    Protected Const EpochDate As Date = #1/29/2010#
    Private Const BRANCH_AFRC As String = "AFRC"
    Private Const BRANCH_ANG As String = "ANG"
    Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
    Private Const SIGNED_TEXT As String = "//SIGNED//"
    Private _sigDao As ISignatueMetaDateDao
    Private lodid As Integer
    Private remarksField As String = ""
    Private replaceIOSig As Boolean = False
    Private signatureService As DBSignService

    ReadOnly Property SigDao() As SignatureMetaDataDao
        Get
            If (_sigDao Is Nothing) Then
                _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
            End If

            Return _sigDao
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

    Public Function GeneratePDFForm(refId As Integer, replaceIOsig As Boolean) As PDFForm Implements IDocumentCreate.GeneratePDFForm

        Me.replaceIOSig = replaceIOsig
        Dim strComments As String = "Generated Form 261 PDF"
        Dim factory As New NHibernateDaoFactory()
        Dim dao As LineOfDutyDao = factory.GetLineOfDutyDao()
        Dim lod = New LineOfDuty()

        If (dao.GetWorkflow(refId) = 27) Then
            lod = New LineOfDuty_v2()
        End If

        lod = dao.GetById(refId)

        Dim wsdao As New WorkStatusDao()
        Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus = wsdao.GetById(lod.Status)

        Dim form261 As New PDFForm(PrintDocuments.FormDD261)

        SetFormField(form261, "lodCaseNumberP1", lod.CaseId)
        SetFormField(form261, "lodCaseNumberP2", lod.CaseId)

        If (lod.Formal AndAlso lod.LODInvestigation IsNot Nothing) Then
            If lod.LODInvestigation.ReportDate IsNot Nothing Then
                If lod.LODInvestigation.ReportDate.HasValue Then
                    SetFormField(form261, "REPORT_DATE", Server.HtmlDecode(lod.LODInvestigation.ReportDate.Value.ToString(DATE_FORMAT)))
                End If
            End If
            If lod.LODInvestigation.InvestigationOf IsNot Nothing Then
                Select Case lod.LODInvestigation.InvestigationOf
                    Case 4, 5
                        SetFormField(form261, "invest_aa", "Yes")
                    Case 1
                        SetFormField(form261, "invest_bb", "Yes")
                    Case 2
                        SetFormField(form261, "invest_cc", "Yes")
                    Case 3
                        SetFormField(form261, "invest_dd", "Yes")
                End Select
            End If
            If lod.LODInvestigation.Status IsNot Nothing Then
                Select Case lod.LODInvestigation.Status
                    Case 1
                        SetFormField(form261, "status_aa", "Yes")
                    Case 2
                        SetFormField(form261, "status_bb", "Yes")
                        SetFormField(form261, "status_b_aa", "Yes")
                    Case 3
                        SetFormField(form261, "status_bb", "Yes")
                        SetFormField(form261, "status_b_bb", "Yes")
                    Case 4
                        SetFormField(form261, "status_cc", "Yes")
                        SetFormField(form261, "status_ca", Server.HtmlDecode(lod.LODInvestigation.InactiveDutyTraining.Trim.ToString))
                    Case 5
                        SetFormField(form261, "status_dd", "Yes")
                End Select
            End If

            If lod.LODInvestigation.DurationStart IsNot Nothing Then
                If lod.LODInvestigation.DurationStart.HasValue Then
                    SetFormField(form261, "startD", Server.HtmlDecode(lod.LODInvestigation.DurationStart.Value.ToString(DATE_FORMAT)))
                    SetFormField(form261, "startH", Server.HtmlDecode(lod.LODInvestigation.DurationStart.Value.ToString(HOUR_FORMAT)))
                End If
            End If
            If lod.LODInvestigation.DurationEnd IsNot Nothing Then

                If lod.LODInvestigation.DurationEnd.HasValue Then
                    SetFormField(form261, "finishD", Server.HtmlDecode(lod.LODInvestigation.DurationEnd.Value.ToString(DATE_FORMAT)))
                    SetFormField(form261, "finishH", Server.HtmlDecode(lod.LODInvestigation.DurationEnd.Value.ToString(HOUR_FORMAT)))
                End If

            End If

            'TODO: Make sure this hardcoded value is nothing to be changed
            If lod.MemberCompo = "6" Then
                SetFormField(form261, "4_TO_Major_Army", "HQ AFRC/ACV")
            Else
                SetFormField(form261, "4_TO_Major_Army", "NGB/CV")
            End If

            SetFormField(form261, "individual_name", lod.MemberName.ToUpper())
            SetFormField(form261, "smLastFirstMiddle", lod.MemberName.ToUpper())
            SetFormField(form261, "individual_ssn", FormatSSN(lod.MemberSSN, False))
            SetFormField(form261, "smSSN", FormatSSN(lod.MemberSSN, False))
            SetFormField(form261, "individual_grade", lod.MemberGrade)
            SetFormField(form261, "smGrade", lod.MemberGrade)
            SetFormField(form261, "organization_station", lod.MemberUnit)

            If lod.LODInvestigation.OtherPersonnel IsNot Nothing Then
                If lod.LODInvestigation.OtherPersonnel.Count <> 0 Then
                    If (lod.LODInvestigation.OtherPersonnel(0) IsNot Nothing) Then
                        SetFormField(form261, "txtName_1", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel(0).Name))
                        SetFormField(form261, "txtGrade_1", lod.LODInvestigation.OtherPersonnel(0).Grade)

                        If lod.LODInvestigation.OtherPersonnel(0).InvestigationMade = True Then
                            SetFormField(form261, "lodMadeYes_1", "Yes")
                        Else
                            SetFormField(form261, "lodMadeYes_1", "No")
                        End If
                    End If
                    If lod.LODInvestigation.OtherPersonnel.Count > 1 Then
                        If (Not lod.LODInvestigation.OtherPersonnel(1) Is Nothing) Then
                            SetFormField(form261, "txtName_2", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel(1).Name))
                            SetFormField(form261, "txtGrade_2", lod.LODInvestigation.OtherPersonnel(1).Grade)

                            If lod.LODInvestigation.OtherPersonnel(1).InvestigationMade = True Then
                                SetFormField(form261, "lodMadeYes_2", "Yes")
                            Else
                                SetFormField(form261, "lodMadeYes_2", "No")
                            End If
                        End If
                    End If
                    If lod.LODInvestigation.OtherPersonnel.Count > 2 Then
                        If (Not lod.LODInvestigation.OtherPersonnel(2) Is Nothing) Then
                            SetFormField(form261, "txtName_3", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel(2).Name))
                            SetFormField(form261, "txtGrade_3", lod.LODInvestigation.OtherPersonnel(2).Grade)

                            If lod.LODInvestigation.OtherPersonnel(2).InvestigationMade = True Then
                                SetFormField(form261, "lodMadeYes_3", "Yes")
                            Else
                                SetFormField(form261, "lodMadeYes_3", "No")
                            End If
                        End If

                    End If

                End If

            End If

            'Basis for findings
            If lod.LODInvestigation.FindingsDate IsNot Nothing Then
                If lod.LODInvestigation.FindingsDate.HasValue Then
                    If (lod.LODInvestigation.FindingsDate.HasValue) Then
                        SetFormField(form261, "2DATE_YYMMDD", Server.HtmlDecode(lod.LODInvestigation.FindingsDate.Value.ToString(DATE_FORMAT)))
                        SetFormField(form261, "1HOUR", Server.HtmlDecode(lod.LODInvestigation.FindingsDate.Value.ToString(HOUR_FORMAT)))
                    End If
                End If
            End If
            SetFormField(form261, "3PLACE", Server.HtmlDecode(lod.LODInvestigation.Place))

            Dim ary As String() = SplitString(Server.HtmlDecode(lod.LODInvestigation.HowSustained), 75)

            SetFormField(form261, "4HOW_SUSTAINED1", ary(0))
            SetFormField(form261, "4HOW_SUSTAINED2", ary(1))

            ary = Nothing

            ary = SplitString(Server.HtmlDecode(lod.LODInvestigation.MedicalDiagnosis), 73)

            SetFormField(form261, "bMEDICAL_DIAGNOSIS1", ary(0))
            SetFormField(form261, "bMEDICAL_DIAGNOSIS2", ary(1))
            ary = Nothing

            If lod.LODInvestigation.PresentForDuty IsNot Nothing Then
                Select Case lod.LODInvestigation.PresentForDuty
                    Case True : SetFormField(form261, "Comb1a", "Yes")
                    Case False : SetFormField(form261, "Comb1b", "No")
                    Case Else
                End Select
            End If
            If lod.LODInvestigation.AbsentWithAuthority IsNot Nothing Then

                Select Case lod.LODInvestigation.AbsentWithAuthority
                    Case True : SetFormField(form261, "Comb2a", "Yes")
                    Case False : SetFormField(form261, "Comb2b", "No")
                    Case Else
                End Select
            End If
            If lod.LODInvestigation.IntentionalMisconduct IsNot Nothing Then
                Select Case lod.LODInvestigation.IntentionalMisconduct
                    Case True : SetFormField(form261, "Comb3a", "Yes")
                    Case False : SetFormField(form261, "Comb3b", "No")
                    Case Else
                End Select
            End If
            If lod.LODInvestigation.MentallySound IsNot Nothing Then

                Select Case lod.LODInvestigation.MentallySound
                    Case True : SetFormField(form261, "Comb4a", "Yes")
                    Case False : SetFormField(form261, "Comb4b", "No")
                    Case Else
                End Select
            End If

            If (Not String.IsNullOrEmpty(lod.LODInvestigation.Remarks)) Then
                ary = SplitString(Server.HtmlDecode(lod.LODInvestigation.Remarks), 80)

                If (ary IsNot Nothing) Then
                    If (ary.Count > 0) Then
                        SetFormField(form261, "g_REMARKS1", ary(0))
                    End If
                    If (ary.Count > 1) Then
                        SetFormField(form261, "g_REMARKS2", ary(1))
                    End If
                End If
                ary = Nothing
            End If

            Dim ioFinding As LineOfDutyFindings
            ioFinding = lod.FindByType(PersonnelTypes.IO)

            If ioFinding IsNot Nothing Then
                If ioFinding.Finding IsNot Nothing Then
                    Select Case (ioFinding.Finding)
                        Case 1 : SetFormField(form261, "findinga", "Yes") 'ILD
                        Case 5 : SetFormField(form261, "findingb", "Yes") 'NDOM
                        Case 4 : SetFormField(form261, "findingc", "Yes") 'DOM

                        Case Else
                    End Select
                End If
            End If

            If lod.LODInvestigation.IsSignedByIO Then

                AddFormalSignatureToForm_v2(form261, lod.LODInvestigation.SignatureInfoIO,
                lod.LODInvestigation.DateSignedIO,
                "", "fSIGNATURE", "aTYPED_NAME_Last_First_M",
                "bGRADE", "cBRANCH_OF_SERVICE", "eORGANIZATION_AND_STATIO",
                DBSignTemplateId.Form261, PersonnelTypes.IO)

            End If

            'Appointing Authority ---------------------------------------------------------------------

            Dim appointingFormalFinding As LineOfDutyFindings
            appointingFormalFinding = lod.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            If appointingFormalFinding IsNot Nothing Then

                AddFormalFinding_v2(form261, appointingFormalFinding, "appointingSubstitutedFindings", "appointingFindings")

                Select Case appointingFormalFinding.DecisionYN
                    Case "Y" : SetFormField(form261, "a_approve_yes_a", "Yes")
                    Case "N" : SetFormField(form261, "a_approve_no_a", "No")
                End Select

            End If

            If lod.LODInvestigation IsNot Nothing Then

                If (lod.LODInvestigation.IsSignedByAppointingAuthority) Then

                    AddFormalSignatureToForm_v2(form261, lod.LODInvestigation.SignatureInfoAppointing,
                    lod.LODInvestigation.DateSignedAppointing,
                    "a_date_a", "a_signature_a", "a_name_a",
                    "a_grade_a", "a_station_a", "a_headquarter_a",
                    DBSignTemplateId.Form348Findings, PersonnelTypes.FORMAL_APP_AUTH)

                End If

                If (lodCurrStatus.StatusCodeType.IsFinal) Then
                    'Final/Approving Authority ----------------------------------------------------------------------

                    Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.FormalApprovingAuthorityAction)

                    If (Not sig Is Nothing) Then

                        If Not lod.LODInvestigation.FinalApprovalFindings Is Nothing Then

                            Select Case lod.LODInvestigation.FinalApprovalFindings
                                Case Finding.In_Line_Of_Duty
                                    SetFormField(form261, "final_approval_findings", "In Line of Duty")
                                Case Finding.Epts_Service_Aggravated
                                    SetFormField(form261, "final_approval_findings", "EPTS-Service Aggravated")
                                Case Finding.Nlod_Due_To_Own_Misconduct
                                    SetFormField(form261, "final_approval_findings", "Not ILOD-Due to Own Misconduct")
                                Case Finding.Epts_Lod_Not_Applicable
                                    SetFormField(form261, "final_approval_findings", "EPTS-LOD Not Applicable")
                                Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                    SetFormField(form261, "final_approval_findings", "Not ILOD-Not Due to Own Misconduct")
                                Case Finding.Recommend_Formal_Investigation
                                    SetFormField(form261, "final_approval_findings", "Formal Investigation")
                            End Select

                            AddSignatureInformationToForm(lodid, form261, sig,
                                        "approval_date", "finalapprovalsig",
                                        "finalNameRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.FORMAL_BOARD_AA)

                        End If
                    End If
                    'need to determine how much of the endless findings field will go here
                    Dim approvingFormalFinding As LineOfDutyFindings
                    approvingFormalFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_AA)
                    If Not approvingFormalFinding Is Nothing Then
                        AddFormalFinding_v2(form261, approvingFormalFinding, "approvingSubstitutedFindings", "approvingFindings")
                    End If

                End If

            End If

            If (lod.CurrentStatusCode <> LodStatusCode.Complete) Then
                'Suppress the page
                form261.SuppressSecondPage()
            End If

        End If

        LogManager.LogAction(ModuleType.LOD, UserAction.ViewDocument, lodid, strComments)

        Return form261
    End Function

    Protected Function AddFormalFinding_v2(ByVal doc As PDFForm, ByVal boardFinding As LineOfDutyFindings, ByVal concurField As String, ByVal findingField As String) As Boolean
        If (boardFinding Is Nothing) Then
            SetFormField(doc, concurField, String.Empty)
            SetFormField(doc, findingField, String.Empty)
            Return False
        End If

        Dim concurText As String = ""
        Dim newFinding As String = ""
        Dim valid As Boolean = False

        If (boardFinding.DecisionYN = "Y") Then
            concurText = ""
            valid = True
        Else
            concurText = "Substituted Findings: "

            If (boardFinding.Finding.HasValue) Then

                valid = True

                newFinding = GetFindingFormText(boardFinding.Finding.Value)
            End If
        End If

        SetFormField(doc, concurField, concurText + newFinding)
        SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText))

        Return valid

    End Function

    Protected Function AddFormalSignatureToForm_v2(ByVal doc As PDFForm, ByVal signature As PersonnelData, ByVal dateSigned As Date?, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal rankField As String, ByVal branchField As String, ByVal unitField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean
        If (signature Is Nothing) Then
            SetFormField(doc, dateField, String.Empty)
            SetFormField(doc, nameField, String.Empty)
            SetFormField(doc, sigField, String.Empty)
            SetFormField(doc, rankField, String.Empty)
            SetFormField(doc, branchField, String.Empty)
            SetFormField(doc, unitField, String.Empty)
            Return False
        End If

        SetFormField(doc, nameField, signature.Name.ToUpper())
        SetFormField(doc, rankField, signature.Grade)

        SetFormField(doc, unitField, signature.PasCodeDescription)

        If (Not String.IsNullOrEmpty(signature.Branch)) Then
            SetFormField(doc, branchField, signature.Branch)
        Else
            If SESSION_COMPO = "6" Then
                SetFormField(doc, branchField, BRANCH_AFRC)
            Else
                SetFormField(doc, branchField, BRANCH_ANG)
            End If

        End If

        If (dateSigned Is Nothing) OrElse (Not dateSigned.HasValue) Then
            'no signature date, so don't add the signature, meaning we're done
            Return False
        End If

        If (dateSigned.Value < EpochDate) Then

            'use the old style signature
            SetFormField(doc, sigField, SIGNED_TEXT)

            'use the passed in date
            SetFormField(doc, dateField, dateSigned.Value.ToString(DATE_FORMAT))

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
            ' Check if the electronic signature should be used for the IO signature field
            If (ptype = PersonnelTypes.IO AndAlso (replaceIOSig = True Or dateSigned.Value < ALODWebUtility.Common.Utility.ARCHIVE_DATE)) Then
                'use the old style signature
                SetFormField(doc, sigField, SIGNED_TEXT)

                'use the passed in date
                SetFormField(doc, dateField, dateSigned.Value.ToString(DATE_FORMAT))

                valid = True
                replaceIOSig = False ' Reset flag
            Else
                'otherwise, clear those fields
                SetFormField(doc, sigField, SIGNED_TEXT)
                'SetFormField(doc, dateField, signature.DateSigned.ToString())
                valid = False
            End If
        End If

        Return valid

    End Function

    Protected Function AddSignatureInformationToForm(ByVal refId As Integer, ByVal doc As PDFForm, ByVal signature As SignatureMetaData, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean
        If (signature Is Nothing) Then
            SetFormField(doc, dateField, String.Empty)
            SetFormField(doc, nameField, String.Empty)
            SetFormField(doc, sigField, String.Empty)
            Return False
        End If

        Dim strSig As String = "USAF"
        If SESSION_COMPO = "5" Then
            strSig = "ANG"
        End If

        SetFormField(doc, nameField, signature.NameAndRank + ", " + strSig)

        Dim dateSigned As Date = signature.date

        VerifySource = New DBSignService(template, refId, ptype)

        Dim valid As Boolean = False

        Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()

        If (signatureStatus = DBSignResult.SignatureValid) Then
            Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()

            Dim sigLine As String = "Digitally signed by " _
                + signInfo.Signature + Environment.NewLine _
                + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT)

            SetFormField(doc, sigField, sigLine)
            SetFormField(doc, dateField, signInfo.DateSigned.ToString("ddMMMyyyy"))
            valid = True
        Else
            'Use electronic signature
            SetFormField(doc, sigField, SIGNED_TEXT)
            SetFormField(doc, dateField, signature.date.ToString("ddMMMyyyy"))
            valid = False
        End If

        Return valid
    End Function

End Class