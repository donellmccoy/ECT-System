<%@ Page Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Tools.Tools_DocumentUploadScan" Codebehind="DocumentUploadScan.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Document Scan</title>
   	<link href="../styles.css" type="text/css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="../Includes/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../includes/DocumentUploadScan.js" type="text/javascript"></script>
    <script src="../includes/Util.js" type="text/javascript"></script>
    <script type="text/javascript" for="DynamicWebTwain1" event="OnPostAllTransfers">
		<!--
		 DynamicWebTwain1_OnPostAllTransfers();
		//-->
	</script>
	<script type="text/javascript" for="DynamicWebTwain1" event="OnPreTransfer">
		<!--
		 DynamicWebTwain1_OnPreTransfer();
		//-->
	</script>
</head>
<body id="HtmlBody" runat="server">
    <form id="form1" runat="server">
		<input id="hActionPage" runat="server" type="hidden" />
		<input id="hMode" runat="server" type="hidden" />
		<input id="hResolution" runat="server" type="hidden" />
		<input id="hPageSide" runat="server" type="hidden" />
		<input id="hScanCustom" runat="server" type="hidden" />
		<input id="hReturnUrl" runat="server" type="hidden" />
		
		<div id="divMsg" runat="server" style="text-align:center; width:100%; margin-top:30px">
			<div id="lblLoadingMsg"><img src="../images/processing.gif" alt="" style="margin-right:10px" /><span class="title">Scanning in progress. Please wait.</span></div>
			<div id="lblStatus" style="padding:10px"></div>
		</div>
		
		<object height="0" width="0" classid="clsid:5220cb21-c88d-11cf-b347-00aa00a28331">
			<param name="LPKPath" value="../controls/DynamicWebTWAIN.lpk" />
		</object>
		<object id="DynamicWebTwain1" name="DynamicWebTwain1" codebase="../controls/DynamicWebTWAIN.cab#version=4,2,1" height="0" width="0" classid="clsid:E7DA7F8D-27AB-4EE9-8FC0-3FEC9ECFE758">
			<param name="_cx" value="26" />
			<param name="_cy" value="26" />
			<param name="JpgQuality" value="80" />
			<param name="Manufacturer" value="DynamSoft Corporation" />
			<param name="ProductFamily" value="Dynamic Web TWAIN" />
			<param name="ProductName" value="Dynamic Web TWAIN" />
			<param name="VersionInfo" value="Dynamic Web TWAIN 4.2" />
			<param name="TransferMode" value="0" />
			<param name="BorderStyle" value="0" />
			<param name="FTPUserName" value="" />
			<param name="FTPPassword" value="" />
			<param name="FTPPort" value="21" />
			<param name="HTTPUserName" value="" />
			<param name="HTTPPassword" value="" />
			<param name="HTTPPort" value="80" />
			<param name="ProxyServer" value="" />
			<param name="IfDisableSourceAfterAcquire" value="0" />
			<param name="IfShowUI" value="-1" />
			<param name="IfModalUI" value="-1" />
			<param name="IfTiffMultiPage" value="0" />
			<param name="IfThrowException" value="0" />
			<param name="MaxImagesInBuffer" value="1" />
			<param name="TIFFCompressionType" value="0" />
			<param name="IfFitWindow" value="-1" />
		</object>
    </form>
</body>
</html>
