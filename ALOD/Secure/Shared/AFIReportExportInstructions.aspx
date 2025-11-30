<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AFIReportExportInstructions.aspx.vb" Inherits="ALOD.AFIReportExportInstructions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .leftColumn-2
        {
            display:inline-block;
            float:left;
            width:30%;
            border-bottom:5px;
        }

        .rightColumn-2
        {
            display:inline-block;
            float:right;
            width:69%;
            border-bottom:5px;
        }

        .flexPanel
        {
            display:flex;
            width:99%;
        }

        div > img
        {
            width:auto;
            height:auto;
            max-height:100%;
            max-width:100%;
        }

        .numberedListItem
        {
            list-style-type:decimal;
            list-style-position: outside;
            margin-left: 30px;
            margin-bottom:5px;
        }

        .mainPanel
        {
            max-width:980px;
            margin-left:auto;
            margin-right:auto;
        }
    </style>
</head>
<body class="mainPanel">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="pnlContent" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                Format Export Instructions
            </div>
            <div class="dataBlock-body">
                For the LOD PM reports there are three ways in which you can export data to an external file: (1) via the "Run Report" button, (2) via the "Unit Navigation" panel links, and (3) via the "Report Results" grid links.
                <br />
                <br />
                <div class="flexPanel">
                    <asp:Panel runat="server" ID="Panel2" CssClass="rightColumn-2">
                        <img src="../../images/Instructions_1.png" alt="Via Report Button Instructions Image" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="Panel1" CssClass="leftColumn-2">
                        <h2>Via the "Run Report" button:</h2>

                        One thing to keep in mind about the "Run Report" button for these reports is that when pressed it re-runs the report at the top most unit level appropriate for your current role. For instance, if you are a HQ Board user then the report is going to re-run the report starting at the NAF level. 
                        
                        <br />
                        <br />

                        <ol>
                            <li class="numberedListItem">Set all "Reporting Options" as approriate for the specific LOD PM report you are running.</li>
                            <li class="numberedListItem">Select the "Output Format" radio button which matches with your desired output file.</li>
                            <li class="numberedListItem">Click the "Run Report" button.</li>
                            <li class="numberedListItem">Save the file to your computer.</li>
                        </ol>
                    </asp:Panel>
                </div>
                <br />
                <br />
                <div class="flexPanel">
                    <asp:Panel runat="server" ID="Panel3" CssClass="rightColumn-2">
                        <img src="../../images/Instructions_2.png" alt="Via Unit Navigation Links Instructions Image 1" />
                        <br />
                        <img src="../../images/Instructions_3.png" alt="Via Unit Navigation Links Instructions Image 2" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="Panel4" CssClass="leftColumn-2">
                        <h2>Via the "Unit Navigation" Panel Links:</h2>

                        When outputting results via the "Unit Navigation" panel links you will be outputting the data which you are currently seeing in the browser. 
                        
                        <br />
                        <br />

                        <ol>
                            <li class="numberedListItem">Set all "Reporting Options" as approriate for the specific LOD PM report you are running.</li>
                            <li class="numberedListItem">Select the "Browser" radio button for the "Output Format".</li>
                            <li class="numberedListItem">Click the "Run Report Button".</li>
                            <li class="numberedListItem">Using the links in the "Report Results" grids and "Unit Navigation" panel navigate down to the desired level of information.</li>
                            <li class="numberedListItem">Back up in the "Report Options" panel select the "Output Format" radio button which matches with the desired output file type.</li>
                            <li class="numberedListItem">In the "Unit Navigation" panel click the right-most hyperlink.</li>
                            <li class="numberedListItem">Save the file to your computer.</li>
                        </ol>
                    </asp:Panel>
                </div>
                <br />
                <br />
                <div class="flexPanel">
                    <asp:Panel runat="server" ID="Panel5" CssClass="rightColumn-2">
                        <img src="../../images/Instructions_2.png" alt="Via Report Results Grid Links Instructions Image 1" />
                        <br />
                        <img src="../../images/Instructions_4.png" alt="Via Report Results Grid Links Instructions Image 2" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="Panel6" CssClass="leftColumn-2">
                        <h2>Via the "Report Results" Grid Links:</h2>

                        When outputting results via the "Report Results" links you will be outputting the data for the unit of the link in which you are clicking. For instance, if you click the 301 Fighter Wing link then you will be outputting the results of the report as executed from the 301 Fighter Wing level. 
                        
                        <br />
                        <br />

                        <ol>
                            <li class="numberedListItem">Set all "Reporting Options" as approriate for the specific LOD PM report you are running.</li>
                            <li class="numberedListItem">Select the "Browser" radio button for the "Output Format".</li>
                            <li class="numberedListItem">Click the "Run Report Button".</li>
                            <li class="numberedListItem">Using the links in the "Report Results" grids and "Unit Navigation" panel navigate down to one level above the desired level of information.</li>
                            <li class="numberedListItem">Back up in the "Report Options" panel select the "Output Format" radio button which matches with your desired output file. </li>
                            <li class="numberedListItem">Back down in the "Report Results" grids click the link of the unit you wish to output the results of the report for.</li>
                            <li class="numberedListItem">Save the file to your computer.</li>
                        </ol>
                    </asp:Panel>
                </div>
                <br />
            </div>
        </asp:Panel>
    </form>
</body>
</html>
