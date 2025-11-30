<%@ Page Language="vb" AutoEventWireup="false" Inherits="ALOD.Web.DBSign.Signing" Codebehind="Signing.aspx.vb" %>
 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Signing</title>
    <script type="text/javascript" src="../../../Script/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="../../../Script/jquery-migrate-3.4.1.min.js"></script>

    <script type="text/javascript" src="../../../Script/jquery.blockUI.min.js"></script>
    <script type="text/javascript" src="../../../Script/common.js"></script>
    <!-- Darel Johnson -->
    <link type="text/css" href="../../../Script/DBSign/css/DBsignJSWS.css" rel="stylesheet" />
    <script type="text/javascript" src="../../../Script/DBSign/js/DBsignJSWS.js"></script>

    <%--<!-- Script functions -->
    <script type="text/javascript" language="JavaScript">
        var objDBsignUWS = null;
        // Callback that is called by UWS when it is fully initialized.
        function onReadyDBsignUWS() {
            objDBsignUWS = document.getElementById("DBsignUWS");
        }

        // Callback that is called by UWS it's initialization fails.
        function onInitFailedDBsignUWS() {
            window.location = "/failure.aspx";
        }

        // Show an error reported by the DBsign UWS
        function showError() {
            alert(objDBsignUWS.DBS_GetErrorField("ERROR_DESCRIPTION") +
            "\n\n" +
            "Error Code: " +
            objDBsignUWS.DBS_GetErrorField("DBS_ERROR_VAL") +
            "\n" + "Error Text: " + objDBsignUWS.DBS_GetErrorField("DBS_ERROR_MSG"));
        }

    </script>--%>

    <%--    <script type="text/javascript">
        $(function () {

            var failureLocation = "failure.aspx";
            var userIdParam = "UserId=0";
            var errorCodeParam = "DBS_ERROR_VAL=400"; //Dummy Error Code
            var errorMessage = "Java JRE not found or request timed out.";
            var errorMessageParam = "ERROR_DESCRIPTION=" + errorMessage;
            var nativeErrorCodeParam = "NATIVE_ERROR_VAL=0";
            var nativeErrorMesgParam = "NATIVE_ERROR_MSG=0";

            setTimeout(function () { window.location = failureLocation + "?" + userIdParam + "&" + errorCodeParam + "&" + errorMessageParam + "&" + nativeErrorCodeParam + "&" + nativeErrorMesgParam }, 90000);
        });
	</script>--%>

    <script type="text/javascript">
        var jsws = null;

        window.onload = function () {

            console.log("window.onload called.");
            jsws = new CDBsignJSWS({
                data_url: DBS_DATA_URL,
                data_url_sas: DBS_DATA_URL_SAS,
                mobile_license_id: DBS_MOBILE_LICENSE_ID,
                instance_name: DBS_INSTANCE_NAME,
                on_ready_callback: onJswsReady, 
                on_init_failure_callback: onJswsInitFailure
            });

            jsws.initialize();
        };

        function onJswsReady() {
            console.log("onJswsReady called.");
            console.log("SIGN_MODE: " + SIGN_MODE);

            if (SIGN_MODE === 'SignOnly') {
                console.log("SIGN_MODE === 'SignOnly' called.");
                console.log("DBS_DTBS: " + DBS_DTBS);
                signData(DBS_DTBS);
            }
            else {
                console.log("Else called.");
                console.log("DBS_TEMPLATE_NAME: " + DBS_TEMPLATE_NAME);
                console.log("DBS_PK_LIST: " + DBS_PK_LIST);
                signTemplate(DBS_TEMPLATE_NAME, DBS_PK_LIST);
            }
        }

        function onJswsInitFailure() {
            //console.log("onJswsInitFailure: redirect to error url...");
            alert("onJswsInitFailure: redirect to error url...");
            redirectUsingForm(DBS_FAILURE_URL, 'get', {
                'DBS_ERROR_VAL': '400', // Dummy Error Code ----- NOT A REAL DBsign CODE!!!
                'ERROR_DESCRIPTION': 'DBsign JSWS failed to initialize.'
                // More information could go here, but your existing code only deals with the parameters above.
            });
        }

        function signData(dtbs) {
            console.log("signData called.");
            if (jsws === null) {
                console.log("signData: redirect to error url...");
                redirectUsingForm(DBS_FAILURE_URL, 'get', {
                    'DBS_ERROR_VAL': '400', // Dummy Error Code ----- NOT A REAL DBsign CODE!!!
                    'ERROR_DESCRIPTION': 'DBsign JSWS is not initialized.'
                    // More information could go here, but your existing code only deals with the parameters above.
                });
            }
            else {
                console.log("jsws.DBS_AppSign called.");
                jsws.DBS_AppSign({
                    DBS_DTBS: dtbs,
                    DBS_DTBS_FMT: 'TEXT',
                    CALLBACK: onSignatureComplete
                });
            }
        }

        function signTemplate(templateName, templatePrimaryKeys) {
            console.log("signTemplate called.");
            if (jsws === null) {
                console.log("signTemplate: redirect to error url...");
                redirectUsingForm(DBS_FAILURE_URL, 'get', {
                    'DBS_ERROR_VAL': '400', // Dummy Error Code ----- NOT A REAL DBsign CODE!!!
                    'ERROR_DESCRIPTION': 'DBsign JSWS is not initialized.'
                    // More information could go here, but your existing code only deals with the parameters above.
                });
            }
            else {
                jsws.DBS_MakeSig({
                    TEMPLATE_NAME: templateName,
                    PK_LIST: templatePrimaryKeys,
                    CALLBACK: onSignatureComplete
                });
            }
        }

        function onSignatureComplete(objResponse) {
            console.log("onSignatureComplete called.");
            console.log(objResponse);

            var dbsErrorInfo = objResponse['DBS_ERROR_INFO'];

            if (dbsErrorInfo['DBS_ERROR_VAL'] != 0) {
                console.log("onSignatureComplete: redirect to error url...");
                redirectUsingForm(DBS_FAILURE_URL, 'post', {
                    'DBS_ERROR_VAL': dbsErrorInfo['DBS_ERROR_VAL'], // Dummy Error Code ----- NOT A REAL DBsign CODE!!!
                    'ERROR_DESCRIPTION': dbsErrorInfo['ERROR_DESCRIPTION']
                    // More information could go here, but your existing code only deals with the parameters above.
                });
            }
            else {
                console.log("onSignatureComplete: redirect to success url...");
                console.log(DBS_APP_SIG_PUT_URL);

                var resultInfo = objResponse['RESULT_INFO'];
                console.log(resultInfo);
                var signerInfo = resultInfo['DBS_SIGNER_INFO'];
                var signerDn = signerInfo['CERT']['SUBJECT_DN'];
                var signDate = signerInfo['SIGNING_TIME'];

                var redirectFunction = function () {
                    redirectUsingForm(DBS_SUCCESS_URL, 'post', {
                        'sgn': SGN
                    });
                };

                var redirectFunctionError = function () {
                    console.log("onSignatureComplete: Error posting signature data to " + DBS_APP_SIG_PUT_URL);
                    redirectUsingForm(DBS_FAILURE_URL, 'post', {
                        'DBS_ERROR_VAL': dbsErrorInfo['DBS_ERROR_VAL'], // Dummy Error Code ----- NOT A REAL DBsign CODE!!!
                        'ERROR_DESCRIPTION': dbsErrorInfo['ERROR_DESCRIPTION']
                        // More information could go here, but your existing code only deals with the parameters above.
                    });
                };

                if (DBS_APP_SIG_PUT_URL) {
                    //$.ajax({
                    //    method: 'POST',
                    //    url: DBS_APP_SIG_PUT_URL,
                    //    data: {
                    //        'DBS_SIGNER_DN': signerDn,
                    //        'DBS_SIGN_DATE': signDate
                    //        // More information could go here, but your existing code doesn't look for it.
                    //    },                    //    dataType: 'text',
                    //    success: redirectFunction,
                    //    error: redirectFunctionError
                    //});
					$.ajax(
						DBS_APP_SIG_PUT_URL,
						{
							method: "POST",
                            data: {
                                'DBS_SIGNER_DN': signerDn,
                                'DBS_SIGN_DATE': signDate
                            },
							dataType: 'text'
						}).done(redirectFunction).fail(redirectFunctionError);
                }
                else {
                    redirectFunction();
                }
            }

        }

        function redirectUsingForm(url, method, parameters) {
                console.log("redirectUsingForm: loading...");
                var form = document.createElement('form');
                form.action = url;
                form.method = method;
                console.log(form);
                if (parameters) {
                    for (var key in parameters) {
                        var input = document.createElement('input');
                        input.name = key;
                        input.value = parameters[key];
                        input.type = 'hidden';

                        form.appendChild(input);
                    }
                }
                console.log(form);
                document.body.appendChild(form);
                console.log("redirectUsingForm: Submitting form to success url...");
                form.submit();
            }
	</script>


</head>
<body style="background-color: #eee;">
    <form id="form" method="post" runat="server">
    <br /> 
    <img src="../../../images/sig_check.gif" alt="" />
    Signing Data...<br /><br />
	Please do not re-click button or refresh page<br />
	while this request is in progress.
    </form>
</body>
</html>
