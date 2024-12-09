using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    
    public interface IOxControlManager
    {
        OxWidth Bottom { get; }
        OxRectangle Bounds { get; set; }
        OxRectangle ClientRectangle { get; }
        OxSize ClientSize { get; set; }
        OxDock Dock { get; set; }
        void DoWithSuspendedLayout(Action method);
        Control GetChildAtPoint(OxPoint pt);
        OxWidth Height { get; set; }
        OxWidth Left { get; set; }
        OxPoint Location { get; set; }
        OxSize MaximumSize { get; set; }
        OxSize MinimumSize { get; set; }
        IOxBox? Parent { get; set; }
        OxWidth Right { get; }
        OxSize Size { get; set; }
        OxWidth Top { get; set; }
        OxWidth Width { get; set; }

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