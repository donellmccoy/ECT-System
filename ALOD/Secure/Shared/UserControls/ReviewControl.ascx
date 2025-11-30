<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_ReviewControl" Codebehind="ReviewControl.ascx.vb" %>

<div  > 
<br /> 
Decision : 
   <asp:RadioButtonList ID="rblDecison" CssClass="fieldNormal" runat="server" RepeatDirection="Vertical">
         <asp:ListItem Value="1">Concur&nbsp;with&nbsp;action&nbsp;of&nbsp;Appointing&nbsp;Authority</asp:ListItem>
         <asp:ListItem Value="0">Non-Concur&nbsp;with&nbsp;action&nbsp;of&nbsp;Appointing&nbsp;Authority</asp:ListItem>
   </asp:RadioButtonList>
  <asp:Label ID="lblDecision" runat="server" CssClass="lblDisableText"></asp:Label>
<br />
 <br />  
Review and Recommendations:
 <br /> 
  
 <asp:TextBox ID="txtReview" runat="server" Height="76px" Width="366px"  TextMode="MultiLine"  ></asp:TextBox>
  
</div> 