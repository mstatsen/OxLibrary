using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxPanelList : List<OxPanel>
{
    public OxPanel? Last =>
        Count > 0
            ? this[Count - 1]
            : default;

    public OxPanel? First =>
        Count > 0
            ? this[0]
            : default;

    public OxWidth Bottom
    {
        get
        {
            OxPanel? last = Last;

            return last is null
                ? OxWh.W0
                : last.Bottom | OxWh.W24;
        }
    }

    public new OxPanelList AddRange(IEnumerable<OxPanel> collection)
    {
        base.AddRange(collection);
        return this;
    }
}