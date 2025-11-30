Imports System.Configuration
Imports System.Text
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports ALODWebUtility.PrintingUtil

Public Class Form348_SARC
    Inherits System.Web.UI.Page
    Implements IDocumentCreate

    Protected Const EpochDate As Date = #1/29/2010#
    Private Const BRANCH_AFRC As String = "AFRC"
    Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
    Private Const ROTC_CADET_ID As Integer = 5
    Private Const SIGNED_TEXT As String = "//SIGNED//"
    Private _daoFactory As NHibernateDaoFactory
    Private _sarcDao As ISARCDAO
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

    Protected ReadOnly Property DaoFactory As NHibernateDaoFactory
        Get
            If (_daoFactory Is Nothing) Then
                _daoFactory = New NHibernateDaoFactory()
            End If

            Return _daoFactory
        End Get
    End Property

    Protected ReadOnly Property SARCDao As ISARCDAO
        Get
            If (_sarcDao Is Nothing) Then
                _sarcDao = DaoFactory.GetSARCDao()
            End If

            Return _sarcDao
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
        Try
            Dim sarc As RestrictedSARC = Nothing

            Dim form348R As New PDFForm(PrintDocuments.FormAFRC348R)

            sarc = SARCDao.GetById(refId)

            SetFormField(form348R, "sarcCaseNumberP1", Server.HtmlDecode(sarc.CaseId))
            SetDateTimeField(form348R, "ReportDateFill", sarc.CreatedDate, "ddMMMyyyy", True)
            SetDateTimeField(form348R, "IncidentDateFill", sarc.IncidentDate, "ddMMMyyyy", True)
            SetFormField(form348R, "DatabaseNumberFill", Server.HtmlDecode(sarc.DefenseSexualAssaultDBCaseNumber))
            SetDateTimeField(form348R, "OrdersStart", sarc.DurationStart, "ddMMMyyyy", True)
            SetDateTimeField(form348R, "d_StartTime", sarc.DurationStart, Utility.HOUR_FORMAT)
            SetDateTimeField(form348R, "OrdersEnd", sarc.DurationEnd, "ddMMMyyyy", True)
            SetDateTimeField(form348R, "d_EndTime", sarc.DurationEnd, Utility.HOUR_FORMAT)

            Set348RDutyStatusField(form348R, sarc)
            Set348RICDRelatedFields(form348R, sarc)

            SetInDutyStatusField(form348R, sarc)

            SetFormField(form348R, "Sec8Block", "BLOCK NOT USED")

            Set348RSignatureFields(form348R, sarc)

            SetRemarksField(form348R, sarc)

            Return form348R
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: Generate348RForm() in PDFCreateFactory.vb generated an exception.")
        End Try
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
            SetFormField(doc, dateField, signInfo.DateSigned.ToString("ddMMMyyyy").ToUpper())
            valid = True
        Else
            'Use electronic signature
            SetFormField(doc, sigField, SIGNED_TEXT)
            SetFormField(doc, dateField, signature.date.ToString("ddMMMyyyy").ToUpper())
            valid = False
        End If

        Return valid
    End Function

    Protected Function GetApprovingAuthorityRemarks(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC) As String
        If (sarc.DutyStatus.HasValue AndAlso sarc.DutyStatus.Value <> ROTC_CADET_ID) Then
            Return GetSARCApprovingAuthorityFindingsRemarksText(sarc)
        End If

        Return String.Empty
    End Function

    Protected Function GetOtherICDCodesRemarksText(ByVal sarc As RestrictedSARC) As String
        Try
            If (Not sarc.ICDOther.HasValue OrElse sarc.ICDOther.Value = False OrElse sarc.ICDList.Count = 0) Then
                Return String.Empty
            End If

            Dim icdRemarks As String = "Other ICD Code(s):"

            For Each code As RestrictedSARCOtherICDCode In sarc.ICDList
                icdRemarks &= " [" & code.ICDCode.GetFullCode(code.ICD7thCharacter) & "] " & code.ICDCode.Description & ";"
            Next

            icdRemarks = icdRemarks.Substring(0, icdRemarks.Length - 1) & Environment.NewLine & Environment.NewLine

            Return icdRemarks
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: GetOtherICDCodesRemarksText() in PDFCreateFactory.vb generated an exception.")
        End Try
    End Function

    Protected Function GetSARCAdminRemarks(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC) As String
        If (sarc.DutyStatus.HasValue AndAlso sarc.DutyStatus.Value <> ROTC_CADET_ID) Then
            Return GetSARCAdminRemarksText(sarc)
        End If

        Return String.Empty
    End Function

    Protected Function GetSARCAdminRemarksText(ByVal sarc As RestrictedSARC) As String
        Try
            If (SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCAdminReview) Is Nothing) Then
                Return String.Empty
            End If

            Dim sarcAdminRemarksHeader As String = "SARC Administrator Remarks: "
            Dim sarcAdminFindings As RestrictedSARCFindings = sarc.FindByType(PersonnelTypes.SARC_ADMIN)
            Dim newLines As String = Environment.NewLine & Environment.NewLine

            If (sarcAdminFindings Is Nothing) Then
                Return (sarcAdminRemarksHeader & "REMARKS NOT FOUND!" & newLines)
            End If

            Return (sarcAdminRemarksHeader & RemoveNewLinesFromString(Server.HtmlDecode(sarcAdminFindings.Remarks)) & newLines)
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: GetSARCAdminRemarksText() in PDFCreate.vb generated an exception.")
        End Try
    End Function

    Protected Function GetSARCApprovingAuthorityFindingsRemarksText(ByVal sarc As RestrictedSARC) As String
        Try
            If (SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCApprovingAuthorityReview) Is Nothing) Then
                Return String.Empty
            End If

            Dim approvingAuthorityFindingsHeader As String = "Reviewing Authority (ARC/A1) Findings: "
            Dim approvingAuthorityFindings As RestrictedSARCFindings = sarc.FindByType(PersonnelTypes.BOARD_AA)
            Dim newLines As String = Environment.NewLine & Environment.NewLine

            If (approvingAuthorityFindings Is Nothing OrElse approvingAuthorityFindings.Finding Is Nothing) Then
                Return (approvingAuthorityFindingsHeader & "FINDINGS NOT FOUND!" & newLines)
            End If

            If (approvingAuthorityFindings.Finding = Finding.Request_Consultation) Then
                Return String.Empty
            End If

            Return (approvingAuthorityFindingsHeader & GetFindingFormText(approvingAuthorityFindings.Finding.Value) & newLines)
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: GetSARCApprovingAuthorityFindingsRemarksText() in PDFCreate.vb generated an exception.")
        End Try
    End Function

    Protected Sub LogSARCDeniedError(ByVal refId As Integer)
        Try
            Dim msg As New StringBuilder()
            msg.Append("Access Denied" + System.Environment.NewLine)
            msg.Append("UserID: " + refId.ToString() + System.Environment.NewLine)
            msg.Append("Request: " + Request.Url.ToString() + System.Environment.NewLine)

            If (Request.UrlReferrer IsNot Nothing) Then
                msg.Append("Referrer: " + Request.UrlReferrer.ToString() + System.Environment.NewLine)
            End If

            msg.Append("Reason: User is attempting to view a Restricted SARC PDF without permission")

            LogManager.LogError(msg.ToString())
            Response.Redirect(ConfigurationManager.AppSettings("AccessDeniedUrl"))
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: LogSARCDeniedError() in PDFCreateFactory.vb generated an exception.")
        End Try
    End Sub

    Protected Sub Set348RDutyStatusField(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        Try
            If (Not sarc.DutyStatus.HasValue()) Then
                Exit Sub
            End If

            Dim fieldName As String = String.Empty

            Select Case sarc.DutyStatus.Value
                Case 1
                    fieldName = "AFRCheck"
                Case 3
                    fieldName = "ANGCheck"
                Case ROTC_CADET_ID
                    fieldName = "AFROTCCheck"
                Case Else
                    Exit Sub
            End Select

            SetFormField(form348R, fieldName, "1")
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: Set348RDutyStatusField() in PDFCreateFactory.vb generated an exception.")
        End Try
    End Sub

    Protected Sub Set348RICDRelatedFields(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        Try
            SetCheckboxField(form348R, "E968Check", sarc.ICDE968)
            SetCheckboxField(form348R, "E969Check", sarc.ICDE969)
            SetCheckboxField(form348R, "OtherCheck", sarc.ICDOther)

            If (sarc.ICDOther.HasValue AndAlso sarc.ICDOther.Value) Then
                SetFormField(form348R, "OtherICDText", "* See Section 10. REMARKS")
            End If
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: Set348RICDRelatedFields() in PDFCreate.vb generated an exception.")
        End Try
    End Sub

    Protected Sub Set348RSignatureFields(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        SetWingSARCRSLSignatureFields(form348R, sarc)
        SetApprovingAuthoritySignatureFields(form348R, sarc)
    End Sub

    Protected Sub SetApprovingAuthoritySignatureFields(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        Try
            If (Not sarc.DutyStatus.HasValue AndAlso sarc.DutyStatus.Value = ROTC_CADET_ID) Then
                Exit Sub
            End If

            AddSignatureInformationToForm(sarc.Id, form348R, SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCApprovingAuthorityReview),
                                              "Sec9DateFill", "LODReviewSign", "Sec9NameRank",
                                              DBSignTemplateId.Form348SARCFindings, PersonnelTypes.BOARD_AA)
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: SetApprovingAuthoritySignatureFields() in PDFCreate.vb generated an exception.")
        End Try
    End Sub

    Protected Sub SetInDutyStatusField(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        If (Not sarc.InDutyStatus.HasValue) Then
            Exit Sub
        End If

        If (sarc.InDutyStatus.Value) Then
            SetCheckboxField(form348R, "YesCheck", True)
        Else
            SetCheckboxField(form348R, "NoCheck", True)
        End If
    End Sub

    Protected Sub SetRemarksField(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        Dim remarksValue As String = String.Empty

        remarksValue &= GetOtherICDCodesRemarksText(sarc)
        remarksValue &= GetSARCAdminRemarks(form348R, sarc)
        remarksValue &= GetApprovingAuthorityRemarks(form348R, sarc)

        SetFormField(form348R, "RemarksFill", remarksValue)
    End Sub

    Protected Sub SetWingSARCRSLSignatureFields(ByVal form348R As PDFForm, ByVal sarc As RestrictedSARC)
        Try

            AddSignatureInformationToForm(sarc.Id, form348R, SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCInitiate),
                                              "Sec7DateFill", "WingSARCSignAFROTC", "Sec7NameRank",
                                              DBSignTemplateId.Form348SARCWing, PersonnelTypes.WING_SARC_RSL)
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: SetWingSARCRSLSignatureFields() in PDFCreate.vb generated an exception.")
        End Try
    End Sub

    Private Sub Add348RNullWatermark(ByVal doc As PDFDocument, ByVal sarc As RestrictedSARC)
        Try
            Dim reason As String = "UNKNOWN"
            Dim sigLine As String = String.Empty
            Dim newLines As Integer = 2

            ' Get cancel reason...
            If (sarc.Cancel_Reason.HasValue) Then
                reason = LookupService.GetCancelReasonDescription(sarc.Cancel_Reason.Value)
            End If

            Dim sig As SignatureMetaData = SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCInitiate)

            sigLine = GetWatermarkSignature(sig, DBSignTemplateId.Form348SARC, sarc.Id, PersonnelTypes.WING_SARC_RSL)

            If (reason.Length > 36) Then
                newLines = 1
            End If

            ' Add watermark strings...
            doc.AddNullWatermarkString(New PDFString() With {.Text = "Case Cancelled", .Linespacing = -20, .FontSize = 36, .FontWeight = "bold", .PostNewLines = 1})
            doc.AddNullWatermarkString(New PDFString() With {.Text = "Reason: " & reason, .FontSize = 24, .FontWeight = "bold", .PostNewLines = newLines})
            doc.AddNullWatermarkString(New PDFString() With {.Text = sigLine, .FontWeight = "bold"})
        Catch ex As Exception
            LogManager.LogError(ex)
            Throw New Exception("Error: Add348RNullWatermark() in PDFCreateFactory.vb generated an exception.")
        End Try
    End Sub

    Private Function GetWatermarkSignature(ByVal signature As SignatureMetaData, ByVal template As DBSignTemplateId, ByVal refId As Integer, ByVal ptype As PersonnelTypes) As String
        If (signature Is Nothing) Then
            Return String.Empty
        End If

        Dim sigLine As String = String.Empty
        Dim dateSigned As Date = signature.date

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

End Class