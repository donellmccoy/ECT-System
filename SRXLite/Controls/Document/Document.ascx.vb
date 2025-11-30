Option Strict On

Imports System.Web.Services
Imports SRXLite.Classes
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Controls.Document

    Partial Class Controls_Document
        Inherits System.Web.UI.UserControl

        Private _doc As SRXLite.Classes.Document
        Private Shared _docPage As DocumentPage
        Private Shared _docPageGuid As DocumentPageGuid

        Private _docID As Long
        Private _user As User
        Private _readOnly As Boolean = False
        Private _scriptManagerID As String
        Private _mode As DocumentMode

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                'Add JS reference
                'ScriptManager.RegisterClientScriptInclude(Page, Page.GetType, "Util.js", Request.ApplicationPath & "/Includes/Util.js")
                'ScriptManager.RegisterClientScriptInclude(Page, Page.GetType, "Document.js", Me.TemplateSourceDirectory & "/Document.js")
                'ScriptManagerProxy1.Scripts.Add(New ScriptReference("~/Includes/Util.js"))
                'ScriptManagerProxy1.Scripts.Add(New ScriptReference(Me.TemplateSourceDirectory & "/Document.js"))

                'Add stylesheet link
                Dim cssLink As New HtmlLink()
                cssLink.Href = Me.TemplateSourceDirectory() & "/Document.css"
                cssLink.Attributes.Add("type", "text/css")
                cssLink.Attributes.Add("rel", "stylesheet")
                Page.Header.Controls.Add(cssLink)
            End If

        End Sub

        Public Enum DocumentMode
            Batch
            Normal
            Thumbnail
        End Enum

#Region " Properties "
        Public Property DocID() As Long
            Get
                Return _docID
            End Get
            Set(ByVal value As Long)
                _docID = value
            End Set
        End Property

        Public Property User() As User
            Get
                Return _user
            End Get
            Set(ByVal value As User)
                _user = value
            End Set
        End Property

        Public Property IsReadOnly() As Boolean
            Get
                Return _readOnly
            End Get
            Set(ByVal value As Boolean)
                _readOnly = value
            End Set
        End Property

        Public WriteOnly Property ScriptManagerID() As String
            Set(ByVal value As String)
                _scriptManagerID = value
            End Set
        End Property

        Public Property Mode() As DocumentMode
            Get
                Return _mode
            End Get
            Set(ByVal value As DocumentMode)
                _mode = value
            End Set
        End Property

        Public Property Width() As String
            Get
                Return divContent.Style("width")
            End Get
            Set(ByVal value As String)
                If divContent.Style("width") Is Nothing Then divContent.Style.Add("width", "")
                divContent.Style("width") = value
            End Set
        End Property

        Public Property BackgroundColor() As String
            Get
                Return divContent.Style("background-color")
            End Get
            Set(ByVal value As String)
                If divContent.Style("background-color") Is Nothing Then divContent.Style.Add("background-color", "")
                divContent.Style("background-color") = value
            End Set
        End Property
#End Region

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

#Region " BindDocument "
        Public Sub BindDocument()
            _doc = New SRXLite.Classes.Document(Me.User, Me.DocID)

            Dim task As New PageAsyncTask( _
            New BeginEventHandler(AddressOf BeginGetDocumentPageList), _
            New EndEventHandler(AddressOf EndGetDocumentPageList), _
            New EndEventHandler(AddressOf AsyncTaskTimeout), _
            Nothing)

            Page.RegisterAsyncTask(task)
        End Sub
#End Region

#Region " GetDocumentPageList (Begin/End)"
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
                    menuDelete.Visible = (Not Me.IsReadOnly) AndAlso (list.Count > 1)
                    CType(Page.FindControl(_scriptManagerID), ScriptManager).EnablePageMethods = Not Me.IsReadOnly
                    divContent.Attributes.Add("onkeydown", "CheckHotKeys(event);")
                    divContent.Attributes.Add("mode", Me.Mode.ToString)

                    For i As Integer = 0 To list.Count - 1
                        data = list(i)
                        Using img As New HtmlImage
                            'img.ID = "p" & i
                            img.Alt = ""
                            With img.Attributes
                                .Add("parentid", Me.ClientID)
                                .Add("class", "page")
                                .Add("xsrc", data.PageUrl & "&c=0&r=&h=&w=")    'Set src and width in JS to fit window
                                .Add("onclick", "onClickImg(event,this);")
                                .Add("oncontextmenu", "return onContextMenuImg(event,this);")
                            End With

                            divContent.Controls.Add(img)
                        End Using
                    Next

                Else
                    data = list(0)

                    Using iframe As New HtmlGenericControl
                        iframe.TagName = "iframe"
                        iframe.ID = "ifrDocument"
                        iframe.Attributes.Add("src", data.PageUrl)
                        iframe.Attributes.Add("style", "width:100%; height:100%")
                        divContent.Style.Add("height", "100%")
                        divContent.Controls.Add(iframe)
                    End Using

                    Menu.Visible = False 'Image context menu not needed

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
            If _doc IsNot Nothing Then _doc.Dispose()
            If _docPage IsNot Nothing Then _docPage.Dispose()
            If _docPageGuid IsNot Nothing Then _docPageGuid.Dispose()
            MyBase.Dispose()
        End Sub
#End Region

    End Class

End Namespace