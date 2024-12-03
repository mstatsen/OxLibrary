namespace OxLibrary
{
    public class OxBorder
    {
        public OxBorder() { }
        public OxBorder(OxBorder prototype) : this()
        { 
            Size = prototype.Size;
            Visible = prototype.Visible;
        }

        private OxWidth size = OxWh.W0;
        private bool visible = true;

        public OxWidth Size
        {
            get => visible ? size : OxWh.W0;
            set => size = value;
        }

        public int IntSize
        {
            get => OxWh.I(Size);
            set => Size = OxWh.W(value);
        }

        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        public bool IsEmpty =>
            Size is OxWidth.None;

        public override bool Equals(object? obj) => 
            obj is OxBorder otherBorder
            && Size == otherBorder.Size;

        public override int GetHashCode() => 
            Size.GetHashCode();

        public override string ToString() =>
            $"Size = {OxWh.I(Size)}";
    }
}