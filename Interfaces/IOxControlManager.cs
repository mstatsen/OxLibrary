using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    
    public interface IOxControlManager
    {
        short Bottom { get; }
        OxDock Dock { get; set; }
        void DoWithSuspendedLayout(Action method);
        short Height { get; set; }
        short Left { get; set; }
        OxPoint Location { get; set; }
        OxSize MaximumSize { get; set; }
        OxSize MinimumSize { get; set; }
        IOxBox? Parent { get; set; }
        short Right { get; }
        OxSize Size { get; set; }
        short Top { get; set; }
        short Width { get; set; }

        void AddHandler(OxHandlerType type, Delegate handler);
        void InvokeHandlers(OxHandlerType type, OxEventArgs args);
        void RemoveHandler(OxHandlerType type, Delegate handler);

        event OxDockChangedEvent DockChanged;
        event OxLocationChangedEvent LocationChanged;
        event OxParentChangedEvent ParentChanged;
        event OxSizeChangedEvent SizeChanged;

        #region Internal properties and methods
        [Obsolete("ZBounds it is used only for internal needs")]
        OxZBounds ZBounds { get; }
        #endregion
    }
}