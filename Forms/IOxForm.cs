namespace OxLibrary.Forms;

public interface IOxForm :
    IOxFormPanel
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

public interface IOxForm<TForm, TFormPanel> : IOxForm
    where TForm : IOxForm<TForm, TFormPanel>
    where TFormPanel : IOxFormPanel<TForm, TFormPanel>, new()
{
    TFormPanel FormPanel { get; }
}