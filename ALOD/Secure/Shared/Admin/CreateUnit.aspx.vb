Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_CreateUnit
        Inherits System.Web.UI.Page
        Protected Const UNKNOWN_UNIT_ID As Integer = 11

#Region "Members"

        Protected Const KEY_ERROR_MESSAGE As String = "ERROR_MESSAGE"
        Protected Const KEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"
        Protected Const MSG_ERROR_ADDING_UNIT As String = "An error occured creating the unit"
        Protected Const MSG_INVALID_PASCODE As String = "Invalid PasCode"
        Protected Const MSG_PASCODE_FOUND As String = "Unit already exists in the system.  Please try a new pascode or edit the existing unit"
        Protected Const MSG_UNIT_ADDED As String = "Unit Added"
        Dim idao As IUnitDao

        Dim newUnitCsID As Integer

        ReadOnly Property UnitDao() As IUnitDao
            Get
                If (idao Is Nothing) Then
                    idao = New NHibernateDaoFactory().GetUnitDao()
                End If

                Return idao
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

#Region "Load"

        Public Sub BindDropDowns()

            ddlComponent.DataBind()
            ddlGainingCommand.DataBind()
            ddlOperationType.DataBind()
            ddlState.DataBind()
            ddlTimeZone.DataBind()
            ddlUnitLevel.DataBind()

        End Sub

        Protected Sub cbDataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlComponent.DataBound, ddlGainingCommand.DataBound, ddlOperationType.DataBound, ddlReportToUnits.DataBound, ddlState.DataBound, ddlTimeZone.DataBound, ddlUnitLevel.DataBound

            Dim list As DropDownList = CType(sender, DropDownList)
            If (list.Items.Count > 1) Then
                list.Items.Insert(0, New ListItem("-- Select --", ""))
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Page.IsPostBack Then
                BindDropDowns()
                btnFindGaining.Attributes.Add("onclick", "showSearcher('" + "Gaining Command" + "','" + ddlGainingCommand.ClientID + "','" + "" + "'); return false;")
                btnFindReporting.Attributes.Add("onclick", "showSearcher('" + "Reporting to Command" + "','" + ddlReportToUnits.ClientID + "','" + "" + "'); return false;")

            End If

        End Sub

#End Region

