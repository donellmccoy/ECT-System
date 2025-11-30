using System;
using System.Data;
using System.Text;
using System.Web;
using ALOD.Core.Interfaces;
using ALOD.Data.Services;
using ALODWebUtility.Printing.FormFieldParsers;
using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace ALODWebUtility.Printing
{
    public class PDFForm : PrintDocument, IDisposable
    {
        #region Members

        public const string DATATYPE_BOOL = "bool";
        public const string DATATYPE_CHOICE = "choice";
        public const string DATATYPE_STRING = "string";
        private Doc _doc = null;
        private IPDFFormFieldParser _formFieldParser;
        private string _formFieldParserName = string.Empty;
        private bool _open = false;

        #endregion

        #region Properties

        public byte[] Buffer
        {
            get { return _doc.GetData(); }
        }

        public Doc Document
        {
            get { return _doc; }
        }

        public Fields Fields
        {
            get { return _doc.Form.Fields; }
        }

        public IPDFFormFieldParser FormFieldParser
        {
            get { return _formFieldParser; }
            private set { _formFieldParser = value; }
        }

        public string FormFieldParserName
        {
            get { return _formFieldParserName; }
            private set { _formFieldParserName = value; }
        }

        public bool IsOpen
        {
            get { return _open; }
        }

        public void FrameRect()
        {
            _doc.FrameRect();
        }

        #endregion

        #region Constructors

        public PDFForm(string fileName) : base()
        {
            InitializeDocument(fileName);
        }

        public PDFForm(int docId) : base(docId)
        {
            InitializeDocument(_filename);
        }

        #endregion

        #region Methods

        public void Close()
        {
            if (_open)
            {
                _open = false;
            }
        }

        /// <summary>
        /// Retrieves data elements from the data store and populates the form fields
        /// </summary>
        /// <param name="keyId"></param>
        /// <remarks></remarks>
        public void Populate(int keyId)
        {
            if (_doc == null) throw new ApplicationException("Document not initialized");

            DataSet ds;
            try
            {
                ds = GetDocumentData(keyId);
                foreach (DataTable dsTable in ds.Tables)
                {
                    SingleItemLayout(dsTable);
                }
            }
            catch (DataException dataEx)
            {
                throw dataEx;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Problems assembling the document. " + ex.Message);
            }
        }

        public void SetBoolField(string fieldName, string value)
        {
            bool checkedVal = false;

            switch (value.ToLower())
            {
                case "yes":
                case "y":
                case "true":
                case "1":
                    checkedVal = true;
                    break;
            }
            if (checkedVal)
            {
                Fields[fieldName].Value = Fields[fieldName].Options[0];
            }
            else
            {
                Fields[fieldName].Value = "Off";
            }
        }

        /// <summary>
        /// Sets a range of related checkboxes.  Only one will be checked
        /// </summary>
        /// <param name="fieldName">the shared prefix for the check boxes</param>
        /// <param name="value">The value that should be selected</param>
        /// <remarks></remarks>
        public void SetChoiceField(string fieldName, string value)
        {
            string key = fieldName + value;

            foreach (Field item in Fields)
            {
                if (item.Name.Substring(0, Math.Min(item.Name.Length - 1, fieldName.Length)) == fieldName)
                {
                    if (item.Name == key)
                    {
                        Fields[key].Value = Fields[key].Options[0];
                    }
                    else
                    {
                    }
                }
            }
        }

        public void SetField(string fieldName, string value)
        {
            AssignDocumentValue(fieldName, value, DATATYPE_STRING);
        }

        public void SetField(string fieldname, string value, string dataType)
        {
            AssignDocumentValue(fieldname, value, dataType);
        }

        public void Stamp()
        {
            _doc.Form.Stamp();
        }

        /// <summary>
        /// Summary for SuppressThirdPage().
        /// With the method RemapPages("2"), we are only printing the first two pages of the document (form348).
        /// </summary>
        /// <remarks>Print the first document only.</remarks>
        public void SuppressInstructionPages()
        {
            _doc.RemapPages("1,2,3");
        }

        /// <summary>
        /// Summary for SuppressSecondPage().
        /// With the method RemapPages("1"), we are only printing the first page of the document (form348).
        /// </summary>
        /// <remarks>Print the first document only.</remarks>
        public void SuppressSecondPage()
        {
            _doc.RemapPages("1");
        }

        protected void InitializeDocument(string fileName)
        {
            _filename = fileName;
            string docPath = HttpContext.Current.Server.MapPath("~/Secure/Documents/" + _filename);
            _doc = new Doc();
            _doc.Read(docPath);
            _open = true;
            FormFieldParserName = LookupService.GetFormFieldParserById(_formFieldParserId);
            FormFieldParser = FormFieldParserFactory.GetParserByName(FormFieldParserName);
        }

        // Used to support the operations of the AssembleDocument method.
        // Will evaluate the field types from the mappings and set the
        // values to the designated data types.
        // Currently supports: String, Datetime, and Number.
        private void AssignDocumentValue(string fieldName, object value, string valueType)
        {
            if (fieldName == null || value == null)
            {
                return;
            }

            try
            {
                Field field = FormFieldParser.ParseField(_doc, fieldName);

                if (field == null)
                {
                    return;
                }

                // As of the writing of this comment (9/13/2016) the only case ever used by ECT is "string"; therefore the SetChoiceField() and SetBoolField() have not been updated to properly handle the newer versions of PDF files created in Adobe LiveCycle...
                switch (valueType.ToLower().Trim())
                {
                    case "string":
                        field.Value = Convert.ToString(value).Trim();
                        break;
                    case "datetime":
                        field.Value = Convert.ToDateTime(value).ToString(); // Assuming default string conversion for DateTime
                        break;
                    case "number":
                        field.Value = Convert.ToInt32(value).ToString();
                        break;
                    case "choice":
                        SetChoiceField(fieldName.Trim(), Convert.ToString(value).Trim());
                        break;
                    case "bool":
                        SetBoolField(fieldName.Trim(), Convert.ToString(value).Trim());
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting field " + fieldName);
            }
        }

        // Used to assemble multiple data items into a single item assignment in a Document.
        // I.E.: Single document item 'txt_Symptoms' is assigned a grouping of multiple data
        // values returned.
        private void GroupedItemsLayout(DataTable dt)
        {
            StringBuilder groupedText = new StringBuilder();
            string currentFieldName = string.Empty;
            string fieldType = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                currentFieldName = Convert.ToString(row[1]).Trim();
                fieldType = Convert.ToString(row[3]).Trim();
                groupedText.Append(Convert.ToString(row[2]).Trim());
                groupedText.Append(_sbDelimiter);
            }
            if (groupedText.Length > 3)
            {
                groupedText.Replace(_sbDelimiter, string.Empty, groupedText.Length - 3, 3);
            }
            AssignDocumentValue(currentFieldName, groupedText.ToString(), fieldType);
        }

        // Used to assemble multiple data items into multiple item assignments constructed
        // with the use of an appended increment.
        // Example: Document items 'txt_Symptom_1', 'txt_Symptom_2' will be assigned data items
        // via an incremented Field assignment. This requires that the field names are defined
        // without the increment value included ('txt_Symptom_') to allow concatenation with the
        // increment.
        private void IncrementedItemsLayout(DataTable dt)
        {
            int incrementValue = 0;
            foreach (DataRow row in dt.Rows)
            {
                incrementValue += 1;
                AssignDocumentValue(Convert.ToString(row[1]).Trim() + incrementValue.ToString(), row[2], Convert.ToString(row[3]).Trim());
            }
        }

        // Used to assemble single item assignments in a Document.
        // I.E.: Single document item 'txt_SoldierName' is assigned single data item row("SoldierName").
        private void SingleItemLayout(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                AssignDocumentValue(Convert.ToString(row[1]).Trim(), row[2], Convert.ToString(row[3]).Trim());
            }
        }

        #endregion

        #region IDisposable

        private bool disposedValue = false; // To detect redundant calls

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (_doc != null)
                    {
                        _doc.Clear();
                    }
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }

        #endregion
    }
}
