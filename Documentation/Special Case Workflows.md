# ECTSystem Special Case Workflows Analysis

This Markdown file documents the Special Case workflows in the ECTSystem/ALOD application, based on codebase analysis (e.g., `WorkflowEnums.cs`, `Start.aspx.vb`, database triggers, and domain models). It covers triggers, scenarios, role-based steps, and simple ASCII diagrams for each workflow.

Workflows are categorized into AFRC (Air Force Reserve Command) and ANG (Air National Guard) variants. Diagrams are high-level ASCII flowcharts derived from code logic (e.g., status transitions, creation functions, and triggers). Roles include Med Tech, Medical Officer (MO), Wing JA, Commander, LODPM, etc., with permissions handled via database procs like `utility_workflow_sp_Page_Access_Permissions`.

For brevity, similar workflows reference shared diagrams. ANG workflows mirror AFRC with component-specific adjustments (e.g., ANGRC instead of AFRC).

---

## AFRC Special Case Workflows

### Trigger SpecCaseIncap Workflow (Incapacitation Pay, Value: 6)
🎯 Scenario: Member requests pay for incapacitation due to service-related injury, requiring validation of eligibility and medical docs.  
🧪 Steps to Test:  
- Start Case as Med Tech → Enter injury details + pay request.  
- Medical Officer Review: Validates medical docs; recommends approval/denial.  
- RMU (Reserve Medical Unit) Review: Concurs or flags issues; updates status.  
- LODPM: Creates tracking entry; sends notifications.  
- Commander Approval: Signs memo; sets deadlines.  
- Finance Officer: Adjudicates pay; finalizes determination.  
- Appeal Option: Member appeals to AFRC/CD if denied.

**Diagram:**
```
[User Selects Incap] --> [Create Case (CreateIncapWorkflow)] --> [Initial Status + Signature]
     | 
     v
[Upload Docs] --> [Status Update Trigger] --> [RMU/MO Review/Approve]
     | 
     v
[Finance Adjudication] --> [Appeal? (If Denied)] --> [Final Closure]
```

### Trigger SpecCaseCongress Workflow (Congressional Inquiry, Value: 7)
🎯 Scenario: Congressional office inquires about a member's case status, triggering expedited response.  
🧪 Steps to Test:  
- Start Case as Admin Staff → Enter inquiry details + member info.  
- LODPM Review: Gathers docs; assigns to responder.  
- Wing JA Review: Legal review; drafts response.  
- Commander Approval: Signs response memo.  
- Congressional Liaison: Sends final response; updates status.  
- Appeal Option: Rare; member may escalate via congress if unsatisfied.

**Diagram:**
```
[Select Congress Inquiry] --> [Create Case] --> [Initial Status]
     | 
     v
[Gather Docs] --> [JA Draft Response] --> [Commander Sign]
     | 
     v
[Liaison Send] --> [Closure] --> [Escalation?]
```

### Trigger SpecCaseBMT Workflow (Basic Military Training, Value: 8)
🎯 Scenario: Injury during basic training requires eligibility check for benefits.  
🧪 Steps to Test:  
- Start Case as Training Instructor → Enter training incident details.  
- Medical Officer Review: Assesses injury; recommends waivers.  
- Training Commander Review: Concurs; assigns investigator if needed.  
- LODPM: Notifies chain; tracks deadlines.  
- Admin Board: Final determination on benefits.  
- Appeal Option: Member appeals to AFRC/CD.

**Diagram:**
```
[Select BMT] --> [Create Case] --> [Initial Status]
     | 
     v
[Assess Injury] --> [Commander Concur] --> [Admin Board Determine]
     | 
     v
[Notify/Track] --> [Appeal?]
```

### Trigger SpecCaseWWD Workflow (Worldwide Duty, Value: 11)
🎯 Scenario: Member seeks duty assignment worldwide despite medical issues.  
🧪 Steps to Test:  
- Start Case as Unit Admin → Enter duty request + medical history.  
- Medical Officer Review: Evaluates fitness for duty.  
- Wing Commander: Approves/denies assignment.  
- LODPM: Sends notifications; updates tracking.  
- Deployment Coordinator: Finalizes duty status.  
- Appeal Option: Appeal to AFRC/SG (Surgeon General).

