using OxLibrary.ControlsManaging;
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
    public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
    public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
    public new IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

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

    public void DoWithSuspendedLayout(Action method) =>
        Manager.DoWithSuspendedLayout(method);

    public Control GetChildAtPoint(OxPoint pt) =>
        Manager.GetChildAtPoint(pt);

    public void Invalidate(OxRectangle rc) =>
        Manager.Invalidate(rc);

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
#pragma warning disable IDE0051 // Remove unused private members
    private new void SetBounds(int x, int y, int width, int height) =>
        base.SetBounds(x, y, width, height);

    private new Size PreferredSize => base.PreferredSize;
    private new Rectangle DisplayRectangle => base.DisplayRectangle;
    private new Size GetPreferredSize(Size proposedSize) => base.GetPreferredSize(proposedSize);
    private new Size LogicalToDeviceUnits(Size value) => base.LogicalToDeviceUnits(value);
    private new void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) =>
        base.SetBounds(x, y, width, height, specified);
    private new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue) =>
        base.GetChildAtPoint(pt, skipValue);

#pragma warning disable IDE0060 // Remove unused parameter
    private new void Invalidate(Rectangle rc, bool invalidateChildren) => Invalidate(true);
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    #endregion
}