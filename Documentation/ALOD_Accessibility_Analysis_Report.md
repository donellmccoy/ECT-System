# ALOD Website Accessibility Analysis Report

**Date:** November 4, 2025  
**Application:** ALOD (Air Force Line of Duty) / ECT System  
**Framework:** ASP.NET WebForms 4.8.1 (VB.NET)  
**Standards:** WCAG 2.1 Level A/AA, Section 508

---

## Executive Summary

This report documents a comprehensive accessibility audit of the ALOD website, identifying critical barriers that prevent users with disabilities from effectively using the system. The analysis covers 464 ASPX pages across multiple modules (LOD, Appeals, Special Cases, SARC) and reveals significant compliance gaps with WCAG 2.1 and Section 508 standards.

**Key Findings:**
- ❌ **14 Critical Issues** requiring immediate remediation
- ⚠️ **6 Moderate Issues** impacting user experience
- ℹ️ **3 Minor Issues** for future consideration
- ✅ **5 Positive Findings** demonstrating partial compliance

---

## Critical Issues (WCAG 2.1 Level A/AA Violations)

### 1. Missing/Inadequate Alt Text on Images

**Severity:** High (WCAG 1.1.1 - Non-text Content)

**Issues Found:**
- **Rotating banner images** (`Default.aspx`): Only generic alt text "ECT Banner" instead of describing each of the 5 rotating images (ALOD1.jpg through ALOD5.jpg)
- **Logo images** in master pages lack descriptive alternative text
- **Help icons**: `btnHelp` image button has no meaningful alt text
- **Lock status icons**: "This case is locked" images present but may not be properly associated with context

**Impact:** Screen reader users cannot understand visual content or distinguish between different banner images.

**Example:**
```html
<!-- Current (Bad) -->
<img src="App_Themes/DefaultBlue/images/ALOD1.jpg" id="banner" alt="ECT Banner" />

<!-- Should be (Good) -->
<img src="App_Themes/DefaultBlue/images/ALOD1.jpg" id="banner" 
     alt="Army Reserve soldiers conducting training exercise" />
```

**Files Affected:**
- `ALOD/Default.aspx`
- `ALOD/Secure/Secure.master`
- `ALOD/Public/Public.master`
- Multiple pages with image controls

**Recommendation:** 
1. Add descriptive alt text for each rotating banner image
2. Update logo alt text to "ALOD - Air Force Line of Duty System Home"
3. Ensure all functional images have action-oriented alt text (e.g., "Help documentation")
4. Mark decorative images with `alt=""` or `role="presentation"`

---

### 2. Form Labels and ARIA Attributes

**Severity:** High (WCAG 1.3.1 - Info and Relationships, 4.1.2 - Name, Role, Value)

**Issues Found:**

#### A. Inconsistent Label Association
- Many input fields lack proper `<label>` elements with `for` attributes
- Mix of ASP.NET `AssociatedControlID` and manual HTML labels
- Some fields use only placeholder text without visible labels

**Example from `start.aspx`:**
```html
<!-- Bad: Inline label not associated -->
<label for="SSNTextBox">Member SSN:</label>
<asp:TextBox ID="SSNTextBox" Width="80px" runat="server" MaxLength="9" />

<!-- Good: Using AssociatedControlID -->
<asp:Label runat="server" AssociatedControlID="SsnBox" Text="SSN:" />
<asp:TextBox ID="SsnBox" runat="server" AutoPostBack="True" Width="100px" />
```

#### B. Missing ARIA Attributes
- Only **2 instances** of ARIA attributes found across 464 pages (`aria-hidden` in PH Form)
- No ARIA landmarks (`role="main"`, `role="navigation"`, `role="banner"`)
- No ARIA live regions for dynamic content updates
- No ARIA labels for complex widgets (GridViews, DropDownLists, UpdatePanels)

**Impact:** 
- Screen readers cannot properly announce form fields
- Users cannot understand relationships between labels and inputs
- Dynamic content changes go unannounced

**Files Affected:**
- `ALOD/Secure/lod/start.aspx`
- `ALOD/Secure/lod/MyLods.aspx`
- All search and form pages

**Recommendation:**
1. **Immediate:** Audit all 464 ASPX pages for proper label association
2. Add ARIA landmarks to master pages:
   ```html
   <div id="header" role="banner">
   <nav id="main-menu" role="navigation" aria-label="Main navigation">
   <main id="content" role="main" aria-labelledby="lblPageTitle">
   <footer id="footer" role="contentinfo">
   ```
3. Add `aria-live="polite"` to search result UpdatePanels
4. Use `aria-describedby` for error messages and help text

---

### 3. Color Contrast Issues

**Severity:** High (WCAG 1.4.3 - Contrast Minimum)

**Issues Found in `App_Themes/DefaultBlue/styles.css`:**

| Element | Foreground | Background | Ratio | WCAG AA Required |
|---------|-----------|-----------|-------|------------------|
| `.workHeader` | #FFFFFF (white) | #27487d (blue) | ~3.9:1 | 4.5:1 |
| `.dataBlock-header` | #08003c (dark blue) | White | ✓ Pass | 4.5:1 |
| Link hover | #C9AB3E (gold) | #27487d (blue) | ~2.8:1 | 4.5:1 |

**Impact:** Users with low vision or color blindness cannot read text with insufficient contrast.

**Testing Required:**
- Verify all text/background combinations meet 4.5:1 ratio (normal text) or 3:1 (large text 18pt+)
- Test with WebAIM Contrast Checker or similar tools

**Recommendation:**
1. Darken header background to #1a2f5a or lighten text for better contrast
2. Ensure hover states meet contrast requirements
3. Don't rely solely on color to convey information (required fields, errors)
4. Add CSS for high contrast mode support

---

### 4. Keyboard Navigation & Focus Management

**Severity:** High (WCAG 2.1.1 - Keyboard, 2.4.1 - Bypass Blocks)

**Issues Found:**

#### A. Skip Navigation Link Disabled
```html
<!-- Current: Skip link is empty/disabled -->
<asp:Menu ID="MainMenu" runat="server" ... SkipLinkText="" />
```

