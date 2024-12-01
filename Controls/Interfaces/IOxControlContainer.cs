namespace OxLibrary.Controls
{
    /// <summary>
    /// Interface OxLibrary controls container 
    /// </summary>
    public interface IOxControlContainer : IOxControl
    {
        /// <summary>
        /// Controls from OxLibrary puted in this container
        /// </summary>
        OxControls OxControls { get; }
        /// <summary>
        /// ClientRectange excluded Margin, Border and Padding
        /// </summary>
        OxRectangle ControlZone { get; }
        bool HandleParentPadding { get; }
        bool Realigning { get; }
        void RealignControls(OxDockType dockType = OxDockType.Unknown);
    }

    public interface IOxControlContainer<TBaseControl> : IOxControl<TBaseControl>, IOxControlContainer
        where TBaseControl : Control
    { 
    }
}