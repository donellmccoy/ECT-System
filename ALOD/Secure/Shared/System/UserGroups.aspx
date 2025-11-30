<%@ Page Title="UserGroups" Language="VB" MasterPageFile="~/Secure/Secure.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_UserGroups" Codebehind="UserGroups.aspx.vb" %>

<%@ Register Src="../UserControls/FeedbackPanel.ascx" TagName="FeedbackPanel" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <uc1:FeedbackPanel ID="FeedbackPanel1" runat="server" />
      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="GroupsView" runat="server" PageSize="20" AllowSorting="True" Width="98%"
                HorizontalAlign="Center" AutoGenerateColumns="False" DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="true" SortExpression="Id" />
                    <asp:TemplateField HeaderText="Name" SortExpression="Description">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Description") %>' Visible="true"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescription"
                                ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Abbr" SortExpression="Abbreviation">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtAbbreviation" runat="server" MaxLength="10" Text='<%# Bind("Abbreviation") %>'
                                Width="40px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtAbbreviation"
                                ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("Abbreviation") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="60px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Compo" SortExpression="Component">
                        <EditItemTemplate>
                            &nbsp;<asp:DropDownList ID="cbCompo" runat="server" SelectedValue='<%# Bind("Component") %>'>
                                <asp:ListItem Value="6" Text="Air Force Reserve"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Air National Guard"></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Component") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="60px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order" SortExpression="SortOrder">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtSortOrder" runat="server" MaxLength="3" Text='<%# Bind("SortOrder") %>'
                                Width="40px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSortOrder"
                                ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtSortOrder" MaximumValue="255" MinimumValue="0" Type="Integer"></asp:RangeValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("SortOrder") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Scope" SortExpression="Scope">
                        <EditItemTemplate>
                            <asp:DropDownList ID="cbScope" runat="server" >
                                <asp:ListItem Text="Unit" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Compo" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("Scope") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="60px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Level" SortExpression="GroupLevel.Name">
                        <EditItemTemplate>
                            <asp:DropDownList runat="server" ID="ddlGridViewUserGroupLevels"/>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblGroupLevel" Text='<%# Eval("GroupLevel.Name") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ReportView" SortExpression="ReportView">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ReportViewList" runat="server" DataSourceID="DataReportView" DataTextField="Name" DataValueField="Value" >
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="ReportView" runat="server" Text='<%#  ALOD.Data.Services.WorkFlowService.ViewDescription (Eval("ReportView")) %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="250px" />
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                                Text="Update" ValidationGroup="edit"></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Edit"
                                Text="Edit"></asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete"></asp:LinkButton>
                            <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure want to delete this group?"
                                TargetControlID="LinkButton2">
                            </ajax:ConfirmButtonExtender>
                        </ItemTemplate>
                        <ItemStyle Width="100px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <ajax:UpdatePanelAnimationExtender ID="animator" runat="server" TargetControlID="UpdatePanel1">
        <Animations>
                <OnUpdating>
                    <ScriptAction script="hidePanels();" />
                </OnUpdating>
                <OnUpdated>
                    <ScriptAction script="updateFeedbackPanels();" />
                </OnUpdated></Animations>
    </ajax:UpdatePanelAnimationExtender>
      <asp:ObjectDataSource ID="DataReportView" runat="server" SelectMethod="GetChainType"
        TypeName="ALOD.Data.Services.LookupService">
    </asp:ObjectDataSource>
    <br />
    <br />
    <h2>
        Add New Group</h2>
    <div style="padding-left: 20px">
        <table>
            <tr>
                <td>
                    Description:
                </td>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Abbreviation:
                </td>
                <td>
                    <asp:TextBox ID="txtAbbreviation" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Component:
                </td>
                <td>
                    <asp:DropDownList ID="cbCompo" runat="server">
                        <asp:ListItem Value="6" Text="Air Force Reserve"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Air National Guard"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Order:
                </td>
                <td>
                    <asp:TextBox ID="txtSortOrder" runat="server"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtSortOrder" MaximumValue="255" MinimumValue="0" Type="Integer"></asp:RangeValidator>
                </td>
            </tr>
            <tr>
                <td>
                    Scope
                </td>
                <td>
                    <asp:DropDownList ID="cbScope" runat="server">
                        <asp:ListItem Text="Unit" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Compo" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Group Level
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlUserGroupLevels"/>
                </td>
            </tr>
            <tr>
                <td>Report View
                </td>
                <td>
                    <asp:DropDownList ID="ReportViewList" runat="server" DataSourceID="DataReportView" DataTextField="Name" DataValueField="Value">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="height: 30px; vertical-align: bottom">
                    <asp:Button runat="server" ID="cmdSubmit" Text="Submit" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
