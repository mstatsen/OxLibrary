using OxLibrary.Panels;

namespace OxLibrary.Forms;

public delegate string GetEmptyMandatoryFieldName();

public class OxDialog<TDialog, TDialogPanel> : OxForm<TDialog, TDialogPanel>

    where TDialog : OxDialog<TDialog, TDialogPanel>
    where TDialogPanel : OxDialogPanel<TDialog, TDialogPanel>, new()
{
    public OxDialog() : base()
    {
        Sizable = false;
        CanMaximize = false;
        CanMinimize = false;
        ShowInTaskbar = false;
    }

    protected override void SetUpForm()
    {
        base.SetUpForm();
        MinimizeBox = false;
        MaximizeBox = false;
        KeyPreview = true;
    }

    public OxDialogButton DialogButtons
    {
        get => FormPanel.DialogButtons;
        set => FormPanel.DialogButtons = value;
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (!e.Handled &&
            e.KeyCode is Keys.Escape)
            DialogResult = DialogResult.Cancel;
    }

    public void SetKeyUpHandler(Control control) =>
        control.KeyUp += DialogControlKeyUpHandler;

    private void DialogControlKeyUpHandler(object? sender, KeyEventArgs e)
    {
        if (!e.Handled)
        {
            if (e.KeyCode is Keys.Enter)
                DialogResult = DialogResult.OK;
            else
            if (e.KeyCode is Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }
    }

    public Control? FirstFocusControl { get; set; }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        e.Cancel = DialogResult switch
        {
            DialogResult.OK or
            DialogResult.Yes or
            DialogResult.Continue =>
                !CanOKClose(),
            DialogResult.Cancel =>
                !CanCancelClose(),
            _ =>
                false,
        };
    }

    public GetEmptyMandatoryFieldName? GetEmptyMandatoryFieldName;

    private bool CheckMandatoryFields()
    {
        string? emptyMandatoryField = GetEmptyMandatoryFieldName?.Invoke();
        emptyMandatoryField ??= EmptyMandatoryField();

        if (emptyMandatoryField.Equals(string.Empty))
            return true;

        OxMessage.ShowError($"{emptyMandatoryField} is mandatory", this);
        return false;
    }

    protected virtual string EmptyMandatoryField() =>
        string.Empty;

    public virtual bool CanOKClose() =>
        CheckMandatoryFields();

    public virtual bool CanCancelClose() => true;

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        FirstFocusControl?.Focus();
    }

    public OxWidth DialogButtonSpace
    { 
        get => FormPanel.DialogButtonSpace;
        set => FormPanel.DialogButtonSpace = value;
    }

    public OxWidth DialogButtonStartSpace
    {
        get => FormPanel.DialogButtonStartSpace;
        set => FormPanel.DialogButtonStartSpace = value;
    }

    public void SetFooterButtonText(OxDialogButton dialogButton, string text) =>
        FormPanel.SetFooterButtonText(dialogButton, text);

    public OxFrame Footer => FormPanel.Footer;
}

public class OxDialog : OxDialog<OxDialog, OxDialogPanel>
{ 
}