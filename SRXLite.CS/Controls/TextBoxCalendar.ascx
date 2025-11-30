<%@ Control Language="C#" AutoEventWireup="true" Inherits="SRXLite.Web.Controls.Controls_TextBoxCalendar" CodeBehind="TextBoxCalendar.ascx.cs" %>
<asp:TextBox ID="txtDate" runat="server" MaxLength="10" Width="75"></asp:TextBox>&nbsp;<asp:image id="imgCalendar" runat="server" ImageUrl="~/Images/calendar.png" AlternateText="" style="cursor:pointer" />
<ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDate" Format="MM/dd/yyyy" PopupButtonID="imgCalendar" PopupPosition="BottomLeft" />
