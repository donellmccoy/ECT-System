<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_EditEmailTemplates" CodeBehind="EditEmailTemplates.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        
         <table style="width: 666px">
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td colspan="2" style="height: 28px">
                            <asp:Label ID="lblMsg" CssClass="valFailure" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                    <td></td>                  
                       <td   style="font-weight:bold"  >
                       
                           &nbsp;</td>
                       <td></td>                  
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            Title:</td>
                        <td style="width: 24px">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            <asp:TextBox ID="txtTitle" runat="server" Width="454px" MaxLength="50"></asp:TextBox></td>
                        <td>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtTitle"
                                Display="Dynamic" ErrorMessage="Title is a required field" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="lod">[Required]</asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            Subject:</td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            <asp:TextBox ID="txtSubject" runat="server" Width="454px" MaxLength="50"></asp:TextBox></td>
                        <td>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSubject"
                                Display="Dynamic" ErrorMessage="Subject is a required field" SetFocusOnError="True" ValidationGroup="lod" EnableClientScript="False">[Required]</asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td rowspan="1" style="width: 355px">
                            Body:</td>
                        <td rowspan="1" >
                        </td>
                    </tr>
                    <tr>
                        <td  style="width: 45px">
                        </td>
                        <td rowspan="2" style="width: 355px">
                            <asp:TextBox ID="txtBody" runat="server" Height="250px" TextMode="MultiLine" Width="454px"></asp:TextBox></td>
                        <td rowspan="2">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtBody"
                                Display="Dynamic" ErrorMessage="Email body is a required field" SetFocusOnError="True" ValidationGroup="lod" EnableClientScript="False">[Required]</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator1" OnServerValidate="BodyMultiLineValidator"
                                runat="server" Display="Dynamic" ErrorMessage="Please limit your e-mail body to 2000 characters"
                                SetFocusOnError="True" ControlToValidate="txtBody" ValidationGroup="lod">[limit to 2000 characters]</asp:CustomValidator></td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            Data Source:</td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            <asp:TextBox ID="txtDataSrc" runat="server" Width="454px" MaxLength="50"></asp:TextBox></td>
                        <td>
                            </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            <asp:CheckBox ID="chkActive" runat="server" Text="Active" /></td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45px">
                        </td>
                        <td style="width: 355px">
                            <br />
                            <asp:Button ID="btnAdd" runat="server" Text="Add a new Email Template" Visible="false"
                                CssClass="btnStructure" ValidationGroup="lod" />
                            <asp:Button ID="btnUpdate" runat="server" Visible="false" Text="Update an Email Template"
                                CssClass="btnStructure" ValidationGroup="lod" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnStructure" CausesValidation="False" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            
        </ContentTemplate>
        </asp:UpdatePanel>
               
          
    </div>
</asp:Content>
