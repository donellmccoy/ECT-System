Imports ALOD.Core.Domain.Lookup
Imports ALOD.Data

Namespace Common

    Public Class PEPPRating
        Inherits IntValuedLookupItem

        Protected _active As Integer = 0            ' active

#Region "Properties"

        Public Property Active() As Integer
            Get
                Return _active
            End Get

            Set(value As Integer)
                _active = value
            End Set
        End Property

#End Region

        ' Get all PEPPRating records from the lookup table for the Sys Admin
        Public Overrides Function GetRecords() As DataTable
            Dim lkup As ILookupDao = New NHibernateDaoFactory().GetLookupDao()
            Return lkup.GetPEPPRatings(0, 0).Tables(0)
        End Function

        ' Insert new PEPPRating into the lookup table
        Public Sub InsertPEPPRating(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPRating", Value, Name, Active)
        End Sub

        ' Edit/Update an existing PEPPRating in the lookup table
        Public Sub UpdatePEPPRating(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPRating", Value, Name, Active)
        End Sub

    End Class

End Namespace