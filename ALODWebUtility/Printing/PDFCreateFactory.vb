Imports System.Configuration
Imports System.Text
Imports System.Text.RegularExpressions
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Printing
    '********************************************
    ' Builds PDF of Forms 348 and 261 and stores in database when workflow status is Complete (isFinal)
    ' This code was borrowed from lod/Print.aspx.vb and modified
    ' GeneratePdf() procedure is called from the NextAction.aspx tab when user selecte Complete or Cancel and Digitally Signs.

    Public Class PDFCreateFactory
        Inherits System.Web.UI.Page

        'this is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        'signatures which occured before this date use the old //signed// format
        'signatures which occured after this data use the new LAST.FIRST.MIDDLE.EDIPIN format
        Protected Const EpochDate As Date = #1/29/2010#

        Private Const BRANCH_AFRC As String = "AFRC"
        Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
        Private Const ROTC_CADET_ID As Integer = 5
        Private Const SIGNED_TEXT As String = "//SIGNED//"
        Private _daoFactory As NHibernateDaoFactory
        Private _sarcDao As ISARCDAO
        Private _sigDao As ISignatueMetaDateDao
        Private _workStatusDao As IWorkStatusDao
        Private lodid As Integer
        Private remarksField As String = ""
        Private replaceIOSig As Boolean = False
        Private signatureService As DBSignService
        Dim type As ModuleType

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

        Protected ReadOnly Property WorkStatusDao As IWorkStatusDao
            Get
                If (_workStatusDao Is Nothing) Then
                    _workStatusDao = DaoFactory.GetWorkStatusDao()
                End If

                Return _workStatusDao
            End Get
        End Property

        Private Property VerifySource() As DBSignService
            Get
                Return signatureService
            End Get
            Set(value As DBSignService)
                signatureService = value
            End Set
        End Property

        Public Function GenerateLOD(ByVal refId As Integer) As PDFDocument
            'Me.replaceIOSig = replaceIOSig
            Dim isRLod As Boolean = False
            Dim updatedRR As Boolean = False
            Dim factory As New NHibernateDaoFactory()
            Dim dao As LineOfDutyDao = factory.GetLineOfDutyDao()
            Dim lod = New LineOfDuty()
            Dim origLod = New LineOfDuty()
            Dim doc = New PDFDocument()
            Dim wsdao As New WorkStatusDao()
            Dim lodCurrStatus As ALOD.Core.Domain.Workflow.WorkStatus

            If (dao.GetWorkflow(refId) = 27) Then
                lod = New LineOfDuty_v2()
            End If

            lod = LodService.GetById(refId)

            ' Check if this LOD is a reinvestigation of another LOD case...
            isRLod = LookupService.GetIsReinvestigationLod(refId)

            If (isRLod) Then
                ' Check if the RR case has a case ID which matches the new ID format implemented when reinvestigation cases were changed to initialze as a formal case,
                ' start in the Formal Appointing Authority review step, and not create a new Form348...
                Dim rr As LODReinvestigation = LodService.requestDao.GetById(LodService.requestDao.GetReinvestigationRequestIdByRLod(refId))

                Dim regex As Regex = New Regex("^\d{8}-\d{3}-RR-\d{3}?", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

                If (regex.IsMatch(rr.CaseId)) Then
                    ' Load the original LOD case and use that to generate the 348 form...

                    If (dao.GetWorkflow(rr.InitialLodId) = 27) Then
                        origLod = New LineOfDuty_v2()
                    End If

                    origLod = LodService.GetById(rr.InitialLodId)
                    updatedRR = True
                End If
            End If

            Dim form261 = New Form261()
            Dim form348 = New Form348()
            Dim form348_v2 = New Form348_v2()

            If (isRLod AndAlso updatedRR) Then
                If (origLod.Workflow = 27) Then
                    doc.AddForm(form348_v2.GeneratePDFForm(origLod.Id, replaceIOSig))
                Else
                    doc.AddForm(form348.GeneratePDFForm(origLod.Id, replaceIOSig))
                End If

                If (lod.LODInvestigation IsNot Nothing) Then
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig))
                End If
            ElseIf (isRLod AndAlso Not updatedRR) Then
                doc.AddForm(form348.GeneratePDFForm(lod.Id, replaceIOSig))

                If (lod.LODInvestigation IsNot Nothing) Then
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig))
                End If
            Else
                If (lod.Workflow = 27) Then
                    doc.AddForm(form348_v2.GeneratePDFForm(lod.Id, replaceIOSig))
                Else
                    doc.AddForm(form348.GeneratePDFForm(lod.Id, replaceIOSig))
                End If

                If (lod.Formal AndAlso lod.LODInvestigation IsNot Nothing) Then
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig))
                End If
            End If

            lodCurrStatus = wsdao.GetById(lod.Status)

            ' If the case has been cancelled, then add a null watermark to the document
            If (lodCurrStatus.StatusCodeType.IsCancel) Then
                AddNullWatermark(doc, lod)
            End If

            Return doc
        End Function

        Public Function GeneratePdf(ByVal refId As Integer, ByVal moduleId As Integer, ByVal replaceIOsig As Boolean) As PDFDocument
            Me.replaceIOSig = replaceIOsig

            Return GeneratePdf(refId, moduleId)
        End Function

        Public Function GeneratePdf(ByVal refId As Integer, ByVal moduleId As Integer) As PDFDocument

            If (moduleId = ModuleType.LOD) Then
                Return GenerateLOD(refId)
            Else
                Return GenerateRestrictedSARCDocument(refId)
            End If

        End Function

        Private Function IsValidSignature(ByVal signature As SignatureEntry) As Boolean
            If (signature Is Nothing) Then
                Return False
            End If

            Return signature.IsSigned
        End Function

