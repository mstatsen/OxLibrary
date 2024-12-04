using OxLibrary.Handlers;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using static System.Windows.Forms.Control;

namespace OxLibrary.Interfaces
{
    public interface IOxControl : IOxControlManager
    {
        AccessibleObject AccessibilityObject { get; }
        string AccessibleDefaultActionDescription { get; set; }
        string AccessibleDescription { get; set; }
        AnchorStyles Anchor { get; set; }
        string AccessibleName { get; set; }
        AccessibleRole AccessibleRole { get; set; }
        bool AllowDrop { get; set; }
        bool AutoSize { get; set; }
        Color BackColor { get; set; }
        Image BackgroundImage { get; set; }
        ImageLayout BackgroundImageLayout { get; set; }
        BindingContext BindingContext { get; set; }
        bool CanFocus { get; }
        bool CanSelect { get; }
        bool Capture { get; set; }
        bool CausesValidation { get; set; }
        string CompanyName { get; }
        bool ContainsFocus { get; }
        ControlCollection Controls { get; }
        ContextMenuStrip ContextMenuStrip { get; set; }
        bool Created { get; }
        Cursor Cursor { get; set; }
        ControlBindingsCollection DataBindings { get; }
        int DeviceDpi { get; }
        bool Disposing { get; }
        bool Enabled { get; set; }
        bool Focused { get; }
        Font Font { get; set; }
        Color ForeColor { get; set; }
        IntPtr Handle { get; }
        bool HasChildren { get; }
        bool InvokeRequired { get; }
        bool IsAccessible { get; set; }
        bool IsAncestorSiteInDesignMode { get; }
        bool IsDisposed { get; }
        bool IsMirrored { get; }
        LayoutEngine LayoutEngine { get; }
        string Name { get; set; }
        bool RecreatingHandle { get; }
        Region Region { get; set; }
        RightToLeft RightToLeft { get; set; }
        ISite Site { get; set; }
        int TabIndex { get; set; }
        bool TabStop { get; set; }
        object Tag { get; set; }
        string Text { get; set; }
        Control TopLevelControl { get; }
        bool UseWaitCursor { get; set; }
        bool Visible { get; set; }
        IAsyncResult BeginInvoke(Action method);
        IAsyncResult BeginInvoke(Delegate method);
        IAsyncResult BeginInvoke(Delegate method, params object[] args);
        void BringToFront();
        bool Contains(Control ctl);
        void CreateControl();
        Graphics CreateGraphics();
        void Dispose();
        DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);
        void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);
        object EndInvoke(IAsyncResult asyncResult);
        Form FindForm();
        bool Focus();
        IContainerControl GetContainerControl();
        Control GetNextControl(Control ctl, bool forward);
        void Hide();
        void Invalidate();
        void Invalidate(Region region);
        void Invalidate(Region region, bool invalidateChildren);
        void Invalidate(bool invalidateChildren);
        void Invoke(Action method);
        object Invoke(Delegate method);
        object Invoke(Delegate method, params object[] args);
        T Invoke<T>(Func<T> method) => (T)Invoke(method, default!);
        int LogicalToDeviceUnits(int value);
        void PerformLayout();
        void PerformLayout(Control affectedControl, string affectedProperty);
        bool PreProcessMessage(ref Message msg);
        PreProcessControlState PreProcessControlMessage(ref Message msg);
        void Refresh();
        void ResetBackColor();
        void ResetBindings();
        void ResetCursor();
        void ResetFont();
        void ResetForeColor();
        void ResetRightToLeft();
        void ResumeLayout();
        void ResumeLayout(bool performLayout);
        void ResetText();
        void Scale(SizeF factor);
        void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap);
        void Select();
        bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);
        void SendToBack();
        void Show();
        void SuspendLayout();
        void Update();

        event EventHandler AutoSizeChanged;
        event EventHandler BackColorChanged;
        event EventHandler BackgroundImageChanged;
        event EventHandler BackgroundImageLayoutChanged;
        event EventHandler BindingContextChanged;
        event EventHandler CausesValidationChanged;
        event UICuesEventHandler ChangeUICues;
        event EventHandler Click;
        event EventHandler ContextMenuStripChanged;
        event ControlEventHandler ControlAdded;
        event ControlEventHandler ControlRemoved;
        event EventHandler CursorChanged;
        event EventHandler? Disposed;
        event EventHandler DoubleClick;
        event EventHandler DpiChangedAfterParent;
        event EventHandler DpiChangedBeforeParent;
        event DragEventHandler DragDrop;
        event DragEventHandler DragEnter;
        event DragEventHandler DragOver;
        event EventHandler DragLeave;
        event EventHandler EnabledChanged;
        event EventHandler Enter;
        event EventHandler FontChanged;
        event EventHandler ForeColorChanged;
        event GiveFeedbackEventHandler GiveFeedback;
        event EventHandler GotFocus;
        event EventHandler HandleCreated;
        event EventHandler HandleDestroyed;
        event HelpEventHandler HelpRequested;
        event InvalidateEventHandler Invalidated;
        event KeyEventHandler KeyDown;
        event KeyPressEventHandler KeyPress;
        event KeyEventHandler KeyUp;
        event LayoutEventHandler Layout;
        event EventHandler Leave;
        event EventHandler LostFocus;
        event MouseEventHandler MouseClick;
        event MouseEventHandler MouseDoubleClick;
        event EventHandler MouseCaptureChanged;
        event MouseEventHandler MouseDown;
        event EventHandler MouseEnter;
        event EventHandler MouseHover;
        event EventHandler MouseLeave;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseWheel;
        event EventHandler Move;
        event PaintEventHandler Paint;
        event PreviewKeyDownEventHandler PreviewKeyDown;
        event QueryContinueDragEventHandler QueryContinueDrag;
        event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;
        event EventHandler RegionChanged;
        event EventHandler Resize;
        event EventHandler RightToLeftChanged;
        event EventHandler StyleChanged;
        event EventHandler SystemColorsChanged;
        event EventHandler TabIndexChanged;
        event EventHandler TabStopChanged;
        event EventHandler TextChanged;
        event CancelEventHandler Validating;
        event EventHandler Validated;
        event EventHandler VisibleChanged;

        void OnDockChanged(OxDockChangedEventArgs e);
        void OnLocationChanged(OxLocationChangedEventArgs e);
        void OnParentChanged(OxParentChangedEventArgs e);
        void OnSizeChanged(OxSizeChangedEventArgs e);
    }
}