Option Strict On

Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports SRXLite.Classes
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Services

    <WebService(Namespace:="http://tempuri.org/srxlite/documentservice")> _
    <WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Public Class DocumentService
        Inherits System.Web.Services.WebService

        Public _login As ServiceLogin
        Private _user As ServiceUser
        Private _doc As Document
        Private _group As Group
        Private _entity As Entity

#Region " Constructor "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _login = New ServiceLogin()
            _user = New ServiceUser()
        End Sub
#End Region

#Region " CreateGroup "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="groupName"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginCreateGroup( _
    ByVal groupName As String, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                'LogError(_login.UserName & " - " & _login.Password)
                _group = New Group(_user)
                Return _group.BeginCreate(groupName, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginCreateGroup", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function EndCreateGroup(ByVal result As IAsyncResult) As Long
            Try
                _user.Authenticate(_login)
                Return _group.EndCreate(result) 'Return groupID

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndCreateGroup", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " DeleteDocument "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginDeleteDocument( _
    ByVal docID As Long, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _doc = New Document(_user, docID)
                Return _doc.BeginDelete(callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginDeleteDocument", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Sub EndDeleteDocument(ByVal result As IAsyncResult)
            Try
                _user.Authenticate(_login)
                _doc.EndDelete(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndDeleteDocument", True)
                Throw soapEx
            End Try
        End Sub
#End Region

#Region " GetEntityDocumentList "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="entityID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetEntityDocumentList( _
     ByVal entityID As String, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _entity = New Entity(_user, entityID)
                Return _entity.BeginGetDocumentList(callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetEntityDocumentList", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function EndGetEntityDocumentList(ByVal result As IAsyncResult) As List(Of DocumentData)
            Try
                _user.Authenticate(_login)
                Return _entity.EndGetDocumentList(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetEntityDocumentList", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " GetGroupDocumentList "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="groupID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetGroupDocumentList( _
     ByVal groupID As Long, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _group = New Group(_user, groupID)
                Return _group.BeginGetDocumentList(callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetGroupDocumentList", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function EndGetGroupDocumentList(ByVal result As IAsyncResult) As List(Of DocumentData)
            Try
                _user.Authenticate(_login)
                Return _group.EndGetDocumentList(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetGroupDocumentList", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " GetDocumentUploadUrl "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="entityName"></param>
        ''' <param name="docTypeID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetDocumentUploadUrl( _
     ByVal entityName As String, _
     ByVal docTypeID As Integer, _
     ByVal groupID As Long, _
     ByVal stylesheetUrl As String, _
     ByVal entityDisplayText As String, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _doc = New Document(_user)
                Return _doc.BeginGetUploadUrl(entityName, docTypeID, groupID, stylesheetUrl, entityDisplayText, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetDocumentUploadUrl", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function EndGetDocumentUploadUrl(ByVal result As IAsyncResult) As String
            Try
                _user.Authenticate(_login)
                Return _doc.EndGetUploadUrl(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetDocumentUploadUrl", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " GetDocumentViewerUrl "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetDocumentViewerUrl( _
     ByVal docID As Long, _
     ByVal isReadOnly As Boolean, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _doc = New Document(_user, docID)
                Return _doc.BeginGetViewerUrl(docID, isReadOnly, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetDocumentViewerUrl", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function EndGetDocumentViewerUrl(ByVal result As IAsyncResult) As String
            Try
                _user.Authenticate(_login)
                Return _doc.EndGetViewerUrl(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetDocumentViewerUrl", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " MoveGroupDocument "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="sourceGroupID"></param>
        ''' <param name="targetGroupID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginMoveGroupDocument( _
    ByVal docID As Long, _
    ByVal sourceGroupID As Long, _
    ByVal targetGroupID As Long, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _group = New Group(_user, sourceGroupID)
                Return _group.BeginMoveDocument(docID, targetGroupID, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginMoveGroupDocument", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Sub EndMoveGroupDocument(ByVal result As IAsyncResult)
            Try
                _user.Authenticate(_login)
                _group.EndMoveDocument(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndMoveGroupDocument", True)
                Throw soapEx
            End Try
        End Sub
#End Region

#Region " UpdateDocumentStatus "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="docStatus"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginUpdateDocumentStatus( _
    ByVal docID As Long, _
    ByVal docStatus As DocumentStatus, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _doc = New Document(_user, docID)
                Return _doc.BeginUpdateStatus(docStatus, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginUpdateDocumentStatus", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Sub EndUpdateDocumentStatus(ByVal result As IAsyncResult)
            Try
                _user.Authenticate(_login)
                _doc.EndUpdateStatus(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndUpdateDocumentStatus", True)
                Throw soapEx
            End Try
        End Sub
#End Region

#Region " UpdateDocumentKeys "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="docKeys"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginUpdateDocumentKeys( _
    ByVal docID As Long, _
    ByVal docKeys As DocumentKeys, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _doc = New Document(_user, docID)
                Return _doc.BeginUpdateKeys(docKeys, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginUpdateDocumentKeys", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Sub EndUpdateDocumentKeys(ByVal result As IAsyncResult)
            Try
                _user.Authenticate(_login)
                _doc.EndUpdateKeys(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndUpdateDocumentKeys", True)
                Throw soapEx
            End Try
        End Sub
#End Region

#Region " UploadDocument "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fileBytes"></param>
        ''' <param name="uploadKeys"></param>
        ''' <param name="groupID"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginUploadDocument( _
    ByVal fileBytes As Byte(), _
    ByVal uploadKeys As UploadKeys, _
    ByVal groupID As Long, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)

                'Validation
                If Not IsFileExtValid(uploadKeys.FileName) Then Throw New Exception("File type not supported.")
                If Not IsFileSizeValid(fileBytes.Length) Then Throw New Exception("File size exceeded the maximum limit of " & GetFileSizeUploadLimitMB() & " MB.")

                _doc = New Document(_user, 0, groupID)
                uploadKeys.InputType = InputType.WebServiceUpload

                Return _doc.BeginUpload(fileBytes, uploadKeys, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginUploadDocument", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod(Description:="")> _
        <SoapHeader("_login")> _
        Public Function EndUploadDocument(ByVal result As IAsyncResult) As Long
            Try
                If _doc.HasErrors Then
                    Throw New Exception(_doc.ErrorMessage)
                End If

                _user.Authenticate(_login)
                _doc.EndUpload(result)
                Return _doc.DocID

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndUploadDocument", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " CopyGroupDocuments "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="oldGroupId"></param>
        ''' <param name="newGroupId"></param>
        ''' <param name="oldDocTypeId"></param>
        ''' <param name="newDocTypeId"></param>		
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginCopyGroupDocuments( _
    ByVal oldGroupId As Long, _
    ByVal newGroupId As Long, _
    ByVal oldDocTypeId As Long, _
    ByVal newDocTypeId As Long, _
    ByVal callback As AsyncCallback, _
    ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                '_group = New Group(_user, docID)
                _group = New Group(_user)
                Return _group.BeginCopyGroupDocuments(oldGroupId, newGroupId, oldDocTypeId, newDocTypeId, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginCopyGroupDocuments", True)
                Throw soapEx
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Sub EndCopyGroupDocuments(ByVal result As IAsyncResult)
            Try
                _user.Authenticate(_login)
                _group.EndCopyGroupDocuments(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndCopyGroupDocuments", True)
                Throw soapEx
            End Try
        End Sub
#End Region

#Region " Dispose "
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If _doc IsNot Nothing Then _doc.Dispose()
            If _group IsNot Nothing Then _group.Dispose()
            If _entity IsNot Nothing Then _entity.Dispose()
            MyBase.Dispose(disposing)
        End Sub
#End Region

    End Class

End Namespace