using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{
    public interface IOxHeader : IOxPanel
    {
        OxHeaderToolBar ToolBar { get; }
        OxLabel Title { get; }
        ContentAlignment TitleAlign { get; set; }
        Font TitleFont { get; set; }
        void AddButton(OxIconButton button, bool startGroup = false);
    }
}
