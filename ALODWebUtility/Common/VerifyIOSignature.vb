Imports System.IO
Imports System.Web

Namespace Common

    Public Class VerifyIOSignature

        Public ErrorMessage As String
        Private ListLoaded As Boolean
        Private lodList As New List(Of Int32)

        Public Function VerifyLod(ByVal lod As Int32) As Boolean

            LoadList()

            If (lodList.Contains(lod)) Then
                Return True
            Else
                Return False
            End If

        End Function

        Private Sub LoadList()

            Dim LodsFromFile As String

            Try

                LodsFromFile = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/VerifyLodsForIOSignature.txt"))

                Dim Lods As String() = LodsFromFile.Split(New [Char]() {","c})

                For Each lod As String In Lods
                    lodList.Add(Convert.ToInt32(lod))
                Next

                ListLoaded = True
            Catch ex As FileNotFoundException
                ListLoaded = False
                ErrorMessage = ex.Message
            Catch ex As FileLoadException
                ListLoaded = False
                ErrorMessage = ex.Message

            End Try

        End Sub

    End Class

End Namespace