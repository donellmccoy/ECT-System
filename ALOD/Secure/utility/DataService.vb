Imports System.Data.Common
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Xml
Imports AjaxControlToolkit
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps

<WebService()>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
<System.Web.Script.Services.ScriptService()>
Public Class DataService
    Inherits System.Web.Services.WebService

    Private _user As AppUser = Nothing
    Dim dataStore As New SqlDataStore
    Dim dbCommand As DbCommand
    Dim ds As New DataSet

    Protected ReadOnly Property CurrentUser() As AppUser
        Get
            If (_user Is Nothing) Then
                _user = UserService.CurrentUser()
            End If
            Return _user
        End Get
    End Property

    <WebMethod()>
    Public Function GetChangeSet(ByVal logId As Integer, ByVal userId As Integer) As XmlDocument

        If (userId > 0) Then
            Return GetChangeSetByUserId(userId)
        Else
            Return GetChangeSetByLogId(logId)
        End If

    End Function

    <WebMethod()>
    Public Function GetChangeSetByLogId(ByVal logId As Integer) As XmlDocument

        Dim doc As New XmlDocument()

        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Changes")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        Dim changes As New ChangeSet
        changes.GetByLogId(logId)
        changes.ToXml(root)

        Return doc

    End Function

    <WebMethod()>
    Public Function GetChangeSetByReferenceId(ByVal refId As Integer, ByVal type As Byte) As XmlDocument

        Dim doc As New XmlDocument()

        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Changes")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        Dim changes As New ChangeSet
        changes.GetByReferenceId(refId, type)
        changes.ToXml(root)

        Return doc

    End Function

    <WebMethod()>
    Public Function GetChangeSetByUserId(ByVal userId As Integer) As XmlDocument
        Dim doc As New XmlDocument()

        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Changes")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        Dim changes As New ChangeSet
        changes.GetByUserID(userId)
        changes.ToXml(root, False)

        Return doc
    End Function

    <WebMethod()>
    Public Function GetDbSignInfo(ByVal refId As Integer, ByVal secId As Integer, ByVal template As Short) As XmlDocument

        Dim doc As New XmlDocument()
        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Whois")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        Dim node As XmlNode = doc.CreateElement("User")
        node.Attributes.Append(doc.CreateAttribute("Name"))
        node.Attributes.Append(doc.CreateAttribute("DateSigned"))

        Dim sign As New DBSignService(template, refId, secId)
        Dim info As DigitalSignatureInfo = sign.GetSignerInfo()

        node.Attributes("Name").Value = info.Name
        node.Attributes("DateSigned").Value = info.DateSigned

        root.AppendChild(node)
        Return doc

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetMessages() As XmlDocument

        Dim xml As New XmlDocument
        Dim root As XmlElement = xml.CreateElement("Root")
        xml.AppendChild(root)
        Dim list As New MessageList
        list = list.RetrieveMessages(CurrentUser.Id, CurrentUser.CurrentRole.Group.Id, True)
        For Each message As Message In list
            Dim node As XmlNode = xml.CreateElement("Row")
            Dim attr1 As XmlAttribute = xml.CreateAttribute("Message")
            Dim attr2 As XmlAttribute = xml.CreateAttribute("From")
            Dim attr3 As XmlAttribute = xml.CreateAttribute("Date")

            attr1.Value = message.Message
            attr2.Value = message.Name
            attr3.Value = message.StartTime
            node.Attributes.Append(attr1)
            node.Attributes.Append(attr2)
            node.Attributes.Append(attr3)
            root.AppendChild(node)
        Next
        Return xml
    End Function

    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Function GetUnits(ByVal p As String, ByVal descr As String, ByVal active As String) As XmlDocument

        Dim pascode As String = p.Trim
        Dim longName As String = descr.Trim

        Dim activeOnly As Boolean
        Boolean.TryParse(active, activeOnly)

        dbCommand = dataStore.GetStoredProcCommand("core_pascode_sp_search")
        dataStore.AddInParameter(dbCommand, "@pascode", DbType.String, pascode)
        dataStore.AddInParameter(dbCommand, "@longName", DbType.String, longName)
        dataStore.AddInParameter(dbCommand, "@active", DbType.Boolean, activeOnly)
        ds = dataStore.ExecuteDataSet(dbCommand)

        Dim xmlDoc As XmlDocument = New XmlDocument()
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
        Return xmlDoc

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetWhois(ByVal userId As String) As XmlDocument

        Dim data As New DataSet()

        data = dataStore.ExecuteDataSet("core_user_sp_GetWhois", userId)
        data.DataSetName = "Results"

        If (data.Tables.Count > 0) Then
            data.Tables(0).TableName = "User"
        End If

        Dim doc As New XmlDocument

        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Whois")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        Dim node As XmlNode = doc.CreateElement("User")

        node.Attributes.Append(doc.CreateAttribute("LastName"))
        node.Attributes.Append(doc.CreateAttribute("FirstName"))
        node.Attributes.Append(doc.CreateAttribute("Rank"))
        node.Attributes.Append(doc.CreateAttribute("Address"))
        node.Attributes.Append(doc.CreateAttribute("City"))
        node.Attributes.Append(doc.CreateAttribute("State"))
        node.Attributes.Append(doc.CreateAttribute("Zip"))
        node.Attributes.Append(doc.CreateAttribute("Phone"))
        node.Attributes.Append(doc.CreateAttribute("DSN"))
        node.Attributes.Append(doc.CreateAttribute("Email"))
        node.Attributes.Append(doc.CreateAttribute("Role"))
        node.Attributes.Append(doc.CreateAttribute("Region"))

        If (data.Tables(0).Rows.Count > 0) Then

            If (Not data.Tables(0).Rows(0)("LastName") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("LastName") <> "") Then
                node.Attributes("LastName").Value = data.Tables(0).Rows(0)("LastName")
            Else
                node.Attributes("LastName").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("FirstName") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("FirstName") <> "") Then
                node.Attributes("FirstName").Value = data.Tables(0).Rows(0)("FirstName")
            Else
                node.Attributes("FirstName").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Rank") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Rank") <> "") Then
                node.Attributes("Rank").Value = data.Tables(0).Rows(0)("Rank")
            Else
                node.Attributes("Rank").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Address") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Address") <> "") Then
                node.Attributes("Address").Value = data.Tables(0).Rows(0)("Address")
            Else
                node.Attributes("Address").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("City") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("City") <> "") Then
                node.Attributes("City").Value = data.Tables(0).Rows(0)("City")
            Else
                node.Attributes("City").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("State") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("State") <> "") Then
                node.Attributes("State").Value = data.Tables(0).Rows(0)("State")
            Else
                node.Attributes("State").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Zip") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Zip") <> "") Then
                node.Attributes("Zip").Value = data.Tables(0).Rows(0)("Zip")
            Else
                node.Attributes("Zip").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Phone") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Phone") <> "") Then
                node.Attributes("Phone").Value = data.Tables(0).Rows(0)("Phone")
            Else
                node.Attributes("Phone").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("DSN") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("DSN") <> "") Then
                node.Attributes("DSN").Value = data.Tables(0).Rows(0)("DSN")
            Else
                node.Attributes("DSN").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Email") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Email") <> "") Then
                node.Attributes("Email").Value = data.Tables(0).Rows(0)("Email")
            Else
                node.Attributes("Email").Value = "N/A"
            End If
            If (Not data.Tables(0).Rows(0)("Role") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Role") <> "") Then
                node.Attributes("Role").Value = data.Tables(0).Rows(0)("Role")
            Else
                node.Attributes("Role").Value = "N/A"
            End If
            'If (Not data.Tables(0).Rows(0)("Region") Is DBNull.Value AndAlso data.Tables(0).Rows(0)("Region") <> "") Then
            '    node.Attributes("Region").Value = data.Tables(0).Rows(0)("Region")
            'Else
            '    node.Attributes("Region").Value = "N/A"
            'End If

        End If

        root.AppendChild(node)

        Return doc

    End Function

    <WebMethod()>
    Public Function GetWorkflowUsers(ByVal refId As String, ByVal moduleId As String, ByVal wsStatus As String, ByVal MemberUnitId As String) As XmlDocument

        Dim data As New DataSet()

        data = dataStore.ExecuteDataSet("workflow_sp_Tracking_GetCurrentUsers", refId, moduleId, wsStatus, MemberUnitId)
        data.DataSetName = "Results"

        If (data.Tables.Count > 0) Then
            data.Tables(0).TableName = "User"
        End If

        Dim doc As New XmlDocument

        Dim declaration As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", Nothing)
        Dim root As XmlNode = doc.CreateElement("Whois")
        doc.InsertBefore(declaration, doc.DocumentElement)
        doc.AppendChild(root)

        For Each row As DataRow In data.Tables(0).Rows

            Dim node As XmlNode = doc.CreateElement("User")

            node.Attributes.Append(doc.CreateAttribute("LastName"))
            node.Attributes.Append(doc.CreateAttribute("FirstName"))
            node.Attributes.Append(doc.CreateAttribute("Rank"))
            node.Attributes.Append(doc.CreateAttribute("Address"))
            node.Attributes.Append(doc.CreateAttribute("City"))
            node.Attributes.Append(doc.CreateAttribute("State"))
            node.Attributes.Append(doc.CreateAttribute("Zip"))
            node.Attributes.Append(doc.CreateAttribute("Phone"))
            node.Attributes.Append(doc.CreateAttribute("DSN"))
            node.Attributes.Append(doc.CreateAttribute("Email"))
            node.Attributes.Append(doc.CreateAttribute("Role"))
            'node.Attributes.Append(doc.CreateAttribute("Region"))

            If (data.Tables(0).Rows.Count > 0) Then
                If (Not row("LastName") Is DBNull.Value AndAlso row("LastName") <> "") Then
                    node.Attributes("LastName").Value = row("LastName")
                Else
                    node.Attributes("LastName").Value = "N/A"
                End If
                If (Not row("FirstName") Is DBNull.Value AndAlso row("FirstName") <> "") Then
                    node.Attributes("FirstName").Value = row("FirstName")
                Else
                    node.Attributes("FirstName").Value = "N/A"
                End If
                If (Not row("Rank") Is DBNull.Value AndAlso row("Rank") <> "") Then
                    node.Attributes("Rank").Value = row("Rank")
                Else
                    node.Attributes("Rank").Value = "N/A"
                End If
                If (Not row("Address") Is DBNull.Value AndAlso row("Address") <> "") Then
                    node.Attributes("Address").Value = row("Address")
                Else
                    node.Attributes("Address").Value = "N/A"
                End If
                If (Not row("City") Is DBNull.Value AndAlso row("City") <> "") Then
                    node.Attributes("City").Value = row("City")
                Else
                    node.Attributes("City").Value = "N/A"
                End If
                If (Not row("State") Is DBNull.Value AndAlso row("State") <> "") Then
                    node.Attributes("State").Value = row("State")
                Else
                    node.Attributes("State").Value = "N/A"
                End If
                If (Not row("Zip") Is DBNull.Value AndAlso row("Zip") <> "") Then
                    node.Attributes("Zip").Value = row("Zip")
                Else
                    node.Attributes("Zip").Value = "N/A"
                End If
                If (Not row("Phone") Is DBNull.Value AndAlso row("Phone") <> "") Then
                    node.Attributes("Phone").Value = row("Phone")
                Else
                    node.Attributes("Phone").Value = "N/A"
                End If
                If (Not row("DSN") Is DBNull.Value AndAlso row("DSN") <> "") Then
                    node.Attributes("DSN").Value = row("DSN")
                Else
                    node.Attributes("DSN").Value = "N/A"
                End If
                If (Not row("Email") Is DBNull.Value AndAlso row("Email") <> "") Then
                    node.Attributes("Email").Value = row("Email")
                Else
                    node.Attributes("Email").Value = "N/A"
                End If
                If (Not row("Role") Is DBNull.Value AndAlso row("Role") <> "") Then
                    node.Attributes("Role").Value = row("Role")
                Else
                    node.Attributes("Role").Value = "N/A"
                End If
                'If (Not row("Region") Is DBNull.Value AndAlso row("Region") <> "") Then
                '    node.Attributes("Region").Value = row("Region")
                'Else
                '    node.Attributes("Region").Value = "N/A"
                'End If

            End If

            root.AppendChild(node)

        Next

        Return doc

    End Function

    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Sub LogUserAction(ByVal encryptedQString As String, ByVal userAction As String, ByVal comment As String)
        Dim query As New SecureQueryString(encryptedQString)
        Dim refId As Integer = query.GetInteger("refId")
        Dim type As ModuleType = [Enum].Parse(GetType(ModuleType), (query("module")))
        Dim status As Integer = query("status")
        LogManager.LogAction(type, CType(userAction, UserAction), refId, comment, status)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function SetMessagesRead() As Boolean
        'Dim mUser As MND.MndUser = Session("User")
        'Dim mStore As New MndUserStore
        Dim list As New MessageList
        list.SetMessagesRead(CurrentUser.Id, CurrentUser.CurrentRole.Group.Id)
        Return True
    End Function

    <WebMethod()>
    Public Function ValidPascode(ByVal pascode As String) As Boolean
        Return New pascode().IsValidPascode(pascode)
    End Function

#Region "CascadingDropdown for medical diagnosis"

    Public Function GetCaseCount(wsId As Int32, compo As Int16) As Integer
        dbCommand = dataStore.GetStoredProcCommand("form348_sp_PilotCaseSearch", wsId, compo)
        ds = dataStore.ExecuteDataSet(dbCommand)
        If ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows.Count
        Else
            Return 0
        End If

        'Return dataStore.ExecuteDataSet(dbCommand)

    End Function

    ''' <summary>
    '''<para>The method calls lookup service method GetChild Units.
    ''' It Passes current session unit id and Report View
    ''' </para>
    ''' </summary>
    ''' <returns>A list of of child units as look up items list</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetChildUnits() As List(Of ALOD.Core.Domain.Users.LookUpItem) '
        Return LookupService.GetChildUnits(CInt(HttpContext.Current.Session("UnitId")), CByte(HttpContext.Current.Session("ReportView")))
    End Function

    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetCompletedAppealCount() As Integer  '
        Return LodService.GetCompletedAppealCount(SESSION_USER_ID, SESSION_REPORT_VIEW, 0, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()

    End Function

    ''' <summary>
    '''<para>The method calls LodService GetCompletedCount.
    ''' It passes current session user's id ,reporting view
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing completed Lod count for MPF</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetCompletedLodCount() As Integer  '
        Dim wsId As Integer
        If (SESSION_GROUP_ID = 2) Then
            wsId = 330
        Else
            wsId = SESSION_WS_ID()
        End If

        Dim compo As String = SESSION_COMPO.ToString()
        Return LodService.GetCompletedCount(SESSION_USER_ID, SESSION_REPORT_VIEW, 0, UserHasPermission(PERMISSION_VIEW_SARC_CASES), False, wsId, compo).ToString()
    End Function

    <WebMethod()>
    Public Function GetICDCategory(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Dim kv As StringDictionary = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues)

        Dim headingId As Integer

        If (Not kv.ContainsKey("Heading")) Then
            Return Nothing
        End If

        headingId = CType(kv("Heading"), Integer)

        Dim icd9Listings As IEnumerable(Of ICD9Code) = provider.GetCategory(headingId)

        Dim values As New List(Of CascadingDropDownNameValue)

        For Each item As ICD9Code In icd9Listings
            values.Add(New CascadingDropDownNameValue(item.Description, item.Id))
        Next

        Return values.ToArray()

    End Function

    <WebMethod()>
    Public Function GetICDChapter(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()

        Dim provider As New LookUp

        Dim o As IEnumerable(Of ICD9Code) = provider.GetChapter()
        Dim values As New List(Of CascadingDropDownNameValue)

        For Each k As ICD9Code In o
            values.Add(New CascadingDropDownNameValue(k.Description, k.Id))
        Next

        Return values.ToArray()

    End Function

    <WebMethod()>
    Public Function GetICDContent(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Dim kv As StringDictionary = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues)

        Dim categoryId As Integer

        If (Not kv.ContainsKey("Category")) Then
            Return Nothing
        End If

        categoryId = CType(kv("Category"), Integer)
        Dim icd9Listings As IEnumerable(Of ICD9Code) = provider.GetCategory(categoryId)

        Dim values As New List(Of CascadingDropDownNameValue)

        For Each item As ICD9Code In icd9Listings
            values.Add(New CascadingDropDownNameValue(item.Description & " - " & item.Code, item.Id))
        Next

        Return values.ToArray()

    End Function

    <WebMethod()>
    Public Function GetICDDiagnosisLevel1(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Return provider.GetICDCodeCDDValues(CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues), "Section")
    End Function

    <WebMethod()>
    Public Function GetICDDiagnosisLevel2(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Return provider.GetICDCodeCDDValues(CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues), "DiagnosisLevel1")
    End Function

    <WebMethod()>
    Public Function GetICDDiagnosisLevel3(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Return provider.GetICDCodeCDDValues(CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues), "DiagnosisLevel2")
    End Function

    <WebMethod()>
    Public Function GetICDDiagnosisLevel4(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Return provider.GetICDCodeCDDValues(CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues), "DiagnosisLevel3")
    End Function

    <WebMethod()>
    Public Function GetICDIncident(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Dim kv As StringDictionary = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues)

        Dim categoryId As Integer

        If (Not kv.ContainsKey("Category")) Then
            Return Nothing
        End If

        categoryId = CType(kv("Category"), Integer)
        Dim icd9Listings As IEnumerable(Of ICD9Code) = provider.GetCategory(categoryId)

        Dim values As New List(Of CascadingDropDownNameValue)

        If categoryId < 467 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 466 And categoryId < 475 Then
            values.Add(New CascadingDropDownNameValue("Illness", "Illness"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 474 And categoryId < 485 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId = 485 Then
            values.Add(New CascadingDropDownNameValue("Illness", "Illness"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 485 And categoryId < 493 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId = 493 Then
            values.Add(New CascadingDropDownNameValue("Illness", "Illness"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 493 And categoryId < 780 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 779 And categoryId < 800 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Illness", "Illness"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 799 And categoryId < 812 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 811 And categoryId < 919 Then
            values.Add(New CascadingDropDownNameValue("Injury", "Injury"))
            values.Add(New CascadingDropDownNameValue("Injury MVA", "Injury-MVA"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 918 And categoryId < 925 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 924 And categoryId < 982 Then
            values.Add(New CascadingDropDownNameValue("Injury", "Injury"))
            values.Add(New CascadingDropDownNameValue("Injury MVA", "Injury-MVA"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 981 And categoryId < 1043 Then
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        ElseIf categoryId > 1042 And categoryId < 1045 Then
            values.Add(New CascadingDropDownNameValue("Injury", "Injury"))
            values.Add(New CascadingDropDownNameValue("Injury MVA", "Injury-MVA"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        Else
            values.Add(New CascadingDropDownNameValue("Illness", "Illness"))
            values.Add(New CascadingDropDownNameValue("Injury", "Injury"))
            values.Add(New CascadingDropDownNameValue("Injury MVA", "Injury-MVA"))
            values.Add(New CascadingDropDownNameValue("Disease", "Disease"))
            values.Add(New CascadingDropDownNameValue("Death", "Death"))
        End If

        Return values.ToArray()

    End Function

    <WebMethod()>
    Public Function GetICDSection(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim provider As New LookUp
        Return provider.GetICDCodeCDDValues(CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues), "Chapter")
    End Function

    Public Function GetInConsultCaseCount() As Integer
        dbCommand = dataStore.GetStoredProcCommand("form348_sp_InConsultCaseSearch_all")
        ds = dataStore.ExecuteDataSet(dbCommand)
        If ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows.Count
        Else
            Return 0
        End If

        'Return dataStore.ExecuteDataSet(dbCommand)

    End Function

    '<summary>
    '<para>The method calls LodService's GetByUser method.
    '</para>
    '<param name="caseID">case Id of the searched lods</param>
    '<param name="ssn">SSN of the searched lods</param>
    '<param name="name">name  the searched lods</param>
    '<param name="isFormal">States if only the formal cases should be searched</param>
    '<param name="unitId">Unit Id of the searched lods</param>
    '</summary>
    ' <returns>A dataset containing all lods which users have view access</returns>
    '<WebMethod(EnableSession:=True)>
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    '<summary>
    '<para>The method calls LodService's GetUndeletedlLods method.
    '</para>
    '<param name="caseID">case Id of the searched lods</param>
    '<param name="ssn">SSN of the searched lods</param>
    '<param name="name">name  the searched lods</param>
    '</summary>
    ' <returns>A dataset containing all lods for the member</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetLodsByUser(ByVal caseID As String, ByVal ssn As String, ByVal name As String, ByVal isFormal As String, ByVal unitId As Integer) As DataSet   '

        Return ALOD.Data.Services.LodService.GetByUser(caseID, ssn, name, 0, Integer.Parse(SESSION_USER_ID), Byte.Parse(SESSION_REPORT_VIEW), SESSION_COMPO, 0, ModuleType.LOD, isFormal, unitId, UserHasPermission(PERMISSION_VIEW_SARC_CASES))

    End Function

    ' <summary>
    '<para>The method calls LodService GetPendingAppealCount.
    ' It Passes current session user's id and and sarc permission status
    ' </para>
    '</summary>
    ' <returns>An Integer value representing pending Inbox count for the user</returns>
    '<WebMethod(EnableSession:=True)>
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    'Public Function GetPendingLegacyLodCount() As Integer
    '    Return LodService.GetPendingLegacyLodCount(SESSION_USER_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
    'End Function
    ''' <summary>
    '''<para>The method calls LodService GetPendingAppealCount.
    ''' It Passes current session user's id and and sarc permission status
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetPendingAppealCount() As Integer  '
        Dim result As Integer = LodService.GetPendingAppealCount(SESSION_USER_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
        Return result
    End Function

    ''' <summary>
    '''<para>The method calls LodService GetPendingAppealCount.
    ''' It Passes current session user's id and and sarc permission status
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetPendingIOLodCount() As Integer  '
        Return LodService.GetPendingIOCount(SESSION_USER_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls LodService GetPendingCount.
    ''' It Passes current session user's id and and sarc permission status
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetPendingLodCount() As Integer  '
        Return LodService.GetPendingCount(SESSION_USER_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
    End Function

    ' '''<summary>
    ' '''<para>The method calls LookupService's GetRMUNames method.
    ' ''' It passes current session user's id.
    ' ''' </para>
    ' '''</summary>
    ' ''' <returns>An Integer value representing user's count with pending status</returns>
    '<WebMethod(EnableSession:=True)> _
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    'Public Function GetRMUNames(searchBegin As String) As Integer  '
    '    Return LookupService.GetRMUNames(searchBegin).ToString()
    'End Function
    ' '''<summary>
    ' '''<para>The method calls LookupService's GetMedGroupNames method.
    ' ''' It passes current session user's id.
    ' ''' </para>
    ' '''</summary>
    ' ''' <returns>An Integer value representing user's count with pending status</returns>
    '<WebMethod(EnableSession:=True)> _
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    'Public Function GetMedGroupNames(searchBegin As String) As Integer  '
    '    Return LookupService.GetMedGroupNames(searchBegin).ToString()
    'End Function
    '''<summary>
    '''<para>The method calls UserService's GetPendingRoleRequestCount method.
    ''' It passes current session user's id.
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing Role Request Count with pending status</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetPendingRoleRequestCount() As Integer  '
        Dim factory As New NHibernateDaoFactory()
        Return factory.GetUserRoleRequestDao().GetAllPendingRequests(SESSION_USER_ID).Tables(0).Rows.Count
    End Function

    '''<summary>
    '''<para>The method calls UserService's GetPendingCount method.
    ''' It passes current session user's id.
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing user's count with pending status</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetPendingUserCount() As Integer  '
        Return UserService.GetPendingCount(SESSION_USER_ID).ToString()
    End Function

    Public Function GetPilotCases(wsId As Int16) As Integer
        dbCommand = dataStore.GetStoredProcCommand("form348_sp_PilotCaseSearch", wsId, SESSION_COMPO)
        ds = dataStore.ExecuteDataSet(dbCommand)
        If ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows.Count
        Else
            Return 0
        End If

        'Return dataStore.ExecuteDataSet(dbCommand)

    End Function

    ' <summary>
    '<para>The method calls LodService GetCompletedCount.
    ' It passes current session user's id ,reporting view
    ' </para>
    '</summary>
    ' <returns>An Integer value representing completed Lod count for MPF</returns>
    '
    '<WebMethod(EnableSession:=True)>
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    'Public Function GetCompletedLegacyLodCount() As Integer  '
    '    Dim wsId As Integer = SESSION_WS_ID()
    '    Dim compo As String = SESSION_COMPO.ToString()
    '    Return LodService.GetCompletedCount(SESSION_USER_ID, SESSION_REPORT_VIEW, 0, UserHasPermission(PERMISSION_VIEW_SARC_CASES), False, wsId, compo).ToString()
    'End Function
    ''' <summary>
    '''<para>The method calls LodService GetReinvestigationRequestCount.
    ''' It Passes current session user's id and and sarc permission status
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetReinvestigationRequestCount() As Integer  '
        Return LodService.GetReinvestigationRequestCount(SESSION_USER_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls SARCSErvice GetRestrictedSARCsCount.
    ''' It Passes current session user's id
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetRestrictedSARCsCaseCount() As Integer  '
        Return SARCService.GetRestrictedSARCsCount(SESSION_USER_ID).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls SARCSErvice GetRestrictedSARCsPostCompletionCount.
    ''' It Passes current session user's id
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetRestrictedSARCsPostCompletionCaseCount() As Integer  '
        Return SARCService.GetRestrictedSARCsPostCompletionCount(SESSION_USER_ID).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls SARCSErvice GetRestrictedSARCsCount.
    ''' It Passes current session user's id
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSARCAppealsCaseCount() As Integer  '
        Return LodService.GetPendingSARCAppealCount(SESSION_USER_ID, UserHasPermission("SARC")).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls SARCSErvice GetRestrictedSARCsPostCompletionCount.
    ''' It Passes current session user's id
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSARCAppealsPostCompletionCaseCount() As Integer  '
        Return LodService.GetCompletedSARCAppealCount(SESSION_USER_ID, SESSION_REPORT_VIEW, 0, UserHasPermission(PERMISSION_VIEW_SARC_CASES)).ToString()
    End Function

    ''' <summary>
    '''<para>The method calls LodService GetSpecialCasesCount.
    ''' It Passes current session user's id and and sarc permission status
    ''' </para>
    '''</summary>
    ''' <returns>An Integer value representing pending Inbox count for the user</returns>
    <WebMethod(EnableSession:=True)>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSpecialCasesCount() As Integer
        Try

            If (HttpContext.Current Is Nothing) OrElse (HttpContext.Current.Session Is Nothing) Then
                Throw New HttpException(501, "Session Error 1")
            End If

            If IsNothing(SESSION_USER_ID) Then
                Throw New HttpException(502, "Session Error 2")
            End If

            Return LodService.GetSpecialCasesCount(SESSION_USER_ID).ToString()
        Catch ex As Exception
            Throw New HttpException(503, "Session Error 3")
        End Try
    End Function

    Public Function GetUnDeletedLodsForMember(ByVal caseID As String, ByVal ssn As String, ByVal name As String) As DataSet   '
        Return ALOD.Data.Services.LodService.GetUndeletedlLods(caseID, ssn, name, 0, Integer.Parse(SESSION_USER_ID), Byte.Parse(SESSION_REPORT_VIEW), SESSION_COMPO, 0, ModuleType.LOD, Nothing, SESSION_UNIT_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES), True)
    End Function

#End Region

End Class