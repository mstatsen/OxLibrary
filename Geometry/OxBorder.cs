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
    private bool visible = true;

    public short Size
    {
        get => visible ? size : (short)0;
        set => size = value;
    }

    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    public bool IsEmpty =>
        Size is 0;

    public override bool Equals(object? obj) => 
        obj is OxBorder otherBorder
        && Size.Equals(otherBorder.Size);

    public override int GetHashCode() => 
        Size.GetHashCode();

    public override string ToString() =>
        $"Size = {Size}";
}