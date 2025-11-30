<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditPHFieldTypes" Codebehind="EditPHFieldTypes.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        (function() {
            var block = element('<%=pnlAddNewFieldType.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlFields" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - PH Field Types
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvPHFieldTypes" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id" AllowPaging="true" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblName" Text='<%# Bind("Name") %>' />
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Type">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataType" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Max Length">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMaxLength" Text='<%# Bind("Length") %>' />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="btnEditFieldType" CommandName="EditFieldType" Text="Edit" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString()%>' />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddNewFieldType" Text="Add New PH Field Type" Visible="true" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAddNewFieldType" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Add New PH Field Type
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblAddFieldTypeName" Text="Name:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAddPHFieldType" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblDataTypeName" Text="Data Type:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlDataTypes" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPlaceholder" Text="Placeholder (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtPlaceholder" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblDataSource" Text="Datasource (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtDataSource" MaxLength="100" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblColor" Text="Color (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtColor" MaxLength="100" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblMaxLength" Text="Max Length (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtMaxLength" MaxLength="6" Width="75px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnAddPHFieldType" Text="Add" />&nbsp;
                <asp:Button runat="server" ID="btnCancelAddPHFieldType" Text="Cancel" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlEditFieldType" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Edit PH Field Type
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblFieldTypeName" Text="Name:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditFieldTypeName" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditDataType" Text="Data Type:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlEditDataTypes" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditPlaceholder" Text="Placeholder (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditPlaceholder" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditDatasource" Text="Datasource (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditDataSource" MaxLength="100" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditColor" Text="Color (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditColor" MaxLength="6" Width="75px" />
                            &nbsp;
                            (Hex format)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditMaxLength" Text="Max Length (optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditMaxLength" MaxLength="6" Width="75px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnEdit" Text="Edit" />&nbsp;
                <asp:Button runat="server" ID="btnCancelEdit" Text="Cancel" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>