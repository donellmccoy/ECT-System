using System;

namespace SRXLite.DataTypes
{
    #region ActionType

    public enum ActionType
    {
        AddDocument = 1,
        DeleteDocument = 2,
        UpdateDocumentStatus = 3,
        AddDocumentPage = 4,
        DeleteDocumentPage = 5,
        AddGroup = 6,
        DeleteGroup = 7,
        AddGroupDocument = 8,
        DeleteGroupDocument = 9,
        AddGroupCopy = 10,
        UpdateDocumentKeys = 11,
        AddBatch = 12,
        GenerateDocumentPDF = 13
    }

    #endregion

    #region BatchType

    public enum BatchType : byte
    {
        Entity = 1,
        DocType = 2
    }

    #endregion

    #region DocumentStatus

    /// <summary>
    /// Document status enumeration
    /// </summary>
    public enum DocumentStatus : byte
    {
        Pending = 1,
        Approved = 2,
        Deleted = 3,
        Test = 4,
        Batch = 5
    }

    #endregion

    #region BatchDocumentType

    public enum BatchDocumentType
    {
        BatchImport = 2
    }

    #endregion

    #region InputType

    public enum InputType : byte
    {
        Upload = 1,
        Scan = 2,
        WebServiceUpload = 3
    }

    #endregion

    #region Category

    public enum Category
    {
        Document = 1,
        Group = 2,
        Batch = 3
    }

    #endregion

    #region UploadKeys

    public class UploadKeys
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public DocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
        public string FileName { get; set; }
        public InputType InputType { get; set; }
    }

    #endregion

    #region DocumentKeys

    public class DocumentKeys
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public DocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
    }

    #endregion

    #region DocumentData

    public class DocumentData
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public long DocID { get; set; }
        public DocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public string EntityName { get; set; }
        public string FileExt { get; set; }
        public string IconUrl { get; set; }
        public bool IsAppendable { get; set; }
        public string OriginalFileName { get; set; }
        public short PageCount { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBySubuserName { get; set; }
    }

    #endregion

    #region DocumentPageData

    public class DocumentPageData
    {
        public long DocPageID { get; set; }
        public short PageNumber { get; set; }
        public string PageUrl { get; set; }
    }

    #endregion

    #region BatchUploadKeys

    public class BatchUploadKeys
    {
        public BatchType BatchType { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
        public string Location { get; set; }
    }

    #endregion

    #region BatchData

    public class BatchData
    {
        public int BatchID { get; set; }
        public BatchType BatchType { get; set; }
        public DateTime DateCreated { get; set; }
        public string DocDescription { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public string EntityName { get; set; }
        public string Location { get; set; }
        public int PageCount { get; set; }
        public string UploadedBySubuserName { get; set; }
    }

    #endregion
}
