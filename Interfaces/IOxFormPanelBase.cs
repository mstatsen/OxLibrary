using OxLibrary.Controls;

namespace OxLibrary.Interfaces;

public interface IOxFormPanelBase : IOxFrameWithHeader
{
    OxIconButton CloseButton { get; }
    OxIconButton RestoreButton { get; }
    OxIconButton MinimizeButton { get; }
}