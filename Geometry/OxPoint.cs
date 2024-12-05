
namespace OxLibrary;

public class OxPoint : OxSize
{
    private new OxWidth Width
    {
        get => base.Width;
        set => base.Width = value;
    }

    private new OxWidth Height
    {
        get => base.Height;
        set => base.Height = value;
    }

    private new Size Size => base.Size;

    public OxWidth X
    {
        get => Width;
        set => Width = value;
    }

    public OxWidth Y
    {
        get => Height;
        set => Height = value;
    }

    public OxPoint(OxWidth x, OxWidth y) : base(x, y) { }
    public OxPoint(OxWidth x, int y) : base(x, y) { }
    public OxPoint(int x, OxWidth y) : base(x, y) { }
    public OxPoint() : base() { }
    public OxPoint(int x, int y) : base(x, y) { }
    public OxPoint(Size size) : base(size) { }
    public OxPoint(OxSize size) : base(size) { }
    public OxPoint(Point point) : base(point) { }
    public OxPoint(OxPoint point) : base(point) { }
    public Point Point => new(Size);

    public override bool Equals(object? obj) =>
        base.Equals(obj)
        || obj is OxPoint otherPoint
            && X.Equals(otherPoint.X)
            && Y.Equals(otherPoint.Y);

    public override int GetHashCode() =>
        X.GetHashCode() ^ Y.GetHashCode();

    public override string ToString() =>
        $"X = {OxWh.I(X)}, Y = {OxWh.I(Y)}";

    public new static OxPoint Empty => new(OxWh.W0, OxWh.W0);

    private new int Z_Width
    {
        get => base.Z_Width;
        set => base.Z_Width = value;
    }

    private new int Z_Height
    {
        get => base.Z_Height;
        set => base.Z_Height = value;
    }

    public int Z_X
    {
        get => Z_Width;
        set => Z_Width = value;
    }

    public int Z_Y
    {
        get => Z_Height;
        set => Z_Height = value;
    }
}