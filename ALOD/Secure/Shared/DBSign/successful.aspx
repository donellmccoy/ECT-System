<%@ Page Language="vb" AutoEventWireup="false" Inherits="ALOD.Web.DBSign.Successful" Codebehind="Successful.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
	<head runat="server">
		<title>Successful</title>
        <META HTTP-EQUIV="CACHE-CONTROL" CONTENT="NO-CACHE" />
		<META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE" />
        <META HTTP-EQUIV="EXPIRES CONTENT="0" />
        
        <script type="text/javascript">
        
            function next()
            {   
                window.parent.endSign();
            }
        
        </script>
	</head>
	<body style="background-color:#EEE;" onload="next();">
		<form id="Form1" method="post" runat="server">
		<br />
		    <img src="../../../images/sig_valid.gif"> Digital signature completed	 
		</form>
	</body>
	
</html>
