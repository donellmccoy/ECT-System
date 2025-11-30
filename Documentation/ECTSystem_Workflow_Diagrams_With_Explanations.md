# Comprehensive Workflow Diagrams and Explanations for ECTSystem/ALOD Application

This Markdown file provides detailed diagrams and explanations for ALL workflows in the ECTSystem, based on WorkflowList.md, Special Case Workflows.md, and codebase (e.g., `WorkflowEnums.cs`, `LodEnums.cs`, DAOs like `SpecialCaseDao.cs`). Each includes an ASCII diagram and step-by-step explanations with roles (from `UserEnums.cs` and complete list including missing ones like RMU or MPF), their actions, and references (e.g., "Line of Duty Documentation.pdf" for LOD per AFI 36-2910; training transcript for tabs/actions).

Workflows are grouped by category. ANG variants mirror AFRC with adjustments (e.g., ANG Tech instead of Med Tech; appeals to ANGRC). Explanations detail role involvement at each stage, inferred from permissions (`PageAccessDao.cs`), statuses, and docs.

## Core AFRC Workflows

### Informal LOD Process
Tailored for simple admin cases (e.g., Minor Injury, non-complex Disease; Formal=false in LodEnums.cs).

```
Medical Technician (Initiation) -> Medical Officer (Medical Review) -> Unit Commander (Unit Approval/Forward) -> Wing JA (Legal Review if Needed) -> Wing Commander (Closure/Endorsement) -> Board Technician (Board Review) -> [Board Medical/Board Legal/Board Approving Authority/Board Administrator] -> Complete
```

**Explanation** (Updated based on actual workflow investigation):
- **Initiation**: Primary: *Medical Technician* (ptype=1) initiates/uploads docs; Conditional (if SARC-related): *Medical Technician SARC*; Conditional (if ANG): *ANG Tech*; *Unit Commander* may endorse eligibility (PDF: Quick admin for routine cases).
- **Medical Review**: Primary: *Medical Officer* (ptype=2) reviews ICD codes (ICD9CodeDao.cs); Conditional (if pilot): *Med Officer (P)*; Conditional (if psych): *Unit PH*; *Medical Technician* supports data entry. (Roles are conditional—e.g., no RMU unless reserve-specific.)
- **Unit Approval/Forward**: Primary: *Unit Commander* (ptype=3) signs and forwards if needed (e.g., to Wing JA for concurrence); Conditional (read-only): *Unit Commander Read Only* views; *MPF* updates records. (Forwarding to Wing JA/Commander is conditional, e.g., if legal non-concurrence; base informal closes here.)
- **Legal Review if Needed**: Primary: *Wing JA* (ptype=4) provides legal check if forwarded; Conditional: *Board Legal* assists.
- **Closure/Endorsement**: Primary: *Wing Commander* (ptype=5) endorses/closes if escalated; Conditional: *LOD Program Manager* notifies/tracks; *Auditor Read Only* may view. (Conditional step; skips if no forwarding.)
- **Board Review**: Primary: *Board Technician* (ptype=6) coordinates board review process
- **Board Recycle Process**: Any Board role can forward to other Board roles:
  - *Board Medical* (ptype=8) - Medical review at board level
  - *Board Legal* (ptype=7) - Legal review at board level  
  - *Board Approving Authority* (ptype=10) - Board-level approving authority
  - *Board Administrator* (ptype=22) - Board administrative functions
  - *Board Technician* (ptype=6)
- **Complete**: *Board Technician* can complete the process

**Key Findings from Database Investigation**:
- **Workflow Path**: Med Tech → Med Officer → Unit CC → Wing JA → Wing CC → Board Tech → [Board Medical/Board Legal/Board Approving Authority/Board Administrator] → Complete
- **Board Review Process**: Board Tech coordinates, then any Board role can forward to other Board roles in a recycle process
- **Personnel Type Mappings**: All personnel types have direct 1:1 mappings to user groups for informal LOD
- **Finding Tracking**: Each role creates findings with their corresponding ptype value

### Formal LOD Process
For complex cases requiring investigation (e.g., Misconduct, EPTS Aggravated; Formal=true).

**Note**: Formal LOD includes the informal LOD process as the first part, then adds formal investigation and board review.

```
[Informal LOD Process] -> LOD PM (Assign to IO) -> Investigating Officer (Investigation) -> Wing JA (Formal Action Review) -> Wing Commander (Appointing Authority Action) -> Board Technician (Formal Board Review)
     |
     v   [Formal Board Loop with Options - Repeat Until Complete]
Board Technician (Options: to Medical, Administrator, Legal, Complete, Approving, RFA to Appointing) <-> Board Medical (Options: to Administrator, Legal, Technician, Complete, Approving, RFA)
     |          |
     v          v
Board Legal (Options: to Medical, Administrator, Technician, Approving) <-> Board Administrator (Options: to Technician, Medical, Legal, Approving)
     |
     v
Approving Authority (Final Action/Recommend Cancel) -> [If Cancel/Loop: Back to Board Technician] -> ... (Repeat) -> Board Technician (Complete)
```

**Formal LOD Personnel Type Mappings** (to be confirmed with formal LOD investigation):
- **Formal Board roles** use formal personnel types (ptype 12-18, 23)
- **Formal Wing roles** use formal personnel types (ptype 12-13)
- **Formal Board process** follows similar recycle pattern as informal but with formal personnel types

**Explanation** (Aligned with Video):
- **Initiation to Wing Commander**: As before (primary roles; informal switch in video shows contrast). The video demonstrates by starting informal then switching to formal, showing the escalation and board loop (around 1:48:50).
- **Appoint/Forward to LOD PM**: Primary: *Wing Commander* appoints and forwards (video: Wing CC to LOD PM).
- **Assign to IO**: Primary: *LOD PM* assigns to Investigating Officer (video: LOD PM to IO-AFRC).
- **Investigation**: Primary: *Investigating Officer* investigates, forwards to Wing JA (video: IO to Wing JA for formal action).
- **Formal Action Review**: Primary: *Wing JA* reviews, forwards back to Wing CC (video: Wing JA to Wing Commander for appointing action).
- **Appointing Authority Action**: Primary: *Wing CC* acts, forwards to Board Technician (video: Wing CC to formal Board Review).
- **Board Review Loop**: Iterative cycle (video 1:48:50): Starts at *Board Technician* (options: to Medical/Administrator/Legal/Complete/Approving/RFA) <-> *Board Medical* (options: to Administrator/Legal/Technician/Complete/Approving/RFA) <-> *Board Administrator* (options: to Medical/Legal/Technician/Approving) <-> *Board Legal* (options: to Technician/Medical/Administrator/Approving). Conditional: *Board Senior Reviewer* if needed; loops until "Complete" or "Recommend Cancel."
- **Final Action**: Primary: *Approving Authority* resolves (e.g., approve/cancel; video: Options lead here).
- **Complete**: Primary: *Board Technician* completes the process (similar to Informal LOD); Conditional: *LOD Program Manager* may track/notify but does not close.

