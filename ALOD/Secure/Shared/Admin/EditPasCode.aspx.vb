Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_EditPasCode
        Inherits System.Web.UI.Page

#Region "Fields/Properties"

        Protected Const KEY_ERROR_MESSAGE As String = "ERROR_MESSAGE"
        Protected Const KEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"
        Protected Const MSG_ERROR_MODFIYING_UNIT As String = "An error occured while updating the unit"
        Protected Const MSG_UNIT_MODIFIED As String = "Successfully updated the unit.Please select the Manage Reporting button to modify the reporting chain."
        Protected Const QUERYSTRING_CSID As String = "csId"
        Protected Const UNKNOWN_UNIT_ID As Integer = 11
        Private _unitDao As IUnitDao

        ReadOnly Property UnitDao() As IUnitDao
            Get
                If (_unitDao Is Nothing) Then
                    _unitDao = New NHibernateDaoFactory().GetUnitDao()
                End If

                Return _unitDao
            End Get
        End Property

        Public ReadOnly Property QueryStringCSId As Integer
            Get
                If (Not String.IsNullOrEmpty(Request.QueryString(QUERYSTRING_CSID))) Then
                    Return CType(Request.QueryString(QUERYSTRING_CSID), Integer)
                End If

                Return 0
            End Get
        End Property

        Protected Property ErrorMessage() As String
            Get
                If (ViewState(KEY_ERROR_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_ERROR_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_ERROR_MESSAGE) = value
            End Set
        End Property

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

#End Region

#Region "Page Methods"

        Public Sub BindDropDowns()
            ddlComponent.DataBind()
            ddlGainingCommand.DataBind()
            ddlOperationType.DataBind()
            ddlReportToUnits.DataBind()
            ddlState.DataBind()
            ddlTimeZone.DataBind()
            ddlUnitLevel.DataBind()
            BindReportGrid()
        End Sub

        Public Sub UpdatePasCode()
            Page.Validate("edit")

            If (Page.IsValid) Then
                Dim csId As Integer = Int32.Parse(Server.HtmlEncode(lblUnitId.Text))
                Dim editUnit As ALOD.Core.Domain.Users.Unit = UnitDao.GetById(csId)

                editUnit.Name = txtUnitDescription.Text.Trim

                If txtUnitNbr.Text.Trim <> String.Empty Then
                    editUnit.UnitNumber = Server.HtmlEncode(txtUnitNbr.Text.Trim)
                End If

                If txtUnitKnd.Text.Trim <> String.Empty Then
                    editUnit.UnitKind = Server.HtmlEncode(txtUnitKnd.Text.Trim)
                End If

                If txtUnitType.Text.Trim <> String.Empty Then
                    editUnit.UnitType = Server.HtmlEncode(txtUnitType.Text.Trim)
                End If

                If txtUnitDet.Text.Trim <> String.Empty Then
                    editUnit.UnitDet = Server.HtmlEncode(txtUnitDet.Text.Trim)
                End If

                If txtUIC.Text.Trim <> String.Empty Then
                    editUnit.Uic = Server.HtmlEncode(txtUIC.Text.Trim)
                End If

                If ddlUnitLevel.SelectedValue.Trim <> String.Empty Then
                    editUnit.CommandStructLevel = Server.HtmlEncode(ddlUnitLevel.SelectedValue)
                End If

                editUnit.PasCode = txtPasCode.Text.Trim

                If txtBaseCode.Text.Trim <> String.Empty Then
                    editUnit.BaseCode = Server.HtmlEncode(txtBaseCode.Text.Trim)
                End If

                Dim unitId As Integer
                editUnit.ParentUnit = IIf(Integer.TryParse(Server.HtmlEncode(ddlReportToUnits.SelectedValue), unitId), UnitDao.GetById(unitId), UnitDao.GetById(UNKNOWN_UNIT_ID))
                editUnit.GainingCommand = IIf(Integer.TryParse(Server.HtmlEncode(ddlGainingCommand.SelectedValue), unitId), UnitDao.GetById(unitId), UnitDao.GetById(UNKNOWN_UNIT_ID))

                If ddlOperationType.SelectedValue.Trim <> String.Empty Then
                    editUnit.CommandStructOperationType = Server.HtmlEncode(ddlOperationType.SelectedValue)
                End If

                If ddlTimeZone.SelectedValue.Trim <> String.Empty Then
                    editUnit.TimeZone = Server.HtmlEncode(ddlTimeZone.SelectedValue)
                End If

                editUnit.Component = Server.HtmlEncode(ddlComponent.SelectedValue)

                If txtAddr1.Text.Trim <> String.Empty Then
                    editUnit.Address1 = Server.HtmlEncode(txtAddr1.Text.Trim)
                End If

                If txtAddr2.Text.Trim <> String.Empty Then
                    editUnit.Address2 = Server.HtmlEncode(txtAddr2.Text.Trim)
                End If

                If txtCity.Text.Trim <> String.Empty Then
                    editUnit.City = Server.HtmlEncode(txtCity.Text.Trim)
                End If

                If ddlState.SelectedValue.Trim <> String.Empty Then
                    editUnit.State = Server.HtmlEncode(ddlState.SelectedValue)
                End If

                If txtZipCode.Text.Trim <> String.Empty Then
                    editUnit.PostalCode = Server.HtmlEncode(txtZipCode.Text.Trim)
                End If

                If txtCountry.Text.Trim <> String.Empty Then
                    editUnit.Country = Server.HtmlEncode(txtCountry.Text.Trim)
                End If

                If txtEmail.Text.Trim <> String.Empty Then
                    editUnit.Email = Server.HtmlEncode(txtEmail.Text.Trim)
                End If

                editUnit.InActive = InActiveCheckBox.Checked
                editUnit.IsCollocated = chkIsCollocated.Checked
                editUnit.ModifiedDate = DateTime.Now
                editUnit.ModifiedBy = UserService.CurrentUser()
                editUnit.UserModified = True

                Try
                    UnitDao.SaveOrUpdate(editUnit)
                    FeedbackMessage = MSG_UNIT_MODIFIED
                    UnitDao.CommitChanges()
                    UnitDao.Evict(editUnit)
                    editUnit = UnitDao.FindById(csId)
                    LoadUnit()
                Catch ex As Exception
                    ErrorMessage = MSG_ERROR_MODFIYING_UNIT
                End Try

                LogManager.LogAction(ModuleType.LOD, UserAction.ModifyUnit, editUnit.Id, "Modified unit with pascode " + editUnit.PasCode)
            End If
        End Sub

        Protected Sub BindReportGrid()
            gvReporting.DataSource = UnitService.GetReportingUnits(QueryStringCSId)
            gvReporting.DataBind()
        End Sub

        Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
            Dim url As New StringBuilder
            url.Append(Page.ResolveUrl("~/Secure/Shared/Admin/ManageHierarchy.aspx"))
            url.Append("?csId=" + QueryStringCSId.ToString())
            Response.Redirect(url.ToString())
        End Sub

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
            UpdatePasCode()
        End Sub

        Protected Sub cbDataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGainingCommand.DataBound, ddlOperationType.DataBound, ddlReportToUnits.DataBound, ddlState.DataBound, ddlTimeZone.DataBound, ddlUnitLevel.DataBound
            Dim list As DropDownList = CType(sender, DropDownList)

            If (list.Items.Count > 1) Then
                list.Items.Insert(0, New ListItem("-- Select --", ""))
            End If
        End Sub

        Protected Sub lnkManageUnits_Click(sender As Object, e As System.EventArgs) Handles lnkManageUnits.Click
            Response.Redirect("~/Secure/Shared/Admin/Pascodes.aspx")
        End Sub

        Protected Sub LoadUnit()
            Dim csId As Integer = Int32.Parse(lblUnitId.Text)
            Dim editUnit As ALOD.Core.Domain.Users.Unit = UnitDao.GetById(csId)

            txtUnitDescription.Text = Server.HtmlEncode(editUnit.Name)
            txtUnitNbr.Text = Server.HtmlEncode(editUnit.UnitNumber)
            txtUnitKnd.Text = Server.HtmlEncode(editUnit.UnitKind)
            txtUnitType.Text = Server.HtmlEncode(editUnit.UnitType)
            txtUnitDet.Text = Server.HtmlEncode(editUnit.UnitDet)
            txtUIC.Text = Server.HtmlEncode(editUnit.Uic)

            If editUnit.ParentUnit IsNot Nothing Then
                SetDropdownByValue(ddlReportToUnits, editUnit.ParentUnit.Id)
            End If

            If editUnit.GainingCommand IsNot Nothing Then
                SetDropdownByValue(ddlGainingCommand, editUnit.GainingCommand.Id)
            End If

            txtPasCode.Text = Server.HtmlEncode(editUnit.PasCode)
            txtBaseCode.Text = Server.HtmlEncode(editUnit.BaseCode)
            ddlOperationType.SelectedValue = Server.HtmlEncode(editUnit.CommandStructOperationType)
            ddlTimeZone.Text = Server.HtmlEncode(editUnit.TimeZone)
            ddlComponent.SelectedValue = Server.HtmlEncode(editUnit.Component)
            txtAddr1.Text = Server.HtmlEncode(editUnit.Address1)
            txtAddr2.Text = Server.HtmlEncode(editUnit.Address2)
            txtCity.Text = Server.HtmlEncode(editUnit.City)
            InActiveCheckBox.Checked = editUnit.InActive
            SetDropdownByValue(ddlState, editUnit.State)
            txtZipCode.Text = Server.HtmlEncode(editUnit.PostalCode)
            txtCountry.Text = Server.HtmlEncode(editUnit.Country)
            txtEmail.Text = Server.HtmlEncode(editUnit.Email)
            chkIsCollocated.Checked = editUnit.IsCollocated
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                BindDropDowns()
                lblUnitId.Text = QueryStringCSId
                LoadUnit()
                AddJavaScriptAttributesToControls()
                ErrorMessage = String.Empty
                FeedbackMessage = String.Empty
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ErrorPanel.Visible = ErrorMessage.Length > 0
            ErrorMessageLabel.Text = ErrorMessage

            FeedbackMessageLabel.Text = FeedbackMessage
            FeedbackPanel.Visible = FeedbackMessage.Length > 0
        End Sub

        Private Sub AddJavaScriptAttributesToControls()
            btnFindGaining.Attributes.Add("onclick", "showSearcher('" + "Gaining Command" + "','" + ddlGainingCommand.ClientID + "','" + "" + "'); return false;")
            btnFindReporting.Attributes.Add("onclick", "showSearcher('" + "Reporting to Command" + "','" + ddlReportToUnits.ClientID + "','" + "" + "'); return false;")
        End Sub

#End Region

    End Class

End Namespace