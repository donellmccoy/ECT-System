Option Strict On

Imports System.Web.Services
Imports SRXLite.Classes
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Tools

    Partial Class Tools_DocumentViewer
        Inherits System.Web.UI.Page

        Private _docGuid As DocumentGuid
        Private _doc As Document
        Private Shared _docPage As DocumentPage
        Private Shared _docPageGuid As DocumentPageGuid

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _docGuid = New DocumentGuid(Request.QueryString("id"))

            Dim task As New PageAsyncTask( _
            New BeginEventHandler(AddressOf BeginGetDocumentGuidData), _
            New EndEventHandler(AddressOf EndGetDocumentGuidData), _
            New EndEventHandler(AddressOf AsyncTaskTimeout), _
            Nothing)
            Dim PrintButton As Boolean = CBool(ConfigurationManager.AppSettings("PrintButton"))
            divPrint.Visible = PrintButton

            RegisterAsyncTask(task)
        End Sub

#Region " AsyncTaskTimeout "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub AsyncTaskTimeout(ByVal result As IAsyncResult)
            'TODO: timeout code
        End Sub
#End Region

#Region " GetDocumentGuidData (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginGetDocumentGuidData(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Return _docGuid.BeginGetData(callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndGetDocumentGuidData(ByVal result As IAsyncResult)
            Dim data As New DocumentGuid.GuidData
            data = _docGuid.EndGetData(result)

            If data.DocID = 0 Then
                'Guid was not found or has expired
                'TODO: 
                divContent.InnerText = "This page has expired."

            Else

                _doc = New Document(data)
                lblDocType.Text = data.DocTypeName

                Dim task As New PageAsyncTask( _
                New BeginEventHandler(AddressOf BeginGetDocumentPageList), _
                New EndEventHandler(AddressOf EndGetDocumentPageList), _
                New EndEventHandler(AddressOf AsyncTaskTimeout), _
                Nothing)

                RegisterAsyncTask(task)

            End If

        End Sub
#End Region

#Region " GetDocumentPageList (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginGetDocumentPageList(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Return _doc.BeginGetPageList(callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndGetDocumentPageList(ByVal result As IAsyncResult)

            ProcessPageList(_doc.EndGetPageList(result))

        End Sub
#End Region

#Region " ProcessPageList "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="list"></param>
        ''' <remarks></remarks>
        Protected Sub ProcessPageList(ByVal list As List(Of DocumentPageData))
            Try
                Dim data As DocumentPageData

                If _doc.IsImage AndAlso _doc.IsAppendable Then
                    Dim crypto As New CryptoManager()
                    HtmlBody.Attributes.Add("onload", "DocumentOnLoad();")
                    menuDelete.Visible = (Not _docGuid.Data.IsReadOnly) AndAlso (list.Count > 1)
                    ScriptManager1.EnablePageMethods = Not _docGuid.Data.IsReadOnly
                    divPDF.Attributes.Add("onclick", String.Format("window.location='{0}';", "../Handlers/GenerateDocumentPDF.ashx?id=" & Request.QueryString("id")))

                    For i As Integer = 0 To list.Count - 1
                        data = list(i)
                        Using img As New HtmlImage
                            Using imgThumbnail As New HtmlImage
                                img.ID = "p" & i
                                img.Alt = ""
                                imgThumbnail.ID = "tp" & i

                                'Images
                                With img.Attributes
                                    .Add("class", "page")
                                    .Add("thumbid", imgThumbnail.ClientID)
                                    .Add("xsrc", data.PageUrl & "&c=0&r=&h=&w=")    'Set src and width in JS to fit window
                                    .Add("onclick", "onClickImg(event,this);")
                                    .Add("oncontextmenu", "return onContextMenuImg(event,this);")
                                End With
                                divImages.Controls.Add(img)

                                'Thumbnail page number labels
                                Using lbl As New HtmlGenericControl
                                    lbl.TagName = "span"
                                    lbl.Attributes.Add("class", "pagenum")
                                    lbl.InnerHtml = (i + 1).ToString
                                    divThumbnails.Controls.Add(lbl)
                                End Using


                                'Thumbnails
                                With imgThumbnail.Attributes
                                    .Add("for", img.ClientID)
                                    .Add("class", "page")
                                    .Add("tsrc", data.PageUrl & "&c=1&r=&h=&w=120")
                                    .Add("onclick", "onClickImg(event,this);")
                                End With
                                divThumbnails.Controls.Add(imgThumbnail)

                            End Using
                        End Using
                    Next

                Else
                    data = list(0)
                    divContent.Controls.Clear()
                    ScriptManager1.EnablePageMethods = False

                    Using iframe As New HtmlGenericControl
                        iframe.TagName = "iframe"
                        iframe.ID = "ifrDocument"
                        iframe.Attributes.Add("src", data.PageUrl)
                        iframe.Attributes.Add("style", "width:100%; height:100%")
                        divContent.Controls.Add(iframe)
                    End Using

                    menu.Visible = False 'Image context menu not needed

                    If Not _doc.IsBrowserViewable Then
                        'Close the window automatically
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType, "CloseWindowJS", "setTimeout('window.close();',5000);", True)
                    End If

                End If

            Catch ex As Exception
                'TODO: error msg
                LogError(ex.ToString)
                Response.Write("error")
            End Try
        End Sub
#End Region

#Region " Page Methods "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        Public Shared Sub DeletePage(ByVal ID As String)
            _docPageGuid = New DocumentPageGuid(ID)
            Dim result As IAsyncResult = _docPageGuid.BeginGetData(Nothing, Nothing)
            _docPageGuid.EndGetData(result)

            _docPage = New DocumentPage(_docPageGuid.Data)
            _docPage.BeginDelete(Nothing, Nothing)
        End Sub
#End Region


#Region " Dispose "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Dispose()
            If _docGuid IsNot Nothing Then _docGuid.Dispose()
            If _doc IsNot Nothing Then _doc.Dispose()
            If _docPage IsNot Nothing Then _docPage.Dispose()
            If _docPageGuid IsNot Nothing Then _docPageGuid.Dispose()
            MyBase.Dispose()
        End Sub
#End Region

    End Class

End Namespace