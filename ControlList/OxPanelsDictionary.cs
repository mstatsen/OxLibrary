using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxPanelsDictionary<TPanel> : Dictionary<TPanel, OxPanelList<TPanel>>
    where TPanel : IOxPanel
{
    public void Add(TPanel panel) =>
        Add(panel, new());
}

public class OxPanelsDictionary : OxPanelsDictionary<OxPanel>
{ }