<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditPHFields" Codebehind="EditPHFields.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        (function() {
            var block = element('<%=pnlAddNewField.ClientID%>');
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
                1 - PH Field Names
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvPHFields" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id" AllowPaging="true" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="350px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddNewField" Text="Add New PH Field Name" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAddNewField" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Add New PH Field Name
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblAddFieldName" Text="Name:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAddPHField" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnAddPHField" Text="Add" />&nbsp;
                <asp:Button runat="server" ID="btnCancelAddPHField" Text="Cancel" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>