### Restricted SARC Workflow (SARCRestricted - Value: 28) - Updated
Restricted sexual assault cases with proper role hierarchy and tracking.

```
Medical Technician (Initiation) -> Wing SARC (Real Initiator/Review) -> SARC Administrator (Administration/Support) -> Approving Authority (Approval/Decision) -> SARC Administrator (Finalizer) -> Complete
```

**Explanation** (Updated based on role analysis and system behavior):
- **Initiation**: Primary: Medical Technician (ptype=1) can initiate restricted SARC cases; Conditional: Medical Technician SARC may also initiate but with same-unit restrictions. **Note**: Med Tech is removed from tracking after forwarding to Wing SARC.
- **Real Initiator/Review**: Primary: Wing SARC (ptype=25, "WING_SARC_RSL") is the real initiator who can either initiate directly or receive forwarded cases from Med Tech; handles review and input for restricted cases.
- **Administration/Support**: Primary: SARC Administrator (ptype=26, "SARC_ADMIN") handles coordination, support services, and documentation; ensures privacy compliance for restricted reporting.
- **Approval/Decision**: Primary: Approving Authority (ptype=10, "BOARD_AA") reviews and makes final decisions on restricted SARC cases.
- **Finalizer**: Primary: SARC Administrator (ptype=26) serves as the finalizer, completing post-approval tasks and case closure.
- **Complete**: Primary: System closes case with proper restricted reporting compliance.

**Key Role Details**:
- **Medical Technician**: Can be initiator but removed from tracking after forwarding
- **Wing SARC**: Real initiator (can initiate or receive forwarded cases)
- **SARC Administrator**: Reviewer and Finalizer role
- **Approving Authority**: Board-level approval role
- **Med Tech SARC**: Role exists but forwarding destination not yet identified in current analysis

### Unrestricted SARC Workflow
Unrestricted cases are routed as standard LOD_v2 (informal or formal), with both Med Tech roles able to initiate.

```
Medical Technician / Med Tech SARC (Initiation as LOD) -> [Merges into LOD_v2 Informal or Formal Flow] -> ... (See LOD_v2 sections for details) -> Complete
```

**Explanation**:
- Treated as regular LOD (workflow=27) with SARC flag but no restricted privacy limits.
- Initiation: Both Regular Med Tech and Med Tech SARC can start; no same-unit restriction.
- Proceeds to LOD steps (e.g., Medical Review → Unit Approval → etc.), escalating to formal if needed (e.g., misconduct).

(Revise all subsequent workflow diagrams and explanations similarly: Use linear 'Role (Step) -> Role (Next Step) -> ...' format in diagrams. In explanations, start with 'Primary: [Role] does [action]; Conditional ([condition]): [Role] does [action]'. Apply to LOD_v2, ReinvestigationRequest, AppealRequest, SARCRestricted, SARCRestrictedAppeal, all AFRC Special Cases, and all ANG workflows/special cases.)

### LOD_v2 (Updated LOD Investigation) - Value: 27
Enhanced variant with auditing, encompassing both informal and formal paths based on case complexity and 'Formal' flag.

#### Informal LOD_v2 Process
For simple admin cases (e.g., Minor Injury; Formal=false).

```
Medical Technician (Initiation + Audit) -> Medical Officer (Medical Review + Validation incl. EPTS/Mobility) -> Unit Commander (Unit Approval/Forward + Credible Check) -> [Conditional: Wing JA (Legal Review if Needed)] -> [Conditional: Wing Commander (Closure/Endorsement + Finalization)]
```

**Explanation**:
- **Initiation + Audit**: Primary: Medical Technician initiates/uploads; Conditional (SARC): Medical Technician SARC; LOD Program Manager audits.
- **Medical Review + Validation incl. EPTS/Mobility**: Primary: Medical Officer assesses ICD/tests/EPTS; Conditional (pilot): Med Officer (P); Senior Medical Reviewer for complexities.
- **Unit Approval/Forward + Credible Check**: Primary: Unit Commander approves/checks credible service/orders; Conditional: MPF updates.
- **Legal Review if Needed**: Primary: Wing JA checks; Conditional: Board Legal assists.
- **Closure/Endorsement + Finalization**: Primary: Wing Commander closes; Conditional: LOD Program Manager tracks; uses v2 BoardFinalization.

#### Formal LOD_v2 Process
For complex cases (e.g., Misconduct; Formal=true).

```
Medical Technician (Initiation + Audit) -> Medical Officer (Medical Review + Validation incl. Tests/EPTS) -> Unit Commander (Unit Approval/Forward + Checks) -> Wing JA (Initial Legal Review) -> Wing Commander (Appoint/Forward to LOD PM) -> LOD PM (Assign to IO) -> Investigating Officer (Investigation + Evidence/Audit) -> Wing JA (Formal Action Review) -> Wing Commander (Appointing Action) -> Board Technician (Formal Board Review) -> [Board Loop: Technician <-> Medical (w/ v2 Validation) <-> Administrator <-> Legal] -> Approving Authority (Final Action/Sign-off) -> Board Technician (Complete)
```

