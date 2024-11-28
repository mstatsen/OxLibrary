using OxLibrary.Interfaces;
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

        private readonly Func<SizeChangedEventArgs, bool> managingOnSizeChanged;
        internal OxControlManager(TBaseControl managingControl, Func<SizeChangedEventArgs, bool> onSizeChanged)
        {
            this.managingControl = managingControl;
            this.managingControl.Disposed += ControlDisposedHandler;
            SetContainerHandlers();
            ControlZone = new(OxWh.W0, OxWh.W0, OxWh.Maximum, OxWh.Maximum);
            managingOnSizeChanged = onSizeChanged;
        }

        private void SetContainerHandlers()
        {
            if (OxControlContainer is null)
                return;
            
            OxControlContainer.ControlAdded += ControlAddedHandler;
            OxControlContainer.ControlRemoved += ControlRemovedHandler;
            OxControlContainer.Padding.SizeChanged += BordersSizeChangedHandler;
            OxControlContainer.Borders.SizeChanged += BordersSizeChangedHandler;
            OxControlContainer.Margin.SizeChanged += BordersSizeChangedHandler;
        }

        private void BordersSizeChangedHandler(object sender, BorderEventArgs e)
        {
            if (OxControlContainer is null)
                return;

            IOxControl parentForRealign = OxControlContainer;

            if (Parent is not null)
                parentForRealign = Parent;

            parentForRealign.RealignControls();
        }

        private void ControlRemovedHandler(object? sender, ControlEventArgs e)
        {
            if (OxControlContainer is null
                || e.Control is not IOxControl oxControl)
                return;

            OxControlContainer.OxControls.Remove(oxControl);
        }

        private void ControlAddedHandler(object? sender, ControlEventArgs e)
        {
            if (OxControlContainer is null
                || e.Control is not IOxControl oxControl)
                return;
            
            OxControlContainer.OxControls.Add(oxControl);

            if (OxControlContainer is IOxWithColorHelper colorHelperControl)
                colorHelperControl.PrepareColors();
        }

        private void ControlDisposedHandler(object? sender, EventArgs e) =>
            OxControlManager.UnRegisterControl(managingControl);

        public bool SizeChanging => false;

        public bool SilentSizeChange(Action method, OxSize oldSize)
        {
            if (SizeChanging)
                return false;

            managingControl.SuspendLayout();

            try
            {
                method();
            }
            finally
            {
                managingControl.ResumeLayout();
            }

            return true;
        }

        public OxWidth Width
        {
            get => OxWh.W(managingControl.Width);
            set
            {
                if (Width.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                managingControl.Width = OxWh.Int(value);
                OnSizeChanged(new(oldSize, Size));
            }
        }

        public OxWidth Height
        {
            get => OxWh.W(managingControl.Height);
            set
            {
                if (Height.Equals(value))
                    return;

                OxSize oldSize = new(Size);
                managingControl.Height = OxWh.Int(value);
                OnSizeChanged(new(oldSize, Size));
            }
        }
        public OxWidth Bottom => OxWh.W(managingControl.Bottom);

        public OxWidth Right => OxWh.W(managingControl.Right);

        public OxWidth Top
        {
            get => OxWh.Sub(managingControl.Top, ParentControlZone.Y);
            set
            {
                /*
                if (Top.Equals(value))
                    return;
                */

                managingControl.Top = OxWh.IAdd(value, ParentControlZone.Y);
            }
        }

        public OxWidth Left
        {
            get => OxWh.Sub(managingControl.Left, ParentControlZone.X);
            set
            {
                /*
                if (Left.Equals(value))
                    return;
                */

                managingControl.Left = OxWh.IAdd(value, ParentControlZone.X);
            }
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
                ? OxRectangle.Max 
                : Parent.ControlZone;

        private static OxRectangle OuterControlZone(IOxControlContainer container) =>
            container.ClientRectangle 
            - container.Padding
            - container.Borders 
            - container.Margin;

        public void RealignControls(OxDockType dockType = OxDockType.Unknown)
        {
            if (OxControlContainer is null
                || OuterControlZone(OxControlContainer).IsEmpty)
                return;

            OxControlContainer.SuspendLayout();
            OxRectangle oldControlZone = new(ControlZone);

            if ((dockType is OxDockType.Unknown)
                || (dockType & OxDockType.Docked) is OxDockType.Docked)
                RealignDockedControls();

            if ((dockType is OxDockType.Unknown)
                || (dockType & OxDockType.Undocked) is OxDockType.Undocked)
                RealignUndockedControls(
                    oldControlZone, 
                    dockType is OxDockType.Undocked
                );
            OxControlContainer.ResumeLayout();
            OxControlContainer.Invalidate();
        }

        private void RealignDockedControls()
        {
            if (OxControlContainer is null)
                return;

            ControlZone = new(OuterControlZone(OxControlContainer));
            OxRectangle currentBounds = new(ControlZone);

            foreach (IOxControl oxControl in OxControlContainer.OxControls.Controls(OxDockType.Docked))
            {
                if (ControlZone.IsEmpty)
                    break;

                OxDock currentDock = oxControl.Dock;
                currentBounds = new(ControlZone);

                if (oxControl is IOxControlContainer childContainer
                    && !childContainer.HandleParentPadding
                    && !OxControlContainer.Padding.IsEmpty)
                    currentBounds += OxControlContainer.Padding;

                switch (currentDock)
                {
                    case OxDock.Fill:
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

                SubstractControlFromControlZone(oxControl);
                oxControl.Manager.SilentSizeChange(
                    () =>
                    {
                        oxControl.Location = currentBounds.Location;
                        oxControl.Size = currentBounds.Size;
                    },
                    oxControl.Size
                );

                oxControl.RealignControls();

                if (ControlZone.IsEmpty)
                    break;
            }
        }

        private void SubstractControlFromControlZone(IOxControl control)
        {
            if (control.Dock is OxDock.Fill)
            {
                ControlZone.Clear();
                return;
            }

            ControlZone = new(
                control.Dock is OxDock.Left
                    ? OxWh.A(ControlZone.X, control.Width)
                    : ControlZone.X,
                control.Dock is OxDock.Top
                    ? OxWh.A(ControlZone.Y, control.Height)
                    : ControlZone.Y,
                control.Dock is OxDock.Left
                             or OxDock.Right
                    ? OxWh.S(ControlZone.Width, control.Width)
                    : ControlZone.Width,
                control.Dock is OxDock.Top
                             or OxDock.Bottom
                    ? OxWh.S(ControlZone.Height, control.Height)
                    : ControlZone.Height
            );
        }

        private void RealignUndockedControls(OxRectangle oldControlZone, bool force)
        {
            if (OxControlContainer is null
                || (!force
                    && oldControlZone.Equals(ControlZone)))
                return;

            if (OxControlContainer is null)
                return;

            foreach (IOxControl oxControl in OxControlContainer.OxControls.Controls(OxDockType.Undocked))
            {
                oxControl.Left = OxWh.Sub(((Control)oxControl).Left, oldControlZone.X);
                oxControl.Top = OxWh.Sub(((Control)oxControl).Top, oldControlZone.Y);

                //TODO: cut width and height if its greater then ControlZone
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
                OxPoint controlLocation = new(Left, Top);
                managingControl.Parent = (Control?)value;
                value?.OxControls.Add(ManagingControl);
                Left = controlLocation.X;
                Top = controlLocation.Y;
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
            get => new(managingControl.Location);
            set => managingControl.Location = value.Point;
        }

        public OxSize MinimumSize
        {
            get => new(managingControl.MinimumSize);
            set
            {
                if (!MinimumSize.Equals(value))
                    managingControl.MinimumSize = value.Size;
            }
        }
        public OxSize MaximumSize
        {
            get => new(managingControl.MaximumSize);
            set
            {
                if (!MaximumSize.Equals(value))
                    managingControl.MaximumSize = value.Size;
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

        public bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (SizeChanging
                || !e.Changed)
                return false;

            managingOnSizeChanged(e);

            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
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