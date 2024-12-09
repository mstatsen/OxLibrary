using OxLibrary.Handlers;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using static System.Windows.Forms.Control;

namespace OxLibrary.Interfaces
{
    public interface IOxControl : IOxControlManager
    {
        void OnDockChanged(OxDockChangedEventArgs e);
        void OnLocationChanged(OxLocationChangedEventArgs e);
        void OnParentChanged(OxParentChangedEventArgs e);
        void OnSizeChanged(OxSizeChangedEventArgs e);

        #region Inherited from Control
        Color BackColor { get; set; }
        string Text { get; set; }
        bool Visible { get; set; }
        void Invalidate();
        Point PointToScreen(Point p);
        void ResumeLayout();
        void SuspendLayout();
        void Update();

        event ControlEventHandler ControlAdded;
        event ControlEventHandler ControlRemoved;
        event EventHandler GotFocus;
        event EventHandler LostFocus;
        event EventHandler VisibleChanged;
        #endregion
    }
}