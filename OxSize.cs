namespace OxLibrary
{
    public delegate void OxSizeChanged(OxSize newSize, OxSize oldSize);

    public class OxSize
    {
        private OxWidth width;

        private OxWidth height;

        public OxWidth Width
        {
            get => width;
            set
            { 
                OxWidth oldValue = width;
                width = value;

                if (oldValue != width)
                    OnSizeChanged(new(oldValue, Height));
            }
        }

        public OxWidth Height
        {
            get => height;
            set
            {
                OxWidth oldValue = height;
                height = value;

                if (oldValue != height)
                    OnSizeChanged(new(Width, oldValue));
            }
        }

        public OxSizeChanged? SizeChanged;

        private readonly bool Creating = false;
        private void OnSizeChanged(OxSize oldSize)
        {
            if (Creating)
                return;

            SizeChanged?.Invoke(this, oldSize);
        }

        public int WidthInt => OxWh.Int(Width);

        public int HeightInt => OxWh.Int(Height);

        public OxSize(OxWidth width, OxWidth height)
        {
            Creating = true;

            try
            {
                Width = OxWh.Max(width, OxWh.W0);
                Height = OxWh.Max(height, OxWh.W0);
            }
            finally
            { 
                Creating = false; 
            }
        }

        public OxSize(OxWidth width, int height) :
            this(width, OxWh.W(height)) { }

        public OxSize(int width, OxWidth height) :
            this(OxWh.W(width), height) { }

        public OxSize() : this(Size.Empty) { }

        public OxSize(int width, int height) 
            : this(OxWh.W(width), OxWh.W(height)) { }

        public OxSize(Size size)
            : this(size.Width, size.Height) { }
        public OxSize(OxSize size)
            : this(size.Width, size.Height) { }
        public OxSize(Point point)
            : this(point.X, point.Y) { }
        public OxSize(OxPoint point)
            : this(point.X, point.Y) { }

        public Size Size => 
            new(WidthInt, HeightInt);

        public override bool Equals(object? obj) => 
            base.Equals(obj)
            || (obj is OxSize otherSize
                && Width.Equals(otherSize.Width)
                && Height.Equals(otherSize.Height)
            );

        public override int GetHashCode() => 
            WidthInt.GetHashCode() ^ HeightInt.GetHashCode();

        public static readonly OxSize Empty = new(0, 0);
    }
}