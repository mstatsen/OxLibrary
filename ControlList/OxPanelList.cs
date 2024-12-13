using OxLibrary.Geometry;
using OxLibrary.Interfaces;

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

    public short Bottom =>
        OxSh.Short(Last is not null ? Last!.Bottom + 24 : 0);

    public new OxPanelList<TPanel> AddRange(IEnumerable<TPanel> list)
    {
        base.AddRange(list);
        return this;
    }

    public TPanel? LastVisible
    {
        get
        {
            TPanel? visiblePanel = default;

            foreach (TPanel panel in this)
                if (panel.IsVisible)
                    visiblePanel = panel;

            return visiblePanel;
        }
    }

    public short Right =>
        OxSh.Short(Last is not null ? Last!.Right : 0);

    public short VisibleWidth
    {
        get
        {
            short result = 0;

            foreach (TPanel panel in FindAll(p => p.IsVisible))
                result += OxSh.Add(panel.Width, panel.Margin.Horizontal);

            return result;
        }
    }

    public short Width
    {
        get
        {
            short result = 0;

            foreach (TPanel panel in this)
                result += OxSh.Add(panel.Width, panel.Margin.Horizontal);

            return result;
        }
    }

    public short VisibleHeight()
    {
        short maxHeight = 0;

        foreach (TPanel panel in FindAll(p => p.IsVisible))
            maxHeight = Math.Max(maxHeight, panel.Height);

        return maxHeight;
    }

    public short Height()
    {
        short maxHeight = 0;

        foreach (TPanel panel in this)
            maxHeight = Math.Max(maxHeight, panel.Height);

        return maxHeight;
    }
}

public class OxPanelList : OxPanelList<IOxPanel>
{
}