<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ICDCodeControl.ascx.vb" Inherits="ALOD.Web.UserControls.ICDCodeControl" %>


<asp:Label ID="lblICDDiagnosis" runat="server" />
<asp:Label ID="lblPostHTMLTags" runat="server" />

<asp:DropDownList ID="ddlICDChapter" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
<br />
<asp:DropDownList ID="ddlICDSection" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
<br />
<asp:DropDownList ID="ddlICDDiagnosisLevel1" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
<br />
<asp:DropDownList ID="ddlICDDiagnosisLevel2" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
<br />
<asp:DropDownList ID="ddlICDDiagnosisLevel3" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
<br />
<asp:DropDownList ID="ddlICDDiagnosisLevel4" runat="server" AutoPostBack="True" Width="600px" CssClass="cascade" />
