using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public interface IOxHeader : IOxPanel
    {
        OxHeaderToolBar ToolBar { get; }
        Label Label { get; }
        ContentAlignment TitleAlign { get; set; }
        Font TitleFont { get; set; }
        void AddToolButton(OxIconButton button, bool startGroup = false);
        bool UnderlineVisible { get; set; }
    }
}
