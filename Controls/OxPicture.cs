using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public partial class OxPicture : OxPane
    {
        private class OxPictureBox : PictureBox
        {
            public OxPictureBox() => DoubleBuffered = true;
        }

        private Bitmap? enabledBitmap;
        private Bitmap? EnabledBitmap 
        {
            get => enabledBitmap;
            set => SetEnabledBitmap(value);
        }

        public bool Stretch 
        { 
            get => picture.Dock == DockStyle.Fill;
            set => picture.Dock = value ? DockStyle.Fill : DockStyle.None;
        }

        private void SetEnabledBitmap(Bitmap? value)
        {
            enabledBitmap = value;
            DisabledBitmap = value == null ? null : GetGrayScale(EnabledBitmap);
        }

        private Bitmap? DisabledBitmap;

        private readonly OxPictureBox picture = new();

        public OxPicture()
        {
            BackColor = Color.Transparent;
            Width = 24;
            Height = 24;
            PictureSize = 16;
        }

        public int PictureSize
        {
            get => picture.Height;
            set => SetPictureSize(value);
        }

        private int picturePadding = 0;

        public int PicturePadding
        {
            get => picturePadding;
            set => SetPicturePadding(value);
        }

        private void SetPicturePadding(int value)
        {
            picturePadding = value;
            PictureSize = Height - picturePadding * 2;
        }

        private void SetPictureSize(int value)
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

            picturePadding = (Height - picture.Height) / 2;

            if (picturePadding < 0)
                picturePadding = 0;

            picture.Left = (Width - picture.Width) / 2;
            picture.Top = (Height - picture.Height) / 2;
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CorrectPicturePosition();
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
            if (value == null)
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
            if (bitmap == null)
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

        protected override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            picture.Enabled = value;
            SetPictureImage();
        }

        private void SetPictureImage() => 
            picture.Image = Enabled ? EnabledBitmap : DisabledBitmap;
    }
}