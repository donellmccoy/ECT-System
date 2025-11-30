using ALOD.Core.Domain.PsychologicalHealth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IPsychologicalHealthDao
    {
        void AddSectionChild(int parentId, int childId, int displayOrder);

        IList<int> ExecuteCollectionProcess(DateTime previousMonth);

        IList<PHFormFieldTotal> ExecuteTotalsCannedReport(PHTotalsReportArgs args);

        DataSet ExecuteTotalsCommentsCannedReport(PHTotalsReportArgs args);

        IList<PHTotalsReportStringValue> ExecuteTotalsStringValuesCannedReport(PHTotalsReportArgs args);

        IList<PHFormValue> ExecuteTotalsSuicideMethodsReport(PHTotalsReportArgs args);

        IList<PHField> GetAllFields();

        IList<PHFieldType> GetAllFieldTypes();

        IList<PHFormField> GetAllFormFields();

        IList<PHSection> GetAllSections();

        int GetCaseIdByReportingPeriod(DateTime reportingPeriod, int caseUnitId);

        DataSet GetCasesByCaseUnit(int caseUnitId);

        DataSet GetDataSetFromProcedure(string storedProcedureName);

        PHField GetFieldById(int id);

        PHFieldType GetFieldTypeById(int id);

        PHFormField GetFormFieldByIds(int sectionId, int fieldId, int fieldTypeId);

        IList<PHFormField> GetFormFieldFieldTypes(int sectionId, int fieldId);

        IList<PHFormValue> GetFormValuesByRefId(int refId);

        DataSet GetNumberedAirForcesForPH();

        StringCollection GetPushReportEmailList(DateTime executionDate, int userGroupId);

        StringCollection GetPushReportUnitsList(DateTime executionDate);

        PHSection GetSectionById(int id);

        IList<PHSection> GetSectionChildren(int parentId);

        StringCollection GetSevenDayWarningEmailList(DateTime executionDate);

        int GetSmallestReportingPeriodYear();

        void InsertField(string name);

        void InsertFieldType(PHFieldType newFieldType);

        void InsertFormField(int sectionId, int fieldId, int fieldTypeId, int fieldDisplayOrder, int fieldTypeDisplayOrder, string toolTip);

        bool InsertFormValue(int refId, int sectionId, int fieldId, int fieldTypeId, string value);

        void InsertSection(string name, string parentName, int fieldColumns, bool isTopLevel, bool hasPageBreak);

        DataSet PHCaseSearch(string caseId, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, int reportingMonth, int reportingYear);

        void RemoveFormField(PHFormField formField);

        bool RemoveFormValue(PHFormValue value);

        void RemoveSectionChild(int parentId, int childId);

        void UpdateField(PHField field);

        void UpdateFieldType(PHFieldType fieldType);

        void UpdateFormField(PHFormFieldUpdateEventArgs e);

        bool UpdateFormValue(PHFormValue value);

        void UpdateSection(PHSection section);
    }
}