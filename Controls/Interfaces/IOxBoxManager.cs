namespace OxLibrary.Controls
{
    public interface IOxBoxManager : IOxControlManager
    {
        OxControls OxControls { get; }
        OxRectangle ControlZone { get; }
        OxRectangle OuterControlZone { get; }
        bool HandleParentPadding { get; }
        bool Realigning { get; }
        void RealignControls(OxDockType dockType = OxDockType.Unknown);
    }
}