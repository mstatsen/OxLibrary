using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxPanelList<TPanel> : List<TPanel>
    where TPanel : IOxPanel
{
    public TPanel? Last =>
        Count > 0
            ? this[Count - 1]
            : default!;

    public TPanel? First =>
        Count > 0
            ? this[0]
            : default!;

    public short Bottom
    {
        get
        {
            TPanel? last = Last;

            return (short)(last is null
                ? 0
                : last.Bottom + 24);
        }
    }

    public new OxPanelList<TPanel> AddRange(IEnumerable<TPanel> list)
    {
        base.AddRange(list);
        return this;
    }
}

public class OxPanelList : OxPanelList<OxPanel>
{ 
}