**Diagram:**
```
[Select WWD] --> [Create Case] --> [Initial Status]
     | 
     v
[Evaluate Fitness] --> [Commander Approve/Deny] --> [Finalize Duty]
     | 
     v
[Notify/Track] --> [Appeal?]
```

### Trigger SpecCasePW Workflow (Profile Waiver, Value: 12)
🎯 Scenario: Member requests waiver for medical profile restrictions.  
🧪 Steps to Test:  
- Start Case as Med Tech → Enter profile details + waiver justification.  
- Medical Officer Review: Flags risks; recommends.  
- Wing JA Review: Legal concurrence.  
- Commander Approval: Signs waiver.  
- LODPM: Logs and notifies.  
- Appeal Option: Member appeals to AFRC/SG if denied.

**Diagram:**
```
[Select PW] --> [Create Case] --> [Initial Status + Signature]
     | 
     v
[MO Recommend] --> [JA Concur] --> [Commander Sign]
     | 
     v
[Log/Notify] --> [Appeal?]
```

### Trigger SpecCaseMEB Workflow (Medical Evaluation Board, Value: 13)
🎯 Scenario: Member's condition requires board evaluation for retention or separation.  
🧪 Steps to Test:  
- Start Case as Medical Officer → Enter condition details.  
- MEB Coordinator Review: Gathers evals; sets board date.  
- Board Members (Medical, Legal, Admin): Review findings; vote on outcome.  
- LODPM: Tracks forwarding dates; notifies.  
- Commander Endorsement: Approves board recommendation.  
- Appeal Option: Member appeals to Physical Evaluation Board (PEB).

**Diagram:**
```
[Select MEB] --> [Create Case] --> [Initial Status]
     | 
     v
[Gather Evals] --> [Board Review/Vote] --> [Commander Endorse]
     | 
     v
[Track/Notify] --> [Appeal to PEB?]
```

### Trigger SpecCaseBCMR Workflow (Board for Correction of Military Records, Value: 14)
🎯 Scenario: Member seeks correction of records (e.g., erroneous discharge).  
🧪 Steps to Test:  
- Start Case as Admin Staff → Enter record error details.  
- Wing JA Review: Prepares case file.  
- BCMR Board Members: Hear case; make determination.  
- LODPM: Sends notifications.  
- Secretary Approval: Final sign-off.  
- Appeal Option: Limited; judicial review if needed.

**Diagram:**
```
[Select BCMR] --> [Create Case] --> [Initial Status]
     | 
     v
[Prepare File] --> [Board Hearing] --> [Secretary Approve]
     | 
     v
[Notify] --> [Judicial Appeal?]
```

### Trigger SpecCaseFT Workflow (Fast Track, Value: 15)
🎯 Scenario: Expedited handling for urgent cases (e.g., deployment-related).  
🧪 Steps to Test:  
- Start Case as Unit Commander → Enter urgent details.  
- LODPM Review: Accelerates processing.  
- Medical/Wing JA Quick Review: Concurs rapidly.  
- Final Approver (e.g., AFRC/CD): Signs off.  
- Appeal Option: Standard AFRC appeal.

**Diagram:**
```
[Select FT] --> [Create Case] --> [Initial Status]
     | 
     v
[Accelerate Process] --> [Quick Reviews] --> [Final Sign-off]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseCMAS Workflow (Case Management and Adjudication System, Value: 16)
🎯 Scenario: Integration with external system for case adjudication.  
🧪 Steps to Test:  
- Start Case as Admin Staff → Enter case data for CMAS sync.  
- System Integrator Review: Validates data transfer.  
- Adjudicator (CMAS Role): Makes determination.  
- LODPM: Logs integration; notifies.  
- Commander Endorsement: Final approval.  
- Appeal Option: Via CMAS appeals process.

**Diagram:**
```
[Select CMAS] --> [Create Case] --> [Initial Status]
     | 
     v
