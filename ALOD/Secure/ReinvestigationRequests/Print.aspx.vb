Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.RR

    Partial Class Secure_rr_Print
        Inherits System.Web.UI.Page

        Public url261 As String

        ' Holds URLs to retrieve Forms 348, 261
        Public url348 As String

        'this is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        'signatures which occured before this date use the old //signed// format
        'signatures which occured after this data use the new LAST.FIRST.MIDDLE.EDIPIN format
        Protected Const EpochDate As Date = #1/29/2010#

        Private Const BRANCH_AFRC As String = "AFRC"
        Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
        Private Const SIGNED_TEXT As String = "//SIGNED//"
        Private _factory As IDaoFactory
        Private _reinvestigation As LODReinvestigation
        Private _RRDao As ILODReinvestigateDAO
        Private lodid As Integer
        Dim type As ModuleType

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim Id As Integer = 0
            Integer.TryParse(Request.QueryString("id"), Id)

            If (Id = 0) Then
                Integer.TryParse(Request.QueryString("refId"), Id)
            End If

            If (Id = 0) Then
                Exit Sub
            End If

            Dim rrDao As ILODReinvestigateDAO = New NHibernateDaoFactory().GetLODReinvestigationDao()
            Dim rr As LODReinvestigation = rrDao.GetById(Id)
            Dim ModuleId As Integer
            Integer.TryParse(Request.QueryString("moduleId"), ModuleId)
            If ModuleId = ModuleType.ReinvestigationRequest Then
                lodid = rr.InitialLodId
            Else
                lodid = Id
            End If

            'save our id for use in other methods
            Dim dao As ILineOfDutyDao = New NHibernateDaoFactory().GetLineOfDutyDao()
            Dim userAccess As PageAccess.AccessLevel = dao.GetUserAccess(SESSION_USER_ID, lodid)

            If (userAccess = PageAccess.AccessLevel.None AndAlso Not UserHasPermission("lodViewAllCases")) Then
                Exit Sub
            End If

            Dim is348only As Boolean = False
            Dim is261only As Boolean = False

            Select Case HttpContext.Current.Request.QueryString("form")
                Case "348"
                    is348only = True
                Case "261"
                    is261only = True
            End Select

            '********************************************
            ' Resets Print button to view Final form 284/261
            Dim docUrl348 As New PrintFinal()
            url348 = docUrl348.GetURL348(lodid, HttpContext.Current.Session("UserName"), dao)

            If (Len(url348) > 1) Then
                Response.Write("go: " & url348)
                url348 = Me.ResolveClientUrl(url348)

                Dim docUrl261 As New PrintFinal()
                url261 = docUrl261.GetURL261(lodid, HttpContext.Current.Session("UserName"), dao)
                If (Len(url261) > 1) Then
                    Response.Write("go: " & url261)
                    url261 = Me.ResolveClientUrl(url261)

                End If
            Else
                'if forms were not saved to database or not final
                'Dim doc As PDFDocument = GeneratePdf(Id, is348only, is261only)
                Dim create As PDFCreateFactory = New PDFCreateFactory()

                Dim doc As PDFDocument = create.GeneratePdf(Id, ModuleType.LOD)
                If (doc IsNot Nothing) Then
                    doc.Render(Page.Response)
                    doc.Close()
                End If

            End If

        End Sub

    End Class

End Namespace