using OxLibrary.Controls;
using OxLibrary.Handlers;

namespace OxLibrary.Forms
{
    public class OxForm : Form, IOxControlContainer<Form>
    {
        private readonly bool Initialized = false;
        public OxFormMainPanel MainPanel { get; internal set; }

        private readonly OxControlManager<Form> manager;

        public IOxControlManager Manager => manager;

        private readonly OxControls oxControls;
        public OxControls OxControls => oxControls;

        public new event OxSizeChanged SizeChanged
        {
            add => manager.SizeChanged += value;
            remove => manager.SizeChanged -= value;
        }

        public virtual void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!Initialized ||
                !e.Changed)
                return;

            MainPanel.Size = Size;
            MainPanel.RealignControls();
        }

        public new event OxLocationChanged LocationChanged
        {
            add => manager.LocationChanged += value;
            remove => manager.LocationChanged -= value;
        }

        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }

        public OxForm()
        {
            oxControls = new(this);
            DoubleBuffered = true;
            manager = OxControlManager.RegisterControl<Form>(this);
            MainPanel = CreateMainPanel();
            SetUpForm();
            PlaceMainPanel();
            Initialized = true;
        }

        public new event OxControlEvent ControlAdded
        {
            add => OxControls.ControlAdded += value;
            remove => OxControls.ControlRemoved -= value;
        }

        public new event OxControlEvent ControlRemoved
        {
            add => OxControls.ControlRemoved += value;
            remove => OxControls.ControlRemoved -= value;
        }

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

        public new IOxControlContainer? Parent
        {
            get => manager.Parent;
            set => manager.Parent = value;
        }

        public new OxRectangle ClientRectangle =>
            manager.ClientRectangle;

        public new OxRectangle DisplayRectangle =>
            manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => manager.Bounds;
            set => manager.Bounds = value;
        }

        public OxRectangle ControlZone =>
            manager.ControlZone;

        public bool HandleParentPadding => false;

        public new OxSize PreferredSize =>
            manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => manager.AutoScrollOffset;
            set => manager.AutoScrollOffset = value;
        }

        public void DoWithSuspendedLayout(Action method) =>
            manager.DoWithSuspendedLayout(method);

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

        public void RealignControls(OxDockType dockType = OxDockType.Unknown) =>
            manager.RealignControls(dockType);

        public bool Realigning =>
            manager.Realigning;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RealignControls();
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members
        private static new void OnLocationChanged(EventArgs e) { }
        private static new void OnSizeChanged(EventArgs e) { }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
    }
}