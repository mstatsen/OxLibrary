using OxLibrary.Controls;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Forms
{
    public class OxForm : Form,
        IOxContainer,
        IOxWithColorHelper
    {
        private readonly bool Initialized = false;
        public OxFormMainPanel MainPanel { get; internal set; }

        public IOxContainerManager Manager { get; }

        public OxControls OxControls => Manager.OxControls;

        public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
        public new event OxDockChangedEvent DockChanged
        {
            add => Manager.DockChanged += value;
            remove => Manager.DockChanged -= value;
        }

        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
        public new event OxLocationChangedEvent LocationChanged
        {
            add => Manager.LocationChanged += value;
            remove => Manager.LocationChanged -= value;
        }

        public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
        public new event OxParentChangedEvent ParentChanged
        {
            add => Manager.ParentChanged += value;
            remove => Manager.ParentChanged -= value;
        }

        public virtual void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!Initialized ||
                !e.Changed)
                return;

            MainPanel.Size = Size;
            RealignControls();
        }

        public new event OxSizeChangedEvent SizeChanged
        {
            add => Manager.SizeChanged += value;
            remove => Manager.SizeChanged -= value;
        }

        public OxForm()
        {
            Initialized = false;

            try
            {
                DoubleBuffered = true;
                Manager = OxControlManagers.RegisterContainer(this);
                MainPanel = CreateMainPanel();
                MainPanel.Colors.BaseColorChanged += BaseColorChangedHandler;
                SetUpForm();
                PlaceMainPanel();
            }
            finally
            {
                Initialized = true;
            }
        }

        private void BaseColorChangedHandler(object? sender, EventArgs e) =>
            PrepareColors();

        public void MoveToScreenCenter()
        {
            Screen screen = Screen.FromControl(this);
            SetBounds(
                OxWh.Add(
                    screen.Bounds.Left,
                    OxWh.Div(
                        OxWh.Sub(screen.WorkingArea.Width, Width),
                        OxWh.W2
                    )
                ),
                OxWh.Add(
                    screen.Bounds.Top,
                    OxWh.Div(
                        OxWh.Sub(screen.WorkingArea.Height, Height),
                        OxWh.W2
                    )
                ),
                Width,
                Height
            );
        }

        protected virtual void SetUpForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            SetUpSizes(WindowState);
            StartPosition = FormStartPosition.CenterParent;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MainPanel.SetIcon();
        }

        public void SetUpSizes(FormWindowState state)
        {
            MaximumSize = OxControlHelper.ScreenSize(this);
            OxSize wantedMinimumSize = WantedMinimumSize;
            MinimumSize = new(
                OxWh.Min(wantedMinimumSize.Width, MaximumSize.Width),
                OxWh.Min(wantedMinimumSize.Height, MaximumSize.Height)
            );
            WindowState = state;
            RealignControls();
        }

        public new FormWindowState WindowState
        {
            get => base.WindowState;
            set
            {
                if (WindowState.Equals(value))
                    return;

                if (value is FormWindowState.Minimized)
                {
                    base.WindowState = value;
                    return;
                }

                OxSize oldSize = new(Size);
                base.WindowState = value;
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public virtual OxSize WantedMinimumSize =>
            new(OxWh.W640, OxWh.W480);

        protected virtual OxFormMainPanel CreateMainPanel() =>
            new(this);

        private void PlaceMainPanel()
        {
            MainPanel.Parent = this;
            MainPanel.Location = new(OxWh.W0, OxWh.W0);
            MainPanel.Size = new(Width, Height);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (Initialized)
                e.Control.Parent = MainPanel;
            else base.OnControlAdded(e);
        }

        protected override void OnTextChanged(EventArgs e) =>
            MainPanel.Text = Text;

        private bool canMaximize = true;
        private bool canMinimize = true;

        public bool CanMaximize
        {
            get => canMaximize;
            set
            {
                canMaximize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }

        public bool CanMinimize
        {
            get => canMinimize;
            set
            {
                canMinimize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }


        private bool sizeble = true;
        public bool Sizeble
        {
            get => sizeble;
            set
            {
                sizeble = value;
                MainPanel.SetMarginsSize();
            }
        }

        public virtual Bitmap? FormIcon => null;

        public void ClearConstraints()
        {
            MinimumSize = OxSize.Empty;
            MaximumSize = MinimumSize;
        }

        public void FreezeSize()
        {
            if (MainPanel is null)
                return;

            MinimumSize = MainPanel.Size;
            MaximumSize = MinimumSize;
        }

        public Color BaseColor
        {
            get => MainPanel.BaseColor;
            set => MainPanel.BaseColor = value;
        }

        public new OxWidth Width
        {
            get => Manager.Width;
            set => Manager.Width = value;
        }

        public new OxWidth Height
        {
            get => Manager.Height;
            set => Manager.Height = value;
        }

        public new OxWidth Top
        {
            get => Manager.Top;
            set => Manager.Top = value;
        }

        public new OxWidth Left
        {
            get => Manager.Left;
            set => Manager.Left = value;
        }

        public new OxWidth Bottom => Manager.Bottom;

        public new OxWidth Right => Manager.Right;

        public new OxSize Size
        {
            get => Manager.Size;
            set => Manager.Size = value;
        }

        public new OxSize ClientSize
        {
            get => Manager.ClientSize;
            set => Manager.ClientSize = value;
        }

        public new OxPoint Location
        {
            get => Manager.Location;
            set => Manager.Location = value;
        }

        public new OxSize MinimumSize
        {
            get => Manager.MinimumSize;
            set => Manager.MinimumSize = value;
        }

        public new OxSize MaximumSize
        {
            get => Manager.MaximumSize;
            set => Manager.MaximumSize = value;
        }

        public new OxDock Dock
        {
            get => Manager.Dock;
            set => Manager.Dock = value;
        }

        public new IOxContainer? Parent
        {
            get => Manager.Parent;
            set => Manager.Parent = value;
        }

        public new OxRectangle ClientRectangle =>
            Manager.ClientRectangle;

        public new OxRectangle DisplayRectangle =>
            Manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => Manager.Bounds;
            set => Manager.Bounds = value;
        }

        public OxRectangle ControlZone =>
            Manager.ControlZone;

        public bool HandleParentPadding => false;

        public new OxSize PreferredSize =>
            Manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => Manager.AutoScrollOffset;
            set => Manager.AutoScrollOffset = value;
        }

        public void DoWithSuspendedLayout(Action method) =>
            Manager.DoWithSuspendedLayout(method);

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            Manager.GetChildAtPoint(pt, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            Manager.GetChildAtPoint(pt);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            Manager.GetPreferredSize(proposedSize);

        public void Invalidate(OxRectangle rc) =>
            Manager.Invalidate(rc);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            Manager.Invalidate(rc, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            Manager.LogicalToDeviceUnits(value);

        public OxPoint PointToClient(OxPoint p) =>
            Manager.PointToClient(p);

        public OxPoint PointToScreen(OxPoint p) =>
            Manager.PointToScreen(p);

        public OxRectangle RectangleToClient(OxRectangle r) =>
            Manager.RectangleToClient(r);

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            Manager.RectangleToScreen(r);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            Manager.SetBounds(x, y, width, height, specified);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            Manager.SetBounds(x, y, width, height);

        public void RealignControls(OxDockType dockType = OxDockType.Unknown) =>
            Manager.RealignControls(dockType);

        public bool Realigning =>
            Manager.Realigning;

        public OxColorHelper Colors => MainPanel.Colors;

        public OxRectangle OuterControlZone => Manager.OuterControlZone;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            MainPanel.Location = new(OxPoint.Empty);
            MainPanel.Size = new(Size);
            RealignControls();
        }

        public virtual void PrepareColors() =>
            MainPanel.PrepareColors();

        #region Hidden base methods
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
        #endregion
    }
}