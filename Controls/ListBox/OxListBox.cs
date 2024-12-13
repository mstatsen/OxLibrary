using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls;

public partial class OxListBox : ListBox, IOxItemsContainer, IOxControlWithManager
{
    public IOxControlManager Manager { get; }
    public OxListBox()
    {
        Manager = OxControlManagers.RegisterControl(this);
        DoubleBuffered = true;
        DrawMode = DrawMode.OwnerDrawFixed;
        DrawItem += DrawItemHadler;
        ItemHeight = 28;
    }

    private IsHighPriorityItem? checkIsHighPriorityItem;
    private IsHighPriorityItem? checkIsMandatoryItem;

    public IsHighPriorityItem? CheckIsHighPriorityItem
    {
        get => checkIsHighPriorityItem;
        set => checkIsHighPriorityItem = value;
    }

    public IsHighPriorityItem? CheckIsMandatoryItem
    {
        get => checkIsMandatoryItem;
        set => checkIsMandatoryItem = value;
    }

    private bool IsHighPriorityItem(object item) =>
        CheckIsHighPriorityItem is not null
        && OxB.B(CheckIsHighPriorityItem.Invoke(item));

    private bool IsMandatoryItem(object item) =>
        CheckIsMandatoryItem is not null
        && OxB.B(CheckIsMandatoryItem.Invoke(item));

    private void DrawItemHadler(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        FontStyle fontStyle = FontStyle.Regular;

        if (IsHighPriorityItem(Items[e.Index]))
            fontStyle |= FontStyle.Underline;

        if (IsMandatoryItem(Items[e.Index]))
            fontStyle |= FontStyle.Bold;

        Font itemFont = new(e.Font ?? OxStyles.Font(11), fontStyle);

        if ((e.State & DrawItemState.Selected) is DrawItemState.Selected)
            e = new DrawItemEventArgs(
                e.Graphics,
                itemFont,
                e.Bounds,
                e.Index,
                e.State ^ DrawItemState.Selected,
                e.ForeColor,
                new OxColorHelper(BackColor).Darker(2));

        e.DrawBackground();
        Rectangle textBounds = new(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
        textBounds.X += 2;
        textBounds.Y +=
            (textBounds.Height -
            TextRenderer.MeasureText(Items[e.Index].ToString(), e.Font).Height) / 2;

        string? itemText = Items[e.Index].ToString();

        e.Graphics.DrawString(
            itemText,
            itemFont,
            IsEnabled ? Brushes.Black : Brushes.Silver,
            textBounds, 
            StringFormat.GenericDefault);
        e.DrawFocusRectangle();
    }

    private void MoveItem(OxUpDown direction)
    {
        if (SelectedItem is null 
            || SelectedIndex < 0)
            return; 

        int newIndex = SelectedIndex + OxUpDownHelper.Delta(direction);

        if (newIndex < 0 || newIndex >= Items.Count)
            return; 

        object selected = SelectedItem;

        Items.Remove(selected);
        Items.Insert(newIndex, selected);
        SetSelected(newIndex, true);
    }

    public void MoveUp() => MoveItem(OxUpDown.Up);
    public void MoveDown() => MoveItem(OxUpDown.Down);

    public void UpdateSelectedItem(object item) =>
        Items[SelectedIndex] = item;

    public int Count => Items.Count;

    public List<object> ObjectList
    {
        get
        {
            List<object> result = new();

            foreach (var item in Items)
                result.Add(item);

            return result;
        }
    }

    public OxBool AvailableMoveUp =>
        OxB.B(IsAvailableMoveUp);

    public bool IsAvailableMoveUp =>
        SelectedIndex > 0;

    public OxBool AvailableMoveDown =>
        OxB.B(IsAvailableMoveDown);

    public bool IsAvailableMoveDown =>
        (SelectedIndex > -1)
        && (SelectedIndex < Count - 1);

    public void RemoveAt(int index) =>
        Items.RemoveAt(index);

    public void RemoveCurrent() =>
        Items.RemoveAt(SelectedIndex);

    public void Add(object item) =>
        Items.Add(item);

    public void AddChild(object item) =>
        Items.Add(item);

    public void Clear() =>
        Items.Clear();

    public IOxControl AsControl() =>
        this;

    public OxBool AvailableChilds =>
        OxB.B(IsAvailableChilds);

    public bool IsAvailableChilds =>
        false;

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
    public void SetAutoSize(bool value) =>
        Manager.SetAutoSize(value);

    public bool IsAutoSize =>
        Manager.IsAutoSize;

    public new OxBool Enabled
    {
        get => Manager.Enabled;
        set => Manager.Enabled = value;
    }
    public void SetEnabled(bool value) =>
        Manager.SetEnabled(value);

    public bool IsEnabled =>
        Manager.IsEnabled;

    public new OxDock Dock
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

    public void WithSuspendedLayout(Action method) =>
        Manager.WithSuspendedLayout(method);

    event OxBoolChangedEvent IOxControlManager.AutoSizeChanged
    {
        add => Manager.AutoSizeChanged += value;
        remove => Manager.AutoSizeChanged -= value;
    }

    public new event OxDockChangedEvent DockChanged
    {
        add => Manager.DockChanged += value;
        remove => Manager.DockChanged -= value;
    }

    event OxBoolChangedEvent IOxControlManager.EnabledChanged
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

    event OxBoolChangedEvent IOxControlManager.VisibleChanged
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