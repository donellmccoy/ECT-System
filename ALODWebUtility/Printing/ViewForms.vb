Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data.Services

Namespace Printing
    Public Module ViewForms

        Public Function LinkAttribute261(ByVal refID As String, ByVal _documents As IList(Of Document)) As String
            Dim strAttribute261 As String = ""
            Dim form261ID As String = "0"

            Dim isDoc As Boolean = False

            If (_documents IsNot Nothing) Then
                For Each docItem In _documents
                    If (docItem.DocType.ToString() = "FinalForm261") Then
                        form261ID = docItem.Id.ToString()
                        isDoc = True
                    End If
                Next
            End If

            If (isDoc) Then
                Dim url261 As String = "../Shared/DocumentViewer.aspx?docID=" & form261ID & "&modId=" & ModuleType.LOD
                strAttribute261 = "viewDoc('" & url261 & "'); return false;"
            Else
                ' handles legacy data for now. See file Search.aspx.vb line 213 for comments
                strAttribute261 = "printForm(" & refID & ",'261');"
            End If

            Return strAttribute261
        End Function

        Public Function LinkAttribute348(ByVal refID As String, ByVal _documents As IList(Of Document), ByVal casetype As String) As String
            Dim strAttribute348 As String = ""
            Dim form348ID As String = "0"
            Dim isDoc As Boolean = False

            Dim lineOfDuty As LineOfDuty = Nothing

            lineOfDuty = LodService.GetById(refID)

            If (lineOfDuty IsNot Nothing AndAlso _documents IsNot Nothing) Then
                ' fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                Dim fileSubString As String = lineOfDuty.CaseId & "-Generated"

                For Each docItem In _documents
                    If (docItem.DocType.ToString() = "FinalForm348" AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                        form348ID = docItem.Id.ToString()
                        isDoc = True
                    End If
                Next
            End If

            If (isDoc) Then
                Dim url348 As String = "../Shared/DocumentViewer.aspx?docID=" & form348ID & "&modId=" & ModuleType.LOD
                strAttribute348 = "viewDoc('" & url348 & "'); return false;"
            Else
                ' handles legacy data for now.
                If (lineOfDuty.Formal = True) Then
                    strAttribute348 = "printForm('" & casetype & "', " & refID & ");"
                Else
                    strAttribute348 = "printForm('" & casetype & "', " & refID & ",'348');"
                End If
            End If

            Return strAttribute348
        End Function

        Public Function PHFormPDFLinkAttribute(ByVal specCase As SpecialCase, ByVal documents As IList(Of Document)) As String
            Dim strAttribute As String = ""
            Dim phFormDocId As String = "0"
            Dim isDoc As Boolean = False

            If (specCase Is Nothing) Then
                Return "return false;"
            End If

            If (documents IsNot Nothing) Then
                ' fileSubString is used to get the correct PH form document if multiple PH forms are associated with the case's group Id.
                ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                Dim fileSubString As String = specCase.CaseId & "-Generated"

                For Each docItem In documents
                    If (docItem.DocType = DocumentType.PHForm AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                        phFormDocId = docItem.Id.ToString()
                        isDoc = True
                    End If
                Next
            End If

            If (isDoc) Then
                Dim docViewerURL As String = "../Shared/DocumentViewer.aspx?docID=" & phFormDocId & "&modId=" & ModuleType.SpecCasePH
                strAttribute = "viewDoc('" & docViewerURL & "'); return false;"
            Else
                strAttribute = "printForms('" & specCase.Id.ToString() & "', 'SC_PH');"
            End If

            Return strAttribute
        End Function

        Public Function RestrictedSARCFormPDFLinkAttribute(ByVal sarc As RestrictedSARC, ByVal documents As IList(Of Document)) As String
            Dim strAttribute As String = ""
            Dim sarcFormDocId As String = "0"
            Dim isDoc As Boolean = False

            If (sarc Is Nothing) Then
                Return "return false;"
            End If

            If (documents IsNot Nothing) Then
                ' fileSubString is used to get the correct SARC document if multiple SARC documents are associated with the case's group Id.
                ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                Dim fileSubString As String = sarc.CaseId & "-Generated"

                For Each docItem In documents
                    If (docItem.DocType = DocumentType.Form348R AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                        sarcFormDocId = docItem.Id.ToString()
                        isDoc = True
                    End If
                Next
            End If

            If (isDoc) Then
                Dim docViewerURL As String = "../Shared/DocumentViewer.aspx?docID=" & sarcFormDocId & "&modId=" & ModuleType.SARC
                strAttribute = "viewDoc('" & docViewerURL & "'); return false;"
            Else
                strAttribute = "printForms('" & sarc.Id.ToString() & "', 'SARC'); return false;"
            End If

            Return strAttribute
        End Function

    End Module
End Namespace