using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    public interface IOxControlManager
    {
        OxPoint AutoScrollOffset { get; set; }
        OxWidth Bottom { get; }
        OxRectangle Bounds { get; set; }
        OxRectangle ClientRectangle { get; }
        OxSize ClientSize { get; set; }
        OxDock Dock { get; set; }
        void DoWithSuspendedLayout(Action method);
        Control GetChildAtPoint(OxPoint pt);
        OxWidth Height { get; set; }
        void Invalidate(OxRectangle rc);
        OxWidth Left { get; set; }
        OxPoint Location { get; set; }
        OxSize MaximumSize { get; set; }
        OxSize MinimumSize { get; set; }
        IOxBox? Parent { get; set; }
        OxPoint PointToClient(OxPoint p);
        OxPoint PointToScreen(OxPoint p);
        OxRectangle RectangleToClient(OxRectangle r);
        OxRectangle RectangleToScreen(OxRectangle r);
        OxWidth Right { get; }
        OxSize Size { get; set; }
        OxWidth Top { get; set; }
        OxWidth Width { get; set; }

        event OxDockChangedEvent DockChanged;
        event OxLocationChangedEvent LocationChanged;
        event OxParentChangedEvent ParentChanged;
        event OxSizeChangedEvent SizeChanged;

        #region Internal properties and methods
        [Obsolete("Z_Height it is used only for internal needs. Instead, use Height")]
        int Z_Height { get; set; }
        [Obsolete("Z_Left it is used only for internal needs. Instead, use Left")]
        int Z_Left { get; set; }
        [Obsolete("Z_Location it is used only for internal needs")]
        Point Z_Location { get; set; }
        [Obsolete("Z_Size it is used only for internal needs")]
        Size Z_Size { get; set; }
        [Obsolete("Z_Top it is used only for internal needs. Instead, use Top")]
        int Z_Top { get; set; }
        [Obsolete("Z_Width it is used only for internal needs. Instead, use Width")]
        int Z_Width { get; set; }
        [Obsolete("Z_RestoreLocation it is used only for internal needs")]
        void Z_RestoreLocation();
        [Obsolete("Z_RestoreSize it is used only for internal needs")]
        void Z_RestoreSize();
        [Obsolete("Z_SaveLocation it is used only for internal needs")]
        void Z_SaveLocation();
        [Obsolete("Z_SaveSize it is used only for internal needs")]
        void Z_SaveSize();
        #endregion
    }
}