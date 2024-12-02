using OxLibrary.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public partial class OxPicture : OxPanel
    {
        private class OxPictureBox : PictureBox, IOxControl<PictureBox>
        {
            private readonly OxControlManager<PictureBox> manager;
            public IOxControlManager Manager => manager;

            public OxPictureBox()
            {
                manager = OxControlManager.RegisterControl<PictureBox>(this);
                DoubleBuffered = true;
            }

            public new OxWidth Width
            {
                get => manager.Width;
                set => manager.Width = value;
            }

            public new OxWidth Height
            {
                get => manager.Height;
                set => manager.Height = value;
            }

            public new OxWidth Top
            {
                get => manager.Top;
                set => manager.Top = value;
            }

            public new OxWidth Left
            {
                get => manager.Left;
                set => manager.Left = value;
            }

            public new OxWidth Bottom =>
                manager.Bottom;

            public new OxWidth Right =>
                manager.Right;

            public new OxSize Size
            {
                get => manager.Size;
                set => manager.Size = value;
            }

            public new OxSize ClientSize
            {
                get => manager.ClientSize;
                set => manager.ClientSize = value;
            }

            public new OxPoint Location
            {
                get => manager.Location;
                set => manager.Location = value;
            }

            public new OxSize MinimumSize
            {
                get => manager.MinimumSize;
                set => manager.MinimumSize = value;
            }

            public new OxSize MaximumSize
            {
                get => manager.MaximumSize;
                set => manager.MaximumSize = value;
            }

            public new virtual OxDock Dock
            {
                get => manager.Dock;
                set => manager.Dock = value;
            }

            public new virtual IOxControlContainer? Parent
            {
                get => manager.Parent;
                set => manager.Parent = value;
            }

            public new OxRectangle ClientRectangle =>
                manager.ClientRectangle;

            public new virtual OxRectangle DisplayRectangle =>
                manager.DisplayRectangle;

            public new OxRectangle Bounds
            {
                get => manager.Bounds;
                set => manager.Bounds = value;
            }

            public new OxSize PreferredSize =>
                manager.PreferredSize;

            public new OxPoint AutoScrollOffset
            {
                get => manager.AutoScrollOffset;
                set => manager.AutoScrollOffset = value;
            }
            public void DoWithSuspendedLayout(Action method) =>
                manager.DoWithSuspendedLayout(method);

            public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
                manager.GetChildAtPoint(pt, skipValue);

            public Control GetChildAtPoint(OxPoint pt) =>
                manager.GetChildAtPoint(pt);

            public OxSize GetPreferredSize(OxSize proposedSize) =>
                manager.GetPreferredSize(proposedSize);

            public void Invalidate(OxRectangle rc) =>
                manager.Invalidate(rc);

            public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
                manager.Invalidate(rc, invalidateChildren);

            public OxSize LogicalToDeviceUnits(OxSize value) =>
                manager.LogicalToDeviceUnits(value);

            public OxPoint PointToClient(OxPoint p) =>
                manager.PointToClient(p);

            public OxPoint PointToScreen(OxPoint p) =>
                manager.PointToScreen(p);

            public OxRectangle RectangleToClient(OxRectangle r) =>
                manager.RectangleToClient(r);

            public OxRectangle RectangleToScreen(OxRectangle r) =>
                manager.RectangleToScreen(r);

            public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
                manager.SetBounds(x, y, width, height, specified);

            public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
                manager.SetBounds(x, y, width, height);

            public new event OxSizeChanged SizeChanged
            {
                add => manager.SizeChanged += value;
                remove => manager.SizeChanged -= value;
            }

            public new event OxLocationChanged LocationChanged
            {
                add => manager.LocationChanged += value;
                remove => manager.LocationChanged -= value;
            }

            public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
            public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members
            private static new void OnLocationChanged(EventArgs e) { }
            private static new void OnSizeChanged(EventArgs e) { }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
        }

        public bool AlwaysEnabled { get; set; } = false;

        private Bitmap? enabledBitmap;
        private Bitmap? EnabledBitmap 
        {
            get => enabledBitmap;
            set => SetEnabledBitmap(value);
        }

        public bool Stretch 
        { 
            get => picture.Dock is OxDock.Fill;
            set => 
                picture.Dock = 
                    value 
                        ? OxDock.Fill 
                        : OxDock.None;
        }

        private void SetEnabledBitmap(Bitmap? value)
        {
            enabledBitmap = value;
            DisabledBitmap = 
                value is null 
                    ? null 
                    : GetGrayScale(EnabledBitmap);
        }

        private Bitmap? DisabledBitmap;

        private readonly OxPictureBox picture = new();
        public PictureBox Picture => picture;

        public OxPicture()
        {
            BackColor = Color.Transparent;
            Width = OxWh.W24;
            Height = OxWh.W24;
            PictureSize = OxWh.W16;
        }

        public OxWidth PictureSize
        {
            get => picture.Height;
            set => SetPictureSize(value);
        }

        private OxWidth picturePadding = OxWh.W0;

        public OxWidth PicturePadding
        {
            get => picturePadding;
            set => SetPicturePadding(value);
        }

        private void SetPicturePadding(OxWidth value)
        {
            picturePadding = value;
            PictureSize = OxWh.Sub(Height, OxWh.Mul(picturePadding, OxWh.W2));
        }

        private void SetPictureSize(OxWidth value)
        {
            if (Stretch)
                return;
            
            picture.Width = value;
            picture.Height = value;
            CorrectPicturePosition();
        }

        private void CorrectPicturePosition()
        {
            if (Stretch)
                return;

            picturePadding = OxWh.Div(OxWh.Sub(Height, picture.Height), OxWh.W2);

            if (picturePadding < 0)
                picturePadding = 0;

            picture.Left = OxWh.Div(OxWh.Sub(Width, picture.Width), OxWh.W2);
            picture.Top = OxWh.Div(OxWh.Sub(Height, picture.Height), OxWh.W2);
        }

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            PreparePicture();
        }

        private void PreparePicture()
        {
            picture.Dock = OxDock.None;
            picture.Parent = this;
            picture.Click += (s, e) => InvokeOnClick(this, null);
            SetPictureSize(Height);
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            picture.SizeChanged += PictureSizeChanged;
            SetHoverHandlers(picture);
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
                CorrectPicturePosition();
        }

        public override void PrepareColors()
        {
            base.PrepareColors();
            picture.BackColor = BackColor;
        }

        private void PictureSizeChanged(object sender, OxSizeChangedEventArgs e)
        {
            if (Stretch)
                return;
            
            CorrectPicturePosition();
            SetImage(picture.Image);
        }

        private void SetHoverHandlers(Control control)
        {
            control.MouseEnter += (s, e) => OnMouseEnter(e);
            control.MouseLeave += (s, e) => OnMouseLeave(e);
        }

        public Image? Image
        {
            get => picture.Image;
            set => SetImage(value);
        }

        private void SetImage(Image? value)
        {
            if (value is null)
            {
                EnabledBitmap = null;
                picture.Image = null;
                return;
            }

            OxBitmapCalcer bitmapCalcer = new(value, new(picture.Size), Stretch);
            picture.SizeMode = bitmapCalcer.SizeMode;

            if (!Stretch)
            {
                picture.Width = bitmapCalcer.ImageBox.Width;
                picture.Height = bitmapCalcer.ImageBox.Height;
            }

            EnabledBitmap = bitmapCalcer.FullBitmap;
            SetPictureImage();
        }

        private static Bitmap? GetGrayScale(Bitmap? bitmap)
        {
            if (bitmap is null)
                return null;

            Bitmap result = new(bitmap);

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    Color oc = result.GetPixel(x, y);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));

                    if (grayScale < 110)
                        grayScale = 110;

                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    result.SetPixel(x, y, nc);
                }

            return result;
        }


        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            picture.Enabled = Enabled;
            SetPictureImage();
        }

        public bool ReadOnly 
        { 
            get => !Enabled; 
            set => Enabled = !value; 
        }

        private void SetPictureImage() => 
            picture.Image = AlwaysEnabled || Enabled ? EnabledBitmap : DisabledBitmap;

        protected override void SetToolTipText(string value)
        {
            base.SetToolTipText(value);
            ToolTip.SetToolTip(picture, value);
        }
    }
}