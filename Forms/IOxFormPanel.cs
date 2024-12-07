using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public interface IOxFormPanel : IOxFrameWithHeader
{
    OxIconButton CloseButton { get; }
    OxIconButton RestoreButton { get; }
    OxIconButton MinimizeButton { get; }
}

public interface IOxFormPanel<TForm, TFormPanel> : IOxFormPanel
    where TForm : IOxForm<TForm, TFormPanel>
    where TFormPanel : IOxFormPanel<TForm, TFormPanel>, new()
{
    TForm? Form { get; set; }
}