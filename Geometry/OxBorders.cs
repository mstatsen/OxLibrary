using OxLibrary.Handlers;

namespace OxLibrary;

public class OxBorders : Dictionary<OxDock, OxBorder>
{
    public static OxBorders operator +(OxBorders left, OxBorders right) =>
        new(
            OxWh.A(left.Top, right.Top),
            OxWh.A(left.Left, right.Left),
            OxWh.A(left.Bottom, right.Bottom),
            OxWh.A(left.Right, right.Right)
        );

    public static OxBorders operator -(OxBorders left, OxBorders right) =>
        new(
            OxWh.S(left.Top, right.Top),
            OxWh.S(left.Left, right.Left),
            OxWh.S(left.Bottom, right.Bottom),
            OxWh.S(left.Right, right.Right)
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
                    borderBounds.X = OxWh.S(borderBounds.Right, border.Value.Size);
                    break;
                case OxDock.Bottom:
                    borderBounds.Y = OxWh.S(borderBounds.Bottom, border.Value.Size);
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

    public bool SetSize(OxDock dock, OxWidth size)
    {
        OxWidth oldSize = this[dock].Size;

        if (oldSize.Equals(size))
            return false;

        OxBorders oldBorders = new(this);
        this[dock].Size = size;
        NotifyAboutSizeChanged(oldBorders);
        return true;
    }

    private void SetSize(OxWidth size)
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

    private OxWidth GetSize(OxDock dock) =>
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

    public OxBorders(OxWidth top, OxWidth left, OxWidth bottom, OxWidth right) : this() => 
        SetSize(top, left, bottom, right);

    public int LeftInt
    {
        get => OxWh.I(Left);
        set => Left = OxWh.W(value);
    }

    public OxWidth Left
    {
        get => GetSize(OxDock.Left);
        set => SetSize(OxDock.Left, value);
    }

    public int HorizontalInt
    {
        get => LeftInt;
        set
        {
            LeftInt = value;
            RightInt = value;
        }
    }

    public OxWidth Horizontal
    {
        get => OxWh.A(Left, Right);
        set
        {
            Left = value;
            Right = value;
        }
    }

    public int TopInt
    {
        get => OxWh.I(Top);
        set => Top = OxWh.W(value);
    }

    public OxWidth Top
    {
        get => GetSize(OxDock.Top);
        set => SetSize(OxDock.Top, value);
    }

    public OxWidth Vertical
    {
        get => OxWh.A(Top, Bottom);
        set
        {
            Top = value;
            Bottom = value;
        }
    }

    public int RightInt
    {
        get => OxWh.I(Right);
        set => Right = OxWh.W(value);
    }

    public OxWidth Right
    {
        get => GetSize(OxDock.Right);
        set => SetSize(OxDock.Right, value);
    }

    public int BottomInt
    {
        get => OxWh.I(Bottom);
        set => Bottom = OxWh.W(value);
    }

    public OxWidth Bottom
    {
        get => GetSize(OxDock.Bottom);
        set => SetSize(OxDock.Bottom, value);
    }

    public OxWidth Size
    {
        get => GetSize(OxDock.Left);
        set => SetSize(value);
    }

    public int SizeInt
    {
        get => OxWh.I(Size);
        set => Size = OxWh.W(value);
    }

    public void SetSize(OxWidth top, OxWidth left, OxWidth bottom, OxWidth right)
    {
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;
    }

    public void SetSize(int top, int left, int bottom, int right)
    {
        TopInt = top;
        LeftInt = left;
        BottomInt = bottom;
        RightInt = right;
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
        $"Left = {OxWh.I(Left)}\n"
        + $"Top = {OxWh.I(Top)}\n"
        + $"Right = {OxWh.I(Right)}\n"
        + $"Bottom = {OxWh.I(Bottom)}\n";
}