using OxLibrary.Geometry;

namespace OxLibrary;

public class OxBorder
{
    public OxBorder() { }
    public OxBorder(OxBorder prototype) : this()
    { 
        Size = prototype.Size;
        Visible = prototype.Visible;
    }

    private short size = 0;
    private OxBool visible = OxB.T;

    public short Size
    {
        get => OxSh.Short(OxB.B(visible) ? size : 0);
        set => size = value;
    }

    public OxBool Visible
    {
        get => visible;
        set => visible = value;
    }

    public bool IsVisible =>
        OxB.B(Visible);

    public void SetVisible(bool value) =>
        Visible = OxB.B(value);

    public OxBool IsEmpty =>
        OxB.B(Size is 0);

    public override bool Equals(object? obj) => 
        obj is OxBorder otherBorder
        && Size.Equals(otherBorder.Size);

    public override int GetHashCode() => 
        Size.GetHashCode();

    public override string ToString() =>
        $"Size = {Size}";
}