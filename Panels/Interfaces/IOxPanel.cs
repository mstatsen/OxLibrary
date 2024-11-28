using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Interfaces;

namespace OxLibrary.Panels
{

    public interface IOxPanel : IOxWithIcon, IOxWithColorHelper, IOxControlContainer<Panel>
    {
        Color DefaultColor { get; }
        bool IsHovered { get; }

        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
    }
}