#Region "UserAction"

        Public Sub CreateUnit()
            Page.Validate("create")
            If Page.IsValid Then

                Dim newUnit As New ALOD.Core.Domain.Users.Unit
                newUnit = New ALOD.Core.Domain.Users.Unit

                newUnit.Name = txtUnitDescription.Text.Trim

                If txtUnitNbr.Text.Trim <> String.Empty Then
                    newUnit.UnitNumber = Server.HtmlEncode(txtUnitNbr.Text.Trim)
                End If
                If txtUnitKnd.Text.Trim <> String.Empty Then
                    newUnit.UnitKind = Server.HtmlEncode(txtUnitKnd.Text.Trim)
                End If
                If txtUnitType.Text.Trim <> String.Empty Then
                    newUnit.UnitType = Server.HtmlEncode(txtUnitType.Text.Trim)
                End If
                If txtUnitDet.Text.Trim <> String.Empty Then
                    newUnit.UnitDet = Server.HtmlEncode(txtUnitDet.Text.Trim)
                End If

                If txtUIC.Text.Trim <> String.Empty Then
                    newUnit.Uic = Server.HtmlEncode(txtUIC.Text.Trim)
                End If
                If ddlUnitLevel.SelectedValue.Trim <> String.Empty Then
                    newUnit.CommandStructLevel = Server.HtmlEncode(ddlUnitLevel.SelectedValue)
                End If

                newUnit.PasCode = PasCodeLbl.Text

                If txtBaseCode.Text.Trim <> String.Empty Then
                    newUnit.BaseCode = Server.HtmlEncode(txtBaseCode.Text.Trim)
                End If
                Dim unitId As Integer
                newUnit.ParentUnit = IIf(Integer.TryParse(Server.HtmlEncode(ddlReportToUnits.SelectedValue), unitId), UnitDao.GetById(unitId), UnitDao.GetById(UNKNOWN_UNIT_ID))
                newUnit.GainingCommand = IIf(Integer.TryParse(Server.HtmlEncode(ddlGainingCommand.SelectedValue), unitId), UnitDao.GetById(unitId), UnitDao.GetById(UNKNOWN_UNIT_ID))

                'newUnit.Scheduling = IIf(Server.HtmlEncode(rblRCPHRAScheduler.SelectedValue) = "Yes", True, False)

                If ddlOperationType.SelectedValue.Trim <> String.Empty Then
                    newUnit.CommandStructOperationType = Server.HtmlEncode(ddlOperationType.SelectedValue)
                End If
                If ddlTimeZone.SelectedValue.Trim <> String.Empty Then
                    newUnit.TimeZone = Server.HtmlEncode(ddlTimeZone.SelectedValue)
                End If

                newUnit.Component = Server.HtmlEncode(ddlComponent.SelectedValue)
                'If geoLocBox.Text.Trim <> String.Empty Then
                'newUnit.GeographicLocation = Server.HtmlEncode(geoLocBox.Text.Trim)
                'End If

                'newUnit.PhysicalExam = IIf(Server.HtmlEncode(rblPhysicaUnit.SelectedValue) = "Yes", True, False)

                If txtAddr1.Text.Trim <> String.Empty Then
                    newUnit.Address1 = Server.HtmlEncode(txtAddr1.Text.Trim)

                End If
                If txtAddr2.Text.Trim <> String.Empty Then
                    newUnit.Address2 = Server.HtmlEncode(txtAddr2.Text.Trim)

                End If
                If txtCity.Text.Trim <> String.Empty Then
                    newUnit.City = Server.HtmlEncode(txtCity.Text.Trim)

                End If
                If ddlState.SelectedValue.Trim <> String.Empty Then
                    newUnit.State = Server.HtmlEncode(ddlState.SelectedValue)
                End If
                If txtZipCode.Text.Trim <> String.Empty Then
                    newUnit.PostalCode = Server.HtmlEncode(txtZipCode.Text.Trim)

                End If
                If txtCountry.Text.Trim <> String.Empty Then
                    newUnit.Country = Server.HtmlEncode(txtCountry.Text.Trim)

                End If
                If txtEmail.Text.Trim <> String.Empty Then
                    newUnit.Email = Server.HtmlEncode(txtEmail.Text.Trim)

                End If
                newUnit.InActive = InActiveCheckBox.Checked
                newUnit.ModifiedDate = DateTime.Now
                newUnit.ModifiedBy = UserService.CurrentUser
                newUnit.UserModified = True
                UnitDao.SaveOrUpdate(newUnit)
                Dim newUnitId As Integer = newUnit.Id

                If (newUnitId > 0) Then

                    UnitDao.CommitChanges()
                    UnitDao.Evict(newUnit)
                    newUnit = Nothing
                    newUnit = UnitDao.FindById(newUnitId)
                    UnitService.SetDefaultChain(newUnit)
                    LogManager.LogAction(ModuleType.LOD, UserAction.CreateNewUnit, newUnit.Id, "Created new unit with pascode " + newUnit.PasCode)
                    ManageHierarchy(newUnit.Id)
                End If

            End If
        End Sub

        Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click
            Dim passed As Boolean = True

            CreateUnit()
        End Sub

        Protected Sub ManageHierarchy(ByVal csId As Integer)

            Dim url As New StringBuilder
            url.Append(Page.ResolveUrl("~/Secure/Shared/Admin/ManageHierarchy.aspx"))
            url.Append("?csId=" + csId.ToString())
            Response.Redirect(url.ToString())

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            ErrorPanel.Visible = ErrorMessage.Length > 0
            ErrorMessageLabel.Text = ErrorMessage

        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click

            Dim pascode As String = PascodeSearchBox.Text.Trim
            If (Not pascode.Length = 4) Then
                ErrorMessage = MSG_INVALID_PASCODE
                Exit Sub
            End If

            'we have a valid pascode, does this pascode already exist in the system?

            Dim units As IList(Of ALOD.Core.Domain.Users.Unit) = UnitService.GetUnitsByPasCode(pascode)

            If (units.Count > 0) Then
                'we did find this pascode, don't add them again
                ErrorMessage = MSG_PASCODE_FOUND
                UnitGrid.DataSource = units
                UnitGrid.DataBind()
                Exit Sub
            End If

            ErrorMessage = String.Empty

            PasCodeLbl.Text = pascode
            SearchPanel.Visible = False
            InputPanel.Visible = True

        End Sub

        Protected Sub UnitGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles UnitGrid.RowCommand

            If (e.CommandName = "EditUnit") Then
                Session("EditId") = e.CommandArgument
                Response.Redirect("~/Secure/Shared/Admin/EditPasCode.aspx?csId=" + e.CommandArgument, True)
            End If

        End Sub

#End Region

    End Class

End Namespace