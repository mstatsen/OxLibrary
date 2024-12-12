using OxLibrary.Controls;
using OxLibrary.Geometry;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public class OxMessagePanel : OxDialogPanel
{
    private readonly OxTextBox MessageBox = new()
    {
        Dock = OxDock.Fill,
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
        HeaderHeight = 30;
        MinimumSize = new(384, 126);
    }

    private void RecalcSize()
    {
        short calcedWidth = OxSH.Max(Footer.ButtonsWidth, OxSH.Min(OxTextHelper.CalcedWidth(MessageBox), 640));
        short calcedMessageHeight = OxTextHelper.CalcedHeight(MessageBox, calcedWidth);
        Padding.Horizontal = OxSH.Min(24, OxSH.Div(calcedWidth, 4));
        calcedWidth += Padding.Horizontal;
        Padding.Vertical = OxSH.Min(24, OxSH.Div(calcedMessageHeight, 4));
        short calcedHeight =
            OxSH.Add(
                HeaderHeight,
                Padding.Vertical,
                calcedMessageHeight,
                Footer.Height
            );

        if (Width.Equals(calcedWidth)
            && Height.Equals(calcedHeight))
            return;

        Size = new(
            calcedWidth,
            calcedHeight
        );
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        MessageBox.Parent = this;
    }

    public override Color DefaultColor => Color.FromArgb(146, 141, 140);

    public string Message
    {
        get => MessageBox.Text;
        set
        {
            MessageBox.Text = value;
            MessageBox.SelectedText = string.Empty;
            RecalcSize();
        }
    }

    protected override short FooterButtonHeight => 34;
}