<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MemberSearchResultsGrid.ascx.vb" Inherits="ALOD.Web.UserControls.MemberSearchResultsGrid" %>

<asp:GridView runat="server" ID="grdMemberSelection" AutoGenerateColumns="false" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblMemberName" Text='<%# Eval("LastName") + ", " + Eval("FirstName") + " " + Eval("MiddleName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SSN">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblMemberSSN" Text='<%# Eval("SSN").ToString().Substring(Eval("SSN").ToString().Length - 4)%>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Rank">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblMemberRank" Text='<%# Eval("Rank")%>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Unit">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblMemberUnit" Text='<%# Eval("Unit") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <asp:Button runat="server" ID="btnSelect" CommandName="MemberSelected" CommandArgument="<%# CType(Container, GridViewRow).RowIndex %>" Text="SELECT"/>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>