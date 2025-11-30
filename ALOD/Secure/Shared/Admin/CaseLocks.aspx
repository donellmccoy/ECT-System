<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_CaseLocks" Codebehind="CaseLocks.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
<div class="indent">
    <asp:GridView runat="server" ID="LockGrid" AutoGenerateColumns="false" Width="100%">
        <columns>
            <asp:BoundField HeaderText="Case Id" DataField="case_id" SortExpression="case_id" />
            <asp:BoundField HeaderText="User" DataField="UserName" SortExpression="lastName" />
            <asp:BoundField HeaderText="Aquired" DataField="LockTime" DataFormatString="{0:MM/dd/yyyy HHmm}" HtmlEncodeFormatString="false" HtmlEncode="false" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="DeleteLockButton" CommandName="DeleteLock" CommandArgument='<%# Eval("lockId") %>' Text="Clear" />
                </ItemTemplate>
            </asp:TemplateField>
        </columns>
        <EmptyDataRowStyle CssClass="emptyItem" />
        <EmptyDataTemplate>
            No Locks Currently Active
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" Runat="Server">
</asp:Content>

