Option Strict On

Imports System.Text
Imports System.Web.Services.Protocols
Imports System.Xml
Imports System.Data.SqlClient
Imports SRXLite.DataAccess

Namespace Modules

    Public Module ExceptionHandling

#Region " CreateSoapException "

        ''' <summary>
        ''' Creates a SoapException with error details.
        ''' </summary>
        ''' <param name="message">Error message.</param>
        ''' <param name="detailMessage">Error message details.</param>
        ''' <param name="source">Source method of the error.</param>
        ''' <param name="writeToErrorLog">Write the error to log file/table.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateSoapException(ByVal message As String, ByVal detailMessage As String, ByVal user As SRXLite.Classes.User, ByVal source As String, ByVal writeToErrorLog As Boolean) As SoapException
            Dim errorID As Integer = -1
            If writeToErrorLog Then
                errorID = LogError(detailMessage, user)
            End If

            'Create nodes
            Dim xmlDoc As New XmlDocument()
            Dim rootNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace)
            Dim errorNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Error", "")
            Dim idNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorID", "")
            Dim messageNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Message", "")
            Dim sourceNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Source", "")

            'Child nodes with error info
            idNode.InnerText = errorID.ToString
            messageNode.InnerText = IIf(Environment.IsDev(), message, "").ToString
            sourceNode.InnerText = source

            'Append the child nodes to the error node
            errorNode.AppendChild(idNode)
            errorNode.AppendChild(messageNode)
            errorNode.AppendChild(sourceNode)

            'Append the error node to the detail node
            rootNode.AppendChild(errorNode)

            'Create new soap exception with error details
            Dim faultCode As XmlQualifiedName = SoapException.ServerFaultCode 'TODO: return different codes based on the error?
            Dim soapEx As New SoapException("SRX Lite Web Service Error", faultCode, source, rootNode)
            Return soapEx
        End Function

#End Region

#Region " FormatLastError "

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatLastError() As String
            Return FormatLastError(HttpContext.Current)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="context"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatLastError(ByVal context As HttpContext) As String
            Dim errorDetails As New StringBuilder

            If context IsNot Nothing Then
                Dim exceptionMessage As String = String.Empty

                ' Sometimes the InnerException is NULL; therefore, we need to check for this before calling the ToString() method...
                If (context.Server.GetLastError().InnerException IsNot Nothing) Then
                    exceptionMessage = context.Server.GetLastError().InnerException.ToString()
                Else
                    exceptionMessage = context.Server.GetLastError().ToString()
                End If

                errorDetails.Append(exceptionMessage)
                errorDetails.Append(vbCrLf & "--------------------------------------------------")
                errorDetails.Append(vbCrLf & "URL=" & context.Request.Url.ToString)
                errorDetails.Append(vbCrLf & "REFERER=" & NullCheck(context.Request.UrlReferrer))
                errorDetails.Append(vbCrLf & "BROWSER=" & NullCheck(context.Request.Browser.Browser))

            End If

            Return errorDetails.ToString
        End Function

#End Region

#Region " GetErrorMsg "

        ''' <summary>
        ''' Returns the error message for an error ID.
        ''' </summary>
        ''' <param name="errorID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetErrorMsg(ByVal errorID As Integer) As String
            Return NullCheck(DB.ExecuteScalar("dsp_ErrorLog_GetErrorMsg " & errorID))
        End Function

#End Region

#Region " GetLastErrorID "

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorID() As Integer
            Dim context As HttpContext = HttpContext.Current
            Dim errorID As Integer = -1
            If context.Server.GetLastError IsNot Nothing Then
                errorID = IntCheck(context.Server.GetLastError.Data("ErrorID"))
            End If
            Return errorID
        End Function

#End Region

#Region " LogError "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="errorMsg"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LogError(ByVal errorMsg As String) As Integer
            Return LogError(errorMsg, 0, 0)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="errorMsg"></param>
        ''' <param name="user"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LogError(ByVal errorMsg As String, ByVal user As SRXLite.Classes.User) As Integer
            Return LogError(errorMsg, user.UserID, user.SubuserID)
        End Function

        ''' <summary>
        ''' Inserts a record into the ErrorLog table.
        ''' </summary>
        ''' <param name="errorMsg"></param>
        ''' <param name="userID"></param>
        ''' <param name="subuserID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LogError(ByVal errorMsg As String, ByVal userID As Short, ByVal subuserID As Integer) As Integer
            Try
                Dim command As New SqlCommand
                command.CommandText = "dsp_ErrorLog_Insert"
                command.CommandType = CommandType.StoredProcedure
                With command.Parameters
                    .Add(getSqlParameter("@ErrorMsg", errorMsg))
                    .Add(getSqlParameter("@UserID", userID))
                    .Add(getSqlParameter("@SubuserID", subuserID))
                End With

                'TODO: write to log file?

                Return IntCheck(DB.ExecuteScalar(command)) 'Return error ID
            Catch
                'TODO:
                Return -1
            End Try
        End Function

#End Region

    End Module

End Namespace