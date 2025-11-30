
function element(ID) {
	return document.getElementById(ID);
}

function parentElement(ID) {
	return parent.document.getElementById(ID);
}

function SelectTextBox(ID) {
	var oTextBox = element(ID);
	oTextBox.focus();
	oTextBox.select();
}

function Trim(sValue) {
	return sValue.replace(/^\s*|\s*$/g,"");
}

function isIn(sValue, sList) {
	//Return true if variable is equal to any one of the phrases in the comma delimited list
	var sArray = sList.split(",");
	for (var i=0; i<sArray.length; i++) {
		if (sValue == sArray[i]) return true;
	}
	return false;
}

function PopUp(URL,W,H,SB,RS) {
	var W = Math.min(window.screen.availWidth,W);
	var H = Math.min(window.screen.availHeight,H);
	var Y = Math.round((window.screen.availHeight-H)/2);
	var X = Math.round((window.screen.availWidth-W)/2);
	if (SB==null) SB=0; 
	if (RS==null) RS=0;
	var Cmd = "toolbar=0,status=1,menubar=0,";
	Cmd += "resizable=" + RS + ",scrollbars=" + SB + ",";
	Cmd += "width=" + W + ",height=" + H + ",top=" + Y + ",left=" + X;
	return window.open(URL,"_blank",Cmd);
}

function getQueryString(url, key) {
	var qs = url.substr(url.indexOf("?")+1)
	var vars = qs.split("&");
	for (var i=0; i<vars.length; i++) {
		var pair = vars[i].split("=");
		if (pair[0] == key) {
			return pair[1];
		}
	}
	return null;
}

function HttpGet(url) {
	var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");	
	xmlhttp.open("GET",url,false);
	xmlhttp.send(null);
	return xmlhttp;
}

function HttpGetAsync(url,onReadyStateChange) {
	var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
	xmlhttp.onreadystatechange = onReadyStateChange;
	xmlhttp.open("GET",url,true);
	xmlhttp.send(null);
	return xmlhttp;
}
