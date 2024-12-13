using OxLibrary.Forms;

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
        DialogResult ShowDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
    }
}