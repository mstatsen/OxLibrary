namespace OxLibrary.Controls
{
    public interface IOxControlContainerManager : IOxControlManager
    {
        /// <summary>
        /// Controls from OxLibrary puted in this container
        /// </summary>
        OxControls OxControls { get; }
        /// <summary>
        /// ClientRectange excluded Margin, Border and Padding
        /// </summary>
        OxRectangle ControlZone { get; }
        OxRectangle OuterControlZone { get; }
        bool HandleParentPadding { get; }
        bool Realigning { get; }
        void RealignControls(OxDockType dockType = OxDockType.Unknown);
    }
}