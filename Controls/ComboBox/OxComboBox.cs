using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls;

public class OxComboBox<T> : ComboBox, IOxControlWithManager
{
    public IOxControlManager Manager { get; }
    public OxComboBox()
    {
        Manager = OxControlManagers.RegisterControl(this);
        DoubleBuffered = true;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawFixed;
    }

    private readonly ToolTip ToolTip = new()
    {
        AutomaticDelay = 500,
        ShowAlways = true
    };

    public event GetToolTip<T>? GetToolTip;

    protected virtual void OnGetToolTip(T item, out string toolTipTitle, out string toolTipText)
    { 
        toolTipTitle = string.Empty;
        toolTipText = string.Empty;
        GetToolTip?.Invoke(item, out toolTipTitle, out toolTipText);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        ToolTip.Hide(this);
    }

    protected override void OnDropDownClosed(EventArgs e)
    {
        base.OnDropDownClosed(e);
        ToolTip.Hide(this);
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedItemChanged(e);

        string toolTipTitle = string.Empty;
        string toolTipText = string.Empty;

        if (SelectedItem is not null)
            OnGetToolTip(SelectedItem, out toolTipTitle, out toolTipText);

        ToolTip.ToolTipTitle = toolTipTitle;
        ToolTip.SetToolTip(this, toolTipText);
    }

    protected bool DrawStrings { get; set; } = true;

    private readonly List<T> tItems = new();

    public new T? SelectedItem
    {
        get => (T?)base.SelectedItem;
        set => base.SelectedItem = value;
    }

    public object? SelectedItemObject
    {
        get => base.SelectedItem;
        set => base.SelectedItem = value;
    }

    public List<T> TItems
    {
        get
        {
            tItems.Clear();

            foreach (object item in Items)
                tItems.Add((T)item);

            return tItems;
        }
    }

    protected virtual Point GetTextStartPosition(Rectangle bounds) => 
        new(bounds.Left, bounds.Top);

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        Color BrushColor =
            ((e.State & DrawItemState.Selected) is DrawItemState.Selected)
                ? new OxColorHelper(BackColor).Darker(2)
                : BackColor;

        e.Graphics.DrawRectangle(new Pen(BrushColor), e.Bounds);
        e.Graphics.FillRectangle(new SolidBrush(BrushColor), e.Bounds);

        if (DrawStrings && e.Index > -1)
        {
            T item = TItems[e.Index];
            string? itemString =
                item is bool boolItem
                ? boolItem
                    ? "Yes"
                    : "No"
                : item!.ToString();

            Point textStartPosition = GetTextStartPosition(e.Bounds);

            e.Graphics.DrawString(itemString,
                e.Font ?? OxStyles.Font(10),
                new SolidBrush(Color.Black),
                new Point(textStartPosition.X, textStartPosition.Y));

            if (DroppedDown && (e.State & DrawItemState.Selected) is DrawItemState.Selected)
            {
                OnGetToolTip(item, out string toolTipTitle, out string toolTipText);
                ToolTip.ToolTipTitle = toolTipTitle;

                if (!toolTipTitle.Equals(string.Empty) 
                    || !toolTipText.Equals(string.Empty))
                    ToolTip.Show(toolTipText, this, e.Bounds.Right, e.Bounds.Bottom);
                else
                    ToolTip.Hide(this);
            }
            else
                ToolTip.Hide(this);
        }
        else ToolTip.Hide(this);
    }

    protected override void OnDropDownStyleChanged(EventArgs e)
    {
        base.OnDropDownStyleChanged(e);
        FlatStyle = FlatStyle.System;
        AutoCompleteMode = DropDownStyle is ComboBoxStyle.DropDownList
            ? AutoCompleteMode.None
            : AutoCompleteMode.Suggest;
    }

    private int hoveredItemIndex = -1;
    private const int CB_GETCURSEL = 0x0147;
    public event EventHandler<HoverItemEventArgs<T>>? HoverItemChanged;

    protected virtual void OnHoverItem(HoverItemEventArgs<T> e) => 
        HoverItemChanged?.Invoke(this, e);

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        switch (m.Msg)
        {
            case CB_GETCURSEL:
                int currentHoveredItemIndex = m.Result.ToInt32();

                if (!hoveredItemIndex.Equals(currentHoveredItemIndex))
                {
                    hoveredItemIndex = currentHoveredItemIndex;
                    OnHoverItem(
                        new HoverItemEventArgs<T>(
                            hoveredItemIndex,
                            hoveredItemIndex < 0 ? default : TItems[hoveredItemIndex])
                    );
                }
                break;
            default:
                break;
        }
    }

    #region Implemention of IOxControl using IOxControlManager
    public virtual void OnAutoSizeChanged(OxBoolChangedEventArgs e) { }
    public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
    public virtual void OnEnabledChanged(OxBoolChangedEventArgs e) { }
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
    public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
    public virtual void OnVisibleChanged(OxBoolChangedEventArgs e) { }
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

    public new OxBool AutoSize
    {
        get => Manager.AutoSize;
        set => Manager.AutoSize = value;
    }

    public bool IsAutoSize =>
        Manager.IsAutoSize;

    public void SetAutoSize(bool value) =>
        Manager.SetAutoSize(value);


    public new virtual OxDock Dock
    {
        get => Manager.Dock;
        set => Manager.Dock = value;
    }

    public new OxBool Enabled
    {
        get => Manager.Enabled;
        set => Manager.Enabled = value;
    }

    public bool IsEnabled =>
        Manager.IsEnabled;

    public void SetEnabled(bool value) =>
        Manager.SetEnabled(value);

    public new OxBool Visible
    {
        get => Manager.Visible;
        set => Manager.Visible = value;
    }

    public bool IsVisible =>
        Manager.IsVisible;

    public void SetVisible(bool value) =>
        Manager.SetVisible(value);

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

public class OxComboBox : OxComboBox<object>
{ 
}