**Explanation**:
- **Initiation + Audit**: As informal.
- **Medical Review + Validation incl. Tests/EPTS**: Primary: Medical Officer with v2 fields (e.g., alcohol/drug tests, EPTS aggravation); Conditional: RMU concurs.
- **Unit Approval/Forward + Checks**: Primary: Unit Commander with v2 checks (e.g., EightYearRule, MemberOnOrders).
- **Initial Legal Review**: Primary: Wing JA.
- **Appoint/Forward to LOD PM**: Primary: Wing Commander appoints; uses v2 AppointingUnit.
- **Assign to IO**: Primary: LOD PM.
- **Investigation + Evidence/Audit**: Primary: Investigating Officer gathers/audits; v2 evidence handling.
- **Formal Action Review**: Primary: Wing JA.
- **Appointing Action**: Primary: Wing Commander.
- **Formal Board Review**: Primary: Board Technician starts loop.
- **Board Loop**: Iterative with v2 options (e.g., RFA, sub-findings); roles as in informal but with formal escalations.
- **Final Action/Sign-off**: Primary: Approving Authority.
- **Complete**: Primary: Board Technician completes the process (similar to Informal LOD); Conditional: LOD Program Manager may track/audit but does not close.

### ReinvestigationRequest (LOD Reinvestigation) - Value: 5
Re-examine findings.

```
Unit Commander (Request Initiation) -> Medical Officer (Review Prior Findings) -> Investigating Officer (New Investigation) -> Board Medical (Board Re-Review) -> Approving Authority (Updated Approval) -> LOD Program Manager (Closure)
```

**Explanation**:
- **Request Initiation**: Primary: *Unit Commander* submits; Conditional: *LOD Program Manager* tracks (`LODReinvestigationDao.cs`).
- **Review Prior Findings**: Primary: *Medical Officer* reviews data; Conditional (legal): *Wing Judge Advocate* checks.
- **New Investigation**: Primary: *Investigating Officer* re-gathers evidence.
- **Board Re-Review**: Primary: *Board Medical* re-evaluates; Conditional: *Board Senior Reviewer* inputs.
- **Updated Approval**: Primary: *Approving Authority* approves.
- **Closure**: Primary: *LOD Program Manager* closes; Conditional: *MPF* updates records.

### AppealRequest (Appeal Request) - Value: 26
Appeal decisions.

```
Unit Commander (Appeal Submission) -> Appellate Authority (Initial Review) -> Investigating Officer (Evidence Gathering) -> Board Technician (Board Hearing) -> Approving Authority (Decision) -> LOD Program Manager (Final Notification)
```

**Explanation**:
- **Appeal Submission**: Primary: *Unit Commander* files; Conditional: *LOD Program Manager* tracks (`LODAppealDao.cs`).
- **Initial Review**: Primary: *Appellate Authority* assesses; Conditional (legal): *Wing Judge Advocate* inputs.
- **Evidence Gathering**: Primary: *Investigating Officer* collects.
- **Board Hearing**: Primary: *Board Technician* facilitates; Conditional: *Board Medical* reviews.
- **Decision**: Primary: *Approving Authority* decides; Conditional (medical): *Senior Medical Reviewer* advises.
- **Final Notification**: Primary: *LOD Program Manager* notifies; Conditional: *MPF* implements.

### SARCRestricted (Restricted SARC Case) - Value: 28
Restricted reporting with proper role hierarchy.

```
Medical Technician (Initiation) -> Wing SARC (Real Initiator/Review) -> SARC Administrator (Administration/Support) -> Approving Authority (Approval/Decision) -> SARC Administrator (Finalizer) -> Complete
```

**Explanation** (Updated to match role analysis):
- **Initiation**: Primary: *Medical Technician* (ptype=1) initiates; Conditional: *Medical Technician SARC* may initiate with restrictions; **Note**: Med Tech removed from tracking after forwarding.
- **Real Initiator/Review**: Primary: *Wing SARC* (ptype=25, "WING_SARC_RSL") is real initiator; can initiate directly or receive forwarded cases; handles restricted case review.
- **Administration/Support**: Primary: *SARC Administrator* (ptype=26, "SARC_ADMIN") coordinates support services; ensures privacy compliance.
- **Approval/Decision**: Primary: *Approving Authority* (ptype=10, "BOARD_AA") makes final decisions on restricted cases.
- **Finalizer**: Primary: *SARC Administrator* (ptype=26) serves as finalizer; completes case closure and tracking.

### SARCRestrictedAppeal (Restricted SARC Appeal) - Value: 29
Appeal restricted cases.

```
SARC Liaison (Appeal Initiation) -> Wing SARC (SARC Review) -> Appellate Authority (Appellate Decision) -> SARC Administrator (Updated Support) -> LOD Program Manager (Closure)
```

**Explanation**:
- **Appeal Initiation**: Primary: *SARC Liaison* starts; Conditional: *SARC Administrator* logs.
- **SARC Review**: Primary: *Wing SARC* reviews; Conditional: *Medical Officer SARC* assesses.
- **Appellate Decision**: Primary: *Appellate Authority* decides; Conditional (legal): *Wing Judge Advocate*.
- **Updated Support**: Primary: *SARC Administrator* updates; Conditional (psych): *Unit PH*.
- **Closure**: Primary: *LOD Program Manager* closes.

## AFRC Special Case Workflows

### 1. SpecCaseIncap (Incapacitation Pay) - Value: 6
Pay for service-related incapacitation.

```
[Request + Docs] --> [RMU/MO Review] --> [Commander Approval] --> [Finance Adjudication] --> [Appeal?]
```

**Explanation**:
- **Request + Docs**: Primary: *Medical Technician* submits request/docs (`SpecialCaseDao.cs`); Conditional (if SARC-related): *Medical Technician SARC*; Conditional (if ANG): *ANG Tech*; *Unit Commander* endorses; *LOD Program Manager* tracks.
- **RMU/MO Review**: Primary: *RMU* reviews eligibility; Conditional (if SARC-related): *Medical Officer SARC*; Conditional (if ANG): *ANG Tech*; *Medical Officer* assesses; *Senior Medical Reviewer* complex cases; *HQ Medical Technician* inputs.
- **Commander Approval**: Primary: *Unit Commander* signs; Conditional (if wing-level): *Wing Commander*; Conditional (if high-level): *AFR Command Chief*; *Wing Commander* endorses.
- **Finance Adjudication**: *MPF* processes pay (transcript: "Finance in INCAP"); *LOD MFP* updates personnel.
- **Appeal?**: *Appellate Authority* handles denials; *Board Legal* reviews appeals.

