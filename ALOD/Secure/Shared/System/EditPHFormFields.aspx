<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditPHFormFields" Codebehind="EditPHFormFields.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        (function() {
            var block = element('<%=pnlAddNewFormField.ClientID%>');
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
                1 - PH Form Fields
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblFilters" Text="Filters:" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlSectionFilter" Width="350px" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlFieldFilter" Width="350px" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlFieldTypeFilter" Width="350px" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:GridView runat="server" ID="gdvPHFormFields" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="20">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdfSectionId" Value='<%# DataBinder.Eval(Container.DataItem, "Section.Id")%>' />
                                <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Field.Id")%>' />
                                <asp:HiddenField runat="server" ID="hdfFieldTypeId" Value='<%# DataBinder.Eval(Container.DataItem, "FieldType.Id")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Section">
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlSections"></asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSectionName"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Field Name">
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlFields"></asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblFieldName"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Field Type">
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlFieldTypes"></asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblFieldTypeName"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="btnEditFormField" CommandName="EditFormField" Text="Edit" CommandArgument='<%# Container.DataItemIndex.ToString()%>' />
                                <asp:LinkButton runat="server" ID="btnDeleteFormField" CommandName="DeleteFormField" Text="Delete" CommandArgument='<%# Container.DataItemIndex.ToString()%>' />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnAddNewFormField" Text="Add New PH Form Field" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAddNewFormField" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Add New PH Form Field
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblSection" Text="Select Section:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlSection" Width="350px" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblField" Text="Select Field Name:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlField" Width="350px" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblFieldDisplayOrder" Text="Display Order:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtFieldDisplayOrder" Width="50px" />
                            &nbsp;
                            (Must be greater than 0)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblFieldType" Text="Select Field Type:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlFieldType" Width="350px" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblFieldTypeDisplayOrder" Text="Display Order:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtFieldTypeDisplayOrder" Width="50px" />
                            &nbsp;
                            (Must be greater than 0)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblToolTip" Text="ToolTip (Optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtToolTip" Width="346px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnAddPHFormField" Text="Add" />&nbsp;
                <asp:Button runat="server" ID="btnCancelAddPHFormField" Text="Cancel" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlEditFormField" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Edit PH Form Field
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditSection" Text="Select Section:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlEditSection" Width="350px" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditField" Text="Select Field Name:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlEditField" Width="350px" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblEditFieldDisplayOrder" Text="Display Order:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditFieldDisplayOrder" Width="50px" />
                            &nbsp;
                            (Must be greater than 0)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditFieldType" Text="Select Field Type:" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlEditFieldType" Width="350px" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblEditFieldTypeDisplayOrder" Text="Display Order:" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditFieldTypeDisplayOrder" Width="50px" />
                            &nbsp;
                            (Must be greater than 0)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblEditToolTip" Text="ToolTip (Optional):" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEditToolTip" Width="346px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button runat="server" ID="btnEditFormField" Text="Submit" />&nbsp;
                <asp:Button runat="server" ID="btnCancelEditFormField" Text="Cancel" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>