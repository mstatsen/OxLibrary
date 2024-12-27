using OxLibrary.Forms;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Panels;

public class OxPanel : Panel, IOxPanel
{
    public IOxBoxManager Manager { get; }
    public OxPanel() : this(OxSize.Empty) { }
    public OxPanel(OxSize size)
    {
        Manager = OxControlManagers.RegisterBox(this);
        BorderVisible = OxB.F;
        Colors = new(DefaultColor);
        Initialized = false;

        WithSuspendedLayout(
            () =>
            {
                DoubleBuffered = true;

                if (!size.Equals(OxSize.Empty))
                    Size = size.ToDPI(this);

                PrepareInnerComponents();
                SetHandlers();
                AfterCreated();
            }
        );

        Initialized = true;
        Visible = OxBool.True;
    }

    protected bool IsVariableWidth =>
        Parent is null
        || OxDockHelper.IsVariableWidth(Dock);

    protected bool IsVariableHeight =>
        Parent is null
        || OxDockHelper.IsVariableHeight(Dock);


    private bool useDisabledStyles = true;

    public bool UseDisabledStyles
    {
        get => useDisabledStyles;
        set => SetUseDisabledStyles(value);
    }

    protected virtual void SetUseDisabledStyles(bool value) =>
        useDisabledStyles = value;

    protected virtual void PrepareInnerComponents() { }

