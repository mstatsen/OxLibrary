namespace OxLibrary.Controls
{
    public interface IOxContainerManager : IOxControlManager
    {
        OxControls OxControls { get; }
        OxRectangle ControlZone { get; }
        OxRectangle OuterControlZone { get; }
        bool HandleParentPadding { get; }
        bool Realigning { get; }
        void RealignControls(OxDockType dockType = OxDockType.Unknown);
    }
}