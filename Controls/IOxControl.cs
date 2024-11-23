using System.ComponentModel;
using System.Windows.Forms.Layout;
using static System.Windows.Forms.Control;

namespace OxLibrary.Controls
{
    public interface IOxControl : IOxControlManager
    {
        AccessibleObject AccessibilityObject { get; }
        IOxControlManager Manager { get; }
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
        event EventHandler ClientSizeChanged;
        event EventHandler Click;
        event EventHandler ContextMenuStripChanged;
        event ControlEventHandler ControlAdded;
        event ControlEventHandler ControlRemoved;
        event EventHandler CursorChanged;
        event EventHandler? Disposed;
        event EventHandler DockChanged;
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
        event EventHandler LocationChanged;
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
        event EventHandler ParentChanged;
        event PreviewKeyDownEventHandler PreviewKeyDown;
        event QueryContinueDragEventHandler QueryContinueDrag;
        event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;
        event EventHandler RegionChanged;
        event EventHandler Resize;
        event EventHandler RightToLeftChanged;
        event EventHandler SizeChanged;
        event EventHandler StyleChanged;
        event EventHandler SystemColorsChanged;
        event EventHandler TabIndexChanged;
        event EventHandler TabStopChanged;
        event EventHandler TextChanged;
        event CancelEventHandler Validating;
        event EventHandler Validated;
        event EventHandler VisibleChanged;
    }

    /// <remarks>
    /// The Control interface that is supposed to be used in the OxDaoEngine component system.
    /// In addition to implementing IOxControl, it is also necessary 
    /// to implement the parent interface IOxControlManager.<br/><br/>
    /// <b>Firstly:</b> you need registry manager as OxControlManager&lt;BaseControl&gt; by call 
    /// <code>manager = OxControlManager.RegisterControl&lt;BaseControlClass&gt;(this,OnSizeChanged);</code>
    /// <b>Secondly:</b> add to your class code, like following 
    /// (<comment>for all code, see comments below ther IOxControl description in IOxControl.cs</comment>):<br/><br/>
    /// <code>
    /// public new OxWidth Width { get =&gt; manager.Width; set =&gt; manager.Width = value;}<br/>
    /// ...<br/>
    /// public virtual bool OnSizeChanged(SizeChangedEventArgs e)<br/>
    /// {<br/>
    ///     if (!SizeChanging &amp;&amp; e.Changed)<br/>
    ///         base.OnSizeChanged(e);<br/>
    ///     return e.Changed;<br/>
    /// }<br/><br/>
    /// protected override sealed void OnSizeChanged(EventArgs e) =&gt;<br/>
    ///     base.OnSizeChanged(e);
    /// </code>
    /// </remarks>
    public interface IOxControl<TBaseControl> : IOxControl
        where TBaseControl : Control
    {
        //new IOxControlManager<TBaseControl> Manager { get; }
    }

    /**
        IOxControlManager Code implementation example for IOxControl<BaseControlClass>
        private readonly OxControlManager<BaseControlClass> manager;
        public OxControlManager<BaseControlClass> Manager => manager;
        

        public ... ()
        {
            manager = OxControlManager.RegisterControl<BaseControlClass>(this,OnSizeChanged);
        }

        OxControlManager<Control> IOxControl<Control>.Manager => default!;

        private readonly List<IOxControl> oxControls = new();
        private readonly OxControls oxControls = new();
        public OxControls OxControls => oxControls;
        private readonly OxDockedControls oxDockedControls = new();
        public OxDockedControls OxDockedControls => oxDockedControls;

        public new OxWidth Width
        {
            get => manager.Width;
            set => manager.Width = value;
        }

        public new OxWidth Height
        {
            get => manager.Height;
            set => manager.Height = value;
        }

        public new OxWidth Top
        {
            get => manager.Top;
            set => manager.Top = value;
        }

        public new OxWidth Left
        {
            get => manager.Left;
            set => manager.Left = value;
        }

        public new OxWidth Bottom => manager.Bottom;

        public new OxWidth Right => manager.Right;

        public new OxSize Size
        {
            get => manager.Size;
            set => manager.Size = value;
        }

        public new OxSize ClientSize
        {
            get => manager.ClientSize;
            set => manager.ClientSize = value;
        }

        public new OxPoint Location 
        {
            get => manager.Location;
            set => manager.Location = value;
        }

        public new OxSize MinimumSize
        {
            get => manager.MinimumSize;
            set => manager.MinimumSize = value;
        }

        public new OxSize MaximumSize
        {
            get => manager.MaximumSize;
            set => manager.MaximumSize = value;
        }

        public new OxDock Dock
        {
            get => manager.Dock;
            set => manager.Dock = value;
        }

        public new IOxControl? Parent
        {
            get => manager.Parent;
            set => manager.Parent = value;
        }

        public new OxRectangle ClientRectangle => manager.ClientRectangle;

        public new OxRectangle DisplayRectangle => manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => manager.Bounds;
            set => manager.Bounds = value;
        }

        public new OxSize PreferredSize => manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => manager.AutoScrollOffset;
            set => manager.AutoScrollOffset = value;
        }

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            manager.GetChildAtPoint(pt, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            manager.GetChildAtPoint(pt);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            manager.GetPreferredSize(proposedSize);

        public void Invalidate(OxRectangle rc) =>
            manager.Invalidate(rc);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            manager.Invalidate(rc, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            manager.LogicalToDeviceUnits(value);

        public OxPoint PointToClient(OxPoint p) =>
            manager.PointToClient(p);

        public OxPoint PointToScreen(OxPoint p) =>
            manager.PointToScreen(p);

        public OxRectangle RectangleToClient(OxRectangle r) =>
            manager.RectangleToClient(r);

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            manager.RectangleToScreen(r);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            manager.SetBounds(x, y, width, height, specified);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            manager.SetBounds(x, y, width, height);

        public void RealignControls() => manager.RealignControls();

        public virtual bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!SizeChanging && e.Changed)
                base.OnSizeChanged(e);

            return e.Changed;
        }

        protected override sealed void OnSizeChanged(EventArgs e) =>
            base.OnSizeChanged(e);
    */
}