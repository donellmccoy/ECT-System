<%@ Page Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" EnableEventValidation="false" Inherits="ALOD.Web.Public_Register1" Title="ECT Registration" CodeBehind="Register1.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" Runat="Server">
    <div style="padding: 10px 30px;">
        <div style="text-align: left;">
            <p style="text-align: center;">
                <strong>STANDARD MANDATORY NOTICE AND CONSENT PROVISION FOR ALL DOD INFORMATION SYSTEM
                    USER AGREEMENTS</strong>
            </p>
            <br />
            By signing this document, you acknowledge and consent that when you access Department
            of Defense (DoD) information systems:
            <br />
            <br />
        </div>
        
        <div style="text-align: left">
            <ul>
                <li>You are accessing a U.S. Government information system (as defined in CNSSI 4009)
                    that is provided for U.S. Government-authorized use only. </li>
            </ul>
        </div>
        <ul>
            <li>You consent to the following conditions: </li>
        </ul>
        <ul>
            <li>The government routinely monitors communications occurring on this information system,
                and any device attached to this information system, for purposes including, but
                not limited to, penetration testing, communications security (COMSEC) monitoring,
                network defense, quality control, employee misconduct investigations, law enforcement
                investigations, and counterintelligence investigations. </li>
            <li>At any time, the government may inspect and/or seize data stored on this information
                system and any device attached to this information system. </li>
            <li>Communications occurring on or data stored on this information system, or any device
                attached to this information system, are not private. They are subject to routine
                monitoring and search. </li>
            <li>Any communications occurring on or data stored on this information system, or any
                device attached to this information system, may be disclosed or used for any U.S.
                Government-authorized purpose. </li>
            <li>Security protections may be utilized on this information system to protect certain
                interests that are important to the government. For example, passwords, access cards,
                encryption or biometric access controls provide security for the benefit of the
                government. These protections are not provided for your benefit or privacy and may
                be modified or eliminated at the government's discretion.</li>
        </ul>
        <p style="text-align: right; margin-right: 20px;">
            &nbsp;
            <asp:Button ID="btnSign"  runat="server" Text="Sign And Continue"  />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
        </p>
<%--Sign and Continue modal--%>
            <asp:Label ID="modalLabel" runat="server" Text=""></asp:Label>
            <ajax:ModalPopupExtender ID="modalExtender" TargetControlID="modalLabel" PopupControlID="modalPan" 
                runat="server"></ajax:ModalPopupExtender>
      <asp:Panel ID="modalPan" runat="server" style="600px; margin-left: 16px;" Height="392px" 
            BorderColor="Black" BorderStyle="Solid" BackColor="White" Width="1208px">
        <div style="background-color:#0033CC; color:white; padding:2px; align-content:center; 
font-size: x-large; font-weight: bold; text-decoration: underline; text-align: center; height: 32px;">
                Please note the following:
            </div>
             <div style="font-weight: bold; text-decoration: underline">
                 &nbsp;New User Account     
            </div>
            &nbsp;&nbsp; If you are requesting a new user account, please contact your LOD-PM. 
                         To establish a new user account, both the AF Form 2875 and current IA 
                         Certificate are required.
            <div style="font-weight: bold; text-decoration: underline">
                Disabled Account    
            </div>
            &nbsp;&nbsp; If your account is disabled due to 90-days of inactivity, please send an email to 
                         the MHS Help Desk Email Address: afrc.mhs@us.af.mil. If your account was disabled, 
                         and has been enabled, you are required to login to ECT by 2400 hours on the same day 
                         or the account will become disabled again. You will have to contact the MHS Help Desk 
                         Email Address: afrc.mhs@us.af.mil. 
            <div style="font-weight: bold; text-decoration: underline">
                Expired IA Certificate    
            </div>
            &nbsp;&nbsp; If your account is disabled due to the expiration of an IA Certificate, please send a 
                         copy of the current IA Certificate, as an attachment, to the MHS Help Desk Email Address: 
                         afrc.mhs@us.af.mil. 
            <div style="font-weight: bold; text-decoration: underline">
                User Account Information Updates   
            </div>
            &nbsp;&nbsp; Please add or update your account information on the User Information Page. You will be 
                         routed to the User Information Page after clicking on the �Acknowledge� button below: 
            <br />
            <br />
            &nbsp; MHS Help Desk (Technical Support) Email Address: <a href="mailto:afrc.mhs@us.af.mil">afrc.mhs@us.af.mil</a> 
            <br />
            &nbsp; Technical Support: 1-888-577-2561, Option 2 
            <br />
            &nbsp; AFRC Functional Support: 1-888-577-2561, Option 3 
            <br />
            &nbsp; ANG Functional Support: 1-888-577-2561, Option 4 
            <br />
            <div align="center">
            <asp:Button ID="btnAck" runat="server" BorderColor="Black" Font-Bold="True" ForeColor="White" 
                Height="37px" Text="Acknowledge" Width="134px" BackColor="#0033CC" />
            </div>

       </asp:Panel>    
       </div>

</asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="ContentHeader">
    <style type="text/css">
        .btnSignModalUnderline; {
            height: 100px;
        }
        .btnSignModalUnderline; {
            height: 79px;
        }
    </style>



</asp:Content>

