
namespace OxLibrary;

public class OxRectangle
{
    public static OxRectangle operator -(OxRectangle rect, OxBorders borders) =>
        new(
            OxWh.A(rect.X, borders.Left),
            OxWh.A(rect.Y, borders.Top),
            OxWh.S(OxWh.S(rect.Width, borders.Left), borders.Right),
            OxWh.S(OxWh.S(rect.Height, borders.Top), borders.Bottom)
        );

    public static OxRectangle operator +(OxRectangle rect, OxBorders borders) =>
        new(
            OxWh.S(rect.X, borders.Left),
            OxWh.S(rect.Y, borders.Top),
            OxWh.A(OxWh.A(rect.Width, borders.Left), borders.Right),
            OxWh.A(OxWh.A(rect.Height, borders.Top), borders.Bottom)
        );

    private OxWidth x;

    private OxWidth y;

    private OxWidth width;

    private OxWidth height;
    public OxWidth X
    {
        get => x;
        set => x = value;
    }

    public OxWidth Y
    {
        get => y;
        set => y = value;
    }

    public OxWidth Width
    {
        get => width;
        set => width = value;
    }

    public OxWidth Height
    {
        get => height;
        set => height = value;
    }

    public OxWidth Right => OxWh.A(X, Width);
    public OxWidth Bottom => OxWh.A(Y, Height);

    public OxPoint TopLeft => new(X, Y);
    public OxPoint TopRight => new(Right, Y);

    public OxPoint BottomLeft => new(X, Bottom);
    public OxPoint BottomRight => new(Right, Bottom);

    private void Set(OxWidth x, OxWidth y, OxWidth width, OxWidth height)
    {
        X = OxWh.Max(x, OxWh.W0);
        Y = OxWh.Max(y, OxWh.W0);
        Width = OxWh.Max(width, OxWh.W0);
        Height = OxWh.Max(height, OxWh.W0);
    }

    public OxRectangle(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
        Set(x, y, width, height);

    public OxRectangle() : this(Rectangle.Empty) { }

    public OxRectangle(int x, int y, int width, int height)
        : this(
              OxWh.W(x), 
              OxWh.W(y), 
              OxWh.W(width), 
              OxWh.W(height)
        ) { }

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
            OxWh.I(X), 
            OxWh.I(Y), 
            OxWh.I(Width), 
            OxWh.I(Height)
        );

    public OxPoint Location => new(X, Y);

    public OxSize Size => new(Width, Height);

    public bool Contains(OxWidth x, OxWidth y) =>
        OxWh.LessOrEquals(X, x)
        && OxWh.Less(x, X | Width)
        && OxWh.LessOrEquals(Y, y)
        && OxWh.Less(y, Y | Height);

    public bool Contains(OxPoint pt) => Contains(pt.X, pt.Y);

    public bool Contains(OxRectangle rect) =>
        OxWh.LessOrEquals(X, rect.X)
        && OxWh.LessOrEquals(rect.X | rect.Width, X | Width)
        && OxWh.LessOrEquals(Y, rect.Y)
        && OxWh.LessOrEquals(rect.Y | rect.Height, Y | Height);

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
        OxWh.S(Width, X) is OxWidth.None
        || OxWh.S(Height, Y) is OxWidth.None;

    public void Clear() =>
        Set(OxWh.W0, OxWh.W0, OxWh.W0, OxWh.W0);

    public override string ToString() =>
        $"X = {OxWh.I(X)}, Y = {OxWh.I(Y)}, Width = {OxWh.I(Width)}, Height = {OxWh.I(Height)}";

    public OxWidth FirstByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                X,
            OxDockVariable.Height =>
                Y,
            _ => OxWh.W0,
        };

    public OxWidth LastByDockVariable(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Right,
            OxDockVariable.Height =>
                Bottom,
            _ => OxWh.W0,
        };

    public static OxRectangle Empty => new(OxWh.W0, OxWh.W0, OxWh.W0, OxWh.W0);
    public static OxRectangle Max => new(OxWh.W0, OxWh.W0, OxWh.Maximum, OxWh.Maximum);

    #region Internal properties
    [Obsolete("Z_X it is used only for internal needs. Instead, use X")]
    public int Z_X
    {
        get => OxWh.I(x);
        set => x = OxWh.W(value);
    }

    [Obsolete("Z_Y it is used only for internal needs. Instead, use Y")]
    public int Z_Y
    {
        get => OxWh.I(y);
        set => y = OxWh.W(value);
    }

    [Obsolete("Z_Width it is used only for internal needs. Instead, use Width")]
    public int Z_Width
    {
        get => OxWh.I(width);
        set => width = OxWh.W(value);
    }

    [Obsolete("Z_Height it is used only for internal needs. Instead, use Height")]
    public int Z_Height
    {
        get => OxWh.I(height);
        set => height = OxWh.W(value);
    }

    [Obsolete("Z_Location it is used only for internal needs. Instead, use Location")]
    public Point Z_Location
    {
        get => new(Z_X, Z_Y);
        set
        {
            Z_X = value.X;
            Z_Y = value.Y;
        }
    }

    [Obsolete("Z_Size it is used only for internal needs. Instead, use Size")]
    public Size Z_Size
    {
        get => new(Z_Width, Z_Height);
        set
        {
            Z_Width = value.Width;
            Z_Height = value.Height;
        }
    }
    #endregion
}