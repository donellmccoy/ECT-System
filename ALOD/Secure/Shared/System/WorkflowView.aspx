<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_WorkflowView" Codebehind="WorkflowView.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div>
                    Select Workflow:
                    <asp:DropDownList ID="ddlWorkflow" runat="server" DataSourceID="workflow" DataTextField="FullTitle"
                        DataValueField="Id" AutoPostBack="True">
                    </asp:DropDownList>
                </div>
                <br />
               
                <div>
                    <asp:GridView ID="grdPageLists" runat="server" AutoGenerateColumns="False" DataSourceID="fillGrid"
                        EmptyDataText="No Record Found" DataKeyNames="PageId">
                        <Columns>
                            <asp:TemplateField HeaderText="Page">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("title") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="200px" />
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" OnClientClick="return confirm('Delete Record?');" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:GridView>
                </div>
                <br />
                <div>
                    <asp:ObjectDataSource ID="workflow" runat="server" SelectMethod="GetByCompo"
                        TypeName="ALODWebUtility.Worklfow.WorkFlowList">
                        
                     <SelectParameters>
                        <asp:Parameter Name="compo" Type="String" />
                    </SelectParameters>    
                        
                        
                    </asp:ObjectDataSource>
                </div>
                <asp:ObjectDataSource ID="pageObject" runat="server" SelectMethod="GetAllPageTitles"
                    TypeName="ALODWebUtility.PageTitles.PageTitleList"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="fillGrid" runat="server" SelectMethod="GetPagesAsDataSet"
                    TypeName="ALODWebUtility.Worklfow.WorkflowViewList" DeleteMethod="DeleteWorkflowView">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlWorkflow" DefaultValue="-1" Name="workflowId"
                            PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="pageId" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlWorkflow" DefaultValue="-1" Name="workflowId"
                            PropertyName="SelectedValue" Type="Int32" />
                    </DeleteParameters>
                </asp:ObjectDataSource>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
