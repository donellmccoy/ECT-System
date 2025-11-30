using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Printing;

namespace ALODWebUtility.Memo
{
    public class Memo
    {
        #region Members/Properties

        protected SqlDataStore _adapter;
        protected bool _addDate = true;
        protected bool _addSignature = true;
        protected bool _addSuspenseDate = false;
        protected bool _approvalRequired = false;
        protected byte _authorGroup;
        protected int _authorId;
        protected string _content = string.Empty;
        protected DateTime _createdDate;
        protected short _currentPage = 0;
        protected int _dataKey = 0;
        protected string _dataProc = string.Empty;
        protected bool _deleted = false;
        protected string _description = string.Empty;
        protected bool _isTemplate = false;
        protected DateTime _lastEdit;
        protected short _letterHead = 0;
        protected int _letterId = 0;
        protected string _mailingAddress = string.Empty;
        protected string _memoDate = string.Empty;
        protected string _officeSymbol = string.Empty;
        protected short _pageOffset = 0;
        protected bool _phi = false;
        protected int _refId = 0;
        protected byte _scope;
        protected int _secondaryId = 0;
        protected string _sigBlock = string.Empty;
        protected string _state = string.Empty;
        protected string _suspenseDate = string.Empty;
        protected StringDictionary _templateData;
        protected string _templateFileName = "Letterhead-1.pdf";
        protected int _templateId = 0;
        protected bool _viewRestricted = false;
        private const string DATE_FORMAT = "d MMMM yyyy";
        private const int LINE_COUNT = 34;
        private const float LINE_WIDTH = 809.55f;
        private AppUser _user = null;

        private string SigBlockOffset = new string(' ', 71);

        public byte AccessScope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public bool AddDate
        {
            get { return _addDate; }
        }

        public bool AddSignature
        {
            get { return _addSignature; }
        }

        public bool AddSuspenseDate
        {
            get { return _addSuspenseDate; }
        }

        public bool ApprovalRequired
        {
            get { return _approvalRequired; }
            set { _approvalRequired = value; }
        }

        public byte AuthorGroup
        {
            get { return _authorGroup; }
            set { _authorGroup = value; }
        }