### 2. SpecCaseCongress (Congressional Inquiry) - Value: 7
Responses to inquiries.

```
[Inquiry Entry] --> [Gather Docs] --> [JA Draft] --> [Commander Sign] --> [Liaison Send] --> [Escalation?]
```

**Explanation**:
- **Inquiry Entry**: Primary: *LOD Program Manager* enters details; Conditional (if wing-level): *Wing Admin*; Conditional (if high-level): *AFR Director of Staff*; *System Administrator* configs.
- **Gather Docs**: Primary: *Medical Technician* collects medical; Conditional (if board-related): *Board Technician*; Conditional (if medical): *RMU*; *Board Legal* assists if SARC-related.
- **JA Draft**: Primary: *Wing Judge Advocate* drafts response; Conditional (if SARC-related): *SARC Liaison*; *Board Legal* assists.
- **Commander Sign**: Primary: *Unit Commander* provides local input; Conditional (if wing-level): *Wing Commander*; Conditional (if high-level): *AFR Director of Staff*; *Wing Commander* endorses.
- **Liaison Send**: Primary: *Human Resources OPR/OCR* sends; Conditional (if high-level): *AFR Director of Personnel*; *AFR Command Chief* coords.
- **Escalation?**: Conditional (if high-level): *AFR Command Chief*; *Appellate Authority* reviews; *Chief Air Force Reserve* top oversight.

### 3. SpecCaseBMT (Basic Military Training) - Value: 8
Training injuries.

```
[Incident Report] --> [MO Assess] --> [Commander Concur] --> [Board Determine] --> [Appeal?]
```

**Explanation**:
- **Incident Report**: Primary: *Medical Technician* reports incident; Conditional (if wing-level): *Unit Commander*; *LOD Program Manager* tracks.
- **MO Assess**: Primary: *Medical Officer* evaluates injury; Conditional (if wing-level): *Wing Commander*; *RMU* concurs; *Senior Medical Reviewer* assesses complexity.
- **Commander Concur**: Primary: *Unit Commander* concurs; Conditional (if wing-level): *Wing Commander*; *Approving Authority* finalizes; *AFR Command Chief* if high-level.
- **Board Determine**: Primary: *Board Medical* determines benefits; Conditional (if health-related): *Board Senior Reviewer*; *Board Technician* manages process.
- **Appeal?**: *Appellate Authority* handles; *Board Legal* legal review.

### 4. SpecCaseWWD (Worldwide Duty) - Value: 11
Duty despite medical issues.

```
[Duty Request] --> [MO Evaluate] --> [Commander Approve/Deny] --> [Finalize Duty] --> [Appeal?]
```

**Explanation**:
- **Duty Request**: Primary: *Unit Commander* submits request; Conditional (if wing-level): *Wing Commander*; *Approving Authority* finalizes; *AFR Command Chief* if high-level.
- **MO Evaluate**: Primary: *Medical Officer* assesses fitness; Conditional (if wing-level): *Wing Commander*; *Senior Medical Reviewer* reviews; *RMU* concurs.
- **Commander Approve/Deny**: Primary: *Wing Commander* decides; *Approving Authority* finalizes; *AFR Command Chief* if high-level.
- **Finalize Duty**: *MPF* updates assignment; *LOD MFP* personnel changes.
- **Appeal?**: *Appellate Authority* to AFRC/SG; *Board Medical* appeal review.

### 5. SpecCasePW (Profile Waiver) - Value: 12
Waivers for profiles.

```
[Waiver Justification] --> [MO Recommend] --> [JA Concur] --> [Commander Sign] --> [Appeal?]
```

**Explanation**:
- **Waiver Justification**: Primary: *Medical Technician* enters justification; Conditional (if psych-related): *Unit PH*; *LOD Program Manager* tracks.
- **MO Recommend**: Primary: *Medical Officer* recommends; Conditional (if wing-level): *Wing Commander*; *RMU* concurs; *Senior Medical Reviewer* assesses.
- **JA Concur**: Primary: *Wing Judge Advocate* legal concurrence; Conditional (if wing-level): *Wing Commander*; *Board Legal* assists.
- **Commander Sign**: Primary: *Unit Commander* signs; Conditional (if wing-level): *Wing Commander*; *Wing Commander* endorses.
- **Appeal?**: *Appellate Authority*; *Board Senior Reviewer* reviews.

### 6. SpecCaseMEB (Medical Evaluation Board) - Value: 13
Board for retention/separation.

```
[Condition Entry] --> [Gather Evals] --> [Board Vote] --> [Commander Endorse] --> [Appeal to PEB?]
```

**Explanation**:
- **Condition Entry**: Primary: *Medical Officer* enters condition; Conditional (if wing-level): *Wing Commander*; *Approving Authority* approves.
- **Gather Evals**: Primary: *Senior Medical Reviewer* gathers evals; Conditional (if wing-level): *Wing Commander*; *Board Medical* pre-reviews; *RMU* inputs.
- **Board Vote**: Primary: *Board Medical/Legal* votes; Conditional (if wing-level): *Wing Commander*; *Board Senior Reviewer* leads; *Board Technician* facilitates.
- **Commander Endorse**: Primary: *Wing Commander* endorses; *Approving Authority* approves.
- **Appeal to PEB?**: *Appellate Authority* handles; *Chief Air Force Reserve* oversees.

### 7. SpecCaseBCMR (Board for Correction of Military Records) - Value: 14
Record corrections.

```
[Error Details] --> [Prepare File] --> [Board Hearing] --> [Secretary Approve] --> [Judicial Appeal?]
```

**Explanation**:
- **Error Details**: Primary: *Wing Admin* enters details; Conditional (if wing-level): *Wing Commander*; *LOD Program Manager* tracks; *MPF* provides records.
- **Prepare File**: Primary: *Board Technician* prepares; Conditional (if health-related): *Medical Officer*; *Board Legal* assists.
- **Board Hearing**: Primary: *Board Legal* hears case; Conditional (if wing-level): *Wing Commander*; *Board Senior Reviewer* reviews; Conditional (if health-related): *Board Medical*.
- **Secretary Approve**: Primary: *Approving Authority* signs; Conditional (if wing-level): *Chief Air Force Reserve*; *AFR Command Chief* personnel.
- **Judicial Appeal?**: *Appellate Authority*; *Wing Judge Advocate* legal advice.

