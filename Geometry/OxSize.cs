using OxLibrary.Handlers;

namespace OxLibrary;

public class OxSize
{
    private OxWidth width;

    private OxWidth height;

    public OxWidth Width
    {
        get => width;
        set
        {
            OxWidth oldValue = width;
            width = value;

            if (!oldValue.Equals(width))
                OnSizeChanged(new(oldValue, Height));
        }
    }

    public OxWidth Height
    {
        get => height;
        set
        {
            OxWidth oldValue = height;
            height = value;

            if (oldValue != height)
                OnSizeChanged(new(Width, oldValue));
        }
    }

    public OxSizeChangedEvent? SizeChanged;

    private readonly bool Creating = false;
    private void OnSizeChanged(OxSize oldSize)
    {
        if (Creating)
            return;

        SizeChanged?.Invoke(this, new OxSizeChangedEventArgs(oldSize, this));
    }

    public int Z_Width
    {
        get => OxWh.I(Width);
        set => Width = OxWh.W(value);
    }

    public int Z_Height
    {
        get => OxWh.I(Height);
        set => Height = OxWh.W(value);
    }

    public OxWidth ByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Width,
            OxDockVariable.Height =>
                Height,
            _ =>
                OxWh.W0
        };

    public OxSize(OxWidth width, OxWidth height)
    {
        Creating = true;

        try
        {
            Width = OxWh.Max(width, OxWh.W0);
            Height = OxWh.Max(height, OxWh.W0);
        }
        finally
        {
            Creating = false;
        }
    }

    public OxSize(OxWidth width, int height) :
        this(width, OxWh.W(height))
    { }

    public OxSize(int width, OxWidth height) :
        this(OxWh.W(width), height)
    { }

    public OxSize() : this(Size.Empty) { }

    public OxSize(int width, int height)
        : this(OxWh.W(width), OxWh.W(height)) { }

    public OxSize(Size size)
        : this(size.Width, size.Height) { }

    public OxSize(OxSize size)
        : this(size.Width, size.Height) { }

    public OxSize(Point point)
        : this(point.X, point.Y) { }

    public OxSize(OxPoint point)
        : this(point.X, point.Y) { }

    public Size Size =>
        new(Z_Width, Z_Height);

    public override bool Equals(object? obj) =>
        base.Equals(obj)
        || obj is OxSize otherSize
            && Width.Equals(otherSize.Width)
            && Height.Equals(otherSize.Height)
        ;

    public override int GetHashCode() =>
        Z_Width.GetHashCode() ^ Z_Height.GetHashCode();

    public override string ToString() =>
        $"Width = {OxWh.I(Width)}, Height = {OxWh.I(Height)}";

    public bool IsEmpty =>
        Width is OxWidth.None
        && Height is OxWidth.None;

    public static readonly OxSize Empty = new(0, 0);
}