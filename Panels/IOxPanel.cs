using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public interface IOxPanel : IOxPane
    {
        OxBorders Paddings { get; }
        OxPane ContentContainer { get; }
        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
    }
}