<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_ScatteredGraphReport" Codebehind="LODGraphReport.aspx.vb" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
                <div class="dataBlock">
                    <div class="dataBlock-header">
                        1 - Report Options
                    </div>
                    <div class="dataBlock-body">
                        <table>
                            <tr>
                                <td class="number">
                                    A
                                </td>
                                <td class="label">
                                    Type:
                                </td>
                                    <td class="value">
                                    <asp:Label runat="server" Text="Line of Duty (LOD)"></asp:Label>

                                </td>
                              
                                <td>
                                     <asp:Label style="text-align:right; padding-left:150px;" ID="MaxValueLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    B
                                </td>
                                <td class="label">Begin Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="BeginDateBox" runat="server" CssClass="datePicker" MaxLength="10"
                                        onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                                    <asp:CheckBox ID="PrevYears" runat="server" Text="Previous Years" style="margin-left: 20px;" />
                                </td>


                                <td>
                                     <asp:Label style="text-align:right; padding-left:150px;" ID="AverageLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    End Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="EndDateBox" runat="server" CssClass="datePicker" MaxLength="10"
                                        onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                                    &nbsp;
                                </td>
                                <td>
                                     <asp:Label style="text-align:right; padding-left:150px; " ID="TotalCasesLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    Formal:
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="IsFormalCheckbox" runat="server" />
                                </td>
                                <td>
                                     <asp:Label style="text-align:right; padding-left:150px; " ID="TargetLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    E
                                </td>
                                <td class="label">
                                    Horizontal Range:
                                </td>
                                <td class="value">
                                    <asp:CheckBox runat="server" ID="XAll" Text="Show All" Style="margin-right:20px;"/>
                                    <asp:Label runat="server" ID="XRangeLabel" Text="Value:" />
                                    <asp:TextBox runat="server" ID="XRange" style="width:30px;" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="XRange" runat="server" ErrorMessage="Numerical Value Only" ValidationExpression="\d+" ></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    Horizontal Groupings:
                                </td>
                                <td class="value">
                                    <asp:RadioButton runat="server" Text="1"  GroupName="HGroup" ID="RB1" Checked="true"/>
                                    <asp:RadioButton runat="server" Text="5"  GroupName="HGroup" ID="RB5"/>
                                    <asp:RadioButton runat="server" Text="10" GroupName="HGroup" ID ="RB10" />
                                    <asp:RadioButton runat="server" Text="20" GroupName="HGroup" ID="RB20"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    F
                                </td>
                                <td class="label">
                                    Action:
                                </td>
                                <td class="value">
                                    <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                                <td>
                                    <asp:Label ID="ErrorMessageLabel"  Width="300" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                </div>

                
                
               
        <asp:Chart ID="ResultChart1" runat="server" Width="800" Height="350" Style="margin-left: 60px;" >
            <Series>
                <asp:Series Name="Year1" Color="#005CE6" ChartType="Spline" ToolTip="Number of Cases: #VALY" BorderWidth="2" Legend="Legend">
                </asp:Series>
                <asp:Series Name="Year2" Color="#33CC33" ChartType="Spline" ToolTip="Number of Cases: #VALY" BorderWidth="2" Legend="Legend">
                </asp:Series>
                <asp:Series Name="Year3" Color="#E68A00" ChartType="Spline" ToolTip="Number of Cases: #VALY" BorderWidth="2" Legend="Legend">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea" >
                    <AxisX Title="Days to Complete" IsLabelAutoFit="true" IsMarginVisible="false" >
                        <LabelStyle Angle="-90" />
                    </AxisX>
                    <AxisY Title="Number of Cases" IsLabelAutoFit="false"  >
                    </AxisY>
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>

        <br />
        <asp:Label runat="server" ID="info" Text="*The graph displays days into groups. Example: If 5 is selected, 5 will represent the average of 1-5 , 10 will represent the average of 6-10, etc." style="font-size:x-small; float:right;"></asp:Label>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">

    <script type="text/javascript">

        $(function () {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        });  
    </script>
</asp:Content>