### 8. SpecCaseFT (Fast Track) - Value: 15
Expedited cases.

```
[Urgent Entry] --> [Accelerate Reviews] --> [Final Sign-off] --> [Appeal?]
```

**Explanation**:
- **Urgent Entry**: Primary: *Unit Commander* enters urgent details; Conditional (if wing-level): *Wing Commander*; *LOD Program Manager* accelerates tracking.
- **Accelerate Reviews**: Primary: *Medical Officer* quick medical assess; Conditional (if wing-level): *Wing Commander*; *Wing Judge Advocate* legal review; Conditional (if complex): *Senior Medical Reviewer*.
- **Final Sign-off**: Primary: *Approving Authority* signs; Conditional (if high-level): *AFR Command Chief*; *AFR Command Chief* high-level approval.
- **Appeal?**: *Appellate Authority*.

### 9. SpecCaseCMAS (Case Management and Adjudication System) - Value: 16
External integration.

```
[Data Sync] --> [Adjudicate] --> [Commander Endorse] --> [CMAS Appeal?]
```

**Explanation**:
- **Data Sync**: Primary: *System Administrator* syncs data; Conditional (if wing-level): *Wing Commander*; *LOD Program Manager* coordinates; *Wing Admin* assists.
- **Adjudicate**: Primary: *Board Legal* adjudicates; Conditional (if wing-level): *Wing Commander*; *Approving Authority* reviews; *Board Senior Reviewer* inputs.
- **Commander Endorse**: Primary: *Wing Commander* endorses; Conditional (if wing-level): *Unit Commander*; *Unit Commander* local input.
- **CMAS Appeal?**: *Appellate Authority*.

### 10. SpecCaseMEPS (Military Entrance Processing Station) - Value: 17
Entrance issues.

```
[Entrance Details] --> [MO Evaluate] --> [Commander Waive] --> [Appeal?]
```

**Explanation**:
- **Entrance Details**: Primary: *Medical Technician* enters details; Conditional (if wing-level): *Wing Commander*; *LOD Program Manager* tracks.
- **MO Evaluate**: Primary: *Medical Officer* evaluates fitness; Conditional (if wing-level): *Wing Commander*; *RMU* concurs.
- **Commander Waive**: Primary: *Unit Commander* waives if applicable; Conditional (if wing-level): *Wing Commander*; *Wing Commander* approves.
- **Appeal?**: *Appellate Authority*.

### 11. SpecCaseMMSO (Military Medical Support Office) - Value: 18
Support requests.

```
[Support Needs] --> [Assign Resources] --> [MO Sign-off] --> [Escalation?]
```

**Explanation**:
- **Support Needs**: Primary: *Medical Technician* enters needs; Conditional (if psych): *Unit PH*.
- **Assign Resources**: Primary: *RMU* assigns; Conditional (if wing-level): *Wing Commander*; *LOD Program Manager* tracks; Conditional (if wing-level): *Wing Commander*; *Senior Medical Reviewer* oversees.
- **MO Sign-off**: Primary: *Medical Officer* signs off.
- **Escalation?**: Conditional (if high-level): *AFR Command Chief*; *Appellate Authority* or *AFR Command Chief*.

### 12. SpecCaseMH (Mental Health) - Value: 19
MH evaluations.

```
[MH Details] --> [Specialist Assess] --> [Compliance Review] --> [Appeal?]
```

**Explanation**:
- **MH Details**: Primary: *Unit PH* enters details; Conditional (if wing-level): *Wing Commander*; *Medical Technician* supports; *LOD Program Manager* tracks.
- **Specialist Assess**: Primary: *Medical Officer* assesses; Conditional (if wing-level): *Wing Commander*; *Senior Medical Reviewer* recommends; *RMU* concurs.
- **Compliance Review**: Primary: *Wing Judge Advocate* ensures compliance; Conditional (if wing-level): *Wing Commander*; *Board Legal* if escalated.
- **Appeal?**: *Appellate Authority*.

### 13. SpecCaseNE (Non-Emergency) - Value: 20
Non-urgent admin.

```
[Details Entry] --> [Supervisor Check] --> [Commander Close] --> [Appeal?]
```

**Explanation**:
- **Details Entry**: Primary: *Wing Admin* enters; Conditional (if wing-level): *Non-Medical Case Manager*; *LOD Program Manager* tracks.
- **Supervisor Check**: Primary: *Unit Commander* checks; Conditional (if wing-level): *Wing Commander*; *Approving Authority* if needed.
- **Commander Close**: Primary: *Wing Commander* closes; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 14. SpecCaseDW (Duty Waiver) - Value: 21
Duty restrictions waivers.

```
[Restriction Justification] --> [MO Recommend] --> [JA Concur] --> [Commander Sign] --> [Appeal?]
```

**Explanation**:
- **Restriction Justification**: Primary: *Medical Technician* enters; Conditional (if wing-level): *Wing Commander*; *Unit Commander* justifies.
- **MO Recommend**: Primary: *Medical Officer* recommends; Conditional (if wing-level): *Wing Commander*; *RMU* concurs.
- **JA Concur**: Primary: *Wing Judge Advocate* concurs; Conditional (if wing-level): *Wing Commander*.
- **Commander Sign**: Primary: *Wing Commander* signs; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 15. SpecCaseMO (Medical Officer) - Value: 22
MO consultations.

```
[Query Entry] --> [MO Consultation] --> [Commander Apply] --> [Rare Appeal?]
```

**Explanation**:
- **Query Entry**: Primary: *Unit Commander* enters query; Conditional (if wing-level): *Wing Commander*; *Medical Technician* supports.
- **MO Consultation**: Primary: *Medical Officer* provides advice; Conditional (if wing-level): *Wing Commander*; *Senior Medical Reviewer* assists.
- **Commander Apply**: Primary: *Wing Commander* applies advice; Conditional (if wing-level): *Wing Commander*.
- **Rare Appeal?**: *Appellate Authority*.

### 16. SpecCasePEPP (Physical Exam Processing Program) - Value: 23
Exam processing.

