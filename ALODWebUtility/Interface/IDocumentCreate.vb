Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Public Interface IDocumentCreate

    Property VerifySource() As DBSignService

    Function GeneratePDFForm(ByVal refId As Integer, ByVal replaceIOsig As Boolean) As PDFForm

End Interface