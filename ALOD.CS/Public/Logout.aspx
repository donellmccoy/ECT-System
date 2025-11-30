<%@ Page Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Logout" Title="ECT::Logout" CodeBehind="Logout.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div style="padding: 20px;">
        <h1>
            Log Out</h1>
        <table style="width: 500px;">
            <tr>
                <td colspan="2" >
                    <span style="font-size: 15pt;">
                    You have been logged out.</span><br />
                    <br />
                    For security reasons you are strongly encouraged to close your browser to remove any credentials cached by your browser.  
                    <br /><br />
                </td>
            </tr>
            <tr>
                <td>
                    <input id="btnClose" class="Button" type="button" value="Close Window" onclick="btnClose_onclick()" />
                </td>
                <td>
                    <input id="btnLogin" class="Button" onclick="window.location='../default.aspx';" type="button" value="Back to Login Page" style="width: 150px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="ContentFooter">

    <script language="javascript" type="text/javascript">
        if (window.opener) {
            alert("Page has timed out, window will close");
            var URL = document.location.pathname.substring(1);
            URL = URL.substring(0, URL.indexOf("/")) + "/default.aspx";
            URL = document.location.protocol + '//' + document.location.host + '/' + URL;
            window.opener.document.location.href = URL;
            top.close();
        }

        function onLoad() {
            if (top.window.location != document.location) {
                top.window.location = document.location;
            } else {
                window.close();
            }
        }

        $(function () {
            // Removed the document.execCommand("ClearAuthenticationCache"); as it's now obsolete.

            var URL = document.location.pathname.substring(1);
            URL = URL.substring(0, URL.indexOf("/")) + "/default.aspx";
            URL = document.location.protocol + '//' + document.location.host + '/' + URL;
            //alert(URL);

            onLoad();
        });

			
function btnClose_onclick() {
    window.close();

}

    </script>

</asp:Content>