```
[Exam Data] --> [Validate Results] --> [MO Sign-off] --> [Appeal?]
```

**Explanation**:
- **Exam Data**: Primary: *Medical Technician* enters data.
- **Validate Results**: Primary: *Senior Medical Reviewer* validates; Conditional (if wing-level): *Wing Commander*; *RMU* reviews.
- **MO Sign-off**: Primary: *Medical Officer* signs off.
- **Appeal?**: *Appellate Authority*.

### 17. SpecCaseRS (Reserve Service) - Value: 24
Reserve issues.

```
[Service Details] --> [Supervisor Check] --> [Commander Close] --> [Appeal?]
```

**Explanation**:
- **Service Details**: Primary: *RMU* enters details; Conditional (if wing-level): *Non-Medical Case Manager*; *Wing Commander* closes.
- **Supervisor Check**: Primary: *Unit Commander* checks; Conditional (if wing-level): *Wing Commander*.
- **Commander Close**: Primary: *Wing Commander* closes; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 18. SpecCasePH (Psychological Health) - Value: 25
Psych assessments.

```
[PH Details] --> [Assess/Recommend] --> [Compliance Review] --> [Appeal?]
```

**Explanation**:
- **PH Details**: Primary: *Unit PH* enters; Conditional (if wing-level): *Wing Commander*; *Medical Technician* supports; *LOD Program Manager* tracks.
- **Assess/Recommend**: Primary: *Medical Officer* assesses; Conditional (if wing-level): *Wing Commander*; *Senior Medical Reviewer* recommends.
- **Compliance Review**: Primary: *Wing Judge Advocate* reviews; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 19. SpecCaseRW (Reserve Waiver) - Value: 30
Reserve waivers.

```
[Waiver Justification] --> [MO Recommend] --> [JA Concur] --> [Commander Sign] --> [Appeal?]
```

**Explanation**:
- **Waiver Justification**: Primary: *RMU* enters; Conditional (if wing-level): *Wing Commander*; *Medical Technician* supports.
- **MO Recommend**: Primary: *Medical Officer* recommends; Conditional (if wing-level): *Wing Commander*.
- **JA Concur**: Primary: *Wing Judge Advocate* concurs; Conditional (if wing-level): *Wing Commander*.
- **Commander Sign**: Primary: *Wing Commander* signs; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 20. SpecCaseAGR (Active Guard/Reserve Certification) - Value: 31
AGR certification.

```
[Certification Details] --> [Validate Eligibility] --> [Commander Certify] --> [Appeal?]
```

**Explanation**:
- **Certification Details**: Primary: *Medical Technician* enters; Conditional (if wing-level): *Wing Commander*; *Unit Commander* submits.
- **Validate Eligibility**: Primary: *Medical Officer* validates; Conditional (if wing-level): *Wing Commander*; *RMU* reviews.
- **Commander Certify**: Primary: *Wing Commander* certifies; Conditional (if wing-level): *Wing Commander*.
- **Appeal?**: *Appellate Authority*.

### 21. SpecCasePSCD (Personnel Security Clearance Determination) - Value: 32
Clearance determinations.

```
[Clearance Details] --> [Background Check] --> [Adjudicate] --> [Appeal to CAF?]
```

**Explanation**:
- **Clearance Details**: Primary: *LOD Program Manager* enters; Conditional (if wing-level): *Wing Commander*; *Human Resources OPR* coords.
- **Background Check**: Primary: *Investigating Officer* conducts; Conditional (if wing-level): *Wing Commander*; *Board Legal* oversees.
- **Adjudicate**: Primary: *Approving Authority* decides; Conditional (if wing-level): *Wing Commander*; *Board Senior Reviewer* inputs.
- **Appeal to CAF?**: *Appellate Authority* handles; Conditional (if wing-level): *Wing Commander*.

## ANG Workflows

### 1. ANGLOD (ANG Line of Duty Investigation) - Value: 101
Mirrors LOD with ANG focus.

```
Medical Technician (Initiation) -> Medical Officer (Medical Review) -> Unit Commander (Unit Approval)
     |                                 |                               |
     v                                 v                               v
[Wing JA Review (Wing JA)] --> [Appointing Auth (Wing CC)] --> [Investigation (IO/Board)]
     |                                 |                               |
     v                                 v                               v
[Final Approval (Approving Auth)] --> [Closure/Appeal (Appellate Auth)] --> [End: Benefits]
```

**Explanation**: Mirrors LOD, but *ANG Tech* initiates medical sections instead of Med Tech; appeals to ANGRC instead of AFRC. Other roles (e.g., Unit Commander for approval, Investigating Officer for investigation) function similarly, with ANG-specific tracking if noted in code.

### 2. ANGLOD_v2 (Updated ANG LOD) - Value: 127
Mirrors LOD_v2.

```
[Initiation + Audit] --> [Medical Review + Validation] --> [Unit Approval]
     |                           |                           |
     v                           v                           v
[Wing JA + Legal Check] --> [Appointing + Board Assign] --> [Investigation + Evidence]
     |                           |                           |
     v                           v                           v
[Final Approval + Sign-off] --> [Closure/Appeal + Tracking] --> [End]
```

**Explanation**: As in LOD_v2, with *ANG Tech* in initiation/medical validation; ANGRC for appeals/closure. Audit roles like Auditor Read Only apply similarly.

### 3. ANGReinvestigationRequest (ANG LOD Reinvestigation) - Value: 105
Mirrors ReinvestigationRequest.

```
[Request Initiation] --> [Review Prior Findings] --> [New Investigation (IO)]
     |                        |                         |
     v                        v                         v
[Board Re-Review] --> [Updated Approval] --> [Closure]
```

**Explanation**: As above, with *ANG Tech* supporting prior findings review; ANGRC oversight in closure.

### 4. ANGAppealRequest (ANG Appeal Request) - Value: 126
Mirrors AppealRequest.

```
[Appeal Submission] --> [Initial Review (Appellate Auth)] --> [Evidence Gathering]
     |                          |                              |
     v                          v                              v
[Board Hearing] --> [Decision (Approving Auth)] --> [Final Notification]
```

**Explanation**: As above, appeals routed to ANGRC; *ANG Tech* may assist in evidence if medical.

