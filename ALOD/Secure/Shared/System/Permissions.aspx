<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_Permissions" Codebehind="Permissions.aspx.vb" %>


<%@ Register Src="../UserControls/FeedbackPanel.ascx" TagName="FeedbackPanel" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <h2>
        Edit Permissions
    </h2>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
			<uc1:FeedbackPanel ID="FeedbackPanel1" runat="server" />
            <asp:GridView ID="gvAllPerms" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource1"
                DataKeyNames="Id" AllowSorting="True" OnRowUpdated="PermRowUpdated" 
                PageSize="20">
                <Columns>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" MaxLength="50" Text='<%# Bind("Name") %>' Width="380px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox1"
                                ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="425px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="Description">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" MaxLength="100" Text='<%# Bind("Description") %>' Width="380px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox2"
                                ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="425px" />
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="Exclude" HeaderText="Exclude" />
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                                Text="Update" ValidationGroup="edit"></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"
                                Text="Edit"></asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete"></asp:LinkButton>
                            <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure you want to delete this permission?"
                                TargetControlID="btnDelete">
                            </ajax:ConfirmButtonExtender>
                        </ItemTemplate>
                        <ItemStyle Width="90px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="DetailsView1" EventName="ItemInserted" />
        </Triggers>
    </asp:UpdatePanel>
    <br />
    <br />
    <h2>
        Add New Permission</h2>
    <p>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="ObjectDataSource1" DefaultMode="Insert" CellPadding="2" CellSpacing="2" GridLines="None">
            <Fields>
                <asp:TemplateField HeaderText="Name:" SortExpression="Name">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="50" Text='<%# Bind("Name") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1"
                            ErrorMessage="Name is required" ValidationGroup="insert">*</asp:RequiredFieldValidator>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Description:" SortExpression="Description">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" MaxLength="100" Text='<%# Bind("Description") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2"
                            ErrorMessage="Description is required" ValidationGroup="insert">*</asp:RequiredFieldValidator>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <InsertItemTemplate>
                        <asp:Button ID="Button1" runat="server" CausesValidation="True" CommandName="Insert"
                            Text="Insert" Width="70px" ValidationGroup="insert" OnClick="InsertPermission" />&nbsp;
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="insert" />
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Button ID="Button1" runat="server" CausesValidation="False" CommandName="New"
                            Text="New" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Fields>
        </asp:DetailsView>
    </p>
	<ajax:UpdatePanelAnimationExtender ID="animator" runat="server" TargetControlID="UpdatePanel1">
		<Animations>
                <OnUpdating>
                    <ScriptAction script="hidePanels();" />
                </OnUpdating>
                <OnUpdated>
                    <ScriptAction script="updateFeedbackPanels();" />
                </OnUpdated></Animations>
	</ajax:UpdatePanelAnimationExtender>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAll"
        TypeName="ALODWebUtility.Permission.PermissionList" UpdateMethod="UpdatePermission" DeleteMethod="DeletePermission"
        InsertMethod="InsertPermission">
        <UpdateParameters>
            <asp:Parameter Name="id" Type="Int32" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="description" Type="String" />
            <asp:Parameter Name="exclude" Type="Boolean" />
        </UpdateParameters>
        <DeleteParameters>
            <asp:Parameter Name="id" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="description" Type="String" />
            <asp:Parameter Name="exclude" Type="Boolean" />
        </InsertParameters>
    </asp:ObjectDataSource>
    &nbsp; &nbsp;
	<br />
</asp:Content>

