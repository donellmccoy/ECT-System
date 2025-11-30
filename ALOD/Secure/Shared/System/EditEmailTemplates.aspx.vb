Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Partial Class Secure_Shared_System_EditEmailTemplates
        Inherits System.Web.UI.Page

        ''' <summary>
        ''' Description: fills EmailTemaple class from web controls and then pass the class to UpdateEmailTemplateInfo subroutine
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub FillEmailTemplateClass()

            Dim factory As IDaoFactory = New NHibernateDaoFactory
            Dim dao As IEmailTemplateDao = factory.GetEmailTemplateDao

            Dim email As EmailTemplate

            If Not IsNothing(Request.QueryString("id")) Then
                email = dao.GetById(Request.QueryString("id"))
            Else
                email = New EmailTemplate
            End If

            email.Subject = Me.txtSubject.Text.ToString()
            email.Title = Me.txtTitle.Text.ToString()
            email.Body = Me.txtBody.Text.ToString()
            email.DataProc = Me.txtDataSrc.Text()
            email.Active = Me.chkActive.Checked
            email.Date = Date.Now
            email.Compo = 6

            dao.SaveOrUpdate(email)

            Response.Redirect("~/Secure/Shared/System/manageEmailTemplate.aspx")
        End Sub

        ''' <summary>
        ''' Called by: SetControls
        ''' Description:
        ''' </summary>
        ''' <param name="id"></param>
        ''' <remarks></remarks>
        Public Sub LoadEmailTemplateInfo(ByVal id As Integer)

            Dim factory As IDaoFactory = New NHibernateDaoFactory
            Dim dao As IEmailTemplateDao = factory.GetEmailTemplateDao

            Dim email As EmailTemplate = dao.GetById(id)

            Me.txtSubject.Text = Server.HtmlEncode(email.Subject)
            Me.txtTitle.Text = Server.HtmlEncode(email.Title)
            Me.txtBody.Text = Server.HtmlEncode(email.Body)
            Me.txtDataSrc.Text = Server.HtmlEncode(email.DataProc)
            Me.chkActive.Checked = Server.HtmlEncode(email.Active)

        End Sub

        Public Sub RedirectPage()
            Response.Redirect("~/Secure/Shared/System/ManageEmailTemplate.aspx", True)
        End Sub

        ''' <summary>
        ''' This subroutine called on page load to fill web controls from database and
        ''' displays appropriate buttons based on query string status.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub SetControls()
            Dim id As Integer
            If IsNothing(Request.QueryString("id")) Then
                Me.ShowButton(btnAdd)
            Else
                id = Request.QueryString("id")
                Me.LoadEmailTemplateInfo(id)
                Me.ShowButton(btnUpdate)
            End If
        End Sub

        Public Sub ShowButton(ByVal btn As Button)
            Me.btnAdd.Visible = False
            Me.btnUpdate.Visible = False
            btn.Visible = True
        End Sub

        ''' <summary>
        ''' Called by: btnAdd_click event
        ''' Description: adds and/or updates email template info to database through EmailTemplate class
        ''' </summary>
        ''' <param name="email"></param>
        ''' <remarks></remarks>
        Public Sub UpdateEmailTemplateInfo(ByVal email As EmailTemplate)

            'If IsNothing(Request.QueryString("id")) Then
            '    email.InsertEmail(CStr(Request.QueryString("compo")))
            '    RedirectPage()
            'Else
            '    email.TemplateID = CInt(Request.QueryString("id"))
            '    email.UpdateEmail()
            '    RedirectPage()
            'End If

        End Sub

        Protected Sub BodyMultiLineValidator(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CustomValidator1.ServerValidate
            If (Me.txtBody.Text.Length < 2001) Then
                args.IsValid = True
            Else
                args.IsValid = False
            End If
        End Sub

        Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click, btnUpdate.Click

            If Page.IsValid Then
                FillEmailTemplateClass()
            End If

        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            RedirectPage()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then
                Me.SetControls()
            End If
        End Sub

    End Class

End Namespace