using OxLibrary.Geometry;

namespace OxLibrary;

public class OxRectangle
{
    public static OxRectangle operator -(OxRectangle rect, OxBorders borders) =>
        new(
            OxSH.Add(rect.X, borders.Left),
            OxSH.Add(rect.Y, borders.Top),
            OxSH.Sub(rect.Width, borders.Left, borders.Right),
            OxSH.Sub(rect.Height, borders.Top, borders.Bottom)
        );

    public static OxRectangle operator +(OxRectangle rect, OxBorders borders) =>
        new(
            OxSH.Sub(rect.X, borders.Left),
            OxSH.Sub(rect.Y, borders.Top),
            OxSH.Add(rect.Width, borders.Left, borders.Right),
            OxSH.Add(rect.Height, borders.Top, borders.Bottom)
        );

    private short x;

    private short y;

    private short width;

    private short height;
    public short X
    {
        get => x;
        set => x = value;
    }

    public short Y
    {
        get => y;
        set => y = value;
    }

    public short Width
    {
        get => width;
        set => width = value;
    }

    public short Height
    {
        get => height;
        set => height = value;
    }

    public short Right => OxSH.Add(X, Width);
    public short Bottom => OxSH.Add(Y, Height);

    public OxPoint TopLeft => new(X, Y);
    public OxPoint TopRight => new(Right, Y);

    public OxPoint BottomLeft => new(X, Bottom);
    public OxPoint BottomRight => new(Right, Bottom);


    public OxRectangle() : this(Rectangle.Empty) { }

    public OxRectangle(int x, int y, int width, int height)
    { 
        X = OxSH.Short(x);
        Y = OxSH.Short(y);
        Width = OxSH.Short(width);
        Height = OxSH.Short(height);
    }

    public OxRectangle(Rectangle rectangle)
        : this(
              rectangle.X,
              rectangle.Y,
              rectangle.Width,
              rectangle.Height
        ) { }

    public OxRectangle(OxRectangle rectangle)
        : this() =>
        CopyFrom(rectangle);

    public OxRectangle(OxPoint location, OxSize size)
        : this(
              location.X, 
              location.Y, 
              size.Width, 
              size.Height
        ) { }

    public OxRectangle(Point location, Size size)
        : this(
              new OxPoint(location),
              new OxSize(size)
        )
    { }

    public Rectangle Rectangle =>
        new(
            X, 
            Y, 
            Width, 
            Height
        );

    public OxPoint Location => new(X, Y);

    public OxSize Size => new(Width, Height);

    public bool Contains(short x, short y) =>
        X <= x
        && x < X + Width
        && Y <= y
        && y < Y + Height;

    public bool Contains(OxPoint pt) => Contains(pt.X, pt.Y);

    public bool Contains(OxRectangle rect) =>
        X <= rect.X
        && rect.X + rect.Width <= X + Width
        && Y <= rect.Y
        && rect.Y + rect.Height <= Y + Height;

    public override bool Equals(object? obj) =>
        base.Equals(obj)
        || obj is OxRectangle otherRect
            && X.Equals(otherRect.X)
            && Y.Equals(otherRect.Y)
            && Width.Equals(otherRect.Width)
            && Height.Equals(otherRect.Height)
        ;

    public override int GetHashCode() =>
        X.GetHashCode()
        ^ Y.GetHashCode()
        ^ Width.GetHashCode()
        ^ Height.GetHashCode();

    public void CopyFrom(OxRectangle other)
    { 
        X = other.X;
        Y = other.Y;
        Width = other.Width;
        Height = other.Height;
    }

    public bool IsEmpty => 
        Width - X is 0
        || Height - Y is 0;

    public void Clear()
    { 
        X = 0;
        Y = 0;
        Width = 0;
        Height = 0;
    }

    public override string ToString() =>
        $"X = {X}, Y = {Y}, Width = {Width}, Height = {Height}";

    public short FirstByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                X,
            OxDockVariable.Height =>
                Y,
            _ => 0,
        };

    public short LastByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Right,
            OxDockVariable.Height =>
                Bottom,
            _ => 0,
        };

    public static OxRectangle Empty => new(0, 0, 0, 0);
    public static OxRectangle Max => new(0, 0, short.MaxValue, short.MaxValue);
}