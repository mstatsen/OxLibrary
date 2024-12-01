using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public sealed class OxControlManager<TBaseControl> : IOxControlManager<TBaseControl>
        where TBaseControl : Control
    {
        private readonly TBaseControl managingControl;
        public IOxControl ManagingControl => (IOxControl)managingControl;
        private IOxControlContainer<TBaseControl>? AsContainer =>
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
            if (AsContainer is not null)
            {
                AsContainer.ControlAdded += ControlAddedHandler;
                AsContainer.ControlRemoved += ControlRemovedHandler;
            }

            if (ManagingControl is IOxWithPadding controlWithPadding)
                controlWithPadding.Padding.SizeChanged += PaddingSizeChangedHandler;

            if (ManagingControl is IOxWithBorders controlWithBorders)
                controlWithBorders.Borders.SizeChanged += BordersSizeChangedHandler;

            if (ManagingControl is IOxWithMargin controlWithMargin)
                controlWithMargin.Margin.SizeChanged += MarginSizeChangedHandler;
        }

        private IOxControlContainer? ParentForRealign =>
            AsContainer is null
                ? null
                : Parent is not null
                    ? Parent
                    : AsContainer;

        private void BordersSizeChangedHandler(object sender, BordersChangedEventArgs e) =>
            ParentForRealign?.RealignControls();

        private void PaddingSizeChangedHandler(object sender, BordersChangedEventArgs e) =>
            ParentForRealign?.RealignControls();

        private void MarginSizeChangedHandler(object sender, BordersChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            if (OxDockHelper.IsVariableWidth(Dock))
                Width = OxWh.S(OriginalWidth, e.OldValue.Horizontal);

            if (OxDockHelper.IsVariableHeight(Dock))
                Height = OxWh.S(OriginalHeight, e.OldValue.Vertical);

            ParentForRealign?.RealignControls();
        }

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
                || e.Control is not IOxControl oxControl)
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

        private int OriginalLeft
        {
            get => managingControl.Left;
            set => managingControl.Left = value;
        }

        private int OriginalTop
        {
            get => managingControl.Top;
            set => managingControl.Top = value;
        }

        private int OriginalWidth
        {
            get => managingControl.Width;
            set => managingControl.Width = value;
        }

        private int OriginalHeight
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

        public OxWidth Bottom => OxWh.S(managingControl.Bottom, ParentControlZone.Y);

        public OxWidth Right => OxWh.S(managingControl.Right, ParentControlZone.X);

        public OxWidth Top
        {
            get => OxWh.S(OriginalTop, ParentControlZone.Y);
            set => OriginalTop = OxWh.IAdd(value, ParentControlZone.Y);
        }

        public OxWidth Left
        {
            get => OxWh.S(OriginalLeft, ParentControlZone.X);
            set => OriginalLeft = OxWh.IAdd(value, ParentControlZone.X);
        }

        private OxDock SavedDock = OxDock.None;
        private OxSize SavedSize = OxSize.Empty;
        public bool DockCnahging { get; private set; } = false;

        public OxDock Dock
        {
            get => SavedDock;
            set
            {
                if (SavedDock.Equals(value))
                    return;

                managingControl.Dock = DockStyle.None;
                SavedDock = value;
                DockCnahging = true;

                try
                {
                    RestoreSize();
                    ParentForRealign?.RealignControls();
                }

                finally
                {
                    DockCnahging = false;
                }
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

        private OxRectangle OuterControlZone
        {
            get
            {
                OxRectangle outerZone = new(ManagingControl.ClientRectangle);

                if (ManagingControl is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (ManagingControl is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (ManagingControl is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;

                return outerZone;
            }
        }

        public bool Realigning { get; private set; } = false;

        public bool ParentRealigning => 
            Parent is not null 
            && Parent.Realigning;

        public void RealignControls(OxDockType dockType = OxDockType.Unknown)
        {
            if (AsContainer is null
                || OuterControlZone.IsEmpty
                || Realigning)
                return;

            Realigning = true;

            try
            {
                DoWithSuspendedLayout(() =>
                {
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
                    AsContainer.Invalidate();
                });
            }
            finally
            {
                Realigning = false;
            }
        }

        private OxSize GetRealControlSize(IOxControl control)
        {
            OxDockVariable dockVariable = OxDockHelper.Variable(control.Dock);
            OxWidth realWidth = 
                dockVariable is OxDockVariable.Width
                             or OxDockVariable.None
                    ? control.Width
                    : ControlZone.Width;

            OxWidth realHeight =
                dockVariable is OxDockVariable.Height
                             or OxDockVariable.None
                    ? control.Height
                    : ControlZone.Height;

            if (control is IOxWithMargin controlWithMargin
                && !controlWithMargin.Margin.IsEmpty)
            {
                if (dockVariable is OxDockVariable.Width)
                    realWidth = OxWh.A(realWidth, controlWithMargin.Margin.Horizontal);

                if (dockVariable is OxDockVariable.Height)
                    realHeight = OxWh.A(realHeight, controlWithMargin.Margin.Vertical);
            }

            return 
                new(
                    OxWh.Min(realWidth, ControlZone.Width),
                    OxWh.Min(realHeight, ControlZone.Height)
                );
        }

        private List<IOxControl> GetControls(OxDockType dockType) => 
            AsContainer is null
                ? new()
                : AsContainer.OxControls.Controls(dockType);

        private void RealignDockedControls()
        {
            if (AsContainer is null)
                return;

            ControlZone = new(OuterControlZone);

            if (ControlZone.IsEmpty)
                return;

            OxRectangle currentBounds;

            foreach (IOxControl control in GetControls(OxDockType.Docked))
            {
                currentBounds = new(ControlZone);

                if (control is IOxControlContainer childContainer
                    && !childContainer.HandleParentPadding
                    && AsContainer is IOxWithPadding containerWithPadding
                    && !containerWithPadding.Padding.IsEmpty)
                    currentBounds += containerWithPadding.Padding;

                OxSize realControlSize = GetRealControlSize(control);

                switch (control.Dock)
                {
                    case OxDock.Right:
                        currentBounds.X = OxWh.S(ControlZone.Right, realControlSize.Width);
                        break;
                    case OxDock.Bottom:
                        currentBounds.Y = OxWh.S(ControlZone.Bottom, realControlSize.Height);
                        break;
                }

                switch (OxDockHelper.Variable(control.Dock))
                {
                    case OxDockVariable.Width:
                        currentBounds.Width = OxWh.Min(control.Width, ControlZone.Width);
                        break;
                    case OxDockVariable.Height:
                        currentBounds.Height = OxWh.Min(control.Height, ControlZone.Height);
                        break;
                }

                SetControlBounds(control, currentBounds);
                SubstractControlFromControlZone(control, realControlSize);

                if (ControlZone.IsEmpty)
                    break;
            }
        }

        private static void SetControlBounds(IOxControl control, OxRectangle newBounds)
        {
            control.Manager.DoWithSuspendedLayout(
                () =>
                {
                    if (!control.Location.Equals(newBounds.Location))
                        control.Location = newBounds.Location;

                    if (!control.Size.Equals(newBounds.Size))
                        control.Size = newBounds.Size;
                }
            );

            if (control is IOxControlContainer container)
                container.RealignControls();

            control.Invalidate();
        }

        private void SubstractControlFromControlZone(IOxControl control, OxSize realControlSize)
        {
            if (control.Dock is OxDock.Fill)
            {
                ControlZone.Clear();
                return;
            }

            OxWidth newX = ControlZone.X;
            OxWidth newY = ControlZone.Y;
            OxWidth newWidth = ControlZone.Width;
            OxWidth newHeight = ControlZone.Height;

            if (control.Dock is OxDock.Left)
                newX = OxWh.A(newX, realControlSize.Width);

            if (control.Dock is OxDock.Top)
                newY = OxWh.A(newY, realControlSize.Height);

            if (OxDockHelper.IsHorizontal(control.Dock))
                newWidth = OxWh.S(newWidth, realControlSize.Width);

            if (OxDockHelper.IsVertical(control.Dock))
                newHeight = OxWh.S(newHeight, realControlSize.Height);

            ControlZone = new(
                newX,
                newY,
                newWidth,
                newHeight
            );
        }

        private void RealignUndockedControls(OxRectangle oldControlZone, bool force)
        {
            if (AsContainer is null
                || (!force
                    && oldControlZone.Equals(ControlZone)))
                return;

            if (AsContainer is null)
                return;

            foreach (IOxControl oxControl in GetControls(OxDockType.Undocked))
            {
                oxControl.Left = OxWh.S(((Control)oxControl).Left, oldControlZone.X);
                oxControl.Top = OxWh.S(((Control)oxControl).Top, oldControlZone.Y);

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
            get => new(Left, Top);
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
            if (!e.Changed)
                return false;

            managingOnSizeChanged(e);

            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
                ParentForRealign?.RealignControls();

            return true;
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