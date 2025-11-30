Namespace Common

    Public Class PwaiverDate

        ''' <summary>
        ''' Difference Between the last day of the month[Source Date] and the day picked by the Board Med.
        ''' </summary>
        ''' <remarks></remarks>
        Private differenceBetweenDays As Integer

        ''' <summary>
        ''' Variable to hold the PWaiver Length.
        ''' </summary>
        ''' <remarks>It can be mora than 90 days.</remarks>
        Private mPWaiverLength As Integer

        ''' <summary>
        ''' Source Date, the date that the Board Medical pick in the Baord Med tab.
        ''' </summary>
        ''' <remarks></remarks>
        Private SourceDate As DateTime

        ''' <summary>
        ''' Default Constructor
        ''' </summary>
        ''' <param name="OrigDate">Source Date picked by the Board Med.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal OrigDate As DateTime)

            SourceDate = OrigDate

        End Sub

        Public Property PWaiverLength() As Integer
            Get
                Return mPWaiverLength
            End Get
            Set(ByVal value As Integer)
                mPWaiverLength = value
            End Set
        End Property

        ''' <summary>
        ''' Calculate the expiration date. Calculate the PWaiver Length (in Days)
        ''' </summary>
        ''' <returns>Date.</returns>
        ''' <remarks></remarks>
        Public Function GetPWaiverExpirationDate() As DateTime

            Dim returnValue As DateTime

            differenceBetweenDays = CalculateDays(SourceDate.AddDays(90))
            PWaiverLength = differenceBetweenDays + 90

            returnValue = SourceDate.AddDays(PWaiverLength)

            Return returnValue

        End Function

        ''' <summary>
        ''' Calculate the Difference Between the last day of the month[Source Date] and the day picked by the Board Med.
        ''' </summary>
        ''' <param name="Static90SourceDate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalculateDays(ByVal Static90SourceDate As DateTime) As Integer

            Dim result As Integer = 0

            Select Case Static90SourceDate.Month
                Case 1, 3, 5, 7, 8, 10, 12
                    'January, March, May, July August, October, December  31 days
                    result = (31 - Static90SourceDate.Day)

                Case 4, 6, 9, 11
                    'April, June, September, November 30 days
                    result = (30 - Static90SourceDate.Day)

                Case 2
                    ' February, variable 28 and 29 days
                    If (Static90SourceDate.Year Mod 4 = 0) Then
                        result = (29 - Static90SourceDate.Day)
                    Else
                        result = (28 - Static90SourceDate.Day)
                    End If

            End Select

            Return result

        End Function

    End Class

End Namespace