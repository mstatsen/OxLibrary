using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxControlManager<TBaseControl> : IOxControlManager<TBaseControl>
        where TBaseControl : Control
    {
        private readonly TBaseControl managingControl;
        public IOxControl ManagingControl => (IOxControl)managingControl;
        private readonly Func<SizeChangedEventArgs, bool> managingOnSizeChanged;
        internal OxControlManager(TBaseControl managingControl, Func<SizeChangedEventArgs, bool> onSizeChanged)
        {
            this.managingControl = managingControl;
            this.managingControl.Disposed += ControlDisposedHandler;
            this.managingControl.ControlAdded += ControlAddedHandler;
            this.managingControl.ControlRemoved += ControlRemovedHandler;
            managingOnSizeChanged = onSizeChanged;
        }

        private void ControlRemovedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl)
                return;

            ManagingControl.OxControls.Remove(oxControl);
            ManagingControl.OxDockedControls.RemoveControl(oxControl);
            RealignControls();
        }

        private void ControlAddedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl)
                return;

            ManagingControl.OxControls.Add(oxControl);
            ManagingControl.OxDockedControls.AddControl(oxControl);
            RealignControls();
        }

        private void ControlDisposedHandler(object? sender, EventArgs e) => 
            OxControlManager.UnRegisterControl(managingControl);

        private bool sizeChanging = false;

        public bool SizeChanging => sizeChanging;

        public bool SilentSizeChange(Action method)
        {
            if (SizeChanging)
                return false;

            sizeChanging = true;

            try
            {
                method();
            }
            finally
            {
                sizeChanging = false;
            }
            return true;
        }

        public OxWidth Width
        {
            get => OxWh.W(managingControl.Width);
            set
            {
                OxWidth oldWidth = Width;

                if (oldWidth.Equals(value))
                    return;

                managingControl.Width = OxWh.Int(value);
                OnSizeChanged(new SizeChangedEventArgs(oldWidth, Height, value, Height));
            }
        }

        public OxWidth Height
        {
            get => OxWh.W(managingControl.Height);
            set
            {
                OxWidth oldHeight = Height;

                if (oldHeight.Equals(value))
                    return;

                managingControl.Height = OxWh.Int(value);
                OnSizeChanged(new SizeChangedEventArgs(Width, oldHeight, Width, value));
            }
        }
        public OxWidth Bottom => OxWh.W(managingControl.Bottom);

        public OxWidth Right => OxWh.W(managingControl.Right);

        public OxWidth Top
        {
            get => OxWh.W(managingControl.Top);
            set => managingControl.Top = OxWh.Int(value);
        }

        public OxWidth Left
        {
            get => OxWh.W(managingControl.Left);
            set => managingControl.Left = OxWh.Int(value);
        }

        public OxDock SavedDock = OxDock.None;

        public OxDock Dock
        {
            get => SavedDock;
            set
            {
                managingControl.Dock = DockStyle.None;
                SavedDock = value;
                Parent?.RealignControls();
            }
        }

        public void RealignControls()
        {
            if (ClientRectangle.IsEmpty)
                return;

            OxRectangle clientZone = ClientRectangle;
            OxRectangle currentBounds = ClientRectangle;

            foreach (IOxControl oxControl in ManagingControl.OxDockedControls.ByPlacingPriority)
            {
                if (clientZone.IsEmpty)
                    break;

                OxDock currentDock = oxControl.Dock;
                currentBounds = clientZone;

                if (currentDock is OxDock.None)
                    continue;

                switch (currentDock)
                {
                    case OxDock.None:
                        continue;
                    case OxDock.Fill:
                        oxControl.Location = currentBounds.Location;
                        oxControl.Size = currentBounds.Size;
                        clientZone.Clear();
                        break;
                    case OxDock.Left:
                    case OxDock.Right:
                        currentBounds.Width = oxControl.Width;
                        clientZone.Width = 
                            OxWh.Max(
                                OxWh.Sub(
                                    clientZone.Width, 
                                    oxControl.Width), 
                                OxWh.W0
                            );

                        if (currentDock is OxDock.Left)
                            clientZone.X = clientZone.X | oxControl.Width;

                        break;
                    case OxDock.Top:
                    case OxDock.Bottom:
                        currentBounds.Height = oxControl.Height;
                        clientZone.Height = 
                            OxWh.Max(
                                OxWh.Sub(
                                    clientZone.Height, 
                                    oxControl.Height), 
                                OxWh.W0
                            );

                        if (currentDock is OxDock.Top)
                            clientZone.Y = clientZone.Y | oxControl.Height;

                        break;
                }

                ManagingControl.Manager.SilentSizeChange(
                    () => 
                    { 
                        oxControl.Location = currentBounds.Location;
                        oxControl.Size = currentBounds.Size;
                    }
                );
            }
        }

        public IOxControl? Parent
        {
            get => (IOxControl?)managingControl.Parent;
            set => managingControl.Parent = (Control?)value;
        }

        public OxSize Size
        {
            get => new(Width, Height);
            set
            {
                bool changed = false;
                OxWidth oldWidth = Width;
                OxWidth oldHeight = Height;

                SilentSizeChange(
                    () =>
                    {
                        if (!oldWidth.Equals(value.Width))
                        {
                            changed = true;
                            Width = value.Width;
                        }

                        if (!oldHeight.Equals(value.Height))
                        {
                            Height = value.Height;
                            changed = true;
                        }
                    }
                );
                
                if (changed)
                    OnSizeChanged(new SizeChangedEventArgs(oldWidth, oldHeight, Width, Height));
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
            set => managingControl.Location = value.Point;
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

        public bool HasOxChildren => ((IOxControl)managingControl).OxControls.Count > 0;

        public bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (sizeChanging || 
                !e.Changed)
                return false;

            managingOnSizeChanged(e);
            RealignControls();
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