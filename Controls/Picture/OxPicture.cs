using OxLibrary.BitmapWorker;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Controls;

public partial class OxPicture : OxPanel
{
    public bool AlwaysEnabled { get; set; } = false;

    private Bitmap? enabledBitmap;
    private Bitmap? EnabledBitmap 
    {
        get => enabledBitmap;
        set => SetEnabledBitmap(value);
    }

    private bool stretch = false;
    public bool Stretch
    {
        get => stretch;
        set
        {
            stretch = value;
            picture.Height = value ? Height : pictureSize;
            CorrectPicturePosition();
        }
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

    private readonly OxPictureBox picture = new()
    { 
        Dock = OxDock.Fill
    };
    public PictureBox Picture => picture;

    public OxPicture()
    {
        BackColor = Color.Transparent;
        Width = 24;
        Height = 24;
        Padding.Size = 0;
    }

    private short pictureSize = 16;
    public short PictureSize
    {
        get => pictureSize;
        set => SetPictureSize(value);
    }

    public short PicturePadding
    {
        get => Padding.Size;
        set
        {
            Padding.Size = value;
            PictureSize = OxSh.Sub(Height, Padding.Vertical);
        }
    }

    private void SetPictureSize(short value)
    {
        if (pictureCorrectionProcess)
            return;

        if (Stretch
            || value.Equals(pictureSize))
            return;

        pictureSize = value;
        CorrectPicturePosition();
    }

    private bool pictureCorrectionProcess = false;

    private void CorrectPicturePosition()
    {
        if (pictureCorrectionProcess)
            return;
        pictureCorrectionProcess = true;
        SetImage(Image);
        picture.Left = OxSh.CenterOffset(Width, picture.Width);
        picture.Top = OxSh.CenterOffset(Height, picture.Height);
        pictureCorrectionProcess = false;
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        PreparePicture();
    }

    private void PreparePicture()
    {
        picture.Parent = this;
        picture.Click += (s, e) => InvokeOnClick(this, null);
        CorrectPicturePosition();
    }

    protected override void SetHandlers()
    {
        base.SetHandlers();
        SetHoverHandlers(picture);
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (!e.IsChanged)
            return;
        
        CorrectPicturePosition();
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        picture.BackColor = BackColor;
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

        Padding.Vertical = OxSh.Short(Stretch ? 0 : OxSh.CenterOffset(Height, pictureSize));
        Padding.Horizontal = OxSh.Short(Stretch ? 0 : OxSh.CenterOffset(Width, pictureSize));
        OxBitmapCalcer bitmapCalcer = new(value, new(pictureSize, pictureSize), Stretch);
        picture.SizeMode = bitmapCalcer.SizeMode;

        if (!Stretch
            && !picture.Size.Equals(bitmapCalcer.ImageBox.Size))
            picture.Size = new(bitmapCalcer.ImageBox.Size);

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

    public override void OnEnabledChanged(OxBoolChangedEventArgs e)
    {
        base.OnEnabledChanged(e);
        picture.Enabled = Enabled;
        SetPictureImage();
    }

    private void SetPictureImage() => 
        picture.Image =
            AlwaysEnabled
            || IsEnabled
                ? EnabledBitmap
                : DisabledBitmap;

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(picture, value);
    }
}