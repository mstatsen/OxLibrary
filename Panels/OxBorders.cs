namespace OxLibrary.Panels
{
    public class BorderEventArgs : EventArgs { }

    public delegate void BorderSizeEventHandler(object sender, BorderEventArgs e);

    public class OxBorders
    {
        private bool SizeChanging = false;
        private void NotifyAboutSizeChanged()
        {
            if (!SizeChanging)
                SizeChanged?.Invoke(this, new BorderEventArgs());
        }

        private bool SetSize(OxDock dock, OxSize size)
        {
            OxSize oldSize = Borders[dock].Size;

            if (oldSize.Equals(size))
                return false;

            Borders[dock].Size = size;
            NotifyAboutSizeChanged();
            return true;
        }

        private void SetSize(OxSize size)
        {
            bool sizeChanged = false;
            SizeChanging = true;

            try
            {
                foreach (OxDock border in Borders.Keys)
                    sizeChanged |= SetSize(border, size);
            }
            finally
            {
                SizeChanging = false;
            }

            if (sizeChanged)
                NotifyAboutSizeChanged();
        }

        private OxSize GetSize(OxDock dock) =>
            Borders[dock].Size;

        public OxBorders()
        {
            foreach (OxDock dock in Enum.GetValues(typeof(OxDock)))
                Borders.Add(dock, new());
        }
        public Dictionary<OxDock, OxBorder> Borders { get; } = new();

        public int LeftInt
        {
            get => (int)Left;
            set => Left = (OxSize)value;
        }

        public OxSize Left
        {
            get => GetSize(OxDock.Left);
            set => SetSize(OxDock.Left, value);
        }

        public int HorizontalInt
        {
            get => LeftInt;
            set
            {
                LeftInt = value;
                RightInt = value;
            }
        }

        public OxSize Horizontal
        {
            get => Left;
            set
            {
                Left = value;
                Right = value;
            }
        }

        public int TopInt
        {
            get => (int)Top;
            set => Top = (OxSize)value;
        }

        public OxSize Top
        {
            get => GetSize(OxDock.Top);
            set => SetSize(OxDock.Top, value);
        }

        public int VerticalInt
        {
            get => LeftInt;
            set
            {
                TopInt = value;
                BottomInt = value;
            }
        }

        public OxSize Vertical
        {
            get => Top;
            set
            {
                Top = value;
                Bottom = value;
            }
        }

        public int RightInt
        {
            get => (int)Right;
            set => Right = (OxSize)value;
        }

        public OxSize Right
        {
            get => GetSize(OxDock.Right);
            set => SetSize(OxDock.Right, value);
        }

        public int BottomInt
        {
            get => (int)Bottom;
            set => Bottom = (OxSize)value;
        }

        public OxSize Bottom
        {
            get => GetSize(OxDock.Bottom);
            set => SetSize(OxDock.Bottom, value);
        }

        public OxSize Size
        {
            get => GetSize(OxDock.Left);
            set => SetSize(value);
        }

        public int SizeInt
        {
            get => (int)Size;
            set => Size = (OxSize)value;
        }

        public void SetSize(OxSize top, OxSize left, OxSize bottom, OxSize right)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public void SetSize(int top, int left, int bottom, int right)
        {
            TopInt = top;
            LeftInt = left;
            BottomInt = bottom;
            RightInt = right;
        }

        public bool Visible(OxDock dock) => 
            Borders[dock].Visible;

        public bool SetVisible(OxDock dock, bool visible)
        {
            bool visibleChanged = !Borders[dock].Visible.Equals(visible);

            if (visibleChanged)
            {
                Borders[dock].Visible = visible;
                NotifyAboutSizeChanged();
            }

            return visibleChanged;
        }

        public void SetVisible(bool visible)
        {
            bool visibleChanged = false;

            foreach (OxDock dock in Borders.Keys) 
                visibleChanged |= SetVisible(dock, visible);

            if (visibleChanged)
                NotifyAboutSizeChanged();
        }
            

        public bool AllVisible
        { 
            get => 
                Borders[OxDock.Top].Visible
                && Borders [OxDock.Left].Visible
                && Borders[OxDock.Bottom].Visible
                && Borders[OxDock.Right].Visible;
            set => SetVisible(value);
        }

        public OxBorder this[OxDock dock] => Borders[dock];

        public BorderSizeEventHandler? SizeChanged;
    }
}