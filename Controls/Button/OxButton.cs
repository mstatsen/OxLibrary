using OxLibrary.Handlers;

namespace OxLibrary.Controls;

public class OxButton : OxIconButton
{
    private readonly OxLabel Label = new()
    {
        Dock = OxDock.Left,
        TextAlign = ContentAlignment.MiddleLeft
    };

    public static readonly OxWidth DefaultWidth = OxWh.W100;
    public static readonly OxWidth DefaultHeight = OxWh.W20;

    public OxButton() : base() { }
    public OxButton(string text, Bitmap? icon) : base(icon, DefaultHeight)
    {
        Size = new(DefaultWidth, DefaultHeight);
        Text = text;
        MinimumSize = OxSize.Empty;
    }

    protected override void AfterCreated()
    {
        base.AfterCreated();
        Picture.Dock = OxDock.Left;
        Picture.Width = OxWh.W16;
        Picture.Stretch = true;
        HiddenBorder = false;
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
        Label.Parent = this;
    }

    protected override void SetHandlers()
    {
        base.SetHandlers();
        SetHoverHandlers(Label);
        SetClickHandler(Label);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        Label.ForeColor = ForeColor;
    }

    protected override string GetText() => 
        Label.Text;

    protected override void SetText(string value)
    {
        Label.Text = value;
        Label.Visible = !value.Equals(string.Empty);
        CalcLabelWidth();
        RecalcPaddings();
    }

    private void RecalcPaddings() =>
        Padding.Left =
            OxWh.Div(
                OxWh.Sub(
                    Width,
                    RealPictureWidth | RealLabelWidth),
                OxWh.W2);

    private void CalcLabelWidth()
    {
        if (Label is null)
            return;

        Label.AutoSize = true;
        OxWidth calcedLabelWidth = Label.Width;
        Label.AutoSize = false;
        calcedLabelWidth = 
            OxWh.Less(
                OxWh.Add(calcedLabelWidth, RealPictureWidth), 
                Width
            )
                ? calcedLabelWidth 
                : OxWh.Sub(Width, RealPictureWidth);
        Label.Width = OxWh.Max(calcedLabelWidth, OxWh.W0);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        CalcLabelWidth();
    }

    private OxWidth RealPictureWidth =>
        Picture.Visible
            ? Picture.Width
            : OxWh.W0;

    private OxWidth RealLabelWidth =>
        Label.Visible
            ? Label.Width
            : OxWh.W0;

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcLabelWidth();
            RecalcPaddings();

            /*
            StartSizeChanging();
            try
            {
                base.SetWidth(Width);
            }
            finally
            {
                EndSizeChanging();
            }
            */
        }

        base.OnSizeChanged(e);
    }

    /*
    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);

        if (Visible)
        {
            CalcLabelWidth();
            RecalcPaddings();
        }
    }
    */

    protected override void SetToolTipText(string value)
    {
        base.SetToolTipText(value);
        ToolTip.SetToolTip(Label, value);
    }
}