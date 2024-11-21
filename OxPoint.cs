namespace OxLibrary
{
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

        public int XInt => OxWh.Int(X);

        public int YInt => OxWh.Int(Y);

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

        public new static readonly OxPoint Empty = new(0, 0);
    }
}