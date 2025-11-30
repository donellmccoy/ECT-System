Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Namespace Web.Special_Case.IRILO

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
        Private _MedTechSig As SignatureMetaData
        Private _sigDao As ISignatueMetaDateDao
        Private dao As ISpecialCaseDAO
        Private lodid As Integer
        Private sc As SC_FastTrack = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_FastTrack
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
            End Get
        End Property

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseFTWorkStatus.InitiateCase)
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

        Function GenerateAsthmaPDF(ByVal refId As Integer) As PDFForm
            Dim FTAsthmaForm As New PDFForm("FT_Asthma.pdf")
            SetFormField(FTAsthmaForm, "ft_CaseId", SpecCase.CaseId)
            SetFormField(FTAsthmaForm, "ft_CaseIdB", SpecCase.CaseId)
            If SpecCase.FastTrackType = 1 Then
                SetFormField(FTAsthmaForm, "ft_FastTrackMainType", "[MEB]  ")
                SetFormField(FTAsthmaForm, "ft_FastTrackMainTypeB", "[MEB]  ")
            Else
                If SpecCase.FastTrackType = 2 Then
                    SetFormField(FTAsthmaForm, "ft_FastTrackMainType", "[WWD]  ")
                    SetFormField(FTAsthmaForm, "ft_FastTrackMainTypeB", "[WWD]  ")
                Else
                End If
            End If
            SetFormField(FTAsthmaForm, "ft_Date", Format(SpecCase.CreatedDate, "dd MMM yyyy"))
            SetFormField(FTAsthmaForm, "ft_PatientName", SpecCase.MemberName)
            SetFormField(FTAsthmaForm, "ft_PatientSSN", SpecCase.MemberSSN)
            SetFormField(FTAsthmaForm, "ft_PatientRank", SpecCase.MemberRank.Rank)  'Might be MemberRank.Grade
            If SpecCase.SuitableDAFSC.HasValue Then
                If SpecCase.SuitableDAFSC = 1 Then
                    SetFormField(FTAsthmaForm, "ft_DAFSC", SpecCase.DAFSC & " [Suitable]")
                Else
                    SetFormField(FTAsthmaForm, "ft_DAFSC", SpecCase.DAFSC)
                End If
            Else
                SetFormField(FTAsthmaForm, "ft_DAFSC", SpecCase.DAFSC)
            End If
            If SpecCase.YearsSatisfactoryService.HasValue Then
                Select Case SpecCase.YearsSatisfactoryService
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_YrsSatSvcOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_YrsSatSvcOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_YrsSatSvcOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_YrsSatSvcOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.PulmonaryFunctionTest.HasValue Then
                SetYNValue(SpecCase.PulmonaryFunctionTest, FTAsthmaForm, "ft_PFT_Y", "ft_PFT_N")
            End If
            If SpecCase.MethacholineChallenge.HasValue Then
                Select Case SpecCase.MethacholineChallenge
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_MethaChallengeOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_MethaChallengeOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_MethaChallengeOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RequiresDailySteroids.HasValue Then
                SetYNValue(SpecCase.RequiresDailySteroids, FTAsthmaForm, "ft_DailySteroidsY", "ft_DailySteroidsN")
            End If
            SetFormField(FTAsthmaForm, "ft_DailySteroidsDosage", SpecCase.DailySteroidDosage)
            If SpecCase.RescueInhalerUsageFrequency.HasValue Then
                Select Case SpecCase.RescueInhalerUsageFrequency
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_InhalerUsageFrequencyOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_InhalerUsageFrequencyOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_InhalerUsageFrequencyOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_InhalerUsageFrequencyOpt4", xmark)
                    Case 5
                        SetFormField(FTAsthmaForm, "ft_InhalerUsageFrequencyOpt5", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.SymptomsExacerbatedByColdExercise.HasValue Then
                SetYNValue(SpecCase.SymptomsExacerbatedByColdExercise, FTAsthmaForm, "ft_SymptomsExacerbatedByExerciseY", "ft_SymptomsExacerbatedByExerciseN")
            End If
            SetFormField(FTAsthmaForm, "ft_SymptomsExacerbatedByExerciseDescription", SpecCase.ExerciseColdSymptomDescription)
            If SpecCase.NormalPFTwithTreatment.HasValue Then
                SetYNValue(SpecCase.NormalPFTwithTreatment, FTAsthmaForm, "ft_PFTWithTreatmentY", "ft_PFTWithTreatmentN")
            End If
            If SpecCase.RequiresSpecialistForMgmt.HasValue Then
                Select Case SpecCase.RequiresSpecialistForMgmt
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_RequiresSpecialistForMgmtOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_RequiresSpecialistForMgmtOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_RequiresSpecialistForMgmtOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_RequiresSpecialistForMgmtOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.MissedWorkDays.HasValue Then
                Select Case SpecCase.MissedWorkDays
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_MissedWorkDaysOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_MissedWorkDaysOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_MissedWorkDaysOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_MissedWorkDaysOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.ExacerbatedSymptomsRequireOralSteroids.HasValue Then
                SetYNValue(SpecCase.ExacerbatedSymptomsRequireOralSteroids, FTAsthmaForm, "ft_ESReqOralSteroidsY", "ft_ESReqOralSteroidsN")
            End If
            SetFormField(FTAsthmaForm, "ft_ESReqOralSteroidsDosage", SpecCase.ExacerbatedSymptomsOralSteroidsDosage)
            If SpecCase.HOIntubation.HasValue Then
                SetYNValue(SpecCase.HOIntubation, FTAsthmaForm, "ft_HOIntubationY", "ft_HOIntubationN")
            End If
            If SpecCase.HasHadERorUrgentCareVisits.HasValue Then
                SetYNValue(SpecCase.HasHadERorUrgentCareVisits, FTAsthmaForm, "ft_ERUrgentCareVisitsY", "ft_ERUrgentCareVisitsN")
            End If
            SetFormField(FTAsthmaForm, "ft_ERUrgentCareVisitsDetails", SpecCase.ERorUrgentCareVisitList)
            If SpecCase.HasBeenHospitalized.HasValue Then
                SetYNValue(SpecCase.HasBeenHospitalized, FTAsthmaForm, "ft_HadHospitalizationsY", "ft_HadHospitalizationsN")
            End If
            SetFormField(FTAsthmaForm, "ft_HospitalizationList", SpecCase.HospitalizationList)
            If SpecCase.RiskForSuddenIncapacitation.HasValue Then
                Select Case SpecCase.RiskForSuddenIncapacitation
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_RiskIncapacitationOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_RiskIncapacitationOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_RiskIncapacitationOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RecommendedFollowUpInterval.HasValue Then
                Select Case SpecCase.RecommendedFollowUpInterval
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_RecommendedIntervalOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_RecommendedIntervalOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_RecommendedIntervalOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_RecommendedIntervalOpt4", xmark)
                    Case 5
                        SetFormField(FTAsthmaForm, "ft_RecommendedIntervalOpt5", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.DAWGRecommendation.HasValue Then
                Select Case SpecCase.DAWGRecommendation
                    Case 1
                        SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt1", xmark)
                    Case 2
                        SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt2", xmark)
                    Case 3
                        SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt3", xmark)
                    Case 4
                        SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt4", xmark)
                    Case 5
                        SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt5", xmark)
                        If sc.FastTrackType = 1 Then
                            SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt5Description", "MEB")
                        Else
                            SetFormField(FTAsthmaForm, "ft_DWAGRecommendationOpt5Description", "WWD")
                        End If
                    Case Else
                        'Do Nothing
                End Select
            End If

            Dim sigblock As String = ""
            If Not IsNothing(MedTechSig) Then
                If MedTechSig.NameAndRank <> "" Then
                    sigblock = MedTechSig.NameAndRank
                    If Not String.IsNullOrEmpty(MedTechSig.Title) Then
                        sigblock = sigblock + ", " + MedTechSig.Title
                    End If
                    sigblock = sigblock & vbCrLf & MedTechSig.date.ToString("dd MMM yyyy")
                End If
                SetFormField(FTAsthmaForm, "ft_SignatureBlock", sigblock)
            End If

            Return FTAsthmaForm
        End Function

        Function GenerateDiabetesPDF(ByVal refId As Integer) As PDFForm
            Dim FTDiabetesForm As New PDFForm("FT_Diabetes.pdf")
            SetFormField(FTDiabetesForm, "ft_CaseId", SpecCase.CaseId)
            SetFormField(FTDiabetesForm, "ft_CaseIdB", SpecCase.CaseId)
            If SpecCase.FastTrackType = 1 Then
                SetFormField(FTDiabetesForm, "ft_FastTrackMainType", "[MEB]  ")
                SetFormField(FTDiabetesForm, "ft_FastTrackMainTypeB", "[MEB]  ")
            Else
                If SpecCase.FastTrackType = 2 Then
                    SetFormField(FTDiabetesForm, "ft_FastTrackMainType", "[WWD]  ")
                    SetFormField(FTDiabetesForm, "ft_FastTrackMainTypeB", "[WWD]  ")
                Else
                End If
            End If
            SetFormField(FTDiabetesForm, "ft_Date", Format(SpecCase.CreatedDate, "dd MMM yyyy"))
            SetFormField(FTDiabetesForm, "ft_PatientName", SpecCase.MemberName)
            SetFormField(FTDiabetesForm, "ft_PatientSSN", SpecCase.MemberSSN)
            SetFormField(FTDiabetesForm, "ft_PatientRank", SpecCase.MemberRank.Rank)  'Might be MemberRank.Grade
            If SpecCase.SuitableDAFSC.HasValue Then
                If SpecCase.SuitableDAFSC = 1 Then
                    SetFormField(FTDiabetesForm, "ft_DAFSC", SpecCase.DAFSC & " [Suitable]")
                Else
                    SetFormField(FTDiabetesForm, "ft_DAFSC", SpecCase.DAFSC)
                End If
            Else
                SetFormField(FTDiabetesForm, "ft_DAFSC", SpecCase.DAFSC)
            End If
            If SpecCase.YearsSatisfactoryService.HasValue Then
                Select Case SpecCase.YearsSatisfactoryService
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_YrsSatSvcOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_YrsSatSvcOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_YrsSatSvcOpt3", xmark)
                    Case 4
                        SetFormField(FTDiabetesForm, "ft_YrsSatSvcOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.FastingBloodSugar.HasValue Then
                Select Case SpecCase.FastingBloodSugar
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_FastingBloodSugarOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_FastingBloodSugarOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_FastingBloodSugarOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.HgbA1C.HasValue Then
                Select Case SpecCase.HgbA1C
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_HgbA1COpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_HgbA1COpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_HgbA1COpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.BodyMassIndex.HasValue Then
                SetFormField(FTDiabetesForm, "ft_BMI", SpecCase.BodyMassIndex)
            End If
            If SpecCase.CurrentOptometryExam.HasValue Then
                SetYNValue(SpecCase.CurrentOptometryExam, FTDiabetesForm, "ft_OptometryExamCurrentY", "ft_OptometryExamCurrentN")
            End If
            If SpecCase.HasOtherSignificantConditions.HasValue Then
                SetYNValue(SpecCase.HasOtherSignificantConditions, FTDiabetesForm, "ft_HasSignificantConditionsY", "ft_HasSignificantConditionsN")
            End If
            SetFormField(FTDiabetesForm, "ft_SignificantConditionList", SpecCase.OtherSignificantConditionsList)
            If SpecCase.ControlledWithOralAgents.HasValue Then
                SetYNValue(SpecCase.ControlledWithOralAgents, FTDiabetesForm, "ft_ControlledByOralAgentsY", "ft_ControlledByOralAgentsN")
            End If
            SetFormField(FTDiabetesForm, "ft_OralAgentsList", SpecCase.OralAgentsList)
            If SpecCase.RequiresInsulin.HasValue Then
                SetYNValue(SpecCase.RequiresInsulin, FTDiabetesForm, "ft_RequiresInsulinY", "ft_RequiresInsulinN")
            End If
            SetFormField(FTDiabetesForm, "ft_InsulinDosageRegime", SpecCase.InsulinDosageRegime)
            If SpecCase.RequiresNonInsulinMed.HasValue Then
                SetYNValue(SpecCase.RequiresNonInsulinMed, FTDiabetesForm, "ft_RequiresNonInsulinMedY", "ft_RequiresNonInsulinMedN")
            End If
            If SpecCase.HasHadERorUrgentCareVisits.HasValue Then
                SetYNValue(SpecCase.HasHadERorUrgentCareVisits, FTDiabetesForm, "ft_ERUrgentCareVisitsY", "ft_ERUrgentCareVisitsN")
            End If
            SetFormField(FTDiabetesForm, "ft_ERUrgentCareVisitsDetails", SpecCase.ERorUrgentCareVisitList)
            If SpecCase.HasBeenHospitalized.HasValue Then
                SetYNValue(SpecCase.HasBeenHospitalized, FTDiabetesForm, "ft_HadHospitalizationsY", "ft_HadHospitalizationsN")
            End If
            SetFormField(FTDiabetesForm, "ft_HospitalizationList", SpecCase.HospitalizationList)
            If SpecCase.RequiresSpecialistForMgmt.HasValue Then
                Select Case SpecCase.RequiresSpecialistForMgmt
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_RequiresSpecialistForMgmtOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_RequiresSpecialistForMgmtOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_RequiresSpecialistForMgmtOpt3", xmark)
                    Case 4
                        SetFormField(FTDiabetesForm, "ft_RequiresSpecialistForMgmtOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.MissedWorkDays.HasValue Then
                Select Case SpecCase.MissedWorkDays
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_MissedWorkDaysOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_MissedWorkDaysOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_MissedWorkDaysOpt3", xmark)
                    Case 4
                        SetFormField(FTDiabetesForm, "ft_MissedWorkDaysOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RiskForSuddenIncapacitation.HasValue Then
                Select Case SpecCase.RiskForSuddenIncapacitation
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_RiskIncapacitationOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_RiskIncapacitationOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_RiskIncapacitationOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RecommendedFollowUpInterval.HasValue Then
                Select Case SpecCase.RecommendedFollowUpInterval
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_RecommendedIntervalOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_RecommendedIntervalOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_RecommendedIntervalOpt3", xmark)
                    Case 4
                        SetFormField(FTDiabetesForm, "ft_RecommendedIntervalOpt4", xmark)
                    Case 5
                        SetFormField(FTDiabetesForm, "ft_RecommendedIntervalOpt5", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.DAWGRecommendation.HasValue Then
                Select Case SpecCase.DAWGRecommendation
                    Case 1
                        SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt1", xmark)
                    Case 2
                        SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt2", xmark)
                    Case 3
                        SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt3", xmark)
                    Case 4
                        SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt4", xmark)
                    Case 5
                        SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt5", xmark)
                        If sc.FastTrackType = 1 Then
                            SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt5Description", "MEB")
                        Else
                            SetFormField(FTDiabetesForm, "ft_DWAGRecommendationOpt5Description", "WWD")
                        End If
                    Case Else
                        'Do Nothing
                End Select
            End If

            Dim sigblock As String = ""

            If Not IsNothing(MedTechSig) Then
                If MedTechSig.NameAndRank <> "" Then
                    sigblock = MedTechSig.NameAndRank
                    If Not String.IsNullOrEmpty(MedTechSig.Title) Then
                        sigblock = sigblock + ", " + MedTechSig.Title
                    End If
                    sigblock = sigblock & vbCrLf & MedTechSig.date.ToString("dd MMM yyyy")
                End If
                SetFormField(FTDiabetesForm, "ft_SignatureBlock", sigblock)
            End If

            Return FTDiabetesForm
        End Function

        Function GenerateGeneralPDF(ByVal refId As Integer) As PDFForm
            Dim FTGeneralForm As New PDFForm(PrintDocuments.GeneralIRILO)
            SetFormField(FTGeneralForm, "ft_CaseId", SpecCase.CaseId)
            If SpecCase.FastTrackType = 1 Then
                SetFormField(FTGeneralForm, "ft_FastTrackMainType", "[MEB]  ")
                SetFormField(FTGeneralForm, "ft_FastTrackMainTypeB", "[MEB]  ")
            Else
                If SpecCase.FastTrackType = 2 Then
                    SetFormField(FTGeneralForm, "ft_FastTrackMainType", "[WWD]  ")
                    SetFormField(FTGeneralForm, "ft_FastTrackMainTypeB", "[WWD]  ")
                Else
                End If
            End If
            SetFormField(FTGeneralForm, "ft_Date", Format(SpecCase.CreatedDate, "dd MMM yyyy"))
            SetFormField(FTGeneralForm, "ft_PatientName", SpecCase.MemberName)
            SetFormField(FTGeneralForm, "ft_PatientSSN", SpecCase.MemberSSN)
            SetFormField(FTGeneralForm, "ft_PatientRank", SpecCase.MemberRank.Rank)  'Might be MemberRank.Grade
            If SpecCase.SuitableDAFSC.HasValue Then
                If SpecCase.SuitableDAFSC = 1 Then
                    SetFormField(FTGeneralForm, "ft_DAFSC", SpecCase.DAFSC & " [Suitable]")
                Else
                    SetFormField(FTGeneralForm, "ft_DAFSC", SpecCase.DAFSC)
                End If
            Else
                SetFormField(FTGeneralForm, "ft_DAFSC", SpecCase.DAFSC)
            End If
            If SpecCase.YearsSatisfactoryService.HasValue Then
                Select Case SpecCase.YearsSatisfactoryService
                    Case 1
                        SetFormField(FTGeneralForm, "ft_YrsSatSvcOpt1", xmark)
                    Case 2
                        SetFormField(FTGeneralForm, "ft_YrsSatSvcOpt2", xmark)
                    Case 3
                        SetFormField(FTGeneralForm, "ft_YrsSatSvcOpt3", xmark)
                    Case 4
                        SetFormField(FTGeneralForm, "ft_YrsSatSvcOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            Dim diagnosis As String
            diagnosis = SpecCase.ICD9Description & vbCrLf & vbCrLf & SpecCase.FTDiagnosis
            SetFormField(FTGeneralForm, "ft_Diagnosis", diagnosis)
            If SpecCase.DxInterferenceWithDuties.HasValue Then
                SetYNValue(SpecCase.DxInterferenceWithDuties, FTGeneralForm, "ft_InGarrisonDutiesY", "ft_InGarrisonDutiesN")
            End If
            SetFormField(FTGeneralForm, "ft_Prognosis", SpecCase.FTPrognosis)
            SetFormField(FTGeneralForm, "ft_OngoingTreatment", SpecCase.FTTreatment)
            SetFormField(FTGeneralForm, "ft_Dosage", SpecCase.FTMedicationsAndDosages)
            If SpecCase.RiskForSuddenIncapacitation.HasValue Then
                Select Case SpecCase.RiskForSuddenIncapacitation
                    Case 1
                        SetFormField(FTGeneralForm, "ft_RiskIncapacitationOpt1", xmark)
                    Case 2
                        SetFormField(FTGeneralForm, "ft_RiskIncapacitationOpt2", xmark)
                    Case 3
                        SetFormField(FTGeneralForm, "ft_RiskIncapacitationOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RecommendedFollowUpInterval.HasValue Then
                Select Case SpecCase.RecommendedFollowUpInterval
                    Case 1
                        SetFormField(FTGeneralForm, "ft_RecommendedIntervalOpt1", xmark)
                    Case 2
                        SetFormField(FTGeneralForm, "ft_RecommendedIntervalOpt2", xmark)
                    Case 3
                        SetFormField(FTGeneralForm, "ft_RecommendedIntervalOpt3", xmark)
                    Case 4
                        SetFormField(FTGeneralForm, "ft_RecommendedIntervalOpt4", xmark)
                    Case 5
                        SetFormField(FTGeneralForm, "ft_RecommendedIntervalOpt5", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.DAWGRecommendation.HasValue Then
                Select Case SpecCase.DAWGRecommendation
                    Case 1
                        SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt1", xmark)
                    Case 2
                        SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt2", xmark)
                    Case 3
                        SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt3", xmark)
                    Case 4
                        SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt4", xmark)
                    Case 5
                        SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt5", xmark)
                        If sc.FastTrackType = 1 Then
                            SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt5Description", "MEB")
                        Else
                            SetFormField(FTGeneralForm, "ft_DWAGRecommendationOpt5Description", "WWD")
                        End If
                    Case Else
                        'Do Nothing
                End Select
            End If

            Dim sigblock As String = ""
            If Not IsNothing(MedTechSig) Then
                If MedTechSig.NameAndRank <> "" Then
                    sigblock = MedTechSig.NameAndRank
                    If Not String.IsNullOrEmpty(MedTechSig.Title) Then
                        sigblock = sigblock + ", " + MedTechSig.Title
                    End If
                    sigblock = sigblock & vbCrLf & MedTechSig.date.ToString("dd MMM yyyy")
                End If
                SetFormField(FTGeneralForm, "ft_SignatureBlock", sigblock)
            End If

            Return FTGeneralForm
        End Function

        Function GenerateSleepPDF(ByVal refId As Integer) As PDFForm
            Dim FTSleepForm As New PDFForm("FT_SleepApnea.pdf")
            SetFormField(FTSleepForm, "ft_CaseId", SpecCase.CaseId)
            SetFormField(FTSleepForm, "ft_CaseIdB", SpecCase.CaseId)
            If SpecCase.FastTrackType = 1 Then
                SetFormField(FTSleepForm, "ft_FastTrackMainType", "[MEB]  ")
                SetFormField(FTSleepForm, "ft_FastTrackMainTypeB", "[MEB]  ")
            Else
                If SpecCase.FastTrackType = 2 Then
                    SetFormField(FTSleepForm, "ft_FastTrackMainType", "[WWD]  ")
                    SetFormField(FTSleepForm, "ft_FastTrackMainTypeB", "[WWD]  ")
                Else
                End If
            End If
            SetFormField(FTSleepForm, "ft_Date", Format(SpecCase.CreatedDate, "dd MMM yyyy"))
            SetFormField(FTSleepForm, "ft_PatientName", SpecCase.MemberName)
            SetFormField(FTSleepForm, "ft_PatientSSN", SpecCase.MemberSSN)
            SetFormField(FTSleepForm, "ft_PatientRank", SpecCase.MemberRank.Rank)  'Might be MemberRank.Grade
            If SpecCase.SuitableDAFSC.HasValue Then
                If SpecCase.SuitableDAFSC = 1 Then
                    SetFormField(FTSleepForm, "ft_DAFSC", SpecCase.DAFSC & " [Suitable]")
                Else
                    SetFormField(FTSleepForm, "ft_DAFSC", SpecCase.DAFSC)
                End If
            Else
                SetFormField(FTSleepForm, "ft_DAFSC", SpecCase.DAFSC)
            End If
            If SpecCase.YearsSatisfactoryService.HasValue Then
                Select Case SpecCase.YearsSatisfactoryService
                    Case 1
                        SetFormField(FTSleepForm, "ft_YrsSatSvcOpt1", xmark)
                    Case 2
                        SetFormField(FTSleepForm, "ft_YrsSatSvcOpt2", xmark)
                    Case 3
                        SetFormField(FTSleepForm, "ft_YrsSatSvcOpt3", xmark)
                    Case 4
                        SetFormField(FTSleepForm, "ft_YrsSatSvcOpt4", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.DaytimeSomnolence.HasValue Then
                SetYNValue(SpecCase.DaytimeSomnolence, FTSleepForm, "ft_DaytimeSomnolenceY", "ft_DaytimeSomnolenceN")
            End If
            SetFormField(FTSleepForm, "ft_DaySleepDescription", SpecCase.DaySleepDescription)
            If SpecCase.HasApneaEpisodes.HasValue Then
                SetYNValue(SpecCase.HasApneaEpisodes, FTSleepForm, "ft_HasApneaY", "ft_HasApneaN")
            End If
            SetFormField(FTSleepForm, "ft_ApneaDescription", SpecCase.ApneaEpisodeDescription)
            If SpecCase.SleepStudyResults.HasValue Then
                SetYNValue(SpecCase.SleepStudyResults, FTSleepForm, "ft_SleepStudyResultsY", "ft_SleepStudyResultsN")
            End If
            If SpecCase.BodyMassIndex.HasValue Then
                SetFormField(FTSleepForm, "ft_BMI", SpecCase.BodyMassIndex)
            End If
            If SpecCase.OralDevicesUsed.HasValue Then
                SetYNValue(SpecCase.OralDevicesUsed, FTSleepForm, "ft_OralDeviceUsedY", "ft_OralDeviceUsedN")
            End If
            If SpecCase.CPAPRequired.HasValue Then
                SetYNValue(SpecCase.CPAPRequired, FTSleepForm, "ft_CPAPRequiredY", "ft_CPAPRequiredN")
            End If
            If SpecCase.BIPAPRequired.HasValue Then
                SetYNValue(SpecCase.BIPAPRequired, FTSleepForm, "ft_BIPAPRequiredY", "ft_BIPAPRequiredN")
            End If
            If SpecCase.ResponseToDevices.HasValue Then
                SetYNValue(SpecCase.ResponseToDevices, FTSleepForm, "ft_ResponseToDevicesY", "ft_ResponseToDevicesN")
            End If
            If SpecCase.RiskForSuddenIncapacitation.HasValue Then
                Select Case SpecCase.RiskForSuddenIncapacitation
                    Case 1
                        SetFormField(FTSleepForm, "ft_RiskIncapacitationOpt1", xmark)
                    Case 2
                        SetFormField(FTSleepForm, "ft_RiskIncapacitationOpt2", xmark)
                    Case 3
                        SetFormField(FTSleepForm, "ft_RiskIncapacitationOpt3", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.RecommendedFollowUpInterval.HasValue Then
                Select Case SpecCase.RecommendedFollowUpInterval
                    Case 1
                        SetFormField(FTSleepForm, "ft_RecommendedIntervalOpt1", xmark)
                    Case 2
                        SetFormField(FTSleepForm, "ft_RecommendedIntervalOpt2", xmark)
                    Case 3
                        SetFormField(FTSleepForm, "ft_RecommendedIntervalOpt3", xmark)
                    Case 4
                        SetFormField(FTSleepForm, "ft_RecommendedIntervalOpt4", xmark)
                    Case 5
                        SetFormField(FTSleepForm, "ft_RecommendedIntervalOpt5", xmark)
                    Case Else
                        'Do Nothing
                End Select
            End If
            If SpecCase.DAWGRecommendation.HasValue Then
                Select Case SpecCase.DAWGRecommendation
                    Case 1
                        SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt1", xmark)
                    Case 2
                        SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt2", xmark)
                    Case 3
                        SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt3", xmark)
                    Case 4
                        SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt4", xmark)
                    Case 5
                        SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt5", xmark)
                        If sc.FastTrackType = 1 Then
                            SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt5Description", "MEB")
                        Else
                            SetFormField(FTSleepForm, "ft_DWAGRecommendationOpt5Description", "WWD")
                        End If
                    Case Else
                        'Do Nothing
                End Select
            End If

            Dim sigblock As String = ""
            If Not IsNothing(MedTechSig) Then
                If MedTechSig.NameAndRank <> "" Then
                    sigblock = MedTechSig.NameAndRank
                    If Not String.IsNullOrEmpty(MedTechSig.Title) Then
                        sigblock = sigblock + ", " + MedTechSig.Title
                    End If
                    sigblock = sigblock & vbCrLf & MedTechSig.date.ToString("dd MMM yyyy")
                End If
                SetFormField(FTSleepForm, "ft_SignatureBlock", sigblock)
            End If

            Return FTSleepForm
        End Function

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

            Dim dateSigned As Date = Format(signature.DateSigned.Value, "dd MMM yyyy")

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

            Dim FTForm As PDFForm
            Select Case SpecCase.ICD9Code
                Case 251 'Diabetes  -- 250
                    FTForm = DirectCast(GenerateDiabetesPDF(refId), PDFForm)
                Case 1045 'Sleep Apnea  -- 327
                    FTForm = GenerateSleepPDF(refId)
                Case 498 'Asthma  -- 493
                    FTForm = GenerateAsthmaPDF(refId)
                Case Else
                    FTForm = GenerateGeneralPDF(refId)
            End Select

            doc.AddForm(FTForm)

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