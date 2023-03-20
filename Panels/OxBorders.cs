namespace OxLibrary.Panels
{
    public class BorderEventArgs : EventArgs { }

    public delegate void BorderSizeEventHandler(object sender, BorderEventArgs e);

    public class OxBorders
    {
        public Dictionary<OxDock, OxBorder> Borders { get; internal set; }

        private void NotifyAboutSizeChanged() =>
            SizeChanged?.Invoke(this, new BorderEventArgs());

        private void SetBorderSize(OxDock dock, int size)
        {
            if (Borders[dock].SetSize(size))
                NotifyAboutSizeChanged();
        }

        public OxBorders(Control control) =>
            Borders = OxBorder.NewFull(control, Color.Transparent, OxSize.None);

        public int Left
        {
            get => Borders[OxDock.Left].GetSize();
            set => SetBorderSize(OxDock.Left, value);
        }

        public OxSize LeftOx
        {
            set => SetBorderSize(OxDock.Left, (int)value);
        }

        public int Horizontal
        {
            get => Left;
            set
            {
                Left = value;
                Right = value;
            }
        }

        public OxSize HorizontalOx
        {
            set
            {
                LeftOx = value;
                RightOx = value;
            }
        }

        public int Top
        {
            get => Borders[OxDock.Top].GetSize();
            set => SetBorderSize(OxDock.Top, value);
        }

        public OxSize TopOx
        {
            set => SetBorderSize(OxDock.Top, (int)value);
        }

        public int Vertical
        {
            get => Left;
            set
            {
                Top = value;
                Bottom = value;
            }
        }

        public OxSize VerticalOx
        {
            set
            {
                TopOx = value;
                BottomOx = value;
            }
        }

        public int Right
        {
            get => Borders[OxDock.Right].GetSize();
            set => SetBorderSize(OxDock.Right, value);
        }

        public OxSize RightOx
        {
            set => SetBorderSize(OxDock.Right, (int)value);
        }

        public int Bottom
        {
            get => Borders[OxDock.Bottom].GetSize();
            set => SetBorderSize(OxDock.Bottom, value);
        }

        public OxSize BottomOx
        {
            set => SetBorderSize(OxDock.Bottom, (int)value);
        }

        public void SetSize(int size)
        {
            bool sizeChanged = false;

            foreach (OxBorder border in Borders.Values)
                sizeChanged |= border.SetSize(size);

            if (sizeChanged)
                NotifyAboutSizeChanged();
        }

        public void SetSize(OxSize size) =>
            SetSize((int)size);

        public int GetSize() =>
            Borders[OxDock.Left].GetSize();

        public int CalcedSize(OxDock dock) =>
            Borders[dock].CalcedSize;

        public OxBorder this[OxDock dock] => Borders[dock];

        public BorderSizeEventHandler? SizeChanged;

        public Color Color
        {
            get => Borders[OxDock.Left].BackColor;
            set
            {
                foreach (OxBorder border in Borders.Values)
                    border.BackColor = value;
            }
        }

        public void ReAlign()
        {
            foreach (OxBorder border in Borders.Values)
                border.ReAlign();
        }
    }
}