#Region "WaterMark"

        Private Sub AddNullWatermark(ByVal doc As PDFDocument, ByVal lod As LineOfDuty)
            Dim reason As String = String.Empty
            Dim sigLine As String = String.Empty
            Dim newLines As Integer = 2

            ' Get cancel reason...
            If (lod.Formal) Then
                If (lod.AppointingCancelReasonId.HasValue AndAlso lod.AppointingCancelReasonId <> 0) Then
                    reason = LookupService.GetCancelReasonDescription(lod.AppointingCancelReasonId)
                ElseIf (lod.ApprovingCancelReasonId.HasValue AndAlso lod.AppointingCancelReasonId <> 0) Then
                    reason = LookupService.GetCancelReasonDescription(lod.ApprovingCancelReasonId)
                Else
                    reason = "Unknown"
                End If

                Dim WingCCsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.AppointingAutorityReview)

                sigLine = GetWatermarkSignature(WingCCsig, DBSignTemplateId.WingCC, lod.Id, PersonnelTypes.APPOINT_AUTH)
            Else
                If (lod.LODMedical.PhysicianCancelReason <> 0) Then
                    reason = LookupService.GetCancelReasonDescription(lod.LODMedical.PhysicianCancelReason)
                Else
                    reason = "Unknown"
                End If

                Dim Medsig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.MedicalOfficerReview)

                sigLine = GetWatermarkSignature(Medsig, DBSignTemplateId.Form348Medical, lod.Id, PersonnelTypes.MED_OFF)
            End If

            If (reason.Length >= 36) Then
                newLines = 1
            End If

            ' Add watermark strings...
            doc.AddNullWatermarkString(New PDFString() With {.Text = "Case Cancelled", .Linespacing = -20, .FontSize = 36, .FontWeight = "bold", .PostNewLines = 1})
            doc.AddNullWatermarkString(New PDFString() With {.Text = "Reason: " & reason, .FontSize = 24, .FontWeight = "bold", .PostNewLines = newLines})
            doc.AddNullWatermarkString(New PDFString() With {.Text = sigLine, .FontWeight = "bold"})
            'doc.AddNullWatermarkString(New PDFString() With {.Text = "Digitally signed by EXAMPLE.EXAMPLE.EXAMPLE.123456789<BR>Date: 2015.08.21 14:52:53 -04'00'", .FontWeight = "bold"})
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

#End Region

#Region "Restricted SARC - Form 348R"

        Public Function GenerateRestrictedSARCDocument(ByVal refId As Integer) As PDFDocument
            Try
                If (Not UserHasPermission(PERMISSION_VIEW_SARC_CASES)) Then
                    LogSARCDeniedError(refId)
                    Return Nothing
                End If

                Dim form348_SARC = New Form348_SARC()
                Dim doc As PDFDocument = New PDFDocument()
                Dim sarc As RestrictedSARC = Nothing
                Dim currentWorkStatus As WorkStatus = Nothing

                sarc = SARCDao.GetById(refId)

                doc.AddForm(form348_SARC.GeneratePDFForm(refId, replaceIOSig))

                currentWorkStatus = WorkStatusDao.GetById(sarc.WorkflowStatus.Id)

                If (currentWorkStatus.StatusCodeType.IsCancel) Then
                    Add348RNullWatermark(doc, sarc)
                End If

                Return doc
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: GenerateSARCRestrictedDocument() in PDFCreateFactory.vb generated an exception.")
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

#End Region

    End Class

End Namespace