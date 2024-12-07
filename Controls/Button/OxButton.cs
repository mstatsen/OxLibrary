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

    private readonly OxWidth AutoSizePadding = OxWh.W4;

    public static readonly OxWidth DefaultWidth = OxWh.W100;
    public static readonly OxWidth DefaultHeight = OxWh.W24;

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
        Picture.Width = OxWh.W16;
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
            OxWidth textWidth =
                OxWh.Add(
                    OxWh.Add(
                        OxTextHelper.GetTextWidth(Text, Label.Font), 
                        OxWh.Double(AutoSizePadding)
                    ),
                    Borders.Horizontal
                );
            Width = OxWh.Add(RealPictureWidth, textWidth);
            Label.Width = textWidth;
        }

        Label.Text = Text;

        if (!AutoSize)
        {
            OxWidth calcedWidth = OxWh.A(RealPictureWidth, RealLabelWidth);

            if (OxWh.Greater(calcedWidth, Width))
            {
                Label.Width = OxWh.S(Width, RealPictureWidth);
                calcedWidth = Width;
            }

            Padding.Left = OxWh.Half(OxWh.S(Width, calcedWidth));
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalcComponents();
    }

    private OxWidth RealPictureWidth =>
        //Picture.Visible
        //    ? 
        Picture.Width
            //: OxWh.W0
            ;

    private OxWidth RealLabelWidth =>
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