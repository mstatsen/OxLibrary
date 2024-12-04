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
        Width = OxWh.W24;
        Height = OxWh.W24;
        //Padding.SizeChanged += PaddingSizeChangedHandler;
        Padding.Size = OxWh.W0;
    }

    private OxWidth pictureSize = OxWh.W16;
    public OxWidth PictureSize
    {
        get => pictureSize;
        set => SetPictureSize(value);
    }

    public OxWidth PicturePadding
    {
        get => Padding.Size;
        set
        {
            Padding.Size = value;
            PictureSize = OxWh.Sub(Height, Padding.Vertical);
        }
    }

    private void SetPictureSize(OxWidth value)
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
        Padding.Vertical = Stretch
            ? OxWh.W0
            : OxWh.Div(OxWh.Sub(Height, pictureSize), OxWh.W2);
        Padding.Horizontal = Stretch
            ? OxWh.W0
            : OxWh.Div(OxWh.Sub(Width, pictureSize), OxWh.W2);

        SetImage(Image);
        picture.Left = OxWh.Div(OxWh.Sub(Width, picture.Width), OxWh.W2);
        picture.Top = OxWh.Div(OxWh.Sub(Height, picture.Height), OxWh.W2);
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
        //SetPictureSize(Height);
    }

    protected override void SetHandlers()
    {
        base.SetHandlers();
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

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        picture.Enabled = Enabled;
        SetPictureImage();
    }

    private void SetPictureImage() => 
        picture.Image =
            AlwaysEnabled
            || Enabled
                ? EnabledBitmap
                : DisabledBitmap;

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(picture, value);
    }
}