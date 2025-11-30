# FORM348_findings ptype Field Analysis

## Overview

This document provides a comprehensive analysis of the `FORM348_findings` table's `ptype` field, including its relationship to the `PersonnelTypes` enum, mapping of values used in `FindByType()` calls, and locations where records are inserted into the table.

## Table Structure

The `FORM348_findings` table is mapped to the `LineOfDutyFindings` entity with the following key fields:

| Field | Type | Description |
|-------|------|-------------|
| `ID` | Primary Key | Unique identifier for the finding record |
| `LODID` | Foreign Key | References the Line of Duty case |
| `ptype` | short | Personnel Type identifier (the focus of this analysis) |
| `finding` | short? | The actual finding value |
| `decision_yn` | string | Decision Yes/No indicator |
| `explanation` | string | Text explanation of the finding |
| `ssn` | string | Social Security Number |
| `name` | string | Personnel name |
| `grade` | string | Military grade |
| `rank` | string | Military rank |
| `compo` | string | Component (Active, Reserve, Guard) |
| `pascode` | string | Personnel Accounting Symbol Code |
| `created_by` | int? | User who created the record |
| `created_date` | DateTime? | Creation timestamp |
| `modified_by` | int | User who last modified the record |
| `modified_date` | DateTime | Last modification timestamp |

## PersonnelTypes Enum

The `ptype` field corresponds to the `PersonnelTypes` enum defined in `ALOD.Core/Domain/Modules/Lod/LodEnums.cs`:

```csharp
public enum PersonnelTypes : short
{
    MED_TECH = 1,                    // Medical Technician
    MED_OFF = 2,                     // Medical Officer
    UNIT_CMDR = 3,                   // Unit Commander
    WING_JA = 4,                     // Wing Judge Advocate
    APPOINT_AUTH = 5,                // Wing CC (Appointing Authority)
    BOARD = 6,                       // Approving Authority
    BOARD_JA = 7,                    // Board Legal
    BOARD_SG = 8,                    // Board Medical
    BOARD_SR = 9,                    // Board Senior Reviewer
    BOARD_AA = 10,                   // Board Approving Authority
    MPF = 11,                        // Military Personnel Flight
    FORMAL_WING_JA = 12,             // Formal Wing JA
    FORMAL_APP_AUTH = 13,            // Formal Appointing Authority
    FORMAL_BOARD_RA = 14,            // Formal Board Reviewing Authority
    FORMAL_BOARD_JA = 15,            // Formal Board Legal
    FORMAL_BOARD_SG = 16,            // Formal Board Medical
    FORMAL_BOARD_SR = 17,            // Formal Board Senior Reviewer
    FORMAL_BOARD_AA = 18,            // Formal Board Approving Authority
    IO = 19,                         // Investigating Officer
    LOD_MFP = 20,                    // LOD Military Personnel Flight
    LOD_PM = 21,                     // LOD Program Manager
    BOARD_A1 = 22,                   // Board Admin
    FORMAL_BOARD_A1 = 23,            // Formal Board Admin
    APPELLATE_AUTH = 24,             // Appellate Authority
    WING_SARC_RSL = 25,              // Wing SARC (Real Initiator)
    SARC_ADMIN = 26,                 // SARC Administrator (Reviewer and Finalizer)
    SENIOR_MEDICAL_REVIEWER = 27,    // Senior Medical Reviewer
    FORMAL_SENIOR_MEDICAL_REVIEWER = 28 // Formal Senior Medical Reviewer
}
```

## Personnel Types to User Groups Mapping (Informal LOD)

Based on actual database investigation, here is the **correct mapping** between Personnel Types and User Groups for informal LOD workflow:

