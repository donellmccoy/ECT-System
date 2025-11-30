using System;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Printing
{
    public class PrintStore
    {
        protected SqlDataStore _adapter;
        private DTPrinting.LetterContentsDataTable _data;
        private DTPrinting.LetterTemplateDataTable _letterTemplates;
        DTPrinting.MemoListDataTable _memoList;
        private DTPrinting.PrintDocumentDetailsDataTable _printingDocumentsTable;

        protected SqlDataStore DataStore
        {
            get
            {
                if (_adapter == null)
                {
                    _adapter = new SqlDataStore();
                }
                return _adapter;
            }
        }

        public DTPrinting.MemoListDataTable GetMemoList(int pk2173, int secondaryId, string workFlowCode)
        {
            _memoList = new DTPrinting.MemoListDataTable();
            DataStore.ExecuteReader(MemoListReader, "core_memo_sp_GetListByRefId", pk2173);
            return _memoList;
        }

        public DTPrinting.LetterContentsDataTable GetTemplateData(string dataProc, params object[] parameters)
        {
            _data = new DTPrinting.LetterContentsDataTable();
            DataStore.ExecuteReader(TemplateDataReader, dataProc, parameters);
            return _data;
        }

        private void MemoListReader(SqlDataStore adapter, IDataReader reader)
        {
            DTPrinting.MemoListRow row = _memoList.NewMemoListRow();

            // 0-LetterID, 1-TemplateID, 2-OriginationDate, 3-Description, 4-AccessLevelID
            // 5-ViewRestrictedByAccessLevel, 6-UserRole, 7-AccessLevel

            row.LetterId = DataStore.GetInt32(reader, 0);
            row.TemplateId = DataStore.GetByte(reader, 1);
            row.CreatedDate = DataStore.GetDateTime(reader, 2, DateTime.Now);
            row.Description = DataStore.GetString(reader, 3);

            _memoList.Rows.Add(row);
        }

        #region Get Document Dataset

        public DataSet DocumentGetData(string procName, int docId, int keyId)
        {
            DataSet ds = null;
            try
            {
                ds = DataStore.ExecuteDataSet(procName, docId, keyId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        #endregion

        #region Print Document Details

        public DTPrinting.PrintDocumentDetailsDataTable DocumentGetDetails(int documentId)
        {
            _printingDocumentsTable = new DTPrinting.PrintDocumentDetailsDataTable();
            DataStore.ExecuteReader(HandleDocumentDetailsRowRead, "print_sp_GetDocumentDetails", documentId);
            return _printingDocumentsTable;
        }

        private void HandleDocumentDetailsRowRead(SqlDataStore adapter, IDataReader reader)
        {
            DTPrinting.PrintDocumentDetailsRow row = _printingDocumentsTable.NewPrintDocumentDetailsRow();

            row.docid = DataStore.GetInt16(reader, 0);
            row.doc_name = DataStore.GetString(reader, 1);
            row.filename = DataStore.GetString(reader, 2);
            row.filetype = DataStore.GetString(reader, 3);
            row.compo = DataStore.GetInt16(reader, 4);
            row.sp_getdata = DataStore.GetString(reader, 5);
            row.FormFieldParserId = DataStore.GetInt32(reader, 6);

            _printingDocumentsTable.Rows.Add(row);
        }

        #endregion

        #region Templates

        public DTPrinting.LetterTemplateDataTable GetLetterTemplate(int refId, short templateId, byte userGroup)
        {
            _letterTemplates = new DTPrinting.LetterTemplateDataTable();
            DataStore.ExecuteReader(LetterTemplateReader, "core_memo_sp_GetTemplateById", refId, templateId, userGroup);
            return _letterTemplates;
        }

        private void LetterTemplateReader(SqlDataStore adapter, IDataReader reader)
        {
            DTPrinting.LetterTemplateRow row = _letterTemplates.NewLetterTemplateRow();

            // 0-Description, 1-Content, 2-MailingAddress, 3-addSignature
            // 4-dataSource, 5-officeSymbol, 6-addDate, 7-addSuspenseDate, 8-ApprovalRequired
            // 9-sigblock

            row.Description = DataStore.GetString(reader, 0);
            row.Content = DataStore.GetString(reader, 1);
            row.MailingAddress = DataStore.GetString(reader, 2);
            row.addSignature = DataStore.GetBoolean(reader, 3, false);
            row.dataProc = DataStore.GetString(reader, 4);
            row.officeSymbol = DataStore.GetString(reader, 5);
            row.addDate = DataStore.GetBoolean(reader, 6, true);
            row.addSuspenseDate = DataStore.GetBoolean(reader, 7, false);
            row.ApprovalRequired = DataStore.GetBoolean(reader, 8, false);
            row.sigBlock = DataStore.GetString(reader, 9);

            _letterTemplates.Rows.Add(row);
        }

        #endregion

        private void TemplateDataReader(SqlDataStore adapter, IDataReader reader)
        {
            DTPrinting.LetterContentsRow row = _data.NewLetterContentsRow();

            row.Key = DataStore.GetString(reader, 0);
            row.Value = DataStore.GetString(reader, 1);

            _data.Rows.Add(row);
        }
    }
}