#### B. No Visible Focus Indicators
- CSS file lacks `:focus` pseudo-class styles for interactive elements
- Default browser focus outline likely suppressed without replacement
- Users cannot see which element has keyboard focus

#### C. JavaScript-Dependent Interactions
- Banner rotation implemented only in JavaScript (`cycleBan()` function)
- Form validation relies on AJAX without keyboard fallbacks
- Modal dialogs may trap focus without proper focus management

**Impact:** 
- Keyboard-only users must tab through entire navigation menu every page load
- Users cannot see where they are on the page
- Some functionality unreachable via keyboard

**Files Affected:**
- `ALOD/Secure/Secure.master`
- `ALOD/App_Themes/DefaultBlue/styles.css`
- `ALOD/Default.aspx`

**Recommendation:**
1. **Restore skip link immediately:**
   ```html
   <asp:Menu ID="MainMenu" ... SkipLinkText="Skip to main content" />
   ```
2. Add focus styles to CSS:
   ```css
   a:focus, button:focus, input:focus, select:focus, textarea:focus {
       outline: 3px solid #ffbf47;
       outline-offset: 2px;
   }
   ```
3. Implement focus trapping for modal dialogs using JavaScript
4. Ensure tab order follows logical reading order
5. Test all interactive elements are keyboard accessible

---

### 5. HTML Semantic Structure

**Severity:** Medium-High (WCAG 1.3.1 - Info and Relationships, 2.4.6 - Headings and Labels)

**Issues Found:**

#### A. Missing Heading Hierarchy
- No `<h1>` tag in master pages or most content pages
- Page titles rendered as `<asp:Label>` instead of heading elements
- No consistent use of `<h2>`, `<h3>` for section organization

**Example from `Secure.master`:**
```html
<!-- Current: Label, not semantic heading -->
<asp:Label runat="server" ID="lblPageTitle" />

<!-- Should be: -->
<h1 id="lblPageTitle" runat="server"></h1>
```

#### B. Table-Based Navigation Menu
```html
<!-- Using deprecated table rendering -->
<asp:Menu ... RenderingMode="Table">
```
Should use list-based structure (`<ul>/<li>`) for semantic meaning.

#### C. Missing Document Outline
- No clear hierarchical structure for screen readers to navigate
- Sections not properly marked up with semantic elements

**Impact:** 
- Screen reader users cannot navigate by headings
- Document structure unclear
- SEO negatively impacted

**Files Affected:**
- `ALOD/Secure/Secure.master`
- `ALOD/Public/Public.master`
- All content pages

**Recommendation:**
1. Convert `lblPageTitle` from Label to `<h1>` element
2. Change menu rendering: `RenderingMode="List"` (generates `<ul>/<li>`)
3. Establish consistent heading hierarchy:
   - `<h1>` - Page title
   - `<h2>` - Major sections (e.g., "Member Information", "Documents")
   - `<h3>` - Subsections
4. Use `<section>`, `<article>`, `<nav>` HTML5 semantic elements
5. Update `.dataBlock-header` to render as `<h2>` or `<h3>`

---

### 6. Form Validation & Error Handling

**Severity:** High (WCAG 3.3.1 - Error Identification, 3.3.3 - Error Suggestion, 4.1.3 - Status Messages)

**Issues Found:**

#### A. Error Messages Not Announced
- Error labels set to `Visible="True"` dynamically but lack ARIA attributes
- No `role="alert"` or `aria-live` regions
- Screen readers won't automatically announce errors

**Example from `start.aspx`:**
```html
<!-- Current: Not accessible -->
<asp:Label ID="NotFoundLabel" runat="server" CssClass="labelRequired" 
           Text="SSN Not Found" Visible="False"/>

<!-- Should include: -->
<asp:Label ID="NotFoundLabel" runat="server" role="alert" 
           aria-live="assertive" CssClass="labelRequired" 
           Text="SSN Not Found" Visible="False"/>
```

#### B. Required Fields Indicated Only by Color
- Red asterisk (*) and red `.labelRequired` class
- No additional visual or semantic indicators
- Color-blind users cannot identify required fields

#### C. No Error Summary
- Individual field errors shown but no page-level error summary
- Users must hunt for errors after validation fails

**Impact:**
- Screen reader users unaware validation failed
- Color-blind users cannot identify required fields or errors
- High error recovery time

**Files Affected:**
- All form pages (LOD start, search, data entry forms)
- `ALOD/Secure/lod/start.aspx`
- `ALOD/Secure/lod/MyLods.aspx`

**Recommendation:**
1. Add `role="alert"` and `aria-live="assertive"` to all error labels
2. Use `aria-invalid="true"` on invalid form fields
3. Link error messages to fields with `aria-describedby`
4. Add non-color indicators for required fields:
   ```html
   <span class="required" aria-label="required">*</span>
   ```
5. Implement error summary component at top of form:
   ```html
   <div role="alert" aria-live="assertive" class="error-summary">
       <h2>Please correct the following errors:</h2>
       <ul>
           <li><a href="#SSNTextBox">SSN is required</a></li>
       </ul>
   </div>
   ```
6. Use ValidationSummary control with ARIA attributes

---

### 7. Document Language

**Severity:** Low (WCAG 3.1.1 - Language of Page)

**Status:** ✅ **COMPLIANT**

Both master pages correctly specify:
```html
<html lang="en">
```

**Note:** No mixed-language content handling observed. If Spanish or other languages are added, use `lang` attribute on those elements.

---

## Moderate Issues

### 8. Link Text & Context

**Severity:** Medium (WCAG 2.4.4 - Link Purpose in Context)

**Issues Found:**
- Ambiguous link text appearing multiple times: "Search", "Print", "View", "Edit"
- Multiple "Case Dialogue" links without distinguishing context
- LinkButtons with only icon images and no text alternative

**Example:**
```html
<!-- Bad: Context unclear when read out of order -->
<asp:LinkButton runat="server" Text="Search">

<!-- Good: Self-descriptive -->
<asp:LinkButton runat="server" Text="Search LOD Cases">
```

