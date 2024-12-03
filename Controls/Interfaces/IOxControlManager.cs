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
        OxRectangle DisplayRectangle { get; }
        OxPoint Location { get; set; }
        OxSize MinimumSize { get; set; }
        OxSize MaximumSize { get; set; }
        OxRectangle Bounds { get; set; }
        OxDock Dock { get; set; }
        OxSize PreferredSize { get; }
        OxPoint AutoScrollOffset { get; set; }
        IOxContainer? Parent { get; set; }
        void DoWithSuspendedLayout(Action method);
        void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height);
        void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified);
        Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue);
        Control GetChildAtPoint(OxPoint pt);
        OxSize GetPreferredSize(OxSize proposedSize);
        void Invalidate(OxRectangle rc);
        void Invalidate(OxRectangle rc, bool invalidateChildren);
        OxSize LogicalToDeviceUnits(OxSize value);
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