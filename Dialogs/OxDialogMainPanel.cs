using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Dialogs
{
    public class OxDialogMainPanel : OxFormMainPanel
    {
        public const int DialogButtonHeight = 32;
        public int DialogButtonSpace = 6;
        public int DialogButtonStartSpace = 10;
        protected readonly Dictionary<OxDialogButton, OxButton> buttonsDictionary = new();
        public void SetButtonText(OxDialogButton dialogButton, string text)
        {
            if (buttonsDictionary.TryGetValue(dialogButton, out var button))
                button.Text = text;
        }

        private OxDialogButton dialogButtons = OxDialogButton.OK | OxDialogButton.Cancel;

        public new OxDialog Form
        {
            get => (OxDialog)base.Form;
            set => base.Form = value;
        }

        public OxDialogButton DialogButtons
        {
            get => dialogButtons;
            set
            {
                dialogButtons = value;

                foreach (KeyValuePair<OxDialogButton, OxButton> item in buttonsDictionary)
                    item.Value.Visible = (dialogButtons & item.Key) == item.Key;

                PlaceButtons();
            }
        }

        public OxDialogMainPanel(OxForm form) : base(form) =>
            SetContentSize(480, 360);

        public readonly OxFrame Footer = new(new Size(300, 42));

        public DialogResult DialogResult { get; private set; }

        protected override void PrepareInnerControls()
        {
            PrepareFooter();
            base.PrepareInnerControls();
        }

        private void CreateButton(OxDialogButton dialogButton)
        {
            OxButton button = new(
                OxDialogButtonsHelper.Name(dialogButton),
                OxDialogButtonsHelper.Icon(dialogButton)
            )
            {
                Parent = Footer,
                Top = (Footer.Height - DialogButtonHeight) / 2,
                Font = new Font(Styles.FontFamily, Styles.DefaultFontSize + 0.5f, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                Visible = (dialogButtons & dialogButton) == dialogButton
            };

            button.SetContentSize(OxDialogButtonsHelper.Width(dialogButton), DialogButtonHeight - 2);
            button.Click += DialogButtonClickHandler;
            buttonsDictionary.Add(dialogButton, button);
        }

        private void PrepareFooter()
        {
            Footer.Dock = DockStyle.Bottom;
            Footer.Parent = this;
            Footer.BlurredBorder = true;

            foreach (OxDialogButton button in OxDialogButtonsHelper.List())
                CreateButton(button);

            PlaceButtons();
            Footer.Borders[OxDock.Left].Visible = false;
            Footer.Borders[OxDock.Right].Visible = false;
            Footer.Borders[OxDock.Bottom].Visible = false;
        }

        private void DialogButtonClickHandler(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            OxButton button = (OxButton)sender;
            OxDialogButton dialogButton = OxDialogButton.OK;

            foreach (KeyValuePair<OxDialogButton, OxButton> item in buttonsDictionary)
                if (item.Value == button)
                    dialogButton = item.Key;

            Form.DialogResult = OxDialogButtonsHelper.Result(dialogButton);
        }

        protected virtual void PlaceButtons(int rightOffset = 0)
        {
            rightOffset = Footer.Width - rightOffset - DialogButtonStartSpace;

            foreach (KeyValuePair<OxDialogButton, OxButton> item in buttonsDictionary)
                if ((dialogButtons & item.Key) == item.Key)
                {
                    item.Value.Left = rightOffset - OxDialogButtonsHelper.Width(item.Key);
                    item.Value.Width = OxDialogButtonsHelper.Width(item.Key);
                    rightOffset = item.Value.Left - DialogButtonSpace;
                }
        }

        protected override int GetCalcedHeight() =>
            base.GetCalcedHeight() + Footer.CalcedHeight;

        protected override void PrepareColors()
        {
            base.PrepareColors();
            Footer.BaseColor = BaseColor;

            foreach (KeyValuePair<OxDialogButton, OxButton> item in buttonsDictionary)
                item.Value.BaseColor = BaseColor;
        }

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            Footer.ReAlign();
            Header.ReAlign();
            Borders.ReAlign();
            Margins.ReAlign();
            SendToBack();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Form.ClearConstraints();
            base.OnSizeChanged(e);
            PlaceButtons();
            Form.FreezeSize();
        }

        public override void SetContentSize(int width, int height)
        {
            base.SetContentSize(width, height);
            Form.ClientSize = new Size(Width, Height);
        }
    }
}