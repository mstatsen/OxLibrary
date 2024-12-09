using OxLibrary.Interfaces;
using OxLibrary.Panels;

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

    public short Bottom
    {
        get
        {
            IOxPanel? last = Last;

            return (short)(last is null
                ? 0
                : last.Bottom + 24);
        }
    }

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
        (short)(Last is not null
            ? Last.Right
            : 0);

    public short Width
    {
        get
        {
            short result = 0;

            foreach (IOxPanel panel in this)
            {
                result += panel.Width;
                result += panel.Margin.Horizontal;
            }

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