        public int AuthorId
        {
            get
            {
                if (_authorId == 0)
                {
                    return CurrentUser.Id;
                }
                return _authorId;
            }
            set { _authorId = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public int ContentLength
        {
            get { return FormattedContent.Length; }
        }

        public short CurrentPage
        {
            get { return _currentPage; }
        }

        public int DataKey
        {
            get
            {
                if (_dataKey == 0)
                {
                    return _refId;
                }
                return _dataKey;
            }
            set { _dataKey = value; }
        }

        public DateTime DateCreated
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string FormatedSignatureBlock
        {
            get
            {
                // if this is not from a template, load whatever we have
                if (!_isTemplate)
                {
                    return _sigBlock;
                }

                // otherwise, format the sigblock
                string signature = _sigBlock;

                if (!_approvalRequired)
                {
                    // Update the signature block to show that a letter has been signed
                    if (signature != "" &&
                     signature.IndexOf("<< Digitally Signed by ") == -1 &&
                     signature.IndexOf("<< Signed by ") == -1)
                    {
                        // Add the signature block to the end of the letter
                        signature = "<< Signed by " + CurrentUser.FullName + " >>" + Environment.NewLine + signature;
                    }
                }

                return signature;
            }
        }

        public string FormattedContent
        {
            get
            {
                return (Content + RepeatStr(Environment.NewLine, 5) + SigBlockOffset + SignatureBlock.Replace(Environment.NewLine, Environment.NewLine + SigBlockOffset)).TrimEnd();
            }
        }

        public bool IsSigned
        {
            get
            {
                if (_sigBlock.Length > 0)
                {
                    if (_sigBlock.IndexOf("<< Digitally Signed by ") != -1 || _sigBlock.IndexOf("<< Signed by ") != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsTemplate
        {
            get { return _isTemplate; }
        }

        public DateTime LastEditDate
        {
            get { return _lastEdit; }
            set { _lastEdit = value; }
        }

        public int LetterId
        {
            get { return _letterId; }
            set { _letterId = value; }
        }

        public string MailingAddress
        {
            get { return _mailingAddress; }
            set { _mailingAddress = value; }
        }

        public string MemoDate
        {
            get
            {
                // if this is a letter always return what we have
                if (_letterId != 0)
                {
                    return _memoDate;
                }

                // otherwise, this is a template
                if (_addDate)
                {
                    return DateTime.Now.ToString(DATE_FORMAT);
                }

                // otherwise, return nothing
                return "";
            }

            set { _memoDate = value; }
        }

        public string OfficeSymbol
        {
            get { return _officeSymbol; }
            set { _officeSymbol = value; }
        }

        public bool Phi
        {
            get { return _phi; }
            set { _phi = value; }
        }

        public int RefId
        {
            get { return _refId; }
            set { _refId = value; }
        }

        public int SecondaryId
        {
            get { return _secondaryId; }
            set { _secondaryId = value; }
        }

        public string SignatureBlock
        {
            get { return _sigBlock; }
            set { _sigBlock = value; }
        }

        public string State
        {
            get { return _state; }
        }

        public string SuspenseDate
        {
            get
            {
                // if this is a letter always return what we have
                if (_letterId != 0)
                {
                    return _suspenseDate;
                }

                // otherwise, this is a template
                if (_addSuspenseDate)
                {
                    return DateTime.Now.AddDays(30).ToString(DATE_FORMAT);
                }

                // otherwise, return nothing
                return "";
            }

            set { _suspenseDate = value; }
        }

        public string TemplateFileName
        {
            get { return _templateFileName; }
            set { _templateFileName = value; }
        }

        public int TemplateId
        {
            get { return _templateId; }
            set { _templateId = value; }
        }

        public bool ViewRestricted
        {
            get { return _viewRestricted; }
            set { _viewRestricted = value; }
        }

        // use the default in case one is not found
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

        private AppUser CurrentUser
        {
            get
            {
                if (_user == null)
                {
                    _user = UserService.CurrentUser();
                }
                return _user;
            }
        }

        #endregion

        public string PageContent
        {
            get
            {
                string part = "";

                Font font = new Font("Arial", 12);
                Bitmap bmp = new Bitmap(100, 100);
                Graphics gfx;
                gfx = Graphics.FromImage(bmp);

                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.LineLimit;

                SizeF rect = new SizeF(LINE_WIDTH, font.GetHeight(gfx) * LINE_COUNT);
                int chars = 0;
                int lines = 0;

                // start with what we have left
                int lengthLeft = ContentLength - _pageOffset;
                part = FormattedContent.Substring(_pageOffset, lengthLeft);

                // now determine how much of what we have left will fit on one page
                SizeF actual = gfx.MeasureString(part, font, rect, format, out chars, out lines);
                part = FormattedContent.Substring(_pageOffset, chars);
                _pageOffset += (short)chars;

                if (lengthLeft > chars)
                {
                    // we now know how many chars will fit, but that is probably mid-word, so find the end
                    // of the preceding sentence and break there.
                    int index = part.LastIndexOfAny(new char[] { '.', '?', '!' });

                    if (index != -1)
                    {
                        _pageOffset = (short)(_pageOffset - (part.Length - index - 1));
                        part = part.Substring(0, index + 1);
                    }
                }

                return part;
            }
        }

        public void LoadCoverLetter(int key, ModuleType type)
        {
            _refId = key;
        }

        public void LoadCurrentLetterByTemplate(int refId, int templateId)
        {
            _refId = refId;
            _templateId = templateId;
            _isTemplate = false;

            DbCommand cmd;
            cmd = DataStore.GetStoredProcCommand("core_memo_sp_GetCurrentMemoByTemplateId");
            DataStore.AddInParameter(cmd, "@refId", DbType.Int32, _refId);
            DataStore.AddInParameter(cmd, "@template", DbType.Byte, _templateId);
            DataStore.AddOutParameter(cmd, "@letterId", DbType.Int32, 32);

            DataStore.ExecuteReader(MemoReader, cmd);

            if (!Convert.IsDBNull(cmd.Parameters["@letterId"].Value))
            {
                _letterId = Convert.ToInt32(cmd.Parameters["@letterId"].Value);
            }
        }

        public void LoadLetter(int letterId)
        {
            _letterId = letterId;
            DataStore.ExecuteReader(MemoReader, "core_memo_sp_GetMemoById", _letterId);
        }

        public void LoadTemplate(int refId, int templateId, int dataKey)
        {
            _refId = refId;
            _templateId = templateId;
            _isTemplate = true;
            _dataKey = dataKey;

            DataStore.ExecuteReader(MemoReader, "core_memo_sp_GetTemplateById", refId, templateId, CurrentUser.CurrentRole.Group.Id);

            PopulateData();
        }

        public bool NextPage()
        {
            if (_pageOffset < ContentLength)
            {
                _currentPage += 1;
                return true;
            }

            return false;
        }

        public bool Save()
        {
            _letterId = Convert.ToInt32(DataStore.ExecuteScalar("core_memo_sp_InsertMemo",
                _refId, _templateId, _letterId, CurrentUser.Id, _mailingAddress, _officeSymbol,
                _memoDate, _suspenseDate, _content, FormatedSignatureBlock));

            return _letterId != 0;
        }

        public void SetField(string key, string value)
        {
            _content = _content.Replace("{" + key.ToUpper() + "}", value);
        }

        public PDFDocument ToPdf(bool isPreview = false)
        {
            PDFForm form;
            List<PDFForm> pages = new List<PDFForm>();
            PDFDocument doc = new PDFDocument();
            string ctField = "Content";

            if (isPreview)
            {
                doc.WaterMark = "PREVIEW DOCUMENT";
            }

            if (_phi)
            {
                doc.WaterMark = "Protected Health Information";
            }

            while (this.NextPage())
            {
                form = new PDFForm(TemplateFileName);

                if (_currentPage == 1)
                {
                    // add our header info to the first page
                    form.SetField("Address", MailingAddress);

                    if (OfficeSymbol.Length > 0)
                    {
                        form.SetField("OfficeSymbol", OfficeSymbol);
                        form.SetField("SuspenseDate", SuspenseDate.Length > 0 ? "S: " + SuspenseDate : SuspenseDate);
                        form.SetField("Date", MemoDate);
                        ctField = "Content";
                    }
                    else
                    {
                        ctField = "Content_full";
                    }
                }
                else
                {
                    // user the higher field for subsequent pages
                    ctField = "Content_full";
                }

                string ct = PageContent;
                form.SetField(ctField, ct);
                form.Stamp();

                pages.Add(form);
            }

            foreach (PDFForm page in pages)
            {
                doc.AddForm(page);
            }

            return doc;
        }

        protected string RepeatStr(string Value, int Count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                sb.Append(Value);
            }
            return sb.ToString();
        }

        protected void TemplateDataReader(SqlDataStore adapter, IDataReader reader)
        {
            _templateData.Add(adapter.GetString(reader, 0), adapter.GetString(reader, 1));
        }

        private void MemoReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-Description, 1-Content, 2-MailingAddress, 3-addSignature
            // 4-dataSource, 5-officeSymbol, 6-addDate, 7-addSuspenseDate, 8-ApprovalRequired
            // 9-sigblock, 10-deleted, 11-letterhead, 12-userId, 13-accessScope, 14-creationDate
            // 15-memoDate, 16-suspenseDate, 17-restricted, 18-authorGroup, 19-fileName

            _description = adapter.GetString(reader, 0);
            _content = adapter.GetString(reader, 1);
            _mailingAddress = adapter.GetString(reader, 2);
            _addSignature = adapter.GetBoolean(reader, 3, false);
            _dataProc = adapter.GetString(reader, 4);
            _officeSymbol = adapter.GetString(reader, 5);
            _addDate = adapter.GetBoolean(reader, 6, true);
            _addSuspenseDate = adapter.GetBoolean(reader, 7, false);
            _approvalRequired = adapter.GetBoolean(reader, 8, false);
            _sigBlock = adapter.GetString(reader, 9);
            _deleted = adapter.GetBoolean(reader, 10);
            _letterHead = adapter.GetByte(reader, 11);
            _authorId = adapter.GetInteger(reader, 12);
            _scope = adapter.GetByte(reader, 13);
            _createdDate = adapter.GetDateTime(reader, 14);
            _memoDate = adapter.GetString(reader, 15);
            _suspenseDate = adapter.GetString(reader, 16);
            _authorGroup = adapter.GetByte(reader, 18);

            // get the letterhead file to use (if it's set)
            string file = adapter.GetString(reader, 19);
            if (file.Length > 0)
            {
                _templateFileName = file;
            }

            _viewRestricted = adapter.GetBoolean(reader, 20);
            _phi = adapter.GetBoolean(reader, 21);
        }

        private bool PopulateData()
        {
            if (_dataProc.Length == 0)
            {
                return false;
            }

            _templateData = new StringDictionary();

            DataStore.ExecuteReader(TemplateDataReader, _dataProc, _refId, DataKey, _templateId, CurrentUser.Id, CurrentUser.CurrentRole.Group.Id);

            foreach (string key in _templateData.Keys)
            {
                _content = _content.Replace("{" + key.ToUpper() + "}", _templateData[key]);
            }
            return true;
        }
    }
}
