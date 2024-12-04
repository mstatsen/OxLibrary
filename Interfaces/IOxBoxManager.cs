namespace OxLibrary.Interfaces;

public interface IOxBoxManager : IOxControlManager
{
    OxRectangle OuterControlZone { get; }
    bool HandleParentPadding { get; }
    bool Realigning { get; }
    void RealignControls(OxDockType dockType = OxDockType.Unknown);
    OxControls OxControls { get; }
}