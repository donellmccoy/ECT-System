using System;
using System.Data;

namespace ALODWebUtility.Printing
{
    public class PrintDocument
    {
        protected int _compo;
        protected int _docid;
        protected string _docname;
        protected string _filename;
        protected string _filetype;
        protected int _formFieldParserId;
        protected string _sbDelimiter = " ~ ";
        protected string _sp_name;

        #region Constructors

        public PrintDocument()
        {
            _docid = 0;
            _docname = string.Empty;
        }

        public PrintDocument(int docId)
        {
            _docid = docId;
            LoadDocumentData();
        }

        #endregion

        #region Properties

        public int Compo
        {
            get { return _compo; }
            set { _compo = value; }
        }

        public int DocID
        {
            get { return _docid; }
            set { _docid = value; }
        }

        public string DocName
        {
            get { return _docname; }
            set { _docname = value; }
        }

        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string FileType
        {
            get { return _filetype; }
            set { _filetype = value; }
        }

        public string SpName
        {
            get { return _sp_name; }
            set { _sp_name = value; }
        }

        #endregion

        #region Database/Loading

        public void LoadDocumentData()
        {
            try
            {
                DTPrinting.PrintDocumentDetailsDataTable documentDetailsTable;
                PrintStore documentDetailsStore = new PrintStore();
                documentDetailsTable = documentDetailsStore.DocumentGetDetails(_docid);
                DTPrinting.PrintDocumentDetailsRow row = (DTPrinting.PrintDocumentDetailsRow)documentDetailsTable.Rows[0];
                _docname = row.doc_name;
                _filename = row.filename;
                _filetype = row.filetype;
                _compo = row.compo;
                _sp_name = row.sp_getdata;
                _formFieldParserId = row.FormFieldParserId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Methods

        // Used to retrieve the Document specific data for assembly.
        protected DataSet GetDocumentData(int keyId)
        {
            PrintStore printStore = new PrintStore();
            try
            {
                return printStore.DocumentGetData(_sp_name, _docid, keyId);
            }
            catch (Exception ex)
            {
                throw new DataException("Unable to acquire data.");
            }
        }

        #endregion
    }
}
