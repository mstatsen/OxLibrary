using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxClickFrameList<TClickFrame> : OxPanelList<TClickFrame>
    where TClickFrame : OxClickFrame, new()
{
    public TClickFrame? FirstVisible =>
        Find(f => f.Visible);

    public TClickFrame? LastVisible
    {
        get
        {
            TClickFrame? visibleFrame = null;

            foreach (TClickFrame frame in this)
                if (frame.Visible)
                    visibleFrame = frame;

            return visibleFrame;
        }
    }

    public OxWidth Right =>
        Last is not null
            ? Last.Right
            : OxWh.W0;

    public OxWidth Width
    {
        get
        {
            OxWidth result = OxWh.W0;

            foreach (TClickFrame frame in this)
            {
                result = OxWh.A(result, frame.Width);
                result = OxWh.A(result, frame.Margin.Horizontal);
            }

            return result;
        }
    }

    public OxWidth Height()
    {
        OxWidth maxHeight = OxWh.W0;

        foreach (TClickFrame frame in this)
            maxHeight = OxWh.Max(maxHeight, frame.Height);

        return maxHeight;
    }

    private TClickFrame? Default()
    {
        foreach (TClickFrame button in this)
            if (button.Visible && button.Enabled && button.Default)
                return button;

        return null;
    }

    public bool ExecuteDefault()
    {
        TClickFrame? defultButton = Default();
        defultButton?.PerformClick();
        return defultButton is not null;
    }
}

public class OxClickFrameList : OxClickFrameList<OxClickFrame>
{
}