using OxLibrary.Geometry;
using OxLibrary.Handlers;

namespace OxLibrary.Controls;

public class OxButton : OxIconButton
{
    private readonly OxLabel Label = new()
    {
        Dock = OxDock.Left,
        TextAlign = ContentAlignment.MiddleLeft,
        AutoSize = OxB.F,
        CutByParentWidth = OxB.T
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

    public override void OnAutoSizeChanged(OxBoolChangedEventArgs e)
    {
        base.OnAutoSizeChanged(e);
        Label.CutByParentWidth = OxB.Not(AutoSize);
        RecalcComponents();
    }

    protected override void SetIcon(Bitmap? value)
    {
        base.SetIcon(value);
        Picture.SetVisible(value is not null);
        Label.TextAlign = 
            Picture.IsVisible
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
        Label.SetVisible(value != string.Empty);
        RecalcComponents();
    }

    private void RecalcComponents()
    {
        if (IsAutoSize)
        {
            Padding.Left = AutoSizePadding;
            short textWidth = OxTextHelper.Width(Text, Label.Font);
            textWidth += OxSh.X2(AutoSizePadding);
            textWidth += Borders.Horizontal;
            Width = OxSh.Add(RealPictureWidth, textWidth);
            Label.Width = textWidth;
        }

        Label.Text = Text;

        if (!IsAutoSize)
        {
            short calcedWidth = OxSh.Add(RealPictureWidth, RealLabelWidth);

            if (calcedWidth > Width)
            {
                Label.Width = OxSh.Sub(Width, RealPictureWidth);
                calcedWidth = Width;
            }

            Padding.Left = OxSh.CenterOffset(Width, calcedWidth);
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

        if (!e.IsChanged)
            return;

        RecalcComponents();
    }

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(Label, value);
    }
}