namespace OxLibrary
{
    public class OxRectangle
    {
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
            : this(OxWh.W(x), OxWh.W(y), OxWh.W(width), OxWh.W(height)) { }

        public OxRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

        public OxRectangle(OxPoint location, OxSize size)
            : this(location.X, location.Y, size.Width, size.Height) { }

        public Rectangle Rectangle => 
            new(OxWh.Int(X), OxWh.Int(Y), OxWh.Int(Width), OxWh.Int(Height));

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
            || (obj is OxRectangle otherRect
                && X.Equals(otherRect.X)
                && Y.Equals(otherRect.Y)
                && Width.Equals(otherRect.Width)
                && Height.Equals(otherRect.Height)
            );

        public override int GetHashCode() => 
            X.GetHashCode() 
            ^ Y.GetHashCode() 
            ^ Width.GetHashCode() 
            ^ Height.GetHashCode();

        public bool IsEmpty =>
            X is OxWidth.None
            && Y is OxWidth.None
            && Width is OxWidth.None
            && Height is OxWidth.None;

        public void Clear() => 
            Set(OxWh.W0, OxWh.W0, OxWh.W0, OxWh.W0);

        public static readonly OxRectangle Empty = new(OxWh.W0, OxWh.W0, OxWh.W0, OxWh.W0);
    }
}