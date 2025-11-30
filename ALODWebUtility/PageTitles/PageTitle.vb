Imports ALOD.Data

Namespace PageTitles

    Public Class PageTitle

        Protected _pageId As Integer = 0
        Protected _title As String = String.Empty

        Public Sub New()

        End Sub

        Public Property PageId() As Integer
            Get
                Return _pageId
            End Get
            Set(ByVal value As Integer)
                _pageId = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Sub Insert()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_pageTitles_sp_Insert", _title)

        End Sub

        Public Sub ToDataRow(ByRef row As DataSets.PageTitlesRow)
            row.pageId = _pageId
            row.title = _title
        End Sub

    End Class

End Namespace