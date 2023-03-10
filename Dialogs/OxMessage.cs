using System.Windows.Forms;

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

        public static void ShowInfo(string Info)
        {
            OxMessage messageForm = new()
            {
                Text = "Information",
                Message = Info,
                DialogButtons = OxDialogButton.OK
            };
            messageForm.MainPanel.BaseColor = new OxColorHelper(messageForm.MainPanel.BaseColor).Bluer(2);
            messageForm.ShowDialog();
            messageForm.Dispose();
        }

        public static void ShowError(string Error)
        {
            OxMessage messageForm = new()
            {
                Text = "Error",
                Message = Error,
                DialogButtons = OxDialogButton.OK
            };
            messageForm.MainPanel.BaseColor = new OxColorHelper(messageForm.MainPanel.BaseColor).Redder(2);
            messageForm.ShowDialog();
            messageForm.Dispose();
        }

        public static DialogResult ShowWarning(string Warning)
        {
            OxMessage messageForm = new()
            {
                Text = "Warning",
                Message = Warning,
                DialogButtons = OxDialogButton.Yes | OxDialogButton.No | OxDialogButton.Cancel
            };

            messageForm.MainPanel.BaseColor = new OxColorHelper(messageForm.MainPanel.BaseColor).Browner(2);
            return messageForm.ShowDialog();
        }

        public static DialogResult ShowConfirm(string Confirm)
        {
            OxMessage messageForm = new()
            {
                Text = "Warning",
                Message = Confirm,
                DialogButtons = OxDialogButton.Yes | OxDialogButton.No
            };

            messageForm.MainPanel.BaseColor = new OxColorHelper(messageForm.MainPanel.BaseColor).Browner(2);
            return messageForm.ShowDialog();
        }

        public static bool Confirmation(string Confirm) =>
            ShowConfirm(Confirm) == DialogResult.Yes;
    }
}