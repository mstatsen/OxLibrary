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
        OxBool Hovered { get; }
        bool IsHovered { get; }
        void SetHovered(bool value);
        DialogResult ShowDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
    }
}