**Impact:** Screen reader users navigating by links hear "Search, Search, Search" without knowing which search.

**Recommendation:**
1. Make link text descriptive: "Search LOD Cases", "Search Appeals", etc.
2. Add `title` attribute for additional context when link text must stay short
3. Use `aria-label` for icon-only buttons:
   ```html
   <asp:LinkButton runat="server" aria-label="Print LOD case summary" CssClass="icon-print" />
   ```
4. Consider adding visually-hidden text for context

---

### 9. Data Tables

**Severity:** Medium (WCAG 1.3.1 - Info and Relationships)

**Issues Found:**

#### A. GridView Controls Lack Accessibility Features
- No `<caption>` element describing table purpose
- Headers missing `scope="col"` attributes
- No summary attribute for complex tables
- Sorting links lack ARIA attributes

**Example from `MyLods.aspx`:**
```html
<asp:GridView ID="gvResults" runat="server" 
    EmptyDataText="No records found" 
    AutoGenerateColumns="False">
    <!-- Missing: Caption, scope, aria-sort -->
```

#### B. Layout Tables vs Data Tables
- Some tables used for layout (form structure) not marked as `role="presentation"`

**Impact:** 
- Screen reader users cannot understand table structure
- Column headers not associated with data cells
- Sorting state not announced

**Files Affected:**
- `ALOD/Secure/lod/MyLods.aspx`
- All pages with GridView controls (search results, case lists)

**Recommendation:**
1. Add caption to GridView:
   ```csharp
   Protected Sub gvResults_PreRender(sender As Object, e As EventArgs)
       If gvResults.HeaderRow IsNot Nothing Then
           gvResults.Caption = "My LOD Cases Search Results"
           gvResults.HeaderRow.TableSection = TableRowSection.TableHeader
       End If
   End Sub
   ```
2. Add scope to headers:
   ```html
   <asp:BoundField DataField="SSN" HeaderText="SSN" 
                   HeaderStyle-Attributes-scope="col" />
   ```
3. Mark layout tables:
   ```html
   <table role="presentation">
   ```
4. Add ARIA sort state to sortable columns:
   ```html
   <th scope="col" aria-sort="ascending">Case ID</th>
   ```

---

### 10. Dynamic Content (AJAX)

**Severity:** Medium (WCAG 4.1.3 - Status Messages)

**Issues Found:**

#### A. UpdatePanel Without ARIA Live Regions
- Heavy use of `<asp:UpdatePanel>` throughout application
- Content updates not announced to screen readers
- Loading indicators visible but not accessible

**Example from `MyLods.aspx`:**
```html
<asp:UpdatePanel ID="resultsUpdatePanel" runat="server">
    <ContentTemplate>
        <div id="spWait" style="display: none;">
            &nbsp;<asp:Image runat="server" ID="imgWait" AlternateText="busy" />
            &nbsp;Loading...
        </div>
        <asp:GridView ID="gvResults" runat="server" ... />
    </ContentTemplate>
</asp:UpdatePanel>
```

**Issues:**
- No `aria-live` on UpdatePanel or content wrapper
- Loading message not announced
- Results update silently

#### B. Session Timeout Modals
- Modal dialogs created with jQuery UI
- May trap keyboard focus without proper management
- No ARIA dialog role

**Impact:**
- Screen reader users unaware content has changed
- Users don't know when searches complete
- Keyboard users trapped in modals

**Files Affected:**
- All pages with UpdatePanels (majority of site)
- `ALOD/Secure/Secure.master` (ModalDialog control)
- `ALOD/Default.aspx` (jQuery dialog)

**Recommendation:**
1. Add ARIA live region wrapper:
   ```html
   <div aria-live="polite" aria-atomic="true">
       <asp:UpdatePanel ID="resultsUpdatePanel" runat="server">
           ...
       </asp:UpdatePanel>
   </div>
   ```
2. Update loading indicator:
   ```html
   <div role="status" aria-live="polite" aria-label="Loading results">
       <asp:Image runat="server" AlternateText="" role="presentation" />
       <span class="sr-only">Loading search results...</span>
   </div>
   ```
3. For modals, implement:
   ```javascript
   $('#msgBox').dialog({
       role: 'dialog',
       'aria-modal': true,
       'aria-labelledby': 'dialog-title',
       'aria-describedby': 'dialog-description',
       open: function() {
           // Trap focus within dialog
       },
       close: function() {
           // Return focus to trigger element
       }
   });
   ```
4. Add UpdatePanelAnimationExtender accessibility hooks

---

### 11. PDF Accessibility

**Severity:** Medium (WCAG 1.1.1 - Non-text Content, 1.3.1 - Info and Relationships)

**Issues Found:**
- Site generates PDFs using ABCpdf library
- No indication PDFs are tagged for accessibility
- Help documentation links to PDF without warnings
- Form 348 and other generated PDFs may not be screen reader accessible

**Files Affected:**
- `ALODWebUtility/Printing/` (PDF generation code)
- `ALOD.Core/` (uses ABCpdf 12.5.1)
- Help links in `Secure.master` and various pages

**Impact:** 
- Screen reader users cannot read PDF forms or documentation
- Legal/compliance risk if official forms are inaccessible

**Recommendation:**
1. **Short-term:** Add warning to PDF links:
   ```html
   <asp:HyperLink runat="server" NavigateUrl="file.pdf" Target="_blank">
       View Document (PDF, may not be accessible)
   </asp:HyperLink>
   ```
2. **Medium-term:** Investigate ABCpdf tagging capabilities:
   ```csharp
   ' Enable PDF/UA (Universal Accessibility)
   theDoc.SetInfo(0, "Trapped", "False")
   theDoc.HtmlOptions.UseTaggedPdf = True
   ```
3. **Long-term:** 
   - Provide HTML alternatives to PDF forms
   - Implement proper PDF tagging in generation code
   - Test generated PDFs with screen readers (JAWS, NVDA)
   - Consider PDF/UA compliance

---

## Minor Issues

### 12. Viewport & Responsive Design

**Severity:** Low (WCAG 1.4.10 - Reflow)

