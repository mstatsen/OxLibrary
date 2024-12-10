using OxLibrary.Controls;
using OxLibrary.ControlsManaging;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public class OxForm<TFormPanel> :
    Form,
    IOxForm<TFormPanel>,
    IOxDependedBox
    where TFormPanel: IOxFormPanel, new()
    
{
    public TFormPanel FormPanel { get; internal set; }
    public OxHeader Header => FormPanel.Header;
    public short HeaderHeight
    {
        get => FormPanel.HeaderHeight;
        set => FormPanel.HeaderHeight = value;
    }

    public OxHeaderToolBar HeaderToolBar =>
        FormPanel.HeaderToolBar;

    public IOxBoxManager Manager { get; }

    public OxForm()
    {
        DoubleBuffered = true;
        Manager = OxControlManagers.RegisterBox(this);
#pragma warning disable IDE0017 // Simplify object initialization
        FormPanel = new();
#pragma warning restore IDE0017 // Simplify object initialization
        FormPanel.Form = this;
        PrepareFormPanel();
        SetUpForm();
        PlaceFormPanel();
    }

    private void PrepareFormPanel()
    {
        SetMargins();
        CloseButton.Click += CloseButtonClickHandler;
        RestoreButton.Click += RestoreButtonClickHandler;
        MinimizeButton.Click += MinimizeButtonClickHandler;
        Colors.BaseColorChanged += BaseColorChangedHandler;
    }

    private void CloseButtonClickHandler(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void RestoreButtonClickHandler(object? sender, EventArgs e) =>
        SetState(WindowState is FormWindowState.Maximized
            ? FormWindowState.Normal
            : FormWindowState.Maximized);

    private void MinimizeButtonClickHandler(object? sender, EventArgs e) =>
        SetState(FormWindowState.Minimized);

    private void BaseColorChangedHandler(object? sender, EventArgs e) =>
        PrepareColors();

    public void MoveToScreenCenter()
    {
        Screen screen = Screen.FromControl(this);
        Location = new(
            OxSH.Add(
                screen.Bounds.Left,
                OxSH.Half(screen.WorkingArea.Width - Width)
            ),
            OxSH.Add(
                screen.Bounds.Top,
                OxSH.Half(screen.WorkingArea.Height - Height)
            )
        );
        Size = new(Width, Height);
    }

    protected virtual void SetUpForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        SetState(WindowState);
        StartPosition = FormStartPosition.CenterParent;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplyFormIcon();
    }

    public void ApplyFormIcon()
    {
        Header.Icon = FormIcon;

        if (FormIcon is not null)
            Icon = FormIcon;
    }

    public void SetState(FormWindowState state)
    {
        MaximumSize = OxControlHelper.ScreenSize(this);
        OxSize wantedMinimumSize = WantedMinimumSize;
        MinimumSize = new(
            Math.Min(wantedMinimumSize.Width, MaximumSize.Width),
            Math.Min(wantedMinimumSize.Height, MaximumSize.Height)
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

            OxSize oldSize = new(Size);
            base.WindowState = value;

            if (value is not FormWindowState.Minimized)
                OnSizeChanged(new(oldSize, Size));
        }
    }

    public virtual OxSize WantedMinimumSize =>
        new(640, 480);

    private void PlaceFormPanel()
    {
        FormPanel.Parent = this;
        FormPanel.Location = new(0, 0);
        FormPanel.Size = new(Width, Height);
    }

    private bool canMaximize = true;
    private bool canMinimize = true;

    public bool CanMaximize
    {
        get => canMaximize;
        set
        {
            canMaximize = value;
            RestoreButton.Visible = CanMaximize;
        }
    }

    public bool CanMinimize
    {
        get => canMinimize;
        set
        {
            canMinimize = value;
            MinimizeButton.Visible = CanMinimize;
        }
    }

    private bool sizable = true;

    public bool Sizable
    {
        get => sizable;
        set
        {
            sizable = value;
            SetMargins();
        }
    }

    private void SetMargins() =>
        Margin.Size = OxSH.IfElseZero(Sizable, 1);

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
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
        if (FormPanel is null)
            return;

        MinimumSize = FormPanel.Size;
        MaximumSize = MinimumSize;
    }

    public Color BaseColor
    {
        get => FormPanel.BaseColor;
        set => FormPanel.BaseColor = value;
    }

    public new Color BackColor
    { 
        get => FormPanel.BackColor;
        set => FormPanel.BackColor = value;
    }

    public new string Text
    { 
        get => FormPanel.Text;
        set => FormPanel.Text = value;
    }

    public OxColorHelper Colors => FormPanel.Colors;

    public virtual void PrepareColors() { }

    #region Implemention of IOxBox using OxFormPanel
    public bool HandleParentPadding => false;

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
    public void OnDockChanged(OxDockChangedEventArgs e) =>
        FormPanel.OnDockChanged(e);

    public void OnLocationChanged(OxLocationChangedEventArgs e) =>
        FormPanel.OnLocationChanged(e);

    public void OnParentChanged(OxParentChangedEventArgs e) =>
        FormPanel.OnParentChanged(e);

    public void OnSizeChanged(OxSizeChangedEventArgs e) { }

    public new IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

    public new short Width
    {
        get => Manager.Width;
        set => Manager.Width = value;
    }

    public new short Height
    {
        get => Manager.Height;
        set => Manager.Height = value;
    }

    public new short Top
    {
        get => Manager.Top;
        set => Manager.Top = value;
    }

    public new short Left
    {
        get => Manager.Left;
        set => Manager.Left = value;
    }

    public new short Bottom =>
        Manager.Bottom;

    public new short Right =>
        Manager.Right;

    public new OxSize Size
    {
        get => Manager.Size;
        set => Manager.Size = value;
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

    public void DoWithSuspendedLayout(Action method) =>
        Manager.DoWithSuspendedLayout(method);

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
    private new Size PreferredSize =>
        base.PreferredSize;

    private new Rectangle DisplayRectangle =>
        base.DisplayRectangle;

    public new OxBorders Padding =>
        FormPanel.Padding;
            
    public bool HeaderVisible
    {
        get => FormPanel.HeaderVisible;
        set => FormPanel.HeaderVisible = value;
    }

    public Color DefaultColor =>
        FormPanel.DefaultColor;

    public bool IsHovered =>
        FormPanel.IsHovered;

    public new OxBorders Margin =>
        FormPanel.Margin;

    public bool BlurredBorder
    { 
        get => FormPanel.BlurredBorder;
        set => FormPanel.BlurredBorder = value;
    }

    public OxBorders Borders =>
        FormPanel.Borders;

    public Color BorderColor
    { 
        get => FormPanel.BorderColor;
        set => FormPanel.BorderColor = value;
    }
    public bool BorderVisible
    { 
        get => FormPanel.BorderVisible;
        set => FormPanel.BorderVisible = value;
    }

    public OxIconButton CloseButton =>
        FormPanel.CloseButton;

    public OxIconButton RestoreButton =>
        FormPanel.RestoreButton;

    public OxIconButton MinimizeButton =>
        FormPanel.MinimizeButton;

    public new Bitmap? Icon
    {
        get => base.Icon.ToBitmap();
        set => base.Icon =
            value is null
                ? null
                : System.Drawing.Icon.FromHandle(value.GetHicon());
    }

    public IOxBox DependedFrom =>
        FormPanel;

    private new Size GetPreferredSize(Size proposedSize) =>
        base.GetPreferredSize(proposedSize);

    private new Size LogicalToDeviceUnits(Size value) =>
        base.LogicalToDeviceUnits(value);
#pragma warning restore IDE0051 // Remove unused private members
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }

    public OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK) =>
        FormPanel.AsDialog(buttons);

    public DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
        FormPanel.ShowAsDialog(owner, buttons);

    public void SetBorderWidth(short value) =>
        FormPanel.SetBorderWidth(value);

    public void SetBorderWidth(OxDock dock, short value) =>
        FormPanel.SetBorderWidth(dock, value);

    public void AddHandler(OxHandlerType type, Delegate handler) =>
        Manager.AddHandler(type, handler);

    public void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
        Manager.InvokeHandlers(type, args);

    public void RemoveHandler(OxHandlerType type, Delegate handler) =>
        Manager.RemoveHandler(type, handler);
    #endregion
}

public class OxForm : OxForm<OxFormPanel>
{ 
}