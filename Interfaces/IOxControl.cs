using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    public interface IOxControl : IOxControlManager
    {
        void OnDockChanged(OxDockChangedEventArgs e);
        void OnLocationChanged(OxLocationChangedEventArgs e);
        void OnParentChanged(OxParentChangedEventArgs e);
        void OnSizeChanged(OxSizeChangedEventArgs e);

        #region Inherited from Control
        AnchorStyles Anchor { get; set; }
        bool AutoSize { get; set; }
        Color BackColor { get; set; }
        bool Enabled { get; set; }
        Color ForeColor { get; set; }
        Font Font { get; set; }
        object? Tag { get; set; }
        string Text { get; set; }
        bool Visible { get; set; }
        void Invalidate();
        Point PointToScreen(Point p);
        void ResumeLayout();
        void SuspendLayout();
        void Update();
        void Dispose();
        bool Focus();

        event EventHandler Click;
        event ControlEventHandler ControlAdded;
        event ControlEventHandler ControlRemoved;
        event EventHandler GotFocus;
        event EventHandler LostFocus;
        event EventHandler VisibleChanged;
        event EventHandler TextChanged;
        event EventHandler BackColorChanged;
        event EventHandler FontChanged;
        event EventHandler ForeColorChanged;
        event KeyEventHandler KeyUp;
        event EventHandler MouseEnter;
        event EventHandler MouseLeave;
        #endregion
    }
}