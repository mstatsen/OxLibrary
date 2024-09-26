using OxLibrary.Controls;

namespace OxLibrary.Dialogs
{
    public class OxMessageMainPanel : OxDialogMainPanel
    {
        private readonly OxLabel label = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false,
            Font = new Font(Styles.FontFamily, Styles.DefaultFontSize + 1.15f, FontStyle.Regular),
            ForeColor = Color.FromArgb(60, 55, 54)
        };

        public OxMessageMainPanel(OxForm form) : base(form)
        {
            Paddings.SetSize(24);
            SetContentSize(240, 120);
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            label.Parent = ContentContainer;
        }

        public override Color DefaultColor => Color.FromArgb(146, 141, 140);

        public string Message
        {
            get => label.Text;
            set => label.Text = value;
        }

        protected override void PlaceButtons(int rightPosition = -1)
        {
            if (Form != null)
            {
                int calcedWidth = 0;

                foreach (OxDialogButton button in buttonsDictionary.Keys)
                    if ((DialogButtons & button) == button)
                        calcedWidth += OxDialogButtonsHelper.Width(button) + DialogButtonSpace;

                SetContentSize(
                    Math.Max(calcedWidth + 160, SavedWidth),
                    Math.Max(calcedWidth / 2, SavedHeight));

                rightPosition = (Width - calcedWidth) / 2 - DialogButtonSpace;
            }

            base.PlaceButtons(rightPosition);
        }
    }
}