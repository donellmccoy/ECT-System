using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.PsychologicalHealth;
using ALOD.Core.Interfaces;

namespace ALODWebUtility.Common
{
    public static class PHUtility
    {
        public const string CSS_FIELD_CSV_BOX = "fieldCSVTextbox";
        public const string CSS_FIELD_CSV_LABEL = "fieldCSVLabel";
        public const string CSS_FIELD_GIANTTEXT_BOX = "fieldGiantTextbox textLimit";
        public const string CSS_FIELD_LARGETEXT_BOX = "fieldLargeTextbox";
        public const string CSS_FIELD_MULTIPLEOPTIONS_BOX = "fieldMultipleOptionsBox";
        public const string CSS_FIELD_NUMBERSTEXT_BOX = "fieldNumbersTextbox";
        public const string CSS_FIELD_OPTIONS_BOX = "fieldOptionsBox";
        public const string CSS_FIELD_SMALLTEXT_BOX = "fieldSmallTextbox";
        public const string CSS_FIELD_VALIDATION_ERROR = "fieldValidationError";
        public const string EMPTY_FIELD_NAME = "{EMPTY}";

        public static void ConfigureUserControl(PHFormField formField, UserControl userControl, string cssClass)
        {
            IPHFormUIBuildingBlock uc = (IPHFormUIBuildingBlock)userControl;

            uc.CssClass = cssClass;
            uc.Placeholder = formField.FieldType.Placeholder;
            uc.DataSource = formField.FieldType.Datasource;
            uc.ToolTip = formField.ToolTip;

            if (!formField.Field.Name.Equals(EMPTY_FIELD_NAME))
            {
                uc.ScreenReaderText = formField.Field.Name + " " + formField.FieldType.Name;
            }
            else
            {
                uc.ScreenReaderText = formField.FieldType.Name;
            }

            if (formField.FieldType.Color.HasValue)
            {
                uc.BackColor = formField.FieldType.Color.Value;
            }

            if (formField.FieldType.Length.HasValue)
            {
                uc.MaxLength = formField.FieldType.Length;
            }
        }

        public static Panel CreateFieldComponentsPanel(int columns, bool isEmptyFieldLabel)
        {
            Panel p = new Panel();

            if (isEmptyFieldLabel)
            {
                p.CssClass = "fieldComponentsPanel-NoLabel";
            }
            else if (columns == 1)
            {
                p.CssClass = "fieldComponentsPanel-1";
            }
            else if (columns == 2)
            {
                p.CssClass = "fieldComponentsPanel-2";
            }
            else
            {
                p.CssClass = "fieldComponentsPanel-3";
            }

            return p;
        }

        public static Panel CreateFieldGroupPanel(bool isLeft)
        {
            Panel p = new Panel();

            if (isLeft)
            {
                p.CssClass = "fieldGroupPanelLeft";
            }
            else
            {
                p.CssClass = "fieldGroupPanelRight";
            }

            return p;
        }

        public static Panel CreateFieldGroupPanel(int c, string h)
        {
            Panel p = new Panel();

            if (c == 1)
            {
                p.CssClass = "fieldGroupPanel-1";
            }
            else if (c == 2)
            {
                if (h.Equals("L"))
                {
                    p.CssClass = "fieldGroupPanelLeft-2";
                }
                else if (h.Equals("R"))
                {
                    p.CssClass = "fieldGroupPanelRight-2";
                }
            }
            else
            {
                if (h.Equals("L"))
                {
                    p.CssClass = "fieldGroupPanelLeft-3";
                }
                else if (h.Equals("R"))
                {
                    p.CssClass = "fieldGroupPanelRight-3";
                }
                else if (h.Equals("C"))
                {
                    p.CssClass = "fieldGroupPanelCenter-3";
                }
            }

            return p;
        }

        public static Label CreateFieldLabel(string text)
        {
            Label lbl = new Label();

            lbl.Text = text;
            lbl.CssClass = "fieldLabel";

            return lbl;
        }

