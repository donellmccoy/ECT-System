Imports ALOD.Core.Domain.Lookup
Imports ALOD.Data

Namespace Common

    Public Class PEPPType
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

        ' Get all PEPPType records from the lookup table for the Sys Admin
        Public Overrides Function GetRecords() As DataTable
            Dim lkup As ILookupDao = New NHibernateDaoFactory().GetLookupDao()
            Return lkup.GetPEPPTypes(0, 0).Tables(0)
        End Function

        ' Insert new PEPPType into the lookup table
        Public Sub InsertPEPPType(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPType", Value, Name, Active)
        End Sub

        ' Edit/Update an existing PEPPType in the lookup table
        Public Sub UpdatePEPPType(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPType", Value, Name, Active)
        End Sub

    End Class

End Namespace