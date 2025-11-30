<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Outside.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_Section508" CodeBehind="Section508.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <h1>Section 508 Compliance</h1>
        <div style="width: 800px; padding-left:30px; text-align: center">
            <div style="width: 700px; text-align: left">
                <p>
                The U.S. Department of Defense is committed to making its electronic and information
                technologies accessible to individuals with disabilities in accordance with 
                <a href="http://www.access-board.gov/the-board/laws/rehabilitation-act-of-1973#508" target="_blank">
               Section 508 of the Rehabilitation Act (29 U.S.C. � 794d), as amended in 1999</a>.
                </p>
                <p>
                Send feedback or concerns related to the accessibility of this website to: <a href="mailto:DoDSection508@osd.mil">DoDSection508@osd.mil</a>.
                </p>
                <p>
                For more information about Section 508, please visit the <a href="http://dodcio.defense.gov/DoDSection508.aspx" target="_blank">DoD Section 508 website</a>.                
                </p>
                <p>
                Last Updated: 08/28/2013
                </p>
                <p/>
            </div>
        </div>
        <div id="frontpage-content">
            <p class="disclaimer" style="padding-left:30px; padding-right:60px;">
                <asp:Label runat="server" ID="SiteDisclaimer" Text="<%$ Resources:Global, SiteDisclaimer %>"></asp:Label></p>
        </div>
    </div>
</asp:Content>


