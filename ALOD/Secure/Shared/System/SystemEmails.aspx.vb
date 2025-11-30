Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports Microsoft.SqlServer.Server

Namespace Web.Sys

    Public Class SystemEmails
        Inherits System.Web.UI.Page

        Private groupSource As IUserGroupDao

        Private ReadOnly Property GroupDao() As IUserGroupDao
            Get
                If (groupSource Is Nothing) Then
                    groupSource = New NHibernateDaoFactory().GetUserGroupDao()
                End If
                Return groupSource
            End Get
        End Property

        Protected Sub btnExportToExcel_Click(sender As Object, e As EventArgs) Handles btnExportToExcel.Click
            Dim mailService As EmailService = New EmailService()

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=UserEmailAddresses_" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"

            Dim table As New HtmlTable

            ' Add the header row...
            Dim row As New HtmlTableRow()
            row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white")

            ' Add the header cells...
            AddTableCell(row, "EMAIL")

            table.Rows.Add(row)

            ' Add data cells...
            Dim odd As Boolean = False
            Dim data As StringCollection = mailService.GetDistributionListBySystemParameters(chkWorkEmail.Checked, chkPersonalEmail.Checked, chkUnitEmail.Checked, GetSelectedUserGroups())

            For Each item As String In data
                row = New HtmlTableRow

                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor)

                AddTableCell(row, item)

                table.Rows.Add(row)
            Next

            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)

            table.RenderControl(html)
            Response.Write(writer.ToString())
            Response.End()
        End Sub

        Protected Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

            ' Reset validation properties...
            lblResults.ForeColor = Drawing.Color.Red
            txtSubject.CssClass = String.Empty
            txtBody.CssClass = String.Empty

            ' Validate input...
            If (String.IsNullOrEmpty(txtSubject.Text)) Then
                lblResults.Text = "Must provide a Subject for the email!"
                txtSubject.CssClass = "fieldRequired"
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtBody.Text)) Then
                lblResults.Text = "Must provide a Body for the email!"
                txtBody.CssClass = "fieldRequired"
                Exit Sub
            End If

            Dim mailService As EmailService = New EmailService()

            If (mailService Is Nothing) Then
                lblResults.Text = "Error! Failed to intialize email service. Sending emails has been aborted."
                Exit Sub
            End If

            Dim addresses As StringCollection = mailService.GetDistributionListBySystemParameters(chkWorkEmail.Checked, chkPersonalEmail.Checked, chkUnitEmail.Checked, GetSelectedUserGroups())

            If (addresses.Count = 0) Then
                lblResults.Text = "No email addresses have been selected. Sending emails has been aborted."
                Exit Sub
            End If

            Dim msg As EMailMessage = mailService.CreateMessage(txtSubject.Text, txtBody.Text, String.Empty, addresses)

            If (msg Is Nothing) Then
                lblResults.Text = "Error! Failed to initialize the email message. Sending emails has been aborted."
                Exit Sub
            End If

            Dim results As Boolean = msg.Send(ddlEmailTemplates.SelectedValue, SESSION_USER_ID)

            If (results = True) Then
                lblResults.Text = "Emails successfully sent!"
                lblResults.ForeColor = Drawing.Color.Green
            Else
                lblResults.Text = "Error! Failed to send emails."
            End If
        End Sub

        Protected Sub chklUserGroups_ItemCheck(sender As Object, e As EventArgs) Handles chklUserGroups.SelectedIndexChanged
            ' Find the list index of the selected checkbox...
            Dim eventSourceControl As String = Request.Form("__EVENTTARGET")
            Dim checkedBox As String() = eventSourceControl.Split("$")
            Dim index As Integer = Integer.Parse(checkedBox(checkedBox.Length - 1))

            If (index = 0) Then
                ' Enable or disable checkboxes depending on the value of the "All" checkbox...
                For i As Integer = 1 To (chklUserGroups.Items.Count - 1)
                    If (chklUserGroups.Items(0).Selected = True) Then
                        chklUserGroups.Items(i).Enabled = False
                    Else
                        chklUserGroups.Items(i).Enabled = True
                    End If
                Next
            End If
        End Sub

        Protected Sub ddlEmailTemplates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlEmailTemplates.SelectedIndexChanged
            If (ddlEmailTemplates.SelectedIndex = 0) Then
                Exit Sub
            End If

            Dim factory As IDaoFactory = New NHibernateDaoFactory()
            Dim dao As IEmailTemplateDao = factory.GetEmailTemplateDao()

            Dim template As EmailTemplate = dao.GetById(Integer.Parse(ddlEmailTemplates.SelectedValue))

            txtSubject.Text = template.Subject
            txtBody.Text = template.Body
        End Sub

        Protected Sub InitControls()
            chkWorkEmail.Checked = True
            InitUserGroupsCheckboxList()
            InitEmailTemplateDDL()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Private Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Private Function GetSelectedUserGroups() As List(Of SqlDataRecord)
            Dim selectedGroups As List(Of SqlDataRecord) = New List(Of SqlDataRecord)()

            If (chklUserGroups.Items(0).Selected = False) Then
                For Each item As ListItem In chklUserGroups.Items
                    If (item.Selected = True) Then
                        Dim value As String = item.Value

                        Dim record As SqlDataRecord = New SqlDataRecord(New SqlMetaData() {New SqlMetaData("Column1", SqlDbType.Int)})

                        record.SetInt32(0, value)

                        selectedGroups.Add(record)
                    End If
                Next
            End If

            Return selectedGroups
        End Function

        Private Sub InitEmailTemplateDDL()
            Dim factory As IDaoFactory = New NHibernateDaoFactory()
            Dim dao As IEmailTemplateDao = factory.GetEmailTemplateDao()

            ddlEmailTemplates.DataSource = dao.GetSystemTemplates()
            ddlEmailTemplates.DataBind()

            ddlEmailTemplates.Items.Insert(0, New ListItem("-- Select Email Template --", 0))
        End Sub

        Private Sub InitUserGroupsCheckboxList()
            chklUserGroups.DataSource = From g In GroupDao.GetAll() Select g Order By g.Description
            chklUserGroups.DataBind()

            For Each item As ListItem In chklUserGroups.Items
                item.Enabled = False
            Next

            chklUserGroups.Items.Insert(0, New ListItem("All", 0))
            chklUserGroups.Items(0).Selected = True
        End Sub

    End Class

End Namespace