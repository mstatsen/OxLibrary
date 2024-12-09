using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{
    public interface IOxFrameWithHeader : IOxPanel
    {
        OxHeader Header { get; }
        short HeaderHeight { get; set; }
        bool HeaderVisible { get; set; }
        OxHeaderToolBar HeaderToolBar { get; }
    }
}