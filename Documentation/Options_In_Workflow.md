# Options In Workflow

This section describes key yes/no options (fields or questions) encountered in the Line of Duty (LOD) workflows within the ECTSystem/ALOD application. These are typically presented as checkboxes, radio buttons, or dropdowns in the UI (e.g., during Medical Review or Unit Approval steps) and influence case progression, eligibility for benefits, and whether the process escalates from informal to formal. Descriptions are based on AFI 36-2910 ("Line of Duty Documentation.pdf") and codebase references (e.g., `LineOfDuty.cs`, `LodEnums.cs`).

## 1. Does the member have 8 years of TAFMS service?
- **Description**: This yes/no field checks if the member has accumulated 8 or more years of Total Active Federal Military Service (TAFMS), which includes all active duty time in the federal military (calculated in years/months/days). 
  - **Yes**: Applies the "8-Year Rule" presumption from AFI 36-2910 (Section 3.4, Attachment 2), assuming any pre-existing condition (EPTS - Existed Prior to Service) was aggravated by military service. This favors an "In Line of Duty" (ILOD) determination, potentially qualifying the member for benefits like disability retirement, incapacitation (INCAP) pay, or medical continuation without needing extensive proof.
  - **No**: No presumption applies; additional evidence (e.g., medical records showing service aggravation) is required, which may lead to a "Not In Line of Duty" (NILOD) finding or formal investigation.
- **Workflow Impact**: Used in LOD_v2 (Value: 27) during Medical Review (by Medical Officer) or Unit Approval (by Unit Commander). Stored as `EightYearRule` in the database/model. Influences findings like EPTS aggravation and can trigger escalations (e.g., "Recommend Formal Investigation").
- **References**: AFI 36-2910 (PDF: Section 4.2.3); Special cases like Incapacitation Pay (SpecCaseIncap, Value: 6) or Medical Evaluation Board (SpecCaseMEB, Value: 13).

## 2. Was the member on orders of 30 days or more?
- **Description**: This yes/no field determines if the injury, illness, or incident occurred while the member was on active duty orders lasting 30 consecutive days or more (e.g., deployment, training, or mobilization orders).
  - **Yes**: Strengthens the presumption of ILOD per AFI 36-2910 (Sections 2.1, 3.2, Attachment 3), often requiring a formal LOD process (especially for misconduct, death, or EPTS cases). Qualifies for extended benefits like INCAP pay (up to 6 months) or MEDCON (medical continuation on active duty).
  - **No**: Defaults to informal LOD handling with potentially limited benefits (e.g., no automatic INCAP pay). Off-duty or short-tour incidents may be ruled NILOD unless clearly service-related.
- **Workflow Impact**: Checked during Initiation (by Medical Technician) or Unit Approval/Forward (by Unit Commander). Stored as `MemberOnOrders` or similar in the model. Can escalate the case (e.g., notify Formal Investigator via Wing Commander) and is critical in special cases like Worldwide Duty (SpecCaseWWD, Value: 11).
- **References**: AFI 36-2910 (PDF: Section 5.1.2); Applies across Informal/Formal LOD, SARC workflows, and ANG variants.

These options ensure compliance with regulations by automating presumptions and routing. If unanswered or invalid, the system may flag errors (e.g., via validation in `NextAction.aspx.vb`). For testing, verify in the UI (e.g., WingCC.aspx) and database (LOD table).
