using OxLibrary.Forms;
using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{

    public interface IOxPanel :
        IOxBox,
        IOxWithMargin,
        IOxWithBorders,
        IOxWithPadding,
        IOxWithIcon,
        IOxWithColorHelper
    {
        Color DefaultColor { get; }
        bool IsHovered { get; }

        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
    }
}