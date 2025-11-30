<%@ Page Title="" Language="vb" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditICDCodes" CodeBehind="EditICDCodes.aspx.vb" EnableEventValidation="false" %>

<%@ Register Src="../UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">

    <div class="indent">
        <asp:Panel runat="server" ID="pnlSearch" CssClass="dataBlock">
            <div class="dataBlock-header">
                Select ICD Code
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlICDCodesBody" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlResults" CssClass="dataBlock">
            <div class="dataBlock-header">
                <asp:UpdatePanel runat="server" ID="upnlResultsHeader">
                    <ContentTemplate>
                        <asp:Label runat="server" ID="lblResultsPanelTitle" Text="Children of ICD Code - NONE SELECTED" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlResultsBody">
                    <ContentTemplate>
                        <asp:Label runat="server" ID="lblError" Visible="false" CssClass="labelRequired" />
                        <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" DataKeyNames="ICD9_ID">
                            <Columns>
                                <asp:TemplateField HeaderText="Code">
                                    <EditItemTemplate>
                                        <asp:Label ID="lblId" runat="server" CssClass="hidden" Text='<%# Bind("ICD9_ID")%>'></asp:Label>
                                        <asp:TextBox ID="txtValue" runat="server" Text='<%# Bind("value")%>' Width="75px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtValue" ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblId" runat="server" CssClass="hidden" Text='<%# Bind("ICD9_ID")%>'></asp:Label>
                                        <asp:Label ID="lblValue" runat="server" Text='<%# Bind("value")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="15%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("text")%>' Width="510px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtDescription" ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("text")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="61%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Is Disease">
                                    <EditItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkIsDisease" Checked='<%# Bind("isDisease")%>' Enabled="true" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkIsDisease" Checked='<%# Bind("isDisease")%>' Enabled="false" />
                                    </ItemTemplate>
                                    <ItemStyle Width="9%" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Active">
                                    <EditItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkActive" Checked='<%# Bind("Active")%>' Enabled="true" />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkActive" Checked='<%# Bind("Active")%>' Enabled="false" />
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center"/>
                                </asp:TemplateField>
                                <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Right" />
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>