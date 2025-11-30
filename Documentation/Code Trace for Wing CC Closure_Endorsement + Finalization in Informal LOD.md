# Code Trace for Wing CC Closure/Endorsement + Finalization in Informal LOD

This document traces the code logic for the Wing Commander's (Wing CC) options in an informal Line of Duty (LOD) workflow, based on analysis of the ECTSystem codebase. It explains how closure should be available when `Formal=false`, potential reasons for the observed bug (only formal options appearing), and references to specific files and lines.

## Overview
- **Issue Observed**: In informal LOD, Wing CC sees only formal escalation options ("Notify Formal Investigator" and "Forward to LOD Board Review"), no direct "Complete" or closure option. This may be a bug, as informal cases should allow unit/wing-level closure without full investigation/board.
- **Expected Behavior**: For simple informal cases (e.g., minor injury, `Formal=false`), Wing CC should have a closure/endorsement option after conditional legal review, setting final findings and auditing (v2-specific).
- **Focus**: Traces LOD_v2 (workflow=27, primary in production). Assumes the UI page is something like `AppAuth.aspx` or `NextAction.aspx` (based on screenshot and similar pages like `Unit.aspx`).

## Step-by-Step Code Trace

### 1. Initiation and Setting Informal Path
- **File**: `ALOD/Secure/lod/Start.aspx.vb` (lines 441-498).
- **Logic**:
  - Creates `LineOfDuty_v2` instance (line 446 in production).
  - `Formal` flag defaults to false (inherited from `LineOfDuty.cs`).
  - Workflow set to 27 (LOD_v2).
  - Status starts at MedTechReview (200 from `LodEnums.cs` line 321).
  - Redirects to init.aspx; informal path determined later by findings (no formal recommendation).

### 2. Progression to Wing CC Step
- **File**: `ALOD.Core/Domain/Modules/Lod/LodEnums.cs` (lines 314-347).
- **Logic**:
  - Status flow for informal: MedTechReview (200) → MedicalOfficerReview (201) → UnitCommanderReview (202) → WingJAReview (203, conditional) → AppointingAutorityReview (204 = Wing CC).
  - If `Formal=false`, skips formal steps like FormalInvestigation (206) or BoardReview (208).
  - Wing CC (204) should offer closure if no escalation (e.g., no "Recommend Formal" finding).

### 3. Generating Next Action Options at Wing CC Step
- **File**: `ALOD.Core/Domain/Modules/Lod/LineOfDuty_v2.cs` (lines 108-118 for `GetCurrentOptions()`; lines 1719-1744 for `ProcessOption()`; lines 693-1716 for `ApplyRulesToOption()`).
- **High-Level Flow**:
  - Page loads LOD object and calls `GetCurrentOptions(lastStatus, daoFactory, userId)` (line 108).
  - Validates (e.g., findings), processes docs, then `ProcessOption` (line 1719).
  - **Visibility Pass** (lines 1721-1733): Filters options from `WorkflowStatus.WorkStatusOptionList` using visibility rules (RuleType=1). Adds visible ones to `RuleAppliedOptions`.
  - **Validation Pass** (lines 1733-1744): Applies validation rules (RuleType=2) to set `OptionValid`.

- **Key Rules for Closure**:
  - Visibility (RuleType=1 in `ApplyRulesToOption`):
    - "formal": Hides if rule value ("True") != `lod.Formal` (false) (line 725) — hides formal options in informal.
    - "laststatuswas"/"not": Ensures from WingJAReview (203) (line 843).
    - "formalrecommended": Hides if no investigation needed (line 955).
  - Closure option ("Complete" or "Endorse and Close", status 220 from `LodEnums.cs` line 330):
    - Visible if `Formal=false` and no escalation.
    - Validation (RuleType=2): Checks memos/docs/findings (e.g., "wingccfindings" line 1106 requires Wing CC findings; "boardfinalization" line 875 checks v2 `BoardFinalization`).
    - If passes, `OptionValid=true`, enables closure (sets `FinalFindings`, signs via `AddSignature` line 63).

- **Closure/Endorsement Execution**:
  - On select, calls `AddSignature` (line 63) for memo signing (`MemoDao.cs`).
  - Updates to Complete (220), sets `FinalDecision`/`FinalFindings` (v2 props).
  - Handles finalization (e.g., notifications, auditing).

### 4. Potential Bug Reasons
- **Rule Misapplication**: Closure might have inverted "formal" rule or be hidden by findings (e.g., Unit sets "Recommend Formal" in `Unit.aspx` lines 434-444 via `rblInLOD_v2`).
- **Status Mismatch**: If lastStatus != 203, hides closure (line 843).
- **v2-Specific Checks**: Unmet v2 validations (e.g., `BoardFinalization`) set valid=false (line 875).
- **UI Issue**: Page may hardcode formal options, ignoring `GetCurrentOptions()` (e.g., Unit.aspx lines 106-144 shows findings radio; similar for Wing CC).

### Recommendations
- **Debug**: Create informal LOD_v2, inspect `Formal` and `GetCurrentOptions()` at status 204.
- **Fix**: Adjust DB rules (`WorkflowOptionRulesDao.cs`) for closure visibility when Formal=false.

(Generated from codebase analysis on 2025-09-29.)