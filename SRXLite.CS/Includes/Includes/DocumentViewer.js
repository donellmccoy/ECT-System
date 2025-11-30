
var _zoomFactor = 1.6;
var _image;
var _thumbnail;
var _menu;
/////////////////////////////////////

function DocumentOnLoad() {
	var w;
	var pages = document.images;
	if (pages.length > 0) {
        //Preload thumbnails
	    for (var i=0; i < pages.length; i++) {
	        var img = pages[i];
            imgSrc = img.getAttribute("tsrc");
            if (imgSrc) {
                var preloadImg = new Image();
                preloadImg.src = imgSrc;
            }
        }
        
		//Images
		for (var i=0; i<pages.length; i++) {
			var img = pages[i];
			var imgSrc = img.getAttribute("xsrc");
			if (imgSrc) { //Fit image to window
				w = img.parentNode.scrollWidth - 30; //divImages
				img.style.width = w;
				img.src = imgSrc + w;
				img.style.display = "block";
			} else {
				//Thumbnails
				imgSrc = img.getAttribute("tsrc");
				if (imgSrc) {
					img.src = imgSrc;
					img.style.display = "block";
				}
			}
		}
	}
}


function getDefaultPageWidth() {
	return document.documentElement.clientWidth - element("divThumbnails").scrollWidth - 40;
}
function getImageSourceHeight() {
	return parseInt(getQueryString(_image.src,"h"));
}
function getImageSourceWidth() {
	return parseInt(getQueryString(_image.src,"w"));
}
function getImageSourceRotateType() {
	return getQueryString(_image.src,"r");
}
function getImageID() {
	return getQueryString(_image.src,"id");
}

function onClickImg(e,image) {
	DeselectImg(_image);
	DeselectImg(_thumbnail);
	
	if (image.getAttribute("for")) {
		//thumbnail image
		_thumbnail = image;
		_image = document.getElementById(_thumbnail.getAttribute("for"));
		_image.scrollIntoView();
		
	} else {
		//image
		_image = image;
		_thumbnail = document.getElementById(_image.getAttribute("thumbid"));
		_thumbnail.scrollIntoView();
	}
	
	SelectImg(_image);
	SelectImg(_thumbnail);

}

function onContextMenuImg(e,image) {
	onClickImg(e,image);
	DisplayMenu(e);
	return false;
}

function SelectImg(image) {
	if (image) image.className = "page pageActive";
}

function DeselectImg(image) {
	if (image) image.className = "page";
}

function DisplayMenu(e) {
	var menu = element("menu");
	var documentBody = document.documentElement;
	var x = e.clientX + documentBody.scrollLeft;
	var y = e.clientY + documentBody.scrollTop;
	var menuWidth = (menu.scrollWidth==0)? 201 : menu.scrollWidth+3;
	var menuHeight = (menu.scrollHeight==0)? 218 : menu.scrollHeight+3;

	//Adjust menu positioning if needed
	if ((menuWidth + e.clientX) > documentBody.clientWidth) x = x - menuWidth;
	if ((menuHeight + e.clientY) > documentBody.clientHeight) y = y - menuHeight;
		
	menu.style.left = x +'px';
	menu.style.top = y +'px';
	menu.style.display = "block";
	menu.focus();
}

function HideMenu() {
	element("menu").style.display = "none";
	return false;
}

function Zoom(dir) {
	var w = _image.width;
	var sourceWidth = getImageSourceWidth();
	var newWidth;
	
	switch (dir) {
		case 1: newWidth = Math.floor(_zoomFactor * w); break;
		case -1: newWidth = Math.floor((1/_zoomFactor) * w); break;
		default:
	}

	if (newWidth > sourceWidth) {
		_image.src = _image.src.replace("&w="+ sourceWidth, "&w="+ newWidth);
	}
	_image.style.height = "";
	_image.style.width = newWidth;
	_image.scrollIntoView(true);
}

var _xmlhttp;
function Rotate(r90) {
	if (r90 < 0) r90 += 4; //Counterclockwise
	var rotation = isNaN(_image.rotation)? r90 : ((_image.rotation + r90) % 4);
	_xmlhttp = HttpGet("../handlers/RotateImage.ashx?id="+ getImageID() +"&r="+ r90);
	if (_xmlhttp.status == 200) {
		_image.rotation = rotation;
		_image.style.filter = "progid:DXImageTransform.Microsoft.BasicImage(rotation="+ _image.rotation +")";
	} else {
		alert("Error: unable to save rotation.");
	}
}

function Resize(i) {
	var sourceWidth = getImageSourceWidth();
	var w, h;

	switch (i) {
		case 0: //Fit Width
			w = getDefaultPageWidth();
			_image.src = _image.src.replace("&w="+ sourceWidth, "&w="+ w);
			_image.style.height = "";
			_image.style.width = w;
			break;
			
		case 1: //Best Fit
			var sourceHeight = getImageSourceHeight();
			h = document.documentElement.clientHeight - 4; //subtract border width
			_image.src = _image.src.replace("&h="+ sourceHeight, "&h="+ h);
			_image.style.height = h;
			_image.style.width = "";
			break;
			
		case 2: //Actual Size
			_image.src = _image.src.replace("&w="+ sourceWidth, "&w=");
			_image.style.height = "";
			_image.style.width = "";
			break;
			
		default:
	}
	
	_image.scrollIntoView(true);
}

function Delete() {
	if (element("divContent").children.length==1) {
		alert("Unable to delete this page because it is the only page in the document.");
		
	} else if (confirm("Are you sure you want to delete this page?")) {
		PageMethods.DeletePage(getImageID(),OnDeleteSuccess,OnDeleteFailed);
	}
}
function OnDeleteSuccess(result) {
	//Remove image
	var imageContainer = element("divImages");
	imageContainer.removeChild(_image);
	_image = null;
	if (imageContainer.children.length==1) {
		menuDelete.style.display = "none";
	}
	
	//Remove thumbnail
	var thumbnailContainer = element("divThumbnails");
	_thumbnail.insertAdjacentHTML("afterEnd", "<div>Deleted</div>");
	thumbnailContainer.removeChild(_thumbnail);
	_thumbnail = null;
}
function OnDeleteFailed() {
	alert("Error: unable to delete page");
}

function Print() {
	window.print();
}

function CheckHotKeys(e) {
	if (!_image) return false;
	switch(e.keyCode) {
		case 107: Zoom(1); break; // +
		case 109: Zoom(-1); break; // -
		case 75: if (e.ctrlKey) Rotate(-1); break; // Ctrl+K
		case 76: if (e.ctrlKey) Rotate(1); break; // Ctrl+L
		case 87: if (e.ctrlKey) Resize(0); break; // Ctrl+W
		case 66: if (e.ctrlKey) Resize(1); break; // Ctrl+B
		case 65: if (e.ctrlKey) Resize(2); break; // Ctrl+A
		case 68: if (e.ctrlKey) Delete(); break; // Ctrl+D
		case 38: window.scrollBy(0,-100); break; // Up
		case 40: window.scrollBy(0,100); break; // Down
		case 37: window.scrollBy(-100,0); break; // Left
		case 39: window.scrollBy(100,0); break; // Right
		default: return;
	}
	return false;
}
