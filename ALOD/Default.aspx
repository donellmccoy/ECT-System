<%@ Page Title="" Language="VB" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web._Default" Codebehind="Default.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

    <script  type="text/javascript">

        /* $(document).ready(function () {Deprecated ready Syntax: -- Diamante Lawson 10/12/2023 */

        (function() {
            $('#msgBox').dialog({
                autoOpen: false,
                modal: true,
                position: 'center',
                resizable: false,
                bgiframe: true,
                width: 650,
                height: 400,

                buttons: {
                    'OK': function() {
                        $(this).dialog('close');
                        var url = $_HOSTNAME + '/login/Login.aspx';
                        window.location = url;
                    },
                    'Cancel': function() {
                        $(this).dialog('close');
                    }
                }

                
            });
            var msg = '<strong>USG Legal Notice </strong> ';
            msg += '<br><br>';
            msg += 'You are accessing a U.S. Government (USG) Information System (IS) that is provided for USG-authorized use only.<br>';
            msg += 'By using this IS (which includes any device attached to this IS), you consent to the following conditions:<br> ';
            msg += '- The USG routinely intercepts and monitors communications on this IS for purposes including, but not limited to, penetration testing, COMSEC monitoring, network operations and defense, personnel misconduct (PM), law enforcement (LE), and counterintelligence (CI) investigations.<br> ';
            msg += '- At any time, the USG may inspect and seize data stored on this IS.<br>';
            msg += '- Communications using, or data stored on, this IS are not private, are subject to routine monitoring, interception, and search, and may be disclosed or used for any USG authorized purpose.<br> ';
            msg += '- This IS includes security measures (e.g., authentication and access controls) to protect USG interests--not for your personal benefit or privacy.<br>';
            msg += '- Notwithstanding the above, using this IS does not constitute consent to PM, LE or CI investigative searching or monitoring of the content of privileged communications, or work product, related to personal representation or services by attorneys, psychotherapists, or clergy, and their assistants. Such communications and work product are private and confidential. See User Agreement for details.<br>';
            msg += '<br> ';
            msg += '<strong>WARNING! FOR OFFICIAL USE ONLY.</strong> <br> ';
            msg += '<br> ';
            msg += 'This document may contain information covered under the Privacy Act, 5 USC 552(a), and/or the Health Insurance Portability and Accountability Act (PL 104-191) and its various implementing regulations.<br> ';
            msg += 'and must be protected in accordance with those provisions.<br> ';
            msg += 'Healthcare information is personal and sensitive and must be treated accordingly.<br> ';
            msg += 'If this correspondence contains healthcare information it is being provided to you after appropriate authorization from the patient or under circumstances that don"t require patient authorization.<br> ';
            msg += 'You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Redisclosure without additional patient consent or as permitted by law is prohibited.<br> ';
            msg += 'Unauthorized redisclosure or failure to maintain confidentiality subjects you to application of appropriate sanction. If you have received this correspondence in error, please notify the sender at once and destroy any copies you have made.<br>';

            $('#msgText').html(msg);

            cycleBan();
        });



        function showDialog() {
            $('#msgBox').dialog('open');
            $('#msgBox').dialog('option', 'position', 'center');
        }
       

        var bannerImg = new Array();
        // Enter the names of the images below
        bannerImg[0] = $_HOSTNAME + '/App_Themes/DefaultBlue/images/ALOD1.jpg';
        bannerImg[1] = $_HOSTNAME + '/App_Themes/DefaultBlue/images/ALOD2.jpg';
        bannerImg[2] = $_HOSTNAME + '/App_Themes/DefaultBlue/images/ALOD3.jpg';
        bannerImg[3] = $_HOSTNAME + '/App_Themes/DefaultBlue/images/ALOD4.jpg';
        bannerImg[4] = $_HOSTNAME + '/App_Themes/DefaultBlue/images/ALOD5.jpg';

        var newBanner = 0;
        var totalBan = bannerImg.length;

        function cycleBan() {
            newBanner++;
            if (newBanner == totalBan) {
                newBanner = 0;
            }
            document.getElementById("banner").src = bannerImg[newBanner];
            // set the time below for length of image display
            // i.e., "4*1000" is 4 seconds
            setTimeout("cycleBan()", 5 * 1000);
        }
       window.onload = cycleBan; 
   
    </script>

    <style>
       .ui-dialog{
            margin-top: 250px; margin-left: 30%;
       }
    </style>

    <div class="hidden" title="USG Legal Notice" id="msgBox">
    <p id="msgText" > </p>
     </div>
        <div id="front-actions">
            <div style="float: left; width: 600px; height: 275px; background-repeat: no-repeat; background-color: transparent; ">
                <img style="width: 600px; height: 275px" src="App_Themes/DefaultBlue/images/ALOD1.jpg" id="banner" alt="ECT Banner" />
            </div>
            <div id="front-login" style="float:left; width:360px; margin-top:50px;">
                <table border="0"style="width: 100%;">
                    <tr>
                        <td style="text-align:center;">
                            <br />
                            <div>
                                <asp:HyperLink ID="WhatIsEct" runat="server" NavigateUrl="~/Public/About.aspx"
                                    Text="<%$ Resources:Global, LODFAQLink %>" Target="_blank" 
                                    Font-Bold="True"></asp:HyperLink><br />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:center;">
                            <asp:Button ID="btnLoginCac" runat="server" Text="Login to ECT"  /><br />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:center;">
                            <asp:LinkButton runat="server" ID="lnkDevLogin">Developer Login</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="frontpage-content">
            <div id="TimeoutMessage">
                <asp:Label runat="server" ID="ErrorLabel" Visible="false"></asp:Label></div>
            <p class="disclaimer">
                <asp:Label runat="server" ID="SiteDisclaimer" Text="<%$ Resources:Global, SiteDisclaimer %>"></asp:Label></p>
        </div>

</asp:Content>
