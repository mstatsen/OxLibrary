
namespace OxLibrary;

public class OxPoint : OxSize
{
    private new short Width
    {
        get => base.Width;
        set => base.Width = value;
    }

    private new short Height
    {
        get => base.Height;
        set => base.Height = value;
    }

    private new Size Size => base.Size;

    public short X
    {
        get => Width;
        set => Width = value;
    }

    public short Y
    {
        get => Height;
        set => Height = value;
    }

    public OxPoint() : base() { }
    public OxPoint(short x, short y) : base(x, y) { }
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
        $"X = {X}, Y = {Y}";

    public new static OxPoint Empty => new(0, 0);
}