| Personnel Type | ptype | User Group | groupId | Description | Workflow Role |
|----------------|-------|------------|---------|-------------|---------------|
| MED_TECH | 1 | Medical Technician | 3 | Medical Technician | Initial medical review |
| MED_OFF | 2 | Medical Officer | 4 | Medical Officer | Medical officer review |
| UNIT_CMDR | 3 | Unit Commander | 2 | Unit Commander | Unit-level determination |
| WING_JA | 4 | Wing Judge Advocate | 6 | Wing Judge Advocate | Legal review |
| APPOINT_AUTH | 5 | Wing Commander | 5 | Wing Commander | Appointing authority |
| BOARD | 6 | Board Technician | 7 | Board Technician | Board coordination |
| BOARD_JA | 7 | Board Legal | 8 | Board Legal | Board legal review |
| BOARD_SG | 8 | Board Medical | 9 | Board Medical | Board medical review |
| BOARD_AA | 10 | Approving Authority | 11 | Approving Authority | Board approving authority |
| BOARD_A1 | 22 | Board Administrator | 97 | Board Administrator | Board administrative functions |

## Informal LOD Workflow Process

### Workflow Path:
```
Med Tech → Med Officer → Unit CC → Wing JA → Wing CC → Board Tech → [Board Medical/Board Legal/Board Approving Authority/Board Administrator] → Complete
```

### Board Review Process:
1. **Board Tech** (ptype = 6) - Initial board coordination
2. **Recycle Process**: Any Board role can forward to other Board roles:
   - Board Medical (ptype = 8)
   - Board Legal (ptype = 7) 
   - Board Approving Authority (ptype = 10)
   - Board Administrator (ptype = 22)
   - Board Tech (ptype = 6)
3. **Complete**: Board Tech can complete the process

### Finding Tracking Table (Informal LOD):

| User Group | groupId | Action | Insert | Update | ptype | Description |
|------------|---------|--------|--------|--------|-------|-------------|
| Unit CC | 2 | Review | 1 | 1 | 3 | Unit Commander findings |
| Wing JA | 6 | Review | 1 | 1 | 4 | Wing Judge Advocate findings |
| Wing CC | 5 | Review | 1 | 1 | 5 | Wing Commander findings |
| Board Tech | 7 | Review | 0 | 1 | - | Board Technician review (no finding) |
| Board Tech | 7 | Next Action | 1 | 1 | 6 | Board Technician findings |
| Board Medical | 9 | Review | 1 | 1 | 8 | Board Medical findings |
| Board Legal | 8 | Review | 1 | 1 | 7 | Board Legal findings |
| Board Approving Authority | 11 | Review | 1 | 1 | 10 | Board Approving Authority findings |
| Board Administrator | 97 | Review | 1 | 1 | 22 | Board Administrator findings |

## FindByType() Mapping

The following table maps the ptype values used in `FindByType()` calls throughout the codebase to their corresponding personnel types:

| FindByType Value | Personnel Type | Description | Usage Context |
|------------------|----------------|--------------|---------------|
| 3 | UNIT_CMDR | Unit Commander | Unit-level findings and determinations |
| 4 | WING_JA | Wing Judge Advocate | Legal review and recommendations |
| 5 | APPOINT_AUTH | Appointing Authority (Wing CC) | Final authority decisions |
| 6 | BOARD | Board Technician | Board coordination and findings |
| 7 | BOARD_JA | Board Legal | Legal review at board level |
| 8 | BOARD_SG | Board Medical | Medical review at board level |
| 9 | BOARD_SR | Board Senior Reviewer | Senior reviewer assessments |
| 10 | BOARD_AA | Board Approving Authority | Board-level approving authority |
| 12 | FORMAL_WING_JA | Formal Wing JA | Formal legal proceedings |
| 13 | FORMAL_APP_AUTH | Formal Appointing Authority | Formal appointing authority decisions |
| 14 | FORMAL_BOARD_RA | Formal Board Reviewing Authority | Formal board review authority |
| 15 | FORMAL_BOARD_JA | Formal Board Legal | Formal board legal review |
| 16 | FORMAL_BOARD_SG | Formal Board Medical | Formal board medical review |
| 17 | FORMAL_BOARD_SR | Formal Board Senior Reviewer | Formal senior reviewer |
| 18 | FORMAL_BOARD_AA | Formal Board Approving Authority | Formal board approving authority |
| 19 | IO | Investigating Officer | Investigation findings |
| 22 | BOARD_A1 | Board Administrator | Board administrative functions |
| 23 | FORMAL_BOARD_A1 | Formal Board Administrator | Formal board administrative functions |

