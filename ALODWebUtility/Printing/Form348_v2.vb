Imports System.Configuration
Imports System.Drawing
Imports System.Text
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
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

Public Class Form348_v2
    Inherits System.Web.UI.Page
    Implements IDocumentCreate

    Protected Const EpochDate As Date = #1/29/2010#
    Private Const BOARD_LEGAL_FINDINGS As Short = 15
    Private Const BOARD_MED_FINDINGS As Short = 16
    Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
    Private Const SIGNED_TEXT As String = "//SIGNED//"
    Private _sigDao As ISignatueMetaDateDao
    Private _wsdao As WorkStatusDao
    Private concurredFinding As Short
    Private lodid As Integer
    Private remarksField As String = ""
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
        Dim msgtest As New StringBuilder()
        Dim lod As LineOfDuty_v2 = LodService.GetById(refId)

        If (IsDBNull(refId)) Then
            msgtest.Append("Form348 Page: Creation of lod \ refId IsDBNull \n")
        ElseIf (refId.ToString() Is Nothing) Then
            msgtest.Append("Form348 Page: Creation of lod \ refId is nothing \n")
        Else
            If (IsDBNull(LodService.GetById(refId))) Then
                msgtest.Append("Form348 Page: Creation of lod \ refId = " + refId.ToString() + " \ LodService.GetById(refId) IsDBNull \ ")
            ElseIf (LodService.GetById(refId) Is Nothing) Then
                msgtest.Append("Form348 Page: Creation of lod \ refId = " + refId.ToString() + " \ LodService.GetById(refId) is nothing \ ")
            ElseIf (LodService.GetById(refId) IsNot Nothing) Then
                msgtest.Append("Form348 Page: Creation of lod \ refId = " + refId.ToString() + " \ LodService.GetById(refId) has Data \ LODID= " + lod.Id.ToString() + "\ ")
            End If
        End If

        Dim isRLod As Boolean = False
        Dim lkupDAO As ILookupDao
        lkupDAO = New NHibernateDaoFactory().GetLookupDao()

        lodid = lod.Id

        If (lod.SarcCase AndAlso Not UserHasPermission(PERMISSION_VIEW_SARC_CASES)) Then
            If (lod.IsRestricted OrElse (Not lod.IsRestricted AndAlso Not UserHasPermission("SARCUnrestricted"))) Then
                Dim msg As New StringBuilder()
                msg.Append("Access Denied" + System.Environment.NewLine)
                msg.Append("UserID: " + lod.Id.ToString + System.Environment.NewLine)
                msg.Append("Request: " + Request.Url.ToString() + System.Environment.NewLine)

                If (Request.UrlReferrer IsNot Nothing) Then
                    msg.Append("Referrer: " + Request.UrlReferrer.ToString() + System.Environment.NewLine)
                End If

                msg.Append("Reason: User is attempting to view SARC PDF without permission")

                LogManager.LogError(msg.ToString())
                Response.Redirect(ConfigurationManager.AppSettings("AccessDeniedUrl"))

                '  Throw New Exception("User is attempting to view SARC PDF without permission refId:" + lod.Id.ToString)
            End If
        End If

        Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus = wsdao.GetById(lod.Status)

        Dim form348_v2 As New PDFForm(PrintDocuments.FormARFC348_v2)

        SetFormField(form348_v2, "lodCaseNumberP1", lod.CaseId)
        SetFormField(form348_v2, "lodCaseNumberP2", lod.CaseId)
        SetFormField(form348_v2, "lodCaseNumberP3", lod.CaseId)

        SetFormField(form348_v2, "part1ToCC", lod.ToUnit)
        SetFormField(form348_v2, "part1From", lod.FromUnit)
        SetFormField(form348_v2, "part1NameFill", lod.MemberName)
        SetFormField(form348_v2, "part1SSNFill", FormatSSN(lod.MemberSSN))
        SetFormField(form348_v2, "part1Rank", lod.GetMemberRankAndGradeForForm(lkupDAO))
        SetFormField(form348_v2, "part1Organization", lod.MemberUnit)
        SetFormField(form348_v2, "part1ReportDate", lod.CreatedDate.ToString("ddMMMyyyy"))

        SetMedicalInfo(form348_v2, lod)

        SetUnitInfo(form348_v2, lod)

        SetWingJudgeAdvocateInfo(form348_v2, lod)

        SetWingCommanderInfo(form348_v2, lod)

        '*************************************
        'The board section only gets added
        'to the 348 for informal cases
        '*************************************

        '*************************************
        'If form 348 is not "Complete", we suppress the whole second page, Print-Out/PDF
        '*************************************
        If (lod.CurrentStatusCode = LodStatusCode.Complete) Then

            If (lod.Formal) Then
                'this is a formal case, so add the 261 text
                'SetFormField(form348_v2, "part6MedicalReview", "(See DD Form 261 for Formal investigation )")
                'Board Medical
                SetFormField(form348_v2, "part6MedicalReview", lod.FindByType(BOARD_MED_FINDINGS).FindingsText)
                'Board Legal
                SetFormField(form348_v2, "part6LegalReview", lod.FindByType(BOARD_LEGAL_FINDINGS).FindingsText)
            Else

                SetBoardMedicalInfo(form348_v2, lod)

                SetBoardLegalInfo(form348_v2, lod)

                SetBoardAdminInfo(form348_v2, lod)

                If (lodCurrStatus.StatusCodeType.IsFinal) Then

                    If (lod.BoardForGeneral = "Y") Then
                        SetBoardTechInfo(form348_v2, lod)
                    Else
                        SetBoardApprovalInfo(form348_v2, lod)
                    End If

                End If
            End If
        End If

        LogManager.LogAction(ModuleType.LOD, UserAction.ViewDocument, lodid, strComments)

        form348_v2.SuppressInstructionPages()

        Return form348_v2
    End Function

    Protected Function AddBoardFinding_v2(ByVal doc As PDFForm, ByVal boardFinding As LineOfDutyFindings, ByVal findingField As String) As Boolean
        If (boardFinding Is Nothing) Then
            SetFormField(doc, findingField, String.Empty)
            Return False
        End If

        Dim concurText As String = ""
        Dim newFinding As String = ""

        If (boardFinding.DecisionYN = "Y") Then
            concurText = "Concur with Appointing Authority. "

            If (findingField = "part6MedicalReview") Then
                Dim message As String = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their pathophysiology and prognosis, as related to causation, the following determination was found in this case:"

                'SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding)
                PossibleRemarks(doc, findingField, message + Environment.NewLine + concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                'SetFormField(doc, findingField, message + Environment.NewLine + concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                Return True
            ElseIf (findingField = "part6LegalReview") Then
                SetFormField(doc, findingField, boardFinding.FindingsText)
                Return True
            Else
                'SetFormField(doc, concurField, concurText + newFinding)
                PossibleRemarks(doc, findingField, concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                'SetFormField(doc, findingField, concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                Return True
            End If
        Else
            concurText = "Non Concur with Appointing Authority. Recommended new finding: "

            If (boardFinding.Finding.HasValue) Then
                newFinding = GetFindingFormText(boardFinding.Finding.Value)

                If (findingField = "part6MedicalReview") Then
                    Dim message As String = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their pathophysiology and prognosis, as related to causation, the following determination was found in this case: "

                    'SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding)
                    PossibleRemarks(doc, findingField, message + Environment.NewLine + concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                    'SetFormField(doc, findingField, message + Environment.NewLine + concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                    Return True
                Else
                    'SetFormField(doc, concurField, concurText + newFinding)
                    PossibleRemarks(doc, findingField, concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                    'SetFormField(doc, findingField, concurText + newFinding + Environment.NewLine + Server.HtmlDecode(boardFinding.FindingsText))
                    Return True
                End If
            Else
                Return False
            End If
        End If

        Return False

    End Function

    Protected Function AddSignatureToForm_v2(ByVal doc As PDFForm, ByVal signature As SignatureMetaData, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean
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

        VerifySource = New DBSignService(template, lodid, ptype)

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

    Protected Function RemoveNewLinesFromString(ByVal data As String) As String
        If (String.IsNullOrEmpty(data)) Then
            Return String.Empty
        End If

        Dim replacedString As String = data
        Dim replaceFinished As Boolean = False
        Dim len As Integer

        While (Not replaceFinished)
            len = replacedString.Length()
            replacedString = replacedString.Replace(Environment.NewLine, " ")

            If (len = replacedString.Length()) Then
                replaceFinished = True
            End If
        End While

        Return replacedString
    End Function

    Private Function GetWatermarkSignature(ByVal signature As SignatureEntry, ByVal template As DBSignTemplateId, ByVal refId As Integer, ByVal ptype As PersonnelTypes) As String
        If (Not IsValidSignature(signature)) Then
            Return String.Empty
        End If

        Dim sigLine As String = String.Empty
        Dim dateSigned As Date = signature.DateSigned.Value

        ' Check if the old style signature is needed...
        If (dateSigned < EpochDate) Then
            sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(DATE_FORMAT)

            Return sigLine
        End If

        ' This signature occured after the epoch, so verify it
        VerifySource = New DBSignService(template, refId, ptype)

        Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()

        ' Check if valid signature...
        If (signatureStatus = DBSignResult.SignatureValid) Then
            Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()

            If (signInfo Is Nothing) Then
                Return String.Empty
            End If

            sigLine = "Digitally signed by " _
                    + signInfo.Signature + "<BR>" _
                    + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT)

            Return sigLine
        Else
            sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(DATE_FORMAT)

            Return sigLine
        End If
    End Function

    Private Sub PossibleRemarks(ByVal doc As PDFForm, ByVal formField As String, ByVal data As String)
        If (String.IsNullOrEmpty(data)) Then
            Exit Sub
        End If

        Dim remarksTitle As String = ""
        Dim maxCountNewLine As Integer = 5

        Dim currCountNewLine As Integer = 1
        Dim currCount As Integer = 0

        Dim textbox1 As System.Windows.Forms.TextBox = New System.Windows.Forms.TextBox()
        textbox1.Multiline = True
        textbox1.WordWrap = True
        'one pixel every 1/96 in.

        textbox1.Width = 715
        textbox1.Height = 77
        textbox1.Font = New Font("Times New Roman", 9)

        data = RemoveNewLinesFromString(data)

        'get the number of approved new lines to be used in the formfield
        If (formField.Equals("part2Description")) Then
            maxCountNewLine = 4
            textbox1.Width = 715
            textbox1.Height = 77
            textbox1.Text = data

            remarksTitle = "Descriptions of Symptoms and Diagnosis (cont'd): "
        ElseIf (formField.Equals("part2Details")) Then
            maxCountNewLine = 4
            textbox1.Width = 715
            textbox1.Height = 77
            textbox1.Text = data

            remarksTitle = "Details Of Death, Injury, Illness Or History of Disease (cont'd): "
        ElseIf (formField.Equals("part2Check13eOther")) Then
            maxCountNewLine = 2
            textbox1.Width = 570
            textbox1.Height = 39
            textbox1.Text = data

            remarksTitle = "Other Relevant Condition(s) (cont'd): "
        ElseIf (formField.Equals("part3InvestigationResult")) Then
            maxCountNewLine = 9
            textbox1.Width = 715
            textbox1.Height = 153
            textbox1.Text = data

            remarksTitle = "Result of Investigation (cont'd): "
        ElseIf (formField.Equals("part6MedicalReview")) Then
            maxCountNewLine = 2
            textbox1.Width = 715
            textbox1.Height = 43
            textbox1.Text = data

            remarksTitle = "Medical Review/Recommendation (cont'd): "
        ElseIf (formField.Equals("part6LegalReview")) Then
            maxCountNewLine = 2
            textbox1.Width = 715
            textbox1.Height = 43
            textbox1.Text = data

            remarksTitle = "Legal Review/Recommendation (cont'd): "
        End If

        currCount = textbox1.GetFirstCharIndexFromLine(maxCountNewLine)

        If (Not currCount = -1) Then
            SetFormField(doc, formField, data.Substring(0, currCount))

            remarksField = remarksField + Environment.NewLine + remarksTitle + data.Substring(currCount, (data.Length() - currCount))

            SetFormField(doc, "part8Remarks", remarksField)
        Else
            SetFormField(doc, formField, data)
        End If

    End Sub

    Private Sub SetBoardAdminInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.BoardPersonnelReview)

        If (Not sig Is Nothing) Then
            Dim boardAdminFindings As LineOfDutyFindings
            boardAdminFindings = lod.FindByType(PersonnelTypes.BOARD_A1)

            If (boardAdminFindings IsNot Nothing) Then
                If (boardAdminFindings.DecisionYN = "Y") Then
                    Dim appointingfinding As LineOfDutyFindings
                    appointingfinding = lod.FindByType(PersonnelTypes.APPOINT_AUTH)

                    If (appointingfinding.Finding IsNot Nothing AndAlso appointingfinding.Finding.HasValue) Then
                        Select Case appointingfinding.Finding.Value
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348_v2, "part6Check32ILOD", "1")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348_v2, "part6Check32FLOD", "1")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348_v2, "part6Check32NILOD", "1")
                        End Select
                    End If
                ElseIf (boardAdminFindings.Finding.HasValue) Then
                    Select Case boardAdminFindings.Finding.Value
                        Case Finding.In_Line_Of_Duty
                            SetFormField(form348_v2, "part6Check32ILOD", "1")
                        Case Finding.Recommend_Formal_Investigation
                            SetFormField(form348_v2, "part6Check32FLOD", "1")
                        Case Finding.Nlod_Not_Due_To_OwnMisconduct
                            SetFormField(form348_v2, "part6Check32NILOD", "1")
                    End Select
                End If

                AddSignatureToForm_v2(form348_v2, sig,
                                        "part6LODDate", "LODSignature33",
                                        "part6LODNameRank",
                                         DBSignTemplateId.Form348Findings,
                                         PersonnelTypes.BOARD_A1)

                If (boardAdminFindings.ReferDES.HasValue AndAlso boardAdminFindings.ReferDES.Value) Then
                    SetFormField(form348_v2, "part6Check32REFER", "1")
                End If
            End If
        End If
    End Sub

    Private Sub SetBoardApprovalInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)
        Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus = wsdao.GetById(lod.Status)

        If (lodCurrStatus.StatusCodeType.IsFinal) Then

            Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.ApprovingAuthorityAction)

            If (Not sig Is Nothing) Then
                Dim approvingfinding As LineOfDutyFindings

                approvingfinding = lod.FindByType(PersonnelTypes.BOARD_AA)

                If (approvingfinding IsNot Nothing) Then
                    If (approvingfinding.Finding IsNot Nothing) Then
                        Select Case approvingfinding.Finding
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348_v2, "part7Check34ILOD", "1")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348_v2, "part7Check34FLOD", "1")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348_v2, "part7Check34NILOD", "1")
                        End Select

                        If (approvingfinding.ReferDES.HasValue AndAlso approvingfinding.ReferDES.Value) Then
                            SetFormField(form348_v2, "part7Check34REFER", "1")
                        End If
                    End If

                    Dim approver As String = ""

                    'approval authority is slightly different

                    'we need to know if the board signed for the general
                    If (lod.BoardForGeneral = "Y") Then
                        'add the board members name to the signature

                        Dim Techsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.BoardReview)

                        If (Not Techsig Is Nothing) Then
                            approver += sig.NameAndRank + " for "
                        End If
                    Else

                        'if the General signed it, add their signature
                        AddSignatureToForm_v2(form348_v2, sig,
                                    "part7ApprovingDate", "ApprovingSignature35",
                                    "part7ApprovingNameRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.BOARD_AA)
                    End If

                    approver += sig.NameAndRank _
                                        + vbCrLf + sig.Title

                    SetFormField(form348_v2, "part7ApprovingNameRank", approver)
                End If
            End If
        End If
    End Sub

    Private Sub SetBoardLegalInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.BoardLegalReview)

        If (Not sig Is Nothing) Then

            Dim legalfinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.BOARD_JA)
            If (AddBoardFinding_v2(form348_v2, legalfinding, "part6LegalReview")) Then

                AddSignatureToForm_v2(form348_v2, sig,
                                    "part6LegalReviewDate", "LegalSignature31",
                                    "part6LegalReviewNameRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.BOARD_JA)

            End If

        End If
    End Sub

    Private Sub SetBoardMedicalInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.BoardMedicalReview)

        If (Not sig Is Nothing) Then

            Dim medicalfinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.BOARD_SG)
            If (AddBoardFinding_v2(form348_v2, medicalfinding, "part6MedicalReview")) Then

                If (Not AddSignatureToForm_v2(form348_v2, sig,
                                    "part6MedicalDate", "MedicalSignature29",
                                    "part6MedicalNameRank",
                                    DBSignTemplateId.Form348Findings,
                                    PersonnelTypes.BOARD_SG)) Then

                    'we failed to add a signature
                    'either it wasn't signed or the signature is not valid
                    'either way, clear the findings fields

                    'ClearFormField(form348_v2, "medicalSubstitutedFindings")
                    'ClearFormField(form348_v2, "part6MedicalReview")
                    'ClearFormField(form348_v2, "part6MedicalDate")
                    'ClearFormField(form348_v2, "MedicalSignature29")
                    'ClearFormField(form348_v2, "part6MedicalNameRank")

                End If
            End If

        End If
    End Sub

    Private Sub SetBoardTechInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)
        If (lod.FinalFindings IsNot Nothing AndAlso lod.FinalFindings.HasValue) Then

            Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.BoardReview)

            If (Not sig Is Nothing) Then
                'if the board signed for the general, add the board signature for the AA as well
                If (lod.BoardForGeneral = "Y") Then
                    Dim approvingfinding As LineOfDutyFindings

                    approvingfinding = lod.FindByType(PersonnelTypes.BOARD)

                    If (approvingfinding IsNot Nothing) Then

                        If approvingfinding.Finding IsNot Nothing Then
                            Select Case approvingfinding.Finding
                                Case Finding.In_Line_Of_Duty
                                    SetFormField(form348_v2, "part7Check34ILOD", "1")
                                Case Finding.Recommend_Formal_Investigation
                                    SetFormField(form348_v2, "part7Check34FLOD", "1")
                                Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                    SetFormField(form348_v2, "part7Check34NILOD", "1")
                            End Select

                            AddSignatureToForm_v2(form348_v2, sig,
                                        "part7ApprovingDate", "ApprovingSignature35",
                                        "part7ApprovingNameRank",
                                        DBSignTemplateId.Form348Findings,
                                        PersonnelTypes.BOARD)

                            Dim approver As String = ""

                            approver += sig.NameAndRank + " for "

                            Dim Appsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.ApprovingAuthorityAction)

                            ' Check if the Approval Authority actually signed the case...
                            If (Not Appsig Is Nothing) Then
                                ' Use the approval authority signature information stored in the case record...
                                approver += Appsig.NameAndRank + Appsig.Title
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
                                        approver += approvalAuthority.AlternateSignatureName + ", " + strSig + ", " + title
                                    Else
                                        approver += approvalAuthority.SignatureName + ", " + strSig + ", " + title
                                    End If
                                Else
                                    approver += "UNKNOWN Approving Authority"
                                End If
                            End If

                            SetFormField(form348_v2, "part7ApprovingNameRank", approver)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub SetMedicalInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)
        '*********************************************
        'Medical Officer / Medical Technician
        '*********************************************

        ''*************************************************************************************************ForTesting88******************************************************************************************
        'Dim msg As New StringBuilder()
        'If (IsDBNull(lod.Id)) Then
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alertMessage('refId is DBNull');", True)
        '    msg.Append("Form348 Page: refId IsDBNull")
        'ElseIf (lod.Id.ToString() Is Nothing) Then
        '    ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alertMessage('refId is nothing');", True)
        '    msg.Append("Form348 Page: refId is nothing")
        'Else

        '    If (IsDBNull(lod.LODUnit_v2)) Then
        '        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alertMessage('refId is " + lod.Id.ToString() + "LodService.GetById(refId) is DBNull');", True)
        '        msg.Append("Form348 Page: refId= " + lod.Id.ToString() + "\ lod.LODUnit_v2 ISDBNull ")
        '    ElseIf (lod.LODUnit_v2 Is Nothing) Then
        '        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alertMessage('refId is " + lod.Id.ToString() + "LodService.GetById(refId) is Nothing');", True)
        '        msg.Append("Form348 Page: refId= " + lod.Id.ToString() + "\ lod.LODUnit_v2 is nothing ")
        '    ElseIf (lod.LODUnit_v2 IsNot Nothing) Then
        '        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Script", "alertMessage('refId is " + lod.Id.ToString() + "LodService.GetById(refId) is Something');", True)
        '        msg.Append("Form348 Page: refId= " + lod.Id.ToString() + "\ lod.LODUnit_v2 has data \ lod.LODUnit_v2.DutyFrom = " + lod.LODUnit_v2.DutyFrom)
        '    End If
        'End If

        ''*************************************************************************************************End***************************************************************************************************

        If (lod.LODMedical_v2 IsNot Nothing) Then

            If (lod.LODMedical_v2.MemberFrom IsNot Nothing) Then
                If (lod.LODMedical_v2.MemberFrom = FromLocation.MTF) Then
                    SetFormField(form348_v2, "part1check2MTF", "1")
                ElseIf (lod.LODMedical_v2.MemberFrom = FromLocation.RMU) Then
                    SetFormField(form348_v2, "part1check2RMU", "1")
                ElseIf (lod.LODMedical_v2.MemberFrom = FromLocation.GMU) Then
                    SetFormField(form348_v2, "part1check2GMU", "1")
                ElseIf (lod.LODMedical_v2.MemberFrom = FromLocation.DeployedLocation) Then
                    SetFormField(form348_v2, "part1check2DepLoc", "1")
                End If
            End If

            If (lod.LODMedical_v2.MemberComponent IsNot Nothing) Then
                If (lod.LODMedical_v2.MemberComponent = MemberComponent.AFR) Then
                    SetFormField(form348_v2, "part1check8AFR", "1")
                ElseIf (lod.LODMedical_v2.MemberComponent = MemberComponent.RegAF) Then
                    SetFormField(form348_v2, "part1check8RegAF", "1")
                ElseIf (lod.LODMedical_v2.MemberComponent = MemberComponent.ANG) Then
                    SetFormField(form348_v2, "part1check8ANG", "1")
                ElseIf (lod.LODMedical_v2.MemberComponent = MemberComponent.USAFACadet) Then
                    SetFormField(form348_v2, "part1check8USAFA", "1")
                ElseIf (lod.LODMedical_v2.MemberComponent = MemberComponent.AFROTCCadet) Then
                    SetFormField(form348_v2, "part1check8AFROTC", "1")
                End If

            End If

            'LogManager.LogError(msg.ToString())
            My.Application.Log.WriteEntry("Test")

            If (Not IsDBNull(lod.LODUnit_v2.DutyFrom)) Then
                If (lod.LODUnit_v2.DutyFrom IsNot Nothing) Then
                    If (lod.LODUnit_v2.DutyFrom.HasValue) Then
                        SetFormField(form348_v2, "part1MbrStartDate", Server.HtmlDecode(lod.LODUnit_v2.DutyFrom.Value.ToString("ddMMMyyyy")))
                        SetFormField(form348_v2, "part1MbrStartTime", Server.HtmlDecode(lod.LODUnit_v2.DutyFrom.Value.ToString(HOUR_FORMAT)))
                    End If
                End If
            End If

            If (Not IsDBNull(lod.LODUnit_v2.DutyTo)) Then
                If (lod.LODUnit_v2.DutyTo IsNot Nothing) Then
                    If (lod.LODUnit_v2.DutyTo.HasValue) Then
                        SetFormField(form348_v2, "part1MbrEndDate", Server.HtmlDecode(lod.LODUnit_v2.DutyTo.Value.ToString("ddMMMyyyy")))
                        SetFormField(form348_v2, "part1MbrEndTime", Server.HtmlDecode(lod.LODUnit_v2.DutyTo.Value.ToString(HOUR_FORMAT)))
                    End If
                End If
            End If

            If (lod.LODMedical_v2.NatureOfEvent IsNot Nothing) Then
                If (lod.LODMedical_v2.NatureOfEvent.Equals("Death")) Then
                    SetFormField(form348_v2, "part2check9Death", "1")
                ElseIf (lod.LODMedical_v2.NatureOfEvent.Equals("Injury") OrElse lod.LODMedical_v2.NatureOfEvent.Equals("Injury-MVA")) Then
                    SetFormField(form348_v2, "part2check9Injury", "1")
                ElseIf (lod.LODMedical_v2.NatureOfEvent.Equals("Illness")) Then
                    SetFormField(form348_v2, "part2check9Illness", "1")
                ElseIf (lod.LODMedical_v2.NatureOfEvent.Equals("Disease")) Then
                    SetFormField(form348_v2, "part2check9Disease", "1")
                End If

            End If

            If (lod.LODMedical_v2.MedicalFacilityType IsNot Nothing) Then
                If (lod.LODMedical_v2.MedicalFacilityType.Equals("Military")) Then
                    SetFormField(form348_v2, "part2check10MilFacility", "1")
                ElseIf (lod.LODMedical_v2.MedicalFacilityType.Equals("Civilian")) Then
                    SetFormField(form348_v2, "part2check10CivFacility", "1")
                End If
            End If

            SetFormField(form348_v2, "part2FacilityName", Server.HtmlDecode(lod.LODMedical_v2.MedicalFacility))

            If (lod.LODMedical_v2.TreatmentDate IsNot Nothing) Then
                If (lod.LODMedical_v2.TreatmentDate.HasValue) Then
                    SetFormField(form348_v2, "part2check10Date", Server.HtmlDecode(lod.LODMedical_v2.TreatmentDate.Value.ToString("ddMMMyyyy")))
                    SetFormField(form348_v2, "part2check10Time", Server.HtmlDecode(lod.LODMedical_v2.TreatmentDate.Value.ToString(HOUR_FORMAT)))
                End If
            End If

            ' Start our diagnosis wit the nature of incident
            Dim natureOfIncident As String = lod.LODMedical.NatureOfIncidentDescription

            ' Add EPTS
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

            ' Add ICD9 info
            If (lod.LODMedical.ICD9Id IsNot Nothing) AndAlso (lod.LODMedical.ICD9Id.HasValue) Then
                Dim code As ICD9Code = Nothing
                code = LookupService.GetIcd9CodeById(lod.LODMedical.ICD9Id.Value)

                If (code IsNot Nothing) Then
                    natureOfIncident += "    " + code.GetFullCode(lod.LODMedical.ICD7thCharacter) + " - " + code.Description
                End If
            End If

            PossibleRemarks(form348_v2, "part2Description", natureOfIncident & ":  " & Server.HtmlDecode(lod.LODMedical_v2.DiagnosisText))
            PossibleRemarks(form348_v2, "part2Details", Server.HtmlDecode(lod.LODMedical_v2.EventDetails))

            If (lod.LODMedical_v2.MemberCondition IsNot Nothing) Then
                If (lod.LODMedical_v2.MemberCondition.Equals("was")) Then
                    SetFormField(form348_v2, "part2Check13aWas", "1")
                ElseIf (lod.LODMedical_v2.MemberCondition.Equals("was not")) Then
                    SetFormField(form348_v2, "part2Check13aWasNot", "1")
                End If
            End If

            If (lod.LODMedical_v2.Influence IsNot Nothing) Then
                If (lod.LODMedical_v2.Influence = MemberInfluence.Alcohol) Then
                    SetFormField(form348_v2, "part2Check13aAlcohol", "1")

                    If (lod.LODMedical_v2.AlcoholTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bAlcohol", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    ElseIf (lod.LODMedical_v2.AlcoholTestDone.Equals("No")) Then
                        SetFormField(form348_v2, "part2Check13bAlcohol", "1")
                        SetFormField(form348_v2, "part2Check13bNo", "1")
                    ElseIf (lod.LODMedical_v2.DrugTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bDrug", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    End If

                ElseIf (lod.LODMedical_v2.Influence = MemberInfluence.Drugs) Then
                    SetFormField(form348_v2, "part2Check13aDrug", "1")

                    If (lod.LODMedical_v2.DrugTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bDrug", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    ElseIf (lod.LODMedical_v2.DrugTestDone.Equals("No")) Then
                        SetFormField(form348_v2, "part2Check13bDrug", "1")
                        SetFormField(form348_v2, "part2Check13bNo", "1")
                    ElseIf (lod.LODMedical_v2.AlcoholTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bAlcohol", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    End If

                ElseIf (lod.LODMedical_v2.Influence = MemberInfluence.AlcoholDrugs) Then
                    SetFormField(form348_v2, "part2Check13aAlcohol", "1")
                    SetFormField(form348_v2, "part2Check13aDrug", "1")

                    If (lod.LODMedical_v2.DrugTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bDrug", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    End If

                    If (lod.LODMedical_v2.AlcoholTestDone.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check13bAlcohol", "1")
                        SetFormField(form348_v2, "part2Check13bYes", "1")
                        SetFormField(form348_v2, "part2Check13bResults", "See attachments")
                    End If

                    If (lod.LODMedical_v2.AlcoholTestDone.Equals("No") AndAlso lod.LODMedical_v2.DrugTestDone.Equals("No")) Then
                        SetFormField(form348_v2, "part2Check13bDrug", "1")
                        SetFormField(form348_v2, "part2Check13bAlcohol", "1")
                        SetFormField(form348_v2, "part2Check13bNo", "1")
                    End If
                End If
            End If

            If (lod.LODMedical_v2.MemberResponsible IsNot Nothing) Then
                If (lod.LODMedical_v2.MemberResponsible.Equals("Yes")) Then
                    SetFormField(form348_v2, "part2Check13cWas", "1")
                ElseIf (lod.LODMedical_v2.MemberResponsible.Equals("No")) Then
                    SetFormField(form348_v2, "part2Check13cWasNot", "1")
                End If
            End If

            If (lod.LODMedical_v2.PsychiatricEval IsNot Nothing) Then
                If (lod.LODMedical_v2.PsychiatricEval.Equals("Yes")) Then
                    SetFormField(form348_v2, "part2Check13dYes", "1")
                    SetFormField(form348_v2, "part2Check13dResults", "See Attachments")

                    If (lod.LODMedical_v2.PsychiatricDate IsNot Nothing) Then
                        If (lod.LODMedical_v2.PsychiatricDate.HasValue) Then
                            SetFormField(form348_v2, "part2Check13dDate", Server.HtmlDecode(lod.LODMedical_v2.PsychiatricDate.Value.ToString("ddMMMyyyy")))
                        End If
                    End If

                ElseIf (lod.LODMedical_v2.PsychiatricEval.Equals("No")) Then
                    SetFormField(form348_v2, "part2Check13dNo", "1")
                End If
            End If

            If (Not String.IsNullOrEmpty(lod.LODMedical_v2.RelevantCondition)) Then
                PossibleRemarks(form348_v2, "part2Check13eOther", Server.HtmlDecode(lod.LODMedical_v2.RelevantCondition))
            Else
                SetFormField(form348_v2, "part2Check13eOther", "None")
            End If

            If (lod.LODMedical_v2.OtherTest IsNot Nothing) Then
                If (lod.LODMedical_v2.OtherTest.Equals("Yes")) Then
                    SetFormField(form348_v2, "part2Check13fYes", "1")
                    SetFormField(form348_v2, "part2Check13fResults", "See Attachments")

                    If (lod.LODMedical_v2.OtherTestDate IsNot Nothing) Then
                        If (lod.LODMedical_v2.OtherTestDate.HasValue) Then
                            SetFormField(form348_v2, "part2Check13fDate", Server.HtmlDecode(lod.LODMedical_v2.OtherTestDate.Value.ToString("ddMMMyyyy")))
                        End If
                    End If

                ElseIf (lod.LODMedical_v2.OtherTest.Equals("No")) Then
                    SetFormField(form348_v2, "part2Check13fNo", "1")
                End If
            End If

            If (lod.LODMedical_v2.DeployedLocation IsNot Nothing) Then
                If (lod.LODMedical_v2.DeployedLocation.Equals("Yes")) Then
                    SetFormField(form348_v2, "part2Check14aYes", "1")
                ElseIf (lod.LODMedical_v2.DeployedLocation.Equals("No")) Then
                    SetFormField(form348_v2, "part2Check14aNo", "1")
                End If
            End If

            If (lod.LODMedical_v2.DeployedLocation IsNot Nothing AndAlso Not lod.LODMedical_v2.DeployedLocation.Equals("Yes")) Then

                If (lod.LODMedical_v2.ConditionEPTS IsNot Nothing) Then
                    If (lod.LODMedical_v2.ConditionEPTS) Then
                        SetFormField(form348_v2, "part2Check14bYes", "1")
                    Else
                        SetFormField(form348_v2, "part2Check14bNo", "1")
                    End If
                End If

                If (lod.LODMedical_v2.ServiceAggravated IsNot Nothing) Then
                    If (lod.LODMedical_v2.ServiceAggravated) Then
                        SetFormField(form348_v2, "part2Check14cYes", "1")
                    Else
                        SetFormField(form348_v2, "part2Check14cNo", "1")
                    End If
                End If

                If (lod.LODMedical_v2.MobilityStandards IsNot Nothing) Then
                    If (lod.LODMedical_v2.MobilityStandards.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check14dYes", "1")
                    ElseIf (lod.LODMedical_v2.MobilityStandards.Equals("No")) Then
                        SetFormField(form348_v2, "part2Check14dNo", "1")
                    End If
                End If

                If (lod.LODMedical_v2.BoardFinalization IsNot Nothing) Then
                    If (lod.LODMedical_v2.BoardFinalization.Equals("Yes")) Then
                        SetFormField(form348_v2, "part2Check14eYes", "1")
                    ElseIf (lod.LODMedical_v2.BoardFinalization.Equals("No")) Then
                        SetFormField(form348_v2, "part2Check14eNo", "1")
                    End If
                End If
            End If

            Dim Medsig As SignatureMetaData

            'Checks legacy path first and if null then checks pilot workflow
            Medsig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.MedicalOfficerReview)
            If (Medsig Is Nothing) Then
                Medsig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v3.MedicalOfficerReview_LODV3) 'Medical Officer (Pilot)
            End If

            AddSignatureToForm_v2(form348_v2, Medsig,
                                   "part2ProviderDate", "ProviderSignature15",
                                   "part2ProviderNameRank",
                                   DBSignTemplateId.Form348Medical,
                                   PersonnelTypes.MED_OFF)
        End If
    End Sub

    Private Sub SetUnitInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim lkupDAO As ILookupDao
        lkupDAO = New NHibernateDaoFactory().GetLookupDao()

        If lod.SarcCase AndAlso lod.IsRestricted Then
            Dim message As String = "This Block not Used"

            SetFormField(form348_v2, "part3ICNameRank", message)
            SetFormField(form348_v2, "part4AdvocateNameRank", message)
            SetFormField(form348_v2, "part5AppointingNameRank", message)
        Else

            '*****************************
            'Unit Commander
            '*****************************
            If (Not IsDBNull(lod.LODUnit_v2)) Then
                If (lod.LODUnit_v2 IsNot Nothing) Then

                    SetFormField(form348_v2, "part3To", lod.AppointingUnit)
                    SetFormField(form348_v2, "part3From", lod.ToUnit)

                    If (lod.LODUnit_v2.SourceInformation IsNot Nothing) Then
                        If (lod.LODUnit_v2.SourceInformation = InfoSource.Member) Then
                            SetFormField(form348_v2, "part3Check18Member", "1")
                        ElseIf (lod.LODUnit_v2.SourceInformation = InfoSource.CivilianPolice) Then
                            SetFormField(form348_v2, "part3Check18CivPolice", "1")
                        ElseIf (lod.LODUnit_v2.SourceInformation = InfoSource.MilitaryPolice) Then
                            SetFormField(form348_v2, "part3Check18MilPolice", "1")
                        ElseIf (lod.LODUnit_v2.SourceInformation = InfoSource.OSI) Then
                            SetFormField(form348_v2, "part3Check18OSI", "1")
                        ElseIf (lod.LODUnit_v2.SourceInformation = InfoSource.Witness) Then
                            SetFormField(form348_v2, "part3Check18Witness", "1")
                        ElseIf (lod.LODUnit_v2.SourceInformation = InfoSource.Other) Then
                            SetFormField(form348_v2, "part3Check18Other", "1")
                            SetFormField(form348_v2, "part3Check18OtherSpecify", lod.LODUnit_v2.SourceInformationSpecify)
                        End If
                    End If

                    If (lod.LODUnit_v2.Witnesses IsNot Nothing AndAlso lod.LODUnit_v2.Witnesses.Count > 0) Then
                        If (lod.LODUnit_v2.Witnesses.Count >= 1) Then
                            Dim wit As WitnessData = lod.LODUnit_v2.Witnesses(0)
                            Dim name As String = Server.HtmlDecode(wit.Name)
                            Dim address As String = Server.HtmlDecode(wit.Address)
                            Dim phonenumber As String = Server.HtmlDecode(wit.PhoneNumber)
                            SetFormField(form348_v2, "part3NameAddr1", name + ", " + address + ", " + phonenumber)
                        End If

                        If (lod.LODUnit_v2.Witnesses.Count >= 2) Then
                            Dim wit As WitnessData = lod.LODUnit_v2.Witnesses(1)
                            Dim name As String = Server.HtmlDecode(wit.Name)
                            Dim address As String = Server.HtmlDecode(wit.Address)
                            Dim phonenumber As String = Server.HtmlDecode(wit.PhoneNumber)
                            SetFormField(form348_v2, "part3NameAddr2", name + ", " + address + ", " + phonenumber)
                        End If

                        If (lod.LODUnit_v2.Witnesses.Count >= 3) Then
                            Dim wit As WitnessData = lod.LODUnit_v2.Witnesses(2)
                            Dim name As String = Server.HtmlDecode(wit.Name)
                            Dim address As String = Server.HtmlDecode(wit.Address)
                            Dim phonenumber As String = Server.HtmlDecode(wit.PhoneNumber)
                            SetFormField(form348_v2, "part3NameAddr3", name + ", " + address + ", " + phonenumber)
                        End If

                        If (lod.LODUnit_v2.Witnesses.Count >= 4) Then
                            Dim wit As WitnessData = lod.LODUnit_v2.Witnesses(3)
                            Dim name As String = Server.HtmlDecode(wit.Name)
                            Dim address As String = Server.HtmlDecode(wit.Address)
                            Dim phonenumber As String = Server.HtmlDecode(wit.PhoneNumber)
                            SetFormField(form348_v2, "part3NameAddr4", name + ", " + address + ", " + phonenumber)
                        End If

                        If (lod.LODUnit_v2.Witnesses.Count >= 5) Then
                            Dim wit As WitnessData = lod.LODUnit_v2.Witnesses(4)
                            Dim name As String = Server.HtmlDecode(wit.Name)
                            Dim address As String = Server.HtmlDecode(wit.Address)
                            Dim phonenumber As String = Server.HtmlDecode(wit.PhoneNumber)
                            SetFormField(form348_v2, "part3NameAddr5", name + ", " + address + ", " + phonenumber)
                        End If
                    Else
                        SetFormField(form348_v2, "part3NameAddr1", "No witnesses presented")
                    End If

                    If (lod.LODUnit_v2.MemberOccurrence IsNot Nothing) Then
                        If (lod.LODUnit_v2.MemberOccurrence = Occurrence.Present) Then
                            SetFormField(form348_v2, "part3Check19Present", "1")
                        ElseIf (lod.LODUnit_v2.MemberOccurrence = Occurrence.AbsentWithAuthority) Then
                            SetFormField(form348_v2, "part3Check19AbsentW", "1")
                        ElseIf (lod.LODUnit_v2.MemberOccurrence = Occurrence.AbsentWithoutAuthority) Then
                            SetFormField(form348_v2, "part3Check19AbsentWO", "1")

                            If (lod.LODUnit_v2.AbsentFrom IsNot Nothing) Then
                                If (lod.LODUnit_v2.AbsentFrom.HasValue) Then
                                    SetFormField(form348_v2, "part3Check19AbsentWODate", Server.HtmlDecode(lod.LODUnit_v2.AbsentFrom.Value.ToString("ddMMMyyyy")))
                                    SetFormField(form348_v2, "part3Check19AbsentWOTime", Server.HtmlDecode(lod.LODUnit_v2.AbsentFrom.Value.ToString(HOUR_FORMAT)))
                                End If
                            End If

                            If (lod.LODUnit_v2.AbsentTo IsNot Nothing) Then
                                If (lod.LODUnit_v2.AbsentTo.HasValue) Then
                                    SetFormField(form348_v2, "part3Check19AbsentWO2Date", Server.HtmlDecode(lod.LODUnit_v2.AbsentTo.Value.ToString("ddMMMyyyy")))
                                    SetFormField(form348_v2, "part3Check19AbsentWO2Time", Server.HtmlDecode(lod.LODUnit_v2.AbsentTo.Value.ToString(HOUR_FORMAT)))
                                End If
                            End If
                        End If
                    End If

                    If (lod.LODUnit_v2.DutyDetermination = DutyStatus.Travel_to_from_duty) Then
                        If (lod.LODUnit_v2.TravelOccurrence.Value = Occurrence.InactiveDutyTraining) Then
                            SetFormField(form348_v2, "part3Check19IDT", "1")
                        ElseIf (lod.LODUnit_v2.TravelOccurrence.Value = Occurrence.DutyOrTraining) Then
                            SetFormField(form348_v2, "part3Check19Duty", "1")
                        End If
                    End If

                    PossibleRemarks(form348_v2, "part3InvestigationResult", Server.HtmlDecode(lod.LODUnit_v2.AccidentDetails))
                    'SetFormField(form348_v2, "part3InvestigationResult", lod.LODUnit_v2.AccidentDetails)

                    If (lod.LODUnit_v2.ProximateCause IsNot Nothing) Then
                        Dim prox = (From n In lkupDAO.GetCauses() Where n.Value = lod.LODUnit_v2.ProximateCause Select n).FirstOrDefault
                        If (lod.LODUnit_v2.ProximateCause = ProximateCause.Misconduct) Then
                            SetFormField(form348_v2, "part3Check20Misconduct", "1")
                        ElseIf (lod.LODUnit_v2.ProximateCause = 13) Then
                            SetFormField(form348_v2, "part3Check20Other", "1")
                            SetFormField(form348_v2, "part3Check20OtherSpecify", Server.HtmlDecode(lod.LODUnit_v2.ProximateCauseSpecify))
                        ElseIf (lod.LODUnit_v2.ProximateCause > 1) Then
                            SetFormField(form348_v2, "part3Check20Other", "1")
                            SetFormField(form348_v2, "part3Check20OtherSpecify", prox.Name)
                        End If
                    End If

                    Dim unitFinding As LineOfDutyFindings
                    unitFinding = lod.FindByType(PersonnelTypes.UNIT_CMDR)
                    If unitFinding IsNot Nothing Then

                        If unitFinding.Finding IsNot Nothing Then
                            concurredFinding = unitFinding.Finding

                            Select Case concurredFinding
                                Case Finding.In_Line_Of_Duty
                                    SetFormField(form348_v2, "part3Check22ILOD", "1")
                                Case Finding.Recommend_Formal_Investigation
                                    SetFormField(form348_v2, "part3Check22FLOD", "1")
                                Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                    SetFormField(form348_v2, "part3Check22NILOD", "1")
                            End Select
                        End If

                        Dim sig As SignatureMetaData

                        'Checks legacy path first and if null then checks pilot workflow
                        sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.UnitCommanderReview)
                        If (sig Is Nothing) Then
                            sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v3.UnitCommanderReview_LODV3) 'Unit Commander Review (Pilot)
                        End If

                        AddSignatureToForm_v2(form348_v2, sig,
                                        "part3ICDate", "CommanderSignature23",
                                        "part3ICNameRank",
                                       DBSignTemplateId.Form348Unit,
                                       PersonnelTypes.UNIT_CMDR)

                    End If
                End If
            End If
        End If
    End Sub

    Private Sub SetWingCommanderInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim sig As SignatureMetaData

        'Checks legacy path first and if null then checks pilot workflow
        sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.AppointingAutorityReview)
        If (sig Is Nothing) Then
            sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v3.AppointingAutorityReview_LODV3) 'Appointing Authority Action (Pilot)
        End If

        If (Not sig Is Nothing) Then

            Dim appointingfinding As LineOfDutyFindings
            appointingfinding = lod.FindByType(PersonnelTypes.APPOINT_AUTH)
            If appointingfinding IsNot Nothing Then

                If appointingfinding.Finding IsNot Nothing Then
                    If appointingfinding.Finding.HasValue Then
                        Select Case appointingfinding.Finding
                            Case Finding.In_Line_Of_Duty
                                SetFormField(form348_v2, "part5Check26ILOD", "1")
                            Case Finding.Recommend_Formal_Investigation
                                SetFormField(form348_v2, "part5Check26FLOD", "1")
                            Case Finding.Nlod_Not_Due_To_OwnMisconduct
                                SetFormField(form348_v2, "part5Check26NILOD", "1")
                        End Select
                    End If

                    AddSignatureToForm_v2(form348_v2, sig,
                                           "part5AppointingDate", "AppointingSignature27",
                                           "part5AppointingNameRank",
                                           DBSignTemplateId.WingCC,
                                           PersonnelTypes.APPOINT_AUTH)
                End If
            End If
        End If
    End Sub

    Private Sub SetWingJudgeAdvocateInfo(ByVal form348_v2 As PDFForm, ByVal lod As LineOfDuty_v2)

        Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.WingJAReview)

        If (Not sig Is Nothing) Then

            Dim jaFinding As LineOfDutyFindings
            jaFinding = lod.FindByType(PersonnelTypes.WING_JA)

            If jaFinding IsNot Nothing Then

                If jaFinding.DecisionYN = "Y" Then
                    SetFormField(form348_v2, "part4Check24Concur", "1")
                ElseIf jaFinding.DecisionYN = "N" Then
                    SetFormField(form348_v2, "part4Check24NonConcur", "1")
                End If

                AddSignatureToForm_v2(form348_v2, sig,
                                           "part4AdvocateDate", "WingSignature25",
                                           "part4AdvocateNameRank",
                                           DBSignTemplateId.Form348Findings,
                                           PersonnelTypes.WING_JA)
            End If
        End If
    End Sub

End Class