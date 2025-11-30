<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_ManageEmailTemplate" Codebehind="ManageEmailTemplate.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" Runat="Server">
    <div>
     <table> 
        <tr>
            <td style="font-weight:bold"  >
                &nbsp;</td>
            <td class="align-left" style="width: 24px">
                &nbsp;</td>
        </tr>
        <tr>
          <td colspan="3">
            <br />
            <asp:Button ID="btnAdd" runat="server" Text="Add a new Email Template" />
          </td>
       </tr>  
  </table>
     
	<hr />
        
        
        <asp:GridView ID="gvemail" runat="server" AutoGenerateColumns="False" >
        <Columns>
			<asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" />
			<asp:TemplateField HeaderText="Title">
			    <ItemTemplate>
                    <asp:Label ID="lbltitle" runat="server"></asp:Label>
			    </ItemTemplate>
			</asp:TemplateField>
		    <asp:TemplateField HeaderText="Subject" ItemStyle-Wrap="false">
			    <ItemTemplate>
                    <asp:Label ID="lblsubject" runat="server"></asp:Label>
			    </ItemTemplate>
			</asp:TemplateField>
            <asp:TemplateField HeaderText="Body" ItemStyle-Wrap="false">
			    <ItemTemplate>
                    <asp:Label ID="lblbody" runat="server"></asp:Label>
			    </ItemTemplate>
			</asp:TemplateField>
            
            <asp:TemplateField HeaderText="Active" SortExpression="Active">
                <ItemTemplate>
                    <asp:CheckBox ID="chkActive" runat="server" Checked='<%# Bind("Active") %>' Enabled="false" />
               
               <ajax:ToggleButtonExtender ID="ToggleButtonExtender3" runat="server" CheckedImageAlternateText="Is Active"
                        CheckedImageUrl="~/images/check.gif" ImageHeight="16" ImageWidth="16" TargetControlID="chkActive"
                        UncheckedImageAlternateText="Not Active" UncheckedImageUrl="~/images/trans.gif">
                    </ajax:ToggleButtonExtender>
                </ItemTemplate>
                
            </asp:TemplateField>
			
			<asp:TemplateField ShowHeader="False">
				<ItemTemplate>

					<asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandArgument='<%# Eval("Id") %>'
						CommandName="Edit" OnCommand="EmailCommand" Text="Edit"></asp:LinkButton>
				
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
        </asp:GridView>

        
    </div>
</asp:Content>

