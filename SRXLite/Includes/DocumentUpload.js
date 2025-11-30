///////////////////////////////////////////////////////////
// DocumentUpload.aspx
///////////////////////////////////////////////////////////
function SwitchInputMethod(value) {
	var isScan = (value=="Scan");
	element("trUpload").style.display = isScan? "none" : "";
	element("trScan").style.display = isScan? "" : "none";
	element("btnUploadScan").value = isScan? "Scan & Upload" : "Upload";
}

function ScanCustomToggle() {
	var checked = element("chkScanCustom").checked;
	var scanSettingElements = element("scanSettings").getElementsByTagName("select");
	for (i=0; i<scanSettingElements.length; i++) {
		scanSettingElements[i].disabled = checked;
	}
}

function CheckDate(txtDate) {
	var docDate = new Date(txtDate.value);
	var currentDate = new Date();

	if (docDate.getFullYear() < 1754) {
		alert("Document Date is invalid.");
		txtDate.select();
		return;
	}
	if (docDate > currentDate) {
		alert("Document Date cannot be a future date.");
		txtDate.select();
	}
}

function Cancel() {
	window.close();
}

function SubmitForm() {
    if (!element("regex_txtDocDescr").isvalid) {
        alert("Document Description is invalid.");
        return;
    }
    
	element("btnUploadScan").disabled = true;
	element("btnSubmit").click();
}
