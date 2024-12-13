using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls;

public class OxDataGridView : DataGridView, IOxControlWithManager
{
    public IOxControlManager Manager { get; }
    public OxDataGridView() =>
        Manager = OxControlManagers.RegisterControl(this);
    protected override bool DoubleBuffered { get => true; }

    public readonly Dictionary<DataGridViewColumn, SortOrder> ColumnSorting = new();

    public event DataGridViewCellMouseEventHandler? SortingChanged;

    protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
    {
        DataGridViewColumn column = Columns[e.ColumnIndex];

        if (column.SortMode is not DataGridViewColumnSortMode.Programmatic)
        {
            base.OnColumnHeaderMouseClick(e);
            return;
        }

        bool existColumnSorting = true;

        if (!ColumnSorting.TryGetValue(column, out var oldSortOrder))
        {
            oldSortOrder = SortOrder.None;
            existColumnSorting = false;
        }

        if (ModifierKeys is not Keys.Control)
        {
            ColumnSorting.Clear();

            foreach(DataGridViewColumn c in Columns)
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
        }

        SortOrder newSortOrder =
            oldSortOrder switch
            {
                SortOrder.Ascending => SortOrder.Descending,
                SortOrder.Descending => SortOrder.None,
                _ => SortOrder.Ascending,
            };

        if (newSortOrder is SortOrder.None 
            && existColumnSorting)
            ColumnSorting.Remove(column);
        else
        if (existColumnSorting)
            ColumnSorting[column] = newSortOrder;
        else
            ColumnSorting.Add(column, newSortOrder);

        Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = newSortOrder;
        base.OnColumnHeaderMouseClick(e);
        SortingChanged?.Invoke(this, e);
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