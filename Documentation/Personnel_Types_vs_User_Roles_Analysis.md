# Personnel Types vs User Roles Analysis

## Overview

This document analyzes the relationship between **Personnel Types** (used in `FORM348_findings.ptype`) and **User Groups/Roles** (used for system access control). While they appear similar, they serve different purposes in the system architecture.

## Key Differences

### Personnel Types (ptype)
- **Purpose**: Identify the **role/position** of the person making a finding in the LOD determination process
- **Scope**: Specific to findings and workflow stages
- **Usage**: Stored in `FORM348_findings.ptype` to track who made what finding
- **Context**: Represents the organizational position when making a decision

### User Groups/Roles
- **Purpose**: Control **system access** and permissions for users
- **Scope**: System-wide access control and security
- **Usage**: Determine what pages/features a user can access
- **Context**: Represents the user's system permissions and capabilities

## Corrected Mapping Analysis (Based on Database Investigation)

### Actual Personnel Types to User Groups Mapping (Informal LOD)

| Personnel Type | ptype | User Group | groupId | Description | Workflow Role |
|----------------|-------|------------|---------|-------------|---------------|
| `MED_TECH` | 1 | Medical Technician | 3 | Medical Technician | Initial medical review |
| `MED_OFF` | 2 | Medical Officer | 4 | Medical Officer | Medical officer review |
| `UNIT_CMDR` | 3 | Unit Commander | 2 | Unit Commander | Unit-level determination |
| `WING_JA` | 4 | Wing Judge Advocate | 6 | Wing Judge Advocate | Legal review |
| `APPOINT_AUTH` | 5 | Wing Commander | 5 | Wing Commander | Appointing authority |
| `BOARD` | 6 | Board Technician | 7 | Board Technician | Board coordination |
| `BOARD_JA` | 7 | Board Legal | 8 | Board Legal | Board legal review |
| `BOARD_SG` | 8 | Board Medical | 9 | Board Medical | Board medical review |
| `BOARD_AA` | 10 | Approving Authority | 11 | Approving Authority | Board approving authority |
| `BOARD_A1` | 22 | Board Administrator | 97 | Board Administrator | Board administrative functions |
| `WING_SARC_RSL` | 25 | Wing SARC | 25 | Wing SARC | Real initiator for restricted SARC cases |
| `SARC_ADMIN` | 26 | SARC Administrator | 103 | SARC Administrator | Reviewer and finalizer for SARC cases |

### Key Corrections from Database Investigation:

1. **BOARD_AA (10) maps to "Approving Authority" (11), NOT "Board Approving Authority"**
2. **BOARD (6) maps to "Board Technician" (7), NOT "Approving Authority"**
3. **All mappings are 1:1 for informal LOD workflow**

### Complex Mappings (1:Many or Context-Dependent)

#### Board Technician → Multiple Personnel Types
- **User Group**: `BoardTechnician` (7)
- **Personnel Types**: 
  - `BOARD` (6) - for informal cases
  - `FORMAL_BOARD_RA` (14) - for formal cases

#### Wing Commander → Multiple Personnel Types
- **User Group**: `WingCommander` (5)
- **Personnel Types**:
  - `APPOINT_AUTH` (5) - for informal cases
  - `FORMAL_APP_AUTH` (13) - for formal cases

#### Wing Judge Advocate → Multiple Personnel Types
- **User Group**: `WingJudgeAdvocate` (6)
- **Personnel Types**:
  - `WING_JA` (4) - for informal cases
  - `FORMAL_WING_JA` (12) - for formal cases

### Missing Mappings (Personnel Types without Direct User Groups)

#### BOARD_SR (9) - Board Senior Reviewer
- **Status**: **NO DIRECT USER GROUP**
- **Analysis**: This personnel type exists in the enum but there's no corresponding user group
- **Evidence**: Line 29 in `UserEnums.cs` shows `//BoardSeniorReviewer = 10,` (commented out)
- **Implication**: This role may be handled by other user groups or may be a legacy/planned feature

#### FORMAL_BOARD_SR (17) - Formal Board Senior Reviewer
- **Status**: **NO DIRECT USER GROUP**
- **Analysis**: Similar to BOARD_SR, no corresponding user group exists

## Key Findings

### 1. BOARD_AA vs Approving Authority
**Your observation was correct!** 
- `BOARD_AA` (ptype = 10) maps to `Approving Authority` (UserGroup = 11)
- The naming is slightly different but they represent the same role
- **Note**: There is NO "Board Approving Authority" user group - it's just "Approving Authority"

