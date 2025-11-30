Imports System.Data.Common
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Xml
Imports ALOD.Data
Imports ALODWebUtility.Common

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class PublicDataService
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Function GetUnits(ByVal p As String, ByVal descr As String, ByVal active As String) As XmlDocument

        Dim xmlDoc As XmlDocument = New XmlDocument()

        If (Not [String].IsNullOrEmpty(SESSION_EDIPIN)) Then

            Dim pascode As String = p.Trim
            Dim longName As String = descr.Trim

            Dim ds As New DataSet
            Dim adapter As New SqlDataStore
            Dim dbCommand As DbCommand

            Dim activeOnly As Boolean
            Boolean.TryParse(active, activeOnly)

            dbCommand = adapter.GetStoredProcCommand("core_pascode_sp_search")
            adapter.AddInParameter(dbCommand, "@pascode", DbType.String, pascode)
            adapter.AddInParameter(dbCommand, "@longName", DbType.String, longName)
            adapter.AddInParameter(dbCommand, "@activeOnly", DbType.Boolean, activeOnly)

            ds = adapter.ExecuteDataSet(dbCommand)

            If ds.Tables.Count > 0 Then
                ds.Tables(0).TableName = "Unit"
                If ds.Tables(0).Rows.Count > 0 Then
                    xmlDoc.LoadXml(ds.GetXml())
                    Return xmlDoc
                End If
            End If

            Dim xmldecl As XmlDeclaration
            xmldecl = xmlDoc.CreateXmlDeclaration("1.0", Nothing, Nothing)
            'Add the new node to the document.
            Dim root As XmlElement = xmlDoc.DocumentElement
            xmlDoc.InsertBefore(xmldecl, root)

        End If

        Return xmlDoc

    End Function

End Class