<%@ Page AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_MsgAdmin"
	Language="VB" MasterPageFile="~/Secure/Secure.master" Codebehind="MsgAdmin.aspx.vb" %>
<%@ MasterType VirtualPath="~/Secure/Secure.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="../UserControls/popupUpdateMessage.ascx" TagName="ucMessageUpdate"
	TagPrefix="uc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentMain">

    <asp:Panel runat="server" ID="Panel1" CssClass="dataBlock">
        <div class="dataBlock-header">
            Messages
        </div>
        <div class="dataBlock-body">
            <asp:UpdatePanel ID="updMain" runat="server">
		        <ContentTemplate>
			        <asp:GridView ID="gvMessages" runat="server" AllowSorting="True"
				        AutoGenerateColumns="False" DataKeyNames="MessageID" DataSourceID="dataMessages"
				        PageSize="20" Width="900px">
				        <Columns>
					        <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
					        <asp:BoundField DataField="StartTime" DataFormatString="{0:MM/dd/yyyy}" HeaderText="Start Date"
						        HtmlEncode="False" SortExpression="StartTime">
						        <HeaderStyle Width="110px" />
					        </asp:BoundField>
					        <asp:BoundField DataField="EndTime" DataFormatString="{0:MM/dd/yyyy}" HeaderText="End Date"
						        HtmlEncode="False" SortExpression="EndTime">
						        <HeaderStyle Width="110px" />
					        </asp:BoundField>
					        <asp:CheckBoxField DataField="Popup" HeaderText="Popup" SortExpression="Popup">
						        <ItemStyle HorizontalAlign="Center" Width="40px" />
					        </asp:CheckBoxField>
					        <asp:TemplateField ItemStyle-Width="50px">
						        <ItemTemplate>
							        <asp:LinkButton ID="btnPopup" runat="server" CommandName="UpdatePopup" Text="Update"></asp:LinkButton>
						        </ItemTemplate>
					        </asp:TemplateField>
					        <asp:TemplateField ItemStyle-Width="50px">
						        <ItemTemplate>
							        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteMessage" Text="Delete"></asp:LinkButton>
							        <ajax:ConfirmButtonExtender runat="server" ID="ConfirmDelete" TargetControlID="btnDelete"
                                                ConfirmText="Are you sure you want to delete this message?" />
						        </ItemTemplate>
					        </asp:TemplateField>
				        </Columns>
				        <EmptyDataTemplate>
					        <center>
						        No messages returned.
					        </center>
				        </EmptyDataTemplate>
			        </asp:GridView>
			        <br />
			
		        </ContentTemplate>
	        </asp:UpdatePanel>
        </div>
    </asp:Panel>


    <asp:Panel runat="server" ID="pnlHyperLinks" CssClass="dataBlock">
            <div class="dataBlock-header">
                Hyper Links
            </div>

            <div class="dataBlock-body">
                <asp:Label runat="server" ID="lblLinkToParameterMessage" Font-Bold="true">* To insert a hyper link into the message place the reference name of a link inside curly brackets. For instance: {LINK_1}</asp:Label>
                <br />
                <asp:GridView runat="server" ID="gdvHyperLinks" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id" AllowPaging="true" PageSize="10">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Reference Name" ReadOnly="True" ItemStyle-Width="10%" />
                        <asp:BoundField DataField="Type.Name" HeaderText="Type" ReadOnly="True" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="DisplayText" HeaderText="Display Text" ReadOnly="True" ItemStyle-Width="30%" />
                        <asp:TemplateField HeaderText="Value">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblLinkValue" />
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        
                    </Columns>
                </asp:GridView>
                <br />
            </div>
        </asp:Panel>

    <asp:Button ID="btnAddMessage" runat="server" Text="Add Message"/>

    <br />
    <br />

    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <uc1:ucMessageUpdate runat="server" ID="UcEditMessage" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:ucMessageUpdate runat="server" ID="ucCreateMessage" />

        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:ObjectDataSource ID="dataMessages" runat="server" SelectMethod="RetrieveAllMessages"
		TypeName="ALODWebUtility.Common.MessageList">
		<SelectParameters>
			<asp:Parameter Name="compo" Type="Int32" />
			<asp:Parameter Name="isAdmin" Type="Boolean" />
		</SelectParameters>
	</asp:ObjectDataSource>
    
</asp:Content>