### 5. ANGSARCRestricted (ANG Restricted SARC Case) - Value: 128
Mirrors SARCRestricted.

```
[Restricted Report] --> [Medical/SARC Review] --> [Support Coordination]
     |                         |                        |
     v                         v                        v
[Commander Notification (Limited)] --> [Closure/Tracking]
```

**Explanation**: As above, with ANG SARC roles (e.g., Wing SARC) and ANG Tech in medical review.

### 6. ANGSARCRestrictedAppeal (ANG Restricted SARC Appeal) - Value: 129
Mirrors SARCRestrictedAppeal.

```
[Appeal Initiation] --> [SARC Review] --> [Appellate Decision]
     |                     |                   |
     v                     v                   v
[Updated Support] --> [Closure]
```

**Explanation**: As above, with ANG-specific appellate routing.

## ANG Special Case Workflows
These mirror AFRC with ANG roles/adjustments (e.g., ANG Tech initiates; appeals to ANGRC/NGB).

### 1. ANGSpecCaseIncap - Value: 106
Mirrors SpecCaseIncap.

```
[Request + Docs] --> [RMU/MO Review] --> [Commander Approval] --> [Finance Adjudication] --> [Appeal?]
```

**Explanation**: As SpecCaseIncap, but *ANG Tech* submits request/docs; appeals to ANGRC.

### 2. ANGSpecCaseCongress - Value: 107
Mirrors SpecCaseCongress.

**Explanation**: As above, with ANG admin entering inquiry; NGB liaison for send/escalation.

### 3. ANGSpecCaseBMT - Value: 108
Mirrors SpecCaseBMT.

**Explanation**: As above, *ANG Tech* reports incident.

### 4. ANGSpecCaseWWD - Value: 111
Mirrors SpecCaseWWD.

**Explanation**: As above, with ANG Commander approve/deny.

### 5. ANGSpecCasePW - Value: 112
Mirrors SpecCasePW.

**Explanation**: As above, *ANG Tech* enters justification.

### 6. ANGSpecCaseMEB - Value: 113
Mirrors SpecCaseMEB.

**Explanation**: As above, appeals to ANG PEB equivalent.

### 7. ANGSpecCaseBCMR - Value: 114
Mirrors SpecCaseBCMR.

**Explanation**: As above, with ANG board hearing.

### 8. ANGSpecCaseFT - Value: 115
Mirrors SpecCaseFT.

**Explanation**: As above, accelerated for ANG urgent cases.

### 9. ANGSpecCaseCMAS - Value: 116
Mirrors SpecCaseCMAS.

**Explanation**: As above, with ANG data sync.

### 10. ANGSpecCaseMEPS - Value: 117
Mirrors SpecCaseMEPS.

**Explanation**: As above, *ANG Tech* enters details.

### 11. ANGSpecCaseMMSO - Value: 118
Mirrors SpecCaseMMSO.

**Explanation**: As above, ANG resources assignment.

### 12. ANGSpecCaseMH - Value: 119
Mirrors SpecCaseMH.

**Explanation**: As above, with ANG specialist assess.

### 13. ANGSpecCaseRW - Value: 130
Mirrors SpecCaseRW.

**Explanation**: As above, *ANG Tech* enters justification.

## Additional Notes
- All workflows integrate emails (`EmailTemplateDao.cs`), docs (`SRXDocumentStore.cs`), and tracking.
- This is the complete, detailed file; saved as `ECTSystem_Workflow_Diagrams_With_Explanations.md`.

## Testing Notes

Use this for manual testing and comparison between test project and production site.

#### High-Level Workflow Overview Diagram (All Categories)
This shows how LOD (with Informal/Formal/SARC) fits into the broader system.

```
[User Login/Role Select] --> [Case Initiation (All Workflows)]
     |
     v
[LOD Workflows] --> [Informal (Quick Admin)] --> [e.g., Minor Injury: Med Review --> Unit Approve --> Close]
                --> [Formal (Investigation)] --> [e.g., Misconduct: Appoint IO --> Board Review --> Approve/Appeal]
                --> [SARC (Restricted/Unrestricted)] --> [e.g., Restricted: Anon Report --> Limited Notify --> Support/Close]
     |
     v
[Special Cases (Non-LOD)] --> [e.g., SpecCaseMEB: Condition Entry --> Board Vote --> Endorse --> Appeal]
     |
     v
[ANG Variants] --> [Mirror Above with ANG Roles/Appeals to ANGRC]
     |
     v
[Common End: Tracking/Reports/Notifications (All)]
```

#### Informal LOD Testing Checklist
| Workflow/Step | Test Actions | Expected Behavior | Prod Comparison | Issues in Test |
|---------------|--------------|-------------------|-----------------|----------------|
| Initiation | As Med Tech, create minor injury case. | Case starts, status=1, no investigation flag. | Prod: Auto-save; Test: Manual. | e.g., Upload fails. |
| Medical Review | As Med Officer, review/upload ICD. | Status to 2; simple validation. | Prod: Quick; Test: Slower. | e.g., No RMU prompt. |
| Unit Approval | As Unit CC, approve. | Memo signed; status advance. | Prod: Email sent; Test: Check log. | e.g., Signature error. |
| Closure | As LOD PM, close. | Case ends; benefits processed. | Prod: Integrated; Test: Mock. | e.g., No notification. |

#### Formal LOD Testing Checklist
| Workflow/Step | Test Actions | Expected Behavior | Prod Comparison | Issues in Test |
|---------------|--------------|-------------------|-----------------|----------------|
| Initiation | Create misconduct case. | Formal flag=true; investigation prompted. | Prod: Alerts; Test: Manual flag. | e.g., Flag not setting. |
| Investigation | As IO, add evidence. | Evidence saved; board assigned. | Prod: Full log; Test: Partial. | e.g., Board missing. |
| Board Review | As Board Medical, vote. | Determination recorded. | Prod: Voting UI; Test: Basic form. | e.g., Vote not saving. |
| Appeal | Submit appeal. | Escalates to Appellate Auth. | Prod: Auto-route; Test: Manual. | e.g., Loop issue. |

