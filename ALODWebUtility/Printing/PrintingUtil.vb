Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Namespace PrintingUtil
    Public Module PrintingUtil

        Public Function CheckForPriorStatus(ByVal status As Short, ByVal refId As Integer) As Boolean
            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (status = LodStatusCode.Complete) Then
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(lod.Id, ModuleType.LOD)

                If (ws IsNot Nothing) Then
                    If (ws.Count > 1) Then
                        If (ws(1).WorkflowStatus.StatusCodeType.Id = LodStatusCode.AppointingAutorityReview) Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            End If

            Return False
        End Function

        Public Sub ClearFormField(ByVal doc As PDFForm, ByVal field As String)
            SetFormField(doc, field, String.Empty)
        End Sub

        Public Function GetFindingFormText(ByVal findingValue As Finding) As String
            Select Case findingValue
                Case Finding.In_Line_Of_Duty
                    Return "Line of Duty"
                Case Finding.Epts_Lod_Not_Applicable
                    Return "EPTS-LOD Not Applicable"
                Case Finding.Nlod_Due_To_Own_Misconduct
                    Return "Not ILOD-Due to Own Misconduct"
                Case Finding.Epts_Service_Aggravated
                    Return "EPTS-Service Aggravated"
                Case Finding.Nlod_Not_Due_To_OwnMisconduct
                    Return "Not ILOD-Not Due to Own Misconduct"
                Case Finding.Recommend_Formal_Investigation
                    Return "Formal Investigation"
                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function IsValidSignature(ByVal signature As SignatureEntry) As Boolean
            If (signature Is Nothing) Then
                Return False
            End If

            Return signature.IsSigned
        End Function

        Public Function RemoveNewLinesFromString(ByVal data As String) As String
            If (String.IsNullOrEmpty(data)) Then
                Return String.Empty
            End If

            Dim replacedString As String = data
            Dim replaceFinished As Boolean = False
            Dim len As Integer

            While (Not replaceFinished)
                len = replacedString.Length()
                replacedString = replacedString.Replace(Environment.NewLine, " ")

                If (len = replacedString.Length()) Then
                    replaceFinished = True
                End If
            End While

            Return replacedString
        End Function

        Public Sub SetCheckboxField(ByVal doc As PDFForm, ByVal field As String, ByVal value As Boolean?)
            If (Not value.HasValue OrElse value.Value = False) Then
                Exit Sub
            End If

            SetFormField(doc, field, "1")
        End Sub

        Public Sub SetDateTimeField(ByVal doc As PDFForm, ByVal field As String, ByVal value As DateTime?, Optional ByVal format As String = "ddMMMyyyy", Optional ByVal toUpper As Boolean = False)
            If (Not value.HasValue) Then
                Exit Sub
            End If

            If (toUpper) Then
                SetFormField(doc, field, value.Value.ToString(format).ToUpper())
            Else
                SetFormField(doc, field, value.Value.ToString(format))
            End If
        End Sub

        Public Sub SetFormField(ByVal doc As PDFForm, ByVal field As String, ByVal value As String)
            If (Not String.IsNullOrEmpty(field)) Then
                Try
                    doc.SetField(field, value)
                Catch ex As Exception

                End Try
            End If
        End Sub

        Function SplitString(ByVal chopString As String, ByVal length As Integer) As String()

            Dim top As String, bottom As String, temp(1) As String
            Dim dif As Integer = 0

            If (String.IsNullOrEmpty(chopString) OrElse chopString.Length = 0) Then
                temp(0) = ""
                temp(1) = ""
            Else
                If chopString.Length > length Then
                    top = chopString.Substring(0, length)
                    temp = Split(top, " ")
                    dif = temp(temp.GetUpperBound(0)).Length

                    top = chopString.Substring(0, length - dif)
                    bottom = chopString.Substring(top.Length, chopString.Length - top.Length)

                    temp(0) = top
                    temp(1) = bottom
                Else
                    temp(0) = chopString
                    temp(1) = String.Empty

                End If
            End If

            Return temp

        End Function

    End Module
End Namespace