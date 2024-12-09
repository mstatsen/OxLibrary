using OxLibrary.Handlers;

namespace OxLibrary.Controls;

public class OxButton : OxIconButton
{
    private readonly OxLabel Label = new()
    {
        Dock = OxDock.Left,
        TextAlign = ContentAlignment.MiddleLeft,
        AutoSize = false,
        CutByParentWidth = true
    };

    private readonly short AutoSizePadding = 4;

    public static readonly short DefaultWidth = 100;
    public static readonly short DefaultHeight = 24;

    public OxButton() : base() { }
    public OxButton(string text, Bitmap? icon) : base(icon, DefaultHeight)
    {
        Size = new(DefaultWidth, DefaultHeight);
        Text = text;
        MinimumSize = OxSize.Empty;
    }

    protected override void OnAutoSizeChanged(OxEventArgs e)
    {
        base.OnAutoSizeChanged(e);
        Label.CutByParentWidth = !AutoSize;
        RecalcComponents();
    }

    protected override void SetIcon(Bitmap? value)
    {
        base.SetIcon(value);
        Picture.Visible = value is not null;
        Label.TextAlign = 
            Picture.Visible
                ? ContentAlignment.MiddleLeft 
                : ContentAlignment.MiddleCenter;
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        Picture.Width = 16;
        Picture.Dock = OxDock.Left;
        Picture.Stretch = true;
        Label.Parent = this;
        HiddenBorder = false;
    }

    protected override void SetHandlers()
    {
        base.SetHandlers();
        SetHoverHandlers(Label);
        SetClickHandler(Label);
    }


    protected override void SetText(string value)
    {
        base.SetText(value);
        Label.Visible = value != string.Empty;
        RecalcComponents();
    }

    private void RecalcComponents()
    {
        if (AutoSize)
        {
            Padding.Left = AutoSizePadding;
            short textWidth = (short)(OxTextHelper.GetTextWidth(Text, Label.Font) + AutoSizePadding * 2 + Borders.Horizontal);
            Width = (short)(RealPictureWidth + textWidth);
            Label.Width = textWidth;
        }

        Label.Text = Text;

        if (!AutoSize)
        {
            short calcedWidth = (short)(RealPictureWidth + RealLabelWidth);

            if (calcedWidth > Width)
            {
                Label.Width = (short)(Width - RealPictureWidth);
                calcedWidth = Width;
            }

            Padding.Left = (short)((Width - calcedWidth) / 2);
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalcComponents();
    }

    private short RealPictureWidth =>
        //Picture.Visible
        //    ? 
        Picture.Width
            //: 0
            ;

    private short RealLabelWidth =>
        Label.Width;

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (e.Changed)
            RecalcComponents();
    }

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(Label, value);
    }
}