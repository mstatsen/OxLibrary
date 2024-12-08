using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public interface IOxBaseFormPanel : IOxFrameWithHeader
{
    OxIconButton CloseButton { get; }
    OxIconButton RestoreButton { get; }
    OxIconButton MinimizeButton { get; }
}

public interface IOxFormPanel : IOxBaseFormPanel
{
    IOxForm? Form { get; set; }
}