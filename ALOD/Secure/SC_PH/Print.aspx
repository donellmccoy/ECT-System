<%@ Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.Print" Codebehind="Print.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body onload="viewPHDoc(<%= ViewDocURL%>);">
    <form id="form1" runat="server">
        <div>
            An unexpected error occured loading the requested document
        </div> 
    </form>
    <script src="../../Script/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function viewPHDoc(url) {
            showPopup({
                'Url': url,
                'Width': 642,
                'Height': 668,
                'Center': true,
                'Resizable': true,
                'ScrollBars': true
            });

            // close Print browser
            self.window.close();
        }
    </script>
</body>
</html>