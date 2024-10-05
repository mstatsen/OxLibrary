namespace OxLibrary.Dialogs
{
    public class OxMessage : OxDialog
    {
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

        public static void ShowInfo(string Info, Control owner)
        {
            OxMessage messageForm = new()
            {
                Text = "Information",
                Message = Info,
                DialogButtons = OxDialogButton.OK
            };

            messageForm.MainPanel.BaseColor = messageForm.MainPanel.Colors.Bluer(2);
            messageForm.ShowDialog(owner);
            messageForm.Dispose();
        }

        public static void ShowError(string Error, Control owner, OxDialogButton buttons = OxDialogButton.OK)
        {
            OxMessage messageForm = new()
            {
                Text = "Error",
                Message = Error,
                DialogButtons = buttons
            };
            messageForm.MainPanel.BaseColor = messageForm.MainPanel.Colors.Redder(2);
            messageForm.ShowDialog(owner);
            messageForm.Dispose();
        }

        public static DialogResult ShowWarning(string Warning, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No | OxDialogButton.Cancel)
        {
            OxMessage messageForm = new()
            {
                Text = "Warning",
                Message = Warning,
                DialogButtons = buttons
            };

            messageForm.MainPanel.BaseColor = messageForm.MainPanel.Colors.Browner(2);
            return messageForm.ShowDialog(owner);
        }

        public static DialogResult ShowConfirm(string Confirm, Control owner, 
            OxDialogButton buttons = OxDialogButton.Yes | OxDialogButton.No)
        {
            OxMessage messageForm = new()
            {
                Text = "Warning",
                Message = Confirm,
                DialogButtons = buttons
            };

            messageForm.MainPanel.BaseColor = messageForm.MainPanel.Colors.Browner(2);
            return messageForm.ShowDialog(owner);
        }

        public static bool Confirmation(string Confirm, Control owner) =>
            ShowConfirm(Confirm, owner) == DialogResult.Yes;
    }
}