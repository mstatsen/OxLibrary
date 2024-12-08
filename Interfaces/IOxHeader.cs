using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Interfaces
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
