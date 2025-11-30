Imports ALOD.Core.Interfaces

Namespace Printing.FormFieldParsers
    Public Module FormFieldParserFactory

        Public Function GetAdobeLiveCycleHierarchicalParser() As IPDFFormFieldParser
            Return New AdobeLiveCycleHierarchicalParser()
        End Function

        Public Function GetNonHierarchicalParser() As IPDFFormFieldParser
            Return New NonHierarchicalParser()
        End Function

        Public Function GetParserByName(ByVal name As String) As IPDFFormFieldParser
            Select Case name
                Case "NonHierarchicalParser"
                    Return GetNonHierarchicalParser()

                Case "AdobeLiveCycleHierachicalParser"
                    Return GetAdobeLiveCycleHierarchicalParser()

                Case Else
                    Return GetNonHierarchicalParser()
            End Select
        End Function

    End Module
End Namespace