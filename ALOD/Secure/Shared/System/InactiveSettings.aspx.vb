Imports ALOD.Core.Domain.Common
Imports ALOD.Data

Public Class InactiveSettings
    Inherits System.Web.UI.Page

    Dim dao As New ALOD.Data.ReminderEmailsDao()
    Private emailTemplateSource As IList(Of EmailTemplate)

    Private ReadOnly Property EmailTemplates() As IList(Of EmailTemplate)
        Get
            If (emailTemplateSource Is Nothing) Then
                Dim list As IQueryable(Of EmailTemplate) = New NHibernateDaoFactory().GetEmailTemplateDao().GetAll()
                emailTemplateSource = (From l In list Where l.Compo = Session("Compo") Select l).ToList()
            End If
            Return emailTemplateSource
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            initControl()
        End If

    End Sub

    Protected Sub updateButton_Click(sender As Object, e As EventArgs)
        dao.ReminderUpdateInactiveSettings(inactive.Text.ToString(), notif.Text.ToString(), cbData.SelectedValue(), active.Checked)
    End Sub

    Private Sub initControl()
        Dim inactiveSettings As New DataSet()
        inactiveSettings = dao.ReminderGetInactiveSettings()

        Dim table As DataTable = inactiveSettings.Tables(0)
        Dim row As DataRow = table.Rows(0)
        inactive.Text = (Convert.ToString(row("interval")))
        notif.Text = (Convert.ToString(row("notification_interval")))
        active.Checked = (Convert.ToBoolean(row("active")))
        cbData.SelectedValue = (Convert.ToInt32(row("templateid")))

        'populate data with email options
        cbData.DataSource = From t In EmailTemplates Where t.Compo = 6 Select t.Title, t.Id
        cbData.DataTextField = "Title"
        cbData.DataValueField = "Id"
        cbData.DataBind()
    End Sub

End Class