Option Strict On

Namespace Modules

    Public Module AppSettings

#Region " EncryptionKey "

        Public Function EncryptionKey() As String
            Return ConfigurationManager.AppSettings("EncryptionKey").ToString
        End Function

#End Region

#Region " Environment "

        Public Class Environment

            Public Shared Function IsDemo() As Boolean
                Return (configValue() = "DEMO")
            End Function

            Public Shared Function IsDev() As Boolean
                Return (configValue() = "DEV")
            End Function

            Public Shared Function IsProd() As Boolean
                Return (configValue() = "PROD")
            End Function

            Private Shared Function configValue() As String
                Return ConfigurationManager.AppSettings("Environment").ToUpper()
            End Function

        End Class

#End Region

#Region " FileSizeUploadLimit "

        Public Function FileSizeUploadLimit() As Integer
            Return IntCheck(ConfigurationManager.AppSettings("FileSizeUploadLimit"))
        End Function

        Public Function GetFileSizeUploadLimitMB() As Double
            Return GetFileSizeMB(FileSizeUploadLimit())
        End Function

        Public Function GetInitialFileSizeUploadLimitMB() As Double
            Return GetFileSizeMB(InitialFileSizeUploadLimit())
        End Function

        Public Function InitialFileSizeUploadLimit() As Integer
            Return IntCheck(ConfigurationManager.AppSettings("InitialFileSizeUploadLimit"))
        End Function

#End Region

#Region " IsEncryptionEnabled "

        Public Function IsEncryptionEnabled() As Boolean
            Return BoolCheck(ConfigurationManager.AppSettings("EncryptionEnabled"), True)
        End Function

#End Region

    End Module

End Namespace