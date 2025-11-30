
/*** Reference: http://www.dynamsoft.com/help/TWAIN4/WebTwain/index.htm#ActiveX ***/

var _uploadStarted = false;
var _DWT;
var _httpServer;
var _actionPage;
var _totalPages = 0;

function Scan() {
	_DWT = element("DynamicWebTwain1");
	_httpServer = document.location.host;
	_actionPage = element("hActionPage").value.replace("&amp;","&");
	
	var mode, resolution, pageSide, isCustomScan;
	mode = element("hMode").value;
	resolution = element("hResolution").value;
	pageSide = element("hPageSide").value;
	isCustomScan = (element("hScanCustom").value=="1");

	switch (_DWT.SourceCount) {
		case 1: _DWT.SelectSourceByIndex(0); break;
		case 0: 
			alert("There were no scanners found at this computer.");
			var html = "<div class='title'>Scanner Not Found</div>";
			var returnUrl = element("hReturnUrl").value;
			if (returnUrl!="") {
				html += "<p><a href='"+ returnUrl +"'>Return to Previous Page</a></p>";
			}
			element("lblLoadingMsg").innerHTML = html;
			return;
		default: _DWT.SelectSource(); break;
	}
	
	_DWT.OpenSource();
	_DWT.XferCount = -1;
	_DWT.IfFeederEnabled = true;
	_DWT.IfAutoFeed = true;
	_DWT.IfShowUI = isCustomScan; //Custom Settings
	
	var isSSL = (document.location.protocol == "https:");
	_DWT.HTTPPort = (isSSL)? 443 : 80;
	_DWT.IfSSL = isSSL;

	if (!_DWT.IfShowUI) {
		_DWT.PixelType = parseInt(mode); //Mode (B/W=0, Gray=1, RGB=2)
		_DWT.IfDuplexEnabled = (pageSide.toUpperCase() == "BOTH"); //Page Side
		_DWT.MaxImagesInBuffer = 1000; //Max number of pages

		//Resolution
		switch (resolution.toUpperCase()+"-"+mode) {
			case "HIGH-0": _DWT.CapValue = 300; break;
			case "HIGH-1": _DWT.CapValue = 300; break;
			case "HIGH-2": _DWT.CapValue = 300; break;
			
			case "NORMAL-0": _DWT.CapValue = 240; break;
			case "NORMAL-1": _DWT.CapValue = 240; break;
			case "NORMAL-2": _DWT.CapValue = 200; break;

			case "LOW-0": _DWT.CapValue = 200; break;
			case "LOW-1": _DWT.CapValue = 150; break;
			case "LOW-2": _DWT.CapValue = 100; break;
			default: _DWT.CapValue = 200; break;
		}
	}
	
	setTimeout("_DWT.AcquireImage();",1000);
}

function DynamicWebTwain1_OnPreTransfer() {
	element("lblStatus").innerHTML = "Scanning page " + (_DWT.HowManyImagesInBuffer + 1);
}

function DynamicWebTwain1_OnPostAllTransfers() {
	if (_uploadStarted) return; //Run this function only once
	_uploadStarted = true;
	_totalPages = _DWT.HowManyImagesInBuffer;
	
	if (_totalPages > 0) {
		UploadImage(0);
	} else {
		alert("There are no images in buffer to upload.");
	}
}

function UploadImage(imageIndex) {
   var msg = _totalPages + " page(s) scanned. Uploading " + (imageIndex+1) + " of " + _totalPages;
	element("lblStatus").innerHTML = msg;
	
	// Try multiple times to upload until success, or user cancels
	var Retry = true;
	while (Retry) {
		//Post file to ActionPage
		if (_DWT.HTTPUploadThroughPost(_httpServer, imageIndex, _actionPage, "temp.tif")) {
			Retry = false; //upload successful

		} else {
			
			var Msg = "Upload failed for Page " + (imageIndex+1)+ "; Try Again?\n" + 
			 "ErrorCode=" + _DWT.ErrorCode + "\n" +
			 "Error: " + _DWT.ErrorString + "\n" +
			 "SSL:" + _DWT.IfSSL + "\n" + 
			 "Port:" + _DWT.HTTPPort + "\n" +
			 "Server Response: " + _DWT.HTTPPostResponseString + "\n";
		
			if (!confirm(Msg)) { //cancel
				CloseWindow();
				return;
			}
		}
	}

	//Repeat call until all images are uploaded
	if (imageIndex < (_totalPages-1)) {
		setTimeout("UploadImage("+ (imageIndex+1) +");", 50); // pause to allow screen to refresh so counter is seen
	} else {
		//finished all uploads
		CloseWindow();
	}
}

function CloseWindow() {
	window.close();
}
