using OxLibrary.Controls;
using OxLibrary.Geometry;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public class OxMessagePanel : OxDialogPanel
{
    private readonly OxTextBox MessageBox = new()
    {
        Dock = OxDock.Top,
        TextAlign = HorizontalAlignment.Center,
        Font = OxStyles.Font(OxStyles.DefaultFontSize + 1.15f),
        ForeColor = Color.FromArgb(60, 55, 54),
        BorderStyle = BorderStyle.None,
        WordWrap = true,
        Multiline = true,
    };

    public override void PrepareColors()
    {
        base.PrepareColors();
        MessageBox.BackColor = BackColor;
    }

    public OxMessagePanel() : base()
    {
        FooterButtonsAlign = HorizontalAlign.Center;
        Size = new(240, 120);
        HeaderHeight = 30;
        Footer.ButtonsPlacing += FooterButtonsPlacingHandler;
    }

    private void FooterButtonsPlacingHandler(object? sender, EventArgs e)
    {
        Size = new(
            OxSH.Max(Footer.ButtonsWidth + 160, Width),
            OxSH.Max(Footer.ButtonsWidth / 2, Height)
        );
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        MessageBox.Parent = this;
        Padding.Horizontal = OxSH.Div(Width, 4);
    }

    public override Color DefaultColor => Color.FromArgb(146, 141, 140);

    public string Message
    {
        get => MessageBox.Text;
        set
        {
            MessageBox.Text = value;
            short calcedMessageHeight = OxTextHelper.CalcedHeight(MessageBox, MessageBox.Width);
            Padding.Top = OxSH.Min(36, OxSH.Div(calcedMessageHeight, 4));
            Padding.Bottom = OxSH.Min(36, OxSH.Div(calcedMessageHeight, 4));
            Size = new(
                Width,
                HeaderHeight
                 + Padding.Vertical
                 + calcedMessageHeight
                 + Footer.Height
            );

            MessageBox.Height = calcedMessageHeight;
        }
    }

    protected override short FooterButtonHeight => 34;
}