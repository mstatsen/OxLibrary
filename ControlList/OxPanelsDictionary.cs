using OxLibrary.Interfaces;

namespace OxLibrary.ControlList;

public class OxPanelsDictionary : Dictionary<IOxPanel, OxPanelList>
{
    public void Add(IOxPanel panel) =>
        Add(panel, new());
}