<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_MemoTemplates" ValidateRequest="false" Codebehind="MemoTemplates.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        $(document).ready(function() {
            var block = element('<%=RolePanel.ClientId%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }

            block = element('<%=EditPanel.ClientId %>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <p style="text-align: right;">
            <asp:Button runat="server" ID="CreateTemplateButton" Text="Create New Memo" />
        </p>
        <div class="">
            <asp:GridView runat="server" ID="TemplateGrid" AutoGenerateColumns="False" Width="100%"
                DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Title" ReadOnly="true" />
                    <asp:BoundField DataField="Active" HeaderText="Active" ReadOnly="true" />
                    <asp:BoundField DataField="AddSignature" HeaderText="Signature" ReadOnly="true" />
                    <asp:BoundField DataField="AddDate" HeaderText="Add Date" ReadOnly="true" />
                    <asp:BoundField DataField="AddSuspenseDate" HeaderText="Add Suspense" ReadOnly="true" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="EditRoles" CommandName="EditRoles" Text="Permissions"
                                CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            &nbsp;
                            <asp:LinkButton runat="server" ID="EditTemplate" CommandName="EditTemplate" Text="Edit"
                                CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    No Templates Found
                </EmptyDataTemplate>
                <EmptyDataRowStyle CssClass='emptyItem' />
            </asp:GridView>
        </div>
        <p>
            &nbsp;</p>
        <asp:Panel runat="server" ID="RolePanel" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                1 - Edit Role Permissions
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Roles:
                        </td>
                        <td class="value" style="width: 550px;">
                            <asp:GridView runat="server" ID="RolesGrid" AutoGenerateColumns="false" Width="100%"
                                DataKeyNames="groupId">
                                <Columns>
                                    <asp:BoundField DataField="name" HeaderText="Role" ReadOnly="true" />
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="CanView" Checked='<%# Eval("CanView") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Create">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="CanCreate" Checked='<%# Eval("CanCreate") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="CanEdit" Checked='<%# Eval("CanEdit") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="CanDelete" Checked='<%# Eval("CanDelete") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="UpdateRolesButton" Text="Save" Width="60px" />&nbsp;
                            <asp:Button runat="server" ID="CancelRolesButton" Text="Cancel" Width="60px" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="EditPanel" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                1 -
                <asp:Label runat="server" ID="EditTitle" Text="Edit Template" />
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label labelRequired">
                            * Title:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="MemoTitle" MaxLength="100" Width="656px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Data Source:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="SourceSelect" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Module:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ModuleSelect">
                                <asp:ListItem Value="1">None</asp:ListItem>
                                <asp:ListItem Value="2">Line of Duty</asp:ListItem>
                                <asp:ListItem Value="10">Participation Waiver</asp:ListItem>
                                <asp:ListItem Value="11">MEB</asp:ListItem>
                                <asp:ListItem Value="13">IRILO</asp:ListItem>
                                <asp:ListItem Value="9">WWD</asp:ListItem>
                                <asp:ListItem Value="22">RS</asp:ListItem>
                                <asp:ListItem Value="27">RW</asp:ListItem>
                                <asp:ListItem Value="30">PSC Determination</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Active:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="Active" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Add Date:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="AddDate" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label">
                            Add Suspense Date:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="AddSuspenseDate" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label">
                            Add Signature:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="AddSignature" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label">
                            Signature Block:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="SignatureBlock" MaxLength="200" TextMode="MultiLine"
                                Rows="4" Columns="80" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label">
                            Body:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="MemoBody" TextMode="MultiLine" Columns="80" Rows="30" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            J
                        </td>
                        <td class="label">
                            Attachments:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="MemoAttachments" TextMode="MultiLine" Columns="80" Rows="5" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="UpdateTemplateButton" Text="Save" Width="60px" />&nbsp;
                            <asp:Button runat="server" ID="CancelEditButton" Text="Cancel" Width="60px" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
