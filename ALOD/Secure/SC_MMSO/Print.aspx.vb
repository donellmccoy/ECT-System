Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Namespace Web.Special_Case.MMSO

    Partial Class Secure_lod_Print
        Inherits System.Web.UI.Page

        Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
        Const xmark As String = "Yes"
        Private dao As ISpecialCaseDAO
        Private lodid As Integer
        Private MMSOForm As PDFForm
        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0
        Private signatureService As DBSignService
        Dim type As ModuleType

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
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
        ''' Adds a digital signature to a form
        ''' </summary>
        ''' <param name="signature">The Signature entry to add to the form</param>
        ''' <param name="sigField">the name of the signature field on the form</param>
        ''' <param name="nameField">the name of the "printed name" field on the form</param>
        ''' <param name="dateField">the name of the date field on the form</param>
        ''' <param name="template">The DBSign template used for this signature</param>
        ''' <remarks>
        ''' </remarks>
        Protected Function AddSignatureToForm(ByVal signature As SignatureMetaData, ByVal sigField As String,
                                              ByVal nameField As String,
                                              ByVal dateField As String,
                                              ByVal template As DBSignTemplateId) As Boolean
            If (signature Is Nothing) Then
                SetFormField(sigField, String.Empty)
                Return False
            End If

            VerifySource = New DBSignService(template, lodid, 0)

            Dim valid As Boolean = False

            Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()

            If (signatureStatus = DBSignResult.SignatureValid) Then
                'if it's valid, add the signing info to the form
                Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()
                SetFormField(sigField, "Digitally signed by " + signInfo.Signature)
                SetFormField(nameField, signature.NameAndRank)
                SetFormField(dateField, signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT))
                valid = True
            Else
                'otherwise, clear those fields
                SetFormField(sigField, String.Empty)
                SetFormField(nameField, String.Empty)
                SetFormField(dateField, String.Empty)
                valid = False
            End If
            Return valid
        End Function

        Protected Function GeneratePdf(ByVal refId As Integer) As PDFDocument
            Dim strpath As String = Page.ResolveClientUrl("~/secure/documents/")
            Dim doc As New PDFDocument

            MMSOForm = New PDFForm("MMSO_Form.pdf")

            SetFormField("fCase_Id", SpecCase.CaseId)
            '---------------Section I---------------
            '---------------Line/Box 1   ---------------
            '        fUSAR(-CheckBox)
            '        fUSNR(-CheckBox)
            '        fUSMCR(-CheckBox)
            'fUSAFR - CheckBox <-- Default Value?
            SetFormField("fUSAFR", xmark)
            '        fARNG(-CheckBox)
            '        fANG(-CheckBox)
            '        fUSCGR(-CheckBox)
            '---------------Line/Box 2   ---------------
            'fMemberName    - Text Field
            SetFormField("fMemberName", SpecCase.MemberName)
            '---------------Line/Box 3   ---------------
            'fMemberRank    - Text Field
            SetFormField("fMemberRank", SpecCase.MemberRank.Rank)
            '---------------Line/Box 4   ---------------
            'fMemberSSN     - Text Field
            SetFormField("fMemberSSN", SpecCase.MemberSSN)
            '---------------Line/Box 5   ---------------
            'fMemberAddress - Text Field
            SetFormField("fMemberAddress", SpecCase.Member_Address_Street + Environment.NewLine +
                         SpecCase.Member_Address_City + ", " + SpecCase.Member_Address_State + "  " + SpecCase.Member_Address_Zip)
            '---------------Line/Box 6   ---------------
            'fMemberDOB     - Text Field
            SetFormDateField("fMemberDOB", SpecCase.MemberDOB)
            '---------------Line/Box 7   ---------------
            'fMemberPhone   - Text Field
            SetFormField("fMemberPhone", SpecCase.Member_Home_Phone)
            '---------------Line/Box 8   ---------------
            '        fTRICARENorth(-CheckBox)
            '        fTRICARESouth(-CheckBox)
            '        fTRICAREWest(-CheckBox)
            If SpecCase.Member_Tricare_Region.HasValue Then
                Select Case SpecCase.Member_Tricare_Region.Value
                    Case 0
                        SetFormField("fTRICARENorth", xmark)
                    Case 1
                        SetFormField("fTRICARESouth", xmark)
                    Case 2
                        SetFormField("fTRICAREWest", xmark)
                End Select
            End If

            'Section 2

            '---------------Section II---------------
            '---------------Line/Box 9   ---------------
            SetFormDateField("fDateOfIllness", SpecCase.InjuryIllnessDate)
            '---------------Line/Box 10  ---------------
            'fDutyDateStart - Text Field
            SetFormDateField("fDutyDateStart", SpecCase.DateIn)
            'fDutyDateEnd   - Text Field
            SetFormDateField("fDutyDateEnd", SpecCase.DateOut)
            '---------------Line/Box 11  ---------------
            'fICD9Info               - Text Field
            SetFormField("fICD9Info", SpecCase.ICD9Id + Environment.NewLine + SpecCase.Medical_Diagnosis)
            'fDocumentsSubmittedDate - Text Field
            '---------------Line/Box 12  ---------------
            '        fLOD(-CheckBox)
            SetFormField("fLOD", xmark)
            '        fORDERS(-CheckBox)
            '---------------Line/Box 13  ---------------
            'fFollowUp - Text Field
            SetFormField("fFollowUp", SpecCase.Follow_Up_Care)
            '---------------Line/Box 14  ---------------
            'fProviderName - Text Field
            SetFormField("fProviderName", SpecCase.Medical_Provider)
            '---------------Line/Box 15  ---------------
            'fProviderContactInfo - Text Field
            SetFormField("fProviderContactInfo", SpecCase.Provider_POC + " " + SpecCase.Provider_POC_Phone)
            '---------------Line/Box 15  ---------------
            'fMTFInfo - Text Field
            SetFormField("fMTFInfo", SpecCase.MTF_Initial_Treatment_Date.Value.ToString("yyMMdd") + ", " + SpecCase.Military_Treatment_Facility_Initial)
            '---------------Line/Box 16  ---------------
            'fProfileInfo - Text Field
            SetFormField("fProfileInfo", SpecCase.Medical_Profile_Info)

            '---------------Section III---------------
            '---------------Line/Box 17  ---------------
            'fMTFNAme      - Text Field
            SetFormField("fMTFNAme", SpecCase.Military_Treatment_Facility_Suggested)
            'fMTFMiles     - Text Field
            SetFormIntField("fMTFMiles", SpecCase.MTF_Suggested_Distance)
            '        fPlaceOfDuty(-CheckBox)
            '        fResidence(-CheckBox)
            If SpecCase.MTF_Suggested_Choice.HasValue Then
                If SpecCase.MTF_Suggested_Choice.Value = 1 Then
                    SetFormField("fPlaceOfDuty", xmark)
                ElseIf SpecCase.MTF_Suggested_Choice = 2 Then
                    SetFormField("fResidence", xmark)
                End If
            End If
            '---------------Line/Box 18  ---------------
            'fUnitInfo     - Text Field
            If SpecCase.Unit_Address2 <> "" Then
                SetFormField("fUnitInfo", SpecCase.UnitOfAssignment + Environment.NewLine + SpecCase.Unit_Address1 + Environment.NewLine +
                            SpecCase.Unit_Address2 + Environment.NewLine + SpecCase.Unit_City + ", " + SpecCase.Unit_State + "  " + SpecCase.Unit_Zip)
            Else
                SetFormField("fUnitInfo", SpecCase.UnitOfAssignment + Environment.NewLine + SpecCase.Unit_Address1 + Environment.NewLine +
                            SpecCase.Unit_City + ", " + SpecCase.Unit_State + "  " + SpecCase.Unit_Zip)
            End If
            '---------------Line/Box 18/A---------------
            'fUnitUIC      - Text Field
            SetFormField("fUnitUIC", SpecCase.Unit_UIC)
            '---------------Line/Box 19  ---------------
            'fUnitPOCInfo  - Text Field
            If Not String.IsNullOrEmpty(SpecCase.Unit_POC_name) Then
                Dim memRank As String = "no rank"
                If SpecCase.Unit_POC_rank.HasValue Then
                    Dim ranks As List(Of UserRank) = Services.LookupService.GetRanksAndGrades()
                    memRank = (From r In ranks Where r.Id = SpecCase.Unit_POC_rank.Value Select r.Title).SingleOrDefault()
                End If
                SetFormField("fUnitPOCInfo", SpecCase.Unit_POC_name + ", " + memRank + ", " + SpecCase.Unit_POC_title)
            End If
            '---------------Line/Box 19/A---------------
            'fUnitPOCPhone - Text Field
            SetFormField("fUnitPOCPhone", SpecCase.Unit_POC_Phone)
            '---------------Line/Box 20  ---------------
            'fCertDigitalSignature - Text Field
            'fCertPrintedName      - Text Field
            'fCertSignedDate       - Text Field
            Dim sigDao As SignatureMetaDataDao = New NHibernateDaoFactory().GetSigMetaDataDao()

            AddSignatureToForm(sigDao.GetByUserGroup(SpecCase.Id, SpecCase.Workflow, CInt(UserGroups.AFRCHQTechnician)), "fCertDigitalSignature", "fCertPrintedName", "fCertSignedDate", DBSignTemplateId.MMSO_Unit)
            doc.AddForm(MMSOForm)
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

        Private Sub ClearFormField(ByVal field As String)
            SetFormField(field, String.Empty)
        End Sub

        Private Function IsValidSignature(ByVal signature As SignatureEntry) As Boolean
            If (signature Is Nothing) Then
                Return False
            End If

            Return signature.IsSigned
        End Function

        Private Sub SetFormDateField(ByVal field As String, ByVal dateval As Date?)
            If dateval.HasValue Then
                MMSOForm.SetField(field, dateval.Value.ToString("yyMMdd"))
            End If
        End Sub

        Private Sub SetFormField(ByVal field As String, ByVal value As String)
            If (Not String.IsNullOrEmpty(value)) Then
                MMSOForm.SetField(field, value)
            End If
        End Sub

        Private Sub SetFormIntField(ByVal field As String, ByVal fieldval As Integer?)
            If fieldval.HasValue Then
                MMSOForm.SetField(field, fieldval.Value.ToString())
            End If
        End Sub

    End Class

End Namespace