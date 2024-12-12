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
        Padding.Size = 24;
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
    }

    public override Color DefaultColor => Color.FromArgb(146, 141, 140);

    public string Message
    {
        get => MessageBox.Text;
        set
        {
            //TODO: Use OxText helper for calc size (see OxLabel)
            MessageBox.Text = value;
            MessageBox.Height =
                OxSH.Add(
                    OxSH.Max(OxSH.Half(value.Length), 23),
                    23 * value.Count(c => c.Equals('\r'))
            );
            Size = new(
                500,
                MessageBox.Bottom + Footer.Height
            );
        }
    }

    protected override short FooterButtonHeight => 34;
}