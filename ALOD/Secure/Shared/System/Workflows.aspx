<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_Workflows" Codebehind="Workflows.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    Compo:
    <asp:DropDownList ID="cbCompo" runat="server" AutoPostBack="True">
        <asp:ListItem Value="6">Air Force Reserve</asp:ListItem>
        <asp:ListItem Value="5">Air National Guard</asp:ListItem>
    </asp:DropDownList>
    &nbsp; Module:
    <asp:DropDownList ID="ddlModule" runat="server" AutoPostBack="True">
    </asp:DropDownList>
    <br />
    <h2>
        Current Workflows</h2>
    <asp:GridView ID="gvWorkflows" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
        DataSourceID="dataWorkflows" HorizontalAlign="Center" Width="900px">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id" />
            <asp:TemplateField HeaderText="Title" SortExpression="Title">
                <EditItemTemplate>
                    &nbsp;<asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Title") %>' Width="182px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox1"
                        ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Title") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="300px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Module" SortExpression="ModuleName">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>                
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("ModuleName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="dataStatus" DataTextField="FullDescription"
                        DataValueField="StatusId">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("StatusDescription") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Formal" SortExpression="IsFormal">
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("IsFormal") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("IsFormal") %>' Enabled="false" />
                    <ajax:ToggleButtonExtender ID="ToggleButtonExtender1" runat="server" CheckedImageAlternateText="Formal"
                        CheckedImageUrl="~/images/check.gif" ImageHeight="16" ImageWidth="16" TargetControlID="CheckBox1"
                        UncheckedImageAlternateText="Informal" UncheckedImageUrl="~/images/trans.gif">
                    </ajax:ToggleButtonExtender>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Active" SortExpression="Active">
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("Active") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("Active") %>' Enabled="false" />
                    <ajax:ToggleButtonExtender ID="ToggleButtonExtender2" runat="server" CheckedImageAlternateText="Active"
                        CheckedImageUrl="~/images/Check.gif" DisabledCheckedImageUrl="~/images/check.gif"
                        DisabledUncheckedImageUrl="~/images/trans.gif" ImageHeight="16" ImageWidth="16"
                        TargetControlID="CheckBox2" UncheckedImageAlternateText="Not Active" UncheckedImageUrl="~/images/trans.gif">
                    </ajax:ToggleButtonExtender>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    &nbsp;
                    <asp:LinkButton ID="lnkSteps" runat="server" CommandArgument='<%# Eval("Id") %>'
                        CommandName="EditSteps" OnCommand="WorkflowCommand">Steps</asp:LinkButton>
                    <asp:LinkButton ID="lnkEditPerms" runat="server" CommandArgument='<%# Eval("Id") %>'
                        CommandName="EditPerms" OnCommand="WorkflowCommand" Visible="False">Permissions</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                        Text="Update" ValidationGroup="edit"></asp:LinkButton>
                    <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                        Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"
                        Text="Edit"></asp:LinkButton>
                    &nbsp;
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:ObjectDataSource ID="dataWorkflows" runat="server" SelectMethod="GetByCompoAndModule"
        TypeName="ALODWebUtility.Worklfow.WorkFlowList" UpdateMethod="UpdateWorkflow">
        <UpdateParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="moduleId" Type="Byte" />
            <asp:Parameter Name="isFormal" Type="Boolean" />
            <asp:Parameter Name="id" Type="Byte" />
            <asp:Parameter Name="active" Type="Boolean" />
            <asp:Parameter Name="initialStatus" Type="Int32" />
        </UpdateParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="ddlModule" Name="type" PropertyName="SelectedValue"
                Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dataStatus" runat="server" SelectMethod="GetByCompoAndModule"
        TypeName="ALODWebUtility.Worklfow.StatusCodeList">
        <SelectParameters>
            <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="ddlModule" Name="type" PropertyName="SelectedValue"
                Type="Byte" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
