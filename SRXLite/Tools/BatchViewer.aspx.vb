Option Strict On

Imports SRXLite.Classes

Namespace Web.Tools

    Partial Class Tools_BatchViewer
        Inherits System.Web.UI.Page

        Private _docGuid As DocumentGuid

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _docGuid = New DocumentGuid(Request.QueryString("id"))

            Dim task As New PageAsyncTask( _
            New BeginEventHandler(AddressOf BeginGetDocumentGuidData), _
            New EndEventHandler(AddressOf EndGetDocumentGuidData), _
            New EndEventHandler(AddressOf AsyncTaskTimeout), _
            Nothing)

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
                Response.Write("This page has expired.")
                Response.End()

            Else

                Document1.DocID = data.DocID
                Document1.User = New User(data.UserID, data.SubuserID)
                Document1.IsReadOnly = data.IsReadOnly
                Document1.ScriptManagerID = ScriptManager1.ID
                Document1.BindDocument()


                Document2.DocID = data.DocID
                Document2.User = New User(data.UserID, data.SubuserID)
                Document2.IsReadOnly = data.IsReadOnly
                Document2.ScriptManagerID = ScriptManager1.ID
                Document2.BindDocument()

            End If

        End Sub
#End Region

#Region " Dispose "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Dispose()
            If _docGuid IsNot Nothing Then _docGuid.Dispose()
            MyBase.Dispose()
        End Sub
#End Region

    End Class

End Namespace