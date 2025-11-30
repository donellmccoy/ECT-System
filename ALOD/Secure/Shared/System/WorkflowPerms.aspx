<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_WorkflowPerms" Codebehind="WorkflowPerms.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div style="text-align: left;">
        Workflow: <asp:Label ID="lblWorkflow" runat="server" Font-Bold="True"></asp:Label>
        &nbsp; Compo: <asp:DropDownList ID="cbCompo" runat="server" AutoPostBack="True">  
            <asp:ListItem Value="6">Air Force Reserve</asp:ListItem>
        <asp:ListItem Value="5" Selected="True">Air National Guard</asp:ListItem>
        </asp:DropDownList><br />
        <br />

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="dataPerms"
            DataKeyNames="GroupId" EnableModelValidation="True">
            <Columns>
                <asp:BoundField DataField="GroupName" HeaderText="GroupName" SortExpression="GroupName">
                    <ItemStyle Width="300px" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Create" SortExpression="CanCreate">
                    <EditItemTemplate>
                        <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("CanCreate") %>' />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbCreate" runat="server" Checked='<%# Bind("CanCreate") %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="View" SortExpression="CanView">
                    <EditItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("CanView") %>' />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbView" runat="server" Checked='<%# Bind("CanView") %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </asp:TemplateField>
            </Columns>
            <RowStyle HorizontalAlign="Left" />
        </asp:GridView>
        <br />
        <div style="padding-left: 400px;">
            <asp:Button ID="Button1" runat="server" Text="Update" />
            <asp:Button runat="server" ID="btnCancel" Text="Cancel" />
        </div>
    </div>
    <!--<asp:ObjectDataSource ID="dataCompo" runat="server" SelectMethod="GetUserCompoAsDataSet"
        TypeName="LookUp"></asp:ObjectDataSource>-->
    <asp:ObjectDataSource ID="dataPerms" runat="server" SelectMethod="GetByWorkflowAndCompo"
        TypeName="ALODWebUtility.Worklfow.WorkflowPermissionList">
        <SelectParameters>
            <asp:QueryStringParameter Name="workflowId" QueryStringField="id" Type="Byte" />
            <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
