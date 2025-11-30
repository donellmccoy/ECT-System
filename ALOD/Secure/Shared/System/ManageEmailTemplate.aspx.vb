Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Partial Class Secure_Shared_System_ManageEmailTemplate
        Inherits System.Web.UI.Page

        Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
            Response.Redirect("~/Secure/Shared/System/EditEmailTemplates.aspx?compo=" + CStr(HttpContext.Current.Session("Compo")), True)
        End Sub

        Protected Sub EmailCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            Response.Redirect("~/Secure/Shared/System/EditEmailTemplates.aspx?id=" + e.CommandArgument + "&compo=" + CStr(HttpContext.Current.Session("Compo")), True)
        End Sub

        Protected Sub gvemail_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvemail.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim emailitem As EmailTemplate = CType(e.Row.DataItem, EmailTemplate)

            Dim lblsubject As Label = CType(e.Row.FindControl("lblsubject"), Label)

            If (emailitem.Subject.Length > 20) Then
                lblsubject.ToolTip = Server.HtmlEncode(emailitem.Subject)
                lblsubject.Text = Server.HtmlEncode(emailitem.Subject.Substring(0, 20) + "...")
            Else
                lblsubject.Text = Server.HtmlEncode(emailitem.Subject)
            End If

            Dim lblbody As Label = CType(e.Row.FindControl("lblbody"), Label)

            If (emailitem.Body.Length > 70) Then
                lblbody.ToolTip = Server.HtmlEncode(emailitem.Body)
                lblbody.Text = Server.HtmlEncode(emailitem.Body.Substring(0, 70) + "...")
            Else
                lblbody.Text = Server.HtmlEncode(emailitem.Body)
            End If

            Dim lbltitle As Label = CType(e.Row.FindControl("lbltitle"), Label)

            If (emailitem.Title.Length > 20) Then
                lbltitle.ToolTip = Server.HtmlEncode(emailitem.Title)
                lbltitle.Text = Server.HtmlEncode(emailitem.Title.Substring(0, 20) + "...")
            Else
                lbltitle.Text = Server.HtmlEncode(emailitem.Title)
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim factory As IDaoFactory = New NHibernateDaoFactory
            Dim dao As IEmailTemplateDao = factory.GetEmailTemplateDao

            Dim emaillist As IList = dao.GetAll.ToList()
            gvemail.DataSource = emaillist
            gvemail.DataBind()

        End Sub

    End Class

End Namespace