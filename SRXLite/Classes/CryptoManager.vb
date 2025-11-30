Option Strict On

Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Caching
Imports System.Data.SqlClient
Imports System.IO
Imports SRXLite.Modules

Namespace Classes

    Public Class CryptoManager

        Private _context As HttpContext
        Private _iv(15) As Byte
        Private _key(23) As Byte

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _context = HttpContext.Current
            GenerateKey(EncryptionKey())
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal context As HttpContext)
            _context = context
            GenerateKey(EncryptionKey())
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="passPhrase"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal passPhrase As String)
            _context = HttpContext.Current
            GenerateKey(passPhrase)
        End Sub

#End Region

#Region " GenerateKey "

        ''' <summary>
        ''' Generates the encryption key and initialization vector.
        ''' </summary>
        ''' <param name="passPhrase"></param>
        ''' <remarks></remarks>
        Private Sub GenerateKey(ByVal passPhrase As String)
            If _context.Cache.Item("Encryption_Key") Is Nothing OrElse
             _context.Cache.Item("Encryption_IV") Is Nothing Then

                Dim phraseAsBytes() As Byte = ASCIIEncoding.ASCII.GetBytes(passPhrase)
                Dim sha384 As New SHA384CryptoServiceProvider
                sha384.ComputeHash(phraseAsBytes)
                Dim result() As Byte = sha384.Hash
                sha384.Clear()

                For i As Integer = 0 To 23 : _key(i) = result(i) : Next '192-bit key
                For i As Integer = 24 To 39 : _iv(i - 24) = result(i) : Next    '64-bit initialization vector

                'Reuse these values since they are accessed frequently by all users
                _context.Cache.Insert("Encryption_Key", _key, Nothing, Cache.NoAbsoluteExpiration, TimeSpan.Zero)
                _context.Cache.Insert("Encryption_IV", _iv, Nothing, Cache.NoAbsoluteExpiration, TimeSpan.Zero)
            Else
                'Get values from cache
                _key = CType(_context.Cache.Item("Encryption_Key"), Byte())
                _iv = CType(_context.Cache.Item("Encryption_IV"), Byte())
            End If
        End Sub

#End Region

#Region " EncryptToBase64String "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="stringToEncrypt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function EncryptToBase64String(ByVal stringToEncrypt As String) As String
            Dim rijndael As New AesCryptoServiceProvider
            Dim inputAsBytes() As Byte = ASCIIEncoding.ASCII.GetBytes(stringToEncrypt)
            Dim encryptedString As String

            Try
                rijndael.KeySize = 192
                rijndael.Key = _key
                rijndael.IV = _iv

                Using rijndaelTransform As ICryptoTransform = rijndael.CreateEncryptor()
                    Using ms As New MemoryStream()
                        Using cs As New CryptoStream(ms, rijndaelTransform, CryptoStreamMode.Write)
                            cs.Write(inputAsBytes, 0, inputAsBytes.Length)
                            cs.FlushFinalBlock()
                            encryptedString = Convert.ToBase64String(ms.ToArray())
                        End Using 'cs
                    End Using 'ms
                End Using 'rijndaelTransform

                'Make string safe for URL
                encryptedString = encryptedString.Replace("+", "!")
                encryptedString = encryptedString.Replace("/", "-")
                encryptedString = encryptedString.Replace("=", "_")

                Return encryptedString
            Finally
                If rijndael IsNot Nothing Then rijndael.Clear()
                If inputAsBytes IsNot Nothing Then Erase inputAsBytes
            End Try
        End Function

#End Region

#Region " DecryptFromBase64String "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="stringToDecrypt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function DecryptFromBase64String(ByVal stringToDecrypt As String) As String
            If stringToDecrypt.Length = 0 Then Return ""

            'Replace original characters
            stringToDecrypt = stringToDecrypt.Replace("!", "+")
            stringToDecrypt = stringToDecrypt.Replace("-", "/")
            stringToDecrypt = stringToDecrypt.Replace("_", "=")

            Dim rijndael As New AesCryptoServiceProvider
            Dim inputAsBytes() As Byte = Convert.FromBase64String(stringToDecrypt)

            Try
                rijndael.KeySize = 192
                rijndael.Key = _key
                rijndael.IV = _iv

                Using rijndaelTransform As ICryptoTransform = rijndael.CreateDecryptor()
                    Using ms As New MemoryStream()
                        Using cs As New CryptoStream(ms, rijndaelTransform, CryptoStreamMode.Write)
                            cs.Write(inputAsBytes, 0, inputAsBytes.Length)
                            cs.FlushFinalBlock()
                            Return Encoding.UTF8.GetString(ms.ToArray())
                        End Using 'cs
                    End Using 'ms
                End Using 'rijndaelTransform
            Finally
                If rijndael IsNot Nothing Then rijndael.Clear()
                If inputAsBytes IsNot Nothing Then Erase inputAsBytes
            End Try
        End Function

#End Region

#Region " Encrypt "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Encrypt(ByVal value As String) As String
            If Not IsEncryptionEnabled() Then Return value
            Return EncryptToBase64String(value)
        End Function

#End Region

#Region " Decrypt "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Decrypt(ByVal value As String) As String
            If Not IsEncryptionEnabled() Then Return value
            Return DecryptFromBase64String(value)
        End Function

#End Region

#Region " EncryptForUrl "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EncryptForUrl(ByVal value As String) As String
            If Not IsEncryptionEnabled() Then Return _context.Server.UrlEncode(value)
            Return EncryptToBase64String(value)
        End Function

#End Region

#Region " DecryptNameValuePairs "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="itemDelimeter"></param>
        ''' <param name="valueDelimeter"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DecryptNameValuePairs(
         ByVal text As String,
         Optional ByVal itemDelimeter As Char = "&"c,
         Optional ByVal valueDelimeter As Char = "="c) As Collection

            Dim Data As New Collection
            Dim Items() As String = Decrypt(text).Split(itemDelimeter)
            Dim NameValue() As String
            For i As Integer = 0 To (Items.Length - 1)
                NameValue = Items(i).Split(valueDelimeter)
                If NameValue.Length > 0 Then
                    Data.Add(NameValue(1), NameValue(0))
                End If
            Next

            Return Data
        End Function

#End Region

    End Class

End Namespace