<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/Public/Outside.master" Inherits="ALOD.Web.Public_Hippa" Codebehind="Hippa.aspx.vb" %>

 <asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
     <div>
        <h1>
            HIPAA Privacy/Security</h1>
        <div style="width: 800px; padding-left:30px; text-align: center;">
            <div style="width: 700px; text-align: left; padding: 5px">
                <b>HIPAA Privacy Rule:</b><br />
                The HIPAA Privacy Rule institutes business processes to protect the use and disclosure
                of protected health information (PHI). PHI is individually identifiable health information,
                including demographics, in paper, electronic, or oral form. PHI is not limited to
                the documents contained in the official medical record. The HIPAA Privacy Rule allows
                the use and disclosure of PHI for treatment, payment and health care operations
                without written authorization from the patient. Other uses and disclosures require
                permission. The compliance date for the HIPAA Privacy rule was April 14, 2003.
                <br />
                <br />
                <b>HIPAA Security Rule:</b><br />
                The HIPAA Security Rule is designed to provide protection for all individually identifiable
                health information that is maintained, transmitted or received in electronic form—not
                just the information in standard transactions. All covered entities were to be in
                compliance with the HIPAA Security Rule no later than April 20, 2005. The safeguards
                in the HIPAA Security Rule are divided into three categories: Administrative Safeguards;
                Physical Safeguards; and Technical Safeguards.
                <br />
                <br />
                <b>HIPAA Training:</b><br />
                The HIPAA training is required. <a href="<%= ConfigurationManager.AppSettings.Get("HipaaTraining") %>" target="_blank">MHS Learn</a>
                <p></p>
            </div>
        </div>
        <div id="frontpage-content">
            <p class="disclaimer" style="padding-left:30px; padding-right:60px;">
                <asp:Label runat="server" ID="SiteDisclaimer" Text="<%$ Resources:Global, SiteDisclaimer %>"></asp:Label></p>
        </div>
    </div>
</asp:Content>