<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/lod/Lod.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_lodBoard" Codebehind="lodBoard.aspx.vb" 
    MaintainScrollPositionOnPostback="true"%>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel ID="LOD_Original" runat="server" Visible="false">
        <div id="dvFormal" runat="server">
           <div id="FrmlAppointingAuthority" visible="True" runat="Server">
                <div class="formHeader">
                    Formal Appointing Authority Action 
                </div>
                <uc:Findings ID="fcfrmlAppAuth" ConcurWith="IO" ShowPrevFindings="True" ShowRemarks="false" ReasonsLabelText="Reasons:" ShownOnText="(Shown on Form 261)" ShowFormText="True" SetReadOnly="False" FindingsLableText="Substituted Findings:"
                runat="Server"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckAppointing" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
             <div id="dvspfrmlAppAuth" runat="Server" visible="false">
                <br />
            </div>
    
            <div id="FrmlBoardLegal" visible="False" runat="Server">
                <div class="formHeader">
                    Formal Board Legal Review
                </div>
                <uc:Findings ID="bdfrmlLegalFindings" Formal="true" SetReadOnly="True"
                    runat="Server" ShownOnText="(Shown on Form 348)" ShowFormText="True" RemarksLableText="Formal Legal Review and Recommendations:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Legal Review and Recommendations:"></uc:Findings>
                    <uc1:SignatureCheck ID="SigCheckLegalFormal" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp5" runat="Server" visible="false">
                <br />
            </div>
            <div id="FrmlBoardMedical" visible="False" runat="Server">
                <div class="formHeader">
                    Formal Board Medical Review
                </div>
                <uc:Findings ID="bdfrmlMedicalFindings" ShowFormText="True" SetReadOnly="True" Formal="true" 
                    runat="Server" ShownOnText="(Shown on Form 348)" RemarksLableText="Formal Medical Review and Recommendations:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Medical Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckMedicalFormal" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp7" runat="Server" visible="false">
                <br />
            </div>
            <div id="FrmlBoardPersonnel" visible="False" runat="Server">
                <div class="formHeader">
                    Formal Board Personnel Review
                </div>

                <table>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label labelRequired">
                            *Does the member have 8 years of credible service:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblFrmlCredibleService" />
                            <span class="normal">
                                <asp:RadioButtonList ID="rblFrmlCredibleService" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Y">Yes</asp:ListItem>
                                    <asp:ListItem Value="N">No</asp:ListItem>
                                </asp:RadioButtonList>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label labelRequired">
                            *Was the member on orders of 31 days or more:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblFrmlWasMemberOnOrders" />
                            <span class="normal">
                                <asp:RadioButtonList ID="rblFrmlWasMemberOnOrders" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Y">Yes</asp:ListItem>
                                    <asp:ListItem Value="N">No</asp:ListItem>
                                </asp:RadioButtonList>
                            </span>
                        </td>
                    </tr>
                </table>

                <uc:Findings ID="bdfrmlPersonnelFindings" ShowFormText="False" SetReadOnly="True" Formal="true" 
                    runat="Server" ShownOnText="(DON'T SHOW THIS)" RemarksLableText="Formal Personnel Review and Recommendations:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Personnel Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckPersonnelFormal" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp6" runat="Server" visible="false">
                <br />
            </div>
            <div id="FrmlBoardAAReview" visible="False" runat="Server">
                <div class="formHeader">
                    Formal Approving Authority Review
                </div>
                <uc:Findings ID="bdFrmlAppAuthFindings" ShowFormText="True" FindingsOnly="True" runat="Server"
                    FindingsLableText="Final Approval:" ShownOnText="(Shown on Form 261)" SetReadOnly="True" RemarksLableText="Formal Approving Authority Comments:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Approving Authority Comments:">
                </uc:Findings>
                <uc1:SignatureCheck ID="SigCheckAAFormal" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp8" runat="Server" visible="false">
                <br />
            </div>
            <div id="FromalBoardRA" visible="False" runat="Server">
                <div class="formHeader">
                    Formal Board Reviewing Authority Review
                </div>
                <uc:Findings ID="bdFormalRAFindings" FindingsOnly="True" FindingsLableText="Final Approval:"
                    runat="Server" SetReadOnly="True" RemarksLableText="Formal Reviewing Authority Comments:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Reviewing Authority Comments:">
                </uc:Findings>

                <table>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            On behalf of Approving Authority
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSelectedFormalApprovingAuthority" />
                            &nbsp;<asp:DropDownList runat="server" ID="ddlFormalApprovingAuthorities" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvsp9" runat="Server" visible="false">
                <br />
            </div>
        </div>
        <div id="dvInformal" runat="server">
              <div id="AppointingAuthority" visible="true" runat="Server">
                <div class="formHeader">
                      Appointing Authority Action
                </div>
                   <uc:Findings ID="fcAppAuth" ShowFormText="True" RemarksLableText="Comments:" FindingsOnly="true"
                     SetReadOnly="False" runat="Server"></uc:Findings>
              <br />
            </div>
             <div id="Div2" runat="Server" visible="false">
                <br />
            </div>
    
            <div id="BoardLegal" visible="False" runat="Server">
                <div class="formHeader">
                    Board Legal Review
                </div>
                <uc:Findings ID="bdLegalFindings"  SetDecisionToggle="True" ShowFormText="True" SetReadOnly="True" runat="Server"
                    RemarksLableText="Legal Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckLegal" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp1" runat="Server" visible="false">
                <br />
            </div>
            <div id="BoardMedical" visible="False" runat="Server">
                <div class="formHeader">
                    Board Medical Review
                </div>
                <uc:Findings ID="bdMedicalFindings"   SetDecisionToggle="True" ShowFormText="True" SetReadOnly="True" runat="Server"
                    RemarksLableText="Medical Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckMedical" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp3" runat="Server" visible="false">
                <br />
            </div>
            <div id="BoardPersonnel" visible="False" runat="Server">
                <div class="formHeader">
                    Board Personnel Review
                </div>

                <table>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label labelRequired">
                            *Does the member have 8 years of credible service:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblCredibleService" />
                            <span class="normal">
                                <asp:RadioButtonList ID="rblCredibleService" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Y">Yes</asp:ListItem>
                                    <asp:ListItem Value="N">No</asp:ListItem>
                                </asp:RadioButtonList>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label labelRequired">
                            *Was the member on orders of 31 days or more:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblWasMemberOnOrders" />
                            <span class="normal">
                                <asp:RadioButtonList ID="rblWasMemberOnOrders" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Y">Yes</asp:ListItem>
                                    <asp:ListItem Value="N">No</asp:ListItem>
                                </asp:RadioButtonList>
                            </span>
                        </td>
                    </tr>
                </table>

                <uc:Findings ID="bdPersonnelFindings"   SetDecisionToggle="True" ShowFormText="False" SetReadOnly="True" runat="Server"
                    RemarksLableText="Personnel Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckPersonnel" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp2" runat="Server" visible="false">
                <br />
            </div>
            <div id="BoardAAReview" visible="False" runat="Server">
                <div class="formHeader">
                    Approving Authority Review
                </div>
                <uc:Findings ID="bdAppAuthFindings" SetReadOnly="True" FindingsOnly="True"
                    runat="Server" FindingsLableText="Final Approval:" RemarksLableText="Approving Authority Comments:">
                </uc:Findings>
                <uc1:SignatureCheck ID="SigCheckAA" runat="server" Template="Form348Findings" CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp4" runat="Server" visible="false">
                <br />
            </div>
  
        <div id="AFRCBoard" visible="False" runat="Server">
            <div class="formHeader"> 
                AFRC Board Review
            </div>
            <uc:Findings ID="bdFindings" SetReadOnly="True" FindingsOnly="true"
                runat="Server" RemarksLableText="Comments:" FindingsLableText="AFRC LOD Board Action:">
            </uc:Findings>
            <uc1:SignatureCheck ID="SigCheckBoardTech" runat="server" Template="Form348Findings"
                CssClass="sigcheck-form no-border" />

            <table>
                <tr>
                    <td class="number">
                        &nbsp;
                    </td>
                    <td class="label">
                        On behalf of Approving Authority
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblSelectedApprovingAuthority" />
                        &nbsp;<asp:DropDownList runat="server" ID="ddlApprovingAuthorities" />
                    </td>
                </tr>
            </table>
            <div id="dvsp04" visible="false" runat="server">
                <br />
            </div>
        </div>
      </div>

    </asp:Panel>


    <asp:Panel ID="LOD_v2_panel" runat="server" Visible="false">
        <div id="dvFormal_v2" runat="server">
           <div id="FrmlAppointingAuthority_v2" visible="True" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Appointing Authority Action 
                </div>
               <div class="dataBlock-body">
                <uc:Findings ID="fcfrmlAppAuth_v2" ConcurWith="IO" ShowPrevFindings="True" ShowRemarks="false" ReasonsLabelText="Reasons:" ShownOnText="(Shown on Form 261)" ShowFormText="True" SetReadOnly="False" FindingsLableText="Substituted Findings:"
                runat="Server"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckAppointing_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
    
    
            <div id="FrmlBoardLegal_v2" visible="False" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Board Legal Review
                </div>
                <div class="dataBlock-body">
                <uc:Findings ID="bdfrmlLegalFindings_v2" Formal="true" SetReadOnly="True"
                    runat="Server" ShownOnText="(Shown on Form 348)" ShowFormText="True" RemarksLableText="Formal Legal Review and Recommendations:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Legal Review and Recommendations:"></uc:Findings>
                    <uc1:SignatureCheck ID="SigCheckLegalFormal_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            

            <div id="FrmlBoardMedical_v2" visible="False" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Board Medical Review
                </div>
                <div class="dataBlock-body">
                <uc:Findings ID="bdfrmlMedicalFindings_v2" ShowFormText="True" SetReadOnly="True" Formal="true" 
                    runat="Server" ShownOnText="(Shown on Form 348)" RemarksLableText="Formal Medical Review and Recommendations:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Medical Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckMedicalFormal_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            


            <div id="FrmlBoardPersonnel_v2" visible="False" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Board Personnel Review
                </div>
                <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlFormalPersonnelFindings_v2">
                    <ContentTemplate>
                        <uc:Findings ID="bdfrmlPersonnelFindings_v2" ShowFormText="False" SetReadOnly="True" Formal="false" DoDecisionAutoPostBack="true" DoFindingsAutoPostBack="true"
                            runat="Server" ShownOnText="(DON'T SHOW THIS)" RemarksLableText="Formal Personnel Review and Recommendations:"
                            ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Personnel Review and Recommendations:"></uc:Findings>

                        <table id="bdfrmlPersonnel_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="FRMLPersonnel_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="bdFRMLPersonnellbl_v2" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="bdfrmlPersonnelREFER_v2" runat="server"/>
                            
                                    <asp:Label runat="server" ID="bdFrmlPersonnelChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:SignatureCheck ID="SigCheckPersonnelFormal_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            

            <div id="FrmlBoardAAReview_v2" visible="False" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Approving Authority Review
                </div>

                <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlFormalAppAuthFindings_v2">
                    <ContentTemplate>
                        <uc:Findings ID="bdFrmlAppAuthFindings_v2" ShowFormText="True" FindingsOnly="True" runat="Server" DoDecisionAutoPostBack="true" DoFindingsAutoPostBack="true"
                            FindingsLableText="Final Approval:" ShownOnText="(Shown on Form 261)" SetReadOnly="True" RemarksLableText="Formal Approving Authority Comments:"
                            ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Approving Authority Comments:">
                        </uc:Findings>
                        <table id="bdfrmlAppAuth_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="FrmlAppAuth_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="FrmlAppAuthlbl_v2" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="FrmlAppAuthREFER_v2" runat="server"/>
                                    <asp:Label runat="server" ID="FrmlAppAuthChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <uc1:SignatureCheck ID="SigCheckAAFormal_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            

            <div id="FromalBoardRA_v2" visible="False" runat="Server" Class="dataBlock">
                <div class="dataBlock-header">
                    Formal Board Reviewing Authority Review
                </div>
                <div class="dataBlock-body">
                <uc:Findings ID="bdFormalRAFindings_v2" FindingsOnly="True" FindingsLableText="Final Approval:"
                    runat="Server" SetReadOnly="True" RemarksLableText="Formal Reviewing Authority Comments:"
                    ShowAdditionalRemarksText="true" AdditionalRemarksLableText="Informal Reviewing Authority Comments:">
                </uc:Findings>

                <table>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            On behalf of Approving Authority
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblSelectedFormalApprovingAuthority_v2" />
                            &nbsp;<asp:DropDownList runat="server" ID="ddlFormalApprovingAuthorities_v2" />
                        </td>
                    </tr>
                </table>
            </div>
            </div>
        </div>
        <div id="dvInformal_v2" runat="server">

              <div id="AppointingAuthority_v2" visible="true" runat="Server" class="dataBlock">
                <div class="dataBlock-header">
                      Appointing Authority Action
                </div>
                <div class="dataBlock-body">
                   <uc:Findings ID="fcAppAuth_v2" ShowFormText="True" RemarksLableText="Comments:" FindingsOnly="true"
                     SetReadOnly="False" runat="Server"></uc:Findings>
            </div>
            </div>
    
    
            <div id="BoardLegal_v2" visible="False" runat="Server" class="dataBlock">
                <div class="dataBlock-header">
                    Board Legal Review
                </div>
                <div class="dataBlock-body">
                <uc:Findings ID="bdLegalFindings_v2"  SetDecisionToggle="True" ShowFormText="True" SetReadOnly="True" runat="Server"
                    RemarksLableText="Legal Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckLegal_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            

            <div id="BoardMedical_v2" visible="False" runat="Server" class="dataBlock">
                <div class="dataBlock-header">
                    Board Medical Review
                </div>
                <div class="dataBlock-body">
                <uc:Findings ID="bdMedicalFindings_v2"   SetDecisionToggle="True" ShowFormText="True" SetReadOnly="True" runat="Server"
                    RemarksLableText="Medical Review and Recommendations:"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckMedical_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            


            <div id="BoardPersonnel_v2" visible="False" runat="Server" class="dataBlock">
                <div class="dataBlock-header">
                    Board Personnel Review
                </div>

                <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlPersonnelFindings_v2">
                    <ContentTemplate>
                        <uc:Findings ID="bdPersonnelFindings_v2" runat="Server" SetDecisionToggle="True" ShowFormText="False" SetReadOnly="True" DoDecisionAutoPostBack="true" DoFindingsAutoPostBack="true"
                            RemarksLableText="Personnel Review and Recommendations:" />

                        <table id="bdPersonnel_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="Label1_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="bdPersonnellbl_v2" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="bdPersonnelREFER_v2" runat="server"/>
                                    <asp:Label runat="server" ID="bdPersonnelChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <uc1:SignatureCheck ID="SigCheckPersonnel_v2" runat="server" Template="Form348Findings"
                    CssClass="sigcheck-form no-border" />
            </div>
            </div>
            <div id="BoardAAReview_v2" visible="False" runat="Server" class="dataBlock">
                <div class="dataBlock-header">
                    Approving Authority Review
                </div>

                <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlAppAuthFindings_v2">
                    <ContentTemplate>
                        <uc:Findings ID="bdAppAuthFindings_v2" SetReadOnly="True" FindingsOnly="True" DoDecisionAutoPostBack="true" DoFindingsAutoPostBack="true"
                            runat="Server" FindingsLableText="Final Approval:" RemarksLableText="Approving Authority Comments:">
                        </uc:Findings>

                        <table id="bdAppAuth_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="Label2_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="bdAppAuthlbl_v2" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="bdAppAuthREFER_v2" runat="server"/>
                                    <asp:Label runat="server" ID="bdAppAuthChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:SignatureCheck ID="SigCheckAA_v2" runat="server" Template="Form348Findings" CssClass="sigcheck-form no-border" />
            </div>
            </div>
  
        <div id="AFRCBoard_v2" visible="False" runat="Server" Class="dataBlock">
            <div class="dataBlock-header"> 
                AFRC Board Review
            </div>
            <div class="dataBlock-body">
            <uc:Findings ID="bdFindings_v2" SetReadOnly="True" FindingsOnly="true"
                runat="Server" RemarksLableText="Comments:" FindingsLableText="AFRC LOD Board Action:">
            </uc:Findings>

            <table>
                <tr>
                    <td class="number">
                        &nbsp;
                    </td>
                    <td class="label">
                        On behalf of Approving Authority
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblSelectedApprovingAuthority_v2" />
                        &nbsp;<asp:DropDownList runat="server" ID="ddlApprovingAuthorities_v2" />
                    </td>
                </tr>

            </table>
                    <uc1:SignatureCheck ID="SigCheckBoardTech_v2" runat="server" Template="Form348Findings"
                        CssClass="sigcheck-form no-border" />
            </div>
            <div id="dvsp04_v2" visible="false" runat="server">
                <br />
            </div>
        </div>
      </div>

    </asp:Panel>
        
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>
