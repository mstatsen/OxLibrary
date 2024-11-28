using OxLibrary.Panels;

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
        OxBorders Padding { get; }
        bool HandleParentPadding { get; }
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        void SetBorderWidth(OxWidth value);
        void SetBorderWidth(OxDock dock, OxWidth value);
        bool BorderVisible { get; set; }
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
    }

    public interface IOxControlContainer<TBaseControl> : IOxControl<TBaseControl>, IOxControlContainer
        where TBaseControl : Control
    { 
    }
}