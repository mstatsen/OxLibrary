using OxLibrary.Controls;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Dialogs
{
    public class OxMessageMainPanel : OxDialogMainPanel
    {
        private readonly OxTextBox MessageBox = new()
        {
            Dock = DockStyle.Top,
            TextAlign = HorizontalAlignment.Center,
            Font = Styles.Font(Styles.DefaultFontSize + 1.15f),
            ForeColor = Color.FromArgb(60, 55, 54),
            BorderStyle = BorderStyle.None,
            WordWrap = true,
            Multiline = true,
        };

        protected override void PrepareColors()
        {
            base.PrepareColors();
            MessageBox.BackColor = BackColor;
        }

        public OxMessageMainPanel(OxForm form) : base(form)
        {
            Padding.Size = OxWh.W24;
            Size = new(OxWh.W240, OxWh.W120);
            HeaderHeight = OxWh.W30;
        }

        protected override HorizontalAlign FooterButtonsAlign => HorizontalAlign.Center;

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            MessageBox.Parent = this;
        }

        public override Color DefaultColor => Color.FromArgb(146, 141, 140);

        public string Message
        {
            get => MessageBox.Text;
            set
            {
                MessageBox.Text = value;
                MessageBox.Height = Math.Max(value.Length / 2, 23) 
                    + 23 * value.Count(c => c.Equals('\r'));
                Size = new(
                    OxWh.W240, 
                    OxWh.Add(MessageBox.Bottom, Padding.Bottom)
                );
            }
        }

        protected override void PlaceButtons()
        {
            if (Form is not null)
            {
                OxWidth calcedWidth = OxWh.W0;

                foreach (OxDialogButton button in buttonsDictionary.Keys)
                    if ((DialogButtons & button).Equals(button))
                        calcedWidth |= OxDialogButtonsHelper.Width(button) | DialogButtonSpace;

                Size = new(
                    OxWh.Max(calcedWidth | OxWh.W160, Width),
                    OxWh.Max(OxWh.Div(calcedWidth, OxWh.W2), Height)
                );
            }

            base.PlaceButtons();
        }

        protected override OxWidth FooterButtonHeight => OxWh.W34;
    }
}