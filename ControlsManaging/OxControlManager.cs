using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary;

public class OxControlManager : IOxControlManager
{
    protected readonly Control ManagingControl;

    public virtual IOxControl OxControl => (IOxControl)ManagingControl;

    internal OxControlManager(Control managingControl)
    {
        ManagingControl = managingControl;
        ManagingControl.Disposed += ControlDisposedHandler;
        SetHandlers();
    }

    private void BordersSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
        RealignParent();

    private void MarginSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
    {
        if (!e.Changed)
            return;

        if (OxDockHelper.IsVariableWidth(Dock))
            Width = OxWh.S(Z_Width, e.OldValue.Horizontal);

        if (OxDockHelper.IsVariableHeight(Dock))
            Height = OxWh.S(Z_Height, e.OldValue.Vertical);

        RealignParent();
    }

    protected virtual void SetHandlers()
    {
        OxControl.VisibleChanged += VisibleChangedHandler;

        if (OxControl is IOxWithBorders controlWithBorders)
            controlWithBorders.Borders.SizeChanged += BordersSizeChangedHandler;

        if (OxControl is IOxWithMargin controlWithMargin)
            controlWithMargin.Margin.SizeChanged += MarginSizeChangedHandler;
    }

    private void VisibleChangedHandler(object? sender, EventArgs e) =>
        RealignParent();

    protected virtual void RealignParent() =>
        Parent?.Realign();

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

    public int Z_Left
    {
        get => ManagingControl.Left;
        set => ManagingControl.Left = value;
    }

    public int Z_Top
    {
        get => ManagingControl.Top;
        set => ManagingControl.Top = value;
    }

    public int Z_Width
    {
        get => ManagingControl.Width;
        set => ManagingControl.Width = value;
    }

    public int Z_Height
    {
        get => ManagingControl.Height;
        set => ManagingControl.Height = value;
    }

    public Point Z_Location
    {
        get => new(Z_Left, Z_Top);
        set
        {
            Z_Left = value.X;
            Z_Top = value.Y;
        }
    }

    public Size Z_Size
    {
        get => new(Z_Width, Z_Height);
        set
        {
            Z_Width = value.Width;
            Z_Height = value.Height;
        }
    }

    public OxWidth Width
    {
        get => CalcedSizePart(OxDockVariable.Width);
        set => SetZOriginalSizePart(OxDockVariable.Width, value);
    }

    public OxWidth Height
    {
        get => CalcedSizePart(OxDockVariable.Height);
        set => SetZOriginalSizePart(OxDockVariable.Height, value);
    }

    private OxWidth CalcedSizePart(OxDockVariable variable)
    {
        OxWidth calcedValue =
            OxWh.W(
                variable switch
                {
                    OxDockVariable.Width =>
                        Z_Width,
                    OxDockVariable.Height =>
                        Z_Height,
                    _ =>
                        0,
                }
            );

        if (OxDockHelper.Variable(Dock).Equals(variable)
            && OxControl is IOxWithMargin controlWithMargin)
            calcedValue = OxWh.S(calcedValue, controlWithMargin.Margin.ByDockVariable(variable));

        return calcedValue;
    }

    private void SetZOriginalSizePart(OxDockVariable variable, OxWidth value)
    {
        if (Size.ByDockVariable(variable).Equals(value))
            return;

        OxSize oldSize = new(Size);
        OxWidth calcedValue = value;

        if (Dock is not OxDock.None
            && OxControl is IOxWithMargin controlWithMargin)
            calcedValue = OxWh.Add(
                value,
                controlWithMargin.Margin.ByDockVariable(variable)
            );

        if (!ParentOuterControlZone.IsEmpty)
        {
            if (Dock is OxDock.Right)
                Z_Left = OxWh.I(OxWh.Sub(ParentOuterControlZone.Right, calcedValue));
            else
                if (Dock is OxDock.Bottom)
                Z_Top = OxWh.I(OxWh.Sub(ParentOuterControlZone.Bottom, calcedValue));

            calcedValue = OxWh.Min(
                calcedValue,
                OxWh.Sub(
                    ParentOuterControlZone.LastByDockVariable(variable),
                    Location.ByDockVariable(variable)
                )
            );
        }

        switch (variable)
        {
            case OxDockVariable.Width:
                Z_Width = OxWh.I(calcedValue);
                break;
            case OxDockVariable.Height:
                Z_Height = OxWh.I(calcedValue);
                break;
        }

        Z_SaveSize();
        OnSizeChanged(new(oldSize, Size));
    }

    public OxWidth Bottom =>
        OxWh.S(ManagingControl.Bottom, ParentOuterControlZone.Y);

    public OxWidth Right =>
        OxWh.S(ManagingControl.Right, ParentOuterControlZone.X);

    public OxWidth Top
    {
        get => OxWh.S(Z_Top, ParentInnerControlZone.Y);
        set
        {
            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
                return;

            OxPoint oldLocation = new(Location);
            Z_Top = OxWh.IAdd(value, ParentInnerControlZone.Y);
            OnLocationChanged(new(oldLocation, Location));
            Z_SaveLocation();
        }
    }

    public OxWidth Left
    {
        get => OxWh.S(Z_Left, ParentInnerControlZone.X);
        set
        {
            if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
                return;

            OxPoint oldLocation = new(Location);
            Z_Left = OxWh.IAdd(value, ParentInnerControlZone.X);
            OnLocationChanged(new(oldLocation, Location));
            Z_SaveLocation();
        }
    }

    private OxDock SavedDock = OxDock.None;
    private OxSize SavedSize = OxSize.Empty;
    private OxPoint SavedLocation = OxPoint.Empty;

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

    public void Z_SaveLocation()
    {
        if (DockCnahging)
            return;

        SavedLocation = new(Z_Left, Z_Top);
    }

    public void Z_SaveSize()
    {
        if (DockCnahging)
            return;

        SavedSize = new(Z_Width, Z_Height);
    }

    public void Z_RestoreLocation() =>
        Z_Location = new(SavedLocation.Z_X, SavedLocation.Z_Y);

    public void Z_RestoreSize()
    {
        if (!SavedSize.IsEmpty)
            Z_Size = new (SavedSize.Z_Width, SavedSize.Z_Height);
    }

    private OxRectangle ParentOuterControlZone =>
        Parent is null
            ? OxRectangle.Max
            : Parent.OuterControlZone;

    private OxRectangle ParentInnerControlZone =>
        Parent is null
            ? OxRectangle.Max
            : Parent.InnerControlZone;

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
            if (e.OldValue is OxDock.Fill
                || e.NewValue is OxDock.Fill)
            {
                Z_RestoreLocation();
                Z_RestoreSize();
            }

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
        e.NewValue?.Realign();
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