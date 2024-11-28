using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public sealed class OxControlManager<TBaseControl> : IOxControlManager<TBaseControl>
        where TBaseControl : Control
    {
        private readonly TBaseControl managingControl;
        public IOxControl ManagingControl => (IOxControl)managingControl;
        private IOxControlContainer<TBaseControl>? OxControlContainer =>
            managingControl as IOxControlContainer<TBaseControl>;

        private bool IsControlContainer => managingControl is IOxControlContainer<TBaseControl>;

        private readonly Func<SizeChangedEventArgs, bool> managingOnSizeChanged;
        internal OxControlManager(TBaseControl managingControl, Func<SizeChangedEventArgs, bool> onSizeChanged)
        {
            this.managingControl = managingControl;
            this.managingControl.Disposed += ControlDisposedHandler;

            if (IsControlContainer)
            {
                OxControlContainer!.ControlAdded += ControlAddedHandler;
                OxControlContainer!.ControlRemoved += ControlRemovedHandler;
            }

            ControlZone = new(OxWh.W0, OxWh.W0, OxWh.Maximum, OxWh.Maximum);
            managingOnSizeChanged = onSizeChanged;
        }

        private void ControlRemovedHandler(object? sender, ControlEventArgs e) =>
            OxControlContainer!.OxControls.Remove((IOxControl)e.Control);

        private void ControlAddedHandler(object? sender, ControlEventArgs e) =>
            OxControlContainer!.OxControls.Add((IOxControl)e.Control);

        private void ControlDisposedHandler(object? sender, EventArgs e) =>
            OxControlManager.UnRegisterControl(managingControl);

        private bool sizeChanging = false;

        public bool SizeChanging => sizeChanging;

        public bool SilentSizeChange(Action method, OxSize oldSize)
        {
            if (SizeChanging)
                return false;

            //sizeChanging = true;
            managingControl.SuspendLayout();

            try
            {
                method();
            }
            finally
            {
                //sizeChanging = false;
                managingControl.ResumeLayout();
            }

            //OnSizeChanged(new(oldSize, Size));
            return true;
        }

        public OxWidth Width
        {
            get => OxWh.W(managingControl.Width);
            set
            {
                OxSize oldSize = new(Size);

                if (Width.Equals(value))
                    return;

                managingControl.Width = OxWh.Int(value);
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxWidth Height
        {
            get => OxWh.W(managingControl.Height);
            set
            {
                OxSize oldSize = new(Size);

                if (Height.Equals(value))
                    return;

                managingControl.Height = OxWh.Int(value);
                OnSizeChanged(new(oldSize, Size));
            }
        }
        public OxWidth Bottom => OxWh.W(managingControl.Bottom);

        public OxWidth Right => OxWh.W(managingControl.Right);

        public OxWidth Top
        {
            get => OxWh.Sub(managingControl.Top, ParentControlZone.Y);
            set => managingControl.Top = OxWh.IAdd(value, ParentControlZone.Y);
        }

        public OxWidth Left
        {
            get => OxWh.Sub(managingControl.Left, ParentControlZone.X);
            set => managingControl.Left = OxWh.IAdd(value, ParentControlZone.X);
        }

        public OxDock SavedDock = OxDock.None;

        public OxDock Dock
        {
            get => SavedDock;
            set
            {
                if (SavedDock.Equals(value))
                    return;

                managingControl.Dock = DockStyle.None;
                SavedDock = value;
                Parent?.RealignControls();
            }
        }

        public OxRectangle ControlZone { get; private set; }

        private OxRectangle ParentControlZone =>
            Parent is null
                ? new(OxWh.W0, OxWh.W0, OxWh.Maximum, OxWh.Maximum)
                : Parent.ControlZone;

        public void RealignControls(OxControlDockType dockType = OxControlDockType.Unknown)
        {
            if (!IsControlContainer
                || OxControlContainer!.FullControlZone.IsEmpty
                )
                return;

            OxRectangle oldControlZone = new(ControlZone);

            ControlZone = new(OxControlContainer!.FullControlZone);
            OxRectangle currentBounds = new(ControlZone);

            if (dockType is not OxControlDockType.Undocked)
                foreach (IOxControl oxControl in OxControlContainer!.OxControls.ByPlacingPriority)
                {
                    if (ControlZone.IsEmpty)
                        break;

                    OxDock currentDock = oxControl.Dock;
                    currentBounds = new(ControlZone);

                    switch (currentDock)
                    {
                        case OxDock.None:
                            continue;
                        case OxDock.Fill:
                            ControlZone.Clear();
                            break;
                        case OxDock.Left:
                            currentBounds.Width = oxControl.Width;
                            break;
                        case OxDock.Right:
                            currentBounds.X =
                                OxWh.A(
                                    currentBounds.X,
                                    OxWh.S(ControlZone.Width, oxControl.Width)
                                );
                            currentBounds.Width = oxControl.Width;
                            break;
                        case OxDock.Top:
                            currentBounds.Height = oxControl.Height;
                            break;
                        case OxDock.Bottom:
                            currentBounds.Y =
                                OxWh.A(
                                    currentBounds.Y,
                                    OxWh.S(ControlZone.Height, oxControl.Height)
                                );
                            currentBounds.Height = oxControl.Height;
                            break;
                    }

                    if (currentDock is not OxDock.Fill)
                        ControlZone = new(
                            currentDock is OxDock.Left
                                ? OxWh.A(ControlZone.X, oxControl.Width)
                                : ControlZone.X,
                            currentDock is OxDock.Top
                                ? OxWh.A(ControlZone.Y, oxControl.Height)
                                : ControlZone.Y,
                            currentDock is OxDock.Left
                                        or OxDock.Right
                                ? OxWh.S(ControlZone.Width, oxControl.Width)
                                : ControlZone.Width,
                            currentDock is OxDock.Top
                                        or OxDock.Bottom
                                ? OxWh.S(ControlZone.Height, oxControl.Height)
                                : ControlZone.Height
                        );

                    oxControl.Manager.SilentSizeChange(
                        () =>
                        {
                            oxControl.Location = currentBounds.Location;
                            oxControl.Size = currentBounds.Size;
                        },
                        oxControl.Size
                    );

                    //oxControl.Invalidate();
                    oxControl.RealignControls();

                    if (ControlZone.IsEmpty)
                        break;
                }

            if (dockType is not OxControlDockType.Docked)
                RealignUndockedControls(oldControlZone);

            OxControlContainer!.Invalidate();
        }

        private void RealignUndockedControls(OxRectangle oldControlZone)
        {
            if (oldControlZone.Equals(ControlZone))
                return;

            foreach (IOxControl oxControl in OxControlContainer!.OxControls.UndockedControls)
            {
                oxControl.Left = OxWh.Sub(((Control)oxControl).Left, oldControlZone.X);
                oxControl.Top = OxWh.Sub(((Control)oxControl).Top, oldControlZone.Y);
            }
        }

        public IOxControlContainer? Parent
        {
            get => (IOxControlContainer?)managingControl.Parent;
            set
            {
                if (value is null && Parent is not null 
                    || value is not null && value.Equals(Parent))
                    return;

                Parent?.OxControls.Remove(ManagingControl);
                managingControl.Parent = (Control?)value;
                value?.OxControls.Add(ManagingControl);
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
            set => managingControl.ClientSize = value.Size;
        }

        public OxPoint Location
        {
            get => new(managingControl.Location);
            set
            {
                managingControl.Location = value.Point;
            }
        }

        public OxSize MinimumSize
        {
            get => new(managingControl.MinimumSize);
            set => managingControl.MinimumSize = value.Size;
        }
        public OxSize MaximumSize
        {
            get => new(managingControl.MaximumSize);
            set => managingControl.MaximumSize = value.Size;
        }

        public OxRectangle ClientRectangle =>
            new(managingControl.ClientRectangle);

        public OxRectangle DisplayRectangle => new(managingControl.DisplayRectangle);

        public OxRectangle Bounds
        {
            get => new(managingControl.Bounds);
            set => managingControl.Bounds = value.Rectangle;
        }

        public OxSize PreferredSize =>
            new(managingControl.Size);

        public OxPoint AutoScrollOffset
        {
            get => new(managingControl.AutoScrollOffset);
            set => managingControl.AutoScrollOffset = value.Point;
        }

        public bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (sizeChanging
                || !e.Changed)
                return false;

            managingOnSizeChanged(e);
            Parent?.RealignControls();
            return true;
        }

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            managingControl.SetBounds(
                OxWh.Int(x),
                OxWh.Int(y),
                OxWh.Int(width),
                OxWh.Int(height)
            );

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            managingControl.SetBounds(
                OxWh.Int(x),
                OxWh.Int(y),
                OxWh.Int(width),
                OxWh.Int(height),
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
            TBaseControl managingControl,
            Func<SizeChangedEventArgs, bool> onSizeChanged)
            where TBaseControl : Control
        {
            OxControlManager<TBaseControl> oxControlManager = new(managingControl, onSizeChanged);
            Controls.Add(managingControl, oxControlManager);
            return oxControlManager;
        }

        public static void UnRegisterControl(Control control) =>
            Controls.Remove(control);
    }
}