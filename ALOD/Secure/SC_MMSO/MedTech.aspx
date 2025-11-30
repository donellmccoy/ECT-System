<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MMSO/SC_MMSO.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MMSO.Secure_sc_mmso_MedTech" ValidateRequest="false" Codebehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MMSO/SC_MMSO.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderNested" runat="Server">


    <style type="text/css">
        .style2
        {
            width: 100%;
        }
        .leftpad
        {
            padding-left: 10px;
        }
        .style5
        {
            width: 179px;
        }
        .style6
        {
            width: 179px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Pre-Authorization Request for Medical Care
        </div>
         <br />
        <asp:Panel style="border: thin solid #000000;" id="Panel1" runat="server">
                <b>Section 1 - Patient Data</b>
                <table style="width:100%;">
                    <tr>    
                        <td class="number">&nbsp;</td>
                        <td class="label labelRequired">
                            *Street&nbsp;Name:
                        </td>
                        <td>
                            <asp:TextBox ID="StreetTB" runat="server" Width="95%" MaxLength="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">&nbsp;</td>
                        <td class="label labelRequired">
                            *City:</td>
                        <td>
                            <asp:TextBox ID="cityTB" runat="server" Width="264px" MaxLength="40"></asp:TextBox>
                            &nbsp; State:<asp:DropDownList ID="stateDDL" runat="server">
                                <asp:ListItem></asp:ListItem>	<asp:ListItem Value="AL">Alabama</asp:ListItem>
	<asp:ListItem Value="AK">Alaska</asp:ListItem>
	<asp:ListItem Value="AZ">Arizona</asp:ListItem>
	<asp:ListItem Value="AR">Arkansas</asp:ListItem>
	<asp:ListItem Value="CA">California</asp:ListItem>
	<asp:ListItem Value="CO">Colorado</asp:ListItem>
	<asp:ListItem Value="CT">Connecticut</asp:ListItem>
	<asp:ListItem Value="DC">District of Columbia</asp:ListItem>
	<asp:ListItem Value="DE">Delaware</asp:ListItem>
	<asp:ListItem Value="FL">Florida</asp:ListItem>
	<asp:ListItem Value="GA">Georgia</asp:ListItem>
	<asp:ListItem Value="HI">Hawaii</asp:ListItem>
	<asp:ListItem Value="ID">Idaho</asp:ListItem>
	<asp:ListItem Value="IL">Illinois</asp:ListItem>
	<asp:ListItem Value="IN">Indiana</asp:ListItem>
	<asp:ListItem Value="IA">Iowa</asp:ListItem>
	<asp:ListItem Value="KS">Kansas</asp:ListItem>
	<asp:ListItem Value="KY">Kentucky</asp:ListItem>
	<asp:ListItem Value="LA">Louisiana</asp:ListItem>
	<asp:ListItem Value="ME">Maine</asp:ListItem>
	<asp:ListItem Value="MD">Maryland</asp:ListItem>
	<asp:ListItem Value="MA">Massachusetts</asp:ListItem>
	<asp:ListItem Value="MI">Michigan</asp:ListItem>
	<asp:ListItem Value="MN">Minnesota</asp:ListItem>
	<asp:ListItem Value="MS">Mississippi</asp:ListItem>
	<asp:ListItem Value="MO">Missouri</asp:ListItem>
	<asp:ListItem Value="MT">Montana</asp:ListItem>
	<asp:ListItem Value="NE">Nebraska</asp:ListItem>
	<asp:ListItem Value="NV">Nevada</asp:ListItem>
	<asp:ListItem Value="NH">New Hampshire</asp:ListItem>
	<asp:ListItem Value="NJ">New Jersey</asp:ListItem>
	<asp:ListItem Value="NM">New Mexico</asp:ListItem>
	<asp:ListItem Value="NY">New York</asp:ListItem>
	<asp:ListItem Value="NC">North Carolina</asp:ListItem>
	<asp:ListItem Value="ND">North Dakota</asp:ListItem>
	<asp:ListItem Value="OH">Ohio</asp:ListItem>
	<asp:ListItem Value="OK">Oklahoma</asp:ListItem>
	<asp:ListItem Value="OR">Oregon</asp:ListItem>
	<asp:ListItem Value="PA">Pennsylvania</asp:ListItem>
	<asp:ListItem Value="RI">Rhode Island</asp:ListItem>
	<asp:ListItem Value="SC">South Carolina</asp:ListItem>
	<asp:ListItem Value="SD">South Dakota</asp:ListItem>
	<asp:ListItem Value="TN">Tennessee</asp:ListItem>
	<asp:ListItem Value="TX">Texas</asp:ListItem>
	<asp:ListItem Value="UT">Utah</asp:ListItem>
	<asp:ListItem Value="VT">Vermont</asp:ListItem>
	<asp:ListItem Value="VA">Virginia</asp:ListItem>
	<asp:ListItem Value="WA">Washington</asp:ListItem>
	<asp:ListItem Value="WV">West Virginia</asp:ListItem>
	<asp:ListItem Value="WI">Wisconsin</asp:ListItem>
	<asp:ListItem Value="WY">Wyoming</asp:ListItem>

                            </asp:DropDownList>
                            &nbsp; Zip code:
                            <asp:TextBox ID="zipTB" runat="server" Width="93px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">&nbsp;</td>
                        <td class="label labelRequired">
                            *Phone number
                        </td>
                        <td>
                            <asp:TextBox ID="phoneTB" runat="server" Width="192px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">&nbsp;</td>
                        <td class="label labelRequired">
                            *TRICARE Region
                        </td>
                        <td>
                            <asp:DropDownList ID="tricareDDL" runat="server">
                                <asp:ListItem Text="North" Value="0" />
                                <asp:ListItem Text="South" Value="1" />
                                <asp:ListItem Text="West" Value="2" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
         </asp:Panel>
        <br/>
        <asp:Panel style="border: thin solid #000000;" id="Panel2" runat="server">
            <b>Section 2 - Pre-Authorization Request</b>
            <table style="width:100%;">
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Date of Injury/Illness
                    </td>
                    <td>
                        <asp:TextBox ID="Injury_Illness_DateTB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Duty Dates</td>
                    <td>
                        <asp:TextBox ID="DateInTB" runat="server"></asp:TextBox>
                        &nbsp;&nbsp;To:&nbsp;&nbsp;
                        <asp:TextBox ID="DateOutTB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label">
                        ICD9 Code</td>
                    <td>
                        <asp:TextBox ID="ICD9TB" runat="server" ReadOnly="True" BorderStyle="None" 
                            Width="50%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Diagnosis or Description<br /> of Injury/Illness
                    </td>
                    <td>
                        <asp:TextBox ID="DiagnosisTB" runat="server" Height="65px" MaxLength="500" TextMode="MultiLine"
                            Width="95%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *List Follow-up Care<br /> Requested
                    </td>
                    <td>
                        <asp:TextBox ID="FollowupCareTB" runat="server" Height="100px" MaxLength="1000" TextMode="MultiLine"
                            Width="95%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Provider
                    </td>
                    <td>
                        <asp:TextBox ID="ProviderTB" runat="server" MaxLength="100" Width="50%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Provider POC
                    </td>
                    <td>
                        <asp:TextBox ID="ProviderPOCTB" runat="server" MaxLength="100" Width="343px"></asp:TextBox>
                        &nbsp; POC Phone #
                        <asp:TextBox ID="ProviderPOCPhoneTB" runat="server" Width="156px" MaxLength="20"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label">
                        Medical Board Information
                    </td>
                    <td>
                        MTF Name:
                        <asp:TextBox ID="MTFNameTB" runat="server" Width="333px" MaxLength="200"></asp:TextBox>
                        &nbsp; Date:<asp:TextBox ID="MTFDateTB" runat="server" CssClass="datePicker"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style6 label labelRequired">
                        *Profile Information/<br /> Limited Duty Board Information
                    </td>
                    <td>
                        <asp:TextBox ID="ProfileInfoTB" runat="server" Height="75px" 
                            TextMode="MultiLine" Width="95%" MaxLength="500"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br/>
        <br/>
        <asp:Panel style="border: thin solid #000000;" id="Panel3" runat="server">
            <b>Section 3 - Unit Certification of Eligibility</b>
            <table class="style2">
                <tr>
                    <td>
                        &nbsp;</td>
                    <td class="style5" style="text-align: right">
                        Nearest Military Treatment Facility</td>
                    <td><hr/>
                        </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Name</td>
                    <td>
                        <asp:TextBox ID="NearestMTFNameTB" runat="server" MaxLength="100" 
                            Width="442px"></asp:TextBox>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Located</td>
                    <td>
                        <asp:TextBox ID="DistanceTB" runat="server" MaxLength="3" Width="68px"></asp:TextBox>
                        &nbsp;miles from the reservist&#39;s
                        <asp:RadioButtonList ID="MTFLocationType" runat="server" 
                            RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="1">Place of Duty</asp:ListItem>
                            <asp:ListItem Value="2">Residence</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td class="style5" style="text-align: right">
                        Current Unit of Assignment</td>
                    <td><hr/>
                        </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        Name:</td>
                    <td>
                        <asp:TextBox ID="UnitOfAssignmentNameTB" runat="server" MaxLength="75" 
                            Width="95%" ReadOnly="True"></asp:TextBox>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        UIC/OFPAC</td>
                    <td>
                        <asp:TextBox ID="UIC_OFPAC_TB" runat="server" Width="25%" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        Address (street, building number)</td>
                    <td>
                        <asp:TextBox ID="UnitAddressTB" runat="server" Width="95%" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        Address Line 2</td>
                    <td>
                        <asp:TextBox ID="UnitAddress2TB" runat="server" Width="95%" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        City</td>
                    <td>
                        <asp:TextBox ID="UnitCityTB" runat="server" Width="256px" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        State</td>
                    <td>
                        <asp:TextBox ID="UnitStateTB" runat="server" MaxLength="2" Width="68px" 
                            ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label">
                        Zip</td>
                    <td>
                        <asp:TextBox ID="UnitZipTB" runat="server" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td class="style5" style="text-align: right">
                        Unit POC</td>
                    <td><hr/>
                        </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Name</td>
                    <td>
                        <asp:TextBox ID="UnitPOCNameTB" runat="server" Width="493px" MaxLength="100"></asp:TextBox>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Rank</td>
                    <td>
                        <asp:DropDownList ID="UnitPOCRankDDL" runat="server" Width="200px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Title</td>
                    <td>
                        <asp:TextBox ID="UnitPOCTitleTB" runat="server" Width="366px" MaxLength="100"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        &nbsp;</td>
                    <td class="style5 label labelRequired">
                        *Phone #</td>
                    <td>
                        <asp:TextBox ID="UnitPOCPhoneTB" runat="server" Width="212px" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="MMSO_Unit" CssClass="sigcheck-form" EnableViewState="true" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        });        
    
    </script>
</asp:Content>
