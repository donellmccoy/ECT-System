<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_Rules" Codebehind="Rules.aspx.vb" %>

 
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <h2>Rules</h2>
  
<asp:GridView  ID="gvRules" Width="90%" runat="server"   AutoGenerateColumns="False" DataSourceID="LinqDataSource1" 
 EmptyDataText="No Record Found"     DataKeyNames="Id"  AllowPaging="True"    PageSize ="20"   AllowSorting="True">
            
 <Columns>
      <asp:TemplateField HeaderText="Rule ID">                  
             <ItemTemplate>
                  <asp:Label ID="Label2" runat="server" Text='<%# Bind("Id") %>'></asp:Label>
             </ItemTemplate>
             <ItemStyle Width="100px" />
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Rule Name">
  
         <ItemTemplate>
              <asp:Label ID="Label1" runat="server" Text='<%# Bind("name") %>'></asp:Label>
          </ItemTemplate>
          <ItemStyle Width="300px" />
         </asp:TemplateField>
                  <asp:TemplateField HeaderText="Rule Type">
                     <ItemTemplate>
                          <%#Eval("core_lkupRuleTypes.Description")%>
                     </ItemTemplate>
                     <EditItemTemplate > 
                        <asp:DropDownList ID="ddlRuleType" DataSourceID ="ruleTypeDataSource"
                            DataValueField="Id"  DataTextField ="Description"  SelectedValue='<%# Bind("ruleType") %>'
                             runat="server"
                            />
                    </EditItemTemplate>              
                    <ItemStyle Width="300px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Workflow">
                     <ItemTemplate>
                          <%#IIf(Eval("core_Workflow.title") Is Nothing, "All", Eval("core_Workflow.title"))%>
                          
                     </ItemTemplate>
                    <EditItemTemplate > 
                            <asp:DropDownList ID="ddlWorkFlowType" DataSourceID ="workflowDataSource"
                            DataValueField="workflowId"  DataTextField ="title" SelectedValue='<%# IIF(Eval("workflow") IS NOTHING,0,Eval("workflow")) %>'
                             runat="server"
                           />
                    </EditItemTemplate>
                    <ItemStyle Width="300px" />
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="RulePrompt">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPrompt"  TextMode ="MultiLine"  Height="40" runat="server" Text='<%# Bind("prompt") %>'></asp:TextBox>
                    </EditItemTemplate> 
                    <ItemTemplate>
                        <asp:Label ID="lblPrompt" runat="server" Text='<%# Bind("prompt") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="300px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Active">
                   <EditItemTemplate>
                        <asp:CheckBox ID="chkActive"  runat="server"  Checked ='<%# Bind("Active") %>'></asp:CheckBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                         <asp:CheckBox ID="chkActive2"  Enabled="false" runat="server"  Checked ='<%# Bind("Active") %>'></asp:CheckBox>
                    </ItemTemplate>                
                </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True"   ShowEditButton="True"  />
     </Columns> 
        </asp:GridView>
 
 
 <br /><br />
 <h2>
		Add New Rule</h2>
 	<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="LinqDataSource1"
			DefaultMode="Insert" CellPadding="2"   CellSpacing="2"     GridLines="None">
			<Fields>
				<asp:TemplateField HeaderText="Name:"  >
			    <InsertItemTemplate>
                     <asp:TextBox ID="txtAddName"  Text='<%# Bind("name") %>' runat="server"  ></asp:TextBox>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtAddName"
                     ErrorMessage="Name is required" ValidationGroup="insert">*</asp:RequiredFieldValidator>
                </InsertItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="Rule Type:"  >
                     <InsertItemTemplate > 
                        <asp:DropDownList ID="ddlRuleType" DataSourceID ="ruleTypeDataSource"
                            DataValueField="Id"  DataTextField ="Description"   SelectedValue='<%# Bind("ruleType") %>'
                             runat="server"
                            />
                   </InsertItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Workflow:"  >
                  <InsertItemTemplate > 
                        <asp:DropDownList ID="ddlAddWorkFlowType" DataSourceID ="workflowDataSource"
                        DataValueField="workflowId"  DataTextField ="title" SelectedValue='<%# IIF(Eval("workflow") IS NOTHING,0,Eval("workflow")) %>'
                         runat="server"
                       />
                 </InsertItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Prompt:">                  
                  <InsertItemTemplate>
                        <asp:TextBox ID="txtAddPrompt"  TextMode ="MultiLine"  Height="40" runat="server" Text='<%# Bind("prompt") %>'></asp:TextBox>
                    </InsertItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Active:">                  
                  <InsertItemTemplate>
                        <asp:CheckBox ID="chkAddActive" Checked ='<%# Bind("Active") %>' runat="server"  ></asp:CheckBox>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
					<InsertItemTemplate>
						<asp:Button ID="btnInsert" runat="server" CausesValidation="True" CommandName="Insert"
							Text="Add" Width="70px" ValidationGroup="insert"   />&nbsp;
							<asp:ValidationSummary       ID="ValidationSummary1" runat="server" ValidationGroup="insert" />
					</InsertItemTemplate>
			 
					         <ItemStyle Width="400px"  HorizontalAlign="Right"/>
       
				</asp:TemplateField>
			</Fields>
            <FieldHeaderStyle HorizontalAlign="Right" Width="90px" Wrap="False" />
		</asp:DetailsView>
 
 
        <asp:LinqDataSource ID="LinqDataSource1" runat="server" 
                ContextTypeName="ALOD.AFLODDataContext" EnableDelete="True" EnableInsert="True" 
                EnableUpdate="True" TableName="core_lkupRules">
        </asp:LinqDataSource>    
      <asp:LinqDataSource ID="workflowDataSource" runat="server" 
                ContextTypeName="ALOD.AFLODDataContext"    TableName="core_Workflows">        
     </asp:LinqDataSource>
      <asp:LinqDataSource ID="ruleTypeDataSource" runat="server" 
                          ContextTypeName="ALOD.AFLODDataContext"   TableName="core_lkupRuleTypes">
      </asp:LinqDataSource>
        &nbsp;<br /><br />
         
</asp:Content>
 
