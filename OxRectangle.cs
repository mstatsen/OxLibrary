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

        public OxRectangle(OxWidth x, OxWidth y, OxWidth width, OxWidth height)
        {
            X = OxWh.Max(x, OxWh.W0);
            Y = OxWh.Max(y, OxWh.W0);
            Width = OxWh.Max(width, OxWh.W0);
            Height = OxWh.Max(height, OxWh.W0);
        }

        public OxRectangle() : this(Rectangle.Empty) { }

        public OxRectangle(int x, int y, int width, int height) 
            : this(OxWh.W(x), OxWh.W(y), OxWh.W(width), OxWh.W(height)) { }

        public OxRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

        public OxRectangle(OxPoint point, OxSize size)
            : this(point.X, point.Y, size.Width, size.Height) { }

        public Rectangle Rectandle => 
            new(OxWh.Int(X), OxWh.Int(Y), OxWh.Int(Width), OxWh.Int(Height));

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

        public static readonly OxRectangle Empty = new(OxWh.W0, OxWh.W0, OxWh.W0, OxWh.W0);
    }
}