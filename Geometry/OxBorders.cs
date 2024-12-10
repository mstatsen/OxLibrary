using OxLibrary.Geometry;
using OxLibrary.Handlers;

namespace OxLibrary;

public class OxBorders : Dictionary<OxDock, OxBorder>
{
    public static OxBorders operator +(OxBorders left, OxBorders right) =>
        new(
            OxSH.Add(left.Top, right.Top),
            OxSH.Add(left.Left, right.Left),
            OxSH.Add(left.Bottom, right.Bottom),
            OxSH.Add(left.Right, right.Right)
        );

    public static OxBorders operator -(OxBorders left, OxBorders right) =>
        new(
            OxSH.Sub(left.Top, right.Top),
            OxSH.Sub(left.Left, right.Left),
            OxSH.Sub(left.Bottom, right.Bottom),
            OxSH.Sub(left.Right, right.Right)
        );

    public void Draw(Graphics g, OxRectangle bounds, Color color)
    {
        if (IsEmpty)
            return;


        foreach (var border in this)
        {
            if (border.Value.IsEmpty)
                continue;

            OxRectangle borderBounds = new(bounds);

            switch (border.Key)
            {
                case OxDock.Right:
                    borderBounds.X = OxSH.Sub(borderBounds.Right, border.Value.Size);
                    break;
                case OxDock.Bottom:
                    borderBounds.Y = OxSH.Sub(borderBounds.Bottom, border.Value.Size);
                    break;
            }

            if (OxDockHelper.IsHorizontal(border.Key))
                borderBounds.Width = border.Value.Size;
            else
                if (OxDockHelper.IsVertical(border.Key))
                    borderBounds.Height = border.Value.Size;

            using Brush brush = new SolidBrush(color);
            g.FillRectangle(brush, borderBounds.Rectangle);
        }
    }

    private bool SizeChanging = false;
    
    private void NotifyAboutSizeChanged(OxBorders oldBorders)
    {
        if (SizeChanging
            || Equals(oldBorders))
            return;

        SizeChanged?.Invoke(
            this, 
            new OxBordersChangedEventArgs(oldBorders, this)
        );
    }

    public bool SetSize(OxDock dock, int size)
    {
        short oldSize = this[dock].Size;

        if (oldSize.Equals(size))
            return false;

        OxBorders oldBorders = new(this);
        this[dock].Size = OxSH.Short(size);
        NotifyAboutSizeChanged(oldBorders);
        return true;
    }

    private void SetSize(int size)
    {
        SizeChanging = true;
        OxBorders oldBorders = new(this);

        try
        {
            foreach (OxDock border in Keys)
                SetSize(border, size);
        }
        finally
        {
            SizeChanging = false;
        }

        NotifyAboutSizeChanged(oldBorders);
    }

    private short GetSize(OxDock dock) =>
        this[dock].Size;

    public OxBorders()
    {
        foreach (OxDock dock in OxDockHelper.All)
            if (OxDockHelper.IsSingleDirectionDock(dock))
                Add(dock, new());
    }

    public OxBorders(OxBorders prototype)
    {
        foreach (KeyValuePair<OxDock, OxBorder> border in prototype)
            Add(border.Key, new(border.Value));
    }

    public OxBorders(int top, int left, int bottom, int right) : this() => 
        SetSize(top, left, bottom, right);

    public short Left
    {
        get => GetSize(OxDock.Left);
        set => SetSize(OxDock.Left, value);
    }

    public short Horizontal
    {
        get => OxSH.Add(Left, Right);
        set
        {
            Left = value;
            Right = value;
        }
    }

    public short Top
    {
        get => GetSize(OxDock.Top);
        set => SetSize(OxDock.Top, value);
    }

    public short Vertical
    {
        get => OxSH.Add(Top, Bottom);
        set
        {
            Top = value;
            Bottom = value;
        }
    }

    public short ByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Horizontal,
            OxDockVariable.Height =>
                Vertical,
            _ =>
                Size
        };

    public short Right
    {
        get => GetSize(OxDock.Right);
        set => SetSize(OxDock.Right, value);
    }

    public short Bottom
    {
        get => GetSize(OxDock.Bottom);
        set => SetSize(OxDock.Bottom, value);
    }

    public short Size
    {
        get => GetSize(OxDock.Left);
        set => SetSize(value);
    }

    public void SetSize(int top, int left, int bottom, int right)
    {
        Top = OxSH.Short(top);
        Left = OxSH.Short(left);
        Bottom = OxSH.Short(bottom);
        Right = OxSH.Short(right);
    }

    public bool Visible(OxDock dock) => 
        this[dock].Visible;

    public bool SetVisible(OxDock dock, bool visible)
    {
        bool visibleChanged = !this[dock].Visible.Equals(visible);

        if (visibleChanged)
        {
            OxBorders oldBorders = new(this);
            this[dock].Visible = visible;
            NotifyAboutSizeChanged(oldBorders);
        }

        return visibleChanged;
    }

    public void SetVisible(bool visible)
    {
        OxBorders oldBorders = new(this);

        foreach (OxDock dock in Keys)
            SetVisible(dock, visible);

        NotifyAboutSizeChanged(oldBorders);
    }

    public bool GetVisible() =>
        this[OxDock.Top].Visible
        || this[OxDock.Left].Visible
        || this[OxDock.Bottom].Visible
        || this[OxDock.Right].Visible;

    public bool IsEmpty
    {
        get 
        {
            foreach (OxDock dock in Keys)
                if (!this[dock].IsEmpty)
                    return false;

            return true;
        }
    }

    public override bool Equals(object? obj) =>
        obj is OxBorders otherBorders
        && Left.Equals(otherBorders.Left)
        && Right.Equals(otherBorders.Right)
        && Top.Equals(otherBorders.Top)
        && Bottom.Equals(otherBorders.Bottom);

    public OxBordersChangedEvent? SizeChanged;

    public override int GetHashCode()
    {
        int result = 0;

        foreach (OxBorder border in this.Values)
            result ^= border.GetHashCode();

        return result;
    }

    public override string ToString() =>
        $"Left = {Left}\n"
        + $"Top = {Top}\n"
        + $"Right = {Right}\n"
        + $"Bottom = {Bottom}\n";
}