using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{
    public interface IOxFrameWithHeader : IOxPanel
    {
        OxHeader Header { get; }
        short HeaderHeight { get; set; }
        OxBool HeaderVisible { get; set; }
        bool IsHeaderVisible { get; }
        void SetHeaderVisible(bool value);
        OxHeaderToolBar HeaderToolBar { get; }
    }
}