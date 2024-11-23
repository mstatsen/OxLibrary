using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public interface IOxPane : IOxWithIcon, IOxControl<Panel>
    {
        Color BaseColor { get; set; }
        OxColorHelper Colors { get; }
        Color DefaultColor { get; }
        bool IsHovered { get; }
        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
        OxBorders Padding { get; }
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        void SetBorderWidth(OxWidth value);
        void SetBorderWidth(OxDock dock, OxWidth value);
        bool BorderVisible { get; set; }
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
    }
}