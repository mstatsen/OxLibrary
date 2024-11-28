namespace OxLibrary.Panels
{
    public class BorderEventArgs : EventArgs { }

    public delegate void BorderSizeEventHandler(object sender, BorderEventArgs e);

    public class OxBorders : Dictionary<OxDock, OxBorder>
    {
        public static OxBorders operator +(OxBorders left, OxBorders right) =>
            new(
                OxWh.A(left.Top, right.Top),
                OxWh.A(left.Left, right.Left),
                OxWh.A(left.Bottom, right.Bottom),
                OxWh.A(left.Right, right.Right)
            );

        public static OxBorders operator -(OxBorders left, OxBorders right) =>
            new(
                OxWh.S(left.Top, right.Top),
                OxWh.S(left.Left, right.Left),
                OxWh.S(left.Bottom, right.Bottom),
                OxWh.S(left.Right, right.Right)
            );

        public void Draw(Graphics g, OxRectangle bounds, Color color)
        {
            if (IsEmpty)
                return;

            foreach (var border in this)
            {
                if (border.Value.IsEmpty)
                    continue;

                OxRectangle borderBounds = new(bounds);

                /*
                if (substractSelfSize)
                {
                    borderBounds.Width = OxWh.Sub(borderBounds.Width, border.Value.Size);
                    borderBounds.Height = OxWh.Sub(borderBounds.Height, border.Value.Size);
                }
                */

                //Point startPoint = Point.Empty;
                //Point finishPoint = Point.Empty;

                switch (border.Key)
                {
                    case OxDock.Right:
                        borderBounds.X = OxWh.S(borderBounds.Right, border.Value.Size);
                        break;
                    case OxDock.Bottom:
                        borderBounds.Y = OxWh.S(borderBounds.Bottom, border.Value.Size);
                        break;
                }

                if (OxDockHelper.IsHorizontal(border.Key))
                    borderBounds.Width = border.Value.Size;
                else
                    if (OxDockHelper.IsVertical(border.Key))
                        borderBounds.Height = border.Value.Size;

                using Brush brush = new SolidBrush(color);
                g.FillRectangle(brush, borderBounds.Rectangle);
            }
        }

        private bool SizeChanging = false;
        private void NotifyAboutSizeChanged()
        {
            if (!SizeChanging)
                SizeChanged?.Invoke(this, new BorderEventArgs());
        }

        public bool SetSize(OxDock dock, OxWidth size)
        {
            OxWidth oldSize = this[dock].Size;

            if (oldSize.Equals(size))
                return false;

            this[dock].Size = size;
            NotifyAboutSizeChanged();
            return true;
        }

        private void SetSize(OxWidth size)
        {
            bool sizeChanged = false;
            SizeChanging = true;

            try
            {
                foreach (OxDock border in Keys)
                    sizeChanged |= SetSize(border, size);
            }
            finally
            {
                SizeChanging = false;
            }

            if (sizeChanged)
                NotifyAboutSizeChanged();
        }

        private OxWidth GetSize(OxDock dock) =>
            this[dock].Size;

        public OxBorders()
        {
            foreach (OxDock dock in OxDockHelper.SingleDirectionDocks)
                Add(dock, new());
        }

        public OxBorders(OxWidth top, OxWidth left, OxWidth bottom, OxWidth right) : this() => 
            SetSize(top, left, bottom, right);

        public int LeftInt
        {
            get => OxWh.Int(Left);
            set => Left = OxWh.W(value);
        }

        public OxWidth Left
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

        public OxWidth Horizontal
        {
            get => Left;
            set
            {
                Left = value;
                Right = value;
            }
        }

        public int HorizontalFullInt => 
            LeftInt + RightInt;

        public OxWidth HorizontalFull => 
            OxWh.A(Left, Right);

        public int TopInt
        {
            get => OxWh.Int(Top);
            set => Top = OxWh.W(value);
        }

        public OxWidth Top
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

        public OxWidth Vertical
        {
            get => Top;
            set
            {
                Top = value;
                Bottom = value;
            }
        }

        public int VerticalIntFull => 
            TopInt + BottomInt;

        public OxWidth VerticalFull =>
            OxWh.A(Top, Bottom);

        public int RightInt
        {
            get => OxWh.Int(Right);
            set => Right = OxWh.W(value);
        }

        public OxWidth Right
        {
            get => GetSize(OxDock.Right);
            set => SetSize(OxDock.Right, value);
        }

        public int BottomInt
        {
            get => OxWh.Int(Bottom);
            set => Bottom = OxWh.W(value);
        }

        public OxWidth Bottom
        {
            get => GetSize(OxDock.Bottom);
            set => SetSize(OxDock.Bottom, value);
        }

        public OxWidth Size
        {
            get => GetSize(OxDock.Left);
            set => SetSize(value);
        }

        public int SizeInt
        {
            get => OxWh.Int(Size);
            set => Size = OxWh.W(value);
        }

        public void SetSize(OxWidth top, OxWidth left, OxWidth bottom, OxWidth right)
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
            this[dock].Visible;

        public bool SetVisible(OxDock dock, bool visible)
        {
            bool visibleChanged = !this[dock].Visible.Equals(visible);

            if (visibleChanged)
            {
                this[dock].Visible = visible;
                NotifyAboutSizeChanged();
            }

            return visibleChanged;
        }

        public void SetVisible(bool visible)
        {
            bool visibleChanged = false;

            foreach (OxDock dock in Keys) 
                visibleChanged |= SetVisible(dock, visible);

            if (visibleChanged)
                NotifyAboutSizeChanged();
        }

        public bool EqualsPadding(Padding padding) => 
            padding.Left.Equals(LeftInt)
            && padding.Right.Equals(RightInt)
            && padding.Top.Equals(TopInt)
            && padding.Bottom.Equals(BottomInt);

        public Padding AsPadding => 
            new(LeftInt, TopInt, RightInt, BottomInt);

        public bool GetVisible() =>
            this[OxDock.Top].Visible
            || this[OxDock.Left].Visible
            || this[OxDock.Bottom].Visible
            || this[OxDock.Right].Visible;

        public bool IsEmpty
        {
            get 
            {
                foreach (OxDock dock in Keys)
                    if (!this[dock].IsEmpty)
                        return false;

                return true;
            }
        }

        public override bool Equals(object? obj) =>
            obj is OxBorders otherBorders
            && Left.Equals(otherBorders.Left)
            && Right.Equals(otherBorders.Right)
            && Top.Equals(otherBorders.Top)
            && Bottom.Equals(otherBorders.Bottom);

        public BorderSizeEventHandler? SizeChanged;

        public override int GetHashCode()
        {
            int result = 0;

            foreach (OxBorder border in this.Values)
                result ^= border.GetHashCode();

            return result;
        }
    }
}