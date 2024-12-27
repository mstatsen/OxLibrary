using OxLibrary.Geometry;
using OxLibrary.Handlers;

namespace OxLibrary;

public class OxSize
{
    private short width;

    private short height;

    public short Width
    {
        get => width;
        set
        {
            short oldValue = width;
            width = value;

            if (!oldValue.Equals(width))
                OnSizeChanged(new(oldValue, Height));
        }
    }

    public short Height
    {
        get => height;
        set
        {
            short oldValue = height;
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

    public short ByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Width,
            OxDockVariable.Height =>
                Height,
            _ =>
                0
        };

    public OxSize() : this(0) { }

    public OxSize(int size) : this(size, size) { }

    public OxSize(int width, int height)
    {
        Creating = true;

        try
        {
            Width = OxSh.Max(width, 0);
            Height = OxSh.Max(height, 0);
        }
        finally
        {
            Creating = false;
        }
    }

    public OxSize(Size size)
        : this(size.Width, size.Height) { }

    public OxSize(OxSize size)
        : this(size.Width, size.Height) { }

    public OxSize(Point point)
        : this(point.X, point.Y) { }

    public OxSize(OxPoint point)
        : this(point.Point) { }

    public Size Size =>
        new(Width, Height);

    public override bool Equals(object? obj) =>
        base.Equals(obj)
        || obj is OxSize otherSize
            && Width.Equals(otherSize.Width)
            && Height.Equals(otherSize.Height)
        ;

    public override int GetHashCode() =>
        Width.GetHashCode() ^ Height.GetHashCode();

    public override string ToString() =>
        $"Width = {Width}, Height = {Height}";

    public bool IsEmpty =>
        Width is 0
        && Height is 0;

    public OxSize ToDPI(Control control) =>
        new(
            OxSh.ToDPI(control, Width), 
            OxSh.ToDPI(control, Height)
        );

    public static readonly OxSize Empty = new(0, 0);
}