[Sync Data] --> [Adjudicate] --> [Commander Endorse]
     | 
     v
[Log/Notify] --> [CMAS Appeal?]
```

### Trigger SpecCaseMEPS Workflow (Military Entrance Processing Station, Value: 17)
🎯 Scenario: Processing for entrance medical issues (limited custom logic in code).  
🧪 Steps to Test:  
- Start Case as Recruiter → Enter entrance details.  
- MEPS Medical Officer Review: Evaluates fitness.  
- LODPM: Tracks status.  
- Commander Approval: Waives if applicable.  
- Appeal Option: To AFRC/SG.

**Diagram:**
```
[Select MEPS] --> [Create Case] --> [Initial Status]
     | 
     v
[Evaluate Fitness] --> [Commander Waive] --> [Track Status]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseMMSO Workflow (Military Medical Support Office, Value: 18)
🎯 Scenario: Support requests for medical services.  
🧪 Steps to Test:  
- Start Case as Med Tech → Enter support needs.  
- MMSO Coordinator Review: Assigns resources.  
- Medical Officer Approval: Signs off.  
- LODPM: Notifies.  
- Appeal Option: Escalate to AFRC/SG.

**Diagram:**
```
[Select MMSO] --> [Create Case] --> [Initial Status]
     | 
     v
[Assign Resources] --> [MO Sign-off] --> [Notify]
     | 
     v
[Escalation?]
```

### Trigger SpecCaseMH Workflow (Mental Health, Value: 19)
🎯 Scenario: Evaluation for mental health conditions.  
🧪 Steps to Test:  
- Start Case as Counselor → Enter MH details (sensitive handling).  
- MH Specialist Review: Assesses and recommends.  
- Commander/Wing JA Review: Ensures compliance.  
- LODPM: Tracks with privacy rules.  
- Appeal Option: To AFRC/SG or external board.

**Diagram:**
```
[Select MH] --> [Create Case] --> [Initial Status]
     | 
     v
[Assess/Recommend] --> [Compliance Review] --> [Track/Notify]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseNE Workflow (Non-Emergency, Value: 20)
🎯 Scenario: General non-urgent administrative case.  
🧪 Steps to Test:  
- Start Case as Admin Staff → Enter details.  
- Supervisor Review: Initial check.  
- LODPM: Updates tracking.  
- Final Approver (Commander): Closes case.  
- Appeal Option: Standard.

**Diagram:**
```
[Select NE] --> [Create Case] --> [Initial Status]
     | 
     v
[Supervisor Check] --> [Commander Close] --> [Track/Update]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseDW Workflow (Duty Waiver, Value: 21)
🎯 Scenario: Waiver for duty restrictions (similar to PW).  
🧪 Steps to Test: (Mirror PW steps; replace "Profile" with "Duty").  

**Diagram:** (Similar to SpecCasePW)
```
[Select DW] --> [Create Case] --> [Initial Status + Signature]
     | 
     v
[MO Recommend] --> [JA Concur] --> [Commander Sign]
     | 
     v
[Log/Notify] --> [Appeal?]
```

### Trigger SpecCaseMO Workflow (Medical Officer, Value: 22)
🎯 Scenario: Consultation with Medical Officer for advice.  
🧪 Steps to Test:  
- Start Case as Unit Admin → Enter query.  
- Medical Officer Review: Provides consultation.  
- LODPM: Logs response.  
- Commander Endorsement: Applies advice.  
- Appeal Option: Rare.

**Diagram:**
```
[Select MO] --> [Create Case] --> [Initial Status]
     | 
     v
[Provide Consultation] --> [Commander Apply] --> [Log Response]
     | 
     v
[Rare Appeal?]
```

### Trigger SpecCasePEPP Workflow (Physical Exam Processing Program, Value: 23)
🎯 Scenario: Processing physical exams for fitness.  
🧪 Steps to Test:  
- Start Case as Med Tech → Enter exam data.  
- PEPP Coordinator Review: Validates results.  
- Medical Officer Approval: Signs off.  
- LODPM: Tracks.  
- Appeal Option: To AFRC/SG.

