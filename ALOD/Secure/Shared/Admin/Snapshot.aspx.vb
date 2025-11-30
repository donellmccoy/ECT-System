Imports System.Data.SqlClient
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Printing

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_Snapshot
        Inherits System.Web.UI.Page

        Public dsResults As New DataSet()
        Dim _daoFactory As IDaoFactory
        Dim _lookupDao As ILookupDao

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim rowCount As Int32 = 0

            If (Not Page.IsPostBack) Then
                rowCount = GetHistoricalData(0).Tables(0).Rows.Count
                lblTotal.Text = rowCount
                RangeValidator1.MaximumValue = Str(rowCount)
            Else
                rowCount = GetHistoricalData(0).Tables(0).Rows.Count.ToString()
                lblTotal.Text = rowCount
                RangeValidator1.MaximumValue = Str(rowCount)

            End If

        End Sub

        Class LODObject
            Dim _dao As ILineOfDutyDao
            Dim _docDao As IDocumentDao
            Dim _lod As LineOfDuty = Nothing
            Dim refId As Int32

            ReadOnly Property DocumentDao() As IDocumentDao
                Get
                    If (_docDao Is Nothing) Then
                        '  _docDao = New SRXDocumentStore(SESSION_USERNAME)
                        _docDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                    End If

                    Return _docDao
                End Get
            End Property

            ReadOnly Property LODDao() As ILineOfDutyDao
                Get
                    If (_dao Is Nothing) Then
                        _dao = New NHibernateDaoFactory().GetLineOfDutyDao()
                    End If

                    Return _dao
                End Get
            End Property

            Protected ReadOnly Property LOD() As LineOfDuty
                Get
                    If (_lod Is Nothing) Then
                        _lod = LODDao.GetById(refId, False)

                    End If
                    Return _lod
                End Get
            End Property

        End Class

#Region "BUTTON EVENTS"

        Protected Sub btnCreate_Click(sender As Object, e As System.EventArgs) Handles btnCreate.Click

            Dim dt As DataTable
            dt = CreateFiles()

            gvProcessed.Enabled = True
            gvProcessed.PageSize = dt.Rows.Count
            gvProcessed.Width() = 900
            gvProcessed.DataSource = dt
            gvProcessed.DataBind()
            gvProcessed.Visible = True
            divProcessed.Visible = True

            btnResults.Enabled = True
            btnCreate.Enabled = False

            gvHistorical.Enabled = False
            gvHistorical.Visible = False

        End Sub

        Protected Sub btnGet_Click(sender As Object, e As System.EventArgs) Handles btnGet.Click

            Dim ds As New DataSet

            gvHistorical.Enabled = True
            gvHistorical.Visible = True

            Try
                If (RangeValidator1.IsValid And RequiredFieldValidator1.IsValid) Then
                    ds.Clear()
                    ds = GetHistoricalData(CInt(txtNumDocs.Text))
                    gvHistorical.DataSource = ds
                    gvHistorical.DataBind()
                    gvHistorical.PageSize = ds.Tables(0).Rows.Count

                    If (ds.Tables(0).Rows.Count > 0) Then
                        btnCreate.Enabled = True
                    Else
                        btnCreate.Enabled = False
                    End If

                End If
            Catch ex As Exception
                lblError.Text = "An Error Occured: " & ex.Message.ToString()

            End Try

            gvResults.Enabled = False
            gvResults.Visible = False
            gvProcessed.Enabled = False
            gvProcessed.Visible = False

        End Sub

        Protected Sub btnResults_Click(sender As Object, e As System.EventArgs) Handles btnResults.Click

            gvResults.Visible = True
            gvResults.Enabled = True

            gvHistorical.Visible = False
            gvHistorical.Enabled = False
            gvProcessed.Visible = False
            gvProcessed.Enabled = False

            Dim ds As New DataSet()
            ds = ViewResults()
            gvResults.DataSource = ds
            gvResults.DataBind()
            gvResults.Width = 900

            If (ds.Tables(0).Rows.Count > 0) Then
                gvResults.PageSize = ds.Tables(0).Rows.Count
                lblFiles.Text = "Total Snapshots created: " & ds.Tables(0).Rows.Count
                lblFiles.Visible = True
                lblRecords.Text = ""
                lblRecords.Visible = False
            Else
                lblFiles.Text = "No Snapshots were created within this date/time range."
                lblRecords.Text = ""
                lblRecords.Visible = False

            End If

        End Sub

#End Region