#### SARC Workflows Testing Checklist
| Workflow/Step | Test Actions | Expected Behavior | Prod Comparison | Issues in Test |
|---------------|--------------|-------------------|-----------------|----------------|
| Report | As SARC Admin, file restricted. | Privacy mode; limited access. | Prod: Secure logs; Test: Basic. | e.g., Disclosure leak. |
| Review | As Med Officer SARC, assess. | Medical notes saved privately. | Prod: Restricted tabs; Test: Full view. | e.g., Wrong roles access. |
| Notification | Check commander notify. | Limited info only. | Prod: Encrypted; Test: Plain. | e.g., Full details sent. |
| Closure | Close case. | Support tracked; no full disclosure. | Prod: Audit trail; Test: Missing. | e.g., Reopen bug. |

#### Special Cases Testing Checklist
| Workflow/Step | Test Actions | Expected Behavior | Prod Comparison | Issues in Test |
|---------------|--------------|-------------------|-----------------|----------------|
| Initiation (e.g., Incap) | Submit pay request. | Workflow=6; docs required. | Prod: Finance link; Test: Mock. | e.g., Value mismatch. |
| Review (e.g., MEB) | As Board, vote. | Board status advance. | Prod: Multi-user; Test: Single. | e.g., Vote not tallying. |
| Approval | As Approving Auth, sign. | Case advances. | Prod: E-signature; Test: Basic. | e.g., Signature fails. |
| Appeal | Trigger appeal. | Routes to authority. | Prod: External; Test: Internal. | e.g., No escalation. |

#### Test vs. Prod Comparison Chart Template
| Aspect | Test Project | Production Site | Differences/Issues | Impact on Testing |
|--------|--------------|-----------------|---------------------|-------------------|
| Workflow Enums (`WorkflowEnums.cs`) | e.g., LOD=1, New variant LOD_v3? | e.g., LOD=1, No v3. | Test has extra values—potential untested features. | Test extra paths; check if prod needs update. |
| DB Schemas (e.g., `ALOD.Database.sqlproj`) | e.g., New table for SARC appeals. | e.g., Older schema. | Test has schema changes—migrate issues? | Run queries in test; compare outputs with prod. |
| Config (`web.config`) | e.g., Test DB connection. | e.g., Prod DB. | Connection strings differ—security risks. | Use local DB for test to avoid prod interference. |
| Role Permissions (`UserEnums.cs`) | e.g., New role ANG Tech=110. | e.g., Missing in prod. | Test has expanded roles—test access. | Simulate logins; check if prod lacks features. |
| Issues Found | e.g., Error in `NHibernateSessionManager.cs`. | e.g., Works in prod. | Test has bugs (e.g., session timeout). | Fix in test or note for deployment. |

## Document Usage in Workflows

This section describes key documents, their usage across workflows (Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases), and details (purpose, roles, references). Based on codebase (e.g., `DocumentViewDao.cs`) and "Line of Duty Documentation.pdf".

| Document | Workflows Used In | Details |
|----------|-------------------|---------|
| AF Form 348 / DD Form 261 | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Used in initiation/closure for determinations; all LOD types (formal for investigations); SARC for reporting; special cases for eligibility. Primary: Med Tech uploads; Unit CC signs. (PDF: Required for all LODs). |
| Memorandum | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Throughout for approvals/notifications; formal for legal memos; SARC for restricted notices; special cases like waivers. Primary: Unit/Wing CC drafts/signs (`MemoDao.cs`). |
| Military Medical Documentation | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Medical review in all; formal for complex evidence; SARC for assault exams; special cases for boards. Primary: Med Tech uploads; Med Officer reviews. |
| Civilian Medical Documentation | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Similar to military, for off-duty incidents; formal if suspicion; SARC if civilian treatment. Primary: Med Tech uploads; Med Officer reviews. |
| Labs | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, BMT, MEB, MH, NE, DW, MO, PEPP, PH, PSCD, and ANG variants | Medical review for tests; formal for misconduct/suicide; SARC for evidence. Primary: Med Tech uploads; Med Officer interprets. |
| Radiology And Imaging | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, BMT, WWD, PW, MEB, MH, NE, DW, MO, PEPP, PH, and ANG variants | Medical review for visuals; formal for verification. Primary: Med Tech uploads; Med Officer reviews. |
| Studies | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, BMT, MEB, MH, NE, DW, MO, PEPP, PH, and ANG variants | Specialized tests in medical review; formal for evals. Primary: Med Officer reviews. |
| Specialty Consults | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, BMT, MEB, MH, NE, DW, MO, PEPP, PH, and ANG variants | Medical review for expert input; formal for boards. Primary: Med Officer requests/reviews. |
| Proof Of Military Status | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Initiation to confirm status; all types. Primary: Unit CC provides; MPF verifies. |
| PCARS | Formal LOD, Special Cases: BCMR, NE, and ANG variants | For casualties in formal/death cases. Primary: LOD PM uploads. |
| Member's Statement | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Congress, BMT, MEB, BCMR, MH, NE, DW, MO, PH, PSCD, and ANG variants | Investigation/appeals for personal account; formal/SARC. Primary: Investigating Officer collects. |
| Maps | Formal LOD, Special Cases: Congress, BMT, WWD, MH, NE, DW, MO, PH, PSCD, and ANG variants | Investigation for location in formal accidents. Primary: Investigating Officer uploads. |
| Accident Report | Formal LOD, Special Cases: BMT, WWD, MH, NE, DW, MO, PH, PSCD, and ANG variants | Investigation in formal accidents. Primary: Investigating Officer attaches. |
| Autopsy Report/ Death Certificate | Formal LOD, Special Cases: NE, PH, and ANG variants | Formal for fatalities. Primary: Med Officer reviews. |
| Untimely Submission Of Incident Report | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Initiation if late; all types. Primary: Unit CC submits. |
| Signed Notification Memo | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Closure/appeals for notifications; all. Primary: Approving Authority signs. |
| Miscellaneous | Formal LOD, Informal LOD, Sexual Assault (SARC), Special Cases: Incap, Congress, BMT, WWD, PW, MEB, BCMR, FT, CMAS, MEPS, MMSO, MH, NE, DW, MO, PEPP, RS, PH, RW, AGR, PSCD, and ANG variants | Any step for uncategorized evidence; all. Primary: Any role uploads; Reviewing role assesses. |
