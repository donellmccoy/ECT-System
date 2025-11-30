Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_ChildUnits
        Inherits System.Web.UI.Page

        '''<summary>
        '''<para>The method calls UnitService's's GetChildChain method.It passes parent unit and chain type as arguments
        ''' </para>
        '''</summary>
        Public Sub GetUnits()
            parentUnitLabelId.Text = SrcNameHdn.Value
            Dim unit As Integer = CInt(SrcUnitIdHdn.Value.Trim)
            Dim units As DataSet = UnitService.GetChildChain(unit, ChainTypeSelect.SelectedValue)
            UnitGrid.DataSource = units
            UnitGrid.DataBind()

        End Sub

        Protected Sub ChainTypeSelect_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChainTypeSelect.SelectedIndexChanged
            GetUnits()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;")
                Dim chainTypes = New NHibernateDaoFactory().GetUnitChainTypeDao().GetAll()
                ChainTypeSelect.DataSource = From p In chainTypes Where p.Active = True Select p
                ChainTypeSelect.DataBind()
            End If

        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            GetUnits()
        End Sub

        Protected Sub SrcUnitIdHdn_ServerChange(ByVal sender As Object, ByVal e As System.EventArgs) Handles SrcUnitIdHdn.ServerChange
            GetUnits()
        End Sub

        '''<summary>
        '''<para>The method calls EditPasCode page.
        ''' </para>
        '''</summary>
        Protected Sub UnitGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles UnitGrid.RowCommand

            If (e.CommandName = "EditUnit") Then
                Session("EditId") = e.CommandArgument
                Response.Redirect("~/Secure/Shared/Admin/EditPasCode.aspx?csId=" + e.CommandArgument, True)
            End If

        End Sub

    End Class

End Namespace