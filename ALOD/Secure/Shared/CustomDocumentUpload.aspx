<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CustomDocumentUpload.aspx.vb" Inherits="ALOD.Web.Docs.CustomDocumentUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Document Upload</title>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <style>
        .uploadBody
        {
            font-family: 'Times New Roman', Arial, Sans-Serif;
	        font-size: 1em;
	        margin: 10px;
	        background-color:white;
        }
        .uploadHeader1
        {
            font-size: 1.8em;	
        }
        .title 
        {
            font-size: 2em;
            color:black;
            text-indent: 0px;
            padding-top:4px;
            padding-bottom:20px;
            font-weight:bold;
        }
        .subtitle 
        {
            color:black;
            font-size: 16px;
        }
        .uploadLabel 
        {
            font-size:16px;
        }
        .uploadLoading 
        {
            display:none;
            margin-top:10px;
            color:green;
            font-weight:bold;
        }
        .uploadStatus 
        {
            display:none;
            margin-top:10px;
            font-weight:bold;
        }

        div.ui-datepicker
        {
            font-size:12px;
        }
    </style>
</head>
<body class="uploadBody">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div id="divDocumentUpload" runat="server">
            <div id="DocTypeName" runat="server" class="title"></div>
            <div id="divSubtitle" runat="server" class="subtitle"></div>
            <div id="divValidation" runat="server" style="color:red;"></div>
            <table >
                <tr>
                    <td class="uploadLabel">
                        Document&nbsp;Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDocDate" MaxLength="10" onChange="DateCheck(this);" runat="server" Width="80" />
                    </td>
                </tr>
                <tr>
                    <td class="uploadLabel">
                        Description:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDocDescr" runat="Server" MaxLength="50" Width="210"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="regex_txtDocDescr" runat="server" ValidationExpression="^[\w\.\-\(\) ]*$" Display="Dynamic" ControlToValidate="txtDocDescr" ErrorMessage="<br />* Alphanumeric characters only"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr style="display:none">
                    <td class="uploadLabel">
                        Keywords:
                    </td>
                    <td>
                        <asp:TextBox ID="txtKeywords" runat="Server" MaxLength="50" Width="210"></asp:TextBox>
                    </td>
                </tr>	
                <tr id="trUpload" runat="server">
                    <td class="uploadLabel">
                        File Name:
                    </td>
                    <td>
                        <asp:FileUpload ID="filePicker" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td style="padding-top:20px">
                        <input id="btnUploadScan" runat="server" type="button" value="Upload" onclick="SubmitUploadForm();" />
                        <input id="btnSubmit" runat="server" type="submit" style="display:none" />&nbsp;
                        <input id="btnCancel" type="button" value="Cancel" onclick="CancelUpload();" />
                        <div id="divLoading" class="uploadLoading">
                            <span id="lblLoadingMsg"></span>
                        </div>
                        <div id="divStatusMsg" class="uploadStatus"></div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script type="text/javascript">

        $(function () {

            $('.datePickerPast').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        });

        function CancelUpload() {
            window.close();
        }

        function SubmitUploadForm() {
            if (!document.getElementById("regex_txtDocDescr").isvalid) {
                alert("Document Description is invalid.");
                return;
            }

            document.getElementById("btnSubmit").click();
        }

        function daysInFebruary(year) {
            // February has 29 days in any year evenly divisible by four,
            // EXCEPT for centurial years which are not also divisible by 400.
            return (((year % 4 === 0) && ((!(year % 100 === 0)) || (year % 400 === 0))) ? 29 : 28);
        }

        function dayInMonth(n) {
            if (n == 4 || n == 6 || n == 9 || n == 11) { return 30; }
            if (n == 2) { return 29; }
            return 31;
        }

        function IsValidDate(strDate) {

            //Validate military time
            var minYear = 1900;
            var maxYear = 2100;

            var parts = strDate.split("/");

            if (parts.length != 3) {
                return false;
            }

            var month = parseInt(parts[0], 10);
            var day = parseInt(parts[1], 10);
            var year = parseInt(parts[2], 10);

            if (month < 1 || month > 12) {
                return false;
            }

            if (day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > dayInMonth(month)) {
                return false;
            }

            if (year === 0 || year < minYear || year > maxYear) {
                return false;
            }

            return true;

        }


        function DateCheck(oTextBox) {
            //Validate date

            var minYear = 1900;
            var maxYear = 2100;
            var strDate = oTextBox.value;

            if (strDate.length === 0) {
                return;
            }

            if (strDate.length < 8 || strDate.length > 10 || (IsValidDate(strDate) === false)) {
                alert("Invalid Date.\nDate format is: MM/DD/YYYY");
                oTextBox.value = "";
                oTextBox.focus();
            }
        }
    </script>
</body>
</html>
