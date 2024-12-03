using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxControlManager<TBaseControl> : IOxControlManager<TBaseControl>
        where TBaseControl : Control
    {
        private readonly TBaseControl managingControl;
        public IOxControl ManagingControl => (IOxControl)managingControl;
        private IOxControlContainer<TBaseControl>? AsContainer =>
            managingControl as IOxControlContainer<TBaseControl>;

        internal OxControlManager(TBaseControl managingControl)
        {
            this.managingControl = managingControl;
            this.managingControl.Disposed += ControlDisposedHandler;
            SetHandlers();
            ControlZone = OxRectangle.Max;
        }

        protected virtual void SetHandlers() { }

        protected void RealignParent() =>
            Parent?.RealignControls();

        private void ControlRemovedHandler(object? sender, ControlEventArgs e)
        {
            if (AsContainer is null
                || e.Control is not IOxControl oxControl)
                return;

            AsContainer.OxControls.Remove(oxControl);
        }

        private void ControlAddedHandler(object? sender, ControlEventArgs e)
        {
            if (AsContainer is null
                || e.Control is not IOxControl oxControl
                || e.Control.Equals(ManagingControl))
                return;

            AsContainer.OxControls.Add(oxControl);

            if (AsContainer is IOxWithColorHelper colorHelperControl)
                colorHelperControl.PrepareColors();
        }

        private void ControlDisposedHandler(object? sender, EventArgs e) =>
            OxControlManager.UnRegisterControl(managingControl);

        public void DoWithSuspendedLayout(Action method)
        {
            managingControl.SuspendLayout();

            try
            {
                method();
            }
            finally
            {
                managingControl.ResumeLayout();
            }
        }

        protected int OriginalLeft
        {
            get => managingControl.Left;
            set => managingControl.Left = value;
        }

        protected int OriginalTop
        {
            get => managingControl.Top;
            set => managingControl.Top = value;
        }

        protected int OriginalWidth
        {
            get => managingControl.Width;
            set => managingControl.Width = value;
        }

        protected int OriginalHeight
        {
            get => managingControl.Height;
            set => managingControl.Height = value;
        }

        public OxWidth Width
        {
            get
            {
                OxWidth width = OxWh.W(OriginalWidth);

                if (OxDockHelper.Variable(Dock) is OxDockVariable.Width
                    && ManagingControl is IOxWithMargin controlWithMargin
                    && !controlWithMargin.Margin.IsEmpty)
                    width = OxWh.S(width, controlWithMargin.Margin.Horizontal);

                return width;
            }
            set
            {
                if (Width.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                OriginalWidth =
                    OxWh.IAdd(value,
                        OxDockHelper.Variable(Dock) is OxDockVariable.Width
                        && ManagingControl is IOxWithMargin controlWithMargin
                        && !controlWithMargin.Margin.IsEmpty
                            ? controlWithMargin.Margin.Horizontal
                            : OxWh.W0
                    );
                OnSizeChanged(new(oldSize, Size));
                SaveSize();
            }
        }

        public OxWidth Height
        {
            get
            {
                OxWidth height = OxWh.W(OriginalHeight);

                if (OxDockHelper.Variable(Dock) is OxDockVariable.Height
                    && ManagingControl is IOxWithMargin controlWithMargin
                    && !controlWithMargin.Margin.IsEmpty)
                    height = OxWh.S(height, controlWithMargin.Margin.Vertical);

                return height;
            }
            set
            {
                if (Height.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                OriginalHeight =
                    OxWh.IAdd(
                        value,
                        OxDockHelper.Variable(Dock) is OxDockVariable.Height
                        && ManagingControl is IOxWithMargin controlWithMargin
                        && controlWithMargin.Margin.IsEmpty
                            ? controlWithMargin.Margin.Vertical
                            : OxWh.W0
                    );
                OnSizeChanged(new(oldSize, Size));
                SaveSize();
            }
        }

        public OxWidth Bottom => 
            OxWh.S(managingControl.Bottom, ParentControlZone.Y);

        public OxWidth Right => 
            OxWh.S(managingControl.Right, ParentControlZone.X);

        public OxWidth Top
        {
            get => OxWh.S(OriginalTop, ParentControlZone.Y);
            set
            {
                OxPoint oldLocation = new(Location);
                OriginalTop = OxWh.IAdd(value, ParentControlZone.Y);
                OnLocationChanged(new(oldLocation, Location));
            }
        }

        public OxWidth Left
        {
            get => OxWh.S(OriginalLeft, ParentControlZone.X);
            set
            {
                OxPoint oldLocation = new(Location);
                OriginalLeft = OxWh.IAdd(value, ParentControlZone.X);
                OnLocationChanged(new(oldLocation, Location));
            }
        }

        private OxDock SavedDock = OxDock.None;
        private OxSize SavedSize = OxSize.Empty;

        private readonly OxHandlers Handlers = new();

        private void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
            Handlers.Invoke(type, ManagingControl, args);

        private void AddHandler(OxHandlerType type, Delegate handler) =>
            Handlers.Add(type, handler);

        private void RemoveHandler(OxHandlerType type, Delegate handler) =>
            Handlers.Remove(type, handler);

        public event OxDockChanged DockChanged
        {
            add => AddHandler(OxHandlerType.DockChanged, value);
            remove => RemoveHandler(OxHandlerType.DockChanged, value);
        }

        public event OxLocationChanged LocationChanged
        {
            add => AddHandler(OxHandlerType.LocationChanged, value);
            remove => RemoveHandler(OxHandlerType.LocationChanged, value);
        }

        public event OxParentChanged ParentChanged
        {
            add => AddHandler(OxHandlerType.ParentChanged, value);
            remove => RemoveHandler(OxHandlerType.ParentChanged, value);
        }

        public event OxSizeChanged SizeChanged 
        { 
            add => AddHandler(OxHandlerType.SizeChanged, value);
            remove => RemoveHandler(OxHandlerType.SizeChanged, value); 
        }

        public bool DockCnahging { get; private set; } = false;

        public OxDock Dock
        {
            get => SavedDock;
            set
            {
                if (SavedDock.Equals(value))
                    return;

                OxDock oldDock = SavedDock;
                managingControl.Dock = DockStyle.None;
                SavedDock = value;
                OnDockChanged(new(oldDock, SavedDock));
            }
        }

        private void SaveSize()
        {
            if (DockCnahging
                || ParentRealigning)
                return;

            SavedSize = new(Size);
        }

        private void RestoreSize()
        {
            switch (OxDockHelper.Variable(SavedDock))
            {
                case OxDockVariable.None:
                    Size = new(SavedSize);
                    break;
                case OxDockVariable.Width:
                    Width = SavedSize.Width;
                    break;
                case OxDockVariable.Height:
                    Height = SavedSize.Height;
                    break;
            }
        }

        public OxRectangle ControlZone { get; private set; }

        private OxRectangle ParentControlZone => 
            Parent is null 
                ? OxRectangle.Max 
                : Parent.ControlZone;

        public bool ParentRealigning => 
            Parent is not null 
            && Parent.Realigning;

        public IOxControlContainer? Parent
        {
            get => (IOxControlContainer?)managingControl.Parent;
            set
            {
                if (value is null && Parent is not null 
                    || value is not null && value.Equals(Parent))
                    return;

                IOxControlContainer? oldParent = Parent;
                //Parent?.OxControls.Remove(ManagingControl);
                //OxPoint controlLocation = new(Left, Top);
                managingControl.Parent = (Control?)value;
                OnParentChanged(new(oldParent, Parent));
                //Parent?.OxControls.Add(ManagingControl);
                //Left = controlLocation.X;
                //Top = controlLocation.Y;
            }
        }

        public OxSize Size
        {
            get => new(Width, Height);
            set
            {
                if (!Width.Equals(value.Width))
                    Width = value.Width;

                if (!Height.Equals(value.Height))
                    Height = value.Height;
            }
        }

        public OxSize ClientSize
        {
            get => new(managingControl.ClientSize);
            set
            {
                if (!ClientSize.Equals(value))
                    managingControl.ClientSize = value.Size;
            }
        }

        public OxPoint Location
        {
            get => new(Left, Top);
            set
            {
                OxPoint oldLocation = new(Location);
                managingControl.Location = value.Point;
                OnLocationChanged(new(oldLocation, Location));
            }
        }

        public OxSize MinimumSize
        {
            get => new(managingControl.MinimumSize);
            set
            {
                if (MinimumSize.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                managingControl.MinimumSize = value.Size;
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxSize MaximumSize
        {
            get => new(managingControl.MaximumSize);
            set
            {
                if (MaximumSize.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                managingControl.MaximumSize = value.Size;
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxRectangle ClientRectangle =>
            new(managingControl.ClientRectangle);

        public OxRectangle DisplayRectangle =>
            new(managingControl.DisplayRectangle);

        public OxRectangle Bounds
        {
            get => new(managingControl.Bounds);
            set
            {
                if (!Bounds.Equals(value))
                    managingControl.Bounds = value.Rectangle;
            }
        }

        public OxSize PreferredSize =>
            new(managingControl.Size);

        public OxPoint AutoScrollOffset
        {
            get => new(managingControl.AutoScrollOffset);
            set
            {
                if (!AutoScrollOffset.Equals(value))
                    managingControl.AutoScrollOffset = value.Point;
            }
        }

        private void OnDockChanged(OxDockChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            DockCnahging = true;

            try
            {
                RestoreSize();
                ManagingControl.OnDockChanged(e);
                InvokeHandlers(OxHandlerType.DockChanged, e);
                RealignParent();
            }
            finally
            {
                DockCnahging = false;
            }
        }

        private void OnLocationChanged(OxLocationChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            ManagingControl.OnLocationChanged(e);
            InvokeHandlers(OxHandlerType.LocationChanged, e);
        }

        private void OnParentChanged(OxParentChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            ManagingControl.OnParentChanged(e);
            InvokeHandlers(OxHandlerType.ParentChanged, e);
            e.NewParent?.RealignControls();
        }

        protected virtual void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            ManagingControl.OnSizeChanged(e);
            InvokeHandlers(OxHandlerType.SizeChanged, e);

            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
                RealignParent();
        }

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            managingControl.SetBounds(
                OxWh.I(x),
                OxWh.I(y),
                OxWh.I(width),
                OxWh.I(height)
            );

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            managingControl.SetBounds(
                OxWh.I(x),
                OxWh.I(y),
                OxWh.I(width),
                OxWh.I(height),
                specified
            );

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            managingControl.GetChildAtPoint(pt.Point, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            managingControl.GetChildAtPoint(pt.Point);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            new(managingControl.GetPreferredSize(proposedSize.Size));

        public void Invalidate(OxRectangle rc) =>
            managingControl.Invalidate(rc.Rectangle);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            managingControl.Invalidate(rc.Rectangle, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            new(managingControl.LogicalToDeviceUnits(value.Size));

        public OxPoint PointToClient(OxPoint p) =>
            new(managingControl.PointToClient(p.Point));

        public OxPoint PointToScreen(OxPoint p) =>
            new(managingControl.PointToScreen(p.Point));

        public OxRectangle RectangleToClient(OxRectangle r) =>
            new(managingControl.RectangleToClient(r.Rectangle));

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            new(managingControl.RectangleToScreen(r.Rectangle));
    }

    public static class OxControlManager
    {
        private static readonly Dictionary<Control, IOxControlManager> Controls = new();

        public static OxControlManager<TBaseControl> RegisterControl<TBaseControl>(
            TBaseControl managingControl)
            where TBaseControl : Control
        {
            OxControlManager<TBaseControl> oxControlManager = new(managingControl);
            Controls.Add(managingControl, oxControlManager);
            return oxControlManager;
        }

        public static OxControlContainerManager<TBaseControl> RegisterContainer<TBaseControl>(
            TBaseControl managingControl)
            where TBaseControl : Control, new()
        {
            OxControlContainerManager<TBaseControl> oxControlManager = new(managingControl);
            Controls.Add(managingControl, oxControlManager);
            return oxControlManager;
        }

        public static void UnRegisterControl(Control control) =>
            Controls.Remove(control);
    }
}