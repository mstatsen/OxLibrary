using OxLibrary.ControlsManaging;
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
        CheckIsHighPriorityItem is null
        || CheckIsHighPriorityItem.Invoke(item);

    private bool IsMandatoryItem(object item) =>
        CheckIsMandatoryItem is null
        || CheckIsMandatoryItem.Invoke(item);

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
            Enabled ? Brushes.Black : Brushes.Silver,
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

    public bool AvailableMoveUp => SelectedIndex > 0;

    public bool AvailableMoveDown => (SelectedIndex > -1)
            && (SelectedIndex < Count - 1);

    public void RemoveAt(int index) => Items.RemoveAt(index);
    public void RemoveCurrent() => Items.RemoveAt(SelectedIndex);

    public void Add(object item) => Items.Add(item);
    public void AddChild(object item) => Items.Add(item);
    public void Clear() => Items.Clear();

    public Control AsControl() => this;

    public bool AvailableChilds => false;

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