Option Strict On

Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports SRXLite.Classes
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Services

    <WebService(Namespace:="http://tempuri.org/srxlite/batchservice")> _
    <WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Public Class BatchService
        Inherits System.Web.Services.WebService

        Public _login As ServiceLogin
        Private _user As ServiceUser
        Private _batch As Batch
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

#Region " GetBatchUploadUrl "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="batchType"></param>
        ''' <param name="entityName"></param>
        ''' <param name="docTypeID"></param>
        ''' <param name="location"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetBatchUploadUrl( _
     ByVal batchType As BatchType, _
     ByVal location As String, _
     ByVal entityName As String, _
     ByVal docTypeID As Integer, _
     ByVal stylesheetUrl As String, _
     ByVal entityDisplayText As String, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _batch = New Batch(_user)
                Return _batch.BeginGetUploadUrl(batchType, location, entityName, docTypeID, stylesheetUrl, entityDisplayText, callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetBatchUploadUrl", True)
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
        Public Function EndGetBatchUploadUrl(ByVal result As IAsyncResult) As String
            Try
                _user.Authenticate(_login)
                Return _batch.EndGetUploadUrl(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetBatchUploadUrl", True)
                Throw soapEx
            End Try
        End Function
#End Region

#Region " GetEntityBatchList "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="entityName"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <WebMethod()> _
        <SoapHeader("_login")> _
        Public Function BeginGetEntityBatchList( _
     ByVal entityName As String, _
     ByVal callback As AsyncCallback, _
     ByVal stateObject As Object) As IAsyncResult

            Try
                _user.Authenticate(_login)
                _entity = New Entity(_user, entityName)
                Return _entity.BeginGetBatchList(callback, stateObject)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "BeginGetEntityBatchList", True)
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
        Public Function EndGetEntityBatchList(ByVal result As IAsyncResult) As List(Of BatchData)
            Try
                _user.Authenticate(_login)
                Return _entity.EndGetBatchList(result)

            Catch accessEx As UnauthorizedAccessException
                LogError(accessEx.ToString)
                Throw accessEx
            Catch ex As Exception
                Dim soapEx As SoapException = CreateSoapException(ex.Message, ex.ToString, _user, "EndGetEntityBatchList", True)
                Throw soapEx
            End Try
        End Function
#End Region


#Region " Dispose "
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If _batch IsNot Nothing Then _batch.Dispose()
            If _entity IsNot Nothing Then _entity.Dispose()
            MyBase.Dispose(disposing)
        End Sub
#End Region

    End Class

End Namespace