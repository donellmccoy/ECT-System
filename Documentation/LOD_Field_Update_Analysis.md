# Analysis of Update Timing for LOD Fields: EightYearRule, PriorToDutytatus, StatusWorsened, and PCARSHistory

## Introduction
This report analyzes the update mechanisms for four specific fields in the Line of Duty (LOD) system: `EightYearRule`, `PriorToDutytatus`, `StatusWorsened`, and `PCARSHistory`. These fields are part of the `LineOfDutyFindings` and `LineOfDutyUnit` domain objects, mapped to database columns via Hibernate ORM. The analysis focuses on when these fields are updated in the workflow and whether the current timing aligns with business requirements.

## Current Update Location
Based on code analysis:
- **EightYearRule** and **PriorToDutytatus** are actively updated in `WingCC.aspx` (Wing Commander level).
- **StatusWorsened** and **PCARSHistory** have commented-out update code in `Unit.aspx` (Unit Commander level) and are not currently updated anywhere.

Updates occur through ASP.NET web forms:
- User selects values via `RadioButtonList` controls.
- On form submission, code-behind sets domain object properties (e.g., `findings.EightYearRule = Convert.ToBoolean(rblEightYearRule.SelectedValue)`).
- Persistence is handled via `LOD.SetFindingByType(findings)` using Hibernate ORM—no stored procedures are involved.

## Identified Issue
The fields are intended to be updated earlier in the workflow, ideally at or before the Unit Commander stage, but:
- They are only updated at Wing Commander level (for active fields).
- `StatusWorsened` and `PCARSHistory` are never updated due to commented code.
- This delays critical data entry, potentially affecting downstream decisions (e.g., formal investigations, IO assignments).

Evidence from code:
- In `Unit.aspx.vb`, lines like `'unitInfo.EightYearRule = Convert.ToBoolean(rblEightYearRule.SelectedValue)` are commented out, indicating prior intent to update at Unit level.
- Similar comments for `StatusWorsened` and `PCARSHistory`.
- WingCC code is active, but workflow status checks (e.g., `LodStatusCode.AppointingAutority_LODV3`) suggest it should precede Wing Commander review.

## Workflow Context
- LOD workflow progresses: Unit → Wing Commander → Formal Actions.
- Early updates ensure data is available for Wing Commander decisions (e.g., concur/non-concur with findings).
- Delaying updates to WingCC may violate separation of duties or miss validation opportunities.

## Recommendations
1. **Re-enable Updates at Unit Level**:
   - Uncomment and activate update logic in `Unit.aspx.vb` for all four fields.
   - Ensure UI controls (`rblEightYearRule`, etc.) are enabled and visible.
   - Add validation to prevent null/invalid selections.

2. **Adjust Workflow Timing**:
   - Update status codes to allow saves at `LodStatusCode.UnitReview` or earlier.
   - Ensure `LOD.SetFindingByType` persists changes correctly at Unit level.

3. **Testing and Validation**:
   - Verify updates save to DB and are reflected in subsequent workflow steps.
   - Add unit tests for domain object persistence.
   - Audit logs should capture changes for compliance.

4. **Code Changes Needed**:
   - Modify `Unit.aspx.vb` `Save` event to include field assignments.
   - Update `WingCC.aspx` to load existing values if set earlier.
   - Remove or clarify commented code to avoid confusion.

## Conclusion
The current implementation updates fields too late in the workflow, with two fields inactive. Moving updates to Unit Commander level aligns with best practices for data timeliness and workflow efficiency. Implementing the recommendations will ensure all fields are updated appropriately before Wing Commander review.

**Date of Analysis**: September 25, 2025  
**Analyst**: GitHub Copilot</content>
<parameter name="filePath">c:\Users\JoeIbarra\OneDrive - divihn.com\Documents\GIT\ECTSystem\LOD_Field_Update_Analysis.md