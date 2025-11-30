using System.Drawing;

namespace ALOD.Core.Interfaces
{
    public interface IPHFormUIBuildingBlock
    {
        string AltAttribute { get; set; }
        Color BackColor { get; set; }
        string ControlId { get; set; }
        string CssClass { get; set; }
        string DataSource { get; set; }
        bool Enabled { get; set; }
        Color ForeColor { get; set; }
        bool IsReadOnly { get; set; }
        int MaxLength { get; set; }
        string Placeholder { get; set; }
        string PrimaryValue { get; set; }
        bool ScreenReaderControlEnabled { get; set; }
        string ScreenReaderText { get; set; }
        string SecondaryValue { get; set; }
        string ToolTip { get; set; }

        bool ValidateInput();
    }
}