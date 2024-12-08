namespace OxLibrary.Interfaces;

public interface IOxForm :
    IOxFormPanelBase
{
    FormWindowState WindowState { get; set; }
    bool CanMaximize { get; set; }
    bool CanMinimize { get; set; }
    bool Sizable { get; }
    Bitmap? FormIcon { get; }
    DialogResult DialogResult { get; set; }
    void SetState(FormWindowState state);
    void FreezeSize();
    void ClearConstraints();
    void ApplyFormIcon();
}

public interface IOxForm<TFormPanel> : IOxForm
    where TFormPanel : IOxFormPanelBase, new()
{
    TFormPanel FormPanel { get; }
}