using OxLibrary.Geometry;
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
        Handlers = new(OxControl);
        ManagingControl.Disposed += ControlDisposedHandler;
        ZBounds = new(ManagingControl);
        SetHandlers();
    }


    public bool IncreaceIfFocused { get; set; } = false;

    private void BordersSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
    {
        if (e.IsChanged)
            RealignParent();
    }

    private void MarginSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        if (OxDockHelper.IsVariableWidth(Dock))
            Width = OxSh.Sub(ZBounds.Width, e.OldValue.Horizontal);

        if (OxDockHelper.IsVariableHeight(Dock))
            Height = OxSh.Sub(ZBounds.Height, e.OldValue.Vertical);

        RealignParent();
    }

    protected virtual void SetHandlers()
    {
        OxControl.GotFocus += GotFocusHandler;
        OxControl.LostFocus += LostFocusHandler;

        if (OxControl is not IOxDependedBox)
        {
            if (OxControl is IOxWithBorders controlWithBorders
                && controlWithBorders.Borders is not null)
                controlWithBorders.Borders.SizeChanged += BordersSizeChangedHandler;

            if (OxControl is IOxWithMargin controlWithMargin
                && controlWithMargin.Margin is not null)
                controlWithMargin.Margin.SizeChanged += MarginSizeChangedHandler;
        }
    }

    private void LostFocusHandler(object? sender, EventArgs e)
    {
        if (IncreaceIfFocused)
        {
            Left += 2;
            Top += 2;
            Height -= 4;
            Width -= 4;
        }
    }

    private void GotFocusHandler(object? sender, EventArgs e)
    {
        if (IncreaceIfFocused)
        {
            Left -= 2;
            Top -= 2;
            Height += 4;
            Width += 4;
        }
    }

    protected virtual void RealignParent()
    {
        if (Parent is null
            || OxB.B(ParentChanging)
            || Parent.IsRealigning)
            return;

        Parent.Realign();
    }

    private void ControlDisposedHandler(object? sender, EventArgs e) =>
        OxControlManagers.UnRegisterControl(ManagingControl);

    public void WithSuspendedLayout(Action method)
    {
        ManagingControl.SuspendLayout();

        try
        {
            IOxBox? dependedFromBox = ManagingControl is IOxDependedBox dependedBox
                ? dependedBox.DependedFrom
                : null; 

            dependedFromBox?.SuspendLayout();

            try
            {
                method();
            }
            finally
            {
                dependedFromBox?.ResumeLayout();
            }
        }
        finally
        {
            ManagingControl.ResumeLayout();
        }
    }

    public OxBool Visible
    {
        get => OxB.B(IsVisible);
        set => SetVisible(OxB.B(value));
    }

    public void SetVisible(bool value)
    {
        if (Visible.Equals(value))
            return;

        ManagingControl.Visible = value;
        OnVisibleChanged(new(!value, value));
    }

    public OxBool Enabled
    {
        get => OxB.B(IsEnabled);
        set => SetEnabled(OxB.B(value));
    }

    public void SetEnabled(bool value)
    {
        if (Enabled.Equals(value))
            return;

        ManagingControl.Visible = value;
        OnEnabledChanged(new(!value, value));
        PrepareColors();
    }

    public OxBool AutoSize 
    {
        get => OxB.B(IsAutoSize);
        set => SetAutoSize(OxB.B(value));
    }
    public bool IsAutoSize => ManagingControl.AutoSize;
    public void SetAutoSize(bool value)
    {
        if (ManagingControl.AutoSize.Equals(value))
            return;

        ManagingControl.AutoSize = value;
        OnAutoSizeChanged(new(!value, value));
    }

    private void OnAutoSizeChanged(OxBoolChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        OxControl.OnAutoSizeChanged(e);
        InvokeHandlers(OxHandlerType.AutoSizeChanged, e);
        RealignParent();
    }

    public bool IsEnabled => ManagingControl.Enabled;
    public bool IsVisible => ManagingControl.Visible;

    public OxZBounds ZBounds { get; }

    public short Width
    {
        get => GetSizePart(OxDockVariable.Width);
        set => SetSizePart(OxDockVariable.Width, value);
    }

    public short Height
    {
        get => GetSizePart(OxDockVariable.Height);
        set => SetSizePart(OxDockVariable.Height, value);
    }

    private short GetSizePart(OxDockVariable variable)
    {
        short calcedValue = ZBounds.GetSizePart(variable);

        if (OxDockHelper.Variable(Dock).Equals(variable)
            && OxControl is not IOxDependedBox
            && OxControl is IOxWithMargin controlWithMargin)
            calcedValue -= controlWithMargin.Margin.ByDockVariable(variable);

        return calcedValue;
    }

    private void SetSizePart(OxDockVariable variable, short value)
    {
        if (Size.ByDockVariable(variable).Equals(value))
            return;

        OxSize oldSize = new(Size);
        short calcedValue = value;
        calcedValue = OxSh.Mul(calcedValue, OxControlHelper.DPIMultiplier(ManagingControl));

        if (Dock is not OxDock.None
            && OxControl is not IOxDependedBox
            && OxControl is IOxWithMargin controlWithMargin)
            calcedValue = OxSh.Add(calcedValue, controlWithMargin.Margin.ByDockVariable(variable));

        if (!ParentOuterControlZone.IsEmpty)
        {
            if (Dock is OxDock.Right)
                ZBounds.Left = OxSh.Sub(ParentOuterControlZone.Right, calcedValue);
            else
                if (Dock is OxDock.Bottom)
                ZBounds.Top = OxSh.Sub(ParentOuterControlZone.Bottom, calcedValue);

            calcedValue = OxSh.Min(
                calcedValue,
                OxSh.Sub(
                    ParentOuterControlZone.LastByDockVariable(variable),
                    Location.ByDockVariable(variable)
                )
            );
        }

        ZBounds.SetSizePart(variable, calcedValue);
        ZBounds.SaveSize(variable);
        OnSizeChanged(new(oldSize, Size));
    }

    public short Bottom =>
        OxSh.Sub(
            ManagingControl.Bottom,
            ParentOuterControlZone.Y
        );

    public short Right =>
        OxSh.Sub(
            ManagingControl.Right,
            ParentOuterControlZone.X
        );

    public short Top
    {
        get => GetLocationPart(OxDockVariable.Height);
        set => SetLocationPart(OxDockVariable.Height, value);
    }

    public short Left
    {
        get => GetLocationPart(OxDockVariable.Width);
        set => SetLocationPart(OxDockVariable.Width, value);
    }

    private short GetLocationPart(OxDockVariable variable) =>
        OxSh.Sub(
            ZBounds.GetLocationPart(variable),
            ParentInnerControlZone.FirstByDockVariable(variable)
        );

    private void SetLocationPart(OxDockVariable variable, short value)
    {
        if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
            return;

        OxPoint oldLocation = new(Location);
        ZBounds.SetLocationPart(variable, value);
        ZBounds.SaveLocation(variable);
        ZBounds.SetLocationPart(
            variable,
            OxSh.Add(
                value,
                ParentInnerControlZone.FirstByDockVariable(variable)
            )
        );
        OnLocationChanged(new(oldLocation, Location));
    }

    private readonly OxHandlers Handlers;

    public void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
        Handlers.Invoke(type, OxControl, args);

    public void AddHandler(OxHandlerType type, Delegate handler) =>
        Handlers.Add(type, handler);

    public void RemoveHandler(OxHandlerType type, Delegate handler) =>
        Handlers.Remove(type, handler);

    public event OxBoolChangedEvent AutoSizeChanged
    {
        add => AddHandler(OxHandlerType.AutoSizeChanged, value);
        remove => RemoveHandler(OxHandlerType.AutoSizeChanged, value);
    }

    public event OxDockChangedEvent DockChanged
    {
        add => AddHandler(OxHandlerType.DockChanged, value);
        remove => RemoveHandler(OxHandlerType.DockChanged, value);
    }

    public event OxBoolChangedEvent EnabledChanged
    {
        add => AddHandler(OxHandlerType.EnabledChanged, value);
        remove => RemoveHandler(OxHandlerType.EnabledChanged, value);
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

    public event OxBoolChangedEvent VisibleChanged
    {
        add => AddHandler(OxHandlerType.VisibleChanged, value);
        remove => RemoveHandler(OxHandlerType.VisibleChanged, value);
    }

    public OxDock Dock
    {
        get => ZBounds.Dock;
        set
        {
            if (OxControl is IOxDependedBox dependedBox)
            { 
                dependedBox.Dock = value;
                return;
            }

            if (ZBounds.Dock.Equals(value))
                return;

            OxDock oldDock = ZBounds.Dock;
            ZBounds.Dock = value;
            OnDockChanged(new(oldDock, ZBounds.Dock));
        }
    }

    private OxRectangle ParentOuterControlZone =>
        Parent is null
        || Parent is IOxDependedBox
            ? OxRectangle.Empty
            : Parent.OuterControlZone;

    private OxRectangle ParentInnerControlZone =>
        Parent is null
        || Parent is IOxDependedBox
            ? OxRectangle.Empty
            : Parent.InnerControlZone;

    protected OxBool ParentChanging = OxB.F;

    public IOxBox? Parent
    {
        get => (IOxBox?)ManagingControl.Parent;
        set
        {
            if (OxHelper.Equals(Parent, value))
                return;

            ParentChanging = OxB.T;
            try
            {
                IOxBox? oldParent = Parent;
                ManagingControl.Parent = (Control?)value;
                OnParentChanged(new(oldParent, Parent));
            }
            finally
            {
                ParentChanging = OxB.F;
            }
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

    private void OnDockChanged(OxDockChangedEventArgs e) => 
        ZBounds.WithoutSave(
            () =>
            {
                if (!e.IsChanged)
                    return;

                if (e.OldValue is OxDock.Fill)
                {
                    ZBounds.RestoreSize();

                    if (e.NewValue is OxDock.None)
                        ZBounds.RestoreLocation();
                }

                OxControl.OnDockChanged(e);
                InvokeHandlers(OxHandlerType.DockChanged, e);
                RealignParent();
            }
        );

    private void OnEnabledChanged(OxBoolChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        OxControl.OnEnabledChanged(e);
        InvokeHandlers(OxHandlerType.EnabledChanged, e);
    }

    private void OnLocationChanged(OxLocationChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        OxControl.OnLocationChanged(e);
        InvokeHandlers(OxHandlerType.LocationChanged, e);
    }

    private void OnParentChanged(OxParentChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        e.OldValue?.Realign();
        OxControl.OnParentChanged(e);
        InvokeHandlers(OxHandlerType.ParentChanged, e);
        e.NewValue?.Realign();
        PrepareColors();
    }

    private void PrepareColors()
    {
        if (OxControl is IOxWithColorHelper withColorHelper)
            withColorHelper.PrepareColors();
    }

    private void OnVisibleChanged(OxBoolChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        OxControl.OnVisibleChanged(e);
        InvokeHandlers(OxHandlerType.VisibleChanged, e);
        RealignParent();
    }

    private void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        if (OxControl is IOxDependedBox dependedBox)
        {
            dependedBox.Size = Size;
            dependedBox.Realign();
        }

        OxControl.OnSizeChanged(e);
        InvokeHandlers(OxHandlerType.SizeChanged, e);

        if (OxDockHelper.DockType(Dock) is OxDockType.Docked)
            RealignParent();
    }
}