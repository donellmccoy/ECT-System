using System;
using System.Web;
using System.Web.UI.WebControls;

namespace ALODWebUtility.Common
{
    public static class WebControlSetters
    {
        public static void SetCheckBox(CheckBox checkbox, bool? val)
        {
            if (val == null || !val.HasValue)
            {
                return;
            }

            checkbox.Checked = val.Value;
        }

        public static void SetDateLabel(Label label, DateTime? theDate)
        {
            if (label == null || !theDate.HasValue)
            {
                return;
            }

            SetLabelText(label, theDate.Value.ToString(Utility.DATE_FORMAT));
        }

        public static void SetDateTextbox(TextBox textbox, DateTime? theDate)
        {
            if (textbox == null || !theDate.HasValue)
            {
                return;
            }

            SetTextboxText(textbox, theDate.Value.ToString(Utility.DATE_FORMAT));
        }

        public static void SetDateTimeLabel(Label label, DateTime? dateTime)
        {
            if (label == null || !dateTime.HasValue)
            {
                return;
            }

            SetLabelText(label, dateTime.Value.ToString(Utility.DATE_HOUR_FORMAT));
        }

        public static void SetDropdownByValue(DropDownList list, string value)
        {
            if (list == null || string.IsNullOrEmpty(value))
            {
                return;
            }

            if (list.Items.FindByValue(value) != null)
            {
                list.SelectedValue = value;
            }
        }

        public static void SetLabelText(Label label, string text)
        {
            if (label == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            label.Text = HttpContext.Current.Server.HtmlDecode(text);
        }

        public static void SetMaxLength(TextBox input)
        {
            input.Attributes.Add("maxLength", input.MaxLength.ToString());
            Utility.AddCssClass(input, "textLimit");
        }

        public static void SetMaxLength(TextBox input, bool charRemaining)
        {
            input.Attributes.Add("maxLength", input.MaxLength.ToString());
            if (charRemaining)
            {
                Utility.AddCssClass(input, "textLimit");
            }
        }

        public static void SetTextboxText(TextBox textbox, string text)
        {
            if (textbox == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text);
        }

        public static void SetTextboxText(TextBox textbox, decimal? text)
        {
            if (textbox == null || !text.HasValue)
            {
                return;
            }

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text.Value.ToString());
        }

        public static void SetTextboxText(TextBox textbox, int? text)
        {
            if (textbox == null || !text.HasValue)
            {
                return;
            }

            textbox.Text = HttpContext.Current.Server.HtmlDecode(text.Value.ToString());
        }

        public static void SetTimeTextbox(TextBox textbox, DateTime? timeAsDateTime)
        {
            if (textbox == null || !timeAsDateTime.HasValue)
            {
                return;
            }

            SetTextboxText(textbox, timeAsDateTime.Value.ToString(Utility.HOUR_FORMAT));
        }
    }
}
