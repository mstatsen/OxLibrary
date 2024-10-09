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
            Paddings.SetSize(24);
            SetContentSize(240, 120);
            Header.SetContentSize(Header.SavedWidth, 30);
        }

        protected override HorizontalAlign FooterButtonsAlign => HorizontalAlign.Center;

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            MessageBox.Parent = ContentContainer;
        }

        public override Color DefaultColor => Color.FromArgb(146, 141, 140);

        public string Message
        {
            get => MessageBox.Text;
            set
            {
                MessageBox.Text = value;
                MessageBox.Height = Math.Max(value.Length / 2, 23) 
                    + 23 * value.Count(c => c =='\r');
                SetContentSize(240, MessageBox.Bottom + Paddings.Bottom);
            }
        }

        protected override void PlaceButtons()
        {
            if (Form != null)
            {
                int calcedWidth = 0;

                foreach (OxDialogButton button in buttonsDictionary.Keys)
                    if ((DialogButtons & button) == button)
                        calcedWidth += OxDialogButtonsHelper.Width(button) + DialogButtonSpace;

                SetContentSize(
                    Math.Max(calcedWidth + 160, SavedWidth),
                    Math.Max(calcedWidth / 2, SavedHeight)
                );
            }

            base.PlaceButtons();
        }

        protected override int FooterButtonHeight => 34;
    }
}