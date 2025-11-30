<%@ Page Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.AccessDenied" title="AccessDenied" CodeBehind="AccessDenied.aspx.cs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentHeader" runat="Server">
    <style type="text/css">
        .deniedText 
        {
            font-weight:bold;
            font-size:10pt;
            font-family:Verdana;
            color: #cc6600;
        } 
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <p>
        <asp:Image ID="imgExclamationIcon1" runat="server" ImageAlign="AbsMiddle" SkinID="imgExclamation" />
        <asp:Label runat="server" ID="lblDeniedSubject" CssClass="deniedText" Text="You do not have access to this page." />
    </p>

    <p>
        <asp:Image ID="imgExclamationIcon3" runat="server" ImageAlign="AbsMiddle" SkinID="imgExclamation" />
        <asp:Label runat="server" ID="lblReturnText" CssClass="deniedText" Text="Click on the 'Back' button on your browser to go back to the previous page." />
    </p>
</asp:Content>
