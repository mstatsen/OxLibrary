using OxLibrary.Geometry;
using OxLibrary.Interfaces;

namespace OxLibrary.ControlList;

public class OxPanelList : List<IOxPanel>
{
    public IOxPanel? Last =>
        Count > 0
            ? this[Count - 1]
            : default!;

    public IOxPanel? First =>
        Count > 0
            ? this[0]
            : default!;

    public short Bottom =>
        OxSH.Short(Last is not null ? Last!.Bottom + 24 : 0);

    public new OxPanelList AddRange(IEnumerable<IOxPanel> list)
    {
        base.AddRange(list);
        return this;
    }

    public IOxPanel? LastVisible
    {
        get
        {
            IOxPanel? visiblePanel = null;

            foreach (IOxPanel panel in this)
                if (panel.Visible)
                    visiblePanel = panel;

            return visiblePanel;
        }
    }

    public short Right =>
        OxSH.Short(Last is not null ? Last!.Right : 0);

    public short Width
    {
        get
        {
            short result = 0;

            foreach (IOxPanel panel in this)
                result += OxSH.Add(panel.Width, panel.Margin.Horizontal);

            return result;
        }
    }

    public short Height()
    {
        short maxHeight = 0;

        foreach (IOxPanel panel in this)
            maxHeight = Math.Max(maxHeight, panel.Height);

        return maxHeight;
    }
}

public class OxPanelList<TPanel> : OxPanelList
    where TPanel : IOxPanel
{
    public new TPanel? First => (TPanel?)base.First;
    public new TPanel? Last => (TPanel?)base.Last;
    public TPanel? FirstVisible => Find(f => f.Visible);
    public new TPanel? LastVisible => (TPanel?)base.LastVisible;

    public TPanel? Find(Predicate<TPanel> match)
    {
        foreach (TPanel panel in this)
            if (match(panel))
                return panel;

        return default;
    }

    public OxPanelList<TPanel> AddRange(IEnumerable<TPanel> list)
    {
        foreach (TPanel panel in list)
            Add(panel);

        return this;
    }
}