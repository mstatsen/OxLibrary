namespace OxLibrary.Forms
{
    public class OxMessage : OxDialog
    {
        private readonly OxMessageType MessageType;
        public OxMessage(OxMessageType messageType) : base()
        { 
            MessageType = messageType;
            Text = OxMessageTypeHelper.Caption(messageType);
            MainPanel.BaseColor = OxMessageTypeHelper.BaseColor(messageType);
            MainPanel.SetIcon();
        }

        protected override OxFormMainPanel CreateMainPanel() =>
            new OxMessageMainPanel(this);

        public new OxMessageMainPanel MainPanel
        {
            get => (OxMessageMainPanel)base.MainPanel;
            set => base.MainPanel = value;
        }
        public string Message
        {
            get => MainPanel.Message;
            set => MainPanel.Message = value;
        }

        public override Bitmap FormIcon =>
            OxMessageTypeHelper.Icon(MessageType);

        private static DialogResult ShowMessage(OxMessageType messageType,
            string message, Control owner, OxDialogButton buttons)
        {
            OxMessage messageForm = new(messageType)
            {
                Message = message,
                DialogButtons = buttons
            };

            try
            {
                return messageForm.ShowDialog(owner);
            }
            finally
            {
                messageForm.Dispose();
            }
        }

        public static void ShowInfo(string Info, Control owner) =>
            ShowMessage(
                OxMessageType.Info,
                Info,
                owner,
                OxDialogButton.OK
            );

        public static DialogResult ShowError(string Error, Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
            ShowMessage(
                OxMessageType.Error,
                Error,
                owner,
                buttons
            );

        public static DialogResult ShowWarning(string Warning, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No | OxDialogButton.Cancel) =>
            ShowMessage(
                OxMessageType.Warning,
                Warning,
                owner,
                buttons
            );

        public static DialogResult ShowConfirm(string Confirm, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No) =>
            ShowMessage(
                OxMessageType.Confirmation,
                Confirm,
                owner,
                buttons
            );

        public static bool Confirmation(string Confirm, Control owner) =>
            ShowConfirm(Confirm, owner) is DialogResult.Yes;

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!e.Handled
                && (MessageType is OxMessageType.Error 
                                or OxMessageType.Info)
                && (e.KeyCode is Keys.Enter 
                              or Keys.Space))
                DialogResult = DialogResult.OK;
        }
    }
}