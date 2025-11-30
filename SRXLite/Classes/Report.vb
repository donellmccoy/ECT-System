Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    Public Class Report

        Private _userID As Short

#Region " Constructors "

        Public Sub New()
        End Sub

        Public Sub New(ByVal userID As Short)
            _userID = userID
        End Sub

#End Region

#Region " UsageSummaryData "

        Public Class UsageSummaryData
            Private _action As String
            Private _category As String
            Private _count As Integer
            Private _userName As String

            Public Property ActionTypeName() As String
                Get
                    Return _action
                End Get
                Set(ByVal value As String)
                    _action = value
                End Set
            End Property

            Public Property CategoryName() As String
                Get
                    Return _category
                End Get
                Set(ByVal value As String)
                    _category = value
                End Set
            End Property

            Public Property Count() As Integer
                Get
                    Return _count
                End Get
                Set(ByVal value As Integer)
                    _count = value
                End Set
            End Property

            Public Property UserName() As String
                Get
                    Return _userName
                End Get
                Set(ByVal value As String)
                    _userName = value
                End Set
            End Property

        End Class

#End Region

#Region " GuidStatsData "

        Public Class GuidStatsData
            Private _docGuidCount As Integer
            Private _docPageGuidCount As Integer
            Private _reportDate As Date

            Public Property DocGuidCount() As Integer
                Get
                    Return _docGuidCount
                End Get
                Set(ByVal value As Integer)
                    _docGuidCount = value
                End Set
            End Property

            Public Property DocPageGuidCount() As Integer
                Get
                    Return _docPageGuidCount
                End Get
                Set(ByVal value As Integer)
                    _docPageGuidCount = value
                End Set
            End Property

            Public Property ReportDate() As Date
                Get
                    Return _reportDate
                End Get
                Set(ByVal value As Date)
                    _reportDate = value
                End Set
            End Property

        End Class

#End Region

#Region " GetUsageSummary "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="startDate"></param>
        ''' <param name="endDate"></param>
        ''' <param name="catg"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUsageSummary(ByVal startDate As Date, ByVal endDate As Date, ByVal catg As DataTypes.Category) As List(Of UsageSummaryData)
            Dim command As New SqlCommand
            command.CommandText = "dsp_Report_GetUsageSummary"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@StartDate", startDate))
                .Add(getSqlParameter("@EndDate", endDate))
                .Add(getSqlParameter("@CategoryID", catg))
            End With

            Dim usageSummaryData As UsageSummaryData
            Dim list As New List(Of UsageSummaryData)

            Using reader As SqlDataReader = DB.ExecuteReader(command)
                While reader.Read()
                    usageSummaryData = New UsageSummaryData
                    usageSummaryData.ActionTypeName = NullCheck(reader("ActionTypeName"))
                    usageSummaryData.CategoryName = NullCheck(reader("CategoryName"))
                    usageSummaryData.Count = IntCheck(reader("Count"))
                    usageSummaryData.UserName = NullCheck(reader("UserName"))

                    list.Add(usageSummaryData)
                End While
            End Using

            Return list
        End Function

#End Region

#Region " GetRequestStatistics "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="startDate"></param>
        ''' <param name="endDate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRequestStatistics(ByVal startDate As Date, ByVal endDate As Date) As List(Of GuidStatsData)
            Dim command As New SqlCommand
            command.CommandText = "dsp_Report_GetGuidStatistics"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@StartDate", startDate))
                .Add(getSqlParameter("@EndDate", endDate))
            End With

            Dim guidStatsData As GuidStatsData
            Dim list As New List(Of GuidStatsData)

            Using reader As SqlDataReader = DB.ExecuteReader(command)
                While reader.Read()
                    guidStatsData = New GuidStatsData
                    guidStatsData.ReportDate = DateCheck(reader("ReportDate"), Nothing)
                    guidStatsData.DocGuidCount = IntCheck(reader("DocGuidCount"))
                    guidStatsData.DocPageGuidCount = IntCheck(reader("DocPageGuidCount"))

                    list.Add(guidStatsData)
                End While
            End Using

            Return list
        End Function

#End Region

    End Class

End Namespace