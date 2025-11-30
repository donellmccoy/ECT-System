<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_Snapshot" Codebehind="Snapshot.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<style type="text/css">

    .content
    {
        padding:6px;       
        border-top:1px groove #666666;
        border-right:1px groove #666666;
        border-bottom:1px groove #333333;
        border-left:1px groove #333333;
        margin-left: 20px;
        margin-bottom: 20px;
        
    }

    .button
    {
        margin-bottom:6px;
    }

    .busy
    {
        font-weight:bold;
        background-color:Black;
        color:#FFFFFF;
        width:200px;
        padding:2px 20px 3px 4px;
    }


</style>


<script type="text/javascript">


    function Processing() {

        //start with a fresh dom
        $('#changeSet').remove();

        //create the main div
        var parent = $.create(
        "DIV",
        {
            "id": "changeSet"
            , "style": "height: 200px; width: 300px;"
        },
        [
            "DIV",
            {
                "id": "dvLoading",
                "class": "labelWait"
                //,"style" : "width: 596px; text-align: center; margin-top: 140px; border-width: 0px;"
            },
            [
                "IMG", { "src": $_HOSTNAME + "/images/busy.gif", "alt": "Loading" }, [],
                "span", {}, [" Processing . . . "]
            ]
        ]
    );

        $('body').append(parent);

        //show our dialog
        $.showDialog('#changeSet', { modal: true, position: 'center', width: 300, height: 200, title: 'Historical Copy Conversion' });

    }

</script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
    <h3>Forms 348/261 Historical Copy Conversion</h3>   

<div class="content" style="float:left;">

    <div style="float:left; padding-right:12px; border-right:1px solid #333333; width:300px;">
        <table>
        <tr>
            <td>Total Number of Records to Convert: </td>
            <td><asp:Label ID="lblTotal" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td>Enter number of Records to Convert:
            <br />
            </td>
            <td><asp:textbox ID="txtNumDocs" runat="server" Width="40px"></asp:textbox></td>
        </tr>
        <tr>
            <td colspan="2">
            <asp:RequiredFieldValidator 
                ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumDocs" 
                Display="Dynamic" ErrorMessage="RequiredFieldValidator" 
                ValidationGroup="GetHistDocs">Enter a number &lt;= Total Documents to Convert.</asp:RequiredFieldValidator>
                
                <asp:RangeValidator ID="RangeValidator1" runat="server" 
                    ControlToValidate="txtNumDocs" Display="Dynamic" ErrorMessage="RangeValidator" 
                    MinimumValue="0" SetFocusOnError="True" Type="Integer" 
                ValidationGroup="GetHistDocs">Enter a number &lt;= Total Documents to Convert.</asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:center;">
        <asp:Button ID="btnGet" runat="server" Text="Get Records" 
            Width="125px" CssClass="button" ValidationGroup="GetHistDocs" />&nbsp;&nbsp;
        <asp:Button ID="btnCreate" runat="server" Text="Create Snapshots" 
                    Enabled="False" Width="125px" CssClass="button" 
                    OnClientClick="Processing();" />

            </td>
        </tr>
        <tr>
            <td colspan="2">Starting Date/Time:&nbsp; <asp:Label ID="lblDate" runat="server"></asp:Label> </td>
        </tr>
        <tr>
            <td colspan="2">Ending Date/Time:&nbsp;&nbsp; <asp:Label ID="lblDate2" runat="server"></asp:Label> </td>
        </tr>   
        <tr>
            <td colspan="2"><asp:Label ID="lblError" runat="server"></asp:Label> 
                <br />
                <asp:Label ID="lblError0" runat="server"></asp:Label> </td>
        </tr>                              
        </table>   
    </div>


    <div style="float:left; margin-left:12px; width:600px;" class="align-left">
        <table>
        <tr>
            <td colspan="2"><strong>View Results</strong><br />
                Enter Date and Time range [mm/dd/yyyy] [hh:mm AM/PM]
            </td>
        </tr>
        <tr>
            <td class="align-right">Start: </td>
            <td class="align-left"><asp:TextBox ID="txtStartDate" runat="server" Width="70px" 
                    Wrap="False"></asp:TextBox>&nbsp; 
                <asp:TextBox ID="txtStartTime" 
                    runat="server" Width="60px" Wrap="False"></asp:TextBox>
                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                    ControlToValidate="txtStartDate" Display="Dynamic" 
                    ErrorMessage="A valid date is required." ValidationGroup="DateRange"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                    ControlToValidate="txtStartDate" Display="Dynamic" 
                    ErrorMessage="Enter a valid date [mm/dd/yyyy]." 
                    ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" 
                    ValidationGroup="DateRange"></asp:RegularExpressionValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                    ErrorMessage="Enter a valid time format [hh:mm AM/PM]." ControlToValidate="txtStartTime" 
                    ValidationExpression="^((0?[1-9]|1[012])(:[0-5]\d){0,2}(\s[AP][M]))$" 
                    ValidationGroup="DateRange"></asp:RegularExpressionValidator>
                </td>
        </tr>
        <tr>
            <td class="align-right">End: </td>
            <td class="align-left"><asp:TextBox ID="txtEndDate" runat="server" Width="70px" 
                    Wrap="False"></asp:TextBox>&nbsp; 
                <asp:TextBox ID="txtEndTime" 
                    runat="server" Width="60px" Wrap="False"></asp:TextBox>
                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                    ControlToValidate="txtEndDate" Display="Dynamic" 
                    ErrorMessage="A valid date is required." ValidationGroup="DateRange"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                    ControlToValidate="txtEndDate" Display="Dynamic" 
                    ErrorMessage="Enter a valid date [mm/dd/yyyy]." 
                    ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" 
                    ValidationGroup="DateRange"></asp:RegularExpressionValidator>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                    ErrorMessage="Enter a valid time format [hh:mm AM/PM]." ControlToValidate="txtEndTime" 
                    Display="Dynamic" 
                    ValidationExpression="^((0?[1-9]|1[012])(:[0-5]\d){0,2}(\s[AP][M]))$" 
                    ValidationGroup="DateRange"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnResults" runat="server" Text="View Results" 
                    Width="125px" CssClass="button" ValidationGroup="DateRange" /></td>
        </tr>    
        </table>    
    
    </div>


