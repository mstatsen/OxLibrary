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
        ReadOnly = true
    };

    public override void PrepareColors()
    {
        base.PrepareColors();
        MessageBox.BackColor = BackColor;
    }

    public OxMessagePanel() : base()
    {
        MinimumSize = new(384, 126);
    }

    protected override void OnFormShown(EventArgs e)
    {
        base.OnFormShown(e);
        MessageBox.SelectionStart = 0;
        MessageBox.SelectionLength = 0;
    }

    private readonly short MinPadding = 20;

    private void RecalcSize()
    {
        short calcedWidth = OxSH.Max(
            Footer.ButtonsWidth, 
            OxSH.Min(OxTextHelper.CalcedWidth(MessageBox), 768)
        );
        short calcedMessageHeight = OxTextHelper.CalcedHeight(MessageBox, calcedWidth);
        Padding.Horizontal = MinPadding;
        calcedWidth += Padding.Horizontal;
        Padding.Vertical = MinPadding;
        short calcedHeight =
            OxSH.Min(
                OxSH.Add(
                    HeaderHeight,
                    Padding.Vertical,
                    calcedMessageHeight,
                    Footer.Height
                ),
                432
            );

        if (Width.Equals(calcedWidth)
            && Height.Equals(calcedHeight))
            return;

        Size = new(
            calcedWidth,
            calcedHeight
        );

        MessageBox.ScrollBars =
            calcedMessageHeight > MessageBox.Height
                ? ScrollBars.Vertical
                : MessageBox.ScrollBars = ScrollBars.None;
    }

    protected override short HeaderHeight => 30;

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        FooterHeight = 34;
        FooterButtonsAlign = HorizontalAlign.Center;
        MessageBox.Parent = this;
    }

    public override Color DefaultColor => Color.FromArgb(146, 141, 140);

    public string Message
    {
        get => MessageBox.Text;
        set
        {
            value = value.Replace("\n", "\r\n");

            if (MessageBox.Text.Equals(value))
                return;

            MessageBox.Text = value;
            RecalcSize();
        }
    }
}