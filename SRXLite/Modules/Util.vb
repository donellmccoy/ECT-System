Option Strict On

Imports System.Drawing.Imaging
Imports SRXLite.DataAccess

Namespace Modules

    Public Module Util

#Region " GetURL "

        ''' <summary>
        ''' Returns the full URL of the website
        ''' </summary>
        ''' <param name="RelativePath"></param>
        ''' <param name="Query"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetURL(Optional ByVal relativePath As String = "", Optional ByVal query As String = "") As String
            Return GetURL(HttpContext.Current, relativePath, query)
        End Function

        ''' <summary>
        ''' Returns the full URL of the website
        ''' </summary>
        ''' <param name="RelativePath"></param>
        ''' <param name="Query"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetURL(ByVal context As HttpContext, Optional ByVal relativePath As String = "", Optional ByVal query As String = "") As String
            Dim currentUri As Uri = context.Request.Url
            Dim uriBuild As New UriBuilder
            With uriBuild
                .Scheme = currentUri.Scheme
                .Host = currentUri.Host
                .Port = currentUri.Port
                .Path = context.Request.ApplicationPath & relativePath
                .Query = query
            End With

            Return uriBuild.Uri.AbsoluteUri
        End Function

#End Region

#Region " BoolCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="defaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BoolCheck(ByVal value As Object, Optional ByVal defaultValue As Boolean = False) As Boolean
            If IsDBNull(value) OrElse IsNothing(value) OrElse value.ToString = "" Then Return defaultValue
            Return CBool(value)
        End Function

#End Region

#Region " ByteCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="DefaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ByteCheck(ByVal value As Object, Optional ByVal defaultValue As Byte = 0) As Byte
            If Not IsNumeric(value) Then Return defaultValue
            Return CByte(value)
        End Function

#End Region

#Region " DateCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="defaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DateCheck(ByVal value As Object, ByVal defaultValue As Date) As Date
            If Not IsDate(value) Then Return defaultValue
            Return CDate(value)
        End Function

#End Region

#Region " IntCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="DefaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IntCheck(ByVal value As Object, Optional ByVal defaultValue As Integer = 0) As Integer
            If Not IsNumeric(value) Then Return defaultValue
            Return CInt(value)
        End Function

#End Region

#Region " LngCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="DefaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LngCheck(ByVal value As Object, Optional ByVal defaultValue As Long = 0) As Long
            If Not IsNumeric(value) Then Return defaultValue
            Return CLng(value)
        End Function

#End Region

#Region " NullCheck "

        ''' <summary>
        ''' Handles null string
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="defaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NullCheck(ByVal value As Object, Optional ByVal defaultValue As String = "") As String
            If IsDBNull(value) OrElse IsNothing(value) Then Return defaultValue
            Return value.ToString.Trim
        End Function

#End Region

#Region " ShortCheck "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="DefaultValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ShortCheck(ByVal value As Object, Optional ByVal defaultValue As Short = 0) As Short
            If Not IsNumeric(value) Then Return defaultValue
            Return CShort(value)
        End Function

#End Region

#Region " FormatFileExt "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatFileExt(ByVal fileName As String) As String
            Return fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower
        End Function

#End Region

#Region " FormatFileName "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatFileName(ByVal fileName As String) As String
            If fileName.IndexOf("\") = -1 Then Return fileName
            Return fileName.Substring(fileName.LastIndexOf("\") + 1)
        End Function

#End Region

#Region " GetCachedFileExtTable "

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCachedFileExtTable() As DataTable
            Return GetCachedFileExtTable(HttpContext.Current)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="context"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCachedFileExtTable(ByVal context As HttpContext) As DataTable
            Dim dt As DataTable = CType(context.Cache("LkupFileExtension"), DataTable)
            If dt Is Nothing Then
                dt = DB.ExecuteDataset("exec dsp_GetFileExtensions").Tables(0)
                context.Cache.Insert("LkupFileExtension", dt, Nothing, Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1))
            End If
            Return dt
        End Function

#End Region

#Region " IsFileExtValid "

        Public Function IsFileExtValid(ByVal FileExt As String) As Boolean
            If FileExt.IndexOf(".") > -1 Then FileExt = FileExt.Substring(FileExt.LastIndexOf(".") + 1)
            Dim dt As DataTable = GetCachedFileExtTable()
            Dim dr() As DataRow = dt.Select("FileExt='" & FileExt & "'")
            Return dr.Length > 0
        End Function

#End Region

#Region " IsFileSizeValid "

        Public Function IsFileSizeValid(ByVal length As Integer) As Boolean
            Return length <= FileSizeUploadLimit()
        End Function

        Public Function IsInitialFileSizeValid(ByVal length As Integer) As Boolean
            Return length <= InitialFileSizeUploadLimit()
        End Function

#End Region

#Region " GetFileSizeMB "

        Public Function GetFileSizeMB(ByVal fileSizeBytes As Integer) As Double
            Return Math.Round(fileSizeBytes / 1048576, 2)
        End Function

#End Region

#Region " GetEncoderInfo "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="mimeType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEncoderInfo(ByVal mimeType As String) As ImageCodecInfo
            For Each codecinfo As ImageCodecInfo In ImageCodecInfo.GetImageEncoders()
                If codecinfo.MimeType = mimeType Then
                    Return codecinfo
                End If
            Next
            Return Nothing
        End Function

#End Region

#Region " GetClientIP "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="context"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetClientIP(Optional ByVal context As HttpContext = Nothing) As String
            If context Is Nothing Then
                If HttpContext.Current IsNot Nothing Then
                    context = HttpContext.Current
                Else
                    Return ""
                End If
            End If

            Dim ipAddress As String = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If ipAddress IsNot Nothing Then
                'Get last ip address in comma-delimited list
                ipAddress = ipAddress.Substring(ipAddress.LastIndexOf(",") + 1).Trim
            Else
                ipAddress = context.Request.ServerVariables("REMOTE_ADDR")
            End If

            Return ipAddress
        End Function

#End Region

    End Module

End Namespace