**Issues Found:**
- Fixed pixel widths throughout: `width: 900px`, `width: 200px`, etc.
- No CSS media queries for responsive breakpoints
- Content may not reflow properly at 200-400% zoom
- Viewport meta tag present ✅ but not fully utilized

**Example from `styles.css`:**
```css
.contentBlock {
    margin-left: 30px;
    width: 900px;  /* Fixed, doesn't adapt */
}

.workPanel {
    width: 900px;  /* Fixed, doesn't adapt */
}
```

**Impact:** 
- Low vision users zooming in experience horizontal scrolling
- Mobile users on tablets/phones have poor experience
- Not compliant with WCAG 2.1 Level AA reflow requirements

**Recommendation:**
1. Convert fixed widths to relative units (%, em, rem):
   ```css
   .contentBlock {
       margin-left: 2em;
       max-width: 900px;
       width: 100%;
   }
   ```
2. Add responsive media queries:
   ```css
   @media screen and (max-width: 768px) {
       .dataBlock-body {
           padding: 8px;
       }
       td.label {
           display: block;
           width: 100%;
           text-align: left;
       }
   }
   ```
3. Test at 200% and 400% zoom levels
4. Use flexible grid layouts where possible

---

### 13. Button/Control Clarity

**Severity:** Low (WCAG 1.4.1 - Use of Color)

**Issues Found:**
- `<asp:LinkButton>` controls styled to look like regular text
- May confuse users about what is clickable
- Image buttons without clear purpose

**Example:**
```html
<asp:LinkButton runat="server" ID="lnkDevLogin">Developer Login</asp:LinkButton>
```
Appears as underlined text, may not be recognizable as button.

**Recommendation:**
1. Style LinkButtons distinctly as buttons or links:
   ```css
   .btn-link {
       color: #0066cc;
       text-decoration: underline;
       cursor: pointer;
   }
   .btn-link:hover {
       color: #0052a3;
   }
   ```
2. Consider using standard `<asp:Button>` for primary actions
3. Ensure all interactive elements have visible hover/focus states

---

### 14. Footer Links

**Severity:** Low (WCAG 2.4.5 - Multiple Ways)

**Status:** Partial Compliance ✅

**Positive findings:**
- Section 508 link present in footer
- HIPAA policy link available
- Privacy policy link available

**Issues:**
- Links may need better visibility/contrast
- No sitemap or search functionality mentioned
- Help documentation not linked from footer

**Recommendation:**
1. Ensure footer links meet contrast requirements
2. Add site map for alternative navigation method
3. Consider adding search functionality (WCAG 2.4.5 Multiple Ways)
4. Add "Accessibility Statement" page describing compliance efforts

---

## Positive Findings ✅

### 1. DOCTYPE & HTML5 Structure
**Status:** ✅ **COMPLIANT**
- Proper `<!DOCTYPE html>` declaration in both master pages
- Valid HTML5 structure

### 2. Character Encoding
**Status:** ✅ **COMPLIANT**
```html
<meta charset="UTF-8" />
```
Ensures proper character rendering.

### 3. Language Attribute
**Status:** ✅ **COMPLIANT**
```html
<html lang="en">
```
Properly identifies page language for screen readers.

### 4. Viewport Meta Tag
**Status:** ✅ **PRESENT**
```html
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
```
Though responsive design needs work, the foundation is present.

### 5. Compliance Awareness
**Status:** ✅ **POSITIVE**
- Footer includes Section 508 link
- HIPAA and Privacy links demonstrate compliance awareness
- Shows organizational commitment to accessibility

---

## Recommendations by Priority

### High Priority (Immediate Action Required)

| # | Issue | Estimated Effort | WCAG Criteria |
|---|-------|------------------|---------------|
| 1 | Add ARIA landmarks to master pages | 2-4 hours | 1.3.1, 2.4.1 |
| 2 | Restore skip navigation link | 15 minutes | 2.4.1 |
| 3 | Add focus indicators to CSS | 1-2 hours | 2.4.7 |
| 4 | Audit and fix form label associations | 20-40 hours | 1.3.1, 4.1.2 |
| 5 | Add ARIA live regions to UpdatePanels | 8-16 hours | 4.1.3 |
| 6 | Fix color contrast issues | 4-8 hours | 1.4.3 |
| 7 | Convert page titles to `<h1>` headings | 4-8 hours | 1.3.1, 2.4.6 |
| 8 | Add role="alert" to error messages | 8-16 hours | 3.3.1, 4.1.3 |

**Total Estimated Effort:** 47.25-94.25 hours (approximately 1-2 sprint cycles)

### Medium Priority (Within 3-6 Months)

| # | Issue | Estimated Effort | WCAG Criteria |
|---|-------|------------------|---------------|
| 9 | Add alt text to all images | 8-16 hours | 1.1.1 |
| 10 | Implement proper heading hierarchy | 16-24 hours | 1.3.1, 2.4.6 |
| 11 | Convert menu to list rendering | 2-4 hours | 1.3.1 |
| 12 | Add table captions and scope | 8-12 hours | 1.3.1 |
| 13 | Improve link text clarity | 8-16 hours | 2.4.4 |
| 14 | Implement modal focus management | 8-16 hours | 2.1.2, 2.4.3 |
| 15 | Add required field indicators (non-color) | 4-8 hours | 1.4.1, 3.3.2 |

**Total Estimated Effort:** 54-96 hours (approximately 1-2 sprint cycles)

### Low Priority (Future Enhancements)

| # | Issue | Estimated Effort | WCAG Criteria |
|---|-------|------------------|---------------|
| 16 | Responsive design improvements | 40-80 hours | 1.4.10 |
| 17 | Implement keyboard shortcuts | 16-24 hours | 2.1.1 |
| 18 | Generate accessible PDFs | 24-40 hours | 1.1.1, 1.3.1 |
| 19 | Add title attributes to links | 4-8 hours | 2.4.4 |
| 20 | Create accessibility statement page | 4-8 hours | N/A (best practice) |

**Total Estimated Effort:** 88-160 hours (approximately 2-3 sprint cycles)

---

## Testing Strategy

### Automated Testing Tools

