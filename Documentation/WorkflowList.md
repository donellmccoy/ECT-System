### List of All Workflows in the ECTSystem/ALOD Application

The system organizes its workflows around **modules** and **case types**, primarily defined in enums like `AFRCWorkflows` and `ModuleType` (from `ALOD.Core/Domain/Workflow/WorkflowEnums.cs`). These represent distinct processes for handling Line of Duty (LOD) investigations, appeals, reinvestigations, Sexual Assault Response Coordinator (SARC) cases, and various Special Cases.

The workflows are divided into two main categories:
- **AFRC (Air Force Reserve Command)** workflows
- **ANG (Air National Guard)** workflows (which mirror many AFRC ones but with component-specific handling)

There are **40+ workflow types** in total, including variants. Each workflow typically involves status transitions, document handling, approvals, and notifications. Below is a comprehensive list, grouped by category for clarity. The enum values are included for reference.

#### **Core AFRC Workflows**
These are the primary workflows for Air Force Reserve components:
1. **LOD** (Line of Duty Investigation) - Value: 1 (Basic LOD process)
2. **LOD_v2** (Updated LOD Investigation) - Value: 27
3. **ReinvestigationRequest** (LOD Reinvestigation) - Value: 5
4. **AppealRequest** (Appeal Request) - Value: 26
5. **SARCRestricted** (Restricted SARC Case) - Value: 28
6. **SARCRestrictedAppeal** (Restricted SARC Appeal) - Value: 29

#### **AFRC Special Case Workflows**
Special Cases (SpecCase) are subtypes of workflows for specific scenarios (e.g., medical, administrative). There are 21 types:
1. **SpecCaseIncap** (Incapacitation Pay) - Value: 6
2. **SpecCaseCongress** (Congressional Inquiry) - Value: 7
3. **SpecCaseBMT** (Basic Military Training) - Value: 8
4. **SpecCaseWWD** (Worldwide Duty) - Value: 11
5. **SpecCasePW** (Profile Waiver) - Value: 12
6. **SpecCaseMEB** (Medical Evaluation Board+) - Value: 13
7. **SpecCaseBCMR** (Board for Correction of Military Records) - Value: 14
8. **SpecCaseFT** (Fast Track) - Value: 15
9. **SpecCaseCMAS** (Case Management and Adjudication System) - Value: 16
10. **SpecCaseMEPS** (Military Entrance Processing Station) - Value: 17
11. **SpecCaseMMSO** (Military Medical Support Office) - Value: 18
12. **SpecCaseMH** (Mental Health) - Value: 19
13. **SpecCaseNE** (Non-Emergency) - Value: 20
14. **SpecCaseDW** (Duty Waiver) - Value: 21
15. **SpecCaseMO** (Medical Officer) - Value: 22
16. **SpecCasePEPP** (Physical Exam Processing Program) - Value: 23
17. **SpecCaseRS** (Reserve Service) - Value: 24
18. **SpecCasePH** (Psychological Health) - Value: 25
19. **SpecCaseRW** (Reserve Waiver) - Value: 30
20. **SpecCaseAGR** (Active Guard/Reserve Certification) - Value: 31
21. **SpecCasePSCD** (Personnel Security Clearance Determination) - Value: 32

#### **ANG Workflows**
These are Air National Guard-specific variants (prefixed with ANG), starting from value 101. There are 19 types, many mirroring AFRC:
1. **ANGLOD** (ANG Line of Duty Investigation) - Value: 101
2. **ANGLOD_v2** (Updated ANG LOD Investigation) - Value: 127
3. **ANGReinvestigationRequest** (ANG LOD Reinvestigation) - Value: 105
4. **ANGAppealRequest** (ANG Appeal Request) - Value: 126
5. **ANGSARCRestricted** (ANG Restricted SARC Case) - Value: 128
6. **ANGSARCRestrictedAppeal** (ANG Restricted SARC Appeal) - Value: 129

#### **ANG Special Case Workflows**
ANG-specific Special Cases (13 types):
1. **ANGSpecCaseIncap** (ANG Incapacitation Pay) - Value: 106
2. **ANGSpecCaseCongress** (ANG Congressional Inquiry) - Value: 107
3. **ANGSpecCaseBMT** (ANG Basic Military Training) - Value: 108
4. **ANGSpecCaseWWD** (ANG Worldwide Duty) - Value: 111
5. **ANGSpecCasePW** (ANG Profile Waiver) - Value: 112
6. **ANGSpecCaseMEB** (ANG Medical Evaluation Board) - Value: 113
7. **ANGSpecCaseBCMR** (ANG Board for Correction of Military Records) - Value: 114
8. **ANGSpecCaseFT** (ANG Fast Track) - Value: 115
9. **ANGSpecCaseCMAS** (ANG Case Management and Adjudication System) - Value: 116
10. **ANGSpecCaseMEPS** (ANG Military Entrance Processing Station) - Value: 117
11. **ANGSpecCaseMMSO** (ANG Military Medical Support Office) - Value: 118
12. **ANGSpecCaseMH** (ANG Mental Health) - Value: 119
13. **ANGSpecCaseRW** (ANG Reserve Waiver) - Value: 130

#### **Additional Workflow-Related Types**
While not full workflows, these are related enums that define subtypes or actions within workflows:
- **INCAP (Incapacitation Pay) Sub-Workflows**: Includes actions like INInitiate (41), INRMU_Action_Approve (42), etc. (about 35 sub-actions for INCAP processing).
- **Workflow Action Types**: E.g., SendEmail (1), SignMemo (2), LockDocuments (4), etc. These are steps within workflows.
- **Option Rules**: Rules like Formal (1), ValidatePreviousStatus (2), etc., used for workflow validation.
- **Module Types**: A parallel enum to AFRCWorkflows, with values like SpecialCasesALL (0), System (1), etc.
- **Case Types**: From `CaseType.cs`, includes types like LOD (1), Appeal (2), Reinvestigation (3), and Special Case subtypes (e.g., Incap (4), Congress (5)).

#### **Notes on Workflows**
- **Structure**: Each workflow has associated tabs (e.g., Investigation, Medical, Unit), statuses (e.g., InitialStatus), and actions (e.g., approvals, document uploads). They are implemented in separate directories under `ALOD/Secure/` (e.g., `lod/`, `SC_FastTrack/`, `ANGlod/`).
- **ANG vs. AFRC**: ANG workflows (100+ values) are variants for Air National Guard, often with similar logic but component-specific rules.
- **Special Cases**: These are the most numerous, each with dedicated handling for unique scenarios like medical boards or waivers.
- **Integration**: All workflows integrate with document management (SRXLite), reporting, and email notifications.
- **Source**: This list is derived from enums in `WorkflowEnums.cs` and `LodEnums.cs`, cross-referenced with directory structures and domain classes.
