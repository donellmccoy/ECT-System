Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Admin

    Public Class ARCNetUserLookup
        Inherits System.Web.UI.Page

        Protected Const SUBTRACTDAYS As Double = -30

        Private _arcnetDao As IARCNetDao

        Protected ReadOnly Property ARCNetDao As IARCNetDao
            Get
                If (_arcnetDao Is Nothing) Then
                    _arcnetDao = New NHibernateDaoFactory().GetARCNetDao()
                End If

                Return _arcnetDao
            End Get
        End Property

        Protected ReadOnly Property BeginDate As Nullable(Of Date)
            Get
                If (txtBeginDate.Text.Length = 0) Then
                    txtBeginDate.Text = Now.AddDays(SUBTRACTDAYS).ToShortDateString()
                End If

                Return Server.HtmlEncode(CDate(txtBeginDate.Text))
            End Get
        End Property

        Protected ReadOnly Property CalendarImage As String
            Get
                Return Utility.GetCalendarImage(Page)
            End Get
        End Property

        Protected ReadOnly Property EDIPI As String
            Get
                If (txtEDIPI.Text.Length = 0) Then
                    Return String.Empty
                End If

                Return Server.HtmlEncode(txtEDIPI.Text)
            End Get
        End Property

        Protected ReadOnly Property EndDate As Nullable(Of Date)
            Get
                If (txtEndDate.Text.Length = 0) Then
                    txtEndDate.Text = Now.ToShortDateString()
                End If

                Dim theEndDate As Date = Server.HtmlEncode(CDate(txtEndDate.Text))

                If (theEndDate < BeginDate.Value) Then
                    txtEndDate.Text = BeginDate.Value.AddDays(1)
                End If

                If (theEndDate > BeginDate.Value.AddMonths(1)) Then
                    txtEndDate.Text = BeginDate.Value.AddMonths(1)
                End If

                Return Server.HtmlEncode(CDate(txtEndDate.Text))
            End Get
        End Property

        Protected ReadOnly Property FirstName As String
            Get
                If (txtUserFirstName.Text.Length = 0) Then
                    Return String.Empty
                End If

                Return Server.HtmlEncode(txtUserFirstName.Text)
            End Get
        End Property

        Protected ReadOnly Property LastName As String
            Get
                If (txtUserLastName.Text.Length = 0) Then
                    Return String.Empty
                End If

                Return Server.HtmlEncode(txtUserLastName.Text)
            End Get
        End Property

        Protected ReadOnly Property MiddleNames As String
            Get
                If (txtUserMiddleName.Text.Length = 0) Then
                    Return String.Empty
                End If

                Return Server.HtmlEncode(txtUserMiddleName.Text)
            End Get
        End Property

        Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
            ExecuteLookup()
        End Sub

        Protected Function BypassDates() As Boolean
            If (Not String.IsNullOrEmpty(EDIPI)) Then
                Return True
            End If

            If (Not String.IsNullOrEmpty(LastName) OrElse
                Not String.IsNullOrEmpty(FirstName) OrElse
                Not String.IsNullOrEmpty(MiddleNames)) Then
                Return True
            End If

            Return False
        End Function

        Protected Function ConstructReportArguments() As ARCNetLookupReportArgs
            Dim args As ARCNetLookupReportArgs = New ARCNetLookupReportArgs()

            args.EDIPIN = EDIPI
            args.LastName = LastName
            args.FirstName = FirstName
            args.MiddleNames = MiddleNames

            If (Not BypassDates()) Then
                args.BeginDate = BeginDate.Value
                args.EndDate = EndDate.Value
            End If

            Return args
        End Function

        Protected Sub ExecuteLookup()
            gdvResults.DataSource = ARCNetDao.GetIAATrainingDataForUsers(ConstructReportArguments())
            gdvResults.DataBind()

            pnlLookupOutput.Visible = True
        End Sub

        Protected Sub gdvResults_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvResults.PageIndexChanging
            gdvResults.PageIndex = e.NewPageIndex
            ExecuteLookup()
        End Sub

        Protected Sub gdvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gdvResults.RowCommand
            If (e.CommandName.Equals("EditUser")) Then
                Session("EditId") = e.CommandArgument
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?Caller=1", True)
            End If
        End Sub

        Protected Sub InitControls()
            InitLastExecutionDateLabel()
        End Sub

        Protected Sub InitLastExecutionDateLabel()
            Dim executionDate As Nullable(Of Date) = ARCNetDao.GetARCNetImportLastExecutionDate()

            If (executionDate.HasValue) Then
                lblLastImportExecutionDate.Text = executionDate.Value.ToString()
            Else
                lblLastImportExecutionDate.Text = "UNKNOWN"
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitControls()

            Session("EditId") = Nothing
        End Sub

    End Class

End Namespace