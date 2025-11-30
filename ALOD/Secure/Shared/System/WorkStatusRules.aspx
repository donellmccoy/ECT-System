<%@ Page Language="VB"  AutoEventWireup="false" MasterPageFile="~/Secure/Popup.master" Inherits="ALOD.Web.Sys.Secure_Shared_System_WorkStatusRules"  title="Rules Editor" Codebehind="WorkStatusRules.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

 <script type="text/javascript">
    
        function showEditor (url)
        {
            showPopup({
                'Url': url,
                'Width': '500',
                'Height': '300',
                'Resizable': true,
                'Center': false,
                'Reload' : false 
         
            });
        }
 </script>

  <div align="right">
    <input type="button" value="Return to Options" onclick="window.parent.close(); return false;"/>  
  </div>
 
  <div align="left"> 
  <table> 
         <tr> 
            <td>
            Compo:
            </td>
            <td align="left" >
                <asp:Label ID="lblCompo" CssClass="labelNormal" runat="server" Text=""></asp:Label>
           
            </td>
        </tr>
        <tr> 
            <td>
            Worlflow:
            </td>
            <td align="left" >
                <asp:Label ID="lblWorkflow" CssClass="labelNormal" runat="server" Text=""></asp:Label>
           
            </td>
        </tr>
         <tr> 
            <td>
            Step:
            </td>
            <td align="left" >
               <asp:Label ID="lblStep" CssClass="labelNormal" runat="server" Text=""></asp:Label>           
             </td>
        </tr>
         <tr> 
            <td>
            Option:
            </td>
            <td align="left" >
              <asp:Label CssClass="labelNormal" runat="server" id="lblOption" > </asp:Label>
            </td>
        </tr>
    </table>
</div>

<br />

    <h3>Current Rules</h3>
       <asp:GridView ID="gvRules" Width="840px" runat="server" AutoGenerateColumns="False"  
                     EmptyDataText="No Record Found"  DataKeyNames="wsr_id" >     
        <Columns>        
           <asp:TemplateField HeaderText="RuleName">
               <ItemTemplate>
                    <%#Eval("name")%>
                </ItemTemplate>
                <ItemStyle Width="150px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="RuleType">
               <ItemTemplate>
                    <%#Eval("Description")%>
                </ItemTemplate>
                <ItemStyle Width="150px" />
            </asp:TemplateField>
             <asp:TemplateField HeaderText="Data" ItemStyle-wrap="True"  >
               <ItemTemplate>
                    <asp:Label ID="lblRuleData" CssClass="wrapItem"  runat="server"  ></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>  
                
            <asp:TemplateField HeaderText="CheckAll">
               <ItemTemplate>
                    <%#Eval("checkAll")%>
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>    
             <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete" CommandArgument='<%#Eval("wsr_id") %>'
                        Text="Delete">
                    </asp:LinkButton>
                    <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure you want to delete this action?"
                        TargetControlID="LinkButton1">
                    </ajax:ConfirmButtonExtender>
                </ItemTemplate>
                <ItemStyle Width="200px"  HorizontalAlign="Right"/>
            </asp:TemplateField> 
        </Columns>         
    </asp:GridView>
     <br />
      <h3>Add New Rule:   
        </h3>  
       <asp:CheckBox ID="chkCopy"  AutoPostBack="true" Text="Show Copy Editor" runat ="server" ></asp:CheckBox>
      <div runat ="server" id="divAdd">  
       <table cellpadding="2">
           <tr>
            <td align="left" colspan="2" height="2"> 
                <asp:Label ID="lblEmpty" CssClass="labelNormal"  runat ="server" Text=" " ></asp:Label>
                 
            </td>
           </tr>
            <tr >
                <td colspan="2" height="5px">
                </td>
            </tr>
           <tr>
            <td width="100px">
                Type Of Rule:
            </td>
             <td align="left" width="150px">
                <asp:DropDownList ID="cbRuleType" runat="server" AutoPostBack="True">
                <asp:ListItem Text="Visibility" Value ="1" /> 
                <asp:ListItem Text="Validation" Value ="2" /> 
                </asp:DropDownList>
            </td> 
        </tr>     
        <tr ><td colspan="2" height="10px"></td> </tr>
        <tr> 
            <td width="100px">
                Rule Name:
             </td>
             <td align="left" width="300px">             
                <asp:DropDownList ID="cbRule" width="100%"  runat="server" AutoPostBack="True"  
                DataTextField="name" DataValueField="Id" ></asp:DropDownList> 
              </td>
         </tr>
          <tr>
             <td>
             </td>
            <td align="left">
                <asp:Label ID="lblPrompt" runat ="server" Text="Please input values" ></asp:Label>
             </td>
           </tr>
         <tr>               
              <td width="100px">               
                Data:
            </td>
             <td align="left" width="450px">  
                 <asp:TextBox ID="txtData"   width="150px"  visible="True" runat="server" ></asp:TextBox>
                 <asp:ListBox ID="lstData"    visible="false" width="300px"  runat="server"   Height="161px" SelectionMode="Multiple" > </asp:ListBox>&nbsp;  
                 
            </td>
        </tr>
         <tr>
             <td>
             </td>
             <td align="left" >
               <asp:CheckBox ID="chkAll" Text="Check if all selected values should be validated"  runat ="server"    />
             </td> 
         </tr>  
     </table>
     <br />
    <table>
     <tr>
       <td  align="right"  style="width:400px;">
         <asp:Button ID="btnAddRule" width="60px" runat="server"   Text="Add" />
         
         </td>
     </tr>
    </table>
    </div>
     <div id="divCopy" Visible="False" runat="server" > 
      Please note that the  existing rules will be overwritten by the rules from this source.  
       <table cellpadding="2">
           <tr>
            <td align="left" colspan="2" height="2"> 
                <asp:Label ID="Label1" CssClass="labelNormal"  runat ="server" Text=" " ></asp:Label>
                             </td>
           </tr> 
           <tr>
            <td width="100px">
                Step:
            </td>
             <td align="left" width="250px">
                <asp:DropDownList ID="cbSteps" runat="server" AutoPostBack="True"  
                    DataTextField="description" width="100%"  DataValueField="ws_id">
                </asp:DropDownList>
            </td> 
        </tr>     
        <tr> 
            <td width="100px">
                Option:
             </td>
             <td align="left" width="250px">             
                <asp:DropDownList ID="cbOptions" width="100%"  runat="server"  
                 ></asp:DropDownList> 
              </td>
         </tr>
        </table>
     <br />
    <table>
     <tr>
       <td  align="right"  width="400px">
         <asp:Button ID="btnCopy" width="250px" runat="server"   Text="Overwrite rules from this source" />
       </td>
     </tr>
    </table>  
    </div>     
   
</asp:Content>
