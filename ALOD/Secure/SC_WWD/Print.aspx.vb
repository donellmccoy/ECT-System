Imports ALOD.Core.Domain
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Namespace Web.Special_Case.WWD

    Partial Class Secure_lod_Print
        Inherits System.Web.UI.Page

        'this is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        'signatures which occured before this date use the old //signed// format
        'signatures which occured after this data use the new LAST.FIRST.MIDDLE.EDIPIN format
        Protected Const EpochDate As Date = #1/29/2010#

        Private Const BRANCH_AFRC As String = "AFRC"
        Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
        Private Const SIGNED_TEXT As String = "//SIGNED//"
        Const xmark As String = "Yes"
        Private _assocaiated As IAssociatedCaseDao
        Private _factory As IDaoFactory
        Private _MedTechSig As SignatureMetaData
        Private _sigDao As ISignatueMetaDateDao
        Private dao As ISpecialCaseDAO
        Private lodid As Integer
        Private sc As SC_WWD = Nothing
        Private scId As Integer = 0
        Private signatureService As DBSignService
        Dim type As ModuleType

        ReadOnly Property associated() As IAssociatedCaseDao
            Get
                If (_assocaiated Is Nothing) Then
                    _assocaiated = factory.GetAssociatedCaseDao()
                End If

                Return _assocaiated
            End Get
        End Property

        ReadOnly Property factory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = factory.GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_WWD
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("RefId"))
                End If

                Return sc
            End Get
        End Property

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
                End If

                Return _MedTechSig
            End Get
        End Property

        Private ReadOnly Property SigDao() As SignatureMetaDataDao
            Get
                If (_sigDao Is Nothing) Then
                    _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
                End If

                Return _sigDao
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

        ''' <summary>
        ''' Sets a mark in either the yes or no field on the form
        ''' </summary>
        ''' <param name="YNindicator">Nullable int - null or 0, check the N field, 1, check the Y field</param>
        ''' <param name="WWDForm">The PDFForm</param>
        ''' <param name="Yfield">Field to set for Y</param>
        ''' <param name="Nfield">Field to set for N</param>
        ''' <remarks></remarks>
        Sub SetYNValue(ByVal YNindicator As Integer?, ByVal WWDForm As PDFForm, ByVal Yfield As String, ByVal Nfield As String)

            If YNindicator.HasValue Then
                If YNindicator.Value > 0 Then
                    SetFormField(WWDForm, Yfield, xmark)
                Else
                    If Not Nfield Is Nothing Then
                        SetFormField(WWDForm, Nfield, xmark)
                    End If
                End If
            Else
                If Not Nfield Is Nothing Then
                    SetFormField(WWDForm, Nfield, String.Empty)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sets a mark in either the yes or no field on the form
        ''' </summary>
        ''' <param name="YNindicator">Nullable date - null , check the N field, otherwise, check the Y field</param>
        ''' <param name="WWDForm">The PDFForm</param>
        ''' <param name="Yfield">Field to set for Y</param>
        ''' <param name="Nfield">Field to set for N</param>
        ''' <remarks></remarks>
        Sub SetYNValue(ByVal YNindicator As Date?, ByVal WWDForm As PDFForm, ByVal Yfield As String, ByVal Nfield As String)

            If YNindicator.HasValue Then
                SetFormField(WWDForm, Yfield, xmark)
            Else
                If Not Nfield Is Nothing Then
                    SetFormField(WWDForm, Nfield, String.Empty)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sets a mark in either the yes or no field on the form
        ''' </summary>
        ''' <param name="YNindicator">Nullable bool - null or false, check the N field, true, check the Y field</param>
        ''' <param name="WWDForm">The PDFForm</param>
        ''' <param name="Yfield">Field to set for Y</param>
        ''' <param name="Nfield">Field to set for N</param>
        ''' <remarks></remarks>
        Sub SetYNValue(ByVal YNindicator As Boolean?, ByVal WWDForm As PDFForm, ByVal Yfield As String, ByVal Nfield As String)

            If YNindicator.HasValue Then
                If YNindicator.Value Then
                    SetFormField(WWDForm, Yfield, xmark)
                Else
                    If Not Nfield Is Nothing Then
                        SetFormField(WWDForm, Nfield, xmark)
                    End If
                End If
            Else
                If Not Nfield Is Nothing Then
                    SetFormField(WWDForm, Nfield, String.Empty)
                End If
            End If
        End Sub

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
                SetFormField(doc, concurField, concurText + newFinding)
                SetFormField(doc, findingField, boardFinding.FindingsText)
                Return True
            Else
                concurText = "Non Concur with Appointing Authority. Recommended new finding: "

                If (boardFinding.Finding.HasValue) Then
                    Select Case boardFinding.Finding.Value
                        Case Finding.In_Line_Of_Duty
                            newFinding = "Line of Duty"
                        Case Finding.Epts_Lod_Not_Applicable
                            newFinding = "EPTS-LOD Not Applicable"
                        Case Finding.Nlod_Due_To_Own_Misconduct
                            newFinding = "Not ILOD-Due to Own Misconduct"
                        Case Finding.Epts_Service_Aggravated
                            newFinding = "EPTS-Service Aggravated"
                        Case Finding.Nlod_Not_Due_To_OwnMisconduct
                            newFinding = "Not ILOD-Not Due to Own Misconduct"
                        Case Finding.Recommend_Formal_Investigation
                            newFinding = "Formal Investigation"
                    End Select

                    SetFormField(doc, concurField, concurText + newFinding)
                    SetFormField(doc, findingField, boardFinding.FindingsText)
                    Return True
                Else
                    Return False
                End If
            End If

            Return False

        End Function

        Protected Function AddFormalFinding(ByVal doc As PDFForm, ByVal boardFinding As LineOfDutyFindings, ByVal concurField As String, ByVal findingField As String) As Boolean

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

                    Select Case boardFinding.Finding.Value
                        Case Finding.In_Line_Of_Duty
                            newFinding = "Line of Duty"
                        Case Finding.Epts_Lod_Not_Applicable
                            newFinding = "EPTS-LOD Not Applicable"
                        Case Finding.Nlod_Due_To_Own_Misconduct
                            newFinding = "Not ILOD-Due to Own Misconduct"
                        Case Finding.Epts_Service_Aggravated
                            newFinding = "EPTS-Service Aggravated"
                        Case Finding.Nlod_Not_Due_To_OwnMisconduct
                            newFinding = "Not ILOD-Not Due to Own Misconduct"
                        Case Finding.Recommend_Formal_Investigation
                            newFinding = "Formal Investigation"
                    End Select

                End If
            End If

            SetFormField(doc, concurField, concurText + newFinding)
            SetFormField(doc, findingField, boardFinding.FindingsText)

            Return valid

        End Function

        Protected Function AddFormalSignatureToForm(ByVal doc As PDFForm, ByVal signature As PersonnelData, ByVal dateSigned As Date?, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal rankField As String, ByVal branchField As String, ByVal unitField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean

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
                SetFormField(doc, branchField, BRANCH_AFRC)
            End If

            If (dateSigned Is Nothing) OrElse (Not dateSigned.HasValue) Then
                'no signature date, so don't add the signature, meaning we're done
                Return False
            End If

            If (dateSigned.Value < EpochDate) Then

                'use the old style signature
                SetFormField(doc, sigField, SIGNED_TEXT)

                'use the passed in date
                SetFormField(doc, dateField, dateSigned.Value.ToString("dd MMM yyyy"))

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
                    + "Date: " + signInfo.DateSigned.ToString("dd MMM yyyy")

                SetFormField(doc, sigField, sigLine)
                SetFormField(doc, dateField, signInfo.DateSigned.ToString("dd MMM yyyy"))
                valid = True
            Else
                'otherwise, clear those fields
                SetFormField(doc, sigField, String.Empty)
                SetFormField(doc, dateField, String.Empty)
                valid = False
            End If

            Return valid

        End Function

        ''' <summary>
        ''' Adds a digital signature to a form
        ''' </summary>
        ''' <param name="doc">The PDF form to sign</param>
        ''' <param name="signature">The Signature entry to add to the form</param>
        ''' <param name="dateField">the name of the date field on the form</param>
        ''' <param name="sigField">the name of the signature field on the form</param>
        ''' <param name="nameField">the name of the name field on the form</param>
        ''' <param name="template">The DBSign template used for this signature</param>
        ''' <param name="ptype">Personnel Type.  Used to retrieve the correct signature information from findings</param>
        ''' <remarks>
        ''' if the dateSigned is before the epoch date, will add //signed// to the form
        ''' if the dateSigned is after the epoch date, will
        '''   1) verify signature is still valid
        '''   2) if so, add the signers digital signature in the form LAST.FIRST.MIDDLE.EDIPIN
        '''   3) if not valid, no signature will be added
        ''' </remarks>
        Protected Function AddSignatureToForm(ByVal doc As PDFForm, ByVal signature As SignatureEntry, ByVal dateField As String, ByVal sigField As String, ByVal nameField As String, ByVal titleField As String, ByVal template As DBSignTemplateId, ByVal ptype As PersonnelTypes) As Boolean

            If (Not IsValidSignature(signature)) Then
                SetFormField(doc, dateField, String.Empty)
                SetFormField(doc, nameField, String.Empty)
                SetFormField(doc, sigField, String.Empty)
                SetFormField(doc, titleField, String.Empty)
                Return False
            End If

            SetFormField(doc, nameField, signature.NameAndRank)

            Dim dateSigned As Date = signature.DateSigned.Value.ToString("dd MMM yyyy")

            If (dateSigned < EpochDate) Then

                'use the old style signature
                SetFormField(doc, sigField, SIGNED_TEXT)

                'use the passed in date
                SetFormField(doc, dateField, dateSigned.ToString("dd MMM yyyy"))

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
                    + "Date: " + signInfo.DateSigned.ToString("dd MMM yyyy")

                SetFormField(doc, sigField, sigLine)
                SetFormField(doc, dateField, signInfo.DateSigned.ToString("dd MMM yyyy"))
                valid = True
            Else
                'otherwise, clear those fields
                SetFormField(doc, sigField, String.Empty)
                SetFormField(doc, dateField, String.Empty)
                valid = False
            End If

            'finally set the title field
            SetFormField(doc, titleField, signature.Title)

            Return valid
        End Function

        Protected Function GeneratePdf(ByVal refId As Integer) As PDFDocument
            Dim strComments As String = "Generated Form 348 PDF"
            Dim strpath As String = Page.ResolveClientUrl("~/secure/documents/")
            Dim doc As New PDFDocument

            Dim WWDForm As New PDFForm("WWDForm.pdf")
            SetFormField(WWDForm, "fCase_Id", SpecCase.CaseId)
            SetFormField(WWDForm, "fMemberName", SpecCase.MemberName + "/" + SpecCase.MemberRank.Title) 'change
            SetFormField(WWDForm, "fMemberSSN", SpecCase.MemberSSN)
            SetFormField(WWDForm, "fMemberDOB", Format(SpecCase.MemberDOB, "dd MMM yyyy"))
            SetFormField(WWDForm, "fPOCRMU", SpecCase.PocUnit)
            If Not IsNothing(MedTechSig) Then
                SetFormField(WWDForm, "fPOCName", MedTechSig.NameAndRank)
            End If
            SetFormField(WWDForm, "fPOCDSN", SpecCase.PocPhoneDSN)
            SetFormField(WWDForm, "fPOCEmail", SpecCase.PocEmail)

            Dim cases = associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow)

            If (cases.Count > 0) Then
                SetYNValue(cases.First.associated_RefId, WWDForm, "fAssociatedLodY", "fAssociatedLodN")
            Else '0 is the same as False or No
                SetYNValue(0, WWDForm, "fAssociatedLodY", "fAssociatedLodN")
            End If
            SetFormField(WWDForm, "fCompletedLodIlodEptsSaN", "Yes")
            SetYNValue(SpecCase.CoverLetterUploaded, WWDForm, "fCoverLetterY", "fCoverLetterN")
            If SpecCase.WWDDocsAttached = 1 Then
                SetYNValue(SpecCase.AFForm469Uploaded, WWDForm, "fAFForm469Y", "fAFForm469N")
                SetYNValue(SpecCase.NarrativeSummaryUploaded, WWDForm, "fNarrativeY", "fNarrativeN")
                SetYNValue(SpecCase.IPEBElection, WWDForm, "fIPEBElectionFormY", "fIPEBElectionFormN")
                SetYNValue(SpecCase.IPEBRefusal, WWDForm, "fIPEBRefuseToSign", Nothing)
                If SpecCase.IPEBSignatureDate.HasValue Then
                    SetFormField(WWDForm, "fIPEBRefuseToSignDate", Format(SpecCase.IPEBSignatureDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fIPEBRefuseToSignDate", String.Empty)
                End If
                If SpecCase.MUQRequestDate.HasValue Then
                    SetFormField(WWDForm, "fMUQRequestDate", Format(SpecCase.MUQRequestDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fMUQRequestDate", String.Empty)
                End If
                If SpecCase.MUQ_Valid.HasValue Then
                    SetYNValue(SpecCase.MUQ_Valid, WWDForm, "fMUQNotReceivedOver60Y", "fMUQNotReceivedOver60N")
                End If
                SetYNValue(SpecCase.CoverLtrIncMemberStatement, WWDForm, "fMUQCoverLetterY", "fMUQCoverLetterN")
                SetYNValue(SpecCase.UnitCmdrMemoUploaded, WWDForm, "fUnitCCMemoY", "fUnitCCMemoN")
                If SpecCase.MedEvalFactSheetSignDate.HasValue Then
                    SetFormField(WWDForm, "fMedEvalFactSheetSignDate", Format(SpecCase.MedEvalFactSheetSignDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fMedEvalFactSheetSignDate", String.Empty)
                End If
                If SpecCase.MedEvalFSWaiverSignDate.HasValue Then
                    SetFormField(WWDForm, "fMedEvalFSWaiverSignDate", Format(SpecCase.MedEvalFSWaiverSignDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fMedEvalFSWaiverSignDate", String.Empty)
                End If
                SetYNValue(SpecCase.PrivatePhysicianDocsUploaded, WWDForm, "fDocumentationFromPhysicianY", "fDocumentationFromPhysicianN")
            End If
            If SpecCase.WWDDocsAttached = 0 Then
                If SpecCase.PS3811RequestDate.HasValue Then
                    SetFormField(WWDForm, "fRequestDateCertMail", Format(SpecCase.PS3811RequestDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fRequestDateCertMail", String.Empty)
                End If
                SetYNValue(SpecCase.PS3811Uploaded, WWDForm, "fPS3811IncludedY", "fPS3811IncludedN")
                SetYNValue(SpecCase.PS3811SignDate, WWDForm, "fPS3811SignedByMemberY", "fPS3811SignedByMemberN")
                If SpecCase.FirstClassMailDate.HasValue Then
                    SetFormField(WWDForm, "fFirstClassMailDate", Format(SpecCase.FirstClassMailDate, "dd MMM yyyy"))
                Else
                    SetFormField(WWDForm, "fFirstClassMailDate", String.Empty)
                End If
                SetYNValue(SpecCase.CoverLtrIncContactAttemptDetails, WWDForm, "fIncludeDetailsToContactMemberY", "fIncludeDetailsToContactMemberN")
                SetYNValue(SpecCase.MemberLetterUploaded, WWDForm, "fCopyofCoverLetterWithAttachmentsY", "fCopyofCoverLetterWithAttachmentsN")
            End If

            Dim sigblock As String = ""

            If (Not SpecCase.Status = SpecCaseWWDWorkStatus.Cancelled) Then
                If (SpecCase.med_off_approved = 0) Then
                    SetFormField(WWDForm, "fDisqualified", "Yes")
                ElseIf (SpecCase.med_off_approved = 1) Then
                    SetFormField(WWDForm, "fQualified", "Yes")
                ElseIf (SCDao.hasRWOA(SpecCase.Workflow, SpecCase.Id) And Not SpecCase.med_off_approved = 2) Then
                    SetFormField(WWDForm, "fRWOA", "Yes")
                End If
            End If
            ' Signature is always Med Tech on date they sign
            If Not IsNothing(MedTechSig) Then
                SetFormField(WWDForm, "fComments", "")  ' Comments hidden 12/26/2012 per AFRC request
                SetFormField(WWDForm, "fDatePOCSignatureDate", MedTechSig.date.ToString("dd MMM yyyy"))
                If MedTechSig.NameAndRank <> "" Then
                    sigblock = MedTechSig.NameAndRank
                    If Not String.IsNullOrEmpty(MedTechSig.Title) Then
                        sigblock = sigblock + ", " + MedTechSig.Title
                    End If
                    SetFormField(WWDForm, "fPOCSignature", sigblock)
                End If
            Else  'HQ Tech if they re-opened a WWD
                SetFormField(WWDForm, "fComments", "")  ' Comments hidden 12/26/2012 per AFRC request
                Dim sig As SignatureMetaData = SigDao.GetByUserGroup(SpecCase.Id, SpecCase.Workflow, Users.UserGroups.AFRCHQTechnician)
                If (Not sig Is Nothing) Then
                    SetFormField(WWDForm, "fDatePOCSignatureDate", sig.date.ToString("dd MMM yyyy"))
                    If sig.NameAndRank <> "" Then
                        sigblock = sig.NameAndRank
                        If sig.Title <> "" Then
                            sigblock = sigblock + ", " + sig.Title
                        End If
                        SetFormField(WWDForm, "fPOCSignature", sigblock)
                    End If
                End If
            End If

            doc.AddForm(WWDForm)

            doc.WaterMark = ""
            Return doc

        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim Id As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), Id)

            If (Id = 0) Then
                Exit Sub
            End If

            'save our id for use in other methods
            lodid = Id

            Dim doc As PDFDocument = GeneratePdf(Id)
            If (doc IsNot Nothing) Then
                doc.Render(Page.Response)
                doc.Close()
            End If
        End Sub

        Private Sub ClearFormField(ByVal doc As PDFForm, ByVal field As String)
            SetFormField(doc, field, String.Empty)
        End Sub

        Private Function IsValidSignature(ByVal signature As SignatureEntry) As Boolean
            If (signature Is Nothing) Then
                Return False
            End If

            Return signature.IsSigned
        End Function

        Private Sub SetFormField(ByVal doc As PDFForm, ByVal field As String, ByVal value As String)
            If (Not String.IsNullOrEmpty(value)) Then
                Try
                    doc.SetField(field, value)
                Catch ex As Exception

                End Try
            End If
        End Sub

    End Class

End Namespace