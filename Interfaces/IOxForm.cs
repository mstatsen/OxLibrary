namespace OxLibrary.Interfaces;

public interface IOxForm :
    IOxFormPanelBase
{
    FormWindowState WindowState { get; set; }
    OxBool CanMaximize { get; set; }
    bool IsCanMaximize { get; }
    void SetCanMaximize(bool value);
    OxBool CanMinimize { get; set; }
    bool IsCanMinimize { get; }
    void SetCanMinimize(bool value);
    OxBool Sizable { get; set; }
    bool IsSizable { get; }
    void SetSizable(bool value);
    Bitmap? FormIcon { get; }
    DialogResult DialogResult { get; set; }
    void SetState(FormWindowState state);
    void FreezeSize();
    void ClearConstraints();
    void ApplyFormIcon();
    event EventHandler? Shown;
}

public interface IOxForm<TFormPanel> : IOxForm
    where TFormPanel : IOxFormPanelBase, new()
{
    TFormPanel FormPanel { get; }
}