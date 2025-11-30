Imports ALOD.Core.Interfaces
Imports WebSupergoo.ABCpdf8
Imports WebSupergoo.ABCpdf8.Objects

Namespace Printing.FormFieldParsers

    Public Class AdobeLiveCycleHierarchicalParser
        Implements IPDFFormFieldParser

        Public Function ParseField(ByVal document As Doc, ByVal desiredField As String) As Field Implements IPDFFormFieldParser.ParseField
            Dim fieldNames As String() = document.Form.GetFieldNames()
            Dim currentField As Field = Nothing

            For Each fieldName As String In fieldNames
                currentField = document.Form(fieldName)

                If (ExtractFieldName(currentField).Equals(desiredField)) Then
                    Return currentField
                End If
            Next

            Return Nothing
        End Function

        Protected Function ExtractFieldName(ByVal fieldObj As Field) As String
            ' Adobe LiveCycle field name format: form#[#].Page#[#].fieldName[#]
            ' Field.PartialName property grabs the fieldName[#] portion for the field name
            Return fieldObj.PartialName.Substring(0, fieldObj.PartialName.Length - 3)
        End Function

    End Class

End Namespace