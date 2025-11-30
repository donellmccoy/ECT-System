<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_TrainingUsers"
    Title="Untitled Page" Codebehind="TrainingUsers.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    Component:
    <asp:DropDownList ID="cbCompo" runat="server" AutoPostBack="True">
        <asp:ListItem Value="2">National Guard</asp:ListItem>
        <asp:ListItem Value="3">Army Reserve</asp:ListItem>
    </asp:DropDownList>
    &nbsp; State/Region:
    <asp:DropDownList ID="cbRegion" runat="server" DataSourceID="ObjectDataSource1" DataTextField="text"
        DataValueField="value">
    </asp:DropDownList>
    &nbsp;
    <asp:Button ID="btnView" runat="server" Text="View In Browser" />
    <asp:Button ID="btnExport" runat="server" Text="Export to Excel" /><br />
    <br />
    <asp:PlaceHolder ID="plOutput" runat="server"></asp:PlaceHolder>
    <br />
    
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetRegionsForCompo"
        TypeName="LookUp">
        <SelectParameters>
            <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
