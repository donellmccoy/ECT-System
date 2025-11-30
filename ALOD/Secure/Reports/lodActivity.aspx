<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_lodActivity" Codebehind="lodActivity.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <%--<asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">--%>
        <%--<ContentTemplate>--%>
            <div class="indent">
                <div id="rpt">
                    <div>
                        <asp:Repeater ID="rptNav" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkButton" runat="server"></asp:LinkButton>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <b>></b>
                            </SeparatorTemplate>
                        </asp:Repeater>
                    </div>
                    <br />
                    <div>
                        <uc1:ReportNav ID="rptTemplate" runat="server" />
                    </div>
                    <br />
                    <br />
                    <!-- <asp:GridView ID="ReportGrid" runat="server" Width="500px"  AutoGenerateColumns="False"
            >
            <Columns> 
             <asp:TemplateField HeaderText="Unit Name"   >
              <ItemTemplate>
                 <asp:LinkButton ID="NameLinkButton" runat="server" CausesValidation="false" CommandArgument='<%# CType(Eval("cs_id"),String)   +";"+ Eval("name") %>'
                                Visible='<%# Eval(“Total”) <>0 %>' CommandName="drill" Text='<%# Eval("name") %>'></asp:LinkButton>
                            <asp:Label ID="NameLinkLabel" runat="server" Text='<%# Eval("name") %>' Visible='<%# Eval(“Total”) =0 %>'></asp:Label>
              </ItemTemplate>
              </asp:TemplateField>
              <asp:BoundField DataField="Total" HeaderText="Number Of Cases"  />
            </Columns>
        </asp:GridView>
        </div>
        <br />-->
                    <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                        height: 22px;">
                        <div id="spWait" class="" style="display: none;">
                            &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                                ImageAlign="AbsMiddle" />&nbsp;Loading...
                        </div>
                    </div>
                    <div id="DetailDiv" visible="false" runat="server">
                        <div class="dataBlock-header">
                            List Of Cases
                        </div>
                        <asp:GridView ID="grvLODActivities" runat="server" Width="100%" AutoGenerateColumns="False"
                            AllowPaging="True" PageSize="20" AllowSorting="True">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="lodid" DataTextField="case_id" SortExpression="case_id"
                                    HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}" />
                                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                                <asp:BoundField DataField="description" HeaderText="Status" SortExpression="description" />
                                <asp:BoundField DataField="member_unit" HeaderText="Unit" SortExpression="member_unit" />
                                <asp:TemplateField HeaderText="Print">
                                    <ItemTemplate>

                                        <%--<asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" AlternateText="Print Forms"
                                            ImageUrl="~/images/pdf.ico" OnClientClick='<%# Eval("lodId", "printForms({0}); return false;") %>' />--%>

                                        <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" ImageUrl="~/images/pdf.ico" />

                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Record Found
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <br />
                    </div>
        <%--</ContentTemplate>--%>
<%--        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="rptTemplate" EventName="RptClicked" />
        </Triggers>
    </asp:UpdatePanel>--%>
<%--    <ajax:UpdatePanelAnimationExtender ID="resultsUpdatePanelAnimationExtender" runat="server"
        TargetControlID="resultsUpdatePanel">
        <Animations>
                <OnUpdating>
                    <ScriptAction script="searchStart();" />
                </OnUpdating>  
                <OnUpdated>     
                    <ScriptAction script="searchEnd();" />
                </OnUpdated>           
        </Animations>
    </ajax:UpdatePanelAnimationExtender>--%>
    </div> </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
    <script type="text/javascript">


        // View final forms 348, 261 if case is complete/final
       /*
        function viewDocs(path, path2) {
            var url = path;

            showPopup({
                'Url': url,
                'Width': 642,
                'Height': 668,
                'Center': true,
                'Resizable': true,
                'ScrollBars': true
            });

            // checks for Form 261
            var url2 = path2;
            var start = path2.indexOf("=");
            var docValue = path2.substr(start + 1, path2.length);

            if (docValue != null && docValue > 0) {
                showPopup({
                    'Url': url2,
                    'Width': 642,
                    'Height': 668,
                    'Center': false,
                    'Resizable': true,
                    'ScrollBars': true
                });
            }
        }
        */

    </script>
</asp:Content>
