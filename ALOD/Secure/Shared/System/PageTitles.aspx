<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_PageTitles" Codebehind="PageTitles.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div>
                <asp:GridView ID="GridView1" runat="server" PageSize="16" AutoGenerateColumns="False" DataSourceID="fillGrid" EmptyDataText="No Record Found" DataKeyNames="pageId">
                    <Columns>
                        <asp:TemplateField HeaderText="Page ID">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("pageId") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Page Title">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("title") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="300px" />
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" />
                    </Columns>
                </asp:GridView>
                &nbsp;
                <br /><br />
                <asp:ObjectDataSource ID="AddPagesObjectSource" runat="server"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="fillGrid" runat="server" SelectMethod="GetAllPageTitles"
                    TypeName="ALODWebUtility.PageTitles.PageTitleList" UpdateMethod="UpdateByPageId">
                    <UpdateParameters>
                        <asp:Parameter Name="pageID" Type="Int32" />
                        <asp:Parameter Name="title" Type="String" />
                    </UpdateParameters>
                </asp:ObjectDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
