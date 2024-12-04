using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxControlManager : IOxControlManager
    {
        protected readonly Control ManagingControl;

        public virtual IOxControl OxControl => (IOxControl)ManagingControl;

        internal OxControlManager(Control managingControl)
        {
            ManagingControl = managingControl;
            ManagingControl.Disposed += ControlDisposedHandler;
            SetHandlers();
            ControlZone = OxRectangle.Max;
        }

        private void BordersSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
            RealignParent();

        private void MarginSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            if (OxDockHelper.IsVariableWidth(Dock))
                Width = OxWh.S(OriginalWidth, e.OldValue.Horizontal);

            if (OxDockHelper.IsVariableHeight(Dock))
                Height = OxWh.S(OriginalHeight, e.OldValue.Vertical);

            RealignParent();
        }

        protected virtual void SetHandlers() 
        {
            if (OxControl is IOxWithBorders controlWithBorders)
                controlWithBorders.Borders.SizeChanged += BordersSizeChangedHandler;

            if (OxControl is IOxWithMargin controlWithMargin)
                controlWithMargin.Margin.SizeChanged += MarginSizeChangedHandler;
        }

        protected virtual void RealignParent() => 
            Parent?.RealignControls();

        private void ControlDisposedHandler(object? sender, EventArgs e) =>
            OxControlManagers.UnRegisterControl(ManagingControl);

        public void DoWithSuspendedLayout(Action method)
        {
            ManagingControl.SuspendLayout();

            try
            {
                method();
            }
            finally
            {
                ManagingControl.ResumeLayout();
            }
        }

        internal int OriginalLeft
        {
            get => ManagingControl.Left;
            set => ManagingControl.Left = value;
        }

        internal int OriginalTop
        {
            get => ManagingControl.Top;
            set => ManagingControl.Top = value;
        }

        internal int OriginalWidth
        {
            get => ManagingControl.Width;
            set => ManagingControl.Width = value;
        }

        internal int OriginalHeight
        {
            get => ManagingControl.Height;
            set => ManagingControl.Height = value;
        }

        public OxWidth Width
        {
            get
            {
                OxWidth width = OxWh.W(OriginalWidth);

                if (OxDockHelper.Variable(Dock) is OxDockVariable.Width
                    && OxControl is IOxWithMargin controlWithMargin
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
                        && OxControl is IOxWithMargin controlWithMargin
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
                    && OxControl is IOxWithMargin controlWithMargin
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
                        && OxControl is IOxWithMargin controlWithMargin
                        && controlWithMargin.Margin.IsEmpty
                            ? controlWithMargin.Margin.Vertical
                            : OxWh.W0
                    );
                OnSizeChanged(new(oldSize, Size));
                SaveSize();
            }
        }

        public OxWidth Bottom => 
            OxWh.S(ManagingControl.Bottom, ParentControlZone.Y);

        public OxWidth Right => 
            OxWh.S(ManagingControl.Right, ParentControlZone.X);

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
            Handlers.Invoke(type, OxControl, args);

        private void AddHandler(OxHandlerType type, Delegate handler) =>
            Handlers.Add(type, handler);

        private void RemoveHandler(OxHandlerType type, Delegate handler) =>
            Handlers.Remove(type, handler);

        public event OxDockChangedEvent DockChanged
        {
            add => AddHandler(OxHandlerType.DockChanged, value);
            remove => RemoveHandler(OxHandlerType.DockChanged, value);
        }

        public event OxLocationChangedEvent LocationChanged
        {
            add => AddHandler(OxHandlerType.LocationChanged, value);
            remove => RemoveHandler(OxHandlerType.LocationChanged, value);
        }

        public event OxParentChangedEvent ParentChanged
        {
            add => AddHandler(OxHandlerType.ParentChanged, value);
            remove => RemoveHandler(OxHandlerType.ParentChanged, value);
        }

        public event OxSizeChangedEvent SizeChanged 
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
                ManagingControl.Dock = DockStyle.None;
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

        public IOxBox? Parent
        {
            get => (IOxBox?)ManagingControl.Parent;
            set
            {
                if (value is null && Parent is not null 
                    || value is not null && value.Equals(Parent))
                    return;

                IOxBox? oldParent = Parent;
                ManagingControl.Parent = (Control?)value;
                OnParentChanged(new(oldParent, Parent));
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
            get => new(ManagingControl.ClientSize);
            set
            {
                if (!ClientSize.Equals(value))
                    ManagingControl.ClientSize = value.Size;
            }
        }

        public OxPoint Location
        {
            get => new(Left, Top);
            set
            {
                OxPoint oldLocation = new(Location);
                ManagingControl.Location = value.Point;
                OnLocationChanged(new(oldLocation, Location));
            }
        }

        public OxSize MinimumSize
        {
            get => new(ManagingControl.MinimumSize);
            set
            {
                if (MinimumSize.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                ManagingControl.MinimumSize = value.Size;
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxSize MaximumSize
        {
            get => new(ManagingControl.MaximumSize);
            set
            {
                if (MaximumSize.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                ManagingControl.MaximumSize = value.Size;
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxRectangle ClientRectangle =>
            new(ManagingControl.ClientRectangle);

        public OxRectangle DisplayRectangle =>
            new(ManagingControl.DisplayRectangle);

        public OxRectangle Bounds
        {
            get => new(ManagingControl.Bounds);
            set
            {
                if (!Bounds.Equals(value))
                    ManagingControl.Bounds = value.Rectangle;
            }
        }

        public OxSize PreferredSize =>
            new(ManagingControl.Size);

        public OxPoint AutoScrollOffset
        {
            get => new(ManagingControl.AutoScrollOffset);
            set
            {
                if (!AutoScrollOffset.Equals(value))
                    ManagingControl.AutoScrollOffset = value.Point;
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
                OxControl.OnDockChanged(e);
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

            OxControl.OnLocationChanged(e);
            InvokeHandlers(OxHandlerType.LocationChanged, e);
        }

        private void OnParentChanged(OxParentChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            OxControl.OnParentChanged(e);
            InvokeHandlers(OxHandlerType.ParentChanged, e);
            e.NewValue?.RealignControls();
        }

        protected virtual void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            OxControl.OnSizeChanged(e);
            InvokeHandlers(OxHandlerType.SizeChanged, e);

            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
                RealignParent();
        }

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            ManagingControl.SetBounds(
                OxWh.I(x),
                OxWh.I(y),
                OxWh.I(width),
                OxWh.I(height)
            );

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            ManagingControl.SetBounds(
                OxWh.I(x),
                OxWh.I(y),
                OxWh.I(width),
                OxWh.I(height),
                specified
            );

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            ManagingControl.GetChildAtPoint(pt.Point, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            ManagingControl.GetChildAtPoint(pt.Point);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            new(ManagingControl.GetPreferredSize(proposedSize.Size));

        public void Invalidate(OxRectangle rc) =>
            ManagingControl.Invalidate(rc.Rectangle);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            ManagingControl.Invalidate(rc.Rectangle, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            new(ManagingControl.LogicalToDeviceUnits(value.Size));

        public OxPoint PointToClient(OxPoint p) =>
            new(ManagingControl.PointToClient(p.Point));

        public OxPoint PointToScreen(OxPoint p) =>
            new(ManagingControl.PointToScreen(p.Point));

        public OxRectangle RectangleToClient(OxRectangle r) =>
            new(ManagingControl.RectangleToClient(r.Rectangle));

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            new(ManagingControl.RectangleToScreen(r.Rectangle));
    }

    public static class OxControlManagers
    {
        private class OxControlManagerCache : Dictionary<Control, IOxControlManager>
        {
            public TManager AddManager<TManager>(Control control, TManager manager)
                where TManager : OxControlManager
            {
                if (!ContainsKey(control))
                    Add(
                        control, manager
                    );

                return (TManager)this[control];
            }
        }

        private static readonly OxControlManagerCache Controls = new();

        public static OxControlManager RegisterControl<TOxControl>(Control baseControl) =>
            Controls.AddManager<OxControlManager>(
                baseControl,
                new OxControlManager(baseControl)
            );

        public static IOxBoxManager<TOxControl> RegisterBox<TOxControl>(Control baseBox)
            where TOxControl :
                    Control,
                    IOxManagingControl<IOxBoxManager<TOxControl>>,
                    IOxManagingControl<IOxControlManager>,
                    IOxControlManager,
                    IOxBox<TOxControl>
            =>
            Controls.AddManager(
                baseBox,
                    new OxBoxManager<TOxControl>(baseBox)
                );

        public static void UnRegisterControl(Control control) =>
            Controls.Remove(control);
    }
}