<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditPHSections" Codebehind="EditPHSections.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        (function() {
            var block = element('<%=pnlAddNewSection.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }

            block = element('<%=pnlAddNewMainSection.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
            
            block = element('<%=pnlSubSectionMappings.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlMainSections" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - PH Main Sections
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvPHMainSections" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="3%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="350px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="43%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Columns">
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlFieldColumns" SelectedValue='<%# Bind("FieldColumns")%>'>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFieldColumns" runat="server" Text='<%# Bind("FieldColumns") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Display Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDisplayOrder" runat="server" MaxLength="10" Width="25px" Text='<%# Bind("DisplayOrder") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Bind("DisplayOrder")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="14%" />
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="HasPageBreak" HeaderText="Page Break" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditSubSections" CommandName="EditSubSections" Text="Sub-Sections" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="10%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddNewMainSection" Text="Add New PH Main Section" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlSubSections" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - PH Sub Sections
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvPHSubSections" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="350px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="45%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Columns">
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlFieldColumns" SelectedValue='<%# Bind("FieldColumns")%>'>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFieldColumns" runat="server" Text='<%# Bind("FieldColumns") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditSubSections" CommandName="EditSubSections" Text="Sub-Sections" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="10%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddNewSubSection" Text="Add New PH Sub Section" />
            </div>
        </asp:Panel>



        <asp:Panel runat="server" ID="pnlAddNewMainSection" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Add New PH Main Section
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblMainAddSectionName" Text="Name:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtMainAddPHSection" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblMainFieldColumns" Text="Columns:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtMainFieldColumns" MaxLength="1" Width="25px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblMainFieldHasPageBreak" Text="Page Break:" />
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkHasPageBreak" Checked="false" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnAddMainPHSection" Text="Add" />&nbsp;
                <asp:Button runat="server" ID="btnCancelAddMainPHSection" Text="Cancel" />
            </div>
        </asp:Panel>




        <asp:Panel runat="server" ID="pnlAddNewSection" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Add New PH Sub Section
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblAddSectionName" Text="Name:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAddPHSection" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblFieldColumns" Text="Columns:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtFieldColumns" MaxLength="1" Width="25px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblAddSectionParent" Text="Parent Section:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlParentSections" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnAddPHSection" Text="Add" />&nbsp;
                <asp:Button runat="server" ID="btnCancelAddPHSection" Text="Cancel" />
            </div>
        </asp:Panel>

        <asp:Panel runat ="server" ID="pnlSubSectionMappings" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Assigned Sub Sections
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvSubSectionMappings" AutoGenerateColumns="False" Width="100%" OnRowDeleting="gdvSubSectionMappings_RowDeleting" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="45%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Display Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDisplayOrder" runat="server" MaxLength="10" Width="25px" Text='<%# Bind("DisplayOrder") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Bind("DisplayOrder")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" ShowEditButton="True" ShowDeleteButton="True" ItemStyle-Width="20%" />
                    </Columns>
                </asp:GridView>
                <br />
                <table>
                    <tr>
                        <td>
                            Select Sub Section:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlSubSections" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Enter Display Order:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtSubSectionDisplayOrder" Width="25px" MaxLength="10" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Action:
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnAddSubSectionMapping" Text="Add" Width="60px" />&nbsp;
                            <asp:Button runat="server" ID="btnCancelSubSectionMapping" Text="Cancel" Width="60px" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
</asp:Content>