#Region "FUNCTIONS AND SUBROUTINES"

        ' Creates new DocGroupIds for docs with no matching group id's;
        Function CreateDocGroupID(ByRef refid As Long, lodDao As LineOfDuty) As Long

            Dim docGroupID As Long
            Dim GetLOD As New LODObject()
            lodDao = GetLOD.LODDao.GetById(refid)
            lodDao.CreateDocumentGroup(GetLOD.DocumentDao)
            docGroupID = lodDao.DocumentGroupId

            Return docGroupID

        End Function

        Function CreateFiles() As DataTable

            Dim refId As Integer
            Dim recCount As Integer = 0
            Dim fileCount As Integer = 0
            Dim ds As New DataSet
            Dim LOD As New LineOfDuty()

            ds = GetHistoricalData(CInt(txtNumDocs.Text))
            lblDate.Text = DateTime.Now.ToString()
            txtStartDate.Text = DateTime.Today.ToShortDateString()
            txtStartTime.Text = DateTime.Now.ToShortTimeString()

            ' table of files processed
            Dim dt As New DataTable()
            dt.Columns.Add("CaseId")
            dt.Columns.Add("SSN")
            dt.Columns.Add("Description")
            dt.Columns.Add("DateAdded")
            dt.Columns.Add("DocDate")
            dt.Columns.Add("DocStatus")
            dt.Columns.Add("DocType")
            dt.Columns.Add("Ext")

            For Each dr As DataRow In ds.Tables(0).Rows

                Try

                    Dim GetLOD As New LODObject()
                    refId = dr("refId")
                    LOD = GetLOD.LODDao.GetById(refId)

                    ' check for missing document group id's in DX_Groups table
                    Dim groupID As Long
                    If (LOD.DocumentGroupId = 0 Or LOD.DocumentGroupId Is Nothing) Then
                        groupID = CreateDocGroupID(refId, LOD.DocumentDao)
                    Else
                        groupID = LOD.DocumentGroupId
                    End If

                    Dim description As String = ""
                    Dim file216 As String = ""

                    If (LOD.Formal) Then
                        '  is261 = True
                        description = "and Form261 "
                        file216 = "_Form261"

                    End If

                    Dim SavePDF As New PDFCreateFactory()
                    Dim doc As PDFDocument = SavePDF.GeneratePdf(LOD.Id, LOD.ModuleType)
                    Dim fileName As String = "Form348" & file216 & "-Case:" & LOD.CaseId.ToString() & "-Generated.pdf"

                    Dim docMetaData As Document = New Document()
                    docMetaData.DateAdded = DateTime.Now
                    docMetaData.DocDate = LOD.CreatedDate
                    docMetaData.Description = "Form348 " & description & "Generated for Case Id: " & LOD.CaseId.ToString()
                    docMetaData.DocStatus = DocumentStatus.Approved
                    docMetaData.Extension = "pdf"
                    docMetaData.SSN = LOD.MemberSSN
                    docMetaData.DocType = DocumentType.FinalForm348

                    Dim DocumentDao As New SRXDocumentStore()
                    doc.Render(fileName)
                    Dim docId As Long = DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)

                    dt.Rows.Add(LOD.CaseId, docMetaData.SSN, docMetaData.Description,
                                    docMetaData.DateAdded, docMetaData.DocDate, docMetaData.DocStatus,
                                    docMetaData.DocType, docMetaData.Extension)
                    fileCount += 1

                    'If (LOD.Formal) Then
                    '    doc = SavePDF.GeneratePdf(LOD.Id, False, True)
                    '    fileName = "Form261-Case" & LOD.CaseId.ToString() & "-Generated.pdf"
                    '    docMetaData.Description = "Form261 Generated for Case Id: " & LOD.CaseId.ToString()
                    '    docMetaData.DocType = DocumentType.FinalForm261

                    '    docId = DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)

                    '    dt.Rows.Add(LOD.CaseId, docMetaData.SSN, docMetaData.Description, _
                    '                    docMetaData.DateAdded, docMetaData.DocDate, docMetaData.DocStatus, _
                    '                    docMetaData.DocType, docMetaData.Extension)
                    '    fileCount += 1

                    'End If
                    recCount += 1

                    LogManager.LogAction(ModuleType.LOD, UserAction.AddedDocument, LOD.Id, "Generated Form 348 PDF")
                Catch ex As Exception
                    ALOD.Logging.LogManager.LogError(ex.Message.ToString(), ex.StackTrace.ToString(), "Snapshot")
                    Continue For

                End Try

            Next

            lblRecords.Text = "Total records processed: " & recCount
            lblRecords.Visible = True
            lblFiles.Text = "Total files created: " & fileCount
            lblFiles.Visible = True

            lblDate2.Text = DateTime.Now.ToString()
            txtEndDate.Text = DateTime.Today.ToShortDateString()
            txtEndTime.Text = DateTime.Now.AddMinutes(1).ToShortTimeString()

            Return dt

        End Function

        Function GetHistoricalData(ByVal recs As Integer) As DataSet

            Dim conn As New SqlConnection()
            conn.ConnectionString = ConfigurationManager.ConnectionStrings("LOD").ToString()

            Dim strRowCount = " "
            If recs > 0 Then
                strRowCount = "SET ROWCOUNT " & recs
            End If

            Dim strSQLall = strRowCount & "SELECT " &
                            "A.lodId AS refId, A.case_id, " &
                            "CASE A.formal_inv WHEN 1 THEN 'X' END AS [Formal Invest], " &
                            "CASE A.status WHEN 13 THEN 'Complete' END AS Status, " &
                            "A.doc_group_id, " &
                            "CONVERT(VARCHAR(10), B.endDate, 101) AS EndDate " &
                            "FROM Form348 A Join " &
                                "(SELECT MAX(wst_id) AS wst_id, refId, MAX(endDate) AS endDate, ws_id " &
                                    "FROM core_WorkStatus_Tracking " &
                                    "WHERE(ws_id = 13) GROUP BY ws_id, refId) B " &
                            "ON A.lodId = B.refId " &
                            "WHERE A.lodId NOT IN " &
                                "(SELECT a.lodId FROM Form348 a JOIN SRXLite.dbo.DX_GroupDocuments gd " &
                                    "ON a.doc_group_id = gd.GroupID " &
                                    "JOIN SRXLite.DBO.DX_Documents d on gd.DocID = d.DocID " &
                                    "WHERE d.DocTypeID = 74) " &
                            "ORDER BY B.endDate DESC"
            ' or d.DocTypeID = 75

            Dim adpt As New SqlDataAdapter(strSQLall, conn)
            Dim ds As New DataSet()
            adpt.Fill(ds, "Historical")

            Return ds

        End Function

        Function ViewResults() As DataSet

            ' handles spacing between date and time if user enters a time value
            Dim sDate As String = ""
            Dim eDate As String = ""

            If txtStartTime.Text.Trim.Length > 0 Then
                sDate = DateTime.Parse(txtStartDate.Text.Trim + " " + txtStartTime.Text.Trim)
            Else
                sDate = DateTime.Parse(txtStartDate.Text.Trim)
            End If

            If txtEndTime.Text.Trim.Length > 0 Then
                eDate = DateTime.Parse(txtEndDate.Text.Trim + " " + txtEndTime.Text.Trim)
            Else
                eDate = DateTime.Parse(txtEndDate.Text.Trim)
            End If

            Try
                'Dim conn As New SqlConnection()
                'conn.ConnectionString = ConfigurationManager.ConnectionStrings("SRXLite").ToString()

                'Dim strSQL As String = "SELECT A.lodId, A.case_id, A.doc_group_id, " & _
                '                            "G.DateCreated, G.DateLastModified, D.OriginalFileName " & _
                '                        "FROM [ALOD].[dbo].[Form348] A " & _
                '                        "INNER JOIN [SRXLite].[dbo].[DX_Groups] G " & _
                '                            "ON A.doc_group_id = G.GroupID " & _
                '                        "INNER JOIN [SRXLite].[dbo].[DX_GroupDocuments] GD " & _
                '                            "ON GD.GroupID = A.doc_group_id " & _
                '                        "INNER JOIN [SRXLite].[dbo].[DX_Documents] D " & _
                '                            "ON GD.DocID = D.DocID " & _
                '                        "WHERE (G.DateLastModified BETWEEN '" & sDate & "' " & _
                '                            "AND '" & eDate & "') " & _
                '                            "AND (A.status = 13) " & _
                '                            "AND (D.DocTypeID = 74) " & _
                '                        "ORDER BY G.DateLastModified DESC"
                ''  OR D.DocTypeID = 75

                'Dim adpt As New SqlDataAdapter(strSQL, conn)
                ''  Dim dsResults As New DataSet()
                'dsResults.Tables.Add("Documents")
                'adpt.Fill(dsResults, "Documents")

                Return LookupDao.GetForm348Snapshot(sDate, eDate)
            Catch ex As Exception
                lblError.Text = "Stack Trace: " & ex.StackTrace.ToString()
                lblError0.Text = "Error Message : " & ex.Message.ToString()

            End Try

            Return dsResults

        End Function

#End Region

#Region "Gridview Events"

        'Protected Sub gvHistorical_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvHistorical.PageIndexChanging

        '    gvHistorical.PageIndex = e.NewPageIndex

        '    Dim ds As New DataSet()
        '    ds = GetHistoricalData(CInt(txtNumDocs.Text))
        '    gvHistorical.DataSource = ds
        '    gvHistorical.DataBind()

        'End Sub

        'Protected Sub gvResults_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvResults.PageIndexChanging

        '    gvResults.PageIndex = e.NewPageIndex

        '    Dim ds As New DataSet()
        '    ds = ViewResults()
        '    gvResults.DataSource = ds
        '    gvResults.DataBind()

        'End Sub

        'Protected Sub gvProcessed_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvProcessed.PageIndexChanging

        '    gvProcessed.PageIndex = e.NewPageIndex

        '    Dim ds As New DataSet()
        '    ds = GetHistoricalData(CInt(txtNumDocs.Text))

        '    '  gvProcessed.DataSource = dt
        '    ' gvProcessed.DataBind()

        'End Sub

#End Region

    End Class

End Namespace