        public static Panel CreateFieldLabelPanel(int columns)
        {
            Panel p = new Panel();

            if (columns == 1)
            {
                p.CssClass = "fieldLabelPanel-1";
            }
            else if (columns == 2)
            {
                p.CssClass = "fieldLabelPanel-2";
            }
            else
            {
                p.CssClass = "fieldLabelPanel-3";
            }

            return p;
        }

        public static Panel CreateFieldPanel()
        {
            Panel p = new Panel();

            p.CssClass = "fieldPanel";

            return p;
        }

        public static HtmlGenericControl CreateSectionHeader(PHSection section)
        {
            HtmlGenericControl h = new HtmlGenericControl("h2");

            h.InnerHtml = section.Name;

            if (section.HasPageBreak)
            {
                h.Attributes.Add("class", "sectionHeader mainSectionBreak");
            }
            else
            {
                h.Attributes.Add("class", "sectionHeader");
            }

            return h;
        }

        public static HyperLink CreateSectionHeaderLink(PHSection section)
        {
            HyperLink l = new HyperLink();

            l.Text = section.Name;
            l.NavigateUrl = "#";
            l.Attributes.Add("onclick", "return false;");

            return l;
        }

        public static HtmlGenericControl CreateSectionInnerFieldset(PHSection section)
        {
            HtmlGenericControl f = new HtmlGenericControl("fieldset");

            f.ID = "pnlSection_Inner_" + section.Id;
            f.Attributes.Add("class", "sectionPanelNoTitle");

            return f;
        }

        public static Panel CreateSectionInnerPanel(PHSection section)
        {
            Panel p = new Panel();

            p.ID = "pnlSection_Inner_" + section.Id;
            p.CssClass = "sectionPanelNoTitle";

            return p;
        }

        public static Panel CreateSectionPanel(PHSection section)
        {
            Panel p = new Panel();

            p.ID = "pnlSection_" + section.Id;
            p.CssClass = "sectionPanel";
            p.GroupingText = "<label>" + section.Name + "</label>";

            return p;
        }

        public static Panel CreateSectionPanel_v2(PHSection section)
        {
            Panel headerPanel = new Panel();
            Panel innerPanel = new Panel();

            headerPanel.ID = "pnlSection_" + section.Id;
            headerPanel.CssClass = "sectionPanel";
            headerPanel.GroupingText = section.Name;
            headerPanel.Font.Bold = true;

            return headerPanel;
        }

        public static string GenerateFieldControlId(int sectionId, int fieldId, int fieldTypeId)
        {
            string controlId = "formfield_" + sectionId.ToString() + "_" + fieldId.ToString() + "_" + fieldTypeId.ToString();

            return controlId;
        }

        public static IList<PHFormValue> GetTestFormValues()
        {
            List<PHFormValue> testValues = new List<PHFormValue>();

            // Human Performance Improvement/Outreach
            testValues.Add(new PHFormValue(0, 1, 21, 1, 85));

            // Walkabout/Unit Visits
            testValues.Add(new PHFormValue(0, 6, 1, 1, 5));
            testValues.Add(new PHFormValue(0, 6, 1, 2, 15));

            testValues.Add(new PHFormValue(0, 6, 2, 1, 55));
            testValues.Add(new PHFormValue(0, 6, 2, 3, 515));

            testValues.Add(new PHFormValue(0, 6, 133, 1, 75));
            testValues.Add(new PHFormValue(0, 6, 133, 1, 715));
            testValues.Add(new PHFormValue(0, 6, 133, 5, "Test Value"));

            // Abuse
            testValues.Add(new PHFormValue(0, 10, 29, 1, 1313));

            // Suidice Method
            testValues.Add(new PHFormValue(0, 8, 49, 4, "2,4,1,5,3"));

            testValues.Add(new PHFormValue(0, 3, 92, 14, "www.google.com,www.stanfieldsystems.com"));

            testValues.Add(new PHFormValue(0, 17, 140, 12, "jfklaflkdsajfsafjslajfkslfjlksjfklsjfls"));

            return testValues;
        }
    }
}