### 2. Missing User Groups
Several personnel types don't have direct user group mappings:
- `BOARD_SR` (9) - Board Senior Reviewer
- `FORMAL_BOARD_SR` (17) - Formal Board Senior Reviewer
- `FORMAL_BOARD_RA` (14) - Formal Board Reviewing Authority
- `FORMAL_BOARD_JA` (15) - Formal Board Legal
- `FORMAL_BOARD_SG` (16) - Formal Board Medical
- `FORMAL_BOARD_AA` (18) - Formal Board Approving Authority
- `FORMAL_BOARD_A1` (23) - Formal Board Admin

### 3. Context-Dependent Mappings
The system uses a `formal` parameter to determine which personnel type to use:
- **Informal cases**: Use standard personnel types (e.g., `BOARD_JA`)
- **Formal cases**: Use formal personnel types (e.g., `FORMAL_BOARD_JA`)

## Code Evidence

### Mapping Function
**File**: `ALODWebUtility/Common/Utility.vb`
```vb
Public Function GetPersonnelTypeByUserGroup(ByVal userGroup As UserGroups, ByVal formal As Boolean) As PersonnelTypes?
    Select Case userGroup
        Case UserGroups.BoardApprovalAuthority
            If (formal) Then
                Return PersonnelTypes.FORMAL_BOARD_AA
            Else
                Return PersonnelTypes.BOARD_AA
            End If
        ' ... other cases
    End Select
End Function
```

### Usage in Web Pages
**File**: `ALOD/Secure/lod/lodBoard.aspx.vb`
```vb
' Board Approving Authority findings
LoadFindingsControl(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, UserGroups.BoardApprovalAuthority)
```

## Architectural Implications

### 1. Separation of Concerns
- **Personnel Types**: Workflow and findings tracking
- **User Groups**: System access and permissions
- **Relationship**: User groups determine access, personnel types determine workflow position

### 2. Formal vs Informal Distinction
The system distinguishes between:
- **Informal LOD cases**: Standard personnel types
- **Formal LOD cases**: Formal personnel types (with "FORMAL_" prefix)

### 3. Missing Roles
Some personnel types exist without corresponding user groups, suggesting:
- Legacy functionality
- Planned features not yet implemented
- Roles handled by other user groups
- System evolution over time

## Restricted SARC Workflow Analysis

### Specialized Role Hierarchy
The Restricted SARC workflow demonstrates a unique role hierarchy where personnel types have specialized functions:

| Personnel Type | ptype | User Group | groupId | Role Function | Special Behavior |
|----------------|-------|------------|---------|---------------|------------------|
| `MED_TECH` | 1 | Medical Technician | 3 | Can initiate | **Removed from tracking after forwarding** |
| `WING_SARC_RSL` | 25 | Wing SARC | 25 | Real initiator | **Can initiate directly or receive forwarded cases** |
| `SARC_ADMIN` | 26 | SARC Administrator | 103 | Reviewer/Finalizer | **Dual role: both reviewer and finalizer** |
| `BOARD_AA` | 10 | Approving Authority | 11 | Board approval | **Final decision authority** |

### Key Characteristics:
1. **Medical Technician**: Can initiate but is removed from tracking after forwarding to Wing SARC
2. **Wing SARC**: Real initiator who can either initiate directly or receive forwarded cases
3. **SARC Administrator**: Serves dual role as both reviewer and finalizer
4. **Approving Authority**: Board-level approval for final decisions

### Workflow Flow:
```
Medical Technician (Initiation) -> Wing SARC (Real Initiator/Review) -> SARC Administrator (Administration/Support) -> Approving Authority (Approval/Decision) -> SARC Administrator (Finalizer) -> Complete
```

## Complete LOD Board Panel Role Mapping Table

This table provides the definitive mapping for all LOD Board panels and their associated roles, including both informal and formal workflows.

