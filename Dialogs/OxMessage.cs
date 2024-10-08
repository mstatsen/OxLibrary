namespace OxLibrary.Dialogs
{
    public class OxMessage : OxDialog
    {
        private readonly MessageType MessageType;
        public OxMessage(MessageType messageType) : base()
        { 
            MessageType = messageType;
            Text = MessageTypeHelper.Caption(messageType);
            MainPanel.BaseColor = MessageTypeHelper.BaseColor(messageType);
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
            MessageTypeHelper.Icon(MessageType);

        private static DialogResult ShowMessage(MessageType messageType,
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
                MessageType.Info,
                Info,
                owner,
                OxDialogButton.OK
            );

        public static DialogResult ShowError(string Error, Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
            ShowMessage(
                MessageType.Error,
                Error,
                owner,
                buttons
            );

        public static DialogResult ShowWarning(string Warning, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No | OxDialogButton.Cancel) =>
            ShowMessage(
                MessageType.Warning,
                Warning,
                owner,
                buttons
            );

        public static DialogResult ShowConfirm(string Confirm, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No) =>
            ShowMessage(
                MessageType.Confirmation,
                Confirm,
                owner,
                buttons
            );

        public static bool Confirmation(string Confirm, Control owner) =>
            ShowConfirm(Confirm, owner) == DialogResult.Yes;
    }
}