## RemoveHardCodedDBName.ps1

### Overview
This PowerShell script scans a SQL Server database for object definitions (stored procedures, functions, views, and triggers) that reference the database-qualified schema names `ALOD.dbo` or `[ALOD].dbo` (in any bracket/case variation). It then removes the hardcoded database name (`ALOD` or `[ALOD]`), keeping only `dbo`, so the objects no longer depend on a specific database name. This helps make cross-database deployments and renames easier.

The script can:
- Identify affected objects and export their definitions to individual `.sql` files
- Output a CSV list of all affected objects
- Interactively drop and recreate those objects with cleaned definitions (after confirmation)
- Produce execution and error logs

### What it looks for
The script searches object definitions for any of the following patterns (case-insensitive):
- `[ALOD].[dbo]`
- `[ALOD].dbo`
- `ALOD.[dbo]`
- `ALOD.dbo`

These matches are replaced so that only `dbo` remains.

### Objects affected
- Stored procedures
- Scalar and table-valued functions
- Views
- Triggers

Tables are not modified.

### Prerequisites
- Windows PowerShell or PowerShell 7
- SQL Server connectivity to the target instance
- PowerShell `SqlServer` module (for `Invoke-Sqlcmd`)
  - Install if needed: `Install-Module -Name SqlServer -Scope CurrentUser`
- Permissions sufficient to query `sys.objects` and to drop/recreate affected objects

### Configuration
Edit the top of the script to point to your SQL Server and database:

```powershell
$server = "D-DII-JI01-01\MSSQLSERVER2022"
$database = "ALOD"
$connectionString = "Server=$server;Database=$database;Integrated Security=True"
```

Optional: change the output directory name (defaults to `FilteredObjectDefinitions`).

### How it works (high level)
1. Queries `sys.objects` and `OBJECT_DEFINITION` to find objects containing the ALOD-dbo patterns.
2. Exports each matching object definition to `FilteredObjectDefinitions/<Schema>_<Name>_<Type>.sql`.
3. Writes a summary CSV: `filtered_objects_list.csv`.
4. Prompts for confirmation before making any changes.
5. If confirmed:
   - Computes a cleaned definition by replacing `ALOD.dbo` variants with `dbo`.
   - Drops the object (if it exists) using the appropriate `DROP` statement.
   - Recreates the object from the cleaned definition.
   - Logs results to `ExecutionLog.txt` and errors to `ExecutionErrors.txt`.

### Usage
1. Open PowerShell in the repository root or the `Scripts` folder.
2. Ensure the `SqlServer` module is available.
3. Edit the script variables (`$server`, `$database`) as needed.
4. Run the script:

```powershell
pwsh ./Scripts/RemoveHardCodedDBName.ps1
# or
powershell -File .\Scripts\RemoveHardCodedDBName.ps1
```

5. Review the printed list of affected objects and the exported `.sql` files.
6. When prompted, enter `y` to proceed with drop-and-recreate, or `n` to abort.

### Output
- `FilteredObjectDefinitions/` — individual `.sql` files of the matched object definitions
- `filtered_objects_list.csv` — list of matched objects with schema, name, and type
- `ExecutionLog.txt` — success messages when objects are recreated
- `ExecutionErrors.txt` — errors (if any) encountered during execution

### Safety and limitations
- Dropping and recreating objects can remove object-level permissions, extended properties, and dependencies not embedded in the definition. Review and re-apply as needed.
- The script does not wrap changes in a transaction; it processes objects one by one.
- Only objects whose definitions contain the target patterns are changed. Tables are ignored.
- The replacement is regex-based and case-insensitive; it reduces `ALOD.dbo` variants to `dbo`.
- Run first in a non-production environment and ensure you have backups/source control.

### Customization notes
- To adjust what gets replaced, edit the `$pattern` variable inside the script. By default it targets any case of `ALOD.dbo` with or without brackets around `ALOD` or `dbo`.
- To change which objects are considered, modify the `WHERE` clause in the T-SQL query assigned to `$query`.

### Troubleshooting
- If `Invoke-Sqlcmd` is not recognized, install the `SqlServer` module and import it: `Import-Module SqlServer`.
- Collation-sensitive environments are handled by applying `COLLATE SQL_Latin1_General_CP1_CI_AS` within the search query.
- If you see recreate errors, inspect the corresponding file in `FilteredObjectDefinitions/` and the entry in `ExecutionErrors.txt`.

### License and ownership
Internal tooling for ECTSystem. If publishing externally, apply the appropriate project license.


