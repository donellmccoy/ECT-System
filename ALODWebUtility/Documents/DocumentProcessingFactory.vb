Imports ALOD.Core.Interfaces

Namespace Documents

    Public Class DocumentProcessingFactory

        Public Function GetCertificationStampStrategy() As IDocumentProcessingStrategy
            Return New CertificationStampStrategy()
        End Function

        Public Function GetDefaultStrategy() As IDocumentProcessingStrategy
            Return New DefaultProcessingStrategy()
        End Function

        Public Function GetStrategyByType(ByVal type As ProcessingStrategyType) As IDocumentProcessingStrategy
            Select Case type
                Case ProcessingStrategyType._Default
                    Return GetDefaultStrategy()

                Case ProcessingStrategyType.CertificationStamp
                    Return GetCertificationStampStrategy()

                Case Else
                    Return GetDefaultStrategy()
            End Select
        End Function

    End Class

End Namespace