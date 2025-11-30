<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_AltSession" CodeBehind="AltSession.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <br />
        <h3>
            Login Error</h3>
        <br />
        <p>
            You are currently logged into the ALOD system from a different session. Please logout
            of the existing session before attempting to login again.
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
