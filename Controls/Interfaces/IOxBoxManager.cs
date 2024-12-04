namespace OxLibrary.Controls
{
    public interface IOxBoxManager : IOxControlManager
    {
        OxRectangle ControlZone { get; }
        OxRectangle OuterControlZone { get; }
        bool HandleParentPadding { get; }
        bool Realigning { get; }
        void RealignControls(OxDockType dockType = OxDockType.Unknown);
    }

    public interface IOxBoxManager<TOxControl> : IOxBoxManager
        where TOxControl :
            Control,
            IOxManagingControl<IOxControlManager>,
            IOxBox<TOxControl>
    {
        OxControls<TOxControl> OxControls { get; }
    }
}