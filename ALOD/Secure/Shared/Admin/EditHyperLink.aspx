<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" ValidateRequest="false" Inherits="ALOD.Web.Admin.EditHyperLink" Codebehind="EditHyperLink.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
    <style type="text/css">
        .hyperContent
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
        
        <asp:Panel runat="server" ID="pnlHyperLinks" CssClass="dataBlock">
            <div class="dataBlock-header">
                Hyper Links
            </div>

            <div class="dataBlock-body">
                <asp:Label runat="server" ID="lblLinkToParameterMessage" Font-Bold="true">* To insert a hyper link into the content place the reference name of a link inside curly brackets. For instance: {LINK_1}</asp:Label>
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
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="btnEditHyperLink" CommandName="EditLink" Text="Edit" CommandArgument='<%# Container.DataItemIndex.ToString()%>' />
                                <asp:LinkButton runat="server" ID="btnDeleteHyperLink" CommandName="DeleteLink" Text="Delete" CommandArgument='<%# Container.DataItemIndex.ToString()%>' />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddHyperLink" Text="Add Link" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAddHyperLink" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                Add New Hyper Link
            </div>

            <div class="dataBlock-body">
                <table style="width:100%;">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lbl1" Text="Reference Name:" />
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtAddLinkName" MaxLength="50" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Type:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlAddHyperLinkType" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Display Text:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtAddDisplayText" MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Value:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtAddWebsiteLink" MaxLength="250" Visible="true" />
                            <asp:DropDownList runat="server" ID="ddlAddHelpDocuments" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Hyper Enabled:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnAddLink" Text="Add" />
                            <asp:Button runat="server" ID="btnCancelAddLink" Text="Cancel" />
                        </td>
                    </tr>
                    <tr runat="server" id="trAddValidationErrors" visible="false">
                        <td class="number">
                            
                        </td>
                        <td class="label">
                            Errors:
                        </td>
                        <td class="value">
                            <asp:BulletedList runat="server" ID="bltAddErrors" CssClass="labelRequired" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlEditHyperLink" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                Edit Hyper Link
            </div>

            <div class="dataBlock-body">
                <table style="width:100%;">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Reference Name:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtEditLinkName" MaxLength="50" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Type:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlEditHyperLinkType" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Display Text:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtEditDisplayText" MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Value:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtEditWebsiteLink" MaxLength="250" Visible="true" />
                            <asp:DropDownList runat="server" ID="ddlEditHelpDocuments" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Hyper Enabled:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnEditLink" Text="Edit" />
                            <asp:Button runat="server" ID="btnCancelEditLink" Text="Cancel" />
                        </td>
                    </tr>
                    <tr runat="server" id="trEditValidationErrors" visible="false">
                        <td class="number">
                            
                        </td>
                        <td class="label">
                            Errors:
                        </td>
                        <td class="value">
                            <asp:BulletedList runat="server" ID="bltEditErrors" CssClass="labelRequired" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        $(function () {
            var block = element('<%=pnlAddHyperLink.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }

            block = element('<%=pnlEditHyperLink.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
        });
    </script>
</asp:Content>