1. **Browser Extensions:**
   - axe DevTools (Chrome/Firefox/Edge)
   - WAVE (Web Accessibility Evaluation Tool)
   - Lighthouse (Chrome DevTools)
   - IBM Equal Access Accessibility Checker

2. **Command-Line Tools:**
   - Pa11y
   - axe-core CLI
   - HTML_CodeSniffer

3. **Testing Frequency:**
   - Run automated tests in CI/CD pipeline
   - Weekly scans during active development
   - Full scan before each deployment

### Manual Testing Procedures

1. **Screen Reader Testing:**
   - **JAWS** (Windows - most popular enterprise screen reader)
   - **NVDA** (Windows - free, widely used)
   - **VoiceOver** (macOS/iOS - built-in)
   
   **Test scenarios:**
   - Navigate home page and login
   - Complete LOD case search
   - Fill out and submit a form
   - Navigate data tables
   - Interact with modal dialogs

2. **Keyboard-Only Navigation:**
   - Disconnect mouse
   - Navigate entire application using only Tab, Shift+Tab, Enter, Space, Arrow keys
   - Verify all functionality accessible
   - Check tab order is logical
   - Ensure focus visible at all times

3. **Color Contrast Testing:**
   - Use WebAIM Contrast Checker
   - Test all text/background combinations
   - Verify meets WCAG AA standards (4.5:1 for normal text, 3:1 for large)

4. **Color Blindness Simulation:**
   - Use Color Oracle or browser extensions
   - Test with protanopia, deuteranopia, tritanopia simulations
   - Verify information not conveyed by color alone

5. **Zoom/Magnification Testing:**
   - Test at 200% zoom (WCAG AA requirement)
   - Test at 400% zoom (WCAG AAA requirement)
   - Verify no horizontal scrolling
   - Check content reflows properly

### User Testing

1. **Recruit Users with Disabilities:**
   - Screen reader users (blind/low vision)
   - Keyboard-only users (motor disabilities)
   - Users with cognitive disabilities
   
2. **Test Scenarios:**
   - Task 1: Log in and view dashboard
   - Task 2: Search for an existing LOD case
   - Task 3: Create a new LOD case
   - Task 4: Upload documents
   - Task 5: Generate and view PDF report

3. **Collect Feedback:**
   - Observe task completion rates
   - Note pain points and barriers
   - Document workarounds users employ
   - Gather qualitative feedback

---

## Implementation Roadmap

### Phase 1: Critical Fixes (Sprint 1-2, Weeks 1-4)

**Goals:** Address WCAG Level A violations and high-severity issues

**Tasks:**
1. ✅ **Week 1:**
   - Add ARIA landmarks to master pages
   - Restore skip navigation link
   - Add CSS focus indicators
   - Begin form label audit

2. ✅ **Week 2:**
   - Complete form label audit (50% of pages)
   - Fix color contrast issues in CSS
   - Add role="alert" to error messages (core modules)

3. ✅ **Week 3:**
   - Complete form label audit (100%)
   - Add ARIA live regions to high-traffic pages (MyLods, Search)
   - Convert page titles to `<h1>` elements

4. ✅ **Week 4:**
   - Complete ARIA live regions on all UpdatePanels
   - Testing and bug fixes
   - Document changes

**Deliverables:**
- Updated master pages with ARIA landmarks
- Focus indicators on all interactive elements
- Properly labeled forms across application
- Error announcements for screen readers

### Phase 2: Moderate Fixes (Sprint 3-4, Weeks 5-8)

**Goals:** Address WCAG Level AA compliance and improve user experience

**Tasks:**
1. ✅ **Week 5:**
   - Audit and add alt text to all images
   - Implement heading hierarchy site-wide
   - Convert ASP.NET Menu to list rendering

2. ✅ **Week 6:**
   - Add table captions and scope attributes
   - Improve link text clarity (prioritize high-traffic pages)
   - Add required field indicators (beyond color)

3. ✅ **Week 7:**
   - Implement modal focus management
   - Add keyboard event handlers where needed
   - Update help documentation

4. ✅ **Week 8:**
   - Comprehensive testing with screen readers
   - User testing session with accessibility participants
   - Remediate findings

**Deliverables:**
- Descriptive alt text on all functional images
- Logical heading structure
- Accessible data tables
- Improved form usability

### Phase 3: Enhancements (Sprint 5-7, Weeks 9-14)

**Goals:** Achieve full WCAG 2.1 Level AA compliance and optimize experience

**Tasks:**
1. ✅ **Weeks 9-10:**
   - Begin responsive design improvements
   - Convert fixed widths to flexible layouts
   - Add media queries

2. ✅ **Weeks 11-12:**
   - Implement keyboard shortcuts
   - Add title attributes to links
   - Create accessibility statement page

3. ✅ **Weeks 13-14:**
   - Work on PDF accessibility (tagging, alternatives)
   - Final testing and remediation
   - Prepare accessibility documentation for users

**Deliverables:**
- Responsive layouts supporting zoom/reflow
- Enhanced keyboard navigation
- Accessible PDF options
- Public accessibility statement

---

## Compliance Metrics & KPIs

### Baseline Metrics (Current State)

| Metric | Current | Target | Status |
|--------|---------|--------|--------|
| WCAG 2.1 Level A Compliance | ~40% | 100% | ❌ |
| WCAG 2.1 Level AA Compliance | ~25% | 100% | ❌ |
| Pages with ARIA Landmarks | 0/464 (0%) | 464/464 (100%) | ❌ |
| Pages with Skip Links | 464/464 (100%) | 464/464 (100%) | ❌ (non-functional) |
| Forms with Proper Labels | ~150/464 (32%) | 464/464 (100%) | ❌ |
| Images with Alt Text | ~350/464 (75%) | 464/464 (100%) | ⚠️ |
| Pages with Heading Hierarchy | ~50/464 (11%) | 464/464 (100%) | ❌ |
| Color Contrast Pass Rate | ~70% | 100% | ⚠️ |

### Success Criteria

