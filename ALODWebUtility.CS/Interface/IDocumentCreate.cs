using ALOD.Data.Services;
using ALODWebUtility.Printing;

public interface IDocumentCreate
{
    DBSignService VerifySource { get; set; }

    PDFForm GeneratePDFForm(int refId, bool replaceIOsig);
}