## Record Insertion Locations

### 1. NHibernate Session Manager
**File**: `ALOD.Data/NHibernateSessionManager.cs`
**Method**: `InsertFinding(LineOfDutyFindings finding)`

```csharp
public void InsertFinding(LineOfDutyFindings finding)
{
    if (finding.LODID.HasValue && DoesFindingExist(finding.LODID.Value, finding.PType))
    {
        throw new InvalidOperationException($"A finding with LODID {finding.LODID} and ptype {finding.PType} already exists.");
    }

    if (!IsValidPType(finding.PType))
    {
        throw new InvalidOperationException($"The PType {finding.PType} is not valid.");
    }

    ISession session = GetSession();
    using (ITransaction transaction = session.BeginTransaction())
    {
        try
        {
            session.Save(finding);
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("An error occurred while inserting the finding.", ex);
        }
    }
}
```

### 2. Web Application Pages

Records are created and saved through various web pages in the `ALOD/Secure/lod/` directory:

#### Unit Commander Findings
**File**: `ALOD/Secure/lod/Unit.aspx.vb`
- Handles unit-level findings and determinations
- Uses `PersonnelTypes.UNIT_CMDR` (ptype = 3)

#### Wing Commander Findings
**File**: `ALOD/Secure/lod/WingCC.aspx.vb`
- Handles appointing authority decisions
- Uses `PersonnelTypes.APPOINT_AUTH` (ptype = 5)
- Handles formal actions with `PersonnelTypes.FORMAL_APP_AUTH` (ptype = 13)

#### Wing Judge Advocate Findings
**File**: `ALOD/Secure/lod/WingJA.aspx.vb`
- Handles legal review and recommendations
- Uses `PersonnelTypes.WING_JA` (ptype = 4)
- Handles formal actions with `PersonnelTypes.FORMAL_WING_JA` (ptype = 12)

#### Board Findings
**File**: `ALOD/Secure/lod/lodBoard.aspx.vb`
- Handles board-level findings and approvals
- Uses various board personnel types (ptype 6-11, 22-23)

#### Senior Medical Reviewer Findings
**File**: `ALOD/Secure/lod/SeniorMedReviewer.aspx.vb`
- Handles senior medical reviewer assessments
- Uses `PersonnelTypes.SENIOR_MEDICAL_REVIEWER` (ptype = 27)

### 3. Common Record Creation Pattern

The typical pattern for creating findings in the web application is:

```vb
Dim finding As LineOfDutyFindings = CreateFinding(RefId)
finding.PType = PersonnelTypes.WING_JA  ' or other personnel type
finding.DecisionYN = "Y"  ' or "N"
finding.Finding = someFindingValue
finding.Explanation = "Some explanation"
finding.FindingsText = "Detailed findings text"
lod.SetFindingByType(finding)
```

### 4. Validation and Lookup

#### PType Validation
**File**: `ALOD.Data/NHibernateSessionManager.cs`
**Method**: `IsValidPType(short ptype)`

```csharp
public bool IsValidPType(short ptype)
{
    ISession session = GetSession();
    var query = session.CreateQuery("SELECT COUNT(*) FROM core_lkupPersonnelTypes WHERE Id = :ptype");
    query.SetParameter("ptype", ptype);
    return (long)query.UniqueResult() > 0;
}
```

#### Duplicate Prevention
**Method**: `DoesFindingExist(int lodId, int ptype)`

```csharp
public bool DoesFindingExist(int lodId, int ptype)
{
    ISession session = GetSession();
    var query = session.CreateQuery("SELECT COUNT(*) FROM LineOfDutyFindings WHERE LODID = :lodId AND PType = :ptype");
    query.SetParameter("lodId", lodId);
    query.SetParameter("ptype", ptype);
    return (long)query.UniqueResult() > 0;
}
```

