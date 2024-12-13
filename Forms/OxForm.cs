using OxLibrary.Controls;
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
    public OxHeader Header =>
        FormPanel.Header;

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

    public void MoveToScreenCenter() => 
        OxMover.MoveToCenter(this);

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
            {
                OnSizeChanged(new(oldSize, Size));
                ApplyRestoreButtonIconAndToolTip();
            }
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

    private OxBool canMaximize = OxB.T;
    private OxBool canMinimize = OxB.T;

    public OxBool CanMaximize
    {
        get => canMaximize;
        set
        {
            if (canMaximize.Equals(value))
                return;

            canMaximize = value;
            RestoreButton.Visible = CanMaximize;
            HeaderToolBar.PlaceButtons();
        }
    }

    public bool IsCanMaximize =>
        OxB.B(CanMaximize);

    public void SetCanMaximize(bool value) =>
        CanMaximize = OxB.B(value);

    public void ApplyRestoreButtonIconAndToolTip()
    {
        if (RestoreButton.Icon is null
            || !RestoreButton.Icon.Equals(RestoreIcon))
            RestoreButton.Icon = RestoreIcon;

        RestoreButton.ToolTipText = RestoreToolTip;
    }

    private Bitmap RestoreIcon =>
        WindowState is FormWindowState.Maximized
            ? OxIcons.Restore
            : OxIcons.Maximize;

    private string RestoreToolTip =>
        WindowState is FormWindowState.Maximized
            ? "Restore window"
            : "Maximize window";

    public OxBool CanMinimize
    {
        get => canMinimize;
        set
        {
            if (canMinimize.Equals(value))
                return;

            canMinimize = value;
            MinimizeButton.Visible = CanMinimize;
            HeaderToolBar.PlaceButtons();
        }
    }

    public bool IsCanMinimize =>
       OxB.B(CanMinimize);
    public void SetCanMinimize(bool value) =>
        CanMinimize = OxB.B(value);


    private OxBool sizable = OxB.T;

    public OxBool Sizable
    {
        get => sizable;
        set
        {
            sizable = value;
            SetMargins();
        }
    }

    public bool IsSizable =>
        OxB.B(Sizable);
    public void SetSizable(bool value) =>
        Sizable = OxB.B(value);

    private void SetMargins() =>
        Margin.Size = OxSh.Short(IsSizable ? 1 : 0);

    protected override void OnShown(EventArgs e)
    {
        Realign();
        base.OnShown(e);
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
    public OxBool HandleParentPadding => OxB.F;

    public OxRectangle InnerControlZone =>
        Manager.InnerControlZone;

    public OxRectangle OuterControlZone =>
        Manager.OuterControlZone;

    public OxControls OxControls =>
        Manager.OxControls;

    public void Realign() =>
        Manager.Realign();

    public OxBool Realigning =>
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

    public void WithSuspendedLayout(Action method) =>
        Manager.WithSuspendedLayout(method);

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

    public new event OxBoolChangedEvent AutoSizeChanged
    {
        add => Manager.AutoSizeChanged += value;
        remove => Manager.AutoSizeChanged -= value;
    }

    public new event OxBoolChangedEvent EnabledChanged
    {
        add => Manager.EnabledChanged += value;
        remove => Manager.EnabledChanged -= value;
    }

    public new event OxBoolChangedEvent VisibleChanged
    {
        add => Manager.VisibleChanged += value;
        remove => Manager.VisibleChanged -= value;
    }

    #region Internal used properties and methods
    [Obsolete("ZBounds it is used only for internal needs")]
    public OxZBounds ZBounds =>
        Manager.ZBounds;
    #endregion

    #endregion

    #region Hidden base methods
    public new OxBorders Padding =>
        FormPanel.Padding;
            
    public OxBool HeaderVisible
    {
        get => FormPanel.HeaderVisible;
        set => FormPanel.HeaderVisible = value;
    }

    public bool IsHeaderVisible =>
        FormPanel.IsHeaderVisible;

    public void SetHeaderVisible(bool value) =>
        FormPanel.SetHeaderVisible(value);

    public Color DefaultColor =>
        FormPanel.DefaultColor;

    public OxBool Hovered =>
        FormPanel.Hovered;

    public new OxBorders Margin =>
        FormPanel.Margin;

    public OxBool BlurredBorder
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
    public OxBool BorderVisible
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

    public bool IsHovered =>
        FormPanel.IsHovered;

    public new OxBool AutoSize 
    {
        get => FormPanel.AutoSize;
        set => FormPanel.AutoSize = value;
    }

    public bool IsAutoSize =>
        FormPanel.IsAutoSize;

    public new OxBool Enabled
    {
        get => FormPanel.Enabled;
        set => FormPanel.Enabled = value;
    }

    public bool IsEnabled =>
        FormPanel.IsEnabled;

    public new OxBool Visible
    {
        get => FormPanel.Visible;
        set => FormPanel.Visible = value;
    }

    public bool IsVisible =>
        FormPanel.IsVisible;

    public bool IsBlurredBorder =>
        FormPanel.IsBlurredBorder;

    public bool IsBorderVisible =>
        FormPanel.IsBorderVisible;

    protected sealed override void OnAutoSizeChanged(EventArgs e) { }
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnEnabledChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    protected sealed override void OnVisibleChanged(EventArgs e) { }

    public DialogResult ShowDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
        FormPanel.ShowDialog(owner, buttons);

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

    public void SetHovered(bool value) =>
        FormPanel.SetHovered(value);

    public void OnAutoSizeChanged(OxBoolChangedEventArgs e) =>
        FormPanel.OnAutoSizeChanged(e);

    public void OnEnabledChanged(OxBoolChangedEventArgs e) =>
        FormPanel.OnEnabledChanged(e);

    public void OnVisibleChanged(OxBoolChangedEventArgs e) =>
        FormPanel.OnVisibleChanged(e);

    public void SetAutoSize(bool value) =>
        FormPanel.SetAutoSize(value);

    public void SetEnabled(bool value) =>
        FormPanel.SetEnabled(value);

    public void SetVisible(bool value) =>
        FormPanel.SetVisible(value);

    public void SetBlurredBorder(bool value) =>
        FormPanel.SetBlurredBorder(value);

    public void SetBorderVisible(bool value) =>
        FormPanel.SetBorderVisible(value);
    #endregion
}

public class OxForm : OxForm<OxFormPanel>
{ 
}