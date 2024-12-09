using OxLibrary.Interfaces;

namespace OxLibrary.ControlList;

public class OxPanelsDictionary : Dictionary<IOxPanel, OxPanelList<IOxPanel>>
{
    public void Add(IOxPanel panel) =>
        Add(panel, new());
}

public class OxPanelsDictionary<TPanel> : OxPanelsDictionary
    where TPanel : IOxPanel
{
    public new void Add(IOxPanel panel) => base.Add(panel);
}