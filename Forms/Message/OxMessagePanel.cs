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
        Padding.Size = 24;
        Size = new(240, 120);
        HeaderHeight = 30;
    }

    protected override HorizontalAlign FooterButtonsAlign => HorizontalAlign.Center;

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
            MessageBox.Height =
                OxSH.Add(
                    OxSH.Max(value.Length / 2, 23),
                    23 * value.Count(c => c.Equals('\r')
                )
            );
            Size = new(
                240,
                OxSH.Add(MessageBox.Bottom, Padding.Bottom)
            );
        }
    }

    protected override void PlaceButtons()
    {
        if (Form is not null)
        {
            short calcedWidth = 0;

            foreach (OxDialogButton button in buttonsDictionary.Keys)
                if ((DialogButtons & button).Equals(button))
                    calcedWidth += OxSH.Add(OxDialogButtonHelper.Width(button), DialogButtonSpace);

            Size = new(
                OxSH.Max(calcedWidth + 160, Width),
                OxSH.Max(calcedWidth / 2, Height)
            );
        }

        base.PlaceButtons();
    }

    protected override short FooterButtonHeight => 34;
}