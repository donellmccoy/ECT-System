<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_CMAS/SC_CMAS.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.CMAS.Secure_sc_cm_HQTech" Codebehind="HQTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_CMAS/SC_CMAS.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label runat="server" ID="lblHeader" Text="HQ AFRC Technician - CMAS" />
        </div>
        <div><br />
            <table style="border-spacing: 5px; width: 100%;">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblDateIn" CssClass="label labelRequired" LabelFor="DateInTB" Text="Date In" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="DateInTB" MaxLength="10" onchange="DateCheck(this);" Width="80" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblDateOut" CssClass="label labelRequired" LabelFor="DateOutTB" Text="Date Out" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="DateOutTB" MaxLength="10" onchange="DateCheck(this);" Width="80" Text="" />
                    </td>
                </tr>
                <tr>
                    <td style="width:100px;">
                        <asp:Label runat="server" ID="lblComments" LabelFor="CommentsTB" Text="Comments" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="CommentsTB" Text="" 
                            TextMode="MultiLine" Width="90%" MaxLength="600" Rows="3" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">


        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

            
    </script>
</asp:Content>
