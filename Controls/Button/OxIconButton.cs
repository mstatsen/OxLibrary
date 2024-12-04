using OxLibrary.Panels;

namespace OxLibrary.Controls;

public class OxIconButton : OxClickFrame
{
    public readonly OxPicture Picture = 
        new()
        {
            Dock = OxDock.Fill
        };

    public OxIconButton() : base() { }
    public OxIconButton(Bitmap? icon, OxWidth Size) : base(new(Size, Size))
    {
        Icon = icon;
    }

    protected override void PrepareInnerComponents()
    {
        Picture.Parent = this;
        Picture.UseDisabledStyles = UseDisabledStyles;
        base.PrepareInnerComponents();
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        Picture.BaseColor = BaseColor;
    }

    protected override Bitmap? GetIcon() => (Bitmap?)Picture.Image;

    protected override void SetIcon(Bitmap? value) => 
        Picture.Image = value;

    public OxWidth IconPadding
    {
        get => Picture.PicturePadding;
        set => Picture.PicturePadding = value;
    }

    protected override void SetUseDisabledStyles(bool value)
    {
        base.SetUseDisabledStyles(value);
        Picture.UseDisabledStyles = UseDisabledStyles;
    }

    protected override void SetHandlers()
    {
        base.SetHandlers();
        SetHoverHandlers(Picture);
        SetClickHandler(Picture);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Picture.Enabled = Enabled;
        PrepareColors();
    }

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(Picture, value);
        ToolTip.SetToolTip(Picture.Picture, value);
    }
}