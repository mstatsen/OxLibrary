namespace OxLibrary.Panels
{
    public class OxBorder
    {
        public OxBorder() { }

        private OxWidth size = OxWh.W0;
        private bool visible = true;

        public OxWidth Size
        {
            get => visible ? size : OxWh.W0;
            set => size = value;
        }

        public int IntSize
        {
            get => OxWh.Int(Size);
            set => Size = OxWh.W(value);
        }

        public bool Visible
        {
            get => visible;
            set => visible = value;
        }
    }
}