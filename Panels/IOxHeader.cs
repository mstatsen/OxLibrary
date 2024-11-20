using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public interface IOxHeader : IOxPane
    {
        OxHeaderToolBar ToolBar { get; }
        EventHandler? Click { get; set; }
        Label Label { get; }
        ContentAlignment TitleAlign { get; set; }
        Font TitleFont { get; set; }
        void AddToolButton(OxIconButton button);
        bool UnderlineVisible { get; set; }
    }
}
