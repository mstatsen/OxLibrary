using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public partial class OxPicture : OxPane
    {
        private class OxPictureBox : PictureBox
        {
            public OxPictureBox() => DoubleBuffered = true;
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
            get => picture.Dock is DockStyle.Fill;
            set => 
                picture.Dock = 
                    value 
                        ? DockStyle.Fill 
                        : DockStyle.None;
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
            get => OxWh.W(picture.Height);
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
            
            picture.Width = (int)value;
            picture.Height = (int)value;
            CorrectPicturePosition();
        }

        private void CorrectPicturePosition()
        {
            if (Stretch)
                return;

            picturePadding = OxWh.Div(OxWh.Sub(Height, picture.Height), OxWh.W2);

            if (picturePadding < 0)
                picturePadding = 0;

            picture.Left = OxWh.Int(OxWh.Div(OxWh.Sub(Width, picture.Width), OxWh.W2));
            picture.Top = OxWh.Int(OxWh.Div(OxWh.Sub(Height, picture.Height), OxWh.W2));
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            PreparePicture();
        }

        private void PreparePicture()
        {
            picture.Parent = this;
            picture.Dock = DockStyle.None;
            picture.Click += (s, e) => InvokeOnClick(this, null);
            SetPictureSize(Height);
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            picture.SizeChanged += PictureSizeChanged;
            SetHoverHandlers(picture);
        }

        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
                CorrectPicturePosition();

            return e.Changed;
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            picture.BackColor = Color.Transparent;
        }

        private void PictureSizeChanged(object? sender, EventArgs e)
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

            OxBitmapCalcer bitmapCalcer = new(value, picture.Size, Stretch);
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