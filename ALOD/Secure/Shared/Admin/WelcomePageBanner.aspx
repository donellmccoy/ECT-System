<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" ValidateRequest="false" Inherits="ALOD.Web.Admin.WelcomePageBanner" Codebehind="WelcomePageBanner.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
    <style type="text/css">
        .bannerContent
        {
            width:100%;
        }

        .visuallyHidden
        {
            border:0;
            clip:rect(0 0 0 0);
            height:1px;
            width:1px;
            margin:1px;
            overflow:hidden;
            padding:0;
            position:absolute;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlBannerConfiguration" CssClass="dataBlock">
            <div class="dataBlock-header">
                Configure Welcome Banner
            </div>

            <div class="dataBlock-body">
                <table style="width:100%;">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Banner Enabled:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkBannerEnabled" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Banner Content:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtBannerContent" TextMode="MultiLine" Rows="5" MaxLength="1000" CssClass="bannerContent" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnSubmit" Text="Save" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlHyperLinks" CssClass="dataBlock">
            <div class="dataBlock-header">
                Hyper Links
            </div>

            <div class="dataBlock-body">
                <asp:Label runat="server" ID="lblLinkToParameterMessage" Font-Bold="true">* To insert a hyper link into the banner content place the reference name of a link inside curly brackets. For instance: {LINK_1}</asp:Label>
                <br />
                <asp:GridView runat="server" ID="gdvHyperLinks" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id" AllowPaging="true" PageSize="10">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Reference Name" ReadOnly="True" ItemStyle-Width="10%" />
                        <asp:BoundField DataField="Type.Name" HeaderText="Type" ReadOnly="True" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="DisplayText" HeaderText="Display Text" ReadOnly="True" ItemStyle-Width="30%" />
                        <asp:TemplateField HeaderText="Value">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblLinkValue" />
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        
                    </Columns>
                </asp:GridView>
                <br />
            </div>
        </asp:Panel>

        
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        $(function () {
            
        });
    </script>
</asp:Content>