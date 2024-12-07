using OxLibrary.Controls;
using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public class OxForm<TForm, TFormPanel>:
    Form,
    IOxForm<TForm, TFormPanel>,
    IOxDependedBox
    where TForm : IOxForm<TForm, TFormPanel>
    where TFormPanel: IOxFormPanel<TForm, TFormPanel>, new()
    
{
    private readonly bool Initialized = false;
    public TFormPanel FormPanel { get; internal set; }
    public OxHeader Header => FormPanel.Header;
    public OxWidth HeaderHeight
    {
        get => FormPanel.HeaderHeight;
        set => FormPanel.HeaderHeight = value;
    }

    public IOxBoxManager Manager { get; }

    public OxForm()
    {
        Initialized = false;

        try
        {
            DoubleBuffered = true;
            Manager = OxControlManagers.RegisterBox(this);
            FormPanel = new()
            {
             //   Form = this
            };
            PrepareFormPanel();
            SetUpForm();
            PlaceFormPanel();
        }
        finally
        {
            Initialized = true;
        }
    }

    private void PrepareFormPanel()
    {
        SetFormPanelMargins();
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

    private void PlaceFormPanel()
    {
        FormPanel.Parent = this;
        FormPanel.Location = new(OxWh.W0, OxWh.W0);
        FormPanel.Size = new(Width, Height);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        if (e.Control is not IOxControl oxControl)
            return;
             
        if (Initialized)
            oxControl.Parent = FormPanel;
        else base.OnControlAdded(e);
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
            SetFormPanelMargins();
        }
    }

    private void SetFormPanelMargins()
    {
        Margin.Size =
            Sizable
                ? OxWh.W2
                : OxWh.W0;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        //FormPanel.Location = new(OxPoint.Empty);
        //FormPanel.Size = new(OxWh.W0, OxWh.W0);
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

    public new void SuspendLayout()
    { 
        base.SuspendLayout();
        FormPanel.SuspendLayout();
    }

    public new void ResumeLayout()
    {
        FormPanel.ResumeLayout();
        base.ResumeLayout();
    }

    public OxColorHelper Colors => FormPanel.Colors;

    public virtual void PrepareColors() { }

    #region Implemention of IOxBox using OxFormPanel
    public bool HandleParentPadding => false;
    public OxRectangle InnerControlZone =>
        ClientRectangle;

    public OxRectangle OuterControlZone =>
        ClientRectangle;

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

    public void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!Initialized ||
            !e.Changed)
            return;

        FormPanel.Size = Size;
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
        get => FormPanel.Dock;
        set => FormPanel.Dock = value;
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
        add => FormPanel.DockChanged += value;
        remove => FormPanel.DockChanged -= value;
    }

    public new event OxLocationChangedEvent LocationChanged
    {
        add => FormPanel.LocationChanged += value;
        remove => FormPanel.LocationChanged -= value;
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

    public OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK) =>
        FormPanel.AsDialog(buttons);

    public DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
        FormPanel.ShowAsDialog(owner, buttons);

    public void SetBorderWidth(OxWidth value) =>
        FormPanel.SetBorderWidth(value);

    public void SetBorderWidth(OxDock dock, OxWidth value) =>
        FormPanel.SetBorderWidth(dock, value);
    #endregion
}

public class OxForm : OxForm<OxForm, OxFormPanel>
{ 
}