using System.Collections.Generic;

namespace OxLibrary.Panels
{
    namespace OxLibrary.Panels
    {
        public class BorderEventArgs : EventArgs { }

        public delegate void BorderSizeEventHandler(object sender, BorderEventArgs e);

        public class OxBorders
        {
            public Dictionary<OxDock, OxBorder> Borders { get; } = new();

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

            public OxBorders()
            {
                foreach (OxDock dock in Enum.GetValues(typeof(OxDock)))
                    Borders.Add(dock, new());
            }

            public int Left
            {
                get => (int)LeftOx;
                set => LeftOx = (OxSize)value;
            }

            public OxSize LeftOx
            {
                get => GetSize(OxDock.Left);
                set => SetSize(OxDock.Left, value);
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
                get => LeftOx;
                set
                {
                    LeftOx = value;
                    RightOx = value;
                }
            }

            public int Top
            {
                get => (int)TopOx;
                set => TopOx = (OxSize)value;
            }

            public OxSize TopOx
            {
                get => GetSize(OxDock.Top);
                set => SetSize(OxDock.Top, value);
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
                get => TopOx;
                set
                {
                    TopOx = value;
                    BottomOx = value;
                }
            }

            public int Right
            {
                get => (int)RightOx;
                set => RightOx = (OxSize)value;
            }

            public OxSize RightOx
            {
                get => GetSize(OxDock.Right);
                set => SetSize(OxDock.Right, value);
            }

            public int Bottom
            {
                get => (int)BottomOx;
                set => BottomOx = (OxSize)value;
            }

            public OxSize BottomOx
            {
                get => GetSize(OxDock.Bottom);
                set => SetSize(OxDock.Bottom, value);
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

            public OxSize SizeAll
            {
                get => GetSize(OxDock.Left);
                set => SetSize(value);
            }

            public int IntSizeAll
            {
                get => (int)SizeAll;
                set => SizeAll = (OxSize)value;
            }

            public OxBorder this[OxDock dock] => Borders[dock];

            public BorderSizeEventHandler? SizeChanged;
        }
    }
}