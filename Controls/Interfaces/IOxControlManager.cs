using OxLibrary.Handlers;

namespace OxLibrary.Controls
{
    public interface IOxControlManager
    {
        OxWidth Width { get; set; }
        OxWidth Height { get; set; }
        OxWidth Top { get; set; }
        OxWidth Left { get; set; }
        OxWidth Bottom { get; }
        OxWidth Right { get; }
        OxSize Size { get; set; }
        OxSize ClientSize { get; set; }
        OxRectangle ClientRectangle { get; }
        OxPoint Location { get; set; }
        OxSize MinimumSize { get; set; }
        OxSize MaximumSize { get; set; }
        OxRectangle Bounds { get; set; }
        OxDock Dock { get; set; }
        OxPoint AutoScrollOffset { get; set; }
        IOxBox? Parent { get; set; }
        void DoWithSuspendedLayout(Action method);
        Control GetChildAtPoint(OxPoint pt);
        void Invalidate(OxRectangle rc);
        OxPoint PointToClient(OxPoint p);
        OxPoint PointToScreen(OxPoint p);
        OxRectangle RectangleToClient(OxRectangle r);
        OxRectangle RectangleToScreen(OxRectangle r);

        event OxDockChangedEvent DockChanged;
        event OxSizeChangedEvent SizeChanged;
        event OxLocationChangedEvent LocationChanged;
        event OxParentChangedEvent ParentChanged;
    }
}