Imports ALOD.Data
Imports ALOD.Core.Domain.Workflow
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Domain.Modules.Lod

Namespace Web.LOD
    Partial Class Secure_lod_Print
        Inherits System.Web.UI.Page

        Private lodid As Integer
        Dim type As ModuleType

        Private Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
        Private Const SIGNED_TEXT As String = "//SIGNED//"
        Private Const BRANCH_AFRC As String = "AFRC"

        'this is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        'signatures which occured before this date use the old //signed// format
        'signatures which occured after this data use the new LAST.FIRST.MIDDLE.EDIPIN format
        Protected Const EpochDate As Date = #1/29/2010#

        ' Holds URLs to retrieve Forms 348, 261
        Public url348 As String
        Public url261 As String


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim Id As Integer = 0
            Integer.TryParse(Request.QueryString("id"), Id)

            If (Id = 0) Then
                Integer.TryParse(Request.QueryString("refId"), Id)
            End If

            If (Id = 0) Then
                Exit Sub
            End If


            'save our id for use in other methods
            lodid = Id
            Dim dao As ILineOfDutyDao = New NHibernateDaoFactory().GetLineOfDutyDao()
            Dim LOD As LineOfDuty = dao.GetById(lodid, False)
            Dim userAccess As PageAccess.AccessLevel = dao.GetUserAccess(SESSION_USER_ID, Id)


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
                url348 = Me.ResolveClientUrl(url348)

                Dim docUrl261 As New PrintFinal()
                url261 = docUrl261.GetURL261(lodid, HttpContext.Current.Session("UserName"), dao)
                If (Len(url261) > 1) Then
                    url261 = Me.ResolveClientUrl(url261)
                End If

            Else
                'if forms were not saved to database or not final
                Dim doc As PDFDocument
                Dim create As PDFCreateFactory = New PDFCreateFactory()

                doc = create.GeneratePdf(Id, ModuleType.LOD)

                If (doc IsNot Nothing) Then
                    doc.Render(Page.Response)
                    doc.Close()
                End If

            End If

        End Sub


    End Class
End Namespace