</div>


<br style="clear:both;" />
<div class="content">
    <asp:Label ID="lblRecords" runat="server" Visible="False"></asp:Label>
    &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="lblFiles" runat="server" Visible="False"></asp:Label>
    &nbsp;&nbsp;&nbsp;&nbsp;<br />

    <div id="divProcessed" runat="server" style="overflow:scroll; width:920px; height:300px; border:1px solid #666666;">

        <asp:GridView ID="gvHistorical" runat="server" Width="900px" 
            EmptyDataText="There are no records to process at this time."  
            CellPadding="2" PageSize="1">
            <HeaderStyle Font-Bold="True" Font-Size="10px" />
            <RowStyle Font-Size="10px" />
        </asp:GridView>


            <asp:GridView ID="gvProcessed" runat="server" Width="900px" 
                EmptyDataText="No records have been processed."  
                CellPadding="2" PageSize="1" 
                    EnableModelValidation="True">
                <HeaderStyle Font-Bold="True" Font-Size="10px" />
                <RowStyle Font-Size="10px" />
            </asp:GridView>


        <asp:GridView ID="gvResults" runat="server" Width="900px" 
            EmptyDataText="No records have been processed."  
            CellPadding="2" Enabled="False" Visible="False" PageSize="1">
            <HeaderStyle Font-Bold="True" Font-Size="10px" />
            <RowStyle Font-Size="10px" />
        </asp:GridView>


    </div>
      
    <br style="clear:both;" />


</div>

  <script type="text/javascript">
      function ShowWait() {
          document.getElementById('ctl00_ContentMain_lblProcessing').innerText = 'P R O C E S S I N G . . .';
          
      }
  </script>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" Runat="Server">
</asp:Content>

