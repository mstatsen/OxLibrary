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
        /// Controls from OxLibrary puted in this container, and sided by Dock
        OxDockedControls OxDockedControls { get; }
        /// <summary>
        /// ClientRectange excluded Margin, Border and Padding
        /// </summary>
        OxRectangle FullControlZone { get; }
        /// <summary>
        /// ClientRectange excluded Margin, Border and Padding and excluded all side controls (OxDock is not None)
        /// </summary>
        OxRectangle ControlZone { get; }
    }

    public interface IOxControlContainer<TBaseControl> : IOxControl<TBaseControl>, IOxControlContainer
        where TBaseControl : Control
    { 
    }
}