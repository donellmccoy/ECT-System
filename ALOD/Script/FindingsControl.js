
function CheckDecision(decision, finding) {
    let radioName = $(radioElementById(finding + "_0")).attr("name");
    // decision : non concur
    if (radioElementById(decision).checked) {
        //$(radioElementById(finding)).attr("disabled", false);
        enableRadioList(radioName, true);
    }
    else {
        //$(radioElementById(finding)).attr("disabled", true);
        unCheckRadioList(radioName);
        enableRadioList(radioName, false);
    }
}

// Initialize event handlers for decision radio buttons to prevent double-firing
// This replaces the onclick attribute on the container with direct handlers on radio buttons
function InitializeDecisionToggleHandlers() {
    // Find all RadioButtonLists with decision toggle data attributes
    $('[data-decision-id][data-finding-id]').each(function() {
        var $container = $(this);
        var decisionId = $container.attr('data-decision-id');
        var findingId = $container.attr('data-finding-id');
        
        // Remove any existing handlers to prevent duplicates
        $container.find('input[type="radio"]').off('change.decisionToggle');
        
        // Attach change handler directly to radio buttons within this container
        // Using 'change' event instead of 'click' to avoid double-firing
        // and to handle both direct clicks and label clicks
        // Namespace the event ('change.decisionToggle') for easy removal
        $container.find('input[type="radio"]').on('change.decisionToggle', function(e) {
            // Stop propagation to prevent event from bubbling to container
            e.stopPropagation();
            CheckDecision(decisionId, findingId);
        });
    });
}

// Initialize on document ready
$(document).ready(function() {
    InitializeDecisionToggleHandlers();
});

// Re-initialize after UpdatePanel postbacks (if using ASP.NET AJAX)
if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
        InitializeDecisionToggleHandlers();
    });
}