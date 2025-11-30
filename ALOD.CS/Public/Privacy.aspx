<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Outside.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_Privacy" CodeBehind="Privacy.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <h1>Privacy and Security Notice</h1>
        <div style="width: 800px; text-align: center">
            <div style="width: 700px; text-align: left; margin-left:-20px;">
                <ul>
                    <li>This site is provided as a public service by the Army Medical Department. This site
                        is intended to be used for viewing and retrieving information only. Unauthorized
                        attempts to change information on this service or tamper with this website are strictly
                        prohibited and may be punishable under the Computer Fraud Act of 1986 and the National
                        Information Infrastructure Protection Act.<p/> </li>
                    <li>All information, including personal information, placed on or sent over this system
                        may be monitored. Statistics and other information about your visit may be recorded.
                        Use of this system constitutes consent to monitoring for these purposes.<p/> </li>
                    <li>For site security purposes and to ensure that this service remains available to
                        all users, this government computer system employs software programs to monitor
                        network traffic to identify unauthorized attempts to upload or change information,
                        or otherwise cause damage.<p/> </li>
                    <li>Cookie Disclaimer � This Website does not use persistent cookies (persistent tokens
                        that pass information back and forth from the client machine to the server). This
                        site may use session cookies (tokens that remain active only until you close your
                        browser) in order to make the site easier to use. This website DOES NOT keep a database
                        of information obtained from these cookies.
                        <br /><br />
                            You can choose not to accept these cookies and still use the site, but it may take
                            you longer to fill out the same information repeatedly and clicking on the banners
                            will not take you to the correct link. Refer to the help information in your browser
                            software for instructions on how to disable cookies.</p>
                    </li>
                </ul>
            </div>
        </div>
        <div id="frontpage-content">
            <p class="disclaimer" style="padding-left:30px; padding-right:60px;">
                <asp:Label runat="server" ID="SiteDisclaimer" Text="<%$ Resources:Global, SiteDisclaimer %>"></asp:Label></p>
        </div>
    </div>
</asp:Content>


