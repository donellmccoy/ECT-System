<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.TestPHForm" Codebehind="TestPHForm.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

    <div class="fieldPanel" style="float:right;margin-right:-28px;">
            <span style="font-weight:bold;">KEY: </span>
            <span>Members Seen =</span><asp:TextBox ID="txtMS" runat="server" Width="22" CssClass="fieldTypeMS">MS</asp:TextBox>&nbsp;&nbsp;
            <span>Frequency =</span><asp:TextBox ID="txtFR" runat="server" Width="22" CssClass="fieldTypeFR">FR</asp:TextBox>&nbsp;&nbsp;
            <span>Follow-up =</span><asp:TextBox ID="txtFO" runat="server" Width="22" CssClass="fieldTypeFO">FO</asp:TextBox>

    </div>

    <br style="clear:both;" />

    <ajax:Accordion ID="accPHForm" runat="server" HeaderCssClass="accHeader" ContentCssClass="accContent" CssClass="acc" HeaderSelectedCssClass="accHeaderSelected">
        <Panes>
        </Panes>
    </ajax:Accordion>

<%--    <div style="float:left;">
        <asp:Button runat="server" ID="btnTestSubmit" Text="Test Submit" />
        <asp:Button runat="server" ID="btnTestPrint" Text="Test Print" />
    </div>--%>


</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
</asp:Content>