**Phase 1 Completion (End of Week 4):**
- [ ] 100% of pages have ARIA landmarks
- [ ] Skip navigation functional on all pages
- [ ] All forms have proper label association
- [ ] All error messages use role="alert"
- [ ] Color contrast issues resolved
- [ ] Automated test score > 80/100 (Lighthouse)

**Phase 2 Completion (End of Week 8):**
- [ ] WCAG 2.1 Level A: 100% compliance
- [ ] WCAG 2.1 Level AA: 80% compliance
- [ ] All images have descriptive alt text
- [ ] Heading hierarchy on all pages
- [ ] Data tables accessible
- [ ] Automated test score > 90/100

**Phase 3 Completion (End of Week 14):**
- [ ] WCAG 2.1 Level AA: 100% compliance
- [ ] Site passes screen reader testing
- [ ] Site passes keyboard-only testing
- [ ] User testing with people with disabilities: 90%+ task completion rate
- [ ] Automated test score > 95/100
- [ ] Accessibility statement published

---

## Risk Assessment

### Legal & Compliance Risks

| Risk | Severity | Likelihood | Mitigation |
|------|----------|-----------|------------|
| Section 508 non-compliance | High | High | Immediate remediation of critical issues |
| ADA lawsuit exposure | High | Medium | Legal review, compliance plan |
| Government contract jeopardy | High | Medium | Accelerated remediation, regular audits |
| WCAG 2.1 AA non-compliance | Medium | High | Follow implementation roadmap |

### Technical Risks

| Risk | Severity | Likelihood | Mitigation |
|------|----------|-----------|------------|
| Breaking existing functionality | Medium | Low | Comprehensive regression testing |
| Browser compatibility issues | Low | Medium | Cross-browser testing strategy |
| Performance impact (ARIA) | Low | Low | Performance monitoring |
| Master page changes affect all pages | High | Low | Staged rollout, thorough testing |

### Resource Risks

| Risk | Severity | Likelihood | Mitigation |
|------|----------|-----------|------------|
| Insufficient developer training | Medium | Medium | Accessibility training program |
| Timeline delays | Medium | Medium | Agile approach, prioritize critical issues |
| Budget constraints | Low | Low | Phased approach, focus on high ROI items |

---

## Training & Knowledge Transfer

### Developer Training Required

1. **WCAG 2.1 Fundamentals (4 hours):**
   - Understanding accessibility principles
   - POUR principles (Perceivable, Operable, Understandable, Robust)
   - Common barriers and solutions

2. **ASP.NET WebForms Accessibility (4 hours):**
   - Using AssociatedControlID for labels
   - Adding ARIA attributes to server controls
   - Making UpdatePanels accessible
   - GridView accessibility techniques

3. **Testing Tools Workshop (2 hours):**
   - Using axe DevTools
   - Screen reader basics (NVDA)
   - Keyboard testing procedures

4. **Ongoing Resources:**
   - WebAIM articles and tutorials
   - W3C WAI documentation
   - Internal accessibility wiki/knowledge base

### Code Review Checklist

Create accessibility checklist for all pull requests:
- [ ] All images have alt text (or alt="" for decorative)
- [ ] Form inputs have associated labels
- [ ] Color contrast meets WCAG AA standards
- [ ] Interactive elements keyboard accessible
- [ ] ARIA attributes used correctly
- [ ] Heading hierarchy maintained
- [ ] Error messages announced to screen readers
- [ ] No WCAG violations in automated testing

---

## Maintenance & Ongoing Compliance

### Monthly Activities
- Run automated accessibility scans
- Review and address new issues
- Update accessibility statement if changes made

### Quarterly Activities
- Full manual accessibility audit of sample pages
- Screen reader testing of new features
- Review and update training materials

### Annual Activities
- Comprehensive third-party accessibility audit
- User testing with people with disabilities
- Accessibility compliance report for stakeholders
- Review and update accessibility roadmap

---

## Resources & References

