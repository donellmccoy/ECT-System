Imports ALOD.Core.Interfaces
Imports WebSupergoo.ABCpdf8
Imports WebSupergoo.ABCpdf8.Objects

Namespace Printing.FormFieldParsers

    Public Class NonHierarchicalParser
        Implements IPDFFormFieldParser

        Public Function ParseField(ByVal document As Doc, ByVal desiredField As String) As Field Implements IPDFFormFieldParser.ParseField
            Dim fieldNames As String() = document.Form.GetFieldNames()
            Dim fullFieldName As String = fieldNames.First(Function(x) x.Equals(desiredField.Trim()))
            Return document.Form(fullFieldName)
        End Function

    End Class

End Namespace