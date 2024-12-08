using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxPaneDictionary : Dictionary<OxPanel, OxPanelList>
{
    public void Add(OxPanel pane) =>
        Add(pane, new OxPanelList());
}