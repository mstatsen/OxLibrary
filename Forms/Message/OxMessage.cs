namespace OxLibrary.Forms;

public class OxMessage : OxDialog<OxMessagePanel>
{
    private readonly OxMessageType MessageType;
    public OxMessage(OxMessageType messageType) : base()
    {
        MessageType = messageType;
        Text = OxMessageTypeHelper.Caption(messageType);
        BaseColor = OxMessageTypeHelper.BaseColor(messageType);
        ApplyFormIcon();
    }

    public string Message
    {
        get => FormPanel.Message;
        set => FormPanel.Message = value;
    }

    public override Bitmap FormIcon =>
        OxMessageTypeHelper.Icon(MessageType);

    private static Dictionary<OxMessageType, OxMessage> messageForms = new();

    private static DialogResult ShowMessage(OxMessageType messageType,
        string message, Control owner, OxDialogButton buttons)
    {
        if (!messageForms.TryGetValue(messageType, out OxMessage? messageForm))
            messageForms.Add(messageType, new(messageType));

        messageForm = messageForms[messageType];
        messageForm.Message = message;
        messageForm.DialogButtons = buttons;
        return messageForm.ShowDialog(owner);
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

        if (e.Handled)
            return;

        if (MessageType is OxMessageType.Error
                        or OxMessageType.Info
            && e.KeyCode is Keys.Enter
                         or Keys.Space)
            DialogResult = DialogResult.OK;
    }
}