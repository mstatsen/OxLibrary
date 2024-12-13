using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    
    public interface IOxControlManager
    {
        short Bottom { get; }
        OxDock Dock { get; set; }
        void WithSuspendedLayout(Action method);
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

        OxBool AutoSize { get; set; }
        bool IsAutoSize { get; }
        void SetAutoSize(bool value);

        OxBool Enabled { get; set; }
        public bool IsEnabled { get; }
        public void SetEnabled(bool value);

        OxBool Visible { get; set; }
        bool IsVisible { get; }
        void SetVisible(bool value);

        void AddHandler(OxHandlerType type, Delegate handler);
        void InvokeHandlers(OxHandlerType type, OxEventArgs args);
        void RemoveHandler(OxHandlerType type, Delegate handler);

        event OxBoolChangedEvent AutoSizeChanged;
        event OxDockChangedEvent DockChanged;
        event OxBoolChangedEvent EnabledChanged; 
        event OxLocationChangedEvent LocationChanged;
        event OxParentChangedEvent ParentChanged;
        event OxSizeChangedEvent SizeChanged;
        event OxBoolChangedEvent VisibleChanged;

        #region Internal properties and methods
        [Obsolete("ZBounds it is used only for internal needs")]
        OxZBounds ZBounds { get; }
        #endregion
    }
}