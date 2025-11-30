using ALOD.Core.Domain.PsychologicalHealth;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace ALOD.Data
{
    public class PsychologicalHealthDao : IPsychologicalHealthDao
    {
        #region SQL DataSource Property

        private SqlDataStore _dataSource;

        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        #endregion SQL DataSource Property

        #region Selects...

        /// <summary>
        /// Retrieves all PH (Psychological Health) fields.
        /// Executes stored procedure: PH_Field_sp_GetAll
        /// </summary>
        /// <returns>A list of PHField objects.</returns>
        public IList<PHField> GetAllFields()
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_Field_sp_GetAll");

            return DataHelpers.ExtractObjectsFromDataSet<PHField>(dSet);
        }

        /// <summary>
        /// Retrieves all PH field types.
        /// Executes stored procedure: PH_FieldType_sp_GetAll
        /// </summary>
        /// <returns>A list of PHFieldType objects.</returns>
        public IList<PHFieldType> GetAllFieldTypes()
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FieldType_sp_GetAll");

            return DataHelpers.ExtractObjectsFromDataSet<PHFieldType>(dSet);
        }

        /// <summary>
        /// Retrieves all PH form fields.
        /// Executes stored procedure: PH_FormField_sp_GetAll
        /// </summary>
        /// <returns>A list of PHFormField objects.</returns>
        public IList<PHFormField> GetAllFormFields()
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FormField_sp_GetAll");

            return DataHelpers.ExtractObjectsFromDataSet<PHFormField>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves all PH sections with hierarchical child relationships.
        /// Executes stored procedure: PH_Section_sp_GetAll
        /// </summary>
        /// <returns>A list of PHSection objects with populated Children collections.</returns>
        public IList<PHSection> GetAllSections()
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_Section_sp_GetAll");

            IList<PHSection> results = DataHelpers.ExtractObjectsFromDataSet<PHSection>(dSet);

            foreach (PHSection s in results)
            {
                // Every PHSection is already in memory; therefore, build up children list from the objects
                // in memory instead of querying the database again...
                foreach (PHSection ss in results)
                {
                    if (ss.Id != s.Id && ss.ParentId == s.Id)
                    {
                        s.Children.Add(ss);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves the case ID for a specific reporting period and case unit.
        /// Executes stored procedure: PH_Workflow_sp_GetCaseIdForReportingPeriod
        /// </summary>
        /// <param name="reportingPeriod">The reporting period date.</param>
        /// <param name="caseUnitId">The ID of the case unit.</param>
        /// <returns>The case ID, or 0 if not found.</returns>
        public int GetCaseIdByReportingPeriod(DateTime reportingPeriod, int caseUnitId)
        {
            Object result = DataSource.ExecuteScalar("PH_Workflow_sp_GetCaseIdForReportingPeriod", reportingPeriod, caseUnitId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Retrieves all PH cases for a specific Wing/RMU.
        /// Executes stored procedure: PH_Workflow_sp_GetCasesByWingRMU
        /// </summary>
        /// <param name="caseUnitId">The ID of the Wing/RMU unit.</param>
        /// <returns>A DataSet containing cases for the specified unit.</returns>
        public DataSet GetCasesByCaseUnit(int caseUnitId)
        {
            return DataSource.ExecuteDataSet("PH_Workflow_sp_GetCasesByWingRMU", caseUnitId);
        }

        /// <summary>
        /// Executes a stored procedure and returns the result as a DataSet.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <returns>A DataSet containing the stored procedure results.</returns>
        public DataSet GetDataSetFromProcedure(string storedProcedureName)
        {
            return DataSource.ExecuteDataSet(storedProcedureName);
        }

        /// <summary>
        /// Retrieves a specific PH field by its ID.
        /// Executes stored procedure: PH_Field_sp_GetById
        /// </summary>
        /// <param name="id">The ID of the field.</param>
        /// <returns>The PHField object, or null if not found.</returns>
        public PHField GetFieldById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_Field_sp_GetById", id);

            return DataHelpers.ExtractObjectFromDataSet<PHField>(dSet);
        }

        /// <summary>
        /// Retrieves a specific PH field type by its ID.
        /// Executes stored procedure: PH_FieldType_sp_GetById
        /// </summary>
        /// <param name="id">The ID of the field type.</param>
        /// <returns>The PHFieldType object, or null if not found.</returns>
        public PHFieldType GetFieldTypeById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FieldType_sp_GetById", id);

            return DataHelpers.ExtractObjectFromDataSet<PHFieldType>(dSet);
        }

        /// <summary>
        /// Retrieves a specific PH form field by its composite key (section, field, field type).
        /// Executes stored procedure: PH_FormField_sp_GetByIds
        /// </summary>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="fieldId">The ID of the field.</param>
        /// <param name="fieldTypeId">The ID of the field type.</param>
        /// <returns>The PHFormField object, or null if not found.</returns>
        public PHFormField GetFormFieldByIds(int sectionId, int fieldId, int fieldTypeId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FormField_sp_GetByIds", sectionId, fieldId, fieldTypeId);

            return DataHelpers.ExtractObjectFromDataSet<PHFormField>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves all field types for a specific section and field combination.
        /// Executes stored procedure: PH_FormField_sp_GetFieldTypes
        /// </summary>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="fieldId">The ID of the field.</param>
        /// <returns>A list of PHFormField objects representing available field types.</returns>
        public IList<PHFormField> GetFormFieldFieldTypes(int sectionId, int fieldId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FormField_sp_GetFieldTypes", sectionId, fieldId);

            return DataHelpers.ExtractObjectsFromDataSet<PHFormField>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves all PH form values for a specific reference ID (case).
        /// Executes stored procedure: PH_FormValue_sp_GetByRefId
        /// </summary>
        /// <param name="refId">The reference ID of the PH case.</param>
        /// <returns>A list of PHFormValue objects for the specified case.</returns>
        public IList<PHFormValue> GetFormValuesByRefId(int refId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_FormValue_sp_GetByRefId", refId);

            return DataHelpers.ExtractObjectsFromDataSet<PHFormValue>(dSet);
        }

        /// <summary>
        /// Retrieves all Numbered Air Forces (NAFs) for PH workflow.
        /// Executes stored procedure: cmdStruct_sp_GetNumberedAirForcesForPH
        /// </summary>
        /// <returns>A DataSet containing Numbered Air Forces.</returns>
        public DataSet GetNumberedAirForcesForPH()
        {
            return DataSource.ExecuteDataSet("cmdStruct_sp_GetNumberedAirForcesForPH");
        }

        /// <summary>
        /// Retrieves a specific PH section by its ID with all children populated.
        /// Executes stored procedure: PH_Section_sp_GetById
        /// </summary>
        /// <param name="id">The ID of the section.</param>
        /// <returns>The PHSection object with children populated, or null if not found.</returns>
        public PHSection GetSectionById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_Section_sp_GetById", id);

            PHSection result = DataHelpers.ExtractObjectFromDataSet<PHSection>(dSet);

            if (result == null || result.Id == 0)
                return null;

            result.Children = GetSectionChildren(result.Id);

            return result;
        }

        /// <summary>
        /// Recursively retrieves all child sections for a parent section.
        /// Executes stored procedure: PH_Section_sp_GetChildren
        /// </summary>
        /// <param name="parentId">The ID of the parent section.</param>
        /// <returns>A list of PHSection objects representing child sections with their own children populated.</returns>
        public IList<PHSection> GetSectionChildren(int parentId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("PH_Section_sp_GetChildren", parentId);

            IList<PHSection> results = DataHelpers.ExtractObjectsFromDataSet<PHSection>(dSet);

            foreach (PHSection s in results)
            {
                s.Children = GetSectionChildren(s.Id);
            }

            return results;
        }

        /// <summary>
        /// Searches for PH cases based on multiple filter criteria.
        /// Executes stored procedure: PH_Workflow_sp_Search
        /// </summary>
        /// <param name="caseId">The case ID to search for.</param>
        /// <param name="statusId">The status ID filter.</param>
        /// <param name="userId">The user ID performing the search.</param>
        /// <param name="compoId">The component ID filter.</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="rptViewId">The reporting view ID.</param>
        /// <param name="module">The module ID.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="reportingMonth">The reporting month filter.</param>
        /// <param name="reportingYear">The reporting year filter.</param>
        /// <returns>A DataSet containing matching PH cases.</returns>
        public DataSet PHCaseSearch(string caseId, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, int reportingMonth, int reportingYear)
        {
            return DataSource.ExecuteDataSet("PH_Workflow_sp_Search", caseId, statusId, userId, rptViewId, compoId, maxCount, module, unitId, reportingMonth, reportingYear);
        }

        #endregion Selects...

        #region Inserts...

        /// <summary>
        /// Adds a child section to a parent section.
        /// Executes stored procedure: PH_Section_sp_AddChild
        /// </summary>
        /// <param name="parentId">The ID of the parent section.</param>
        /// <param name="childId">The ID of the child section to add.</param>
        /// <param name="displayOrder">The display order of the child section.</param>
        public void AddSectionChild(int parentId, int childId, int displayOrder)
        {
            DataSource.ExecuteNonQuery("PH_Section_sp_AddChild", parentId, childId, displayOrder);
        }

        /// <summary>
        /// Inserts a new PH field.
        /// Executes stored procedure: PH_Field_sp_Insert
        /// </summary>
        /// <param name="name">The name of the new field.</param>
        public void InsertField(string name)
        {
            DataSource.ExecuteNonQuery("PH_Field_sp_Insert", name);
        }

        /// <summary>
        /// Inserts a new PH field type.
        /// Executes stored procedure: PH_FieldType_sp_Insert
        /// </summary>
        /// <param name="newFieldType">The PHFieldType object containing field type details.</param>
        public void InsertFieldType(PHFieldType newFieldType)
        {
            DataSource.ExecuteNonQuery("PH_FieldType_sp_Insert", newFieldType.Name, newFieldType.DataTypeId, newFieldType.Datasource, newFieldType.Placeholder, (newFieldType.Color.HasValue ? newFieldType.Color.Value.Name : string.Empty), newFieldType.Length);
        }

        /// <summary>
        /// Inserts a new PH form field mapping.
        /// Executes stored procedure: PH_FormField_sp_Insert
        /// </summary>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="fieldId">The ID of the field.</param>
        /// <param name="fieldTypeId">The ID of the field type.</param>
        /// <param name="fieldDisplayOrder">The display order of the field.</param>
        /// <param name="fieldTypeDisplayOrder">The display order of the field type.</param>
        /// <param name="toolTip">The tooltip text for the field.</param>
        public void InsertFormField(int sectionId, int fieldId, int fieldTypeId, int fieldDisplayOrder, int fieldTypeDisplayOrder, string toolTip)
        {
            DataSource.ExecuteNonQuery("PH_FormField_sp_Insert", sectionId, fieldId, fieldTypeId, fieldDisplayOrder, fieldTypeDisplayOrder, toolTip);
        }

        /// <summary>
        /// Inserts a new PH form value.
        /// Executes stored procedure: PH_FormValue_sp_Insert
        /// </summary>
        /// <param name="refId">The reference ID of the PH case.</param>
        /// <param name="sectionId">The ID of the section.</param>
        /// <param name="fieldId">The ID of the field.</param>
        /// <param name="fieldTypeId">The ID of the field type.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>True if the value was successfully inserted; otherwise, false.</returns>
        public bool InsertFormValue(int refId, int sectionId, int fieldId, int fieldTypeId, string value)
        {
            Object result = DataSource.ExecuteScalar("PH_FormValue_sp_Insert", refId, sectionId, fieldId, fieldTypeId, value);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Inserts a new PH section.
        /// Executes stored procedure: PH_Section_sp_Insert
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="parentName">The name of the parent section.</param>
        /// <param name="fieldColumns">The number of field columns.</param>
        /// <param name="isTopLevel">Whether this is a top-level section.</param>
        /// <param name="hasPageBreak">Whether the section has a page break.</param>
        public void InsertSection(string name, string parentName, int fieldColumns, bool isTopLevel, bool hasPageBreak)
        {
            DataSource.ExecuteNonQuery("PH_Section_sp_Insert", name, parentName, fieldColumns, isTopLevel, hasPageBreak);
        }

        #endregion Inserts...

        #region Updates...

        /// <summary>
        /// Updates an existing PH field.
        /// Executes stored procedure: PH_Field_sp_Update
        /// </summary>
        /// <param name="field">The PHField object with updated values.</param>
        public void UpdateField(PHField field)
        {
            if (field == null)
            {
                return;
            }

            DataSource.ExecuteNonQuery("PH_Field_sp_Update", field.Id, field.Name);
        }

        /// <summary>
        /// Updates an existing PH field type.
        /// Executes stored procedure: PH_FieldType_sp_Update
        /// </summary>
        /// <param name="fieldType">The PHFieldType object with updated values.</param>
        public void UpdateFieldType(PHFieldType fieldType)
        {
            if (fieldType == null)
            {
                return;
            }

            string colorName = null;

            if (fieldType.Color.HasValue)
            {
                colorName = fieldType.Color.Value.Name;
            }

            DataSource.ExecuteNonQuery("PH_FieldType_sp_Update", fieldType.Id, fieldType.Name, fieldType.DataTypeId, fieldType.Datasource, fieldType.Placeholder, colorName, fieldType.Length);
        }

        /// <summary>
        /// Updates an existing PH form field mapping.
        /// Executes stored procedure: PH_FormField_sp_Update
        /// </summary>
        /// <param name="e">The PHFormFieldUpdateEventArgs containing old and new values.</param>
        public void UpdateFormField(PHFormFieldUpdateEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            DataSource.ExecuteNonQuery("PH_FormField_sp_Update", e.OldSectionId, e.OldFieldId, e.OldFieldTypeId, e.NewSectionId, e.NewFieldId, e.NewFieldTypeId, e.OldFieldDisplayOrder, e.OldFieldTypeDisplayOrder, e.NewFieldDisplayOrder, e.NewFieldTypeDisplayOrder, e.OldToolTip, e.NewToolTip);
        }

        /// <summary>
        /// Updates an existing PH form value.
        /// Executes stored procedure: PH_FormValue_sp_Update
        /// </summary>
        /// <param name="value">The PHFormValue object with updated values.</param>
        /// <returns>True if the value was successfully updated; otherwise, false.</returns>
        public bool UpdateFormValue(PHFormValue value)
        {
            Object result = DataSource.ExecuteScalar("PH_FormValue_sp_Update", value.RefId, value.SectionId, value.FieldId, value.FieldTypeId, value.RawValue);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Updates an existing PH section.
        /// Executes stored procedure: PH_Section_sp_Update
        /// </summary>
        /// <param name="section">The PHSection object with updated values.</param>
        public void UpdateSection(PHSection section)
        {
            if (section == null)
            {
                return;
            }

            DataSource.ExecuteNonQuery("PH_Section_sp_Update", section.Id, section.Name, section.ParentId, section.FieldColumns, section.IsTopLevel, section.DisplayOrder, section.HasPageBreak);
        }

        #endregion Updates...

        #region Removes...

        /// <summary>
        /// Removes a PH form field mapping.
        /// Executes stored procedure: PH_FormField_sp_Delete
        /// </summary>
        /// <param name="formField">The PHFormField object to remove.</param>
        public void RemoveFormField(PHFormField formField)
        {
            if (formField == null || !formField.IsValid())
            {
                return;
            }

            DataSource.ExecuteNonQuery("PH_FormField_sp_Delete", formField.Section.Id, formField.Field.Id, formField.FieldType.Id);
        }

        /// <summary>
        /// Removes a PH form value.
        /// Executes stored procedure: PH_FormValue_sp_Delete
        /// </summary>
        /// <param name="value">The PHFormValue object to remove.</param>
        /// <returns>True if the value was successfully removed; otherwise, false.</returns>
        public bool RemoveFormValue(PHFormValue value)
        {
            if (value == null || !value.IsValid())
            {
                return false;
            }

            Object result = DataSource.ExecuteScalar("PH_FormValue_sp_Delete", value.RefId, value.SectionId, value.FieldId, value.FieldTypeId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Removes a child section from a parent section.
        /// Executes stored procedure: PH_Section_sp_RemoveChild
        /// </summary>
        /// <param name="parentId">The ID of the parent section.</param>
        /// <param name="childId">The ID of the child section to remove.</param>
        public void RemoveSectionChild(int parentId, int childId)
        {
            DataSource.ExecuteNonQuery("PH_Section_sp_RemoveChild", parentId, childId);
        }

        #endregion Removes...

        #region Application Warmup Operations...

        /// <summary>
        /// Executes the PH data collection process for a previous month (application warmup operation).
        /// Executes stored procedure: PH_Workflow_sp_ExecuteCollectionProcess
        /// </summary>
        /// <param name="previousMonth">The date representing the previous month to process.</param>
        /// <returns>A list of case IDs that were processed.</returns>
        public IList<int> ExecuteCollectionProcess(DateTime previousMonth)
        {
            IList<int> results = new List<int>();

            if (previousMonth == null)
            {
                return results;
            }

            DataSet dSet = DataSource.ExecuteDataSet("PH_Workflow_sp_ExecuteCollectionProcess", previousMonth.Year, previousMonth.Month, GetServerIPAddress());

            if (dSet == null)
                return results;

            if (dSet.Tables.Count == 0)
                return results;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return results;

            foreach (DataRow row in dTable.Rows)
            {
                results.Add(DataHelpers.GetIntFromDataRow("Id", row));
            }

            return results;
        }

        /// <summary>
        /// Retrieves email addresses for PH push report recipients.
        /// Executes stored procedure: PH_Workflow_sp_GetPushReportEmails
        /// </summary>
        /// <param name="executionDate">The execution date for the report.</param>
        /// <param name="userGroupId">The user group ID.</param>
        /// <returns>A collection of email addresses.</returns>
        public StringCollection GetPushReportEmailList(DateTime executionDate, int userGroupId)
        {
            StringCollection collection = new StringCollection();

            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            DataSource.ExecuteReader(rowReader, "PH_Workflow_sp_GetPushReportEmails", executionDate.Year, executionDate.Month, userGroupId);

            return collection;
        }

        /// <summary>
        /// Retrieves a list of units for PH push reports.
        /// Executes stored procedure: PH_Workflow_sp_GetPushReportUnits
        /// </summary>
        /// <param name="executionDate">The execution date for the report.</param>
        /// <returns>A collection of unit identifiers.</returns>
        public StringCollection GetPushReportUnitsList(DateTime executionDate)
        {
            StringCollection collection = new StringCollection();

            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            DataSource.ExecuteReader(rowReader, "PH_Workflow_sp_GetPushReportUnits", executionDate.Year, executionDate.Month);

            return collection;
        }

        /// <summary>
        /// Retrieves email addresses for seven-day warning notifications.
        /// Executes stored procedure: PH_Workflow_sp_GetSevenDayWarningEmails
        /// </summary>
        /// <param name="executionDate">The execution date for the warning.</param>
        /// <returns>A collection of email addresses.</returns>
        public StringCollection GetSevenDayWarningEmailList(DateTime executionDate)
        {
            StringCollection collection = new StringCollection();

            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            DataSource.ExecuteReader(rowReader, "PH_Workflow_sp_GetSevenDayWarningEmails", executionDate.Year, executionDate.Month);

            return collection;
        }

        protected string GetServerIPAddress()
        {
            return System.Web.HttpContext.Current.Request.ServerVariables["LOCA‌​L_ADDR"];
        }

        #endregion Application Warmup Operations...

        #region PH Canned Report Operations...

        /// <summary>
        /// Executes the PH totals canned report.
        /// Executes stored procedure: report_sp_PHTotalsReport
        /// </summary>
        /// <param name="args">The report arguments containing filter criteria.</param>
        /// <returns>A list of PHFormFieldTotal objects, or an empty list if arguments are invalid.</returns>
        public IList<PHFormFieldTotal> ExecuteTotalsCannedReport(PHTotalsReportArgs args)
        {
            if (!args.IsValid())
                return new List<PHFormFieldTotal>();

            DataSet dSet = DataSource.ExecuteDataSet("report_sp_PHTotalsReport", args.UnitId, args.IncludeSubUnits, args.Collocated, args.ViewType, args.BeginReportingPeriod, args.EndReportingPeriod);

            return DataHelpers.ExtractObjectsFromDataSet<PHFormFieldTotal>(dSet);
        }

        /// <summary>
        /// Executes the PH totals comments canned report.
        /// Executes stored procedure: report_sp_PHTotalsCommentsReport
        /// </summary>
        /// <param name="args">The report arguments containing filter criteria.</param>
        /// <returns>A DataSet containing report comments, or null if arguments are invalid.</returns>
        public DataSet ExecuteTotalsCommentsCannedReport(PHTotalsReportArgs args)
        {
            if (!args.IsValid())
                return null;

            return DataSource.ExecuteDataSet("report_sp_PHTotalsCommentsReport", args.UnitId, args.IncludeSubUnits, args.Collocated, args.ViewType, args.BeginReportingPeriod, args.EndReportingPeriod);
        }

        /// <summary>
        /// Executes the PH totals string values canned report.
        /// Executes stored procedure: report_sp_PHTotalsStringValuesReport
        /// </summary>
        /// <param name="args">The report arguments containing filter criteria.</param>
        /// <returns>A list of PHTotalsReportStringValue objects, or an empty list if arguments are invalid.</returns>
        public IList<PHTotalsReportStringValue> ExecuteTotalsStringValuesCannedReport(PHTotalsReportArgs args)
        {
            if (!args.IsValid())
                return new List<PHTotalsReportStringValue>();

            DataSet dSet = DataSource.ExecuteDataSet("report_sp_PHTotalsStringValuesReport", args.UnitId, args.IncludeSubUnits, args.Collocated, args.ViewType, args.BeginReportingPeriod, args.EndReportingPeriod);

            return DataHelpers.ExtractObjectsFromDataSet<PHTotalsReportStringValue>(dSet);
        }

        /// <summary>
        /// Executes the PH totals suicide methods report.
        /// Executes stored procedure: report_sp_PHTotalsSuicideMethodsReport
        /// </summary>
        /// <param name="args">The report arguments containing filter criteria.</param>
        /// <returns>A list of PHFormValue objects, or an empty list if arguments are invalid.</returns>
        public IList<PHFormValue> ExecuteTotalsSuicideMethodsReport(PHTotalsReportArgs args)
        {
            if (!args.IsValid())
                return new List<PHFormValue>();

            DataSet dSet = DataSource.ExecuteDataSet("report_sp_PHTotalsSuicideMethodsReport", args.UnitId, args.IncludeSubUnits, args.Collocated, args.ViewType, args.BeginReportingPeriod, args.EndReportingPeriod);

            return DataHelpers.ExtractObjectsFromDataSet<PHFormValue>(dSet);
        }

        /// <summary>
        /// Retrieves the earliest reporting period year from PH cases.
        /// Executes stored procedure: PH_Report_sp_GetSmallestReportingPeriodYear
        /// </summary>
        /// <returns>The earliest reporting period year, or the current year if not found.</returns>
        public int GetSmallestReportingPeriodYear()
        {
            Object result = DataSource.ExecuteScalar("PH_Report_sp_GetSmallestReportingPeriodYear");

            if (result == null)
                return DateTime.Now.Year;

            return (int)result;
        }

        #endregion PH Canned Report Operations...
    }
}