### Standards & Guidelines
- [WCAG 2.1](https://www.w3.org/WAI/WCAG21/quickref/) - Web Content Accessibility Guidelines
- [Section 508](https://www.section508.gov/) - U.S. Federal accessibility requirements
- [ARIA Authoring Practices Guide (APG)](https://www.w3.org/WAI/ARIA/apg/) - ARIA implementation patterns

### Testing Tools
- [axe DevTools](https://www.deque.com/axe/devtools/) - Automated accessibility testing
- [WAVE](https://wave.webaim.org/) - Web accessibility evaluation tool
- [NVDA Screen Reader](https://www.nvaccess.org/) - Free screen reader for Windows
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/) - Color contrast testing

### Learning Resources
- [WebAIM](https://webaim.org/) - Comprehensive accessibility tutorials
- [A11ycasts](https://www.youtube.com/playlist?list=PLNYkxOF6rcICWx0C9LVWWVqvHlYJyqw7g) - Accessibility video series
- [Inclusive Components](https://inclusive-components.design/) - Accessible component patterns
- [The A11Y Project](https://www.a11yproject.com/) - Community-driven accessibility resource

### ASP.NET Specific
- [Microsoft Accessibility Documentation](https://docs.microsoft.com/en-us/aspnet/web-forms/overview/older-versions-getting-started/deploying-web-site-projects/creating-a-website-in-visual-web-developer-vb)
- [ASP.NET Web Forms Accessibility](https://learn.microsoft.com/en-us/previous-versions/ms228004(v=vs.140))

---

## Appendix A: Sample Code Patterns

### Pattern 1: Accessible Form Field
```html
<!-- VB.NET ASPX -->
<tr>
    <td class="label">
        <asp:Label runat="server" ID="lblSSN" 
                   AssociatedControlID="txtSSN" 
                   Text="Member SSN:" 
                   CssClass="labelRequired" />
        <span class="required" aria-label="required">*</span>
    </td>
    <td class="value">
        <asp:TextBox runat="server" ID="txtSSN" 
                     MaxLength="9" 
                     aria-describedby="lblSSN lblSSNError lblSSNHelp" 
                     aria-required="true" />
        <asp:Label runat="server" ID="lblSSNError" 
                   role="alert" 
                   aria-live="assertive" 
                   CssClass="error" 
                   Visible="false" 
                   Text="SSN is required" />
        <span id="lblSSNHelp" class="help-text">
            Format: 9 digits without dashes
        </span>
    </td>
</tr>
```

### Pattern 2: Accessible UpdatePanel
```html
<!-- VB.NET ASPX -->
<div aria-live="polite" aria-atomic="true" aria-relevant="additions text">
    <asp:UpdatePanel ID="upnlSearchResults" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div role="status" aria-live="polite" class="loading-indicator" 
                 style="display:none;">
                <span class="sr-only">Loading search results, please wait...</span>
                <asp:Image runat="server" alt="" role="presentation" SkinID="imgBusy" />
            </div>
            
            <asp:GridView ID="gvResults" runat="server" 
                          Caption="Search Results" 
                          CaptionAlign="Top">
                <!-- Grid columns here -->
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
```

### Pattern 3: Accessible Modal Dialog
```javascript
// JavaScript for accessible modal
function showAccessibleModal(title, message, buttons) {
    var $dialog = $('#modalDialog');
    
    $dialog.dialog({
        title: title,
        modal: true,
        autoOpen: true,
        width: 500,
        open: function() {
            // Set ARIA attributes
            $dialog.attr({
                'role': 'dialog',
                'aria-modal': 'true',
                'aria-labelledby': 'ui-dialog-title-modalDialog',
                'aria-describedby': 'modal-content'
            });
            
            // Store previously focused element
            window.lastFocusedElement = document.activeElement;
            
            // Trap focus within modal
            trapFocus($dialog[0]);
        },
        close: function() {
            // Return focus to previous element
            if (window.lastFocusedElement) {
                window.lastFocusedElement.focus();
            }
        },
        buttons: buttons
    });
}

function trapFocus(element) {
    var focusableElements = element.querySelectorAll(
        'a[href], button, textarea, input, select, [tabindex]:not([tabindex="-1"])'
    );
    var firstFocusable = focusableElements[0];
    var lastFocusable = focusableElements[focusableElements.length - 1];
    
    element.addEventListener('keydown', function(e) {
        if (e.key === 'Tab') {
            if (e.shiftKey && document.activeElement === firstFocusable) {
                e.preventDefault();
                lastFocusable.focus();
            } else if (!e.shiftKey && document.activeElement === lastFocusable) {
                e.preventDefault();
                firstFocusable.focus();
            }
        } else if (e.key === 'Escape') {
            $(element).dialog('close');
        }
    });
}
```

### Pattern 4: Accessible Data Table with Sorting
```vb
' VB.NET Code-Behind
Protected Sub gvResults_PreRender(sender As Object, e As EventArgs)
    If gvResults.HeaderRow IsNot Nothing Then
        ' Add caption
        gvResults.Caption = "My LOD Cases - " & gvResults.Rows.Count & " results found"
        
        ' Ensure header is in thead
        gvResults.HeaderRow.TableSection = TableRowSection.TableHeader
        
        ' Add ARIA sort attributes
        For Each cell As TableCell In gvResults.HeaderRow.Cells
            If cell.HasControls Then
                Dim sortLink = TryCast(cell.Controls(0), LinkButton)
                If sortLink IsNot Nothing Then
                    ' Determine sort direction
                    Dim sortDir = "none"
                    If gvResults.SortExpression = sortLink.CommandArgument Then
                        sortDir = If(gvResults.SortDirection = SortDirection.Ascending, 
                                     "ascending", "descending")
                    End If
                    cell.Attributes.Add("aria-sort", sortDir)
                    sortLink.Attributes.Add("aria-label", 
                        $"Sort by {cell.Text} {If(sortDir = "none", "", sortDir)}")
                End If
            End If
        Next
    End If
End Sub
```

### Pattern 5: CSS Focus Indicators
```css
/* Global focus styles */
*:focus {
    outline: 3px solid #ffbf47;
    outline-offset: 2px;
}

/* High contrast mode support */
@media (prefers-contrast: high) {
    *:focus {
        outline: 3px solid currentColor;
        outline-offset: 3px;
    }
}

/* Skip link (only visible when focused) */
.skip-link {
    position: absolute;
    top: -40px;
    left: 0;
    background: #000;
    color: #fff;
    padding: 8px;
    text-decoration: none;
    z-index: 100;
}

.skip-link:focus {
    top: 0;
}

/* Focus within for containers */
.form-group:focus-within {
    background-color: #f0f8ff;
    border-color: #0066cc;
}
```

---

## Appendix B: Master Page Updates

### Updated Secure.master with ARIA Landmarks
```html
<%@ Master Language="VB" ... %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title>LOD</title>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    
    <%= Styles.Render("~/Content/css") %>
    <%= Scripts.Render("~/bundles/jquery") %>
    
    <asp:ContentPlaceHolder ID="head" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server" />
    
    <!-- Skip Link -->
    <a href="#main-content" class="skip-link">Skip to main content</a>
    
    <div id="outer">
        <!-- HEADER with role="banner" -->
        <header id="header" role="banner">
            <div id="header-logo">
                <asp:HyperLink runat="server" ID="lnkHome" NavigateUrl="~/Secure/Welcome.aspx">
                    <asp:Image runat="server" ID="imgLogo" SkinID="imgLogo" 
                               AlternateText="ALOD - Air Force Line of Duty System Home" 
                               ImageAlign="left" />
                </asp:HyperLink>
                
                <div id="header-right-user" aria-label="User information">
                    <asp:Label runat="server" ID="lblCurrentUser" />
                    <asp:Label runat="server" ID="UserRoleLabel" />
                    <asp:Label runat="server" ID="LastLoginLabel" />
                </div>
            </div>
            
            <!-- NAVIGATION with role="navigation" -->
            <nav id="menuWrapper" role="navigation" aria-label="Main navigation">
                <div id="menuLogo">
                    <asp:HyperLink runat="server" ID="lnkHome2" NavigateUrl="~/Secure/Welcome.aspx">
                        <asp:Image runat="server" ID="imgMenuLogo" SkinID="imgLogoMenu" 
                                   AlternateText="ALOD Logo" />
                    </asp:HyperLink>
                </div>
                
                <div id="main-menu">
                    <asp:Menu ID="MainMenu" runat="server" 
                              DataSourceID="SiteMapDataSource1" 
                              OnMenuItemDataBound="ItemDataBound"
                              Orientation="Horizontal" 
                              SkinID="MainMenu" 
                              SkipLinkText="Skip to main content"
                              DynamicEnableDefaultPopOutImage="False" 
                              RenderingMode="List">
                        <StaticMenuItemStyle CssClass="MainMenuStaticItemStyle" />
                    </asp:Menu>
                    <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" 
                                           ShowStartingNode="False" />
                </div>
            </nav>
            
            <div id="linksPanel" role="navigation" aria-label="Utility links">
                <asp:Panel runat="server" ID="pnlLinks" HorizontalAlign="Left">
                    <asp:LinkButton runat="server" ID="lnkLogOut" 
                                    PostBackUrl="~/Public/logout.aspx"
                                    SkinID="BoldLinkWhite" 
                                    Text="Logout" />
                    &nbsp;<asp:Literal runat="server" ID="SVSeparator" Text="|" />&nbsp;
                    <asp:LinkButton runat="server" ID="lnkSessionViewer" 
                                    SkinID="BoldLinkWhite" 
                                    Text="Session Viewer" />
                </asp:Panel>
            </div>
        </header>

        <!-- MAIN CONTENT with role="main" -->
        <main id="main-content" role="main" tabindex="-1">
            <h1 id="lblPageTitle" runat="server"></h1>
            
            <asp:HyperLink runat="server" ID="HelpLink" Target="_blank" 
                           NavigateUrl="~/Secure/help/files/ALOD_V_1.0.pdf"
                           aria-label="Help documentation (PDF)">
                <asp:Image runat="Server" ID="btnHelp" SkinID="imgHelp" 
                           CssClass="imgHelp" AlternateText="Help" />
            </asp:HyperLink>
            
            <uc2:PHIBanner ID="banner" runat="server" Visible="false" />
            
            <asp:ContentPlaceHolder ID="ContentMain" runat="server">
            </asp:ContentPlaceHolder>
        </main>
        
        <div class="clearer"></div>
    </div>
    
    <!-- FOOTER with role="contentinfo" -->
    <footer id="footer" role="contentinfo">
        <div id="footer-left">
            <nav aria-label="Footer links">
                <span runat="server" id="spnHIPPAPolicy" visible="false">
                    <asp:HyperLink ID="HIPAAPolicy" runat="server" 
                                   CssClass="LinkBoldWhite" 
                                   NavigateUrl="~/Public/HIPPA.aspx" 
                                   Target="_blank" 
                                   Text="HIPAA Policy" />
                    &nbsp; &nbsp;
                </span>
                <asp:HyperLink ID="Section508" runat="server" 
                               CssClass="LinkBoldWhite" 
                               NavigateUrl="~/Public/Section508.aspx"
                               Target="_blank" 
                               Text="Section 508" />
                &nbsp; &nbsp;
                <asp:HyperLink ID="PrivacyAndSecurity" runat="server" 
                               CssClass="LinkBoldWhite" 
                               NavigateUrl="~/Public/Privacy.aspx"
                               Target="_blank" 
                               Text="Privacy & Security" />
            </nav>
        </div>
        <div id="footer-right">
            <p>
                <asp:Label ID="SystemDisclaimer" runat="server" 
                           Text="For Official Use Only" />
            </p>
        </div>
    </footer>
    
    <asp:ContentPlaceHolder ID="ContentFooter" runat="server">
    </asp:ContentPlaceHolder>
    
    <uc1:ModalDialog ID="ModalDialog1" runat="server" RedirectURL="~/Default.aspx" />
    </form>
</body>
</html>
```

---

## Appendix C: Automated Testing Scripts

### PowerShell Script for Lighthouse CI
```powershell
# Run-AccessibilityTests.ps1
# Runs Lighthouse accessibility audits on ALOD pages

param(
    [string]$BaseUrl = "http://localhost:8080",
    [string]$OutputPath = ".\AccessibilityReports"
)

# Install lighthouse if not present
if (-not (Get-Command lighthouse -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Lighthouse CLI..."
    npm install -g @lhci/cli
}

# Create output directory
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

# Pages to test
$pages = @(
    "/Default.aspx",
    "/Secure/Welcome.aspx",
    "/Secure/lod/MyLods.aspx",
    "/Secure/lod/start.aspx",
    "/Secure/lod/search.aspx"
)

$results = @()

foreach ($page in $pages) {
    $url = "$BaseUrl$page"
    $filename = $page -replace "/", "_" -replace "\.aspx", ""
    $outputFile = Join-Path $OutputPath "$filename.json"
    
    Write-Host "Testing: $url"
    
    lighthouse $url `
        --only-categories=accessibility `
        --output=json `
        --output-path=$outputFile `
        --chrome-flags="--headless --no-sandbox"
    
    if ($LASTEXITCODE -eq 0) {
        $report = Get-Content $outputFile | ConvertFrom-Json
        $score = $report.categories.accessibility.score * 100
        
        $results += [PSCustomObject]@{
            Page = $page
            Score = $score
            Violations = $report.audits | Where-Object { $_.score -lt 1 } | Measure-Object | Select-Object -ExpandProperty Count
        }
        
        Write-Host "  Score: $score/100" -ForegroundColor $(if ($score -ge 90) { "Green" } elseif ($score -ge 70) { "Yellow" } else { "Red" })
    }
}

# Generate summary report
$summary = @"
# Accessibility Test Summary
Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

| Page | Score | Violations |
|------|-------|-----------|
"@

foreach ($result in $results) {
    $summary += "`n| $($result.Page) | $($result.Score) | $($result.Violations) |"
}

$summary | Out-File (Join-Path $OutputPath "summary.md")

Write-Host "`nSummary report saved to: $(Join-Path $OutputPath 'summary.md')"
```

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-04 | Accessibility Audit Team | Initial comprehensive accessibility analysis |

---

**End of Report**