## Related Tables

### Lookup Tables
- `core_lkupPersonnelTypes` - Validates ptype values
- `core_lkupWorkflowFindings` - Defines available findings by workflow

### Related Findings Tables
- `Form348_AP_Findings` - Appeal findings (similar structure)
- `Form348_RR_Findings` - Reinvestigation findings (similar structure)
- `Form348_AP_SARC_Findings` - SARC appeal findings

## Workflow Integration

The system uses a hierarchical workflow approach where different personnel types can make findings at different stages:

1. **Unit Level**: Unit Commander (ptype = 3)
2. **Wing Level**: Wing JA (ptype = 4), Wing CC (ptype = 5)
3. **Board Level**: Various board personnel (ptype = 6-11, 22-23)
4. **Formal Proceedings**: Formal personnel types (ptype = 12-18, 23)
5. **Investigation**: Investigating Officer (ptype = 19)
6. **Special Cases**: SARC, Senior Medical Reviewer (ptype = 25-28)
7. **Restricted SARC**: Wing SARC (Real Initiator), SARC Administrator (Reviewer/Finalizer) (ptype = 25-26)

## Key Methods

### FindByType Method
**File**: `ALOD.Core/Domain/Modules/Lod/LineOfDuty.cs`
```csharp
public virtual LineOfDutyFindings FindByType(short pType)
{
    IEnumerable<LineOfDutyFindings> lst = (from p in LodFindings where p.PType == pType select p);
    if (lst.Count() > 0)
    {
        LineOfDutyFindings old = lst.First();
        return old;
    }
    return null;
}
```

### SetFindingByType Method
**File**: `ALOD.Core/Domain/Modules/Lod/LineOfDuty.cs`
```csharp
public virtual LineOfDutyFindings SetFindingByType(LineOfDutyFindings fnd)
{
    // Implementation handles finding creation/update logic
}
```

## Restricted SARC Workflow Analysis

### Role Hierarchy and Tracking
Based on role analysis, the Restricted SARC workflow follows this pattern:

| Personnel Type | ptype | Role | Description | Workflow Position |
|----------------|-------|------|-------------|-------------------|
| `MED_TECH` | 1 | Medical Technician | Can initiate but removed from tracking after forwarding | Initial entry point |
| `WING_SARC_RSL` | 25 | Wing SARC | Real initiator (can initiate or receive forwarded cases) | Primary review authority |
| `SARC_ADMIN` | 26 | SARC Administrator | Reviewer and Finalizer role | Administration and closure |
| `BOARD_AA` | 10 | Approving Authority | Board-level approval | Final decision authority |

### Key Findings for Restricted SARC:
1. **Medical Technician** (ptype=1): Can initiate but is removed from tracking after forwarding to Wing SARC
2. **Wing SARC** (ptype=25): Real initiator who can either initiate directly or receive forwarded cases
3. **SARC Administrator** (ptype=26): Serves dual role as both reviewer and finalizer
4. **Approving Authority** (ptype=10): Board-level approval for final decisions

### Workflow Flow:
```
Medical Technician (Initiation) -> Wing SARC (Real Initiator/Review) -> SARC Administrator (Administration/Support) -> Approving Authority (Approval/Decision) -> SARC Administrator (Finalizer) -> Complete
```

## Conclusion

The `ptype` field in `FORM348_findings` serves as a critical identifier that links findings to specific personnel roles in the Line of Duty determination process. The system maintains referential integrity through lookup tables and validation methods, ensuring that only valid personnel types can create findings at appropriate workflow stages.

The hierarchical nature of the personnel types reflects the military organizational structure, with each level having specific authorities and responsibilities in the LOD determination process. The Restricted SARC workflow demonstrates a specialized role hierarchy where certain personnel types (like Medical Technician) can initiate but are removed from tracking after forwarding, while others (like Wing SARC) serve as real initiators with full workflow authority.
