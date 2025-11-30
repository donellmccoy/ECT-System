<%@ Page Language="vb" AutoEventWireup="false" Inherits="ALOD.Web.DBSign.Failure" Codebehind="Failure.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">

    <script type="text/javascript">

         window.onload = function () {

             console.log("now on failure.aspx");
             console.log("location: " + window.location);
           
        };
          
            function next()
            {   
              // window.parent.endSign();
            }
        
        </script>

    <title>Failure</title>
</head>
<body style="background-color: #eee;" onload="next();">
    <form id="Form1" method="post" runat="server" style="">
        <br />
        <img src="../../../images/sig_error.gif" alt="Signature Error">&nbsp;
        <asp:Label ID="lblFailed" runat="server" Font-Bold="True" ForeColor="Red">Signature Failed</asp:Label>
        <br />
        <br />
        <asp:Label ID="lblErrorMsg" runat="server" CssClass="labelRequired"></asp:Label>
        <br />
        <br />
        Press "Digitally Sign" button to try again.
    </form>
</body>
</html>
