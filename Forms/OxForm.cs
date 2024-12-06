using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Forms;

public class OxForm : Form,
    IOxBox<OxForm>,
    IOxWithColorHelper
{
    private readonly bool Initialized = false;
    public OxFormMainPanel MainPanel { get; internal set; }

    public IOxBoxManager Manager { get; }

    public OxForm()
    {
        Initialized = false;

        try
        {
            DoubleBuffered = true;
            Manager = OxControlManagers.RegisterBox(this);
            MainPanel = CreateMainPanel();
            MainPanel.Colors.BaseColorChanged += BaseColorChangedHandler;
            SetUpForm();
            PlaceMainPanel();
        }
        finally
        {
            Initialized = true;
        }
    }

    private void BaseColorChangedHandler(object? sender, EventArgs e) =>
        PrepareColors();

    public void MoveToScreenCenter()
    {
        Screen screen = Screen.FromControl(this);
        Location = new(
            OxWh.Add(
                OxWh.W(screen.Bounds.Left),
                OxWh.Div(
                    OxWh.Sub(screen.WorkingArea.Width, Width),
                    OxWh.W2
                )
            ),
            OxWh.Add(
                OxWh.W(screen.Bounds.Top),
                OxWh.Div(
                    OxWh.Sub(screen.WorkingArea.Height, Height),
                    OxWh.W2
                )
            )
        );
        Size = new(
            OxWh.I(Width),
            OxWh.I(Height)
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
        Realign();
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
            MainPanel.SetHeaderButtonsVisible();
        }
    }

    public bool CanMinimize
    {
        get => canMinimize;
        set
        {
            canMinimize = value;
            MainPanel.SetHeaderButtonsVisible();
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

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        //MainPanel.Location = new(OxPoint.Empty);
        //MainPanel.Size = new(OxWh.W0, OxWh.W0);
        Realign();
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

    public OxColorHelper Colors => MainPanel.Colors;

    public virtual void PrepareColors() { }

    #region Implemention of IOxBox using IOxBoxManager
    public virtual bool HandleParentPadding => false;
    public OxRectangle InnerControlZone =>
        Manager.InnerControlZone;

    public OxRectangle OuterControlZone =>
        Manager.OuterControlZone;

    public OxControls OxControls =>
        Manager.OxControls;

    public void Realign() =>
        Manager.Realign();

    public bool Realigning =>
        Manager.Realigning;
    #endregion

    #region Implemention of IOxControl using IOxControlManager
    public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
    public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
    public virtual void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!Initialized ||
            !e.Changed)
            return;

        MainPanel.Size = Size;
        Realign();
    }

    public new IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

    public OxPoint PointToScreen(OxPoint p) =>
        Manager.PointToScreen(p);

    public new OxWidth Width
    {
        get => Manager.Width;
        set => Manager.Width = value;
    }

    public new OxWidth Height
    {
        get => Manager.Height;
        set => Manager.Height = value;
    }

    public new OxWidth Top
    {
        get => Manager.Top;
        set => Manager.Top = value;
    }

    public new OxWidth Left
    {
        get => Manager.Left;
        set => Manager.Left = value;
    }

    public new OxWidth Bottom =>
        Manager.Bottom;

    public new OxWidth Right =>
        Manager.Right;

    public new OxSize Size
    {
        get => Manager.Size;
        set => Manager.Size = value;
    }

    public new OxSize ClientSize
    {
        get => Manager.ClientSize;
        set => Manager.ClientSize = value;
    }

    public new OxPoint Location
    {
        get => Manager.Location;
        set => Manager.Location = value;
    }

    public new OxSize MinimumSize
    {
        get => Manager.MinimumSize;
        set => Manager.MinimumSize = value;
    }

    public new OxSize MaximumSize
    {
        get => Manager.MaximumSize;
        set => Manager.MaximumSize = value;
    }

    public new virtual OxDock Dock
    {
        get => Manager.Dock;
        set => Manager.Dock = value;
    }

    public new OxRectangle ClientRectangle =>
        Manager.ClientRectangle;

    public new OxRectangle Bounds
    {
        get => Manager.Bounds;
        set => Manager.Bounds = value;
    }

    public new OxPoint AutoScrollOffset
    {
        get => Manager.AutoScrollOffset;
        set => Manager.AutoScrollOffset = value;
    }

    public void DoWithSuspendedLayout(Action method) =>
        Manager.DoWithSuspendedLayout(method);

    public Control GetChildAtPoint(OxPoint pt) =>
        Manager.GetChildAtPoint(pt);

    public void Invalidate(OxRectangle rc) =>
        Manager.Invalidate(rc);

    public OxPoint PointToClient(OxPoint p) =>
        Manager.PointToClient(p);

    public OxRectangle RectangleToClient(OxRectangle r) =>
        Manager.RectangleToClient(r);

    public OxRectangle RectangleToScreen(OxRectangle r) =>
        Manager.RectangleToScreen(r);

    public new event OxDockChangedEvent DockChanged
    {
        add => Manager.DockChanged += value;
        remove => Manager.DockChanged -= value;
    }

    public new event OxLocationChangedEvent LocationChanged
    {
        add => Manager.LocationChanged += value;
        remove => Manager.LocationChanged -= value;
    }

    public new event OxParentChangedEvent ParentChanged
    {
        add => Manager.ParentChanged += value;
        remove => Manager.ParentChanged -= value;
    }

    public new event OxSizeChangedEvent SizeChanged
    {
        add => Manager.SizeChanged += value;
        remove => Manager.SizeChanged -= value;
    }

    #region Internal used properties and methods
    [Obsolete("ZBounds it is used only for internal needs")]
    public OxZBounds ZBounds =>
        Manager.ZBounds;
    #endregion

    #endregion

    #region Hidden base methods
#pragma warning disable IDE0051 // Remove unused private members
    private new void SetBounds(int x, int y, int width, int height) =>
        base.SetBounds(x, y, width, height);

    private new Size PreferredSize =>
        base.PreferredSize;

    private new Rectangle DisplayRectangle =>
        base.DisplayRectangle;

    private new Size GetPreferredSize(Size proposedSize) =>
        base.GetPreferredSize(proposedSize);

    private new Size LogicalToDeviceUnits(Size value) =>
        base.LogicalToDeviceUnits(value);
    private new void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) =>
        base.SetBounds(x, y, width, height, specified);
    private new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue) =>
        base.GetChildAtPoint(pt, skipValue);

#pragma warning disable IDE0060 // Remove unused parameter
    private new void Invalidate(Rectangle rc, bool invalidateChildren) =>
        Invalidate(true);
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    #endregion
}