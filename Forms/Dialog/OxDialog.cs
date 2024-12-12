using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public delegate string GetEmptyMandatoryFieldName();

public class OxDialog<TDialogPanel> : OxForm<TDialogPanel>

    where TDialogPanel : OxDialogPanel, new()
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

    public void SetKeyUpHandler(IOxControl control) =>
        control.KeyUp += DialogControlKeyUpHandler;

    private void DialogControlKeyUpHandler(object? sender, KeyEventArgs e)
    {
        if (e.Handled)
            return;
        
        switch (e.KeyCode)
        {
            case Keys.Enter:
                DialogResult = DialogResult.OK;
                break;
            case Keys.Escape:
                DialogResult = DialogResult.Cancel;
                break;
        }
    }

    public IOxControl? FirstFocusControl { get; set; }

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
        FirstFocusControl?.Focus();
        base.OnShown(e);
    }

    public void SetFooterButtonText(OxDialogButton dialogButton, string text) =>
        FormPanel.SetFooterButtonText(dialogButton, text);

    public IOxPanel Footer => FormPanel.Footer;
}

public class OxDialog : OxDialog<OxDialogPanel>
{ 
}