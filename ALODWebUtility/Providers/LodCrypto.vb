Imports System.Security.Cryptography
Imports System.Text

Namespace Providers

    Public Class LodCrypto

        Private Shared key As String = "SD23F@#^LSKDJsfwjdad1211197dKASJ2}{#H@#I"

        Private Shared ReadOnly Property filling() As String
            Get
                Dim ticks As String = DateTime.Now.Ticks.ToString
                Dim len As String = ticks.Length
                Return ticks.ToString.Substring(len - 6, 5)
            End Get
        End Property

        ''' <summary>
        ''' Decrypt as string
        ''' </summary>
        ''' <param name="encrypted"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Decrypt(ByVal encrypted As String) As String

            Dim decrypted As String = Nothing
            Dim inputBytes As Byte() = Nothing

            Try
                inputBytes = Convert.FromBase64String(encrypted)

                Dim hashmd5 As New MD5CryptoServiceProvider
                Dim tdesProvider As New TripleDESCryptoServiceProvider
                tdesProvider.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key))
                tdesProvider.Mode = CipherMode.ECB
                tdesProvider.Padding = PaddingMode.PKCS7

                decrypted = ASCIIEncoding.ASCII.GetString(
                    tdesProvider.CreateDecryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length))
            Catch ex As Exception

            End Try

            Dim final As String = decrypted.Substring(0, decrypted.Length - filling.Length)
            Return final

        End Function

        ''' <summary>
        ''' Encrypt a string
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Encrypt(ByVal text As String) As String

            Dim encrypted As String = Nothing

            Try
                Dim inputBytes As Byte() = UTF8Encoding.UTF8.GetBytes(text + filling)
                Dim hashmd5 As New MD5CryptoServiceProvider
                Dim tdesProvider As New TripleDESCryptoServiceProvider
                tdesProvider.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key))
                tdesProvider.Mode = CipherMode.ECB
                tdesProvider.Padding = PaddingMode.PKCS7

                encrypted = Convert.ToBase64String(
                    tdesProvider.CreateEncryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length))
            Catch ex As Exception

            End Try

            Return encrypted

        End Function

    End Class

End Namespace