    protected virtual OxRectangle BorderRectangle =>
        new OxRectangle(ClientRectangle) - Margin;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!IsBlurredBorder)
            Margin.Draw(e.Graphics, new(ClientRectangle), MarginColor);

        Borders.Draw(e.Graphics, BorderRectangle, BorderColor);
    }

    protected virtual void SetHandlers() { }

    protected virtual void AfterCreated() { }

    protected bool Initialized { get; set; } = false;

    public virtual Color DefaultColor => Color.FromArgb(142, 142, 138);

    public new string Text
    {
        get
        {
            string text = GetText();
            return text is null ? string.Empty : text;
        }
        set => SetText(value is null ? string.Empty : value);
    }

    protected virtual void SetText(string value) =>
        base.Text = value;

    protected virtual string GetText() =>
        base.Text;

    public OxBool Hovered
    {
        get => OxB.B(IsHovered);
        set => SetHovered(OxB.B(value));
    }

    public virtual bool IsHovered
    {
        get
        {
            Point thisPoint = PointToClient(Cursor.Position);
            return 
                (thisPoint.X >= 0)
                && (thisPoint.X <= Size.Width)
                && (thisPoint.Y >= 0)
                && (thisPoint.Y <= Size.Height
            );
        }
    }

    public virtual void SetHovered(bool value) { }

    protected readonly ToolTip ToolTip =
        new()
        {
            AutomaticDelay = 500,
            InitialDelay = 100,
            ShowAlways = true
        };

    public string ToolTipText
    {
        get => ToolTip.GetToolTip(this);
        set => SetToolTipText(value);
    }

    protected virtual void SetToolTipText(string value) =>
        ToolTip.SetToolTip(this, value);

    public virtual void PutBack(OxPanelViewer viewer)
    {
        foreach (Form form in viewer.OwnedForms)
            viewer.RemoveOwnedForm(form);

        Initialized = false;
        Initialized = true;
    }

    public DialogResult ShowDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK)
    {
        DialogResult result = AsDialog(buttons).ShowDialog((IWin32Window)owner);
        PanelViewer?.Dispose();
        PanelViewer = null;
        return result;
    }

    public bool ShowDialogIsOK(Control owner, OxDialogButton buttons = OxDialogButton.OK) =>
        ShowDialog(owner, buttons).Equals(DialogResult.OK);

    protected OxPanelViewer? PanelViewer;

    protected OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK)
    {
        PrepareDialogCaption(out string? dialogCaption);
        PanelViewer = new OxPanelViewer(this, buttons)
        {
            Text = OxTextHelper.ToString(dialogCaption)
        };
        PanelViewer.ButtonsWithBorders.Clear();
        PrepareDialog(PanelViewer);
        return PanelViewer;
    }

    protected virtual void PrepareDialogCaption(out string? dialogCaption) =>
        dialogCaption = Text;

    protected virtual void PrepareDialog(OxPanelViewer dialog) { }

    public new virtual IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

    public new OxBool AutoSize
    {
        get => Manager.AutoSize;
        set => Manager.AutoSize = value;
    }

    public bool IsAutoSize =>
        Manager.IsAutoSize;

    public void SetAutoSize(bool value) =>
        Manager.SetAutoSize(value);

    #region IOxWithPadding implementaion
    private readonly OxBorders padding = new();
    public new OxBorders Padding => padding;
    #endregion

    #region IOxWithBorder implementation
    private readonly OxBorders borders =
        new()
        {
            Size = 1
        };

    public OxBorders Borders => borders;

    private bool useDefaultBorderColor = true;
    public bool UseDefaultBorderColor
    {
        get => useDefaultBorderColor;
        set
        {
            useDefaultBorderColor = value;
            Invalidate();
        }
    }

    private Color borderColor = Color.Transparent;
    public Color BorderColor
    {
        get =>
            useDefaultBorderColor
            ? GetBorderColor()
            : borderColor;
        set
        {
            useDefaultBorderColor = false;
            borderColor = value;
            Invalidate();
        }
    }

    public void SetBorderWidth(short value) =>
        Borders.Size = value;

    public void SetBorderWidth(OxDock dock, short value) =>
        Borders[dock].Size = value;

    public OxBool BorderVisible
    {
        get => GetBorderVisible();
        set => SetBorderVisible(value);
    }

    protected void SetBorderVisible(OxBool value) =>
        Borders.SetVisible(value);

    protected virtual OxBool GetBorderVisible() =>
        Borders.IsVisible;

    public bool IsBorderVisible => OxB.B(BorderVisible);
    public void SetBorderVisible(bool value) => BorderVisible = OxB.B(value);

    protected virtual Color GetBorderColor() =>
        IsEnabled
        || !UseDisabledStyles
            ? BaseColor
            : Colors.Lighter(2);
    #endregion

    #region IOxWithMargin implementation
    private readonly OxBorders margin = new();

    public new OxBorders Margin => margin;

    private OxBool blurredBorder = OxB.F;
    public OxBool BlurredBorder
    {
        get => blurredBorder;
        set
        {
            blurredBorder = value;

            if (!margin.IsEmpty)
                Invalidate();
        }
    }

    private Color MarginColor =>
        !IsBlurredBorder
        && Parent is not null
            ? Parent.BackColor
            : BackColor;
    #endregion

    #region IOxWithColorHelper implementation
    public OxColorHelper Colors { get; }
    public Color BaseColor
    {
        get => Colors.BaseColor;
        set
        {
            if (Colors.BaseColor.Equals(value)
                || BaseColorChanging)
                return;

            BaseColorChanging = true;

            try
            {
                Colors.BaseColor = value;
                PrepareColors();
            }
            finally
            {
                BaseColorChanging = false;
                Invalidate();
            }
        }
    }

    private bool BaseColorChanging = false;

    protected virtual Color GetBackColor() =>
        Colors.Lighter(IsEnabled || !useDisabledStyles ? 7 : 8);

    protected virtual Color GetForeColor() =>
        Colors.Darker(IsEnabled || !useDisabledStyles ? 7 : -3);

    public virtual void PrepareColors()
    {
        BackColor = GetBackColor();
        ForeColor = GetForeColor();
        ColorizeControls();
    }

    private void ColorizeControls()
    {
        foreach (IOxControl control in OxControls)
        {
            if (control is IOxPanel panel)
                panel.BaseColor = BaseColor;
            else control.BackColor = BackColor;
        }
    }
    #endregion

    #region IWithIcon implementation
    public Bitmap? Icon
    {
        get => GetIcon();
        set
        {
            if (!OxHelper.Changed(Icon, value))
                return;

            SetIcon(value);
        }
    }

    protected virtual void SetIcon(Bitmap? value) { }
    protected virtual Bitmap? GetIcon() => null;
    #endregion

    #region Implemention of IOxBox using IOxBoxManager
    public virtual OxBool HandleParentPadding => OxB.T;

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

    public virtual void OnAutoSizeChanged(OxBoolChangedEventArgs e) { }
    public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
    public virtual void OnEnabledChanged(OxBoolChangedEventArgs e) { }
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
    public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
    public virtual void OnVisibleChanged(OxBoolChangedEventArgs e) { }

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

    public new OxBool Visible
    {
        get => Manager.Visible;
        set => Manager.Visible = value;
    }

    public bool IsVisible =>
        Manager.IsVisible;

    public void SetVisible(bool value) =>
        Manager.SetVisible(value);

    public new OxBool Enabled
    {
        get => Manager.Enabled;
        set => Manager.Enabled = value;
    }
    public bool IsEnabled =>
        Manager.IsEnabled;

    public void SetEnabled(bool value) =>
        Manager.SetEnabled(value);

    public bool IsBlurredBorder =>
        OxB.B(BlurredBorder);

    public void SetBlurredBorder(bool value) => BlurredBorder = OxB.B(value);

    public void WithSuspendedLayout(Action method) =>
        Manager.WithSuspendedLayout(method);

    public new event OxBoolChangedEvent AutoSizeChanged
    {
        add => Manager.AutoSizeChanged += value;
        remove => Manager.AutoSizeChanged -= value;
    }

    public new event OxDockChangedEvent DockChanged
    {
        add => Manager.DockChanged += value;
        remove => Manager.DockChanged -= value;
    }

    public new event OxBoolChangedEvent EnabledChanged
    {
        add => Manager.EnabledChanged += value;
        remove => Manager.EnabledChanged -= value;
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

    public new event OxBoolChangedEvent VisibleChanged
    {
        add => Manager.VisibleChanged += value;
        remove => Manager.VisibleChanged -= value;
    }

    public void AddHandler(OxHandlerType type, Delegate handler) =>
        Manager.AddHandler(type, handler);

    public void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
        Manager.InvokeHandlers(type, args);

    public void RemoveHandler(OxHandlerType type, Delegate handler) =>
        Manager.RemoveHandler(type, handler);

    public short ToDPI(int size) => OxSh.ToDPI(this, size);

    #region Internal used properties and methods
    [Obsolete("ZBounds it is used only for internal needs")]
    public OxZBounds ZBounds =>
        Manager.ZBounds;
    #endregion

    #endregion

    #region Hidden base methods
    protected sealed override void OnAutoSizeChanged(EventArgs e) { }
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnEnabledChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    protected sealed override void OnVisibleChanged(EventArgs e) { }
    #endregion
}