Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_ManageHierarchy
        Inherits System.Web.UI.Page

        Protected Const CHAIN_UPDATE_MESSAGE As String = "The rebuild has started, you will be notified when it is complete"
        Protected Const KEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"

        Protected Property FeedbackMessage() As String
            Get
                If (ViewState(KEY_FEEDBACK_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_FEEDBACK_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_FEEDBACK_MESSAGE) = value
            End Set
        End Property

#Region "Load"

        Protected Sub BindReportGrid()
            Dim csId As Integer
            Integer.TryParse(Server.HtmlEncode(Request.QueryString("csId")), csId)
            gvReporting.DataSource = UnitService.GetReportingUnits(csId)
            gvReporting.DataBind()
        End Sub

        Protected Sub gvReporting_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvReporting.RowDataBound

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim btnEdit As Button = CType(e.Row.FindControl("btnEdit"), Button)
                Dim hdnNewUnit As HtmlControls.HtmlInputHidden = CType(e.Row.FindControl("hdnUnit"), HtmlControls.HtmlInputHidden)
                Dim lblNewUnitName As Label = CType(e.Row.FindControl("lblNewUnitName"), Label)
                Dim chnType As String = CType(e.Row.FindControl("lblChainType"), Label).Text
                btnEdit.Attributes.Add("onclick", "showSearcher('" + chnType + "','" + hdnNewUnit.ClientID + "','" + lblNewUnitName.ClientID + "'); return false;")

            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim csId As Integer
            If Not (Integer.TryParse(Server.HtmlEncode(Request.QueryString("csId")), csId)) Then
                gvReporting.Visible = False
                btnUpdate.Visible = False
                Return
            End If

            If Not Page.IsPostBack Then
                Dim pCode As New pascode(csId)
                BindReportGrid()
                pCode.LoadPasCode()
                lblPasCode.Text = Server.HtmlEncode(pCode.pas_code)
                lblUnitName.Text = Server.HtmlEncode(pCode.long_name)

            End If

        End Sub

#End Region

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click

            Dim url As New StringBuilder
            url.Append(Page.ResolveUrl("~/Secure/Shared/Admin/EditPasCode.aspx"))
            url.Append("?csId=" + Request.QueryString("csId"))
            Response.Redirect(url.ToString())

        End Sub

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

            UpdateReporting()
            BindReportGrid()

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            FeedbackPanel.Visible = FeedbackMessage.Length > 0
            FeedbackMessageLabel.Text = FeedbackMessage

        End Sub

        Protected Sub UpdateReporting()

            Dim csId As Integer
            If Not (Int32.TryParse(Request.QueryString("csId"), csId)) Then
                Response.Redirect(Resources._Global.StartPage)
                FeedbackMessage = String.Empty
            End If
            Dim editUnit As ALOD.Core.Domain.Users.Unit = New NHibernateDaoFactory().GetUnitDao().GetById(csId)

            For Each row As GridViewRow In gvReporting.Rows
                Dim chainType As String = CType(row.FindControl("lblChainType"), Label).Text
                Dim name As String = CType(row.FindControl("lblUnitName"), Label).Text
                Dim newUnit As String = CType(row.FindControl("hdnUnit"), HtmlControls.HtmlInputHidden).Value
                Dim newUnitId As Integer
                If (Int32.TryParse(newUnit, newUnitId)) Then
                    editUnit.ReportingStructure.Add(chainType, newUnitId)
                End If
            Next

            UnitService.UpdateReportingChain(editUnit, SESSION_USER_ID)
            UnitService.UpdateAffectedUnits(csId, SESSION_USER_ID)
            FeedbackMessage = CHAIN_UPDATE_MESSAGE

        End Sub

    End Class

End Namespace