<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_StatusCodes" Codebehind="StatusCodes.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    Module:
    <asp:DropDownList runat="server" ID="ddlModule" AutoPostBack="True">
    </asp:DropDownList>
    <br />
    <br />
    <asp:GridView ID="gvCodes" runat="server" AutoGenerateColumns="False" DataSourceID="dataCodes"
        AllowSorting="True" DataKeyNames="statusId">
        <Columns>
            <asp:BoundField DataField="displayOrder" HeaderText="Order" SortExpression="displayOrder" />
            <asp:BoundField DataField="statusId" ReadOnly="true" HeaderText="statusId" SortExpression="statusId" />
            <asp:TemplateField HeaderText="Description" SortExpression="description">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("description") %>' Width="240px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox1"
                        ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="300px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Module" SortExpression="moduleName">
                <EditItemTemplate>
                    &nbsp;<asp:DropDownList ID="DropDownList1" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("moduleName") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="130px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User Group" SortExpression="groupName">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="dataGroups" DataTextField="text"
                        DataValueField="value" SelectedValue='<%# Bind("groupId") %>' AppendDataBoundItems="True">
                        <asp:ListItem Selected="True" Value="0">None</asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("groupName") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="220px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Final" SortExpression="isFinal">
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("isFinal") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("isFinal") %>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Approved" SortExpression="isApproved">
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("isApproved") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("isApproved") %>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Appeal" SortExpression="canAppeal">
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("canAppeal") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("canAppeal") %>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Disposition" SortExpression="isDisposition">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkIsDisposition" runat="server" Checked='<%# Bind("isDisposition")%>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkIsDisposition" runat="server" Checked='<%# Bind("isDisposition")%>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Formal" SortExpression="isFormal">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkIsFormal" runat="server" Checked='<%# Bind("isFormal")%>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkIsFormal" runat="server" Checked='<%# Bind("isFormal")%>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                        Text="Update"></asp:LinkButton>
                    <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                        Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"
                        Text="Edit" ValidationGroup="edit"></asp:LinkButton>
                    <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Delete"
                        Text="Delete"></asp:LinkButton>
                    <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure you want to delete this Status Code?"
                        TargetControlID="LinkButton2">
                    </ajax:ConfirmButtonExtender>
                </ItemTemplate>
                <ItemStyle Width="110px" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <h2>
        Add New Status Code:</h2>
    <table>
        <tr>
            <td style="width: 150px; text-align:right;">
                Description:
            </td>
            <td style="width: 180px">
                <asp:TextBox ID="txtDescr" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescr"
                    ErrorMessage="Description is required" ValidationGroup="insert">*</asp:RequiredFieldValidator>
            </td>
            <td style="width: 100px; text-align:right;">
                Is Final:
            </td>
            <td style="width: 180px">
                <asp:CheckBox ID="cbFinal" runat="server" />
            </td>
            <td style="width: 100px; text-align:right;">
                Is Disposition:
            </td>
            <td style="width: 180px">
                <asp:CheckBox ID="cbDisposition" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 150px; text-align:right;">
                User Group:
            </td>
            <td style="width: 180px;">
                <asp:DropDownList ID="cbAddGroup" runat="server" DataSourceID="dataGroups" DataTextField="text"
                    DataValueField="value">
                </asp:DropDownList>
            </td>
            <td style="width: 100px; text-align: right;">
                Is Approved:
            </td>
            <td style="width: 180px">
                <asp:CheckBox ID="cbApproved" runat="server" />
            </td>
            <td style="width: 100px; text-align:right;">
                Is Formal:
            </td>
            <td style="width: 180px">
                <asp:CheckBox ID="cbFormal" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 150px; text-align:right;">
                Module:
            </td>
            <td style="width: 180px">
                <asp:DropDownList ID="ddlModule2" runat="server">
                </asp:DropDownList>
            </td>
            <td style="width: 100px; text-align:right;">
                Can Appeal:
            </td>
            <td style="width: 180px">
                <asp:CheckBox ID="cbAppeal" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 150px; text-align:right;">
            </td>
            <td style="width: 180px">
            </td>
            <td style="width: 100px; text-align:right;">
            </td>
            <td style="width: 180px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td style="width: 150px; text-align:right;">
            </td>
            <td style="width: 180px">
            </td>
            <td style="width: 100px; text-align:right;">
            </td>
            <td style="width: 180px; text-align:left;">
                <asp:Button ID="btnAdd" runat="server" Text="Add Code" ValidationGroup="insert" />
            </td>
        </tr>
    </table>
    <br />
    <br />
    <asp:ObjectDataSource ID="dataCodes" runat="server" TypeName="ALODWebUtility.Worklfow.StatusCodeList" SelectMethod="GetByCompoAndModuleAsDataSet"
        DeleteMethod="DeleteStatusCode" UpdateMethod="UpdateStatusCode">
        <SelectParameters>
            <asp:SessionParameter Name="compo" SessionField="Compo" />
            <asp:ControlParameter ControlID="ddlModule" Name="type" PropertyName="SelectedValue"
                Type="Byte" />
        </SelectParameters>
        <DeleteParameters>
            <asp:Parameter Name="statusId" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="statusId" Type="Int32" />
            <asp:Parameter Name="description" Type="String" />
            <asp:Parameter Name="moduleId" Type="Object" />
            <asp:Parameter Name="groupId" Type="Byte" />
            <asp:Parameter Name="isFinal" Type="Boolean" />
            <asp:Parameter Name="isApproved" Type="Boolean" />
            <asp:Parameter Name="canAppeal" Type="Boolean" />
            <asp:Parameter Name="displayOrder" Type="Byte" />
            <asp:Parameter Name="isDisposition" Type="Boolean" />
            <asp:Parameter Name="isFormal" Type="Boolean" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dataGroups" runat="server" SelectMethod="GetGroupsByCompo"
        TypeName="ALODWebUtility.LookUps.LookUp">
        <SelectParameters>
            <asp:SessionParameter Name="compo" SessionField="Compo" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
