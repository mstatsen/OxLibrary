namespace OxLibrary.Forms;

public interface IOxForm :
    IOxBaseFormPanel
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
    where TFormPanel : IOxBaseFormPanel, new()
{
    TFormPanel FormPanel { get; }
}