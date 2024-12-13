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

    private void BordersSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
        RealignParent();

    private void MarginSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
    {
        if (!e.Changed)
            return;

        if (OxDockHelper.IsVariableWidth(Dock))
            Width = OxSH.Sub(ZBounds.Width, e.OldValue.Horizontal);

        if (OxDockHelper.IsVariableHeight(Dock))
            Height = OxSH.Sub(ZBounds.Height, e.OldValue.Vertical);

        RealignParent();
    }

    protected virtual void SetHandlers()
    {
        OxControl.VisibleChanged += VisibleChangedHandler;
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

    private void VisibleChangedHandler(object? sender, EventArgs e) =>
        RealignParent();

    protected virtual void RealignParent() =>
        Parent?.Realign();

    private void ControlDisposedHandler(object? sender, EventArgs e) =>
        OxControlManagers.UnRegisterControl(ManagingControl);

    public void WithSuspendedLayout(Action method)
    {
        ManagingControl.SuspendLayout();
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
            ManagingControl.ResumeLayout();
        }
    }

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

        if (Dock is not OxDock.None
            && OxControl is not IOxDependedBox
            && OxControl is IOxWithMargin controlWithMargin)
            calcedValue = OxSH.Add(value, controlWithMargin.Margin.ByDockVariable(variable));

        if (!ParentOuterControlZone.IsEmpty)
        {
            if (Dock is OxDock.Right)
                ZBounds.Left = OxSH.Sub(ParentOuterControlZone.Right, calcedValue);
            else
                if (Dock is OxDock.Bottom)
                ZBounds.Top = OxSH.Sub(ParentOuterControlZone.Bottom, calcedValue);

            calcedValue = OxSH.Min(
                calcedValue,
                OxSH.Sub(
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
        OxSH.Sub(
            ManagingControl.Bottom,
            ParentOuterControlZone.Y
        );

    public short Right =>
        OxSH.Sub(
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
        OxSH.Sub(
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
            OxSH.Add(
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

    public IOxBox? Parent
    {
        get => (IOxBox?)ManagingControl.Parent;
        set
        {
            if (value is null && Parent is null
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
                Width = (value.Width);

            if (!Height.Equals(value.Height))
                Height = (value.Height);
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

    private void OnDockChanged(OxDockChangedEventArgs e) => 
        ZBounds.WithoutSave(
            () =>
            {
                if (!e.Changed)
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

    internal void OnLocationChanged(OxLocationChangedEventArgs e)
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

    internal void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.Changed)
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