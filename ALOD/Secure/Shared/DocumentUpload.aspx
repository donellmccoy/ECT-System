<%@ Page Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.Docs.Secure_Shared_DocumentUpload" Codebehind="DocumentUpload.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <asp:Image runat="server" ID="ErrorIcon" AlternateText="Error" ImageUrl="~/images/warning.gif" ImageAlign="AbsMiddle" />
    

       <asp:Label ID="ErrorLabel" runat="server"  Text="An error occured initializing the document transfer system.  Please try again later"></asp:Label>
    
    </div>
    </form>
</body>
</html>
