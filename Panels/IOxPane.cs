using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public interface IOxPane : IOxWithIcon, IOxControl<Panel>
    {
        void StartSizeChanging();
        void EndSizeChanging();
        Color BaseColor { get; set; }
        void ReAlignControls();
        void ReAlign();
        OxColorHelper Colors { get; }
        Color DefaultColor { get; }
        bool IsHovered { get; }
        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
        OxBorders Padding { get; }
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        OxWidth BorderWidth { get; set; }
        bool BorderVisible { get; set; }
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
    }
}