**Diagram:**
```
[Select PEPP] --> [Create Case] --> [Initial Status]
     | 
     v
[Validate Results] --> [MO Sign-off] --> [Track]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseRS Workflow (Reserve Service, Value: 24)
🎯 Scenario: Reserve-specific service issues.  
🧪 Steps to Test: (Generic; similar to NE).  

**Diagram:** (Similar to SpecCaseNE)
```
[Select RS] --> [Create Case] --> [Initial Status]
     | 
     v
[Supervisor Check] --> [Commander Close] --> [Track/Update]
     | 
     v
[Appeal?]
```

### Trigger SpecCasePH Workflow (Psychological Health, Value: 25)
🎯 Scenario: Psychological assessments (similar to MH).  
🧪 Steps to Test: (Mirror MH steps).  

**Diagram:** (Similar to SpecCaseMH)
```
[Select PH] --> [Create Case] --> [Initial Status]
     | 
     v
[Assess/Recommend] --> [Compliance Review] --> [Track/Notify]
     | 
     v
[Appeal?]
```

### Trigger SpecCaseRW Workflow (Reserve Waiver, Value: 30)
🎯 Scenario: Waiver for reserve duties (similar to PW/DW).  
🧪 Steps to Test: (Mirror PW steps).  

**Diagram:** (Similar to SpecCasePW)
```
[Select RW] --> [Create Case] --> [Initial Status + Signature]
     | 
     v
[MO Recommend] --> [JA Concur] --> [Commander Sign]
     | 
     v
[Log/Notify] --> [Appeal?]
```

### Trigger SpecCaseAGR Workflow (Active Guard/Reserve Certification, Value: 31)
🎯 Scenario: Certification for AGR status.  
🧪 Steps to Test:  
- Start Case as Recruiter → Enter certification details.  
- AGR Coordinator Review: Validates eligibility.  
- Commander Approval: Certifies.  
- LODPM: Notifies.  
- Appeal Option: To AFRC/CD.

**Diagram:**
```
[Select AGR] --> [Create Case] --> [Initial Status]
     | 
     v
[Validate Eligibility] --> [Commander Certify] --> [Notify]
     | 
     v
[Appeal?]
```

### Trigger SpecCasePSCD Workflow (Personnel Security Clearance Determination, Value: 32)
🎯 Scenario: Determination for security clearance.  
🧪 Steps to Test:  
- Start Case as Security Manager → Enter clearance details.  
- Investigator Review: Conducts background check.  
- Adjudicator (Security Board): Makes determination.  
- LODPM: Tracks status.  
- Appeal Option: To DoD CAF (Central Adjudication Facility).

**Diagram:**
```
[Select PSCD] --> [Create Case] --> [Initial Status]
     | 
     v
[Background Check] --> [Adjudicate] --> [Track Status]
     | 
     v
[Appeal to CAF?]
```

---

## ANG Special Case Workflows
ANG workflows (Values 106–130) mirror AFRC counterparts with ANG-specific roles (e.g., ANGRC/CC for appeals, NGB/JA for legal). Steps and diagrams are identical, adjusted for component (e.g., replace AFRC/CD with ANGRC/CC). Examples below; refer to AFRC equivalents for full details.

### Trigger ANGSpecCaseIncap Workflow (Value: 106)
🎯 Scenario: Similar to AFRC Incap, but for ANG members.  
🧪 Steps to Test: (Mirror SpecCaseIncap; use ANGRC for appeals).  

**Diagram:** (Identical to SpecCaseIncap)

### Trigger ANGSpecCaseCongress Workflow (Value: 107)
🎯 Scenario: Similar to AFRC Congress.  
🧪 Steps to Test: (Mirror; use NGB liaison).  

**Diagram:** (Identical to SpecCaseCongress)

*(Similarly for: ANGSpecCaseBMT=108, WWD=111, PW=112, MEB=113, BCMR=114, FT=115, CMAS=116, MEPS=117, MMSO=118, MH=119, RW=130. Diagrams match AFRC counterparts.)*

---

This analysis is derived from the codebase as of the provided context. For updates or deeper dives, consult the source files.