| Panel Name | Role | UserGroup ID | PersonnelType | SectionName | Status Code Required | Workflow Type |
|------------|------|--------------|---------------|-------------|---------------------|---------------|
| **Informal Board Review** | Board Technician | 7 | `BOARD` (6) | `BOARD_REV` (10) | `BoardReview` (11) | Informal |
| **Informal Board Medical Review** | Board Medical | 9 | `BOARD_SG` (8) | `BOARD_MED_REV` (11) | `BoardMedicalReview` (12) | Informal |
| **Informal Board Legal Review** | Board Legal | 8 | `BOARD_JA` (7) | `BOARD_LEGAL_REV` (12) | `BoardLegalReview` (13) | Informal |
| **Informal Board Personnel Review** | Board Administrator | 97 | `BOARD_A1` (22) | `BOARD_PERSONNEL_REV` (23) | `BoardPersonnelReview` (169) | Informal |
| **Informal Approving Authority Review** | Approving Authority | 11 | `BOARD_AA` (10) | `BOARD_APPROVING_AUTH_REV` (14) | `ApprovingAuthorityAction` (15) | Informal |
| **Formal Board Reviewing Authority Review** | Board Technician | 7 | `FORMAL_BOARD_RA` (14) | `FORMAL_BOARD_REV` (15) | `FormalBoardReview` (20) | Formal |
| **Formal Board Medical Review** | Board Medical | 9 | `FORMAL_BOARD_SG` (16) | `FORMAL_BOARD_MED_REV` (16) | `FormalBoardMedicalReview` (21) | Formal |
| **Formal Board Legal Review** | Board Legal | 8 | `FORMAL_BOARD_JA` (15) | `FORMAL_BOARD_LEGAL_REV` (17) | `FormalBoardLegalReview` (22) | Formal |
| **Formal Board Personnel Review** | Board Administrator | 97 | `FORMAL_BOARD_A1` (23) | `FORMAL_BOARD_PERSONNEL_REV` (24) | `FormalBoardPersonnelReview` (170) | Formal |
| **Formal Approving Authority Review** | Approving Authority | 11 | `FORMAL_BOARD_AA` (18) | `FORMAL_BOARD_APPROVING_AUTH_REV` (19) | `FormalApprovingAuthorityAction` (24) | Formal |
| **Formal Appointing Authority Action** | Wing Commander | 5 | `FORMAL_APP_AUTH` (13) | `FORMAL_ACTION_APP_AUTH` (9) | `FormalActionByAppointingAuthority` (10) | Formal |

### Key Insights from Complete Mapping:

1. **Dual Role System**: Board Technician serves dual roles:
   - **Informal**: `BOARD` (6) for informal board review
   - **Formal**: `FORMAL_BOARD_RA` (14) for formal board reviewing authority

2. **Formal vs Informal Distinction**: Each board role has both informal and formal personnel types:
   - **Informal**: Standard personnel types (e.g., `BOARD_SG`)
   - **Formal**: Formal personnel types with "FORMAL_" prefix (e.g., `FORMAL_BOARD_SG`)

3. **Wing Commander Special Case**: The **Formal Appointing Authority Action** panel exists in LOD Board page for final substituted findings after formal investigation, while the Wing CC page handles initial formal investigation actions.

4. **Access Control**: Each panel's visibility and editability is controlled by:
   - **User Group**: Determines if user can access the panel
   - **Status Code**: Determines if panel is editable (ReadWrite) or read-only
   - **Formal Flag**: Formal panels only accessible when `lod.Formal = true`

5. **Workflow Progression**: The panels follow a logical progression:
   - **Informal**: Board Review → Board Medical/Legal/Personnel → Approving Authority
   - **Formal**: Formal Board Review → Formal Board Medical/Legal/Personnel → Formal Approving Authority → Formal Appointing Authority Action

### Implementation Notes:

- **Access Control**: Implemented via `ReadSectionList()` method in `LineOfDuty.cs`
- **Panel Visibility**: Controlled by `EnableSections()` method in `lodBoard.aspx.vb`
- **Status Validation**: Each panel checks for specific status codes before allowing access
- **Formal Flag Check**: Formal panels require `lod.Formal = true` to be accessible

This mapping ensures proper role-based access control and maintains the integrity of the LOD determination workflow across both informal and formal processes.

## Conclusion

**Personnel Types** and **User Groups** are related but serve different purposes:

1. **Personnel Types** identify the organizational role when making findings
2. **User Groups** control system access and permissions
3. **Most mappings are 1:1**, but some are context-dependent (formal vs informal)
4. **Some personnel types lack direct user group mappings**, indicating system evolution
5. **BOARD_AA does map to Approving Authority** - your observation was correct!
6. **Restricted SARC workflow** demonstrates specialized role hierarchy with unique tracking behavior
7. **LOD Board panels** have specific role mappings that control both visibility and editability

The system uses both concepts together: user groups determine what a user can access, while personnel types determine what role they're acting in when making findings. The Restricted SARC workflow shows how certain personnel types can have specialized behaviors (like being removed from tracking after forwarding) that differ from standard workflow patterns. The Complete LOD Board Panel Role Mapping Table above provides the definitive reference for all board-level panel access control.

