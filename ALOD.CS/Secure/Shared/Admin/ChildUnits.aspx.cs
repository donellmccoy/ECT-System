using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ALOD.Data;
using ALOD.Data.Services;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_ChildUnits : System.Web.UI.Page
    {
        // TODO: Uncomment when UserControls are converted
        // protected ALOD.Web.UserControls.Secure_Shared_UserControls_unitSearcher unitSearcher;
        protected HtmlInputHidden hdnIdClient;
        protected HtmlInputHidden hdnNameClient;
        protected Panel FeedbackPanel;
        protected TextBox UnitNameTxt;
        protected RequiredFieldValidator RequiredFieldValidator2;
        protected HtmlInputHidden SrcNameHdn;
        protected HtmlInputHidden SrcUnitIdHdn;
        protected Button btnFindUnit;
        protected DropDownList ChainTypeSelect;
        protected RequiredFieldValidator RequiredFieldValidator1;
        protected Button SearchButton;
        protected Label lblUnitMsg;
        protected ValidationSummary ValidationSummary1;
        protected UpdatePanel resultsUpdatePanel;
        protected Label parentUnitLabelId;
        protected Image imgWait;
        protected GridView UnitGrid;
        protected UpdatePanelAnimationExtender resultsUpdatePanelAnimationExtender;

        /// <summary>
        /// The method calls UnitService's GetChildChain method. It passes parent unit and chain type as arguments.
        /// </summary>
        public void GetUnits()
        {
            parentUnitLabelId.Text = SrcNameHdn.Value;
            int unit = int.Parse(SrcUnitIdHdn.Value.Trim());
            DataSet units = UnitService.GetChildChain(unit, Convert.ToByte(ChainTypeSelect.SelectedValue));
            UnitGrid.DataSource = units;
            UnitGrid.DataBind();
        }

        protected void ChainTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetUnits();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;");
                var chainTypes = new NHibernateDaoFactory().GetUnitChainTypeDao().GetAll();
                ChainTypeSelect.DataSource = from p in chainTypes where p.Active == true select p;
                ChainTypeSelect.DataBind();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            GetUnits();
        }

        protected void SrcUnitIdHdn_ServerChange(object sender, EventArgs e)
        {
            GetUnits();
        }

        /// <summary>
        /// The method calls EditPasCode page.
        /// </summary>
        protected void UnitGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUnit")
            {
                Session["EditId"] = e.CommandArgument;
                Response.Redirect("~/Secure/Shared/Admin/EditPasCode.aspx?csId=" + e.CommandArgument, true);
            }
        }
    }
}
